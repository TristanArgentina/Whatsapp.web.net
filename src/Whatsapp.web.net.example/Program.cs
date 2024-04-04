using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Whatsapp.web.net;
using Whatsapp.web.net.Authentication;
using Whatsapp.web.net.EventArgs;
using Whatsapp.web.net.example;

Console.WriteLine("Hello, World!");

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(@"appsettings.json", optional: false, reloadOnChange: true);

builder.Services
    .AddLogging(loggingBuilder => loggingBuilder.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Information));

builder.Services.AddOptions<WhatsappOptions>()
    .BindConfiguration("Whatsapp")
    .ValidateOnStart();

builder.Services.AddSingleton(provider => provider.GetRequiredService<IOptions<WhatsappOptions>>().Value.Puppeteer);
builder.Services.AddSingleton(provider => provider.GetRequiredService<IOptions<WhatsappOptions>>().Value.WebVersionCache);

builder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();
builder.Services.AddSingleton<IRegisterEventService, RegisterEventService>();
builder.Services.AddSingleton<IAuthenticatorProvider, AuthenticatorProvider>();
builder.Services.AddSingleton<Client>();
builder.Services.AddSingleton<HandleEvents>();

await using var serviceProvider = builder.Services.BuildServiceProvider();

var eventDispatcher = serviceProvider.GetService<IEventDispatcher>();
var client = serviceProvider.GetService<Client>();
var handleEvents = serviceProvider.GetService<HandleEvents>();

//TaskUtils.KillProcessesByName("chrome", whatsappOptions.Puppeteer.ExecutablePath);


await client.Initialize();


handleEvents.SetHandle();
eventDispatcher.EmitReady();

Console.ReadLine();

client.Dispose();