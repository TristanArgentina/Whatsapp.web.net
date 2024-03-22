using System.Drawing;
using ConsoleApp2;
using Whatsapp.web.net;
using Whatsapp.web.net.AuthenticationStrategies;
using Whatsapp.web.net.EventArgs;
using Whatsapp.web.net.Extensions;

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

var qr = new GenerateQR();
var client = new Client(parserFunctions, parserInjected, eventDispatcher, registerEventService, options);

EventHandler<DispatcherEventArg> emit = (sender, eventArg) =>
{
    Console.Write($"{eventArg.DispatcherEventsType}: ");
    switch (eventArg.DispatcherEventsType)
    {
        case DispatcherEventsType.AUTHENTICATED:
            {
                var args = (AuthenticatedEventArg)eventArg;
                Console.WriteLine($"User: {args.Info}");
                break;
            }
        case DispatcherEventsType.QR_RECEIVED:
            try
            {
                Console.WriteLine($"Evento: {eventArg.DispatcherEventsType}");
                var args = (QRReceivedEventArgs)eventArg;
                var ms = qr.Generate(args.Qr.ToString());
                var sw = Image.FromStream(ms.Result);
                sw.Save("testQR.png");
            }
            catch (Exception e)
            {

            }

            break;
        case DispatcherEventsType.READY:
            Console.WriteLine($"Evento: {eventArg.DispatcherEventsType}");
            break;
        case DispatcherEventsType.MESSAGE_RECEIVED:
            {
                var args = (MessageReceivedEventArgs)eventArg;
                Console.WriteLine($"{args.Message.From.Id} : {args.Message.Body}");
                if (args.Message.Body == "Ping!")
                {
                    args.Message.Reply(client, "Pong!");
                }
                break;
            }
        case DispatcherEventsType.MESSAGE_CREATE:
            {
                var args = (MessageCreateEventArgs)eventArg;
                Console.WriteLine($"{args.Message.From.Id} : {args.Message.Body}");
                break;
            }
        case DispatcherEventsType.UNREAD_COUNT:
            {
                var args = (UnreadCountEventArg)eventArg;
                Console.WriteLine(args.Chat.ToString());
                break;
            }
        case DispatcherEventsType.MESSAGE_ACK:
            {
                var args = (MessageACKEventArg)eventArg;
                Console.WriteLine($"ASK: {args.MessageAsk}");
                break;
            }
        case DispatcherEventsType.CONTACT_CHANGED:
            {
                var args = (ContactChangedEventArg)eventArg;
                Console.WriteLine($" {args.OldId} to {args.NewId}");
                break;
            }
        default:
            Console.WriteLine("");
            break;
    }
};

eventDispatcher.DispatchEventGeneric += emit;

await client.Initialize();

Console.ReadLine();

client.Dispose();