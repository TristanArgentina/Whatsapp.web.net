using Newtonsoft.Json;

namespace Whatsapp.web.net.Domains;

/// <summary>
/// Media attached to a message
/// </summary>
public class MessageMedia
{

    /// <summary>
    /// MIME type of the attachment
    /// </summary>
    public string Mimetype { get; set; }

    /// <summary>
    /// Base64 encoded data that represents the file
    /// </summary>
    public string Data { get; set; }

    /// <summary>
    /// Document file name. Value can be null
    /// </summary>
    public string Filename { get; private set; }

    /// <summary>
    /// Document file size in bytes. Value can be null
    /// </summary>
    public long? FileSize { get; private set; }

    public MessageMedia(string filename, string mimetype, string data, long? fileSize = null)
    {
        Data = data;
        Mimetype = mimetype;
        Filename = filename;
        FileSize = fileSize;

    }

    [JsonConstructor]
    public MessageMedia(dynamic? dynamicData)
    {
        if (dynamicData is not null)
        {
            Patch(dynamicData);
        }
    }

    public MessageMedia(string? mimetype, string? data)
    {
        Data = data;
        Mimetype = mimetype;
    }

    private void Patch(dynamic data)
    {
        Data = data.data;
        Mimetype = data.mimetype;
        Filename = data.filename;
        FileSize = data.filesize;
    }


    public static MessageMedia FromFilePath(string filePath)
    {
        var base64Data = Convert.ToBase64String(File.ReadAllBytes(filePath));
        var mimeType = Util.GetMimeType(filePath);
        var filename = Path.GetFileName(filePath);

        return new MessageMedia(filename, mimeType, base64Data, base64Data.Length);
    }

    public static async Task<MessageMedia> FromUrl(string url, bool unsafeMime = false, string? filename = null, HttpClient? client = null, int reqOptionsSize = 0)
    {
        var pUrl = new Uri(url);
        string? mimeType = null;

        if (!unsafeMime)
            throw new InvalidOperationException("Unable to determine MIME type using URL. Set unsafeMime to true to download it anyway.");

        async Task<(string?, string?, string?, long?)> FetchData(string url, HttpClient httpClient, int size)
        {
            using var response = await httpClient.GetAsync(url);
            var mime = response.Content.Headers.ContentType.MediaType;
            var contentDisposition = response.Content.Headers.ContentDisposition;
            var name = contentDisposition?.FileName;
            var data = await response.Content.ReadAsByteArrayAsync();
            var base64Data = Convert.ToBase64String(data);
            var fileSize = data.LongLength;

            return (name, mime, base64Data, fileSize);
        }

        using var httpClient = client ?? new HttpClient();
        var (name, mime, data, size) = await FetchData(url, httpClient, reqOptionsSize);
        var finalFilename = filename ?? name ?? Path.GetFileName(pUrl.LocalPath) ?? "file";

        mimeType ??= mime;

        return new MessageMedia(finalFilename, mimeType, data, size);
    }

    protected bool Equals(MessageMedia other)
    {
        return Mimetype == other.Mimetype && Data == other.Data && Filename == other.Filename;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((MessageMedia)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Mimetype, Data, Filename);
    }
}