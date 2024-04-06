namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a Reaction on WhatsApp
/// </summary>
public class Reaction
{
    public MessageId Id { get; set; }

    public MessageAck Ack { get; set; }

    /// <summary>
    /// MsgKey
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
        Id = new MessageId(data.id);
        Ack = Enum.Parse(typeof(MessageAck), data.ack.ToString());
        Key = new MsgKey(data.msgKey);
        ParentKey = new MsgKey(data.parentMsgKey);
        SenderId = UserId.Create(data.senderUserJid);
        Text = data.reactionText;
        Timestamp = Util.ConvertToDate(data.timestamp);
        Orphan = data.orphan;
        Read = data.read;
    }
}