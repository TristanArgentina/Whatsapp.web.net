using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.Managers;

public interface IMessageManager : IManager
{
    Task<Message> Send(UserId from, string content);

    Task<Message> Send(UserId from, MessageMedia attachmentData, MessageOptions options);

    Task<Message> Send(UserId fromId, object content, MessageOptions? options = null);

    Task<Message> Send(string fromId, object content, MessageOptions? options = null);

    /// <summary>
    /// React to this message with an emoji
    /// </summary>
    /// <param name="msgId"></param>
    /// <param name="reaction">Emoji to react with. Send an empty string to remove the reaction.</param>
    /// <returns></returns>
    Task React(MessageId? msgId, string reaction);

    /// <summary>
    /// Forwards this message to another chat (that you chatted before, otherwise it will fail)
    /// </summary>
    /// <param name="msgId"></param>
    /// <param name="chatId">Chat model or chat ID to which the message will be forwarded</param>
    /// <returns></returns>
    Task Forward(MessageId? msgId, string chatId);

    Task<MessageMedia?> DownloadMedia(MessageId? msgId, bool hasMedia);

    /// <summary>
    ///     Deletes a message from the chat
    /// </summary>
    /// <param name="msgId"></param>
    /// <param name="everyone">
    ///     If true and the message is sent by the current user or the user is an admin, will delete it for everyone in the chat.
    ///     Value can be null.
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

    /// <summary>
    /// Get information about message delivery status. May return null if the message does not exist or is not sent by you.
    /// </summary>
    /// <param name="msgId"></param>
    /// <returns></returns>
    Task<MessageInfo?> GetInfo(MessageId msgId);

    /// <summary>
    /// Gets the reactions associated with the given message
    /// </summary>
    /// <param name="msgId"></param>
    /// <param name="hasReaction"></param>
    /// <returns></returns>
    Task<ReactionList?> GetReactions(MessageId msgId, bool hasReaction);

    /// <summary>
    /// Edits the current message.
    /// </summary>
    /// <param name="msgId"></param>
    /// <param name="content"></param>
    /// <param name="options">Options used when editing the message</param>
    /// <returns></returns>
    Task<Message?> Edit(MessageId msgId, string content, MessageOptions? options = null);

    /// <summary>
    ///     Reloads this Message object's data in-place with the latest values from WhatsApp Web.
    ///     Note that the Message must still be in the web app cache for this to work, otherwise will return null.
    /// </summary>
    /// <returns></returns>
    Task<Message?> Get(MessageId msgId);

    /// <summary>
    /// Returns the quoted message, if any
    /// </summary>
    /// <param name="msgId"></param>
    /// <param name="hasQuotedMsg"></param>
    /// <returns></returns>
    Task<Message?> GetQuoted(MessageId msgId, bool hasQuotedMsg = true);

    /// <summary>
    /// Sends a message as a reply to this message. If chatId is specified, it will be sent through the specified Chat. If not, it will send the message in the same Chat as the original message was sent.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="content"></param>
    /// <param name="contactId"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    Task<Message> Reply(Message msg, object content, string? contactId = null, MessageOptions? options = null);
}