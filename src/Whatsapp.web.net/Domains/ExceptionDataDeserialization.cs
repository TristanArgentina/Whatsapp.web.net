namespace Whatsapp.web.net.Domains;

public class ExceptionDataDeserialization : Exception
{
    public dynamic Data { get; }

    public Exception Exception { get; }

    public ExceptionDataDeserialization(dynamic data, Exception exception)
    {
    }
}