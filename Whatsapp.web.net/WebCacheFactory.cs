namespace Whatsapp.web.net;

public static class WebCacheFactory
{
    public static WebCache CreateWebCache(string type, WebVersionCache options)
    {
        switch (type)
        {
            case "remote":
                return new RemoteWebCache(options.RemotePath, options.Strict);
            case "local":
                return new LocalWebCache(options.LocalPath, options.Strict);
            case "none":
                return new WebCache();
            default:
                throw new Exception($"Invalid WebCache type {type}");
        }
    }
}