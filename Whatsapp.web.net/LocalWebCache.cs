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
        if (!Directory.Exists(_path))
        {
            Directory.CreateDirectory(_path);
        }
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

    public override async Task Persist(string indexHtml, string version)
    {

        var filePath = Path.Combine(_path, $"{version.Replace("JSHandle:","")}.html");
        Directory.CreateDirectory(_path);
        await File.WriteAllTextAsync(filePath, indexHtml);
    }
}