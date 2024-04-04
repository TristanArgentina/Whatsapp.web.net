namespace Whatsapp.web.net;

public interface IRemoteStore
{
    Task<bool> SessionExists(string sessionName);

    Task Extract(string sessionName, string compressedSessionPath);
    
    Task Save(string sessionName);
    
    Task Delete(string sessionName);
}