using Whatsapp.web.net.Domains;
using Whatsapp.web.net.Elements;

namespace Whatsapp.web.net.Extensions;

public static class ChatExtensions
{
    public static async Task<Message> SendMessage(this Chat chat, Client client, object content, ReplayOptions? options = null)
    {
        return await client.SendMessage(chat.Id, content, options);
    }

    public static async Task<bool> SendSeen(this Chat chat, Client client)
    {
        return await client.SendSeen(chat.Id);
    }

    public static async Task<bool> ClearMessages(this Chat chat, Client client)
    {
        return await client.ClearMessagesAsync(chat.Id);
    }

    public static async Task<bool> Delete(this Chat chat, Client client)
    {
        return await client.DeleteChatByIdAsync(chat.Id);
    }

    public static async Task SendStateTyping(this Chat chat, Client client)
    {
        await client.SendStateTypingChatByIdAsync(chat.Id);
    }

    public static async Task SendStateRecording(this Chat chat, Client client)
    {
        await client.SendStateRecordingChatByIdAsync(chat.Id);
    }

    public static async Task ClearState(this Chat chat, Client client)
    {
        await client.ClearStateChatByIdAsync(chat.Id);
    }

    public static async Task Archive(this Chat chat, Client client)
    {
        await client.ArchiveChat(chat.Id);
    }

    public static async Task UnArchive(this Chat chat, Client client)
    {
        await client.UnArchiveChat(chat.Id);
    }

    public static async Task<bool> Pin(this Chat chat, Client client)
    {
        return await client.PinChat(chat.Id);
    }

    public static async Task<bool> Unpin(this Chat chat, Client client)
    {
        return await client.UnpinChat(chat.Id);
    }

    public static async Task Mute(this Chat chat, Client client, DateTime? unMuteDate)
    {
        await client.MuteChat(chat.Id, unMuteDate);
    }

    public static async Task UnMute(this Chat chat, Client client)
    {
        await client.UnmuteChat(chat.Id);
    }

    public static async Task MarkUnread(this Chat chat, Client client)
    {
        await client.MarkChatUnread(chat.Id);
    }

    public static async Task<List<Message>> FetchMessages(this Chat chat, Client client, SearchOptions searchOptions)
    {
        return await client.FetchMessagesChatByIdSync(chat.Id, searchOptions);
    }

    public static async Task<Contact> GetContact(this Chat chat, Client client)
    {
        return await client.GetContactById(chat.Id);
    }

    public static async Task<List<Label>> GetLabels(this Chat chat, Client client)
    {
        return await client.GetChatLabels(chat.Id);
    }

    public static async Task ChangeLabels(this Chat chat, Client client, List<object> labelIds)
    {
        await client.AddOrRemoveLabels(labelIds, [chat.Id]);
    }
}