using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net;

public class OrderData
{
    public List<ProductData> Products { get; set; }
    public string Subtotal { get; set; }
    public string Total { get; set; }
    public string Currency { get; set; }
    public long CreatedAt { get; set; }
}