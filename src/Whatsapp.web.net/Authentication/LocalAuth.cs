namespace Whatsapp.web.net.Authentication;

public class LocalAuth : BaseAuthStrategy
{
    protected override string PrefixFileName => "session";

    public LocalAuth(PuppeteerOptions puppeteerOptions, WebVersionCache webVersionCache)
        : base(puppeteerOptions, webVersionCache)
    {
    }

    protected override ILoginWebCache CreateLoginWebCache()
    {
        return new LocalLoginWebCache(UserDataDir, WebVersionCache.Strict);
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