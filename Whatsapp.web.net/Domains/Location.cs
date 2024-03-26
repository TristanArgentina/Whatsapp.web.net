namespace Whatsapp.web.net.Domains;

/// <summary>
/// Location information
/// </summary>
public class Location
{
    /// <summary>
    /// Location latitude
    /// </summary>
    public double Latitude { get; private set; }

    /// <summary>
    /// Location longitude
    /// </summary>
    public double Longitude { get; private set; }

    /// <summary>
    /// Location full description
    /// </summary>
    public LocationOptions? Description { get; private set; }

    public Location(double latitude, double longitude, LocationOptions? description = null)
    {
        Latitude = latitude;
        Longitude = longitude;
        Description = description;
    }

    public Location(dynamic data)
    {
        Patch(data);
    }

    private void Patch(dynamic? data)
    {
        if (data is null) return;

        Latitude = (double)data.lat;
        Longitude = (double)data.lng;
        Description = data.loc is string loc
            ? new LocationOptions() { Name = loc.Split('\n')[0], Address = loc.Split('\n')[1], Url = data.clientUrl }
            : null;
    }
}