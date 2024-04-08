using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Whatsapp.web.net.Domains;

public class LocationOptions
{
    [JsonConstructor]
    public LocationOptions(dynamic data)
    {
        Patch(data);
    }

    public LocationOptions(string? name, string? address = null)
    {
        Name = name;
        Address = address;
    }

    private void Patch(dynamic data)
    {
        Name = data.Value.Split('\n')[0];
        Address = data.Value.Split('\n')[1];
    }

    /// <summary>
    /// Name for the location
    /// </summary>
    public string? Name { get; private set; }

    /// <summary>
    /// Location address
    /// </summary>
    public string? Address { get; private set; }

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Address)) return Name;
        if (string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Address)) return Address;
        if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Address)) return $"{Name}\n{Address}";
        return string.Empty;
    }

    public static LocationOptions? Create(dynamic? data)
    {
        if (data is null || data.Type != JTokenType.String || string.IsNullOrEmpty(data!.ToString()))
        {
            return null;
        }
        return new LocationOptions(data);
    }
}