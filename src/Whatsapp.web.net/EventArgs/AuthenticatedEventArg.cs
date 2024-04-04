using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class AuthenticatedEventArg : DispatcherEventArg
{
    public ClientInfo Info { get; }
    public object? Payload { get; }

    public AuthenticatedEventArg(ClientInfo info, object? payload) 
        : base(DispatcherEventsType.AUTHENTICATED)
    {
        Info = info;
        Payload = payload;
    }
}