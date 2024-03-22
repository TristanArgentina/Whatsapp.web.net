using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class ContactChangedEventArg : DispatcherEventArg
{
    public Message Message { get; }
    public string OldId { get; }
    public string NewId { get; }
    public bool IsContact { get; }

    public ContactChangedEventArg(Message message, string oldId, string newId, bool isContact)
        : base(DispatcherEventsType.CONTACT_CHANGED)
    {
        Message = message;
        OldId = oldId;
        NewId = newId;
        IsContact = isContact;
    }
}