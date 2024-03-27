using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Whatsapp.web.net.AuthenticationStrategies;

public class RemoteAuth : BaseAuthStrategy
{
    private readonly string? clientId;
    private readonly string dataPath;
    private readonly IEventDispatcher _dispatcher;
    private readonly IRemoteStore? store;
    private readonly int backupSyncIntervalMs;
    private string userDataDir;
    private string tempDir;
    private Timer backupSync;
    private readonly string[] requiredDirs = ["Default", "IndexedDB", "Local Storage"];

    public RemoteAuth(IEventDispatcher dispatcher, IRemoteStore? store, string clientId, string dataPath = "./.wwebjs_auth/", int backupSyncIntervalMs = 60000)
    {
        if (!IsValidClientId(clientId))
        {
            throw new ArgumentException("Invalid clientId. Only alphanumeric characters, underscores, and hyphens are allowed.");
        }

        if (!IsValidBackupSyncInterval(backupSyncIntervalMs))
        {
            throw new ArgumentException("Invalid backupSyncIntervalMs. Accepts values starting from 60000ms (1 minute).");
        }

        if (store == null)
        {
            throw new ArgumentNullException("Remote database store is required.");
        }

        _dispatcher = dispatcher;
        this.store = store;
        this.clientId = clientId;
        this.dataPath = Path.GetFullPath(dataPath);
        this.backupSyncIntervalMs = backupSyncIntervalMs;

        // Handle optional dependencies
        try
        {
            // Check if the optional dependencies are available
            if (!Directory.Exists(this.dataPath) || Directory.GetFiles(this.dataPath).Length == 0)
            {
                // If any of the dependencies are not available, set them to null
                // Note: This approach assumes that you're only checking the existence of the data directory, you can adjust it as needed
                this.store = null;
                this.clientId = null;
                this.backupSyncIntervalMs = 0;
            }
        }
        catch
        {
            // If any exception occurs while checking for dependencies, set them to null
            this.store = null;
            this.clientId = null;
            this.backupSyncIntervalMs = 0;
        }
    }

    private bool IsValidClientId(string clientId)
    {
        if (string.IsNullOrEmpty(clientId))
        {
            return true; // Allow null or empty clientId
        }

        var idRegex = new Regex(@"^[-_\w]+$");
        return idRegex.IsMatch(clientId);
    }

    private bool IsValidBackupSyncInterval(int backupSyncIntervalMs)
    {
        return backupSyncIntervalMs >= 60000;
    }

    public override async Task BeforeBrowserInitialized()
    {
        var puppeteerOpts = Options.Puppeteer;
        var sessionDirName = string.IsNullOrEmpty(clientId) ? "RemoteAuth" : $"RemoteAuth-{clientId}";
        var dirPath = Path.Combine(dataPath, sessionDirName);

        if (!string.IsNullOrEmpty(puppeteerOpts.UserDataDir) && puppeteerOpts.UserDataDir != dirPath)
        {
            throw new InvalidOperationException("RemoteAuth is not compatible with a user-supplied userDataDir.");
        }

        userDataDir = dirPath;
        tempDir = $"{dataPath}/wwebjs_temp_session_{clientId}";

        await ExtractRemoteSession();

        Options.Puppeteer = new PuppeteerOptions
        {
            UserDataDir = dirPath
        };
    }

    public override async Task Logout()
    {
        await Disconnect();
    }

    public override async Task Destroy()
    {
        backupSync?.Dispose();
    }

    public async Task Disconnect()
    {
        await DeleteRemoteSession();

        if (Directory.Exists(userDataDir))
        {
            Directory.Delete(userDataDir, true);
        }

        backupSync?.Dispose();
    }

    public async Task AfterAuthReady()
    {
        var sessionExists = await store.SessionExists(SessionName);
        if (!sessionExists)
        {
            await Delay(60000); // Initial delay sync required for session to be stable enough to recover
            await StoreRemoteSession(true);
        }

        backupSync = new Timer(async _ => await StoreRemoteSession(), null, 0, backupSyncIntervalMs);
    }

    public async Task StoreRemoteSession(bool emit = false)
    {
        // Compress & Store Session
        var pathExists = Directory.Exists(userDataDir);
        if (pathExists)
        {
            await CompressSession();
            await store.Save(SessionName);
            File.Delete($"{SessionName}.zip");
            Directory.Delete(tempDir, true);
            if (emit)
            {
                _dispatcher.EmitRemoteSessionSaved();
            }
        }
    }

    // Implement other methods as needed

    private async Task ExtractRemoteSession()
    {
        var pathExists = Directory.Exists(userDataDir);
        var compressedSessionPath = $"{SessionName}.zip";
        var sessionExists = await store.SessionExists(SessionName);

        if (pathExists)
        {
            Directory.Delete(userDataDir, true);
        }

        if (sessionExists)
        {
            await store.Extract(SessionName, compressedSessionPath);
            await UncompressSession(compressedSessionPath);
        }
        else
        {
            Directory.CreateDirectory(userDataDir);
        }
    }

    private async Task DeleteRemoteSession()
    {
        var sessionExists = await store.SessionExists(SessionName);
        if (sessionExists)
        {
            await store.Delete(SessionName);
        }
    }

    private async Task CompressSession()
    {
        var archive = new ZipArchive(File.Create($"{SessionName}.zip"), ZipArchiveMode.Create);

        await CopyDirectory(userDataDir, tempDir);
        await DeleteMetadata();

        archive.Dispose();
    }

    private async Task UncompressSession(string compressedSessionPath)
    {
        await Task.Run(() =>
        {
            using var zip = ZipFile.OpenRead(compressedSessionPath);
            foreach (var entry in zip.Entries)
            {
                var destinationPath = Path.Combine(userDataDir, entry.FullName);
                if (entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\"))
                {
                    Directory.CreateDirectory(destinationPath);
                }
                else
                {
                    entry.ExtractToFile(destinationPath, true);
                }
            }
        });

        File.Delete(compressedSessionPath);
    }

    private async Task DeleteMetadata()
    {
        var sessionDirs = new[] { tempDir, Path.Combine(tempDir, "Default") };
        foreach (var dir in sessionDirs)
        {
            foreach (var element in Directory.EnumerateFileSystemEntries(dir))
            {
                if (!requiredDirs.Contains(element))
                {
                    if (Directory.Exists(element))
                    {
                        Directory.Delete(element, true);
                    }
                    else
                    {
                        File.Delete(element);
                    }
                }
            }
        }
    }

    private async Task CopyDirectory(string sourceDir, string targetDir)
    {
        await Task.Run(() =>
        {
            foreach (var dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourceDir, targetDir));
            }

            foreach (var newPath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourceDir, targetDir), true);
            }
        });
    }

    private async Task Delay(int milliseconds)
    {
        await Task.Delay(milliseconds);
    }
}