using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class MessageCiphertextEventArgs : DispatcherEventArg
{
    public Message Message { get; }

    public MessageCiphertextEventArgs(Message message)
        : base(DispatcherEventsType.MESSAGE_CIPHERTEXT)
    {
        Message = message;
    }
}