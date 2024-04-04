using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class RevokedEveryoneEventArg : DispatcherEventArg
{
    public Message Message { get; }

    public Message? RevokedMsg { get; }

    public RevokedEveryoneEventArg(Message message, Message? revokedMsg)
        : base(DispatcherEventsType.MESSAGE_REVOKED_EVERYONE)
    {
        Message = message;
        RevokedMsg = revokedMsg;
    }
}