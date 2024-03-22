namespace Whatsapp.web.net.Domains;

public class Call
{
    public string Id { get; set; }
    public string From { get; set; }
    public long Timestamp { get; set; }
    public bool IsVideo { get; set; }
    public bool IsGroup { get; set; }
    public bool FromMe { get; set; }
    public bool CanHandleLocally { get; set; }
    public bool WebClientShouldHandle { get; set; }
    public object Participants { get; set; }

    public Call(dynamic data)
    {
        Patch(data);
    }

    private void Patch(dynamic? data)
    {
        if (data is null) return;
        Id = data.id;
        From = data.peerJid;
        Timestamp = data.offerTime;
        IsVideo = data.isVideo;
        IsGroup = data.isGroup;
        FromMe = data.outgoing;
        CanHandleLocally = data.canHandleLocally;
        WebClientShouldHandle = data.webClientShouldHandle;
        Participants = data.participants;
    }

}