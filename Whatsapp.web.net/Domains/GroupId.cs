namespace Whatsapp.web.net.Domains;

public class GroupId : UserId
{
    public string Id { get; set; }

    public string FromId { get; set; }

    public GroupId(string user, string server): base(user, server)
    {
        SetId();
    }

    private void SetId()
    {
        if (!User.Contains('-')) return;
        var userSplit = User.Split('-');

        FromId = userSplit[0];
        Id = userSplit[1];
    }

    private GroupId()
    {
    }

    public new static GroupId Create(dynamic data)
    {
        var contactId = new GroupId();
        contactId.Patch(data);
        contactId.SetId();
        return contactId;
    }
}