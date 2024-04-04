using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class MessageReactionEventArgs : DispatcherEventArg
{
    public Reaction Reaction { get; }

    public MessageReactionEventArgs(Reaction reaction)
        : base(DispatcherEventsType.MESSAGE_REACTION)
    {
        Reaction = reaction;
    }
}