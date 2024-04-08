using Newtonsoft.Json.Linq;

namespace Whatsapp.web.net.Domains;

public class PlayedInfo
{
    public string Id { get; private set; }

    public DateTime? T { get; private set; }

    public PlayedInfo(dynamic? data)
    {
        Patch(data);
    }

    private void Patch(dynamic? data)
    {
        if (data is null) return;
        if (data.Type == JTokenType.Null) return;
        Id = data.id;
        T = Util.ConvertToDate(data.t);
    }

}