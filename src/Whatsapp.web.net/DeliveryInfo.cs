using Newtonsoft.Json.Linq;

namespace Whatsapp.web.net;

public class DeliveryInfo
{
    public DeliveryInfo(dynamic data)
    {
        Patch(data);
    }

    private void Patch(dynamic data)
    {
        if (data is null) return;
        if (data.Type == JTokenType.Null) return;
        Id = data.id;
        T = Util.ConvertToDate(data.t);
    }

    public string Id { get; set; }

    public DateTime? T { get; set; }
}