namespace Whatsapp.web.net.EventArgs;

public class ChatRemovedEventArgs : DispatcherEventArg
{
    public string ChatId { get; }

    public ChatRemovedEventArgs(string chatId)
        : base(DispatcherEventsType.CHAT_REMOVED)
    {
        ChatId = chatId;
    }
}