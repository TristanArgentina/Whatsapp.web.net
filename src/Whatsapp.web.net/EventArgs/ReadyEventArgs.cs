namespace Whatsapp.web.net.EventArgs;

public class ReadyEventArgs : DispatcherEventArg
{
    public ReadyEventArgs()
        : base(DispatcherEventsType.READY)
    {
    }
}