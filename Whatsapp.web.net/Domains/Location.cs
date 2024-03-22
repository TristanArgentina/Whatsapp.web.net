namespace Whatsapp.web.net.Domains;

public class Location
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
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