namespace Whatsapp.web.net.EventArgs;

public class DisconnectedEventArgs : DispatcherEventArg
{
    public DisconnectedEventArgs(dynamic data)
        : base(DispatcherEventsType.DISCONNECTED)
    {
        Patch(data);
    }

    private void Patch(dynamic? data)
    {
        if (data is null) return;
        State = data == "NAVIGATION" 
            ? WAState.NAVIGATION 
            : (WAState) Enum.Parse<WAState>(data.ToString());
    }

    /// <summary>
    /// reason that caused the disconnect
    /// </summary>
    public WAState State { get; set; }
}