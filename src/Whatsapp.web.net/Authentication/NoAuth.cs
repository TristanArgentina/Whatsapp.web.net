namespace Whatsapp.web.net.Authentication;

// No additional functionality needed, inherits everything from BaseAuthStrategy
public class NoAuth(PuppeteerOptions puppeteerOptions, WebVersionCache webVersionCache)
    : BaseAuthStrategy(puppeteerOptions, webVersionCache)
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