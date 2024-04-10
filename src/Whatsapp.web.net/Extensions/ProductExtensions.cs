using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.Extensions;

public static class ProductExtensions
{
    public static async Task<ProductMetadata?> GetData(this Product product, Client client)
    {
        return await client.Commerce.GetProductMetadataById(product.Id);
    }
}