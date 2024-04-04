namespace Whatsapp.web.net.Domains;

public class InviteV4
{
    public InviteV4(dynamic? data)
    {
        Patch(data);
    }

    private void Patch(dynamic? data)
    {
        if (data is null) return;
        InviteCode = data.inviteCode;
        InviteCodeExp = data.inviteCodeExp;
        GroupId = data.inviteGrp;
        GroupName = data.inviteGrpName;
        FromId = UserId.Create(data.from);
        ToId = UserId.Create(data.to);
    }

    public string InviteCode { get; set; }

    public long InviteCodeExp { get; set; }
    
    public string GroupId { get; set; }
    
    public string GroupName { get; set; }
    
    public UserId FromId { get; set; }
    
    public UserId ToId { get; set; }
}