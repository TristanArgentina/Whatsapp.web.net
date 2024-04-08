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

}