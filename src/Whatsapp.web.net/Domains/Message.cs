﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a Message on WhatsApp
/// </summary>
public class Message
{
    /// <summary>
    ///  MediaKey that represents the sticker 'ID'
    /// </summary>
    public string? MediaKey { get; private set; }

    /// <summary>
    /// ID that represents the message
    /// </summary>
    public MessageId? Id { get; private set; }

    /// <summary>
    ///  ACK status for the message
    /// </summary>
    public MessageAck? Ack { get; private set; }

    /// <summary>
    /// Indicates if the message has media available for download
    /// </summary>
    public bool HasMedia { get; private set; }

    /// <summary>
    /// Message content
    /// </summary>
    public string Body { get; private set; }

    /// <summary>
    /// Message type
    /// </summary>
    public string Type { get; private set; }

    /// <summary>
    /// Unix timestamp for when the message was created
    /// </summary>
    public DateTime? Timestamp { get; private set; }

    /// <summary>
    /// ID for the Chat that this message was sent to, except if the message was sent by the current user.
    /// </summary>
    public UserId? From { get; private set; }

    /// <summary>
    /// ID for who this message is for.
    /// If the message is sent by the current user, it will be the Chat to which the message is being sent.
    /// If the message is sent by another user, it will be the ID for the current user.
    /// </summary>
    public UserId? To { get; private set; }

    /// <summary>
    /// If the message was sent to a group, this field will contain the user that sent the message.
    /// </summary>
    public UserId Author { get; private set; }

    /// <summary>
    /// String that represents from which device type the message was sent
    /// </summary>
    public string DeviceType { get; private set; }

    /// <summary>
    /// Indicates if the message was forwarded
    /// </summary>
    public bool IsForwarded { get; private set; }

    /// <summary>
    /// Indicates how many times the message was forwarded.
    /// The maximum value is 127.
    /// </summary>
    public int? ForwardingScore { get; private set; }

    /// <summary>
    /// Indicates if the message is a status update
    /// </summary>
    public bool IsStatus { get; private set; }

    /// <summary>
    /// Indicates if the message was starred
    /// </summary>
    public bool IsStarred { get; private set; }

    /// <summary>
    /// Indicates if the message was a broadcast
    /// </summary>
    public bool Broadcast { get; private set; }

    /// <summary>
    /// Indicates if the message was sent as a reply to another message.
    /// </summary>
    public bool HasQuotedMsg { get; private set; }

    /// <summary>
    /// Indicates whether there are reactions to the message
    /// </summary>
    public bool HasReaction { get; private set; }

    /// <summary>
    /// Indicates the duration of the message in seconds
    /// </summary>
    public string? Duration { get; private set; }

    /// <summary>
    /// Location information contained in the message, if the message is type "location"
    /// </summary>
    public Location? Location { get; private set; }

    /// <summary>
    /// List of vCards contained in the message.
    /// </summary>
    public List<VCard>? VCards { get; private set; }

    /// <summary>
    /// Group Invite Data
    /// </summary>
    public InviteV4? InviteV4 { get; private set; }

    /// <summary>
    /// Indicates the mentions in the message body.
    /// </summary>
    public List<string> MentionedIds { get; private set; } = [];

    /// <summary>
    /// Indicates whether there are group mentions in the message body
    /// </summary>
    public List<GroupMention> GroupMentions { get; private set; } = [];

    /// <summary>
    /// Order ID for message type ORDER
    /// </summary>
    public string? OrderId { get; private set; }

    /// <summary>
    /// Order Token for message type ORDER
    /// </summary>
    public string? Token { get; private set; }

    /// <summary>
    /// Indicates whether the message is a Gif
    /// </summary>
    public bool IsGif { get; private set; }

    /// <summary>
    /// Indicates if the message will disappear after it expires
    /// </summary>
    public bool IsEphemeral { get; private set; }


    /// <summary>
    /// Links included in the message.
    /// Array of {link: string, isSuspicious: boolean}
    /// </summary>
    public List<dynamic> Links { get; private set; }

    public string? Title { get; private set; }

    public string? Description { get; private set; }

    public string? BusinessOwnerJid { get; private set; }

    public string? ProductId { get; private set; }

    public DateTime? LatestEditSenderTimestampMs { get; private set; }

    public MessageId? LatestEditMsgKey { get; private set; }

    public dynamic? DynamicReplyButtons { get; private set; }

    public string? SelectedButtonId { get; private set; }

    public string? SelectedRowId { get; private set; }

