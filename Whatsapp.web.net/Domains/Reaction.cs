namespace Whatsapp.web.net.Domains;

public class Reaction
{
    /// <summary>
    /// Reaction ID
    /// </summary>
    public object Id { get; set; }

    /// <summary>
    /// Orphan
    /// </summary>
    public int Orphan { get; set; }

    /// <summary>
    /// Orphan reason
    /// </summary>
    public string OrphanReason { get; set; }

    /// <summary>
    /// Unix timestamp for when the reaction was created
    /// </summary>
    public long Timestamp { get; set; }

    /// <summary>
    /// Reaction
    /// </summary>
    public string Reaction1 { get; set; }

    /// <summary>
    /// Read
    /// </summary>
    public bool Read { get; set; }

    /// <summary>
    /// Message ID
    /// </summary>
    public object MsgId { get; set; }

    /// <summary>
    /// Sender ID
    /// </summary>
    public string SenderId { get; set; }

    /// <summary>
    /// ACK
    /// </summary>
    public int? Ack { get; set; }

    public Reaction(dynamic data)
    {
        if (data is not null)
        {
            Patch(data);
        }
    }

    private void Patch(dynamic data)
    {
        Id = data.id;
        Orphan = data.orphan;
        OrphanReason = data.orphanReason;
        Timestamp = data.timestamp;
        Reaction1 = data.reaction;
        Read = data.read;
        MsgId = data.msgid;
        SenderId = data.senderId;
        Ack = data.ack;
    }
}