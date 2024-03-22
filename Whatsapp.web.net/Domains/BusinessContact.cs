namespace Whatsapp.web.net.Domains;

public class BusinessContact : Contact
{
    private BusinessContact() 
    {
    }

    public static BusinessContact CreateBusinessContact( dynamic data)
    {
        var contact = new BusinessContact();
        contact.Patch(data);
        return contact;
    }
}