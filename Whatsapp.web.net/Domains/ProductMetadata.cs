namespace Whatsapp.web.net.Domains;

public class ProductMetadata
{
    public ProductMetadata(dynamic? data)
    {
        if (data is not null)
        {
            Patch(data);
        }

    }

    public string Id { get; set; }
    public string RetailerId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public void Patch(dynamic data)
    {
        Id = data.id;
        RetailerId = data.railer_id;
        Name = data.name;
        Description = data.description;
    }
}