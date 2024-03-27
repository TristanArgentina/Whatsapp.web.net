using NUnit.Framework;
using Whatsapp.web.net.Domains;
using Whatsapp.web.net.EventArgs;

namespace Whatsapp.web.net.test;

[TestFixture]
public class MessageManagerTests
{
    private Client? _client;
    private EventDispatcher? _eventDispatcher;
    private readonly UserId _userId = new ContactId("******", "c.us");

    [OneTimeSetUp]
    public async Task Setup()
    {
        (_client, _eventDispatcher) = await ClientHelper.CreateClient();

    }

    [Test]
    public void SendTextTest()
    {
        var expectedContent = "Hello world";

        Message? message = null;

        _eventDispatcher!.MessageCreateEvent += (_, args) => message = args.Message;

        var msg = _client!.Message.Send(_userId, expectedContent).Result;
        Assert.That(message is not null);
        Assert.That(message!.To.Id == _userId.Id);
        Assert.That(message.From.Id == _userId.Id);
        Assert.That(message.Type == MessageTypes.TEXT);
        Assert.That(message.Id!.FromMe);
        Assert.That(message.Body == expectedContent);
    }


    [Test]
    public void SendImageFromFilePathTest()
    {
        var expectedMedia = MessageMedia.FromFilePath("image.png");

        Message? message = null;

        _eventDispatcher!.MessageCreateEvent += (_, args) => message = args.Message;

        var msg = _client!.Message.Send(_userId, expectedMedia).Result;
        Assert.That(message is not null);
        Assert.That(message!.To.Id == _userId.Id);
        Assert.That(message.From.Id == _userId.Id);
        Assert.That(message.Type == MessageTypes.IMAGE);
        Assert.That(message.Id!.FromMe);
        Assert.That(message.HasMedia);

        var resultMedia = _client.Message.DownloadMedia(msg.Id, message.HasMedia).Result;
        Assert.That(resultMedia is not null);
    }

    [Test]
    public void SendImageAsDocumentFromFilePathTest()
    {
        var expectedMedia = MessageMedia.FromFilePath("image.png");

        Message? message = null;

        _eventDispatcher!.MessageCreateEvent += (_, args) => message = args.Message;

        var msg = _client!.Message.Send(_userId, expectedMedia, new MessageOptions { SendMediaAsDocument = true }).Result;
        Assert.That(message is not null);
        Assert.That(message!.To.Id == _userId.Id);
        Assert.That(message.From.Id == _userId.Id);
        Assert.That(message.Type == MessageTypes.DOCUMENT);
        Assert.That(message.Id!.FromMe);
        Assert.That(message.HasMedia);

        var resultMedia = _client.Message.DownloadMedia(msg.Id, message.HasMedia).Result;
        Assert.That(resultMedia is not null);
        Assert.That(resultMedia!.Equals(expectedMedia));
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
        Assert.That(message.Type == MessageTypes.IMAGE);
        Assert.That(message.Id.FromMe);
        Assert.That(message.HasMedia);
        var resultMedia = _client.Message.DownloadMedia(msg.Id, message.HasMedia).Result;
        Assert.That(resultMedia is not null);
    }

    [Test]
    public void SendImageAStickerFromFilePathTest()
    {
        var expectedMedia = MessageMedia.FromFilePath("image.png");

        Message? message = null;

        _eventDispatcher!.MessageCreateEvent += (_, args) => message = args.Message;

        var messageOptions = new MessageOptions
        {
            SendMediaAsSticker = true,
            StickerAuthor = "Author Test",
            StickerName = "Robot"
        };
        var msg = _client!.Message.Send(_userId, expectedMedia, messageOptions).Result;
        Assert.That(message is not null);
        Assert.That(message!.To.Id == _userId.Id);
        Assert.That(message.From.Id == _userId.Id);
        Assert.That(message.Type == MessageTypes.STICKER);
        Assert.That(message.Id!.FromMe);
        Assert.That(message.HasMedia);

        var resultMedia = _client.Message.DownloadMedia(msg.Id, message.HasMedia).Result;
        Assert.That(resultMedia is not null);
    }


    [Test]
    public void SendLocationTest()
    {
        var description = "Sakananjy\nMadagascar";
        var expectedLocation = new Location(-21.537863752674124, 45.40548904077514, description);
        var msg = _client!.Message.Send(_userId, expectedLocation).Result;
        Assert.That(msg is not null);
        Assert.That(msg!.To.Id == _userId.Id);
        Assert.That(msg.From.Id == _userId.Id);
        Assert.That(msg.Type == MessageTypes.LOCATION);
        Assert.That(msg.Id!.FromMe);
        
        Assert.That(msg.Location is not null);
        Assert.That(msg.Location!.Latitude == expectedLocation.Latitude);
        Assert.That(msg.Location.Longitude == expectedLocation.Longitude);
        Assert.That(msg.Location.Description! == description);
    }

    [Test]
    public void SendVCardTest()
    {

        var expectedVCard = new VCard()
        {
            Version = "3.0",
            FullName = "John Doe",
            Names = ["Dow", "John"],
            Email = "john@doe.com",
            Telephone = "1234567890",
            Revision = DateTime.Now.ToUniversalTime()
        };

        var msg = _client!.Message.Send(_userId, expectedVCard.ToString()).Result;

        Assert.That(msg is not null);
        Assert.That(msg!.To.Id == _userId.Id);
        Assert.That(msg.From.Id == _userId.Id);
        Assert.That(msg.Type == MessageTypes.CONTACT_CARD);
        Assert.That(msg.Id!.FromMe);
        Assert.That(msg.VCards is not null);
        Assert.That(msg.VCards.Count == 1);
        Assert.That(msg.VCards[0].Version == expectedVCard.Version);
        Assert.That(msg.VCards[0].FullName == expectedVCard.FullName);
        Assert.That(msg.VCards[0].Names.SequenceEqual(expectedVCard.Names));
        Assert.That(msg.VCards[0].Email == expectedVCard.Email);
        Assert.That(msg.VCards[0].Telephone == expectedVCard.Telephone);
    }
}