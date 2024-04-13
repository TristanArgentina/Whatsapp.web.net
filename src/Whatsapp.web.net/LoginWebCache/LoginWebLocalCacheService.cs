namespace Whatsapp.web.net.LoginWebCache;

public class LoginWebLocalCacheService(PuppeteerOptions puppeteerOptions, LoginWebCacheOptions loginWebCacheOptions)
    : LoginWebCacheBaseService(puppeteerOptions, loginWebCacheOptions)
{
    protected override string PrefixFileName => "session";

    protected override ILoginWebCache CreateLoginWebCache()
    {
        return new LoginWebLocalCache(UserDataDir, LoginWebCacheOptions.Strict);
    }

    public override Task Logout()
    {
        if (!string.IsNullOrEmpty(UserDataDir))
        {
            if (Directory.Exists(UserDataDir))
            {
                Directory.Delete(UserDataDir, true);
            }
        }

        return Task.CompletedTask;
    }
}