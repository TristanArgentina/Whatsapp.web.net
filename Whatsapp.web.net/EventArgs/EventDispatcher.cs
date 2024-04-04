using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.EventArgs;

public class EventDispatcher : IEventDispatcher
{
    public EventDispatcher()
    {
    }

    public event EventHandler<MessageCreateEventArgs>? MessageCreateEvent;
    public event EventHandler<MessageReceivedEventArgs>? MessageReceivedEvent;
    public event EventHandler<MessageCiphertextEventArgs>? MessageCiphertextEvent;
    public event EventHandler<MessageReactionEventArgs>? MessageReactionEvent;
    public event EventHandler<MessageACKEventArg>? MessageACKEvent;
    public event EventHandler<MessageEditEventArgs>? MessageEditEvent;
    public event EventHandler<DispatcherEventArg>? DispatchEventGeneric;
    public event EventHandler<AuthenticatedEventArg>? AuthenticatedEvent;
    public event EventHandler<LoadingScreenEventArg>? LoadingScreenEvent;
    public event EventHandler<RevokedEveryoneEventArg>? RevokedEveryoneEvent;
    public event EventHandler<ContactChangedEventArg>? ContactChangedEvent;
    public event EventHandler<UnreadCountEventArg>? UnreadCountEvent;
    public event EventHandler<MediaUploadedEventArg>? MediaUploadedEvent;
    public event EventHandler<StateChangedEventArg>? StateChangedEvent;
    public event EventHandler<DisconnectedEventArgs>? DisconnectedEvent;
    public event EventHandler<BatteryChangedEventArgs>? BatteryChangedEvent;
    public event EventHandler<IncomingCallEventArgs>? IncomingCallEvent;
    public event EventHandler<ChatRemovedEventArgs>? ChatRemovedEvent;
    public event EventHandler<ChatArchivedEventArgs>? ChatArchivedEvent;
    public event EventHandler<AuthenticationFailureEventArgs>? AuthenticationFailureEvent;
    public event EventHandler<QRReceivedEventArgs>? QRReceivedEvent;
    public event EventHandler<GroupJoinEventArgs>? GroupJoinEvent;
    public event EventHandler<GroupLeaveEventArgs>? GroupLeaveEvent;
    public event EventHandler<GroupAdminChangedEventArgs>? GroupAdminChangedEvent;
    public event EventHandler<GroupMembershipRequestEventArgs>? GroupMembershipRequestEvent;
    public event EventHandler<GroupUpdateEventArgs>? GroupUpdateEvent;
    public event EventHandler<DispatcherEventArg>? RemoteSessionSavedEvent;
    public event EventHandler<ReadyEventArgs>? ReadyEvent;


