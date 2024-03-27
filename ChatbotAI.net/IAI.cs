namespace ChatbotAI.net;

public interface IAI
{
    Task<string> Ask(string fromId, string substring);

    Task<string> ConvertToText(string filePath, MemoryStream memoryStream);
}