namespace Whatsapp.web.net;

public class WebCache
{
    public virtual Task<string> Resolve(string version)
    {
        return Task.FromResult(string.Empty);
    }

    public virtual Task Persist(string versionContent)
    {
        return Task.CompletedTask;
    }
}