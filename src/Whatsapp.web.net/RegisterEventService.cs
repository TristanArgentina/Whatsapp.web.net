using PuppeteerSharp;
using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net;

public class RegisterEventService : IRegisterEventService
{
    private readonly IEventDispatcher? _eventDispatcher;

    private Message _lastMessage;

    public RegisterEventService(IEventDispatcher? eventDispatcher)
    {
        _eventDispatcher = eventDispatcher;
    }

    public void Register(IPage page)
    {
        page.ExposeFunctionAsync("onAddMessageEvent", OnAddMessageEvent());
        page.ExposeFunctionAsync("onChangeMessageTypeEvent", OnChangeMessageTypeEvent());
        page.ExposeFunctionAsync("onChangeMessageEvent", OnChangeMessageEvent());
        page.ExposeFunctionAsync("onRemoveMessageEvent", OnRemoveMessageEvent());
        page.ExposeFunctionAsync("onMessageAckEvent", OnMessageAckEvent());
        page.ExposeFunctionAsync("onChatUnreadCountEvent", OnChatUnreadCountEvent());
        page.ExposeFunctionAsync("onMessageMediaUploadedEvent", OnMessageMediaUploadedEvent());
        page.ExposeFunctionAsync("onBatteryStateChangedEvent", OnBatteryStateChangedEvent());
        page.ExposeFunctionAsync("onIncomingCall", OnIncomingCall());
        page.ExposeFunctionAsync("onReaction", OnReaction());
        page.ExposeFunctionAsync("onRemoveChatEvent", OnRemoveChatEvent());
        page.ExposeFunctionAsync("onArchiveChatEvent", OnArchiveChatEvent());
        page.ExposeFunctionAsync("onEditMessageEvent", OnEditMessageEvent());
        page.ExposeFunctionAsync("onAddMessageCiphertextEvent", OnAddMessageCiphertextEvent());
        //TODO: missing implement
        //await page.ExposeFunctionAsync("onAppStateChangedEvent", OnAppStateChangedEvent());
    }



    //TODO: missing implement
    //private async Func<dynamic, bool> OnAppStateChangedEvent()
    //{
    //    return (dynamic state) =>
    //    {
    //        _eventDispatcher!.EmitStateChanged(state);

    //        var ACCEPTED_STATES = new List<WAState>
    //        {
    //            WAState.CONNECTED,
    //            WAState.OPENING,
    //            WAState.PAIRING,
    //            WAState.TIMEOUT
    //        };

    //        if (_options.TakeoverOnConflict)
    //        {
    //            ACCEPTED_STATES.Add(WAState.CONFLICT);

    //            if (state == WAState.CONFLICT)
    //            {
    //                await Task.Delay(_options.TakeoverTimeoutMs);
    //                await PupPage.EvaluateFunctionAsync("() => window.Store.AppState.takeover()");
    //            }
    //        }

    //        if (!ACCEPTED_STATES.Contains(state))
    //        {
    //            /**
    //             * Emitted when the client has been disconnected
    //             * @event Client#disconnected
    //             * @param {WAState|"NAVIGATION"} reason reason that caused the disconnect
    //             */
    //            await _options.AuthStrategy.Disconnect();
    //            _eventDispatcher!.EmitDisconnected(state);
    //            Destroy();
    //        }
    //    };
    //}

    private Func<dynamic, bool> OnAddMessageCiphertextEvent()
    {
        return msg =>
        {

            _eventDispatcher!.EmitMessageCiphertext(Message.Create(msg));
            return true;
        };
    }

    private Func<dynamic, string, string, bool> OnEditMessageEvent()
    {
        return (msg, newBody, prevBody) =>
        {

            if (msg.type == MessageTypes.REVOKED)
            {
                return false;
            }

            _eventDispatcher!.EmitMessageEdit(Message.Create(msg), newBody, prevBody);

            return true;
        };
    }

    private Func<string, bool, bool, bool> OnArchiveChatEvent()
    {
        return (chatId, currState, prevState) =>
        {
            _eventDispatcher!.EmitChatArchived(chatId, currState, prevState);
            return true;
        };
    }

    private Func<string, bool> OnRemoveChatEvent()
    {
        return (chatId) =>
        {
            _eventDispatcher!.EmitChatRemoved(chatId);
            return true;
        };
    }

    private Func<dynamic, bool> OnReaction()
    {
        return reactions =>
        {
            foreach (var reaction in reactions)
            {
                _eventDispatcher!.EmitMessageReaction(new Reaction(reaction));
            }

            return true;
        };
    }

    private Func<dynamic, bool> OnIncomingCall()
    {
        return call =>
        {
            var cll = new Call(call);
            _eventDispatcher!.EmitIncomingCall(cll);
            return true;
        };
    }

    private Func<dynamic, bool> OnBatteryStateChangedEvent()
    {
        return (state) =>
        {
            int? battery = state.battery;
            bool? plugged = state.plugged;
            if (battery is null) return false;

            _eventDispatcher!.EmitBatteryChanged(battery.Value, plugged.Value);
            return true;
        };
    }

