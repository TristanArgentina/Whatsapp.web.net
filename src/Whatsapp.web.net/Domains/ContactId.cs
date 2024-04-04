namespace Whatsapp.web.net.Domains;

public class ContactId : UserId
{
    public ContactId(string user, string server) : base(user, server)
    {
    }

    private ContactId()
    {
    
    }

    public new static ContactId Create(dynamic data)
    {
        var contactId = new ContactId();
        contactId.Patch(data);
        return contactId;
    }
}