namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a Product on WhatsAppBusiness
/// </summary>
public class Product
{
    /// <summary>
    /// Product ID
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Price
    /// </summary>
    public string Price { get; set; }

    /// <summary>
    /// Product Thumbnail
    /// </summary>
    public string ThumbnailUrl { get; set; }

    /// <summary>
    /// Currency
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Product Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Product Quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Product metadata
    /// </summary>
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