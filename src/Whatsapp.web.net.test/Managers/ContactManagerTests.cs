﻿using NUnit.Framework;

namespace Whatsapp.web.net.test.Managers;

[TestFixture]
public class ContactManagerTests : TestBase
{
    [Test]
    public void GetContactByIdTest()
    {

        var contact = Client!.Contact.Get(ContactId1.Id).Result;

        Assert.That(contact is not null);
        Assert.That(contact.Id._serialized == ContactId1._serialized);
        Assert.That(!contact.IsGroup);
    }

    [Test]
    public void GetAllContactsTest()
    {
        var contacts = Client!.Contact.Get().Result;
        var contact = contacts.FirstOrDefault(c => c.Id._serialized == ContactId1._serialized);
        Assert.That(contact is not null);
        Assert.That(contact.Id._serialized == ContactId1._serialized);
        Assert.That(!contact.IsGroup);
    }

    [Test]
    public void GetBlockContactsTest()
    {
        var contact = Client!.Contact.Get(ContactId2.Id).Result;
        var success = Client!.Contact.Block(contact).Result;
        Assert.That(success);
    }

    [Test]
    public void GetUnBlockContactsTest()
    {
        var contact = Client!.Contact.Get(ContactId2.Id).Result;
        var success = Client!.Contact.Unblock(contact).Result;
        Assert.That(success);
    }

    [Test]
    public void GetBlockedContactsTest()
    {
        var contacts = Client!.Contact.GetBlocked().Result;
        Assert.That(contacts is not null);
        Assert.That(contacts.Any());
    }

    [Test]
    public void GetCountryCodeTest()
    {
        var country = Client!.Contact.GetCountryCode(ContactId1.User).Result;
        Assert.That(country is not null);
        Assert.That(country == ContactId1.User.Substring(0,2));
    }

    [Test]
    public void GetFormattedNumberTest()
    {
        var result = Client!.Contact.GetFormattedNumber(ContactId1.User).Result;
        Assert.That(result is not null);
        Assert.That(result.StartsWith("+"));
        Assert.That(result.Contains("-"));
        Assert.That(result.Contains(" "));

    }

    [Test]
    public void GetAboutTest()
    {
        var result = Client!.Contact.GetAbout(ContactId1._serialized).Result;
        Assert.That(result is not null);
        Assert.That(result.Equals("to be or not to be", StringComparison.InvariantCultureIgnoreCase));
    }


    [Test]
    public void GetProfilePicUrlTest()
    {
        var result = Client!.Contact.GetProfilePicUrl(ContactId1._serialized).Result;
        Assert.That(result is not null);
    }

}