    public event EventHandler<RevokedMeEventArg>? RevokedMeEvent;
    public void EmitAuthenticated(ClientInfo info, object? @object = null)
    {
        Task.Run(() =>
        {
            var eventArg = new AuthenticatedEventArg(info, @object);
            DispatchEventGeneric?.Invoke(this, eventArg);
            AuthenticatedEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitReady()
    {
        Task.Run(() =>
        {
            var eventArg = new ReadyEventArgs();
            DispatchEventGeneric?.Invoke(this, eventArg);
            ReadyEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitLoadingScreen(int percent, string message)
    {
        Task.Run(() =>
        {
            var arg = new LoadingScreenEventArg(percent, message);
            DispatchEventGeneric?.Invoke(this, arg);
            LoadingScreenEvent?.Invoke(this, arg);
        });
    }


    public void EmitRevokedEveryone(Message message, Message? revokedMsg)
    {
        Task.Run(() =>
        {
            var eventArg = new RevokedEveryoneEventArg(message, revokedMsg);
            DispatchEventGeneric?.Invoke(this, eventArg);
            RevokedEveryoneEvent?.Invoke(this, eventArg);
        });
    }


    public void EmitContactChanged(Message message, string oldId, string newId, bool isContact)
    {
        Task.Run(() =>
        {
            var eventArg = new ContactChangedEventArg(message, oldId, newId, isContact);
            DispatchEventGeneric?.Invoke(this, eventArg);
            ContactChangedEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitRevokedMe(Message message)
    {
        Task.Run(() =>
        {
            var eventArg = new RevokedMeEventArg(message);
            DispatchEventGeneric?.Invoke(this, eventArg);
            RevokedMeEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitMessageACK(Message message, MessageAck messageAsk)
    {
        Task.Run(() =>
        {
            var eventArg = new MessageACKEventArg(message, messageAsk);
            DispatchEventGeneric?.Invoke(this, eventArg);
            MessageACKEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitUnreadCount(string chatId)
    {
        Task.Run(() =>
        {
            var eventArg = new UnreadCountEventArg(chatId);
            DispatchEventGeneric?.Invoke(this, eventArg);
            UnreadCountEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitMediaUploaded(Message message)
    {
        Task.Run(() =>
        {
            var eventArg = new MediaUploadedEventArg(message);
            DispatchEventGeneric?.Invoke(this, eventArg);
            MediaUploadedEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitStateChanged(dynamic state)
    {
        Task.Run(() =>
        {
            var eventArg = new StateChangedEventArg(state);
            DispatchEventGeneric?.Invoke(this, eventArg);
            StateChangedEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitDisconnected(dynamic state)
    {
        Task.Run(() =>
        {
            var eventArg = new DisconnectedEventArgs(state);
            DispatchEventGeneric?.Invoke(this, eventArg);
            DisconnectedEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitBatteryChanged(int battery, bool plugged)
    {
        Task.Run(() =>
        {
            var eventArg = new BatteryChangedEventArgs(battery, plugged);
            DispatchEventGeneric?.Invoke(this, eventArg);
            BatteryChangedEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitIncomingCall(Call call)
    {
        Task.Run(() =>
        {
            var eventArg = new IncomingCallEventArgs(call);
            DispatchEventGeneric?.Invoke(this, eventArg);
            IncomingCallEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitMessageReaction(Reaction reaction)
    {
        Task.Run(() =>
        {
            var eventArg = new MessageReactionEventArgs(reaction);
            DispatchEventGeneric?.Invoke(this, eventArg);
            MessageReactionEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitChatRemoved(string chatId)
    {
        Task.Run(() =>
        {
            var eventArg = new ChatRemovedEventArgs(chatId);
            DispatchEventGeneric?.Invoke(this, eventArg);
            ChatRemovedEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitChatArchived(string chatId, bool currState, bool prevState)
    {
        Task.Run(() =>
        {
            var eventArg = new ChatArchivedEventArgs(chatId, currState, prevState);
            DispatchEventGeneric?.Invoke(this, eventArg);
            ChatArchivedEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitMessageEdit(Message message, string newBody, string prevBody)
    {
        Task.Run(() =>
        {
            var eventArg = new MessageEditEventArgs(message, newBody, prevBody);
            DispatchEventGeneric?.Invoke(this, eventArg);
            MessageEditEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitMessageCiphertext(Message message)
    {
        Task.Run(() =>
        {
            var eventArg = new MessageCiphertextEventArgs(message);
            DispatchEventGeneric?.Invoke(this, eventArg);
            MessageCiphertextEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitAuthenticationFailure(object payload)
    {
        Task.Run(() =>
        {
            var eventArg = new AuthenticationFailureEventArgs(payload);
            DispatchEventGeneric?.Invoke(this, eventArg);
            AuthenticationFailureEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitQRReceived(string qr)
    {
        Task.Run(() =>
        {
            var eventArg = new QRReceivedEventArgs(qr);
            DispatchEventGeneric?.Invoke(this, eventArg);
            QRReceivedEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitGroupJoin(GroupNotification notification)
    {
        Task.Run(() =>
        {
            var eventArg = new GroupJoinEventArgs(notification);
            DispatchEventGeneric?.Invoke(this, eventArg);
            GroupJoinEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitGroupLeave(GroupNotification notification)
    {
        Task.Run(() =>
        {
            var eventArg = new GroupLeaveEventArgs(notification);
            DispatchEventGeneric?.Invoke(this, eventArg);
            GroupLeaveEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitGroupAdminChanged(GroupNotification notification)
    {
        Task.Run(() =>
        {
            var eventArg = new GroupAdminChangedEventArgs(notification);
            DispatchEventGeneric?.Invoke(this, eventArg);
            GroupAdminChangedEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitGroupMembershipRequest(GroupNotification notification)
    {
        Task.Run(() =>
        {
            var eventArg = new GroupMembershipRequestEventArgs(notification);
            DispatchEventGeneric?.Invoke(this, eventArg);
            GroupMembershipRequestEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitGroupUpdate(GroupNotification notification)
    {
        Task.Run(() =>
        {
            var eventArg = new GroupUpdateEventArgs(notification);
            DispatchEventGeneric?.Invoke(this, eventArg);
            GroupUpdateEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitMessageCreate(Message message)
    {
        Task.Run(() =>
        {
            var eventArg = new MessageCreateEventArgs(message);
            DispatchEventGeneric?.Invoke(this, eventArg);
            MessageCreateEvent?.Invoke(this, eventArg);
        });
    }

    public void EmitMessageReceived(Message message)
    {
        Task.Run(() =>
        {
            var eventArg = new MessageReceivedEventArgs(message);
            DispatchEventGeneric?.Invoke(this, eventArg);
            MessageReceivedEvent?.Invoke(this, eventArg);
        });
    }
}