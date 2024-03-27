namespace Whatsapp.web.net.test;

public interface IAI
{
    Task<string> Ask(string fromId, string substring);
}