using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.Extensions;

public static class MessageExtensions
{
    public static UserId GetChatId(this Message msg, Client client)
    {
        return client.Message.GetContactId(msg);
    }

    /// <summary>
    /// Reloads this Message object's data in-place with the latest values from WhatsApp Web. 
    /// Note that the Message must still be in the web app cache for this to work, otherwise will return null.
    /// </summary>
    /// <returns></returns>
    public static async Task<Message?> ReloadAsync(this Message msg, Client client)
    {
        return await client.Message.Get(msg.Id);
    }

    /// <summary>
    /// Returns the Chat this message was sent in
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    public static async Task<Chat> GetChat(this Message msg, Client client)
    {
        var chatId = msg.GetChatId(client);
        return client.Chat.Get(chatId._serialized).Result;
    }

    /// <summary>
    /// Returns the Contact this message was sent from
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    public static async Task<Contact> GetContactId(this Message msg, Client client)
    {
        return await client.Contact.Get(msg.Author is not null ? msg.Author.Id : msg.From.Id);
    }

    /// <summary>
    /// Returns the Contacts mentioned in this message
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Contact>> GetMentions(this Message msg, Client client)
    {
        List<Task<Contact>> contactTasks = msg.MentionedIds.Select(async id => await client.Contact.Get(id)).ToList();
        var contacts = await Task.WhenAll(contactTasks);
        return contacts.ToList();
    }

    public static async Task<List<GroupChat>> GetGroupMentions(this Message msg, Client client)
    {
        List<Task<Chat>> groupTask = msg.GroupMentions.Select(async m => client.Chat.Get(m.Id).Result).ToList();
        var chats = await Task.WhenAll(groupTask);
        return chats.OfType<GroupChat>().ToList();
    }

    public static async Task<Message?> GetQuotedMessage(this Message msg, Client client)
    {
        return await client.Message.GetQuoted(msg.Id, msg.HasQuotedMsg);
    }

    public static async Task<Message> Reply(this Message msg, Client client, object content,
        MessageOptions? options = null)
    {
        var chatId = msg.GetChatId(client);
        return await client.Message.Reply(msg, content, chatId, options);
    }

    public static async Task<Message> Reply(this Message msg, Client client, UserId chatId, object content,
        MessageOptions? options = null)
    {
        return await client.Message.Reply(msg, content, chatId, options);
    }

    public static async Task<dynamic> AcceptGroupV4Invite(this Message msg, Client client)
    {
        return client.Group.AcceptInvite(msg.InviteV4);
    }

    /// <summary>
    ///  React to this message with an emoji
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="client"></param>
    /// <param name="redaction">Emoji to react with. Send an empty string to remove the reaction.</param>
    /// <returns></returns>
    public static async Task SendReact(this Message msg, Client client, string redaction)
    {
        await client.Message.SendReact(msg.Id, redaction);
    }

    public static async Task Forward(this Message msg, Client client, string chatId)
    {
        await client.Message.Forward(msg.Id, chatId);
    }

    public static async Task<MessageMedia?> DownloadMedia(this Message msg, Client client)
    {
        return await client.Message.DownloadMedia(msg.Id, msg.HasMedia);
    }

    /// <summary>
    /// Deletes a message from the chat
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="client"></param>
    /// <param name="everyone">If true and the message is sent by the current user or the user is an admin, will delete it for everyone in the chat.</param>
    /// <returns></returns>
    public static async Task Delete(this Message msg, Client client, bool everyone)
    {
        await client.Message.Delete(msg.Id, everyone);
    }

    /// <summary>
    /// Stars this message
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    public static async Task Star(this Message msg, Client client)
    {
        await client.Message.Star(msg.Id);
    }

    /// <summary>
    /// Unstars this message
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    public static async Task Unstar(this Message msg, Client client)
    {
        await client.Message.UnStar(msg.Id);
    }

    public static async Task<bool> Pin(this Message msg, Client client, int duration)
    {
        return await client.Message.Pin(msg.Id, duration);
    }

    public static async Task<bool> Unpin(this Message msg, Client client)
    {
        return await client.Message.Unpin(msg.Id);
    }

    public static async Task<MessageInfo?> GetInfo(this Message msg, Client client)
    {
        return await client.Message.GetInfo(msg.Id);
    }

    public static async Task<Order?> GetOrder(this Message msg, Client client)
    {
        return await client.Commerce.GetOrderAsync(msg.Type, msg.OrderId, msg.Token, msg.GetChatId(client).Id);
    }

    public static async Task<Payment> GetPayment(this Message msg, Client client)
    {
        return await client.Commerce.GetPayment(msg.Id, msg.Type);
    }

    public static async Task<ReactionList[]> GetReactions(this Message msg, Client client)
    {
        return await client.Message.GetReactions(msg.Id, msg.HasReaction);
    }

    public static async Task<Message> Edit(this Message msg, Client client, string content, dynamic? options = null)
    {
        return await client.Message.Edit(msg.Id, content, options);
    }
}