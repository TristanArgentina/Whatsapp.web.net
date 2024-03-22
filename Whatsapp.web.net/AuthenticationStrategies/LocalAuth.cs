namespace Whatsapp.web.net.AuthenticationStrategies;

public class LocalAuth : BaseAuthStrategy
{
    private readonly string _clientId;
    private readonly string _dataPath;
    private string _userDataDir;

    public LocalAuth(string? clientId = null, string dataPath = "./.wwebjs_auth/")
    {
        if (string.IsNullOrEmpty(clientId) || !IsValidClientId(clientId))
        {
            throw new ArgumentException("Invalid clientId. Only alphanumeric characters, underscores, and hyphens are allowed.");
        }

        _clientId = clientId;
        _dataPath = Path.GetFullPath(dataPath);
    }

    private bool IsValidClientId(string clientId)
    {
        var idRegex = new System.Text.RegularExpressions.Regex(@"^[-_\w]+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        return idRegex.IsMatch(clientId);
    }

    public override async Task BeforeBrowserInitialized()
    {
        if (Client is null)
        {
            throw new Exception("Client is null. Missing setup.");
        }
        var puppeteerOpts = Options.Puppeteer;
        var sessionDirName = string.IsNullOrEmpty(_clientId) ? "session" : $"session-{_clientId}";
        _userDataDir = Path.Combine(_dataPath, sessionDirName);

        if (!string.IsNullOrEmpty(puppeteerOpts.UserDataDir) && puppeteerOpts.UserDataDir != _userDataDir)
        {
            throw new InvalidOperationException("LocalAuth is not compatible with a user-supplied userDataDir.");
        }

        if (!Directory.Exists(_userDataDir))
        {
            Directory.CreateDirectory(_userDataDir);
        }

        Options.Puppeteer.UserDataDir = _userDataDir;
    }

    public override async Task Logout()
    {
        if (!string.IsNullOrEmpty(_userDataDir))
        {
            if (Directory.Exists(_userDataDir))
            {
                Directory.Delete(_userDataDir, true);
            }
        }
    }
}