namespace Whatsapp.web.net.EventArgs;

public class DispatcherEventArg : System.EventArgs
{
    public DispatcherEventArg(DispatcherEventsType dispatcherEventsType)
    {
        DispatcherEventsType = dispatcherEventsType;
    }

    public DispatcherEventsType DispatcherEventsType { get; }

}