using System.Text.RegularExpressions;

namespace Whatsapp.web.net;

public class LocalWebCache : WebCache
{
    private readonly string _path;
    private readonly bool _strict;

    public LocalWebCache(string path, bool strict = false)
    {
        _path = path;
        _strict = strict;
    }

    public override async Task<string> Resolve(string version)
    {
        var filePath = Path.Combine(_path, $"{version}.html");

        try
        {
            return await File.ReadAllTextAsync(filePath);
        }
        catch (Exception err)
        {
            if (_strict)
            {
                throw new Exception($"Couldn't load version {version} from the cache", err);
            }
            return null;
        }
    }

    public override async Task Persist(string indexHtml)
    {
        // extract version from index (e.g. manifest-2.2206.9.json -> 2.2206.9)
        var match = Regex.Match(indexHtml, @"manifest-([\d\\.]+)\.json");
        var version = match.Success ? match.Groups[1].Value : null;
        if (version == null) return;

        var filePath = Path.Combine(_path, $"{version}.html");
        Directory.CreateDirectory(_path);
        await File.WriteAllTextAsync(filePath, indexHtml);
    }
}