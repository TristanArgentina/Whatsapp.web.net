using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PuppeteerSharp;
using Whatsapp.web.net.Authentication;
using Whatsapp.web.net.Domains;
using Whatsapp.web.net.Managers;
using Whatsapp.web.net.scripts;
using ErrorEventArgs = PuppeteerSharp.ErrorEventArgs;

namespace Whatsapp.web.net;

public class Client : IDisposable, IAsyncDisposable
{

    const string PROGRESS = "//*[@id='app']/div/div/div[2]/progress";
    const string PROGRESS_MESSAGE = "//*[@id='app']/div/div/div[3]";

    // Define selectors
    const string INTRO_IMG_SELECTOR = "[data-icon='search']";
    const string INTRO_QRCODE_SELECTOR = "div[data-ref] canvas";

    const string QR_CONTAINER = "div[data-ref]";
    const string QR_RETRY_BUTTON = "div[data-ref] > span > button";


    private readonly IJavaScriptParser _parserInjected;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IRegisterEventService _registerEventService;
    private readonly IJavaScriptParser _parserFunctions;
    private readonly WhatsappOptions _options;
    private readonly IAuthenticator _authStrategy;
    
    private IBrowser _pupBrowser;
    private IPage _pupPage;

    public ClientInfo ClientInfo { get; private set; }

    private readonly StreamWriter _streamWriter;

    public IContactManager Contact { get; private set; }
    public IChatManager Chat { get; private set; }
    public IGroupChatManager Group { get; private set; }
    public IMessageManager Message { get; private set; }
    public ICommerceManager Commerce { get; private set; }

    public Client(IEventDispatcher eventDispatcher, IRegisterEventService registerEventService, 
        IOptions<WhatsappOptions> options, IAuthenticatorProvider authenticatorProvider)
    {
        _streamWriter = new StreamWriter("log.txt", true);
        _parserFunctions = JavaScriptParserFactory.Create("Whatsapp.web.net.scripts.functions.js");
        _parserInjected = JavaScriptParserFactory.Create("Whatsapp.web.net.scripts.injected.js");
        _eventDispatcher = eventDispatcher;
        _registerEventService = registerEventService;
        _options = options.Value;
        _authStrategy = authenticatorProvider.GetAuthenticator();
    }

    public async Task<Task> Initialize()
    {
        await InitializePage();

        await _authStrategy.AfterBrowserInitialized();
        await InitWebVersionCacheAsync();
        //TODO: missing
        //await PupPage.EvaluateExpressionOnNewDocumentAsync(_parserFunctions.GetMethod("modificarErrorStack"));

        await _pupPage.GoToAsync(Constants.WhatsWebURL, new NavigationOptions
        {
            WaitUntil = [WaitUntilNavigation.DOMContentLoaded],
            Timeout = 0,
            Referer = "https://whatsapp.com/"
        });

        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getElementByXpath"));

        var lastPercent = default(int?);
        var lastPercentMessage = default(string);

        await _pupPage.EvaluateFunctionAsync(_parserInjected.GetMethod("registerLoadingScreen"));

        await _pupPage.ExposeFunctionAsync<int, string, bool>("onLoadingScreen", (percent, message) =>
        {
            if (lastPercent == percent && lastPercentMessage == message) return true;

            _eventDispatcher.EmitLoadingScreen(percent, message);
            lastPercent = percent;
            lastPercentMessage = message;

            return true;
        });

        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("observeProgress"), new { PROGRESS, PROGRESS_MESSAGE });

        var continueTask = await AuthenticationIfNeed();
        if (continueTask != Task.CompletedTask)
        {
            return continueTask;
        }

        // TODO: missing implementation
        // this.interface = new InterfaceController(this);


