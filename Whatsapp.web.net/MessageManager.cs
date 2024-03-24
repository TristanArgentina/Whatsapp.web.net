using Newtonsoft.Json;
using PuppeteerSharp;
using Whatsapp.web.net.Domains;
using Whatsapp.web.net.Elements;
using Whatsapp.web.net.Extensions;

namespace Whatsapp.web.net;

public class MessageManager : IMessageManager
{
    private readonly IJavaScriptParser _parserFunctions;
    private readonly IPage _pupPage;

    public MessageManager(IJavaScriptParser parserFunctions, IPage pupPage)
    {
        _parserFunctions = parserFunctions;
        _pupPage = pupPage;
    }

    public static UserId GetContactId(Message msg)
    {
        return msg.Id.FromMe ? msg.To : msg.From;
    }

    public async Task<Message> Send(UserId from, string content)
    {
        return await Send(from.Id, content);
    }

    public async Task<Message> Send(UserId from, MessageMedia attachmentData, ReplayOptions options)
    {
        return await Send(from.Id, attachmentData, options);
    }

    public async Task<Message> Send(UserId fromId, object content, ReplayOptions? options = null)
    {
        return await Send(fromId.Id, content, options);
    }

    public async Task<Message> Send(string fromId, object content, ReplayOptions? options = null)
    {
        var mentions = new List<UserId>();
        if (options != null && options.Mentions != null && options.Mentions.Any())
        {
            mentions = options.Mentions.OfType<Contact>().Select(c => c.Id).ToList();
        }

        var internalOptions = new Dictionary<string, object?>
        {
            { "linkPreview", options != null && options.LinkPreview && !options.LinkPreview ? null : true },
            { "sendAudioAsVoice", options != null && options.SendAudioAsVoice ? options.SendAudioAsVoice: null },
            { "sendVideoAsGif", options != null && options.SendVideoAsGif ? options.SendVideoAsGif : null },
            { "sendMediaAsSticker", options != null && options.SendMediaAsSticker ? options.SendMediaAsSticker : null },
            { "sendMediaAsDocument", options != null && options.SendMediaAsDocument ? options.SendMediaAsDocument : null },
            { "caption", options != null && !string.IsNullOrEmpty( options.Caption) ? options.Caption: null },
            { "quotedMessageId", options != null && options.QuotedMessageId is not null ? options.QuotedMessageId : null },
            { "parseVCards", options != null && options.ParseVCards && !options.ParseVCards? false : true },
            { "mentionedJidList", mentions },
            { "groupMentions", options != null && options.GroupMentions is not null && options.GroupMentions.Any() ? options.GroupMentions : null },
            { "extraOptions", options?.Extra }
        };

        var sendSeen = options != null && options.SendSeen ? options.SendSeen : true;

        if (content is MessageMedia)
        {
            internalOptions["attachment"] = content;
            content = "";
        }
        else if (options != null && options.Media is not null)
        {
            internalOptions["attachment"] = options.Media;
            internalOptions["caption"] = content;
            content = "";
        }
        else if (content is Location)
        {
            internalOptions["location"] = content;
            content = "";
        }
        else if (content is Poll)
        {
            internalOptions["poll"] = content;
            content = "";
        }
        else if (content is Contact contactCard)
        {
            internalOptions["contactCard"] = contactCard.Id;
            content = "";
        }
        else if (content is IList<Contact> contactList && contactList.Any())
        {
            internalOptions["contactCardList"] = contactList.Select(contact => ((Contact)contact).Id).ToList();
            content = "";
        }
        else if (content is Buttons buttons)
        {
            if (buttons.Type != "chat") internalOptions["attachment"] = buttons.Body;
            internalOptions["buttons"] = buttons;
            content = "";
        }
        else if (content is List)
        {
            internalOptions["list"] = content;
            content = "";
        }

        if (internalOptions.ContainsKey("sendMediaAsSticker") && internalOptions["sendMediaAsSticker"] != null && internalOptions.ContainsKey("attachment"))
        {
            internalOptions["attachment"] = await Util.FormatToWebpSticker(
                (MessageMedia)internalOptions["attachment"],
                new StickerMetadata
                {
                    Name = options != null && !string.IsNullOrEmpty(options.StickerName) ? options.StickerName : null,
                    Author = options != null && !string.IsNullOrEmpty(options.StickerAuthor) ? options.StickerAuthor : null,
                    Categories = options != null && options.StickerCategories is not null && options.StickerCategories.Any() ? options.StickerCategories.ToArray() : null
                },
                _pupPage
            );
        }

        var serialize = JsonConvert.SerializeObject(internalOptions);
        var method = _parserFunctions.GetMethod("sendMessageAsyncToChat");

        var newMessage = await _pupPage.EvaluateFunctionAsync<dynamic>(method, fromId, content, serialize, sendSeen);

        return new Message(newMessage);
    }
    public async Task React(MessageId? msgId, string reaction)
    {
        if (msgId is null) return;
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("reactToMessage"), msgId, reaction);
    }

    public async Task Forward(MessageId? msgId, string chatId)
    {
        if (msgId is null) return;
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("forwardMessages"), msgId, chatId);
    }

    public async Task<MessageMedia?> DownloadMedia(MessageId? msgId, bool hasMedia)
    {
        if (msgId is null) return null;
        if (!hasMedia) return null;

        var result = await _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("retrieveAndConvertMedia"), msgId);
        return result == null ? null : new MessageMedia(result);
    }

    /// <summary>
    /// Deletes a message from the chat
    /// </summary>
    /// <param name="msgId"></param>
    /// <param name="everyone">If true and the message is sent by the current user or the user is an admin, will delete it for everyone in the chat.</param>
    /// <returns></returns>
    public async Task Delete(MessageId msgId, bool? everyone)
    {
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("deleteMessageAsyncWithPermissions"), msgId, everyone);
    }

    /// <summary>
    /// Stars this message
    /// </summary>
    /// <param name="msgId"></param>
    /// <returns></returns>
    public async Task Star(MessageId msgId)
    {
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("starMessageIfAllowed"), msgId);
    }

    /// <summary>
    ///  Unstars this message
    /// </summary>
    /// <param name="msgId"></param>
    /// <returns></returns>
    public async Task UnStar(MessageId msgId)
    {
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("unstarMessage"), msgId);
    }


    /// <summary>
    /// Pins the message (group admins can pin messages of all group members)
    /// </summary>
    /// <param name="duration"> The duration in seconds the message will be pinned in a chat</param>
    /// <returns>Returns true if the operation completed successfully, false otherwise</returns>
    public async Task<bool> Pin(MessageId msgId, int duration)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("pinMessage"), msgId, duration);
    }

    /// <summary>
    /// Unpins the message (group admins can unpin messages of all group members)
    /// </summary>
    /// <param name="msgId"></param>
    /// <returns>Returns true if the operation completed successfully, false otherwise</returns>
    public async Task<bool> Unpin(MessageId msgId)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("unpinMessage"), msgId);
    }

    public async Task<MessageInfo?> GetInfo(MessageId msgId)
    {
        var infoJson = await _pupPage.EvaluateFunctionAsync<string>(_parserFunctions.GetMethod("getMessageInfo"), msgId);

        return infoJson == null ? null : JsonConvert.DeserializeObject<MessageInfo>(infoJson);
    }

    public async Task<ReactionList?> GetReactions(MessageId msgId, bool hasReaction)
    {
        if (!hasReaction) return null;
        var data = await _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("getReactions"), msgId);
        return new ReactionList(data);
    }

    public async Task<Message?> Edit(MessageId msgId, string content, ReplayOptions? options = null)
    {
        if (!msgId.FromMe) return null;

        if (options?.Mentions != null)
        {
            options.Mentions = options.Mentions.Select(m => m is Contact c ? c.Id : m).ToList();
        }

        var internalOptions = new
        {
            linkPreview = options?.LinkPreview == false ? (bool?)null : true,
            mentionedJidList = options?.Mentions ?? [],
            groupMentions = options?.GroupMentions,
            extraOptions = options?.Extra
        };


        var data = await _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("editMessage"), msgId, content, internalOptions);

        return data != null ? new Message(data) : null;
    }

    /// <summary>
    /// Reloads this Message object's data in-place with the latest values from WhatsApp Web. 
    /// Note that the Message must still be in the web app cache for this to work, otherwise will return null.
    /// </summary>
    /// <returns></returns>
    public async Task<Message?> Get(MessageId msgId)
    {
        var newData = await _pupPage.EvaluateFunctionAsync<Message>(_parserFunctions.GetMethod("getMessageModelById"), msgId);
        return newData == null ? null : new Message(newData);
    }

    public async Task<Message?> GetQuoted(MessageId msgId, bool hasQuotedMsg = true)
    {
        if (!hasQuotedMsg) return null;
        var quotedMsg = await _pupPage.EvaluateFunctionAsync<Message>(_parserFunctions.GetMethod("getQuotedMessageModel"), msgId);
        return quotedMsg == null ? null : new Message(quotedMsg);
    }

    public async Task<Message> Reply(Message msg, object content, string? contactId = null, ReplayOptions? options = null)
    {
        if (string.IsNullOrEmpty(contactId))
        {
            contactId = msg.GetContactId().Id;
        }

        options ??= new ReplayOptions();
        options.QuotedMessageId = msg.Id;

        return await Send(contactId, content, options);
    }

}