using Newtonsoft.Json;
using PuppeteerSharp;
using Whatsapp.web.net.Domains;
using Whatsapp.web.net.Extensions;
using Whatsapp.web.net.scripts;

namespace Whatsapp.web.net.Managers;

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

    public async Task<Message> Send(UserId from, MessageMedia attachmentData, MessageOptions options)
    {
        return await Send(from.Id, attachmentData, options);
    }

    public async Task<Message> Send(UserId fromId, object content, MessageOptions? options = null)
    {
        return await Send(fromId.Id, content, options);
    }

    public async Task<Message> Send(string fromId, object content, MessageOptions? options = null)
    {
        options ??= new MessageOptions();
        var internalOptions = BuildInternalOptions(options);

        var sendSeen = options.SendSeen;

        if (content is MessageMedia messageMedia)
        {
            internalOptions["attachment"] = messageMedia;
            content = "";
            if (internalOptions.ContainsKey("sendMediaAsSticker")
                && (bool)internalOptions["sendMediaAsSticker"] 
                && internalOptions.ContainsKey("attachment"))
            {
                internalOptions["attachment"] = await Util.FormatToWebpSticker(
                    messageMedia,
                    new StickerMetadata
                    {
                        Name = options.StickerName,
                        Author = options.StickerAuthor,
                        Categories = options.StickerCategories is not null && options.StickerCategories.Any() ? options.StickerCategories.ToArray() : null
                    },
                    _pupPage
                );
            }
        }
        else if (options is { Media: not null })
        {
            internalOptions["attachment"] = options.Media;
            internalOptions["caption"] = content;
            content = "";
        }
        else switch (content)
        {
            case Location:
                internalOptions["location"] = content;
                content = "";
                break;
            case Poll:
                internalOptions["poll"] = content;
                content = "";
                break;
            case Contact contactCard:
                internalOptions["contactCard"] = contactCard.Id._serialized;
                content = "";
                break;
            case IList<Contact> contactList when contactList.Any():
                internalOptions["contactCardList"] = contactList.Select(contact => contact.Id._serialized).ToList();
                content = "";
                break;
            case Contact[] contactList when contactList.Any():
                internalOptions["contactCardList"] = contactList.Select(contact => contact.Id._serialized).ToList();
                content = "";
                break;
                case Buttons buttons:
                {
                    if (buttons.Type != "chat") internalOptions["attachment"] = buttons.Body;
                    internalOptions["buttons"] = buttons;
                    content = "";
                    break;
                }
            case List:
                internalOptions["list"] = content;
                content = "";
                break;
        }



        var serialize = JsonConvert.SerializeObject(internalOptions);
        var method = _parserFunctions.GetMethod("sendMessageAsyncToChat");

        var newMessage = await _pupPage.EvaluateFunctionAsync<dynamic>(method, fromId, content, internalOptions, sendSeen);

        return new Message(newMessage);
    }


    private Dictionary<string, object?> BuildInternalOptions(MessageOptions options)
    {
        var internalOptions = new Dictionary<string, object?>
        {
            { "linkPreview", options.LinkPreview },
            { "sendAudioAsVoice", options.SendAudioAsVoice },
            { "sendVideoAsGif", options.SendVideoAsGif },
            { "sendMediaAsSticker", options.SendMediaAsSticker },
            { "sendMediaAsDocument", options.SendMediaAsDocument },
            { "caption", options.Caption},
            { "quotedMessageId", options.QuotedMessageId },
            { "parseVCards", options.ParseVCards },
            { "groupMentions", options.GroupMentions?.Any() == true ? options.GroupMentions : null },
            { "extraOptions", options.Extra }
        };

        if (options.Mentions?.Any() == true)
        {
            internalOptions["mentionedJidList"] = options.Mentions.OfType<Contact>().Select(c => c.Id._serialized).ToList();
        }

        return internalOptions;
    }

    public async Task SendReact(MessageId? msgId, string reaction)
    {
        if (msgId is null) return;
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("reactToMessage"), msgId._serialized, reaction);
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

        dynamic result = await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getMessageMedia"), msgId._serialized);
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

    public async Task<Message?> Edit(MessageId msgId, string content, MessageOptions? options = null)
    {
        if (!msgId.FromMe) return null;

        //TODO: review
        //if (options?.Mentions != null)
        //{
        //    options.Mentions = options.Mentions.Select(m => m is Contact c ? c.Id : m).ToList();
        //}

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

    public async Task<Message> Reply(Message msg, object content, string? contactId = null, MessageOptions? options = null)
    {
        if (string.IsNullOrEmpty(contactId))
        {
            contactId = msg.GetContactId().Id;
        }

        options ??= new MessageOptions();
        options.QuotedMessageId = msg.Id;

        return await Send(contactId, content, options);
    }

}