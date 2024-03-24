using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net;

public interface IMessageManager : IManager
{
    Task<Message> Send(UserId from, string content);

    Task<Message> Send(UserId from, MessageMedia attachmentData, ReplayOptions options);

    Task<Message> Send(UserId fromId, object content, ReplayOptions? options = null);

    Task<Message> Send(string fromId, object content, ReplayOptions? options = null);


    Task React(MessageId? msgId, string reaction);

    Task Forward(MessageId? msgId, string chatId);

    Task<MessageMedia?> DownloadMedia(MessageId? msgId, bool hasMedia);

    /// <summary>
    ///     Deletes a message from the chat
    /// </summary>
    /// <param name="msgId"></param>
    /// <param name="everyone">
    ///     If true and the message is sent by the current user or the user is an admin, will delete it for
    ///     everyone in the chat.
    /// </param>
    /// <returns></returns>
    Task Delete(MessageId msgId, bool? everyone);

    /// <summary>
    ///     Stars this message
    /// </summary>
    /// <param name="msgId"></param>
    /// <returns></returns>
    Task Star(MessageId msgId);

    /// <summary>
    ///     Unstars this message
    /// </summary>
    /// <param name="msgId"></param>
    /// <returns></returns>
    Task UnStar(MessageId msgId);

    /// <summary>
    ///     Pins the message (group admins can pin messages of all group members)
    /// </summary>
    /// <param name="duration"> The duration in seconds the message will be pinned in a chat</param>
    /// <returns>Returns true if the operation completed successfully, false otherwise</returns>
    Task<bool> Pin(MessageId msgId, int duration);

    /// <summary>
    ///     Unpins the message (group admins can unpin messages of all group members)
    /// </summary>
    /// <param name="msgId"></param>
    /// <returns>Returns true if the operation completed successfully, false otherwise</returns>
    Task<bool> Unpin(MessageId msgId);

    Task<MessageInfo?> GetInfo(MessageId msgId);
    
    Task<ReactionList?> GetReactions(MessageId msgId, bool hasReaction);
    
    Task<Message?> Edit(MessageId msgId, string content, ReplayOptions? options = null);

    /// <summary>
    ///     Reloads this Message object's data in-place with the latest values from WhatsApp Web.
    ///     Note that the Message must still be in the web app cache for this to work, otherwise will return null.
    /// </summary>
    /// <returns></returns>
    Task<Message?> Get(MessageId msgId);

    Task<Message?> GetQuoted(MessageId msgId, bool hasQuotedMsg = true);
    
    Task<Message> Reply(Message msg, object content, string? contactId = null, ReplayOptions? options = null);
}