using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class IncomingCallEventArgs : DispatcherEventArg
{
    public Call Call { get; }

    public IncomingCallEventArgs(Call call)
        : base(DispatcherEventsType.INCOMING_CALL)
    {
        Call = call;
    }
}