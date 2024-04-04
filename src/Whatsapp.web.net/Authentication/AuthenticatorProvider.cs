namespace Whatsapp.web.net.Authentication;

public class AuthenticatorProvider : IAuthenticatorProvider
{
    private readonly WebVersionCache _webVersionCache;
    private readonly PuppeteerOptions _puppeteerOptions;
    private readonly IRemoteStore? _remoteStore;

    public AuthenticatorProvider(WebVersionCache webVersionCache, PuppeteerOptions puppeteerOptions,
        IRemoteStore? remoteStore = null)
    {
        _webVersionCache = webVersionCache;
        _puppeteerOptions = puppeteerOptions;
        _remoteStore = remoteStore;
    }

    public IAuthenticator GetAuthenticator()
    {
        return _webVersionCache.Type switch
        {
            "local" => new LocalAuth(_puppeteerOptions, _webVersionCache),
            "remote" => new RemoteAuth(_remoteStore, _puppeteerOptions, _webVersionCache),
            _ => new NoAuth(_puppeteerOptions, _webVersionCache)
        };
    }
}