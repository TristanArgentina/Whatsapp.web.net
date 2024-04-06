using Newtonsoft.Json.Linq;

namespace Whatsapp.web.net.Domains;

public class ReadInfo
{
    public string Id { get; set; }
    public DateTime? T { get; set; }

    public ReadInfo(dynamic? data)
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