using NUnit.Framework;
using Whatsapp.web.net.Domains;
using Whatsapp.web.net.EventArgs;

namespace Whatsapp.web.net.test;

[TestFixture]
public class ChatManagerTests
{
    private Client? _client;
    private EventDispatcher? _eventDispatcher;
    private readonly UserId _userId = new ContactId("5493816092122", "c.us");
    private readonly UserId _userId_aux = new ContactId("5493816402134", "c.us");

    [OneTimeSetUp]
    public async Task Setup()
    {
        (_client, _eventDispatcher) = await ClientHelper.CreateClient();

    }

    [Test]
    public void GetChatByIdTest()
    {

        var chat = _client!.Chat.Get(_userId.Id).Result;

        Assert.That(chat is not null);
        Assert.That(chat.Id._serialized == _userId._serialized);
        Assert.That(!chat.IsGroup);
    }

    [Test]
    public void GetAllChatsTest()
    {
        var chats = _client!.Chat.Get().Result;
        var chat = chats.FirstOrDefault(c => c.Id._serialized == _userId._serialized);
        Assert.That(chat is not null);
        Assert.That(chat.Id._serialized == _userId._serialized);
        Assert.That(!chat.IsGroup);
    }
}

[TestFixture]
public class ContactManagerTests
{
    private Client? _client;
    private EventDispatcher? _eventDispatcher;
    private readonly UserId _userId = new ContactId("5493816092122", "c.us");
    private readonly UserId _userId_aux = new ContactId("5493814128871", "c.us");

    [OneTimeSetUp]
    public async Task Setup()
    {
        (_client, _eventDispatcher) = await ClientHelper.CreateClient();

    }

    [Test]
    public void GetContactByIdTest()
    {

        var contact = _client!.Contact.Get(_userId.Id).Result;

        Assert.That(contact is not null);
        Assert.That(contact.Id._serialized == _userId._serialized);
        Assert.That(!contact.IsGroup);
    }

    [Test]
    public void GetAllContactsTest()
    {
        var contacts = _client!.Contact.Get().Result;
        var contact = contacts.FirstOrDefault(c => c.Id._serialized == _userId._serialized);
        Assert.That(contact is not null);
        Assert.That(contact.Id._serialized == _userId._serialized);
        Assert.That(!contact.IsGroup);
    }

    [Test]
    public void GetBlockContactsTest()
    {
        var contact = _client!.Contact.Get(_userId_aux.Id).Result;
        var success = _client!.Contact.Block(contact).Result;
        Assert.That(success);
    }

    [Test]
    public void GetUnBlockContactsTest()
    {
        var contact = _client!.Contact.Get(_userId_aux.Id).Result;
        var success = _client!.Contact.Unblock(contact).Result;
        Assert.That(success);
    }

    [Test]
    public void GetBlockedContactsTest()
    {
        var contacts = _client!.Contact.GetBlocked().Result;
        Assert.That(contacts is not null);
        Assert.That(contacts.Any());
    }
}