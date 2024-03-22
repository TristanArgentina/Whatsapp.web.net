namespace Whatsapp.web.net;

public class WebpImage
{
    public byte[] Exif { get; set; }

    public Task Load(byte[] data)
    {
        // Implement image loading logic
        return Task.CompletedTask;
    }

    public Task<byte[]> Save()
    {
        // Implement image saving logic
        return Task.FromResult(Array.Empty<byte>());
    }
}