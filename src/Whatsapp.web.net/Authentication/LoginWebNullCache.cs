namespace Whatsapp.web.net.Authentication;

public class LoginWebNullCache : ILoginWebCache
{
    public Task<string?> Resolve(string version)
    {
        return Task.FromResult<string?>(null);
    }

    public Task Persist(string versionContent, string version)
    {
        return Task.CompletedTask;
    }
}