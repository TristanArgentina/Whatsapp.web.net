using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class MessageReceivedEventArgs : DispatcherEventArg
{
    /// <summary>
    /// Transport message.
    /// </summary>
    public Message Message { get; }

    public MessageReceivedEventArgs(Message message) 
        : base(DispatcherEventsType.MESSAGE_RECEIVED)
    {
        Message = message;
    }
}