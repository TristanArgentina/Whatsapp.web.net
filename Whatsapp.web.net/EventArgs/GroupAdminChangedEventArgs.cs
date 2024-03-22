using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class GroupAdminChangedEventArgs : DispatcherEventArg
{
    public GroupNotification Notification { get; }

    public GroupAdminChangedEventArgs(GroupNotification notification)
        : base(DispatcherEventsType.GROUP_ADMIN_CHANGED)
    {
        Notification = notification;
    }
}