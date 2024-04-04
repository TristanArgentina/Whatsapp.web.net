using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class GroupUpdateEventArgs : DispatcherEventArg
{
    public GroupNotification Notification { get; }

    public GroupUpdateEventArgs(GroupNotification notification)
        : base(DispatcherEventsType.GROUP_UPDATE)
    {
        Notification = notification;
    }
}