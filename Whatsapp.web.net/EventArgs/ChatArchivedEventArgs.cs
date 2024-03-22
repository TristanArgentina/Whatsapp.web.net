using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class ChatArchivedEventArgs : DispatcherEventArg
{
    public Chat Chat { get; }
    public bool CurrState { get; }
    public bool PrevState { get; }

    public ChatArchivedEventArgs(Chat chat, bool currState, bool prevState)
        : base(DispatcherEventsType.CHAT_ARCHIVED)
    {
        Chat = chat;
        CurrState = currState;
        PrevState = prevState;
    }
}