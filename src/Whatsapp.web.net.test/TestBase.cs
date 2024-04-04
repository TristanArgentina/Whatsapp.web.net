using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Whatsapp.web.net.Authentication;
using Whatsapp.web.net.Domains;
using Whatsapp.web.net.EventArgs;

namespace Whatsapp.web.net.test;

public class TestBase
{
    protected IEventDispatcher? EventDispatcher;
    protected Client? Client;
    protected ContactId? ContactId1;
    protected ContactId? ContactId2;
    private ServiceProvider? _serviceProvider;

    [OneTimeSetUp]
    public async Task Setup()
    {

        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(@"appsettings.json", optional: false, reloadOnChange: true);

        builder.Services.AddOptions<WhatsappOptions>()
            .BindConfiguration("Whatsapp")
            .ValidateOnStart();

        builder.Services.AddOptions<DummyOptions>()
            .BindConfiguration("Dummy")
            .ValidateOnStart();

        builder.Services.AddSingleton(provider => provider.GetRequiredService<IOptions<WhatsappOptions>>().Value.Puppeteer);
        builder.Services.AddSingleton(provider => provider.GetRequiredService<IOptions<WhatsappOptions>>().Value.WebVersionCache);

        builder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();
        builder.Services.AddSingleton<IRegisterEventService, RegisterEventService>();
        builder.Services.AddSingleton<IAuthenticatorProvider, AuthenticatorProvider>();
        builder.Services.AddSingleton<Client>();

        _serviceProvider = builder.Services.BuildServiceProvider();

        EventDispatcher = _serviceProvider.GetService<IEventDispatcher>();
        Client = _serviceProvider.GetService<Client>();
        var dummyOptions = _serviceProvider.GetRequiredService<IOptions<DummyOptions>>().Value;
        ContactId1 = new ContactId(dummyOptions.User1.User, dummyOptions.User1.Server);
        ContactId2 = new ContactId(dummyOptions.User2.User, dummyOptions.User2.Server);
        var puppeteerOptions = _serviceProvider.GetRequiredService<PuppeteerOptions>();
        TaskUtils.KillProcessesByName("chrome", puppeteerOptions.ExecutablePath!);

        await Client!.Initialize();
    }
}