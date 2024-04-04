using System.Text;
using System.Text.Json;
using FFMpegCore;
using FFMpegCore.Arguments;
using FFMpegCore.Pipes;
using PuppeteerSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using Whatsapp.web.net.Domains;
using Image = SixLabors.ImageSharp.Image;

namespace Whatsapp.web.net;

public class Util
{
    public static DateTime ConvertToDate(dynamic timestampDynamic)
    {
        var timestamp = (long) timestampDynamic;

        if (timestamp < -62135596800)
        {
            return DateTime.MinValue;
        }

        if (timestamp > 253402300799)
        {
            return DateTime.MaxValue;
        }
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
    }

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
            default:
                return "application/octet-stream"; 
        }
    }
    
    public static async Task<MessageMedia> FormatToWebpSticker(MessageMedia media, StickerMetadata metadata, IPage pupPage)
    {
        MessageMedia webpMedia;

        if (media.Mimetype.Contains("image"))
            webpMedia = await FormatImageToWebpSticker(media, pupPage);
        else if (media.Mimetype.Contains("video"))
            webpMedia = await FormatVideoToWebpSticker(media);
        else
            throw new Exception("Invalid media format");

        if (metadata.Name == null && metadata.Author == null) return webpMedia;
        
        var img = Image.Load(Convert.FromBase64String(webpMedia.Data));
        var hash = GenerateHash(32);
        var stickerPackId = hash;
        var packName = metadata.Name;
        var author = metadata.Author;
        var categories = metadata.Categories ?? [""];
        var json = new { sticker_pack_id = stickerPackId, sticker_pack_name = packName, sticker_pack_publisher = author, emojis = categories };
        var exifAttr = new byte[] { 0x49, 0x49, 0x2A, 0x00, 0x08, 0x00, 0x00, 0x00, 0x01, 0x00, 0x41, 0x57, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00 };
        var jsonBuffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(json));
        var exif = new byte[exifAttr.Length + jsonBuffer.Length];
        Buffer.BlockCopy(exifAttr, 0, exif, 0, exifAttr.Length);
        Buffer.BlockCopy(jsonBuffer, 0, exif, exifAttr.Length, jsonBuffer.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(jsonBuffer.Length), 0, exif, 14, 4);
        img.Metadata.ExifProfile = new ExifProfile(exif);
        using var stream = new MemoryStream();
        await img.SaveAsync(stream, new WebpEncoder());
        webpMedia.Data = Convert.ToBase64String(stream.ToArray());

        return webpMedia;
    }

    public static async Task<MessageMedia> FormatImageToWebpSticker(MessageMedia media, IPage pupPage)
    {
        if (!media.Mimetype.Contains("image"))
            throw new Exception("media is not an image");

        if (media.Mimetype.Contains("webp"))
            return media;

        return await pupPage.EvaluateFunctionAsync<MessageMedia>(@"(media) => {
            return window.WWebJS.toStickerData(media);
        }", media);
    }

    public static async Task<MessageMedia> FormatImageToWebpSticker(MessageMedia media, Page pupPage)
    {
        if (!media.Mimetype.Contains("image"))
            throw new Exception("media is not an image");

        if (media.Mimetype.Contains("webp"))
            return media;

        var evaluateScript = $"window.WWebJS.toStickerData({{ mimeType: '{media.Mimetype}', data: '{media.Data}' }})";
        var result = await pupPage.EvaluateExpressionAsync<string>(evaluateScript);

        // Create and return a new MessageMedia with the webp data
        return new MessageMedia("image/webp", result);
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




    public static async Task<MessageMedia> FormatVideoToWebpSticker(MessageMedia media)
    {
        if (!media.Mimetype.Contains("video"))
            throw new Exception("media is not a video");

        var videoType = media.Mimetype.Split('/')[1];

        var tempFile = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.webp");

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
        await ffmpegCmd.ProcessAsynchronously();

        var data = Convert.ToBase64String(await File.ReadAllBytesAsync(tempFile));
        File.Delete(tempFile);


        return new MessageMedia(media.Filename, "image/webp", data);
    }
}