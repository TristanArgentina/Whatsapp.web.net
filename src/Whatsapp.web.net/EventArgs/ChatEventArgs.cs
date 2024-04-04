using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class ChatEventArgs : DispatcherEventArg
{
    public Chat Chat { get; }
    public bool? CurrState { get; }
    public bool? PrevState { get; }

    public ChatEventArgs(DispatcherEventsType dispatcherEventsType, Chat chat, bool? currState = null, bool? prevState = null)
        : base(dispatcherEventsType)
    {
        Chat = chat;
        CurrState = currState;
        PrevState = prevState;
    }
}