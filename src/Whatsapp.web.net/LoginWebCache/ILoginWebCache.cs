namespace Whatsapp.web.net.LoginWebCache;

public interface ILoginWebCache
{
    Task<string?> Resolve(string version);

    Task Persist(string versionContent, string version);
}