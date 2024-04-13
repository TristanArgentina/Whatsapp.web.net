namespace Whatsapp.web.net.Domains;

public class OrderData
{
    public List<ProductData> Products { get; private set; }
    public string Subtotal { get; private set; }
    public string Total { get; private set; }
    public string Currency { get; private set; }
    public DateTime CreatedAt { get; private set; }
}