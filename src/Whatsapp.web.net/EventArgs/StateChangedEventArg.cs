namespace Whatsapp.web.net.EventArgs;

public class StateChangedEventArg : DispatcherEventArg
{
    public dynamic State { get; }

    public StateChangedEventArg(dynamic state) : base(DispatcherEventsType.STATE_CHANGED)
    {
        State = state;
    }
}