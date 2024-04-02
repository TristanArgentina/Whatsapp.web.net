using NUnit.Framework;

namespace Whatsapp.web.net.test.Managers;

[TestFixture]
public class ChatManagerTests : TestBase
{
    [Test]
    public void GetChatByIdTest()
    {

        var chat = Client!.Chat.Get(ContactId1.Id).Result;

        Assert.That(chat is not null);
        Assert.That(chat.Id._serialized == ContactId1._serialized);
        Assert.That(!chat.IsGroup);
    }

    [Test]
    public void GetAllChatsTest()
    {
        var chats = Client!.Chat.Get().Result;
        var chat = chats.FirstOrDefault(c => c.Id._serialized == ContactId1._serialized);
        Assert.That(chat is not null);
        Assert.That(chat.Id._serialized == ContactId1._serialized);
        Assert.That(!chat.IsGroup);
    }
}