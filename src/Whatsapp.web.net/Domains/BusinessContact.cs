namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a Business Contact on WhatsApp
/// </summary>
public class BusinessContact : Contact
{
    /// <summary>
    /// The contact's business profile
    /// </summary>
    public object BusinessProfile { get; set; }

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