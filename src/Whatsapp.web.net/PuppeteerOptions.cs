using PuppeteerSharp;

namespace Whatsapp.web.net;

public class PuppeteerOptions
{
    public bool Headless { get; set; } = true;

    public ViewPortOptions? DefaultViewport { get; set; } = null;

    /// <summary>
    /// Base directory where the site information will be saved.
    /// When empty or null, it is set to the directory where it is running.
    /// </summary>
    public string? DirectoryBase { get; set; }

    public string? BrowserWSEndpoint { get; set; }

    public IEnumerable<string>? Args { get; set; }

    public string? ExecutablePath { get; set; }
}