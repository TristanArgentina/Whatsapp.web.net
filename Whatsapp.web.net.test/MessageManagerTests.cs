using NUnit.Framework;
using Whatsapp.web.net.Domains;
using Whatsapp.web.net.EventArgs;

namespace Whatsapp.web.net.test;

[TestFixture]
public class MessageManagerTests
{
    private Client? _client;
    private EventDispatcher? _eventDispatcher;
    private readonly UserId _userId = new ContactId("5493816092122","c.us");

    [OneTimeSetUp]
    public async Task Setup()
    {
        (_client, _eventDispatcher) = await ClientHelper.CreateClient();

    }



    [Test]
    public void SendTextTest()
    {
        var expectedContent = "Buenos días";

        Message? message = null;

        _eventDispatcher!.MessageCreateEvent += (_, args) => message = args.Message;

        var msg = _client!.Message.Send(_userId, expectedContent).Result;
        Assert.That(message is not null);
        Assert.That(message.To.Id == _userId.Id);
        Assert.That(message.From.Id == _userId.Id);
        Assert.That(message.Type == MessageTypes.TEXT);
        Assert.That(message.Id.FromMe);
        Assert.That(message.Body == expectedContent);
    }


    [Test]
    public void SendImageFromFilePathTest()
    {
        var expectedContent = MessageMedia.FromFilePath("image.png");

        Message? message = null;

        _eventDispatcher!.MessageCreateEvent += (_, args) => message = args.Message;

        var msg = _client!.Message.Send(_userId, expectedContent).Result;
        Assert.That(message is not null);
        Assert.That(message.To.Id == _userId.Id);
        Assert.That(message.From.Id == _userId.Id);
        //Assert.That(message.Type == MessageTypes.IMAGE);
        Assert.That(message.Id.FromMe);
        Assert.That(message.HasMedia);

        Assert.DoesNotThrow(() => _client.Message.DownloadMedia(msg.Id, message.HasMedia));
    }

    [Test]
    public void SendImageFromUrlTest()
    {
        var expectedMedia = MessageMedia.FromUrl("https://www.google.com/images/branding/googlelogo/2x/googlelogo_light_color_272x92dp.png", true).Result;

        Message? message = null;

        _eventDispatcher!.MessageCreateEvent += (_, args) => message = args.Message;

        var msg = _client!.Message.Send(_userId, expectedMedia).Result;
        Assert.That(message is not null);
        Assert.That(message.To.Id == _userId.Id);
        Assert.That(message.From.Id == _userId.Id);
        //Assert.That(message.Type == MessageTypes.IMAGE);
        Assert.That(message.Id.FromMe);
        Assert.That(message.HasMedia);
        var resultMedia = _client.Message.DownloadMedia(msg.Id, message.HasMedia).Result;
        Assert.That(resultMedia is not null);
        Assert.That(resultMedia!.Equals(expectedMedia));
    }

}