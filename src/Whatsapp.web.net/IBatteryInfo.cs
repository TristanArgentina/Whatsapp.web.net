namespace Whatsapp.web.net;

public interface IBatteryInfo
{
    /// <summary>
    /// The current battery percentage
    /// </summary>
    int Battery { get; set; }
    /// <summary>
    /// Indicates if the phone is plugged in (true) or not (false)
    /// </summary>
    bool Plugged { get; set; }
}