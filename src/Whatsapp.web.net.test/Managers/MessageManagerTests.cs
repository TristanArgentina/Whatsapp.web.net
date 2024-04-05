using NUnit.Framework;
using Whatsapp.web.net.Domains;
using Whatsapp.web.net.Extensions;

namespace Whatsapp.web.net.test.Managers;

[TestFixture]
public class MessageManagerTests : TestBase
{
    [Test]
    public void SendTextTest()
    {
        var expectedContent = "Hello world";

        Message? message = null;

        EventDispatcher!.MessageCreateEvent += (_, args) => message = args.Message;

        var msg = Client!.Message.Send(ContactId1, expectedContent).Result;
        Assert.That(message is not null);
        Assert.That(message!.To.Id == ContactId1.Id);
        Assert.That(message.From.Id == ContactId1.Id);
        Assert.That(message.Type == MessageTypes.TEXT);
        Assert.That(message.Id!.FromMe);
        Assert.That(message.Body == expectedContent);

        Assert.That(msg is not null);
        Assert.That(msg!.To.Id == ContactId1.Id);
        Assert.That(msg.From.Id == ContactId1.Id);
        Assert.That(msg.Type == MessageTypes.TEXT);
        Assert.That(msg.Id!.FromMe);
        Assert.That(msg.Body == expectedContent);
    }


    [Test]
    public void SendImageFromFilePathTest()
    {
        var expectedMedia = MessageMedia.FromFilePath("image.png");

        Message? message = null;

        EventDispatcher!.MessageCreateEvent += (_, args) => message = args.Message;

        var msg = Client!.Message.Send(ContactId1, expectedMedia).Result;
        Assert.That(message is not null);
        Assert.That(message!.To.Id == ContactId1.Id);
        Assert.That(message.From.Id == ContactId1.Id);
        Assert.That(message.Type == MessageTypes.IMAGE);
        Assert.That(message.Id!.FromMe);
        Assert.That(message.HasMedia);

        var resultMedia = Client.Message.DownloadMedia(msg.Id, message.HasMedia).Result;
        Assert.That(resultMedia is not null);

        Assert.That(msg is not null);
        Assert.That(msg!.To.Id == ContactId1.Id);
        Assert.That(msg.From.Id == ContactId1.Id);
        Assert.That(msg.Type == MessageTypes.IMAGE);
        Assert.That(msg.Id!.FromMe);
        Assert.That(msg.HasMedia);
    }

    [Test]
    public void SendImageAsDocumentFromFilePathTest()
    {
        var expectedMedia = MessageMedia.FromFilePath("image.png");

        Message? message = null;

        EventDispatcher!.MessageCreateEvent += (_, args) => message = args.Message;

        var msg = Client!.Message.Send(ContactId1, expectedMedia, new MessageOptions { SendMediaAsDocument = true }).Result;
        Assert.That(message is not null);
        Assert.That(message!.To.Id == ContactId1.Id);
        Assert.That(message.From.Id == ContactId1.Id);
        Assert.That(message.Type == MessageTypes.DOCUMENT);
        Assert.That(message.Id!.FromMe);
        Assert.That(message.HasMedia);

        var resultMedia = Client.Message.DownloadMedia(msg.Id, message.HasMedia).Result;
        Assert.That(resultMedia is not null);
        Assert.That(resultMedia!.Equals(expectedMedia));

        Assert.That(msg is not null);
        Assert.That(msg!.To.Id == ContactId1.Id);
        Assert.That(msg.From.Id == ContactId1.Id);
        Assert.That(msg.Type == MessageTypes.DOCUMENT);
        Assert.That(msg.Id!.FromMe);
        Assert.That(msg.HasMedia);
    }

    [Test]
    public void SendImageFromUrlTest()
    {
        var expectedMedia = MessageMedia.FromUrl("https://www.google.com/images/branding/googlelogo/2x/googlelogo_light_color_272x92dp.png", true).Result;

        Message? message = null;

        EventDispatcher!.MessageCreateEvent += (_, args) => message = args.Message;

        var msg = Client!.Message.Send(ContactId1, expectedMedia).Result;
        Assert.That(message is not null);
        Assert.That(message.To.Id == ContactId1.Id);
        Assert.That(message.From.Id == ContactId1.Id);
        Assert.That(message.Type == MessageTypes.IMAGE);
        Assert.That(message.Id.FromMe);
        Assert.That(message.HasMedia);
        var resultMedia = Client.Message.DownloadMedia(msg.Id, message.HasMedia).Result;
        Assert.That(resultMedia is not null);
    }

    [Test]
    public void SendImageAStickerFromFilePathTest()
    {
        var expectedMedia = MessageMedia.FromFilePath("image.png");

        Message? message = null;

        EventDispatcher!.MessageCreateEvent += (_, args) => message = args.Message;

        var messageOptions = new MessageOptions
        {
            SendMediaAsSticker = true,
            StickerAuthor = "Author Test",
            StickerName = "Robot"
        };
        var msg = Client!.Message.Send(ContactId1, expectedMedia, messageOptions).Result;
        Assert.That(message is not null);
        Assert.That(message!.To.Id == ContactId1.Id);
        Assert.That(message.From.Id == ContactId1.Id);
        Assert.That(message.Type == MessageTypes.STICKER);
        Assert.That(message.Id!.FromMe);
        Assert.That(message.HasMedia);

        var resultMedia = Client.Message.DownloadMedia(msg.Id, message.HasMedia).Result;
        Assert.That(resultMedia is not null);
    }


