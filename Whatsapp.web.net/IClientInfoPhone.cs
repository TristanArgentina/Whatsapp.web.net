namespace Whatsapp.web.net;

public interface IClientInfoPhone
{
    /// <summary>
    /// WhatsApp Version running on the phone
    /// </summary>
    string WaVersion { get; set; }
    /// <summary>
    /// OS Version running on the phone (iOS or Android version)
    /// </summary>
    string OsVersion { get; set; }
    /// <summary>
    /// Device manufacturer
    /// </summary>
    string DeviceManufacturer { get; set; }
    /// <summary>
    /// Device model
    /// </summary>
    string DeviceModel { get; set; }
    /// <summary>
    /// OS build number
    /// </summary>
    string OsBuildNumber { get; set; }
}