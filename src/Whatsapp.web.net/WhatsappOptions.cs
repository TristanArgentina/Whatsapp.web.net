using PuppeteerSharp;
using Whatsapp.web.net.LoginWebCache;

namespace Whatsapp.web.net;

public class WhatsappOptions
{
    /// <summary>
    /// Puppeteer launch options. View docs here: https://github.com/puppeteer/puppeteer/
    /// </summary>
    public PuppeteerOptions Puppeteer { get; set; } = new();

    /// <summary>
    /// The version of WhatsApp Web to use. 
    /// </summary>
    public string WebVersion { get; set; } 

    /// <summary>
    /// Determines how to retrieve the WhatsApp Web version.
    /// Defaults to a local cache (LocalWebCache) that falls back to latest if the requested version is not found.
    /// </summary>
    public LoginWebCacheOptions LoginWebCache { get; set; } = new();

    /// <summary>
    /// Timeout for authentication selector in puppeteer
    /// </summary>
    public int AuthTimeoutMs { get; set; } = 30 * 1000;

    /// <summary>
    /// How many times should the qrcode be refreshed before giving up
    /// </summary>
    public int QrMaxRetries { get; set; } = 0;

    /// <summary>
    /// If another whatsapp web session is detected (another browser), take over the session in the current browser
    /// </summary>
    public bool TakeoverOnConflict { get; set; } = false;

    /// <summary>
    /// How much time to wait before taking over the session
    /// </summary>
    public int TakeoverTimeoutMs { get; set; } = 0;

    /// <summary>
    /// User agent to use in puppeteer
    /// </summary>
    public string UserAgent { get; set; } 

    /// <summary>
    /// Sets bypassing of page&#x27;s Content-Security-Policy.
    /// </summary>
    public bool BypassCSP { get; set; } = false;

    /// <summary>
    /// Proxy Authentication object.
    /// </summary>
    public Credentials? ProxyAuthentication { get; set; } = null;

}