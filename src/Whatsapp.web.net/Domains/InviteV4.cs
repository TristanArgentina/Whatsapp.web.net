namespace Whatsapp.web.net.Domains;

public class InviteV4
{

    public string InviteCode { get; private set; }

    public long InviteCodeExp { get; private set; }

    public string GroupId { get; private set; }

    public string GroupName { get; private set; }

    public UserId FromId { get; private set; }

    public UserId ToId { get; private set; }

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

}