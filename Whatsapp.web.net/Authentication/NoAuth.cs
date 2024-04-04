namespace Whatsapp.web.net.Authentication;

public class NoAuth : BaseAuthStrategy
{
    protected override string PrefixFileName => throw new NotImplementedException();

    // No additional functionality needed, inherits everything from BaseAuthStrategy
    public NoAuth(PuppeteerOptions puppeteerOptions, WebVersionCache webVersionCache)
        : base(puppeteerOptions, webVersionCache)
    {
        LoginWebCache = new LoginWebCache();
    }

    protected override string CalculateUserDataDir()
    {
        return GetDataPath();
    }

    protected override ILoginWebCache CreateLoginWebCache()
    {
        return new LoginWebCache();
    }
}