namespace Whatsapp.web.net.LoginWebCache;

// No additional functionality needed, inherits everything from BaseAuthStrategy
public class LoginWebNullCacheService(PuppeteerOptions puppeteerOptions, LoginWebCacheOptions loginWebCacheOptions)
    : LoginWebCacheBaseService(puppeteerOptions, loginWebCacheOptions)
{
    protected override string CalculateUserDataDir()
    {
        return GetDataPath();
    }

    protected override ILoginWebCache CreateLoginWebCache()
    {
        return new LoginWebNullCache();
    }
}