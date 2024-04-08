using System.Text.RegularExpressions;

namespace Whatsapp.web.net.scripts;

public class JavaScriptParser : IJavaScriptParser
{
    private readonly Dictionary<string, string> _methods = new();

    public JavaScriptParser(string content)
    {
        LoadMethodsFromFile(content);
    }

    public string? GetMethod(string methodName)
    {
        return _methods.GetValueOrDefault(methodName);
    }

    private void LoadMethodsFromFile(string content)
    {
        var pattern = @"function\s+(\w+)\s*\(";
        var regex = new Regex(pattern);
        var matches = regex.Matches(content);

        foreach (Match match in matches)
        {
            var methodName = match.Groups[1].Value;
            var methodStartIndex = match.Index;
            var methodEndIndex = FindEndOfMethod(content, methodStartIndex);
            if (methodEndIndex == -1)
            {
                throw new Exception($"method '{methodName}' not found");
            }
            var methodContent = content.Substring(methodStartIndex, methodEndIndex - methodStartIndex + 1);
            _methods.Add(methodName, methodContent);
        }
    }

    private int FindEndOfMethod(string content, int startIndex)
    {
        var bracketCount = 0;
        for (var i = startIndex; i < content.Length; i++)
        {
            if (content[i] == '{')
            {
                bracketCount++;
            }
            else if (content[i] == '}')
            {
                bracketCount--;
                if (bracketCount == 0)
                {
                    return i;
                }
            }
        }
        return -1;
    }
}