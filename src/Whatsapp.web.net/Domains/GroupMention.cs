namespace Whatsapp.web.net.Domains;

public class GroupMention
{
    public GroupMention(dynamic data)
    {
        Patch(data);
    }

    public string Subject { get; set; }

    public string Id { get; set; }

    public GroupJid GroupJid { get; set; }

    private void Patch(dynamic data)
    {
        if (data == null) return;

        Subject = data.subject;
        Id = data.id;
        GroupJid = new GroupJid(data.groupJid);
    }
}