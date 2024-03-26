namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a Order on WhatsApp
/// </summary>
public class Order
{
    public List<Product> Products { get; set; }

    /// <summary>
    /// Order Subtotal
    /// </summary>
    public string Subtotal { get; set; }

    /// <summary>
    /// Order Total
    /// </summary>
    public string Total { get; set; }

    /// <summary>
    /// Order Currency
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Order Created At
    /// </summary>
    public long CreatedAt { get; set; }

    public Order(dynamic? data)
    {
        if (data != null)
        {
            Patch(data);
        }
    }

    protected void Patch(dynamic? data)
    {
        if (data is null) return;

        if (data.products != null)
        {
            Products = [];
            foreach (var productData in data.Products)
            {
                Products.Add(new Product(productData));
            }
        }

        Subtotal = data.subtotal;
        Total = data.total;
        Currency = data.currency;
        CreatedAt = data.createdAt;
    }
}