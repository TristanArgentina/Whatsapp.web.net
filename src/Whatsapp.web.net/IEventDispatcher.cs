using Whatsapp.web.net.Domains;
using Whatsapp.web.net.EventArgs;

namespace Whatsapp.web.net;

public interface IEventDispatcher
{
    event EventHandler<AuthenticatedEventArg>? AuthenticatedEvent;
    event EventHandler<LoadingScreenEventArg>? LoadingScreenEvent;
    event EventHandler<RevokedEveryoneEventArg>? RevokedEveryoneEvent;
    event EventHandler<ContactChangedEventArg>? ContactChangedEvent;
    event EventHandler<MessageACKEventArg>? MessageACKEvent;
    event EventHandler<UnreadCountEventArg>? UnreadCountEvent;
    event EventHandler<MediaUploadedEventArg>? MediaUploadedEvent;
    event EventHandler<StateChangedEventArg>? StateChangedEvent;
    event EventHandler<DisconnectedEventArgs>? DisconnectedEvent;
    event EventHandler<BatteryChangedEventArgs>? BatteryChangedEvent;
    event EventHandler<IncomingCallEventArgs>? IncomingCallEvent;
    event EventHandler<MessageReactionEventArgs>? MessageReactionEvent;
    event EventHandler<ChatRemovedEventArgs>? ChatRemovedEvent;
    event EventHandler<ChatArchivedEventArgs>? ChatArchivedEvent;
    event EventHandler<MessageEditEventArgs>? MessageEditEvent;
    event EventHandler<MessageCiphertextEventArgs>? MessageCiphertextEvent;
    event EventHandler<AuthenticationFailureEventArgs>? AuthenticationFailureEvent;
    event EventHandler<QRReceivedEventArgs>? QRReceivedEvent;
    event EventHandler<GroupJoinEventArgs>? GroupJoinEvent;
    event EventHandler<GroupLeaveEventArgs>? GroupLeaveEvent;
    event EventHandler<GroupAdminChangedEventArgs>? GroupAdminChangedEvent;
    event EventHandler<GroupMembershipRequestEventArgs>? GroupMembershipRequestEvent;
    event EventHandler<GroupUpdateEventArgs>? GroupUpdateEvent;
    event EventHandler<MessageCreateEventArgs>? MessageCreateEvent;
    event EventHandler<MessageReceivedEventArgs>? MessageReceivedEvent;
    event EventHandler<DispatcherEventArg>? RemoteSessionSavedEvent;
    event EventHandler<RevokedMeEventArg>? RevokedMeEvent;
    event EventHandler<ReadyEventArgs>? ReadyEvent;

    void EmitAuthenticated(ClientInfo info, object? @object = null);

    void EmitLoadingScreen(int percent, string message);

    void EmitRevokedEveryone(Message message, Message? revokedMsg);

    void EmitContactChanged(Message message, string oldId, string newId, bool isContact);

    void EmitRevokedMe(Message message);

    void EmitMessageACK(Message message, MessageAck messageAsk);

    void EmitUnreadCount(string chatId);

    void EmitMediaUploaded(Message message);

    void EmitStateChanged(dynamic state);

    void EmitDisconnected(dynamic state);

    void EmitBatteryChanged(int battery, bool plugged);

    void EmitIncomingCall(Call call);

    void EmitMessageReaction(Reaction reaction);

    void EmitChatRemoved(string chatId);

    void EmitChatArchived(string chatId, bool currState, bool prevState);

    void EmitMessageEdit(Message message, string newBody, string prevBody);

    void EmitMessageCiphertext(Message message);

    void EmitAuthenticationFailure(object payload);

    void EmitQRReceived(string qr);

    void EmitGroupJoin(GroupNotification notification);

    void EmitGroupLeave(GroupNotification notification);

    void EmitGroupAdminChanged(GroupNotification notification);

    void EmitGroupMembershipRequest(GroupNotification notification);

    void EmitGroupUpdate(GroupNotification notification);

    void EmitMessageCreate(Message message);

    void EmitMessageReceived(Message message);

    void EmitReady();
}