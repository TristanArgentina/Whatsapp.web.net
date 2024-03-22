namespace Whatsapp.web.net.Domains;

public class Product
{
    public string Id { get; set; }
    public string Price { get; set; }
    public string ThumbnailUrl { get; set; }
    public string Currency { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public ProductMetadata? Data { get; set; }

    public Product(dynamic data)
    {
        if (data != null)
        {
            Patch(data);
        }
    }

    protected void Patch(dynamic? data)
    {
        if (data is null) return;

        Id = data.id;
        Price = data.price ?? string.Empty;
        ThumbnailUrl = data.thumbnailUrl;
        Currency = data.currency;
        Name = data.name;
        Quantity = data.quantity;
    }
}