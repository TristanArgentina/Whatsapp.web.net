using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class MessageEditEventArgs : DispatcherEventArg
{
    public Message Message { get; }
    public string NewBody { get; }
    public string PrevBody { get; }

    public MessageEditEventArgs(Message message, string newBody, string prevBody)
        :base( DispatcherEventsType.MESSAGE_EDIT)
    {
        Message = message;
        NewBody = newBody;
        PrevBody = prevBody;
    }
}