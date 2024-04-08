namespace Whatsapp.web.net;

/// <summary>
/// Options for searching messages. Right now only limit and fromMe is supported.
/// </summary>
public class SearchOptions
{
    /// <summary>
    /// Return only messages from the bot number or vise versa.
    /// To get all messages, leave the option undefined.
    /// </summary>
    public bool? FromMe { get; set; }

    /// <summary>
    /// The amount of messages to return.
    /// If no limit is specified, the available messages will be returned.
    /// Note that the actual number of returned messages may be smaller if there aren't enough messages in the conversation.
    /// Set this to Infinity to load all messages.
    /// </summary>
    public int? Limit { get; set; } = 1000;
}