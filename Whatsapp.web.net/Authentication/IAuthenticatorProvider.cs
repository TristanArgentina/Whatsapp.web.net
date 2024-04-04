namespace Whatsapp.web.net.Authentication;

public interface IAuthenticatorProvider
{
    IAuthenticator GetAuthenticator();
}