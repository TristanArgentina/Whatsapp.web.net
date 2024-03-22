using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class RevokedMeEventArg : DispatcherEventArg
{
    public Message Message { get; }

    public RevokedMeEventArg(Message message) : base(DispatcherEventsType.MESSAGE_REVOKED_ME)
    {
        Message = message;
    }
}