﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Whatsapp.web.net.Domains;

/// <summary>
/// Location information
/// </summary>
public class Location
{
    private LocationOptions? _locationOptions;

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
    public string Description => _locationOptions?.ToString() ?? "";

    /// <summary>
    /// URL address to be shown within a location message
    /// </summary>
    public string? Url { get; private set; }


    public Location(double latitude, double longitude, LocationOptions? locationOptions = null, string? url = null)
    {
        _locationOptions = locationOptions;
        Latitude = latitude;
        Longitude = longitude;
        Url = url;
    }

    [JsonConstructor]
    public Location(dynamic data)
    {
        Patch(data);
    }

    public Location(double latitude, double longitude, string description)
    : this(latitude, longitude, new LocationOptions(description))
    {
    }

    private void Patch(dynamic? data)
    {
        if (data is null) return;

        Latitude = (double)data.lat;
        Longitude = (double)data.lng;
        Url = data.clientUrl;
        _locationOptions = LocationOptions.Create(data.loc);
    }
}