using System.Text;

namespace Whatsapp.web.net.Authentication;

public class LocalLoginWebCache : LoginWebCache
{
    private readonly string _path;
    private readonly bool _strict;

    public LocalLoginWebCache(string path, bool strict)
    {
        _path = path;
        _strict = strict;
    }

    public override async Task<string?> Resolve(string version)
    {
        if (!Directory.Exists(_path))
        {
            Directory.CreateDirectory(_path);
        }
        var filePath = Path.Combine(_path, $"{version}.html");
        if (File.Exists(filePath))
        {
            return await File.ReadAllTextAsync(filePath, Encoding.UTF8);
        }

        if (_strict)
        {
            throw new Exception($"Couldn't load version {version} from the cache");
        }
        return null;
    }

    public override async Task Persist(string indexHtml, string version)
    {
        var filePath = Path.Combine(_path, $"{version}.html");
        Directory.CreateDirectory(_path);
        await File.WriteAllTextAsync(filePath, indexHtml);
    }
}