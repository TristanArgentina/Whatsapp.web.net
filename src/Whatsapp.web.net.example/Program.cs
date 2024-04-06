using Whatsapp.web.net;
using Whatsapp.web.net.example;

Console.WriteLine("Hello, World!");

var bootstrapper = new Bootstrapper();
var client = bootstrapper.Start();
var handleEvents = new HandleEvents(client,bootstrapper.EventDispatcher);

//TaskUtils.KillProcessesByName("chrome", whatsappOptions.Puppeteer.ExecutablePath);

client!.Console += (_, eventArgs) =>
{
    Console.WriteLine($"{DateTime.Now:G}:{eventArgs.Message.Text}");
    if (eventArgs.Message.Args is null ) return;
    for (var i = 0; i < eventArgs.Message.Args.Count; ++i)
    {
        Console.WriteLine($"{i}: {eventArgs.Message.Args[i].JsonValueAsync<string>()}");
    }
};

handleEvents.SetHandle();

Console.ReadLine();

client.Dispose();