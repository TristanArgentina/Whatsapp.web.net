namespace Whatsapp.web.net.Domains;

public class PrivateChat : Chat
{
    public PrivateChat(dynamic? data)
    {
        Patch(data);
    }
}