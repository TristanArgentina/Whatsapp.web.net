namespace Whatsapp.web.net.Domains;

public class PrivateContact : Contact
{
    private PrivateContact()
    {
    }

    public static PrivateContact CreatePrivateContact(dynamic data)
    {
        var contact = new PrivateContact();
        contact.Patch(data);
        return contact;
    }
}