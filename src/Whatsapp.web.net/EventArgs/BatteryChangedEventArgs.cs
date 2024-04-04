namespace Whatsapp.web.net.EventArgs;

public class BatteryChangedEventArgs : DispatcherEventArg
{
    
    public int Battery { get; }
    
    public bool Plugged { get; }

    public BatteryChangedEventArgs(int battery, bool plugged)
        : base(DispatcherEventsType.BATTERY_CHANGED)
    {
        Battery = battery;
        Plugged = plugged;
    }
}