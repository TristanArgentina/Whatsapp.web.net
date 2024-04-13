using System.IO.Compression;
using Whatsapp.web.net.EventArgs;

namespace Whatsapp.web.net.LoginWebCache;

public class LoginWebRemoteCacheService : LoginWebCacheBaseService
{
    protected override string PrefixFileName => "RemoteAuth";

    event EventHandler<DispatcherEventArg>? DispatchEventGeneric;

    private readonly IRemoteStore? _store;
    private readonly int _backupSyncIntervalMs;
    private readonly string _tempDir;
    private Timer? _backupSync;
    private readonly string[] _requiredDirs = ["Default", "IndexedDB", "Local Storage"];

    public LoginWebRemoteCacheService(IRemoteStore? store,
        PuppeteerOptions puppeteerOptions, LoginWebCacheOptions loginWebCacheOptions) : base(puppeteerOptions, loginWebCacheOptions)
    {
        if (string.IsNullOrEmpty(loginWebCacheOptions.RemotePath))
        {
            throw new ArgumentException("webVersionCache.remotePath is required when using the remote cache");
        }
        if (!IsValidBackupSyncInterval(loginWebCacheOptions.BackupSyncIntervalMs))
        {
            throw new ArgumentException("Invalid backupSyncIntervalMs. Accepts values starting from 60000ms (1 minute).");
        }

        _store = store ?? throw new ArgumentNullException("Remote database store is required.");
        _backupSyncIntervalMs = loginWebCacheOptions.BackupSyncIntervalMs;
        ;
        var dataPath = GetDataPath();
        _tempDir = !string.IsNullOrEmpty(loginWebCacheOptions.ClientId)
            ? $"{dataPath}/wwebnet_temp_session_{loginWebCacheOptions.ClientId}"
            : $"{dataPath}/wwebnet_temp_session";
    }


    protected override ILoginWebCache CreateLoginWebCache()
    {
        return new LoginWebRemoteCache(LoginWebCacheOptions.RemotePath, LoginWebCacheOptions.Strict);
    }

    private bool IsValidBackupSyncInterval(int backupSyncIntervalMs)
    {
        return backupSyncIntervalMs >= 60000;
    }

    public override async Task BeforeBrowserInitialized()
    {
        await ExtractRemoteSession();
    }
    
    public override async Task Logout()
    {
        await Disconnect();
    }

    public override async Task Destroy()
    {
        _backupSync?.Dispose();
    }

    public async Task Disconnect()
    {
        await DeleteRemoteSession();

        if (Directory.Exists(UserDataDir))
        {
            Directory.Delete(UserDataDir, true);
        }

        _backupSync?.Dispose();
    }

    public async Task AfterAuthReady()
    {
        var sessionExists = await _store.SessionExists(SessionName);
        if (!sessionExists)
        {
            await Delay(60000); // Initial delay sync required for session to be stable enough to recover
            await StoreRemoteSession(true);
        }

        _backupSync = new Timer(async _ => await StoreRemoteSession(), null, 0, _backupSyncIntervalMs);
    }

    public async Task StoreRemoteSession(bool emit = false)
    {
        // Compress & Store Session
        var pathExists = Directory.Exists(UserDataDir);
        if (pathExists)
        {
            await CompressSession();
            await _store.Save(SessionName);
            File.Delete($"{SessionName}.zip");
            Directory.Delete(_tempDir, true);
            if (emit)
            {
                EmitRemoteSessionSaved();
            }
        }
    }

    // Implement other methods as needed

    private async Task ExtractRemoteSession()
    {
        var pathExists = Directory.Exists(UserDataDir);
        var compressedSessionPath = $"{SessionName}.zip";
        var sessionExists = await _store.SessionExists(SessionName);

        if (pathExists)
        {
            Directory.Delete(UserDataDir, true);
        }

        if (sessionExists)
        {
            await _store.Extract(SessionName, compressedSessionPath);
            await UncompressSession(compressedSessionPath);
        }
        else
        {
            Directory.CreateDirectory(UserDataDir);
        }
    }

    private async Task DeleteRemoteSession()
    {
        var sessionExists = await _store.SessionExists(SessionName);
        if (sessionExists)
        {
            await _store.Delete(SessionName);
        }
    }
    
    private async Task CompressSession()
    {
        var archive = new ZipArchive(File.Create($"{SessionName}.zip"), ZipArchiveMode.Create);

        await CopyDirectory(UserDataDir, _tempDir);
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
                var destinationPath = Path.Combine(UserDataDir, entry.FullName);
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
        var sessionDirs = new[] { _tempDir, Path.Combine(_tempDir, "Default") };
        foreach (var dir in sessionDirs)
        {
            foreach (var element in Directory.EnumerateFileSystemEntries(dir))
            {
                if (!_requiredDirs.Contains(element))
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

    public void EmitRemoteSessionSaved()
    {
        Task.Run(() =>
        {
            var eventArg = new DispatcherEventArg(DispatcherEventsType.REMOTE_SESSION_SAVED);
            DispatchEventGeneric?.Invoke(this, eventArg);
        });
    }
}