namespace Whatsapp.web.net.Domains;

public class Order
{
    public List<Product> Products { get; set; }
    public string Subtotal { get; set; }
    public string Total { get; set; }
    public string Currency { get; set; }
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