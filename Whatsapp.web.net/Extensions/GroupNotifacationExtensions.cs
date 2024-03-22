using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.Extensions;

public static class GroupNotifacationExtensions
{

    public static async Task<Chat> GetChat(this GroupNotification groupNotification, Client client)
    {
        return client.GetChatById(groupNotification.ChatId.Id);
    }

    public static async Task<Contact> GetContact(this GroupNotification groupNotification, Client client)
    {
        return await client.GetContactById(groupNotification.Author.Id);
    }

    public static async Task<List<Contact>> GetRecipients(this GroupNotification groupNotification, Client client)
    {
        var recipients = new List<Contact>();
        foreach (var recipientId in groupNotification.RecipientIds)
        {
            var contact = await client.GetContactById(recipientId);
            recipients.Add(contact);
        }
        return recipients;
    }

    public static async Task<Message> Reply(this GroupNotification groupNotification, Client client,dynamic content, ReplayOptions? options = null)
    {
        return await client.SendMessage(groupNotification.ChatId.Id, content, options);
    }
}