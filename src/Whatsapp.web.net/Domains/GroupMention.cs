namespace Whatsapp.web.net.Domains;

public class GroupMention
{
    public GroupMention(dynamic data)
    {
        Patch(data);
    }

    public string Subject { get; private set; }

    public string Id { get; private set; }

    public GroupId GroupJid { get; private set; }

    private void Patch(dynamic data)
    {
        if (data == null) return;

        Subject = data.subject;
        Id = data.id;
        GroupJid = GroupId.Create(data.groupJid);
    }
}