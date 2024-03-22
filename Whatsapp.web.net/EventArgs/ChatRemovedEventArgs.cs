using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class ChatRemovedEventArgs : DispatcherEventArg
{
    public Chat Chat { get; }

    public ChatRemovedEventArgs(Chat chat)
        : base(DispatcherEventsType.CHAT_REMOVED)
    {
        Chat = chat;
    }
}