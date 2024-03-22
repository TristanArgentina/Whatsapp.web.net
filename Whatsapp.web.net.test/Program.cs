using Whatsapp.web.net;
using Whatsapp.web.net.AuthenticationStrategies;
using Whatsapp.web.net.EventArgs;
using Whatsapp.web.net.test;

Console.WriteLine("Hello, World!");

var options = new WhatsappOptions
{
    UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.67 Safari/537.36",
    AuthStrategy = new LocalAuth(2.ToString()),
    FfmpegPath = @"C:\ffmpeg",
    Puppeteer = new PuppeteerOptions
    {
        ExecutablePath = @"C:\chromium-browser\chrome.exe"
    },
    WebVersionCache = new WebVersionCache
    {
        Type = "local",
        LocalPath = "./.wwebjs_cache/"
    }
};

var parserFunctions = new JavaScriptParser(@".\scripts\functions.js");
var parserInjected = new JavaScriptParser(@".\scripts\injected.js");
var eventDispatcher = new EventDispatcher();
var registerEventService = new RegisterEventService(eventDispatcher, parserFunctions, options);
var client = new Client(parserFunctions, parserInjected, eventDispatcher, registerEventService, options);

var handleEvents = new HandleEvents(client, eventDispatcher);
await client.Initialize();
handleEvents.SetHandle();

Console.ReadLine();

client.Dispose();