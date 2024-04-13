namespace Whatsapp.web.net;

public class StickerMetadata
{
    /// <summary>
    /// Sets the name of the sticker, (if sendMediaAsSticker is true).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Sets the author of the sticker, (if sendMediaAsSticker is true).
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Sets the categories of the sticker, (if sendMediaAsSticker is true). Provide emoji char array, can be null.
    /// </summary>
    public string[]? Categories { get; set; }
}