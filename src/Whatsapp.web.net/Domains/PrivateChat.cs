namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a Private Chat on WhatsApp
/// </summary>
public class PrivateChat : Chat
{
    public PrivateChat(dynamic? data)
    {
        Patch(data);
    }
}