    public Poll Poll { get; private set; }

    public List<UserId> Recipients { get; private set; } = [];

    public List<string> TemplateParams { get; private set; } = [];

    private Message(dynamic data)
    {
        Patch(data);
    }

    private void Patch(dynamic data)
    {
        Id = new MessageId(data.id);
        Ack = data.ack;
        MediaKey = data.mediaKey;
        HasMedia = data.mediaKey != null && data.mediaKey.Type != JTokenType.Null
                && data.directPath != null && data.directPath.Type != JTokenType.Null;

        Body = HasMedia ? data.caption ?? string.Empty : data.body ?? data.pollName ?? string.Empty;
        Type = data.type;
        Timestamp = Util.ConvertToDate(data.t);
        From = UserId.Create(data.from);
        To = UserId.Create(data.to);
        Author = UserId.Create(data.author);
        DeviceType = Id.Id.Length > 21
            ? "android"
            : Id.Id.Substring(0, 2) == "3A" ? "ios" : "web";
        IsForwarded = data.isForwarded ?? false;
        ForwardingScore = data.forwardingScore ?? 0;
        IsStatus = data.isStatusV3 ?? Id.Remote.Id == "status@broadcast";
        IsStarred = data.star ?? false;
        Broadcast = data.broadcast ?? false;
        HasQuotedMsg = data.quotedMsg != null;
        HasReaction = data.hasReaction ?? false;
        Duration = data.duration ?? null;
        Location = data.type == MessageTypes.LOCATION
            ? new Location(data)
            : null;
        VCards = data.type == MessageTypes.CONTACT_CARD_MULTI
            ? ((JArray)data.vcardList).Select((Func<dynamic, VCard>)(c => new VCard(c.vcard))).ToList()
            : data.type == MessageTypes.CONTACT_CARD
                ? [new VCard(data.body)]
                : null;
        InviteV4 = data.type == MessageTypes.GROUP_INVITE
            ? new InviteV4(data)
            : null;
        MentionedIds = data.mentionedJidList is not null && data.mentionedJidList.Type != JTokenType.Null
            ? ((JArray)data.mentionedJidList).ToObject<string[]>().ToList()
            : [];
        GroupMentions = data.groupMentions.Type is not null && data.groupMentions.Type != JTokenType.Null
            ? ((JArray)data.groupMentions).ToObject<dynamic[]>().Select(i => new GroupMention(i)).ToList()
            : [];
        OrderId = data.orderId ?? null;
        Token = data.token ?? null;
        IsGif = data.isGif ?? false;
        IsEphemeral = data.isEphemeral ?? false;
        Title = data.title ?? null;
        Description = data.description ?? null;
        BusinessOwnerJid = data.businessOwnerJid ?? null;
        ProductId = data.productId ?? null;
        LatestEditSenderTimestampMs = Util.ConvertToDate(data.latestEditSenderTimestampMs);
        LatestEditMsgKey = data.latestEditMsgKey is not null && data.latestEditMsgKey.Type != JTokenType.Null
            ? new MessageId(data.latestEditMsgKey)
            : null;
        Links = data.links is not null ? ((JArray)data.links).ToObject<dynamic[]>().ToList() : [];
        DynamicReplyButtons = data.dynamicReplyButtons ?? null;
        SelectedButtonId = data.selectedButtonId ?? null;
        SelectedRowId = data.listResponse?.singleSelectReply?.selectedRowId ?? null;
        Recipients = data.recipients is not null
            ? ((JArray)data.recipients).Select(r => UserId.Create(r)).ToList()
            : [];
        TemplateParams = data.templateParams is not null
            ? ((JArray)data.templateParams).Select(t => t.ToString()).ToList()
            : [];
        if (Type == MessageTypes.POLL_CREATION)
        {
            Poll = new Poll(data.pollName, data.pollOptions, new PollSendOptions()
            {
                AllowMultipleAnswers = !data.pollSelectableOptionsCount,
                PollInvalidated = data.pollInvalidated,
                IsSentCagPollCreation = data.isSentCagPollCreation
            });
        }
    }

    public override string ToString()
    {
        return $"{From} : {Body}";
    }

    public static Message? Create(dynamic? data)
    {
        if (data is null) return null;
        if (data.Type == JTokenType.Null) return null;
        if (data is string) return null;
        try
        {
            return new Message(data);
        }
        catch (Exception e)
        {
            throw new ExceptionDataDeserialization(data, e);
        }
       
    }
}