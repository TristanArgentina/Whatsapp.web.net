using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Whatsapp.web.net.EventArgs;
using Whatsapp.web.net.LoginWebCache;

namespace Whatsapp.web.net;

public class Bootstrapper
{
    private readonly string _fileNameConfig;
    
    /// <summary>
    /// This is client
    /// </summary>
    public Client? Client { get; set; }

    /// <summary>
    /// This is event dispatcher
    /// </summary>
    public IEventDispatcher? EventDispatcher { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// 
    /// </summary>
    public ConfigurationManager ConfigurationManager { get; }

    /// <summary>
    /// 
    /// </summary>
    public ServiceProvider ServiceProvider { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileNameConfig"></param>
    /// <param name="services"></param>
    /// <param name="configurationManager"></param>
    public Bootstrapper(string fileNameConfig = "appsettings.json",
        IServiceCollection? services = null,
        ConfigurationManager? configurationManager = null
        )
    {
        _fileNameConfig = fileNameConfig;
        
        if (services is null)
        {
            var builder = Host.CreateApplicationBuilder();
            services = builder.Services;
            configurationManager = builder.Configuration;
        }


        Services = services;
        ConfigurationManager = configurationManager ?? throw new ArgumentException("configurationManager cannot be null.");

        Register();
    }

    public Client Start()
    {
        Client = ServiceProvider.GetService<Client>();
        Client!.Initialize().Wait();
        EventDispatcher!.EmitReady();
        return Client;
    }

    private void Register()
    {
        ConfigurationManager.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(_fileNameConfig, optional: false, reloadOnChange: true);

        Services
            .AddLogging(loggingBuilder => loggingBuilder.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Information));

        Services.AddOptions<WhatsappOptions>()
            .BindConfiguration("Whatsapp")
            .ValidateOnStart();

        Services.AddSingleton(provider => provider.GetRequiredService<IOptions<WhatsappOptions>>().Value.Puppeteer);
        Services.AddSingleton(provider => provider.GetRequiredService<IOptions<WhatsappOptions>>().Value.LoginWebCache);

        Services.AddSingleton<IEventDispatcher, EventDispatcher>();
        Services.AddSingleton<IRegisterEventService, RegisterEventService>();
        Services.AddSingleton<ILoginWebCacheProvider, LoginWebCacheProvider>();
        Services.AddSingleton<Client>();

        ServiceProvider = Services.BuildServiceProvider();
        EventDispatcher = ServiceProvider.GetService<IEventDispatcher>();
    }


}