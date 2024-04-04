namespace Whatsapp.web.net.Authentication;

public class WebVersionCache
{

    /// <summary>
    /// Client identification, in the case that several clients are used, a cache part is created for each client.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// It is the type of cache that will be used, corresponding to where the data is stored.
    /// Allowed values are 'Local', 'Remote' or '' (Empty)
    /// </summary>
    public string Type { get; set; } = "local";

    /// <summary>
    /// It is the folder where the site information will be saved. It is used in addition to the DirectoryBase folder.
    /// </summary>
    public string? RelativeLocalPath { get; set; } = "wwebnet_cache";

    /// <summary>
    /// It is the url that says where it is stored remotely.
    /// </summary>
    public string? RemotePath { get; set; }

    /// <summary>
    /// When the value is true, it raises an exception if the cache does not exist.
    /// </summary>
    public bool Strict { get; set; }

    /// <summary>
    /// Set the interval to be backed up. It is used only when the cache is remote type.
    /// </summary>
    public int BackupSyncIntervalMs { get; set; } = 60000;
}