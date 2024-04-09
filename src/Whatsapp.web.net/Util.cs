using System.Text;
using FFMpegCore;
using FFMpegCore.Arguments;
using FFMpegCore.Pipes;
using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using Whatsapp.web.net.Domains;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace Whatsapp.web.net;

public class Util
{
    public static DateTime? ConvertToDate(dynamic timestampDynamic)
    {
        if (timestampDynamic == null) return null;
        if (timestampDynamic.Type == JTokenType.Null) return null;
        if (string.IsNullOrEmpty(timestampDynamic.ToString())) return null;
        var timestamp = (long)timestampDynamic;

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
        {
            var sticker = ConvertToSticker(metadata, media.Data);
            return new MessageMedia(media.Filename, "image/webp", Convert.ToBase64String(sticker));
        }

        if (media.Mimetype.Contains("video"))
        {
            webpMedia = await FormatVideoToWebpSticker(media);
            var sticker = ConvertToSticker(metadata, webpMedia.Data);
            media.Data = Convert.ToBase64String(sticker);
            return media;
        }
        throw new Exception("Invalid media format");

    }

    private static byte[] ConvertToSticker(StickerMetadata metadata, string data)
    {
        var imageBytes = Convert.FromBase64String(data);
        var ms = ConvertImageToWebp(imageBytes, metadata);
        return ms.ToArray();
    }


    public static MemoryStream ConvertImageToWebp(byte[] imageBytes, StickerMetadata metadata)
    {
        using var ms = new MemoryStream(imageBytes);
        using var image = Image.Load<Rgba32>(ms);

        var exifProfile = new ExifProfile();
        exifProfile.SetValue(ExifTag.ImageUniqueID, GenerateHash(32));
        exifProfile.SetValue(ExifTag.ImageDescription, metadata.Name);
        exifProfile.SetValue(ExifTag.Artist, metadata.Author);
        exifProfile.SetValue(ExifTag.UserComment, metadata.Author);
        image.Metadata.ExifProfile = exifProfile;
        var outputMemoryStream = new MemoryStream();

        image.SaveAsWebp(outputMemoryStream);
        outputMemoryStream.Position = 0;

        return outputMemoryStream;
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