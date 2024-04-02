 [![Discord Chat](https://img.shields.io/discord/698610475432411196.svg?logo=discord)](https://discord.com/channels/1224834763886428252/1224834764356456518)  

 
# whatsapp.web.net
A WhatsApp API client that connects through the WhatsApp Web browser app

It uses Puppeteer to run a real instance of Whatsapp Web to avoid getting blocked.

**NOTE:** It is based on the whatsapp-web.js project.

**It is experimental and is not intended for use in production.**
## Example usage

```c#
using NUnit.Framework;
using Whatsapp.web.net.Domains;

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
        Assert.That(msg.VCards[0].Telephone.Value.Replace("+","").Replace(" ","") == ContactId1.User);
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
        Assert.That(msg.VCards[0].Telephone.Value.Replace("+", "").Replace(" ", "") == ContactId1.User);
    }

}
```

## Contributing

Pull requests are welcome! If you see something you'd like to add, please do. For drastic changes, please open an issue first.

## Supporting the project

You can support the maintainer of this project through the links below

- [Support via PayPal](https://paypal.me/tristanrobra)

## Disclaimer

This project is not affiliated, associated, authorized, endorsed by, or in any way officially connected with WhatsApp or any of its subsidiaries or its affiliates. The official WhatsApp website can be found at https://whatsapp.com. "WhatsApp" as well as related names, marks, emblems and images are registered trademarks of their respective owners.

## License

Copyright 2024 Tristán Robra

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this project except in compliance with the License.
You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
