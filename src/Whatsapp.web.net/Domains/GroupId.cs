namespace Whatsapp.web.net.Domains;

public class GroupId : UserId
{
    public string FromId { get; set; }

    public GroupId(string user, string server) : base(user, server)
    {
        SetId();
    }

    private void SetId()
    {
        if (!User.Contains('-')) return;
        var userSplit = User.Split('-');
        if (userSplit.Length !=  2) return;
        FromId = userSplit[0];
        SetId($"{userSplit[1]}@{Server}");
    }

    private GroupId()
    {
    }

    public new static GroupId Create(dynamic data)
    {
        var groupId = new GroupId();
        groupId.Patch(data);
        groupId.SetId();
        return groupId;
    }
}