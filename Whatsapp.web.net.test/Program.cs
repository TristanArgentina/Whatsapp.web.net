using ChatbotAI.net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Whatsapp.web.net;
using Whatsapp.web.net.EventArgs;
using Whatsapp.web.net.test;

Console.WriteLine("Hello, World!");

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(@"appsettings.json", optional: false, reloadOnChange: true);

builder.Services
    .AddLogging(loggingBuilder => loggingBuilder.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Information));

builder.Services.AddOptions<WhatsappOptions>()
    .BindConfiguration("Whatsapp")
    .ValidateOnStart();

builder.Services.AddOptions<OpenAIOptions>()
    .BindConfiguration("OpenAI")
    .ValidateOnStart();

await using var serviceProvider = builder.Services.BuildServiceProvider();

var whatsappOptions = serviceProvider.GetRequiredService<IOptions<WhatsappOptions>>().Value;
var openAIOptions = serviceProvider.GetRequiredService<IOptions<OpenAIOptions>>().Value;



var parserFunctions = new JavaScriptParser(@".\scripts\functions.js");
var parserInjected = new JavaScriptParser(@".\scripts\injected.js");
var eventDispatcher = new EventDispatcher();
var registerEventService = new RegisterEventService(eventDispatcher, parserFunctions, whatsappOptions);
var client = new Client(parserFunctions, parserInjected, eventDispatcher, registerEventService, whatsappOptions);

await client.Initialize().Result;

var handleEvents = new HandleEvents(client, eventDispatcher, openAIOptions);

handleEvents.SetHandle();
eventDispatcher.EmitReady();

//var chat = client.Chat.Get("120363248319028492@g.us").Result;
Console.ReadLine();

client.Dispose();