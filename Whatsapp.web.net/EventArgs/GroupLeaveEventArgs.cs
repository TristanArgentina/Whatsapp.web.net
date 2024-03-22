using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class GroupLeaveEventArgs : DispatcherEventArg
{
    public GroupNotification Notification { get; }

    public GroupLeaveEventArgs(GroupNotification notification)
        : base(DispatcherEventsType.GROUP_LEAVE)
    {
        Notification = notification;
    }
}