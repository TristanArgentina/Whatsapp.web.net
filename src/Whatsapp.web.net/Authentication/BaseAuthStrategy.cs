using System.Text.RegularExpressions;

namespace Whatsapp.web.net.Authentication;

public abstract class BaseAuthStrategy : IAuthenticator
{
    public string? UserDataDir { get; protected set; }

    public ILoginWebCache LoginWebCache { get; protected set; }

    protected string SessionName;

    protected abstract string PrefixFileName { get;} 

    protected PuppeteerOptions PuppeteerOptions;
    protected readonly WebVersionCache WebVersionCache;

    protected BaseAuthStrategy(PuppeteerOptions puppeteerOptions, WebVersionCache webVersionCache)
    {
        if (!IsValidClientId(webVersionCache.ClientId))
        {
            throw new ArgumentException("Invalid clientId. Only alphanumeric characters, underscores, and hyphens are allowed.");
        }

        PuppeteerOptions = puppeteerOptions;
        WebVersionCache = webVersionCache;
        UserDataDir = CalculateUserDataDir();
        LoginWebCache = CreateLoginWebCache();
    }

    protected virtual string CalculateUserDataDir()
    {
        var dataPath = GetDataPath();

        var clientId = WebVersionCache.ClientId;
        SessionName = string.IsNullOrEmpty(clientId) ? PrefixFileName : $"{PrefixFileName}-{clientId}";
        var userDataDir = Path.Combine(dataPath, WebVersionCache.RelativeLocalPath ?? "", SessionName);

        if (!Directory.Exists(userDataDir))
        {
            Directory.CreateDirectory(userDataDir);
        }

        return userDataDir;
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

    public virtual Task BeforeBrowserInitialized()
    {
        return Task.CompletedTask;
    }

    public virtual Task AfterBrowserInitialized()
    {
        return Task.CompletedTask;
    }

    public virtual async Task<(bool Failed, bool Restart, object FailureEventPayload)> OnAuthenticationNeeded()
    {
        return (Failed: false, Restart: false, FailureEventPayload: null);
    }

    public virtual Task<object> GetAuthEventPayload()
    {
        return Task.FromResult<object>(null);
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
}