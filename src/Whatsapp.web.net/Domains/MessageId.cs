using Newtonsoft.Json.Linq;

namespace Whatsapp.web.net.Domains;

public class MessageId
{
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
    public UserId Remote { get; private set; }

    public UserId? Participant { get; private set; }

    public string _serialized { get; private set; }


    public MessageId(dynamic data)
    {
        Patch(data);
    }

    private void Patch(dynamic? data)
    {
        if (data is null || data.Type == JTokenType.Null) return;
        Id = data.id;
        FromMe = data.fromMe;
        Remote = UserId.Create(data.remote);
        Participant = UserId.Create(data.participant);
        _serialized = data._serialized ?? Id;
    }

   
}