using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.Extensions;

public static class CallExtensions
{
    public static async void Reject(this Call call, Client client)
    {
        await client.Reject(call.From, call.Id);
    }
}