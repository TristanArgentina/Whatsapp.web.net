namespace Whatsapp.web.net.Domains;

public class GroupJid
{
    public string Server { get; private set; }

    public string User { get; private set; }


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


    public static GroupJid? Create(dynamic? data)
    {
        return new GroupJid(data);
    }
}