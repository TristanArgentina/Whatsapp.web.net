using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class ChatArchivedEventArgs : DispatcherEventArg
{
    public string ChatId { get; }
    public bool CurrState { get; }
    public bool PrevState { get; }

    public ChatArchivedEventArgs(string chatId, bool currState, bool prevState)
        : base(DispatcherEventsType.CHAT_ARCHIVED)
    {
        ChatId = chatId;
        CurrState = currState;
        PrevState = prevState;
    }
}