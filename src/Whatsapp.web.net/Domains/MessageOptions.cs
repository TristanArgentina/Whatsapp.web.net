namespace Whatsapp.web.net.Domains;

public class MessageOptions
{
    /// <summary>
    /// Show links preview. Has no effect on multi-device accounts.
    /// </summary>
    public bool LinkPreview { get; set; } = true;

    /// <summary>
    /// Send audio as voice message with a generated waveform
    /// </summary>
    public bool SendAudioAsVoice { get; set; }

    /// <summary>
    /// Send video as gif
    /// </summary>
    public bool SendVideoAsGif { get; set; }

    /// <summary>
    /// Send media as a sticker
    /// </summary>
    public bool SendMediaAsSticker { get; set; }

    /// <summary>
    /// Send media as a document
    /// </summary>
    public bool SendMediaAsDocument { get; set; }

    /// <summary>
    /// Send photo/video as a view once message
    /// </summary>
    public bool IsViewOnce { get; set; }

    /// <summary>
    /// Automatically parse vCards and send them as contacts
    /// </summary>
    public bool ParseVCards { get; set; } = true;

    /// <summary>
    /// Image or video caption
    /// </summary>
    public string? Caption { get; set; } = "";

    /// <summary>
    /// Id of the message that is being quoted (or replied to)
    /// </summary>
    public MessageId? QuotedMessageId { get; set; }

    /// <summary>
    ///  User IDs to mention in the message
    /// </summary>
    public List<string>? Mentions { get; set; }

    /// <summary>
    ///  An array of object that handle group mentions
    /// </summary>
    public List<GroupMention>? GroupMentions { get; set; }

    /// <summary>
    /// Mark the conversation as seen after sending the message
    /// </summary>
    public bool SendSeen { get; set; } = true;

    /// <summary>
    ///  Media to be sent
    /// </summary>
    public MessageMedia? Media { get; set; }


    public object? Extra { get; set; }

    /// <summary>
    /// Sets the name of the sticker, (if sendMediaAsSticker is true).
    /// </summary>
    public string? StickerName { get; set; }

    /// <summary>
    /// Sets the author of the sticker, (if sendMediaAsSticker is true).
    /// </summary>
    public string? StickerAuthor { get; set; }

    /// <summary>
    /// Sets the categories of the sticker, (if sendMediaAsSticker is true). Provide emoji char array, can be null.
    /// </summary>
    public List<string>? StickerCategories { get; set; }
}