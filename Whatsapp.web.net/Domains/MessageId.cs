namespace Whatsapp.web.net.Domains;

public class MessageId
{
    public MessageId(dynamic data)
    {
        Patch(data);
    }

    private void Patch(dynamic? data)
    {
        if (data is null) return;
        Id = data.id;
        FromMe = data.fromMe;
        Remote = UserId.Create(data.remote);
        Participant = UserId.Create(data.participant);
        Serialized = data._serialized ?? Id;

    }

    public string Serialized { get; set; }

    /// <summary>
    /// Indicates if the message was sent by the current user
    /// </summary>
    public bool FromMe { get; private set; }

    /// <summary>
    /// ID that represents the message
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public UserId Remote { get; set; }

    public UserId? Participant { get; set; }
}