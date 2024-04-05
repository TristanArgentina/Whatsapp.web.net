using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class MessageChangeEventArgs : DispatcherEventArg
{
    public Message Message { get; }

    public MessageChangeEventArgs(Message message)
        : base(DispatcherEventsType.MESSAGE_CREATE)
    {
        Message = message;
    }
}