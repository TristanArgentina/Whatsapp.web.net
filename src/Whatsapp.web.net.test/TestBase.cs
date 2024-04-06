using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.test;

public class TestBase
{
    protected IEventDispatcher? EventDispatcher;
    protected Client? Client;
    protected ContactId? ContactId1;
    protected ContactId? ContactId2;

    [OneTimeSetUp]
    public async Task Setup()
    {
        var bootstrapper = new Bootstrapper();

        bootstrapper.Services.AddOptions<DummyOptions>()
            .BindConfiguration("Dummy")
            .ValidateOnStart();

        Client = bootstrapper.Start();
        EventDispatcher = bootstrapper.EventDispatcher;

        var serviceProvider = bootstrapper.Services.BuildServiceProvider();
        var dummyOptions = serviceProvider.GetRequiredService<IOptions<DummyOptions>>().Value;
        
        ContactId1 = new ContactId(dummyOptions.User1.User, dummyOptions.User1.Server);
        ContactId2 = new ContactId(dummyOptions.User2.User, dummyOptions.User2.Server);
        
        var puppeteerOptions = serviceProvider.GetRequiredService<PuppeteerOptions>();
        
        TaskUtils.KillProcessesByName("chrome", puppeteerOptions.ExecutablePath!);

        await Client!.Initialize();
    }
}