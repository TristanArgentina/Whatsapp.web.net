namespace Whatsapp.web.net.Domains;

public class ReplayOptions
{
    public bool LinkPreview { get; set; }
    public bool SendAudioAsVoice { get; set; }
    public bool SendVideoAsGif { get; set; }
    public bool SendMediaAsSticker { get; set; }
    public bool SendMediaAsDocument { get; set; }
    public bool IsViewOnce { get; set; }
    public bool ParseVCards { get; set; }
    public string? Caption { get; set; }
    public MessageId? QuotedMessageId { get; set; }
    public List<object>? Mentions { get; set; }
    public List<GroupMention>? GroupMentions { get; set; }
    public bool SendSeen { get; set; }
    public MessageMedia? Media { get; set; }
    public object? Extra { get; set; }
    public string? StickerName { get; set; }
    public string? StickerAuthor { get; set; }
    public List<string>? StickerCategories { get; set; }
}