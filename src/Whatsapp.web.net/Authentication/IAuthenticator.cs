namespace Whatsapp.web.net.Authentication;

public interface IAuthenticator
{
    string? UserDataDir { get; }

    Task BeforeBrowserInitialized();
    
    Task AfterBrowserInitialized();

    Task AfterAuthReady();
    
    Task Disconnect();
    
    Task Destroy();
    
    Task Logout();

    ILoginWebCache LoginWebCache { get; }
}