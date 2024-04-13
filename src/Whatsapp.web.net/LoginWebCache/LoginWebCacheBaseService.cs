using System.Text.RegularExpressions;

namespace Whatsapp.web.net.LoginWebCache;

public abstract class LoginWebCacheBaseService : ILoginWebCacheService
{
    protected readonly LoginWebCacheOptions LoginWebCacheOptions;

    private ILoginWebCache? _loginWebCache;

    private string? _serviceName;

    private string? _userDataDir;

    protected PuppeteerOptions PuppeteerOptions;

    protected LoginWebCacheBaseService(PuppeteerOptions puppeteerOptions, LoginWebCacheOptions loginWebCacheOptions)
    {
        if (!IsValidClientId(loginWebCacheOptions.ClientId))
            throw new ArgumentException(
                "Invalid clientId. Only alphanumeric characters, underscores, and hyphens are allowed.");

        PuppeteerOptions = puppeteerOptions;
        LoginWebCacheOptions = loginWebCacheOptions;
    }

    protected virtual string PrefixFileName => string.Empty;

    protected string SessionName => _serviceName ??= CalculateServiceName();
    
    public ILoginWebCache Get() => _loginWebCache ??= CreateLoginWebCache();

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
        var userDataDir = Path.Combine(dataPath, LoginWebCacheOptions.RelativeLocalPath ?? "", SessionName);
        if (!Directory.Exists(userDataDir)) Directory.CreateDirectory(userDataDir);
        return userDataDir;
    }

    private string CalculateServiceName()
    {
        var clientId = LoginWebCacheOptions.ClientId;
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