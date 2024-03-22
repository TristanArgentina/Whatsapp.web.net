namespace Whatsapp.web.net;

public class WebVersionCache
{
    public string Type { get; set; } = "local";

    public string? RemotePath { get; set; }

    public string? LocalPath { get; set; } 

    public bool Strict { get; set; }
}