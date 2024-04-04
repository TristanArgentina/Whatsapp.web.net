using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class MediaUploadedEventArg : DispatcherEventArg
{
    public Message Message { get; }

    public MediaUploadedEventArg(Message message)
        : base(DispatcherEventsType.MEDIA_UPLOADED)
    {
        Message = message;
    }
}