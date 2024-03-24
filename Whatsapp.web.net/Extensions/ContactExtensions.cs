using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.Extensions;

public static class ContactExtensions
{
    public static async Task<string?> GetProfilePicUrl(this Contact contact, Client client)
    {
        return await client.Contact.GetProfilePicUrl(contact.Id.Id);
    }

    public static async Task<string> GetFormattedNumber(this Contact contact, Client client)
    {
        return await client.Contact.GetFormattedNumber(contact.Id.Id);
    }

    public static async Task<string> GetCountryCode(this Contact contact, Client client)
    {
        return await client.Contact.GetCountryCode(contact.Id.Id);
    }

    public static async Task<Chat?> GetChat(this Contact contact, Client client)
    {
        return contact.IsMe ? null : client.Chat.Get(contact.Id.Id).Result;
    }

    public static async Task<bool> Block(this Contact contact, Client client)
    {
        return client.Contact.Block(contact).Result;
    }

    public static async Task<bool> Unblock(this Contact contact, Client client)
    {
        return client.Contact.Unblock(contact).Result;
    }

    public static async Task<string?> GetAbout(this Contact contact, Client client)
    {
        return client.Contact.GetAbout(contact).Result;
    }

    public static async Task<List<string>> GetCommonGroups(this Contact contact, Client client)
    {
        return await client.Group.GetCommonGroups(contact.Id.Id);
    }
}