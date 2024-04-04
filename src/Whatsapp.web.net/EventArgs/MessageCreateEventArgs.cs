using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class MessageCreateEventArgs : DispatcherEventArg
{
    public Message Message { get; }

    public MessageCreateEventArgs(Message message)
        : base(DispatcherEventsType.MESSAGE_CREATE)
    {
        Message = message;
    }
}