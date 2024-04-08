namespace Whatsapp.web.net.Domains;

public class Participant
{
    public int StatusCode { get; private set; }
    public string Message { get; private set; }
    public bool IsGroupCreator { get; private set; }
    public bool IsInviteV4Sent { get; private set; }
}