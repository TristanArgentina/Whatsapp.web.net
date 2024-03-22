namespace Whatsapp.web.net.Domains;

public class MessageMedia
{
    public string MimeType { get; set; }
    public string Data { get; set; }
    public string Filename { get; private set; }
    public long? FileSize { get; private set; }
    public string FileName { get; set; }

    public MessageMedia(string filename, string mimeType, string data, long? fileSize = null)
    {
        Data = data;
        MimeType = mimeType;
        Filename = filename;
        FileSize = fileSize;

    }

    public MessageMedia(dynamic? dynamicData)
    {
        if (dynamicData is not null)
        {
            Patch(dynamicData);
        }
    }

    public MessageMedia(string? mimeType, string? data)
    {
        Data = data;
        MimeType = mimeType;
    }

    private void Patch(dynamic data)
    {
        MimeType = data.mimeType;
        Data = data.data;
        Filename = data.filename;
        FileSize = data.fileSize;
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
        var (data, mime, name, size) = await FetchData(url, httpClient, reqOptionsSize);
        var finalFilename = filename ?? name ?? Path.GetFileName(pUrl.LocalPath) ?? "file";

        mimeType ??= mime;

        return new MessageMedia(finalFilename, mimeType, data, size);
    }
}