    private Func<dynamic, bool> OnMessageMediaUploadedEvent()
    {
        return (msg) =>
        {
            _eventDispatcher!.EmitMediaUploaded(Message.Create(msg));
            return true;
        };
    }

    private Func<dynamic, bool> OnChatUnreadCountEvent()
    {
        return (data) =>
        {
            if (data is null || data.id is null) return false;

            _eventDispatcher!.EmitUnreadCount(data.id.ToString());

            return true;
        };
    }

    private Func<dynamic, dynamic, bool> OnMessageAckEvent()
    {
        return (msg, ack) =>
        {
            var messageAsk = (MessageAck)Enum.Parse<MessageAck>(ack.ToString(), true);
            _eventDispatcher!.EmitMessageACK(Message.Create(msg), messageAsk);
            return true;

        };
    }

    private Func<dynamic, bool> OnRemoveMessageEvent()
    {
        return msg =>
        {
            if (!(bool)msg.isNewMsg) return false;
            _eventDispatcher!.EmitRevokedMe(Message.Create(msg));
            return true;
        };
    }

    private Func<dynamic, bool> OnChangeMessageEvent()
    {
        return msg =>
        {
            // Create a new Message object
            Message message = Message.Create(msg);
            _eventDispatcher!.EmitMessageChanged(message);
            if (msg.type != "revoked")
            {
                _lastMessage = message;
            }

            // Determine if the message is related to a participant or a contact changing their phone number
            bool isParticipant = msg.type == MessageTypes.GP2 && msg.subtype == "modify";
            bool isContact = msg.type == MessageTypes.NOTIFICATION_TEMPLATE && msg.subtype == "change_number";

            if (!isParticipant && !isContact) return false;
           

            // Extract old and new IDs
            string newId = isParticipant ? message.Recipients[0].Id : msg.to;
            var oldId = isParticipant ? message.Author.Id : message.TemplateParams.FirstOrDefault(id => id != newId);

            _eventDispatcher!.EmitContactChanged(message, oldId, newId, isContact);
            return true;
        };
    }

    private Func<dynamic, bool> OnChangeMessageTypeEvent()
    {
        return (dynamic msg) =>
        {
            if ((string)msg.type != "revoked") return false;
            // Create a new Message object
            var message = Message.Create(msg);

            Message revoked_msg = null;

            // Check if there is a last message and if its ID matches the revoked message ID
            if (_lastMessage != null && message.Id.Id == _lastMessage.Id.Id)
            {
                // Create a new Message object for the last message
                revoked_msg = _lastMessage;
            }
            _eventDispatcher!.EmitRevokedEveryone(message, revoked_msg);
            return true;

        };
    }

    private Func<dynamic, bool> OnAddMessageEvent()
    {
        return (dynamic msg) =>
        {
            if (msg.type == MessageTypes.GP2)
            {
                var notification = new GroupNotification(msg);
                var strings = new[] { "add", "invite", "linked_group_join" };
                if (strings.Contains((string)msg.subtype))
                {
                    /**
                 * Emitted when a user joins the chat via invite link or is added by an admin.
                 * @event Client#group_join
                 * @param {GroupNotification} notification GroupNotification with more information about the action
                 */
                    _eventDispatcher!.EmitGroupJoin(notification);
                }
                else if (msg.subtype == "remove" || msg.subtype == "leave")
                {
                    /**
                 * Emitted when a user leaves the chat or is removed by an admin.
                 * @event Client#group_leave
                 * @param {GroupNotification} notification GroupNotification with more information about the action
                 */
                    _eventDispatcher!.EmitGroupLeave(notification);
                }
                else if (msg.subtype == "promote" || msg.subtype == "demote")
                {
                    /**
                 * Emitted when a current user is promoted to an admin or demoted to a regular user.
                 * @event Client#group_admin_changed
                 * @param {GroupNotification} notification GroupNotification with more information about the action
                 */
                    _eventDispatcher!.EmitGroupAdminChanged(notification);
                }
                else if (msg.subtype == "created_membership_requests")
                {
                    /**
                 * Emitted when some user requested to join the group
                 * that has the membership approval mode turned on
                 * @event Client#group_membership_request
                 * @param {GroupNotification} notification GroupNotification with more information about the action
                 * @param {string} notification.chatId The group ID the request was made for
                 * @param {string} notification.author The user ID that made a request
                 * @param {number} notification.timestamp The timestamp the request was made at
                 */
                    _eventDispatcher!.EmitGroupMembershipRequest(notification);
                }
                else
                {
                    /**
                 * Emitted when group settings are updated, such as subject, description or picture.
                 * @event Client#group_update
                 * @param {GroupNotification} notification GroupNotification with more information about the action
                 */
                    _eventDispatcher!.EmitGroupUpdate(notification);
                }

                return true;
            }

            var message = Message.Create(msg);


            // Emitted when a new message is created, which may include the current user's own messages.
            //The message that was created
            _eventDispatcher!.EmitMessageCreate(message);

            if (message.Id.FromMe)
            {
                // Emitted when a new message is received.
                // The message that was received
                _eventDispatcher!.EmitMessageReceived(message);
            }

            return true;
        };
    }
}