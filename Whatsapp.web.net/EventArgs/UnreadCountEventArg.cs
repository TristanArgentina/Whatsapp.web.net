using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class UnreadCountEventArg : DispatcherEventArg
{
    public Chat Chat { get; }

    public UnreadCountEventArg(Chat chat) : base(DispatcherEventsType.UNREAD_COUNT)
    {
        Chat = chat;
    }

    public override string ToString()
    {
        return $"Chat: \n {Chat}";
    }
}