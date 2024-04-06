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

public class Client(
    IEventDispatcher eventDispatcher,
    IRegisterEventService registerEventService,
    IOptions<WhatsappOptions> options,
    IAuthenticatorProvider authenticatorProvider)
    : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Raised when the page crashes
    /// </summary>
    public event EventHandler<ErrorEventArgs>? PageCrashError;

    /// <summary>
    /// Raised when an uncaught exception happens within the page.
    /// </summary>
    public event EventHandler<PageErrorEventArgs>? PageError;

    /// <summary>
    /// Raised when JavaScript within the page calls one of console API methods, e.g. <c>console.log</c> or <c>console.dir</c>. Also emitted if the page throws an error or a warning.
    /// The arguments passed into <c>console.log</c> appear as arguments on the event handler.
    /// </summary>
    /// <example>
    /// An example of handling <see cref="Console"/> event:
    /// <code>
    /// <![CDATA[
    /// page.Console += (sender, e) =>
    /// {
    ///     for (var i = 0; i < e.Message.Args.Count; ++i)
    ///     {
    ///         System.Console.WriteLine($"{i}: {e.Message.Args[i]}");
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public event EventHandler<ConsoleEventArgs>? Console;

    const string PROGRESS = "//*[@id='app']/div/div/div[2]/progress";
    const string PROGRESS_MESSAGE = "//*[@id='app']/div/div/div[3]";

    // Define selectors
    const string INTRO_IMG_SELECTOR = "[data-icon='search']";
    const string INTRO_QRCODE_SELECTOR = "div[data-ref] canvas";

    const string QR_CONTAINER = "div[data-ref]";
    const string QR_RETRY_BUTTON = "div[data-ref] > span > button";


    private readonly IJavaScriptParser _parserInjected = JavaScriptParserFactory.Create("Whatsapp.web.net.scripts.injected.js");
    private readonly IJavaScriptParser _parserFunctions = JavaScriptParserFactory.Create("Whatsapp.web.net.scripts.functions.js");
    private readonly WhatsappOptions _options = options.Value;
    private readonly IAuthenticator _authStrategy = authenticatorProvider.GetAuthenticator();

    private IBrowser? _pupBrowser;
    private IPage? _pupPage;

    public ClientInfo? ClientInfo { get; private set; }

    public IContactManager? Contact { get; private set; }

    public IChatManager? Chat { get; private set; }

    public IGroupChatManager? Group { get; private set; }

    public IMessageManager? Message { get; private set; }

    public ICommerceManager? Commerce { get; private set; }

    public async Task<Task> Initialize()
    {
        var result = await InitializePage();
        _pupBrowser = result.PupBrowser;
        _pupPage = result.PupPage;

        if (_pupPage is null) throw new Exception("The page did not initialize");
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

            eventDispatcher.EmitLoadingScreen(percent, message);
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


        registerEventService.Register(_pupPage);
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("registerEventListeners"));
        CreateManagers();
        return Task.CompletedTask;
    }

    private void CreateManagers()
    {
        if (_pupPage is null) throw new Exception("The page did not initialize");
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
        if (_pupPage is null) throw new Exception("The page did not initialize");
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
                eventDispatcher.EmitAuthenticationFailure(result.FailureEventPayload);

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
                eventDispatcher.EmitQRReceived(qr);
                if (_options.QrMaxRetries > 0)
                {
                    qrRetries++;
                    if (qrRetries > _options.QrMaxRetries)
                    {
                        // Emits disconnected event
                        eventDispatcher.EmitDisconnected("Max qrcode retries reached");
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
        eventDispatcher.EmitAuthenticated(ClientInfo, authEventPayload);

        return Task.CompletedTask;
    }

    private async Task<(IBrowser PupBrowser, IPage PupPage)> InitializePage()
    {
        IBrowser? pupBrowser;
        IPage? pupPage;

        await _authStrategy.BeforeBrowserInitialized();

        if (_options.Puppeteer is { BrowserWSEndpoint: not null })
        {
            pupBrowser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                DefaultViewport = _options.Puppeteer.DefaultViewport
            });
            pupPage = await pupBrowser.NewPageAsync();
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

            pupBrowser = await Puppeteer.LaunchAsync(launchOptions);
            pupPage = (await pupBrowser.PagesAsync())[0];
        }

        if (_options.ProxyAuthentication is not null)
        {
            await pupPage.AuthenticateAsync(_options.ProxyAuthentication);
        }

        await pupPage.SetUserAgentAsync(_options.UserAgent);
        if (_options.BypassCSP)
        {
            await pupPage.SetBypassCSPAsync(true);
        }

        pupPage.Console += Console;
        pupPage.PageError += PageError;
        pupPage.Error += PageCrashError;

        return (pupBrowser, pupPage);
    }

    private string? _currentIndexHtml;

    public async Task InitWebVersionCacheAsync()
    {
        if (_pupPage is null) throw new Exception("The page did not initialize");

        var requestedVersion = _options.WebVersion;
        var versionContent = await _authStrategy.LoginWebCache.Resolve(requestedVersion);

        if (versionContent != null)
        {
            await _pupPage.SetRequestInterceptionAsync(true);
            _pupPage.Request += async (_, e) =>
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
            _pupPage.Response += async (_, e) =>
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
        if (_pupPage is null) throw new Exception("The page did not initialize");
        return await _pupPage.EvaluateExpressionAsync(_parserFunctions.GetMethod("getBatteryStatus"));
    }

    private async Task Destroy()
    {
        if (_pupBrowser is not null)
        {
            await _pupBrowser.CloseAsync();
        }
        await _authStrategy.Destroy();
    }

    public void Dispose()
    {
        if (_pupBrowser is not null)
        {
            _pupBrowser.Dispose();
        }
        if (_pupPage is not null)
        {
            _pupPage.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_pupBrowser is not null)
        {
            await _pupBrowser.DisposeAsync();
        }
        if (_pupPage is not null)
        {
            await _pupPage.DisposeAsync();
        }
    }

    public async Task Reject(string peerJid, string callId)
    {
        if (_pupPage is not null)
        {
            await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("rejectCall"), peerJid, callId);
        }
    }
}
