using Newtonsoft.Json.Linq;

namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a Reaction on WhatsApp
/// </summary>
public class Reaction
{
    public MessageId Id { get; private set; }

    public MessageAck? Ack { get; private set; }

    /// <summary>
    /// MsgKey
    /// </summary>
    public MsgKey Key { get; private set; }

    public MsgKey ParentKey { get; private set; }

    /// <summary>
    /// Orphan
    /// </summary>
    public int Orphan { get; private set; }

    /// <summary>
    /// Unix timestamp for when the reaction was created
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Reaction
    /// </summary>
    public string Text { get; private set; }

    /// <summary>
    /// Read
    /// </summary>
    public bool Read { get; private set; }

    /// <summary>
    /// Sender ID
    /// </summary>
    public UserId SenderId { get; private set; }

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
        Ack = data.ack is not null && data.ack.Type != JTokenType.Null ? Enum.Parse(typeof(MessageAck), data.ack.ToString()) : null;
        Key = new MsgKey(data.msgKey);
        ParentKey = new MsgKey(data.parentMsgKey);
        SenderId = UserId.Create(data.senderUserJid);
        Text = data.reactionText;
        Timestamp = Util.ConvertToDate(data.timestamp);
        Orphan = data.orphan;
        Read = data.read ?? false;
    }
}