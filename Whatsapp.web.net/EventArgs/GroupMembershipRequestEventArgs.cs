using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class GroupMembershipRequestEventArgs : DispatcherEventArg
{
    public GroupNotification Notification { get; }

    public GroupMembershipRequestEventArgs(GroupNotification notification)
        : base(DispatcherEventsType.GROUP_MEMBERSHIP_REQUEST)
    {
        Notification = notification;
    }
}