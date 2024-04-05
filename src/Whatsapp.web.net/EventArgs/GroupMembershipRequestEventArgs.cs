using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class GroupMembershipRequestEventArgs : DispatcherEventArg
{
    // 
    // The example of the {@link notification} output:
    // {
    //     Id: {
    //         FromMe: false,
    //         Remote: "groupId@g.us",
    //         Id: "123123123132132132",
    //         Participant: "number@c.us",
    //         Serialized: "false_groupId@g.us_123123123132132132_number@c.us"
    //     },
    //     Body: "",
    //     Type: "created_membership_requests",
    //     Timestamp: 1694456538,
    //     ChatId: "groupId@g.us",
    //     Author: "number@c.us",
    //     RecipientIds: []
    // }
    // 
    // 
    public GroupNotification Notification { get; }

    public GroupMembershipRequestEventArgs(GroupNotification notification)
        : base(DispatcherEventsType.GROUP_MEMBERSHIP_REQUEST)
    {
        Notification = notification;
    }
}