using System.Text;

namespace Whatsapp.web.net.Authentication;

public class LoginWebLocalCache(string path, bool strict) 
    : ILoginWebCache
{
    public async Task<string?> Resolve(string version)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    
        var filePath = Path.Combine(path, $"{version}.html");
        
        if (File.Exists(filePath))
        {
            return await File.ReadAllTextAsync(filePath, Encoding.UTF8);
        }

        if (strict)
        {
            throw new Exception($"Couldn't load version {version} from the cache");
        }
        return null;
    }

    public async Task Persist(string indexHtml, string version)
    {
        var filePath = Path.Combine(path, $"{version}.html");
        Directory.CreateDirectory(path);
        await File.WriteAllTextAsync(filePath, indexHtml);
    }
}