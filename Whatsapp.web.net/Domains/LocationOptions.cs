namespace Whatsapp.web.net.Domains;

public class LocationOptions
{
    /// <summary>
    /// Name for the location
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Location address
    /// </summary>
    public string? Address { get; set; }

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Address) ) return Name;
        if (string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Address) ) return Address;
        if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Address)) return $"{Name}\n{Address}";
        return string.Empty;
    }
}