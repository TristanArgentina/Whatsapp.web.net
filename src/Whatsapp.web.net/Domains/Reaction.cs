namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a Reaction on WhatsApp
/// </summary>
public class Reaction
{
    /// <summary>
    /// Reaction ID
    /// </summary>
    public MsgKey Key { get; set; }

    public MsgKey ParentKey { get; set; }

    /// <summary>
    /// Orphan
    /// </summary>
    public int Orphan { get; set; }

    /// <summary>
    /// Unix timestamp for when the reaction was created
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Reaction
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Read
    /// </summary>
    public bool Read { get; set; }

    /// <summary>
    /// Sender ID
    /// </summary>
    public UserId SenderId { get; set; }

    public Reaction(dynamic data)
    {
        if (data is not null)
        {
            Patch(data);
        }
    }

    private void Patch(dynamic data)
    {
        Key = new MsgKey(data.msgKey);
        ParentKey = new MsgKey(data.parentMsgKey);
        SenderId = UserId.Create(data.senderId);
        Text = data.reactionText;
        Timestamp = Util.ConvertToDate( data.timestamp);
        Orphan = data.orphan;
        Read = data.read;
    }
}