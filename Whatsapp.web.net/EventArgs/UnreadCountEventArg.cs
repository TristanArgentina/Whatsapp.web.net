namespace Whatsapp.web.net.EventArgs;

public class UnreadCountEventArg : DispatcherEventArg
{
    public string ChatId { get; }

    public UnreadCountEventArg(string chatId) : base(DispatcherEventsType.UNREAD_COUNT)
    {
        ChatId = chatId;
    }

    public override string ToString()
    {
        return $"Chat: \n {ChatId}";
    }
}