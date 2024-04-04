namespace Whatsapp.web.net.EventArgs;

public class AuthenticationFailureEventArgs : DispatcherEventArg
{
    public object Payload { get; }

    public AuthenticationFailureEventArgs(object payload)
        : base(DispatcherEventsType.AUTHENTICATION_FAILURE)
    {
        Payload = payload;
    }
}