using System.Reflection;

namespace Whatsapp.web.net.scripts;

public static class JavaScriptParserFactory
{
    public static IJavaScriptParser Create(string resourceName)
    {
        var readFileEmbebed = ReadFileEmbebed(resourceName);

        return new JavaScriptParser(readFileEmbebed);
    }

    private static string ReadFileEmbebed(string resourceName)
    {
        var assembly = Assembly.GetAssembly(typeof(JavaScriptParser))!;
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null) throw new Exception("Resource not exists.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}