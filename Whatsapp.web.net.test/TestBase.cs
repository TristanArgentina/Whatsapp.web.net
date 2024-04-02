using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Whatsapp.web.net.Domains;
using Whatsapp.web.net.EventArgs;

namespace Whatsapp.web.net.test;

public class TestBase
{
    protected WhatsappOptions WhatsappOptions;
    protected EventDispatcher EventDispatcher;
    protected Client Client;
    protected DummyOptions DummyOptions;
    protected ContactId ContactId1;
    protected ContactId ContactId2;

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


        await using var serviceProvider = builder.Services.BuildServiceProvider();

        WhatsappOptions = serviceProvider.GetRequiredService<IOptions<WhatsappOptions>>().Value;
        DummyOptions = serviceProvider.GetRequiredService<IOptions<DummyOptions>>().Value;
        ContactId1 = new ContactId(DummyOptions.User1.User, DummyOptions.User1.Server);
        ContactId2 = new ContactId(DummyOptions.User2.User, DummyOptions.User2.Server);

        EventDispatcher = new EventDispatcher();
        var registerEventService = new RegisterEventService(EventDispatcher, WhatsappOptions);
        Client = new Client(EventDispatcher, registerEventService, WhatsappOptions);

        await Client.Initialize().Result;
        EventDispatcher.EmitReady();

       
    }
}