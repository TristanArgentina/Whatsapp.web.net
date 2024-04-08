namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a Order on WhatsApp
/// </summary>
public class Order
{
    public List<Product> Products { get; private set; }

    /// <summary>
    /// Order Subtotal
    /// </summary>
    public string Subtotal { get; private set; }

    /// <summary>
    /// Order Total
    /// </summary>
    public string Total { get; private set; }

    /// <summary>
    /// Order Currency
    /// </summary>
    public string Currency { get; private set; }

    /// <summary>
    /// Order Created At
    /// </summary>
    public long CreatedAt { get; private set; }

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