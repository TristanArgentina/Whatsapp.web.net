using System.Text.RegularExpressions;

namespace Whatsapp.web.net.Authentication;

public abstract class BaseAuthStrategy : IAuthenticator
{
    protected readonly WebVersionCache WebVersionCache;

    private ILoginWebCache? _loginWebCache;

    private string? _serviceName;

    private string? _userDataDir;

    protected PuppeteerOptions PuppeteerOptions;

    protected BaseAuthStrategy(PuppeteerOptions puppeteerOptions, WebVersionCache webVersionCache)
    {
        if (!IsValidClientId(webVersionCache.ClientId))
            throw new ArgumentException(
                "Invalid clientId. Only alphanumeric characters, underscores, and hyphens are allowed.");

        PuppeteerOptions = puppeteerOptions;
        WebVersionCache = webVersionCache;
    }

    protected virtual string PrefixFileName => string.Empty;

    protected string SessionName => _serviceName ??= CalculateServiceName();
    
    public ILoginWebCache LoginWebCache => _loginWebCache ??= CreateLoginWebCache();

    public string UserDataDir => _userDataDir ??= CalculateUserDataDir();

    public virtual Task BeforeBrowserInitialized()
    {
        return Task.CompletedTask;
    }

    public virtual Task AfterBrowserInitialized()
    {
        return Task.CompletedTask;
    }

    public virtual Task AfterAuthReady()
    {
        return Task.CompletedTask;
    }

    public virtual Task Disconnect()
    {
        return Task.CompletedTask;
    }

    public virtual Task Destroy()
    {
        return Task.CompletedTask;
    }

    public virtual Task Logout()
    {
        return Task.CompletedTask;
    }

    protected virtual string CalculateUserDataDir()
    {
        var dataPath = GetDataPath();
        var userDataDir = Path.Combine(dataPath, WebVersionCache.RelativeLocalPath ?? "", SessionName);
        if (!Directory.Exists(userDataDir)) Directory.CreateDirectory(userDataDir);
        return userDataDir;
    }

    private string CalculateServiceName()
    {
        var clientId = WebVersionCache.ClientId;
        return string.IsNullOrEmpty(clientId) ? PrefixFileName : $"{PrefixFileName}-{clientId}";
    }

    protected abstract ILoginWebCache CreateLoginWebCache();

    protected string GetDataPath()
    {
        var dataPath = !string.IsNullOrEmpty(PuppeteerOptions.DirectoryBase)
            ? Path.GetFullPath(PuppeteerOptions.DirectoryBase)
            : Environment.CurrentDirectory;
        return dataPath;
    }

    private bool IsValidClientId(string? clientId)
    {
        if (string.IsNullOrEmpty(clientId)) return true;
        var idRegex = new Regex(@"^[-_\w]+$", RegexOptions.IgnoreCase);
        return idRegex.IsMatch(clientId);
    }
}