    [Test]
    public void SendLocationTest()
    {
        var description = "Sakananjy\nMadagascar";
        var expectedLocation = new Location(-21.537863752674124, 45.40548904077514, description);
        var msg = Client!.Message.Send(ContactId1, expectedLocation).Result;
        Assert.That(msg is not null);
        Assert.That(msg!.To.Id == ContactId1.Id);
        Assert.That(msg.From.Id == ContactId1.Id);
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
            FullName = "Johnson Don",
            Names = ["Don", "Johnson"],
            Email = new Email("Johnson@don.com"),
            Telephone = new Phone("3107140202"),
            Revision = DateTime.Now.ToUniversalTime()
        };

        var msg = Client!.Message.Send(ContactId1, expectedVCard.ToString()).Result;

        Assert.That(msg is not null);
        Assert.That(msg!.To.Id == ContactId1.Id);
        Assert.That(msg.From.Id == ContactId1.Id);
        Assert.That(msg.Type == MessageTypes.CONTACT_CARD);
        Assert.That(msg.Id!.FromMe);
        Assert.That(msg.VCards is not null);
        Assert.That(msg.VCards.Count == 1);
        Assert.That(msg.VCards[0].Version == expectedVCard.Version);
        Assert.That(msg.VCards[0].FullName == expectedVCard.FullName);
        Assert.That(msg.VCards[0].Names.SequenceEqual(expectedVCard.Names));
        Assert.That(msg.VCards[0].Email.Value == expectedVCard.Email.Value);
        Assert.That(msg.VCards[0].Telephone.Value == expectedVCard.Telephone.Value);
    }

    [Test]
    public void SendVCardTurnOffParsingTest()
    {

        var expectedVCard = new VCard()
        {
            Version = "3.0",
            FullName = "Johnson Don",
            Names = ["Don", "Johnson"],
            Email = new Email("Johnson@don.com"),
            Telephone = new Phone("3107140202"),
            Revision = DateTime.Now.ToUniversalTime()
        };

        var msg = Client!.Message.Send(ContactId1, expectedVCard.ToString(), new MessageOptions { ParseVCards = false }).Result;

        Assert.That(msg is not null);
        Assert.That(msg!.To.Id == ContactId1.Id);
        Assert.That(msg.From.Id == ContactId1.Id);
        Assert.That(msg.Type == MessageTypes.TEXT);
        Assert.That(msg.Id!.FromMe);
        Assert.That(msg.VCards is null);
    }

    [Test]
    public void SendContactAsContactCardTest()
    {
        var contact = Client!.Contact.Get(ContactId1.Id).Result;

        var msg = Client!.Message.Send(ContactId1, contact).Result;

        Assert.That(msg is not null);
        Assert.That(msg.Type == MessageTypes.CONTACT_CARD);
        Assert.That(msg!.To.Id == ContactId1.Id);
        Assert.That(msg.From.Id == ContactId1.Id);
        Assert.That(msg.Id!.FromMe);
        Assert.That(msg.Body.Contains("BEGIN:VCARD"));
        Assert.That(msg.VCards is not null);
        Assert.That(msg.VCards.Count == 1);
        Assert.That(msg.VCards[0].Telephone.Value
            .Replace("+", "")
            .Replace("-", "")
            .Replace(" ", "") == ContactId1.User);
    }

    [Test]
    public void SendMultipleContactAsContactCardTest()
    {
        var contact1 = Client!.Contact.Get(ContactId1.Id).Result;
        var contact2 = Client!.Contact.Get(ContactId2.Id).Result;

        var msg = Client!.Message.Send(ContactId1, new[] { contact1, contact2 }).Result;

        Assert.That(msg is not null);
        Assert.That(msg.Type == MessageTypes.CONTACT_CARD_MULTI);
        Assert.That(msg!.To.Id == ContactId1.Id);
        Assert.That(msg.From.Id == ContactId1.Id);
        Assert.That(msg.Id!.FromMe);
        Assert.That(msg.VCards is not null);
        Assert.That(msg.VCards.Count == 2);
        Assert.That(msg.VCards[0].Telephone.Value
            .Replace("+", "")
            .Replace("-", "")
            .Replace(" ", "") == ContactId1.User);
    }

    [Test]
    public void SendReactTest()
    {
        var msg = Client!.Message.Send(ContactId1, "Hello kitty 2024").Result;
        Assert.That(msg is not null);

        var expectedReact = "\ud83d\udc4d\ud83c\udffc";

        string? reactionText = null;
        var manualEvent = new ManualResetEvent(false);

        EventDispatcher!.MessageReactionEvent += (_, args) =>
        {
            reactionText = args.Reaction.Text;
            manualEvent.Set();
        };

        msg.SendReact(Client, expectedReact).Wait();
        var eventSignaled = manualEvent.WaitOne(5000);
        Assert.That(eventSignaled);
        Assert.That(expectedReact.Equals(reactionText));
    }

    [Test]
    public void ForwardTest()
    {
        var expectedContent = "Good morning";
        var content = string.Empty;
        var msg = Client!.Message.Send(ContactId1, expectedContent).Result;
        Assert.That(msg is not null);
        EventDispatcher!.MessageCreateEvent += (_, args) =>
        {
            content = args.Message.Body;
        };
        msg.Forward(Client, "120363248319028492@g.us").Wait();

        Assert.That(expectedContent.Equals(content));
    }

    [Test]
    public void DeleteTest()
    {
        var expectedContent = "This message should be deleted.";
        var msg = Client!.Message.Send(ContactId1, expectedContent).Result;
        Assert.That(msg is not null);
        msg.Delete(Client, false).Wait();
        Thread.Sleep(10000);
        Assert.That(Client.Message.Get(msg.Id).Result is null);

    }
}