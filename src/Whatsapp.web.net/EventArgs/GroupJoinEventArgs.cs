using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class GroupJoinEventArgs : DispatcherEventArg
{
    public GroupNotification Notification { get; }

    public GroupJoinEventArgs(GroupNotification notification)
        : base(DispatcherEventsType.GROUP_JOIN)
    {
        Notification = notification;
    }
}