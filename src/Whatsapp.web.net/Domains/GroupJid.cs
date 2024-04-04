namespace Whatsapp.web.net.Domains;

public class GroupJid
{
    public GroupJid(dynamic data)
    {
        Patch(data);
    }

    private void Patch(dynamic? data)
    {
        if (data is null) return;
        Server = data.server;
        User = data.user;
    }

    public string Server { get; set; }

    public string User { get; set; }

    public static GroupJid? Create(dynamic? data)
    {
        return new GroupJid(data);
    }
}