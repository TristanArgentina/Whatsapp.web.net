using PuppeteerSharp;

namespace Whatsapp.web.net;

public class PuppeteerOptions
{
    public bool Headless { get; set; } = true;

    public ViewPortOptions? DefaultViewport { get; set; } = null;

    public string? UserDataDir { get; set; }

    public string? BrowserWSEndpoint { get; set; }

    public IEnumerable<string>? Args { get; set; }

    public string? ExecutablePath { get; set; }
}