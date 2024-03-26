using System.Text;
using System.Text.Json;
using FFMpegCore;
using FFMpegCore.Arguments;
using FFMpegCore.Pipes;
using PuppeteerSharp;
using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net;

public class Util
{
    public static Dictionary<string, object> MergeDefault(Dictionary<string, object> def, Dictionary<string, object> given)
    {
        if (given == null) return def;
        foreach (var kvp in def)
        {
            if (!given.ContainsKey(kvp.Key) || given[kvp.Key] == null)
            {
                given[kvp.Key] = kvp.Value;
            }
            else if (given[kvp.Key] is Dictionary<string, object>)
            {
                given[kvp.Key] = MergeDefault((Dictionary<string, object>)kvp.Value, (Dictionary<string, object>)given[kvp.Key]);
            }
        }
        return given;
    }

    public static string GetMimeType(string filename)
    {
        var extension = Path.GetExtension(filename).ToLowerInvariant();
        switch (extension)
        {
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";
            case ".png":
                return "image/png";
            // Añade más casos según sea necesario
            default:
                return "application/octet-stream"; // Tipo MIME predeterminado
        }
    }

    public static void SetFfmpegPath(string path)
    {
        // Establece la variable de entorno PATH para incluir la ruta de FFmpeg
        var currentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
        var newPath = $"{currentPath};{path}";
        Environment.SetEnvironmentVariable("PATH", newPath);

        // Verifica que la ruta especificada sea válida
        if (!VerifyFfmpegPath(path))
        {
            throw new Exception("La ruta de FFmpeg especificada no es válida.");
        }
    }

    private static bool VerifyFfmpegPath(string path)
    {
        // Verifica si el archivo ejecutable de FFmpeg existe en la ruta especificada
        return File.Exists(Path.Combine(path, "ffmpeg.exe"));
    }

    public static async Task<MessageMedia> FormatToWebpSticker(MessageMedia media, StickerMetadata metadata, IPage pupPage)
    {
        MessageMedia webpMedia;

        if (media.MimeType.Contains("image"))
            webpMedia = await FormatImageToWebpSticker(media, pupPage);
        else if (media.MimeType.Contains("video"))
            webpMedia = await FormatVideoToWebpSticker(media);
        else
            throw new Exception("Invalid media format");

        if (metadata.Name != null || metadata.Author != null)
        {
            var img = new WebpImage();
            var hash = GenerateHash(32);
            var stickerPackId = hash;
            var packname = metadata.Name;
            var author = metadata.Author;
            var categories = metadata.Categories ?? [""];
            var json = new { sticker_pack_id = stickerPackId, sticker_pack_name = packname, sticker_pack_publisher = author, emojis = categories };
            var exifAttr = new byte[] { 0x49, 0x49, 0x2A, 0x00, 0x08, 0x00, 0x00, 0x00, 0x01, 0x00, 0x41, 0x57, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00 };
            var jsonBuffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(json));
            var exif = new byte[exifAttr.Length + jsonBuffer.Length];
            Buffer.BlockCopy(exifAttr, 0, exif, 0, exifAttr.Length);
            Buffer.BlockCopy(jsonBuffer, 0, exif, exifAttr.Length, jsonBuffer.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(jsonBuffer.Length), 0, exif, 14, 4);
            await img.Load(Convert.FromBase64String(webpMedia.Data));
            img.Exif = exif;
            webpMedia.Data = Convert.ToBase64String(await img.Save());
        }

        return webpMedia;
    }

    public static async Task<MessageMedia> FormatImageToWebpSticker(MessageMedia media, IPage pupPage)
    {
        if (!media.MimeType.Contains("image"))
            throw new Exception("media is not an image");

        if (media.MimeType.Contains("webp"))
            return media;

        return await pupPage.EvaluateFunctionAsync<MessageMedia>(@"(media) => {
            return window.WWebJS.toStickerData(media);
        }", media);
    }



    public static string GenerateHash(int length)
    {
        var result = new StringBuilder();
        var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var charactersLength = characters.Length;
        var random = new Random();

        for (var i = 0; i < length; i++)
        {
            result.Append(characters[random.Next(charactersLength)]);
        }

        return result.ToString();
    }


    public static async Task<MessageMedia> FormatImageToWebpSticker(MessageMedia media, Page pupPage)
    {
        if (!media.MimeType.Contains("image"))
            throw new Exception("media is not an image");

        if (media.MimeType.Contains("webp"))
            return media;

        var evaluateScript = $"window.WWebJS.toStickerData({{ mimeType: '{media.MimeType}', data: '{media.Data}' }})";
        var result = await pupPage.EvaluateExpressionAsync<string>(evaluateScript);

        // Create and return a new MessageMedia with the webp data
        return new MessageMedia("image/webp", result);
    }

    public static async Task<MessageMedia> FormatVideoToWebpSticker(MessageMedia media)
    {
        if (!media.MimeType.Contains("video"))
            throw new Exception("media is not a video");

        var videoType = media.MimeType.Split('/')[1];

        var tempFile = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.webp");


        // Construir la línea de comandos FFmpeg para convertir el video a webp
        var ffmpegCmd = FFMpegArguments
            .FromPipeInput(new StreamPipeSource(new MemoryStream(Convert.FromBase64String(media.Data))))
            .OutputToPipe(new StreamPipeSink(new FileStream(tempFile, FileMode.Create)), options =>
            {
                options.Seek(TimeSpan.FromSeconds(0));
                options.WithArgument(new DurationArgument(TimeSpan.FromSeconds(5)));
                options.WithArgument(new VideoCodecArgument("LibWebP"));
                options.WithArgument(new FrameRateArgument(10));
                options.Resize(512, 512);
            });

        // Ejecutar el comando FFmpeg
        await ffmpegCmd.ProcessAsynchronously();

        // Leer el archivo de salida webp
        var data = Convert.ToBase64String(await File.ReadAllBytesAsync(tempFile));
        File.Delete(tempFile);


        return new MessageMedia(media.FileName, "image/webp", data);
    }
}