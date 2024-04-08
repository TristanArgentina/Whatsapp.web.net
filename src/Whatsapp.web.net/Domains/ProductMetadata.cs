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

    public string Id { get; private set; }
    public string RetailerId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    public void Patch(dynamic data)
    {
        Id = data.id;
        RetailerId = data.railer_id;
        Name = data.name;
        Description = data.description;
    }
}