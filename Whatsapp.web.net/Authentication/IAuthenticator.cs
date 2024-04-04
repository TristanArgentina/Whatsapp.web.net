namespace Whatsapp.web.net.Authentication;

public interface IAuthenticator
{
    Task BeforeBrowserInitialized();
    
    Task AfterBrowserInitialized();
    
    Task<(bool Failed, bool Restart, object FailureEventPayload)> OnAuthenticationNeeded();
    
    Task<object> GetAuthEventPayload();
    
    Task AfterAuthReady();
    
    Task Disconnect();
    
    Task Destroy();
    
    Task Logout();
    string? UserDataDir { get; }

    ILoginWebCache LoginWebCache { get; }
}