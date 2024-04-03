using System.Net;
using Newtonsoft.Json;
using PuppeteerSharp;
using Whatsapp.web.net.AuthenticationStrategies;
using Whatsapp.web.net.Domains;
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
    private readonly BaseAuthStrategy _authStrategy;
    private IBrowser _pupBrowser;

    private IPage _pupPage;
    public IPage PupPage
    {
        get => _pupPage;
        private set => _pupPage = value;
    }

    public ClientInfo ClientInfo { get; private set; }

    private readonly StreamWriter _streamWriter;

    public IContactManager Contact { get; private set; }
    public IChatManager Chat { get; private set; }
    public IGroupChatManager Group { get; private set; }
    public IMessageManager Message { get; private set; }
    public ICommerceManager Commerce { get; private set; }

    public Client(IEventDispatcher eventDispatcher, IRegisterEventService registerEventService, WhatsappOptions options)
    {
        _streamWriter = new StreamWriter("log.txt", true);
        _parserFunctions = JavaScriptParserFactory.Create("Whatsapp.web.net.scripts.functions.js");
        _parserInjected = JavaScriptParserFactory.Create("Whatsapp.web.net.scripts.injected.js");
        _eventDispatcher = eventDispatcher;
        _registerEventService = registerEventService;
        _options = options;
        _authStrategy = _options.AuthStrategy;

        TaskUtils.KillProcessesByName("chrome", options.Puppeteer.ExecutablePath);
        _authStrategy.Setup(this, options);
    }

    public async Task<Task> Initialize()
    {
        await InitializePage();

        await _authStrategy.AfterBrowserInitialized();
        await InitWebVersionCacheAsync();
        //TODO: missing
        //await PupPage.EvaluateExpressionOnNewDocumentAsync(_parserFunctions.GetMethod("modificarErrorStack"));

        await PupPage.GoToAsync(Constants.WhatsWebURL, new NavigationOptions
        {
            WaitUntil = [WaitUntilNavigation.Load],
            Timeout = 0,
            Referer = "https://whatsapp.com/"
        });

        await PupPage.AddScriptTagAsync(new AddTagOptions()
        {
            Url = "https://unpkg.com/moduleraid/dist/moduleraid.iife.js"
        });




        await PupPage.AddScriptTagAsync(new AddTagOptions()
        {
            Content = "import ModuleRaid from 'moduleRaid';window.mR = new moduleRaid();",
            Type = "module"
        });
        await PupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getElementByXpath"));

        var lastPercent = default(int?);
        var lastPercentMessage = default(string);

        await PupPage.EvaluateFunctionAsync(_parserInjected.GetMethod("registerLoadingScreen"));

        await PupPage.ExposeFunctionAsync<int, string, bool>("onLoadingScreen", (percent, message) =>
        {
            if (lastPercent == percent && lastPercentMessage == message) return true;

            _eventDispatcher.EmitLoadingScreen(percent, message);
            lastPercent = percent;
            lastPercentMessage = message;

            return true;
        });

        await PupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("observeProgress"), new { PROGRESS, PROGRESS_MESSAGE });

        var continueTask = await AuthenticationIfNeed();
        if (continueTask != Task.CompletedTask)
        {
            return continueTask;
        }

        // TODO: missing implementation
        // this.interface = new InterfaceController(this);


        _registerEventService.Register(PupPage);
        Thread.Sleep(5 * 1000);
        await PupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("registerEventListeners"));
        CreateManagers();
        return Task.CompletedTask;
    }

    private void CreateManagers()
    {
        Chat = new ChatManager(_parserFunctions, PupPage);
        Message = new MessageManager(_parserFunctions, PupPage);
        Contact = new ContactManager(_parserFunctions, PupPage);
        Group = new GroupChatManager(_parserFunctions, PupPage);
        Commerce = new CommerceManager(_parserFunctions, PupPage);
    }


    /// <summary>
    /// Asynchronously handles authentication if needed and retry.
    /// </summary>
    /// <returns></returns>
    private async Task<Task> AuthenticationIfNeed()
    {
        // Wait for either selector to appear first
        var imgSelectorTask = PupPage.WaitForSelectorAsync(INTRO_IMG_SELECTOR, new WaitForSelectorOptions { Timeout = _options.AuthTimeoutMs });
        var qrSelectorTask = PupPage.WaitForSelectorAsync(INTRO_QRCODE_SELECTOR, new WaitForSelectorOptions { Timeout = _options.AuthTimeoutMs });
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
            await PupPage.ExposeFunctionAsync("qrChanged", (string qr) =>
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
            await PupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("startQRCodeObserver"),
                JsonConvert.SerializeObject(new { QR_CONTAINER, QR_RETRY_BUTTON }));

            try
            {
                await PupPage.WaitForSelectorAsync(INTRO_IMG_SELECTOR, new WaitForSelectorOptions { Timeout = 0 });
            }
            catch (Exception error)
            {
                if (!error.Message.Contains("Target closed")) throw;
                // Something has called .Destroy() while waiting
                return Task.CompletedTask;

            }
        }

        var version = PupPage.EvaluateFunctionHandleAsync(_parserFunctions.GetMethod("getWWebVersion")).Result.ToString();
        var isCometOrAbove = int.Parse(version.Split('.')[1]) >= 3000;
        if (isCometOrAbove)
        {
            await PupPage.EvaluateFunctionAsync(_parserInjected.GetMethod("exposeStore"));
        }
        else
        {
            await PupPage.EvaluateFunctionAsync(_parserInjected.GetMethod("registerModuleRaid"));
            await PupPage.EvaluateFunctionAsync(_parserInjected.GetMethod("exposeStore2_3"));
        }

        // Evaluate ExposeStore 
        


        // Wait for window.Store to be defined
        await PupPage.WaitForFunctionAsync("() => window.Store != undefined");

        // Unregister service workers
        await PupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("unregisterServiceWorkers"));

        // Load utility functions
        await PupPage.EvaluateFunctionAsync(_parserInjected.GetMethod("loadUtils"));

        // Expose client info

        var clientInfo = await PupPage.EvaluateFunctionAsync<object>(_parserFunctions.GetMethod("serializeConnectionAndUser"));

        ClientInfo = new ClientInfo(clientInfo);

        // Get authentication event payload
        var authEventPayload = await _authStrategy.GetAuthEventPayload();

        // Emit authenticated event
        _eventDispatcher.EmitAuthenticated(ClientInfo, authEventPayload);

        return Task.CompletedTask;
    }

    private async Task InitializePage()
    {
        IBrowser browser;
        IPage page;

        await _authStrategy.BeforeBrowserInitialized();

        if (_options.Puppeteer is { BrowserWSEndpoint: not null })
        {
            browser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                DefaultViewport = _options.Puppeteer.DefaultViewport
            });
            page = await browser.NewPageAsync();
        }
        else
        {
            var browserArgs = new List<string>(_options.Puppeteer.Args ?? Array.Empty<string>());

            // navigator.webdriver fix
            browserArgs.Add($"--disable-blink-features=AutomationControlled");


            var launchOptions = new LaunchOptions
            {
                Args = browserArgs.ToArray(),
                Headless = _options.Puppeteer.Headless,
                UserDataDir = _options.Puppeteer.UserDataDir,
                DefaultViewport = _options.Puppeteer.DefaultViewport,
                ExecutablePath = _options.Puppeteer.ExecutablePath
            };

            browser = await Puppeteer.LaunchAsync(launchOptions);
            page = (await browser.PagesAsync())[0];
        }

        if (_options.ProxyAuthentication is not null)
        {
            await page.AuthenticateAsync(_options.ProxyAuthentication);
        }

        await page.SetUserAgentAsync(_options.UserAgent);
        if (_options.BypassCSP)
        {
            await page.SetBypassCSPAsync(true);
        }

        page.Console += ConsoleWrite;
        page.PageError += PageError;
        page.Error += PupPageError;

        _pupBrowser = browser;
        PupPage = page;


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
        var webCacheOptions = _options.WebVersionCache;
        var webCache = WebCacheFactory.CreateWebCache(webCacheOptions.Type, webCacheOptions);

        var requestedVersion = _options.WebVersion;
        var versionContent = await webCache.Resolve(requestedVersion);

        if (versionContent != null)
        {
            await PupPage.SetRequestInterceptionAsync(true);
            PupPage.Request += async (sender, e) =>
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
        else
        {
            PupPage.Response += async (sender, e) =>
            {
                if (e.Response.Ok && e.Response.Url == Constants.WhatsWebURL)
                {
                    await webCache.Persist(await e.Response.TextAsync());
                }
            };
        }
    }

    public async Task<object> GetBatteryStatus()
    {
        return await PupPage.EvaluateExpressionAsync(_parserFunctions.GetMethod("getBatteryStatus"));
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
        PupPage.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _pupBrowser.DisposeAsync();
        await _streamWriter.DisposeAsync();
        await PupPage.DisposeAsync();
    }

    public async Task Reject(string peerJid, string callId)
    {
        await PupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("rejectCall"), peerJid, callId);
    }
}