        _registerEventService.Register(_pupPage);
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("registerEventListeners"));
        CreateManagers();
        return Task.CompletedTask;
    }

    private void CreateManagers()
    {
        Chat = new ChatManager(_parserFunctions, _pupPage);
        Message = new MessageManager(_parserFunctions, _pupPage);
        Contact = new ContactManager(_parserFunctions, _pupPage);
        Group = new GroupChatManager(_parserFunctions, _pupPage);
        Commerce = new CommerceManager(_parserFunctions, _pupPage);
    }


    /// <summary>
    /// Asynchronously handles authentication if needed and retry.
    /// </summary>
    /// <returns></returns>
    private async Task<Task> AuthenticationIfNeed()
    {
        // Wait for either selector to appear first
        var imgSelectorTask = _pupPage.WaitForSelectorAsync(INTRO_IMG_SELECTOR, new WaitForSelectorOptions { Timeout = _options.AuthTimeoutMs });
        var qrSelectorTask = _pupPage.WaitForSelectorAsync(INTRO_QRCODE_SELECTOR, new WaitForSelectorOptions { Timeout = _options.AuthTimeoutMs });
        var needAuthentication = await Task.WhenAny(imgSelectorTask, qrSelectorTask);

        needAuthentication.Wait();

        // Check if an error occurred on the first found selector
        if (needAuthentication.IsFaulted)
        {
            // Scan-qrcode selector was found. Needs authentication
            var result = await _authStrategy.OnAuthenticationNeeded();
            if (result.Failed)
            {
                // Handle authentication failure
                // Emits authentication failure event
                _eventDispatcher.EmitAuthenticationFailure(result.FailureEventPayload);

                await Destroy();
                if (!result.Restart) return needAuthentication;

                // Session restore failed so try again without session to force new authentication
                return Initialize();
            }
        }

        // if (needAuthentication.Result.RemoteObject.ClassName == "HTMLCanvasElement") return Task.CompletedTask;
        if (needAuthentication == qrSelectorTask)
        {

            var qrRetries = 0;
            var continueObserving = true;

            // Expose qrChanged function to the page
            await _pupPage.ExposeFunctionAsync("qrChanged", (string qr) =>
            {
                // Emits QR received event
                _eventDispatcher.EmitQRReceived(qr);
                if (_options.QrMaxRetries > 0)
                {
                    qrRetries++;
                    if (qrRetries > _options.QrMaxRetries)
                    {
                        // Emits disconnected event
                        _eventDispatcher.EmitDisconnected("Max qrcode retries reached");
                        continueObserving = false;
                    }
                }

                return continueObserving;
            });


            // Observe changes in QR container
            await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("startQRCodeObserver"),
                JsonConvert.SerializeObject(new { QR_CONTAINER, QR_RETRY_BUTTON }));

            try
            {
                await _pupPage.WaitForSelectorAsync(INTRO_IMG_SELECTOR, new WaitForSelectorOptions { Timeout = 0 });
            }
            catch (Exception error)
            {
                if (!error.Message.Contains("Target closed")) throw;
                // Something has called .Destroy() while waiting
                return Task.CompletedTask;

            }
        }


        var jsHandle = _pupPage.EvaluateFunctionHandleAsync(_parserFunctions.GetMethod("getWWebVersion")).Result;
        var version = jsHandle.JsonValueAsync<string>().Result;

        if (_options.WebVersionCache.Type == "local" && !string.IsNullOrEmpty(_currentIndexHtml))
        {
            await _authStrategy.LoginWebCache.Persist(_currentIndexHtml, version);
        }

        

        var isCometOrAbove = int.Parse(version.Split('.')[1]) >= 3000;
        if (isCometOrAbove)
        {
            await _pupPage.AddScriptTagAsync(new AddTagOptions()
            {
                Content = "import ModuleRaid from 'moduleRaid';window.mR = new moduleRaid();",
                Type = "module"
            });
            await _pupPage.EvaluateFunctionAsync(_parserInjected.GetMethod("exposeStore"));
        }
        else
        {
            await _pupPage.EvaluateFunctionAsync(_parserInjected.GetMethod("registerModuleRaid"));
            await _pupPage.EvaluateFunctionAsync(_parserInjected.GetMethod("exposeStore2_3"));
        }

        // Wait for window.Store to be defined
        await _pupPage.WaitForFunctionAsync("() => window.Store != undefined");

        // Unregister service workers
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("unregisterServiceWorkers"));

        // Load utility functions
        await _pupPage.EvaluateFunctionAsync(_parserInjected.GetMethod("loadUtils"));
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("compareWwebVersions"));


        // Expose client info
        var clientInfo = await _pupPage.EvaluateFunctionAsync<object>(_parserFunctions.GetMethod("serializeConnectionAndUser"));

        ClientInfo = new ClientInfo(clientInfo);

        // Get authentication event payload
        var authEventPayload = await _authStrategy.GetAuthEventPayload();

        // Emit authenticated event
        _eventDispatcher.EmitAuthenticated(ClientInfo, authEventPayload);

        return Task.CompletedTask;
    }

    private async Task InitializePage()
    {
        //IBrowser browser;
        //IPage PupPage;

        await _authStrategy.BeforeBrowserInitialized();

        if (_options.Puppeteer is { BrowserWSEndpoint: not null })
        {
            _pupBrowser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                DefaultViewport = _options.Puppeteer.DefaultViewport
            });
            _pupPage = await _pupBrowser.NewPageAsync();
        }
        else
        {
            var browserArgs = new List<string>(_options.Puppeteer.Args ?? Array.Empty<string>()) {
                // navigator.webdriver fix
                $"--disable-blink-features=AutomationControlled"};


            var launchOptions = new LaunchOptions
            {
                Args = browserArgs.ToArray(),
                Headless = _options.Puppeteer.Headless,
                UserDataDir = _authStrategy.UserDataDir,
                DefaultViewport = _options.Puppeteer.DefaultViewport,
                ExecutablePath = _options.Puppeteer.ExecutablePath
            };

            _pupBrowser = await Puppeteer.LaunchAsync(launchOptions);
            _pupPage = (await _pupBrowser.PagesAsync())[0];
        }

        if (_options.ProxyAuthentication is not null)
        {
            await _pupPage.AuthenticateAsync(_options.ProxyAuthentication);
        }

        await _pupPage.SetUserAgentAsync(_options.UserAgent);
        if (_options.BypassCSP)
        {
            await _pupPage.SetBypassCSPAsync(true);
        }

        _pupPage.Console += ConsoleWrite;
        _pupPage.PageError += PageError;
        _pupPage.Error += PupPageError;

        //_pupBrowser = browser;
        //PupPage = PupPage;


    }

    private void PupPageError(object? sender, ErrorEventArgs e)
    {
        Console.WriteLine($"PupPageError: {e.Error}");
    }

    private void PageError(object? sender, PageErrorEventArgs e)
    {
        Console.WriteLine($"PageError: {e.Message}");
    }

    private readonly List<string> mensajes = [];
    private string _currentIndexHtml;

    private void ConsoleWrite(object? sender, ConsoleEventArgs e)
    {
        var value = e.Message.Text;
        if (!mensajes.Contains(value))
        {
            //mensajes.Add(value);
            Console.WriteLine(value);
            _streamWriter.WriteLine(value);
        }

    }

    public async Task InitWebVersionCacheAsync()
    {
        var requestedVersion = _options.WebVersion;
        var versionContent = await _authStrategy.LoginWebCache.Resolve(requestedVersion);

        if (versionContent != null)
        {
            await _pupPage.SetRequestInterceptionAsync(true);
            _pupPage.Request += async (sender, e) =>
            {
                if (e.Request.Url == Constants.WhatsWebURL)
                {
                    await e.Request.RespondAsync(new ResponseData
                    {
                        Status = HttpStatusCode.OK,
                        ContentType = "text/html",
                        Body = versionContent
                    });
                }
                else
                {
                    await e.Request.ContinueAsync();
                }
            };
        }
        if (_options.WebVersionCache.Type == "local")
        {
            _pupPage.Response += async (sender, e) =>
            {
                if (e.Response.Ok && e.Response.Url == Constants.WhatsWebURL)
                {
                    var textAsync = await e.Response.TextAsync();
                    _currentIndexHtml = textAsync;
                }
            };
        }
    }

    public async Task<object> GetBatteryStatus()
    {
        return await _pupPage.EvaluateExpressionAsync(_parserFunctions.GetMethod("getBatteryStatus"));
    }

    private async Task Destroy()
    {
        await _pupBrowser.CloseAsync();
        await _authStrategy.Destroy();
    }

    public void Dispose()
    {
        _pupBrowser.Dispose();
        _streamWriter.Dispose();
        _pupPage.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _pupBrowser.DisposeAsync();
        await _streamWriter.DisposeAsync();
        await _pupPage.DisposeAsync();
    }

    public async Task Reject(string peerJid, string callId)
    {
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("rejectCall"), peerJid, callId);
    }
}
