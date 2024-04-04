namespace Whatsapp.web.net;

public class RemoteLoginWebCache : LoginWebCache
{
    private readonly string _remotePath;
    private readonly bool _strict;

    public RemoteLoginWebCache(string remotePath, bool strict)
    {
        if (string.IsNullOrEmpty(remotePath))
        {
            throw new ArgumentException("webVersionCache.remotePath is required when using the remote cache");
        }

        _remotePath = remotePath;
        _strict = strict;
    }

    public override async Task<string> Resolve(string version)
    {
        var remotePath = _remotePath.Replace("{version}", version);

        try
        {
            using var httpClient = new HttpClient();
            var cachedRes = await httpClient.GetAsync(remotePath);
            if (cachedRes.IsSuccessStatusCode)
            {
                return await cachedRes.Content.ReadAsStringAsync();
            }
        }
        catch (Exception err)
        {
            Console.Error.WriteLine($"Error fetching version {version} from remote: {err.Message}");
        }

        if (_strict)
        {
            throw new Exception($"Couldn't load version {version} from the archive");
        }

        return null;
    }

    public override Task Persist(string versionContent, string version)
    {
        // Nothing to do here
        return Task.CompletedTask;
    }
}