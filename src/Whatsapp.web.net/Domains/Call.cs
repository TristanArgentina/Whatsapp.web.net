namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a Call on WhatsApp
/// </summary>
public class Call
{
    /// <summary>
    /// Call ID
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// From
    /// </summary>
    public string From { get; private set; }

    /// <summary>
    /// Unix timestamp for when the call was created
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Is video
    /// </summary>
    public bool IsVideo { get; private set; }

    /// <summary>
    /// Is Group
    /// </summary>
    public bool IsGroup { get; private set; }

    /// <summary>
    /// Indicates if the call was sent by the current user
    /// </summary>
    public bool FromMe { get; private set; }

    /// <summary>
    /// Indicates if the call can be handled in waweb
    /// </summary>
    public bool CanHandleLocally { get; private set; }

    /// <summary>
    /// Indicates if the call Should be handled in waweb
    /// </summary>
    public bool WebClientShouldHandle { get; private set; }

    /// <summary>
    /// Object with participants
    /// </summary>
    public object Participants { get; private set; }

    public Call(dynamic data)
    {
        Patch(data);
    }

    private void Patch(dynamic? data)
    {
        if (data is null) return;
        Id = data.id;
        From = data.peerJid;
        Timestamp = DateTimeOffset.FromUnixTimeSeconds((long)data.offerTime).UtcDateTime;
        IsVideo = data.isVideo;
        IsGroup = data.isGroup;
        FromMe = data.outgoing;
        CanHandleLocally = data.canHandleLocally;
        WebClientShouldHandle = data.webClientShouldHandle;
        Participants = data.participants;
    }

}