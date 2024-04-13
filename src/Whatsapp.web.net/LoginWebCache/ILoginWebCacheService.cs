namespace Whatsapp.web.net.LoginWebCache;

public interface ILoginWebCacheService
{
    string? UserDataDir { get; }

    Task BeforeBrowserInitialized();
    
    Task AfterBrowserInitialized();

    Task AfterAuthReady();
    
    Task Disconnect();
    
    Task Destroy();
    
    Task Logout();

    ILoginWebCache Get();
}