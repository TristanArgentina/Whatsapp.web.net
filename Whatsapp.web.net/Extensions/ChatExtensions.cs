using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.Extensions;

public static class ChatExtensions
{
    public static async Task<Message> SendMessage(this Chat chat, Client client, object content, MessageOptions? options = null)
    {
        return await client.Message.Send(chat.Id, content, options);
    }

    public static async Task<bool> SendSeen(this Chat chat, Client client)
    {
        return await client.Chat.SendSeen(chat.Id.Id);
    }

    public static async Task<bool> ClearMessages(this Chat chat, Client client)
    {
        return await client.Chat.ClearMessages(chat.Id.Id);
    }

    public static async Task<bool> Delete(this Chat chat, Client client)
    {
        return await client.Chat.Delete(chat.Id.Id);
    }

    public static async Task SendStateTyping(this Chat chat, Client client)
    {
        await client.Chat.SendStateTyping(chat.Id.Id);
    }

    public static async Task SendStateRecording(this Chat chat, Client client)
    {
        await client.Chat.SendStateRecording(chat.Id.Id);
    }

    public static async Task ClearState(this Chat chat, Client client)
    {
        await client.Chat.ClearState(chat.Id.Id);
    }

    public static async Task Archive(this Chat chat, Client client)
    {
        await client.Chat.Archive(chat.Id.Id);
    }

    public static async Task UnArchive(this Chat chat, Client client)
    {
        await client.Chat.UnArchive(chat.Id.Id);
    }

    public static async Task<bool> Pin(this Chat chat, Client client)
    {
        return await client.Chat.Pin(chat.Id.Id);
    }

    public static async Task<bool> Unpin(this Chat chat, Client client)
    {
        return await client.Chat.Unpin(chat.Id.Id);
    }

    public static async Task Mute(this Chat chat, Client client, DateTime? unMuteDate)
    {
        await client.Chat.Mute(chat.Id.Id, unMuteDate);
    }

    public static async Task UnMute(this Chat chat, Client client)
    {
        await client.Chat.UnMute(chat.Id.Id);
    }

    public static async Task MarkUnread(this Chat chat, Client client)
    {
        await client.Chat.Unread(chat.Id.Id);
    }

    public static async Task<List<Message>> FetchMessages(this Chat chat, Client client, SearchOptions searchOptions)
    {
        return await client.Chat.FetchMessages(chat.Id.Id, searchOptions);
    }

    public static async Task<Contact> GetContact(this Chat chat, Client client)
    {
        return await client.Contact.GetContactById(chat.Id.Id);
    }

    public static async Task<List<Label>> GetLabels(this Chat chat, Client client)
    {
        return await client.Chat.GetLabels(chat.Id.Id);
    }

    public static async Task ChangeLabels(this Chat chat, Client client, List<object> labelIds)
    {
        await client.Chat.AddOrRemoveLabels(labelIds, [chat.Id.Id]);
    }
}