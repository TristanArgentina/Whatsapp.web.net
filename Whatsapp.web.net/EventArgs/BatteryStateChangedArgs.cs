namespace Whatsapp.web.net.EventArgs;

public class BatteryStateChangedArgs : DispatcherEventArg
{
    public int Battery { get; }

    public bool CurrState { get; }

    public BatteryStateChangedArgs(int battery, bool currState):base(DispatcherEventsType.BATTERY_CHANGED)
    {
        Battery = battery;
        CurrState = currState;
    }
}