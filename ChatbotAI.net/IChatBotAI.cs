namespace ChatbotAI.net;

public interface IChatBotAI
{
    Task<string> Ask(string fromId, string substring);

    Task<string> ConvertToText(string filePath, MemoryStream memoryStream);
    
    byte[] GenerateSpeechFromText(string text);
    
    string GetAudioTranscription(byte[] audio);
}