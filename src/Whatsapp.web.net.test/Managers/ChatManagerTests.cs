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

    [Test]
    public void PinChatTest()
    {
        var chat = Client!.Chat.Get(GroupId1.Id).Result;
        if (chat.Pinned)
        {
            TestUnPin();
            TestPin();
        }
        else
        {
            TestPin();
            TestUnPin();
        }
    }

    private void TestPin()
    {
        Client!.Chat.Pin(GroupId1.Id);
        Thread.Sleep(2000);
        var chat = Client!.Chat.Get(GroupId1.Id).Result;
        Assert.That(chat.Pinned);
    }

    private void TestUnPin()
    {
        Client!.Chat.Unpin(GroupId1.Id);
        Thread.Sleep(2000);
        var chat = Client!.Chat.Get(GroupId1.Id).Result;
        Assert.That(!chat.Pinned);
    }


    [Test]
    public void ArchiveChatTest()
    {
        var chat = Client!.Chat.Get(GroupId1.Id).Result;
        if (chat.Archived)
        {
            TestUnArchive();
            TestArchive();
        }
        else
        {
            TestArchive();
            TestUnArchive();
        }
    }

    private void TestArchive()
    {
        Client!.Chat.Archive(GroupId1.Id);
        var chat = Client!.Chat.Get(GroupId1.Id).Result;
        Assert.That(chat.Archived);
    }

    private void TestUnArchive()
    {
        Client!.Chat.UnArchive(GroupId1.Id);
        Thread.Sleep(2000);
        var chat = Client!.Chat.Get(GroupId1.Id).Result;
        Assert.That(!chat.Archived);
    }


    [Test]
    public void MuteChatTest()
    {
        var chat = Client!.Chat.Get(GroupId1.Id).Result;
        if (chat.IsMuted)
        {
            TestUnMute();
            TestMute();
        }
        else
        {
            TestMute();
            TestUnMute();
        }
    }

    private void TestMute()
    {
        Client!.Chat.Mute(GroupId1.Id, DateTime.Now.AddDays(2));
        Thread.Sleep(2000);
        var chat = Client!.Chat.Get(GroupId1.Id).Result;
        Assert.That(chat.IsMuted);
    }

    private void TestUnMute()
    {
        Client!.Chat.UnMute(GroupId1.Id);
        Thread.Sleep(2000);
        var chat = Client!.Chat.Get(GroupId1.Id).Result;
        Assert.That(!chat.IsMuted);
    }

    [Test]
    public void GetMessagesTest()
    {
        var text1 = "The series was originally published in English by Bloomsbury in the United Kingdom and Scholastic Press in the United States.";
        var text2 = "A series of many genres, including fantasy, drama, coming-of-age fiction, and the British school story (which includes elements of mystery, thriller, adventure, horror, and romance), the world of Harry Potter explores numerous themes and includes many cultural meanings and references.";
        var text3 = "Major themes in the series include prejudice, corruption, madness, and death.";
        Client!.Message.Send(GroupId1, text1).Wait();
        Client!.Message.Send(GroupId1, text2).Wait();
        Client!.Message.Send(GroupId1, text3).Wait();
        Thread.Sleep(5000);
        var messages = Client!.Chat.GetMessages(GroupId1.Id, new SearchOptions()).Result;

        Assert.That(messages[^3].Body.Equals(text1));
        Assert.That(messages[^2].Body.Equals(text2));
        Assert.That(messages[^1].Body.Equals(text3));
    }
}