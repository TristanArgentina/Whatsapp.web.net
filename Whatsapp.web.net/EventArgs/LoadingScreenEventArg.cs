namespace Whatsapp.web.net.EventArgs;

public class LoadingScreenEventArg : DispatcherEventArg
{
    public int Percent { get; }

    public string Message { get; }

    public LoadingScreenEventArg(int percent, string message) : base(DispatcherEventsType.LOADING_SCREEN)
    {
        Percent = percent;
        Message = message;
    }
}