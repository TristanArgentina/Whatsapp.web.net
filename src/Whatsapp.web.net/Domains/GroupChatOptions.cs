namespace Whatsapp.web.net.Domains;

public class GroupChatOptions
{
    public int MessageTimer { get; set; } = 0;
    
    public string ParentGroupId { get; set; }
    
    public bool AutoSendInviteV4 { get; set; } = true;
    
    public string Comment { get; set; } = "";
}