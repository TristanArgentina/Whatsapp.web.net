using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class AuthenticatedEventArg(ClientInfo info) 
    : DispatcherEventArg(DispatcherEventsType.AUTHENTICATED)
{
    public ClientInfo Info { get; } = info;
}