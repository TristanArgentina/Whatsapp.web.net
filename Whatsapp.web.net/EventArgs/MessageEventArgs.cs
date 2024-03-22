using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class MessageEventArgs : DispatcherEventArg
{
    public Message Msg { get; }
    public MessageAck MessageAsk { get; }
    public string? NewBody { get; }
    public string? PrevBody { get; }

    public MessageEventArgs(DispatcherEventsType dispatcherEventsType, Message msg, string? newBody = null, string? prevBody = null)
        : base(dispatcherEventsType)
    {
        Msg = msg;
        NewBody = newBody;
        PrevBody = prevBody;
    }

    public MessageEventArgs(DispatcherEventsType dispatcherEventsType, Message msg, MessageAck messageAsk) : base(dispatcherEventsType)
    {
        Msg = msg;
        MessageAsk = messageAsk;
    }
}