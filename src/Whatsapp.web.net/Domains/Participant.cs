namespace Whatsapp.web.net.Domains;

public class Participant
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public bool IsGroupCreator { get; set; }
    public bool IsInviteV4Sent { get; set; }
}