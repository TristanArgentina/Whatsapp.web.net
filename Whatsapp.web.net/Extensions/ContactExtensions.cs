using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.Extensions;

public static class ContactExtensions
{
    public static async Task<string?> GetProfilePicUrl(this Contact contact, Client client)
    {
        return await client.GetProfilePicUrl(contact.Id.Serialized);
    }

    public static async Task<string> GetFormattedNumber(this Contact contact, Client client)
    {
        return await client.GetFormattedNumber(contact.Id.Serialized);
    }

    public static async Task<string> GetCountryCode(this Contact contact, Client client)
    {
        return await client.GetCountryCode(contact.Id.Serialized);
    }

    public static async Task<Chat?> GetChat(this Contact contact, Client client)
    {
        return contact.IsMe ? null : client.GetChatById(contact.Id.Serialized);
    }

    public static async Task<bool> Block(this Contact contact, Client client)
    {
        return client.Block(contact).Result;
    }

    public static async Task<bool> Unblock(this Contact contact, Client client)
    {
        return client.Unblock(contact).Result;
    }

    public static async Task<string?> GetAbout(this Contact contact, Client client)
    {
        return client.GetAbout(contact).Result;
    }

    public static async Task<List<string>> GetCommonGroups(this Contact contact, Client client)
    {
        return await client.GetCommonGroups(contact.Id.Serialized);
    }
}