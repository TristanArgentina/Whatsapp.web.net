using ChatbotAI.net;
using Whatsapp.web.net;
using Whatsapp.web.net.AuthenticationStrategies;
using Whatsapp.web.net.EventArgs;
using Whatsapp.web.net.test;

Console.WriteLine("Hello, World!");
var config = Utils.BuildConfig([]);

var options = new WhatsappOptions
{
    UserAgent = config["AppSettings:"],
    AuthStrategy = new LocalAuth(config["AppSettings:AuthStrategy:ClientId"]),
    FfmpegPath = config["AppSettings:FfmpegPath"],
    Puppeteer = new PuppeteerOptions
    {
        //        ExecutablePath = @"C:\chromium-browser\chrome.exe"
        ExecutablePath = config["AppSettings:Puppeteer:ExecutablePath"],
        Headless = Convert.ToBoolean(config["AppSettings:Puppeteer:Headless"])
    },
    WebVersionCache = new WebVersionCache
    {
        Type = config["AppSettings:WebVersionCache:Type"],
        LocalPath = config["AppSettings:WebVersionCache:LocalPath"]
    },
    WebVersion = "2.2412.50"
};

var parserFunctions = new JavaScriptParser(@".\scripts\functions.js");
var parserInjected = new JavaScriptParser(@".\scripts\injected.js");
var eventDispatcher = new EventDispatcher();
var registerEventService = new RegisterEventService(eventDispatcher, parserFunctions, options);
var client = new Client(parserFunctions, parserInjected, eventDispatcher, registerEventService, options);
var ai = new AI("gpt-3.5-turbo", config["AppSettings:OpenAI:ApiKey"]);
await client.Initialize().Result;
var handleEvents = new HandleEvents(client, eventDispatcher, ai);
handleEvents.SetHandle();
eventDispatcher.EmitReady();

//var chat = client.Chat.Get("120363248319028492@g.us").Result;
Console.ReadLine();

client.Dispose();