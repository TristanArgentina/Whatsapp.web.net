using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class MessageACKEventArg : DispatcherEventArg
{
    public Message Message { get; }
    public MessageAck MessageAsk { get; }

    public MessageACKEventArg(Message message, MessageAck messageAsk) : base(DispatcherEventsType.MESSAGE_ACK)
    {
        Message = message;
        MessageAsk = messageAsk;
    }
}