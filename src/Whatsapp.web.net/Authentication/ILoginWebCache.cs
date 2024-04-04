namespace Whatsapp.web.net.Authentication;

public interface ILoginWebCache
{
    Task<string> Resolve(string version);

    Task Persist(string versionContent, string version);
}