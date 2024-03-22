namespace Whatsapp.web.net.EventArgs;

public class QRReceivedEventArgs : DispatcherEventArg
{
    public object Qr { get; }

    public QRReceivedEventArgs(object qr) : base(DispatcherEventsType.QR_RECEIVED)
    {
        Qr = qr;
    }
}