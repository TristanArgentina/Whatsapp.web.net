using FFMpegCore.Pipes;
using Microsoft.Extensions.Configuration;

namespace Whatsapp.web.net.test;

public static class Utils
{
    public static void SaveToMp3(string base64Audio, string mp3FilePath)
    {
        using var audioOutputStream = File.Open(mp3FilePath, FileMode.OpenOrCreate);

        var audioBytes = Convert.FromBase64String(base64Audio);

        using var memoryStream = new MemoryStream(audioBytes);
        FFMpegCore.FFMpegArguments.FromPipeInput(new StreamPipeSource(memoryStream))
            .OutputToPipe(new StreamPipeSink(audioOutputStream), options =>
                options.ForceFormat("mp3"))
            .ProcessSynchronously();
    }

    public static IConfigurationRoot BuildConfig(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>()
            .AddCommandLine(args)
            .Build();
        return config;
    }
}