namespace Whatsapp.web.net.LoginWebCache;

public class LoginWebCacheProvider(
    LoginWebCacheOptions loginWebCacheOptions,
    PuppeteerOptions puppeteerOptions,
    IRemoteStore? remoteStore = null)
    : ILoginWebCacheProvider
{
    public ILoginWebCacheService Get()
    {
        return loginWebCacheOptions.Type switch
        {
            "local" => new LoginWebLocalCacheService(puppeteerOptions, loginWebCacheOptions),
            "remote" => new LoginWebRemoteCacheService(remoteStore, puppeteerOptions, loginWebCacheOptions),
            _ => new LoginWebNullCacheService(puppeteerOptions, loginWebCacheOptions)
        };
    }
}