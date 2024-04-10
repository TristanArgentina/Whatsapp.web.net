using System.Drawing;
using Whatsapp.web.net.Domains;
using Whatsapp.web.net.EventArgs;
using Whatsapp.web.net.Extensions;

namespace Whatsapp.web.net.example;

public class HandleEvents
{
    public Client Client { get; set; }
    private readonly IEventDispatcher? _eventDispatcher;

    public HandleEvents(IEventDispatcher? eventDispatcher)
    {
        _eventDispatcher = eventDispatcher;
    }

    public void SetHandle()
    {
        // Define event handlers
        _eventDispatcher.LoadingScreenEvent += OnLoadingScreen();
        _eventDispatcher.QRReceivedEvent += OnQRReceived();
        _eventDispatcher.AuthenticatedEvent += OnAuthenticated();
        _eventDispatcher.AuthenticationFailureEvent += OnAuthenticationFailure();
        _eventDispatcher.ReadyEvent += OnReady();
        _eventDispatcher.MessageReceivedEvent += OnMessageReceived();
        _eventDispatcher.MessageCreateEvent += OnMessageCreate();
        _eventDispatcher.MessageCiphertextEvent += OnMessageCiphertext();
        _eventDispatcher.RevokedEveryoneEvent += OnRevokedEveryone();
        _eventDispatcher.RevokedMeEvent += OnRevokedMe();
        _eventDispatcher.MessageACKEvent += OnMessageACK();
        _eventDispatcher.GroupJoinEvent += OnGroupJoin();
        _eventDispatcher.GroupLeaveEvent += OnGroupLeave();
        _eventDispatcher.GroupUpdateEvent += OnGroupUpdate();
        _eventDispatcher.StateChangedEvent += OnStateChanged();
        _eventDispatcher.MessageReactionEvent += OnMessageReaction();

        // Change to false if you don't want to reject incoming calls
        var rejectCalls = true;

        _eventDispatcher.IncomingCallEvent += OnIncomingCall(rejectCalls);
        _eventDispatcher.DisconnectedEvent += OnDisconnected();
        _eventDispatcher.ContactChangedEvent += OnContactChanged();
        _eventDispatcher.GroupAdminChangedEvent += OnGroupAdminChanged();
        _eventDispatcher.GroupMembershipRequestEvent += OnGroupMembershipRequest();

    }

    private EventHandler<MessageReactionEventArgs>? OnMessageReaction()
    {
        return (_, args) =>
        {
            ConsoleWriteLineEvent("MessageReaction", args.Reaction);
        };
    }



    private static EventHandler<ReadyEventArgs>? OnReady()
    {
        return (_, _) => { ConsoleWriteLineEvent("READY"); };
    }


    private EventHandler<GroupMembershipRequestEventArgs>? OnGroupMembershipRequest()
    {
        return async (_, args) =>
        {
            ConsoleWriteLineEvent("GroupMembershipRequest", args.Notification);
            // You can approve or reject the newly appeared membership request: 
            await Client.Chat.ApproveMembership(args.Notification.ChatId.Id, args.Notification.Author.Id);
            await Client.Chat.RejectMembership(args.Notification.ChatId.Id, args.Notification.Author.Id);
        };
    }

    private static EventHandler<GroupAdminChangedEventArgs>? OnGroupAdminChanged()
    {
        return (_, args) =>
        {
            ConsoleWriteLineEvent("GroupAdminChanged", args.Notification);
            if (args.Notification.Type == "promote")
            {
                /**
                  * Emitted when a current user is promoted to an admin.
                  * {@link notification.Author} is a user who performs the action of promoting/demoting the current user.
                  */
                Console.WriteLine($"You were promoted by {args.Notification.Author}");
            }
            else if (args.Notification.Type == "demote")
            {
                /** Emitted when a current user is demoted to a regular user. */
                Console.WriteLine($"You were demoted by {args.Notification.Author}");
            }
        };
    }

    private EventHandler<ContactChangedEventArg>? OnContactChanged()
    {
        return async (_, args) =>
        {
            ConsoleWriteLineEvent("GroupAdminChanged", args.Message);
            var message = args.Message;
            var oldId = args.OldId;
            var newId = args.NewId;
            var isContact = args.IsContact;

            // The time the event occurred.
            var eventTime = message.Timestamp.Value;

            var fromId = message.To ?? message.From;
            Console.WriteLine(
                $"The contact {oldId.Substring(0, oldId.Length - 5)}" +
                $"{(!isContact ? " that participates in group " +
                                 $"{Client.Chat.Get(fromId._serialized).Result.Name} " : " ")}" +
                $"changed their phone number\nat {eventTime}.\n" +
                $"Their new phone number is {newId.Substring(0, newId.Length - 5)}.\n");

            /**
             * Information about the @param {message}:
             *
             * 1. If a notification was emitted due to a group participant changing their phone number:
             * @param {message.Author} is a participant's id before the change.
             * @param {message.Recipients[0]} is a participant's id after the change (a new one).
             *
             * 1.1 If the contact who changed their number WAS in the current user's contact list at the time of the change:
             * @param {message.To} is a group chat id the event was emitted in.
             * @param {message.From} is a current user's id that got an notification message in the group.
             * Also the @param {message.FromMe} is TRUE.
             *
             * 1.2 Otherwise:
             * @param {message.From} is a group chat id the event was emitted in.
             * @param {message.To} is @type {undefined}.
             * Also @param {message.FromMe} is FALSE.
             *
             * 2. If a notification was emitted due to a contact changing their phone number:
             * @param {message.TemplateParams} is an array of two user's ids:
             * the old (before the change) and a new one, stored in alphabetical order.
             * @param {message.From} is a current user's id that has a chat with a user,
             * whos phone number was changed.
             * @param {message.To} is a user's id (after the change), the current user has a chat with.
             */
        };
    }

    private static EventHandler<DisconnectedEventArgs>? OnDisconnected()
    {
        return (_, args) =>
        {
            ConsoleWriteLineEvent("Disconnected", args.State);
        };
    }


    private EventHandler<IncomingCallEventArgs>? OnIncomingCall(bool rejectCalls)
    {
        return async (_, args) =>
        {
            Console.WriteLine("Call received, rejecting. GOTO Line 261 to disable " + args.Call);
            if (rejectCalls) args.Call.Reject(Client);
            await Client.Message.Send(args.Call.From,
                $"[{(args.Call.FromMe ? "Outgoing" : "Incoming")}] Phone call from {args.Call.From}, type {(args.Call.IsGroup ? "group" : "")} {(args.Call.IsVideo ? "video" : "audio")} call. {(rejectCalls ? "This call was automatically rejected by the script." : "")}");
        };
    }

    private static EventHandler<StateChangedEventArg>? OnStateChanged()
    {
        return (_, args) => { Console.WriteLine("CHANGE STATE " + args.State); };
    }

    private static EventHandler<GroupUpdateEventArgs>? OnGroupUpdate()
    {
        return (_, args) =>
        {
            // Group picture, subject or description has been updated.
            Console.WriteLine("update " + args.Notification);
        };
    }

    private EventHandler<GroupLeaveEventArgs>? OnGroupLeave()
    {
        return (_, args) =>
        {
            // User has left or been kicked from the group.
            Console.WriteLine("leave " + args.Notification);
            args.Notification.Reply(Client, "User left.");
        };
    }

    private EventHandler<GroupJoinEventArgs>? OnGroupJoin()
    {
        return (_, args) =>
        {
            // User has joined or been added to the group.
            Console.WriteLine("join " + args.Notification);
            args.Notification.Reply(Client, "User joined.");
        };
    }

    private EventHandler<MessageACKEventArg>? OnMessageACK()
    {
        return (_, args) =>
        {
            if (args.MessageAsk == MessageAck.ACK_READ)
            {
                // The message was read
            }
        };
    }

    private EventHandler<RevokedMeEventArg>? OnRevokedMe()
    {
        return (_, args) =>
        {
            // Fired whenever a message is only deleted in your own view.
            Console.WriteLine(args.Message.Body); // message before it was deleted.
        };
    }

    private EventHandler<RevokedEveryoneEventArg>? OnRevokedEveryone()
    {
        return async (_, args) =>
        {
            // Fired whenever a message is deleted by anyone (including you)
            Console.WriteLine(args.Message); // message after it was deleted.
            if (args.RevokedMsg != null)
            {
                Console.WriteLine(args.RevokedMsg); // message before it was deleted.
            }
        };
    }

    private EventHandler<MessageCiphertextEventArgs>? OnMessageCiphertext()
    {
        return (_, args) =>
        {
            var msg = args.Message;
            // Receiving new incoming messages that have been encrypted
            // msg.Type == "ciphertext"
            // msg.Body = "Waiting for this message. Check your phone.";

            // do stuff here
        };
    }

    private EventHandler<MessageCreateEventArgs>? OnMessageCreate()
    {
        return async (_, args) =>
        {
            ConsoleWriteLineEvent("MessageCreate", args.Message);
            var msg = args.Message;

            // Unpins a message
            if (msg.Id.FromMe && msg.Body.StartsWith("!unpin"))
            {
                var pinnedMsg = await msg.GetQuotedMessage(Client);
                if (pinnedMsg != null)
                {
                    // Will unpin a message
                    var result = await pinnedMsg.Unpin(Client);
                    Console.WriteLine(result); // True if the operation completed successfully, false otherwise
                }
            }
        };
    }

    private static EventHandler<LoadingScreenEventArg>? OnLoadingScreen()
    {
        return (_, args) => { Console.WriteLine($"LOADING SCREEN {args.Percent}% {args.Message}"); };
    }

    private EventHandler<MessageReceivedEventArgs>? OnMessageReceived()
    {
        return async (_, args) =>
        {
            var msg = args.Message;
            ConsoleWriteLineEvent("MESSAGE RECEIVED", msg);

            if (msg.Body == "!ping reply")
            {
                var chatId = msg.GetChatId(Client);
                await msg.Reply(Client, chatId, "pong");
            }
            else if (msg.Body == "!ping")
            {
                await Client.Message.Send(msg.From, "pong");
            }
            else if (msg.Body.StartsWith("!sendto "))
            {
                var parts = msg.Body.Split(' ');
                var number = parts[1];
                var messageIndex = msg.Body.IndexOf(number) + number.Length;
                var message = msg.Body.Substring(messageIndex);
                number = number.Contains("@c.us") ? number : $"{number}@c.us";
                var chat = await msg.GetChat(Client);
                chat.SendSeen(Client);
                await Client.Message.Send(number, message);
            }
            else if (msg.Body.StartsWith("!subject "))
            {
                var chat = await msg.GetChat(Client);
                if (chat is GroupChat groupChat)
                {
                    var newSubject = msg.Body.Substring(9);
                    groupChat.SetSubject(Client, newSubject);
                }
                else
                {
                    await msg.Reply(Client, "This command can only be used in a group!");
                }
            }
            else if (msg.Body.StartsWith("!echo "))
            {
                await msg.Reply(Client, msg.Body.Substring(6));
            }
            else if (msg.Body.StartsWith("!preview "))
            {
                var text = msg.Body.Substring(9);
                await msg.Reply(Client, null, text, new MessageOptions { LinkPreview = true });
            }
            else if (msg.Body.StartsWith("!desc "))
            {
                var chat = await msg.GetChat(Client);
                if (chat is GroupChat groupChat)
                {
                    var newDescription = msg.Body.Substring(6);
                    groupChat.SetDescription(Client, newDescription);
                }
                else
                {
                    await msg.Reply(Client, "This command can only be used in a group!");
                }
            }
            else if (msg.Body == "!leave")
            {
                var chat = await msg.GetChat(Client);
                if (chat is GroupChat groupChat)
                {
                    groupChat.Leave(Client);
                }
                else
                {
                    await msg.Reply(Client, "This command can only be used in a group!");
                }
            }
            else if (msg.Body.StartsWith("!join "))
            {
                var inviteCode = msg.Body.Split(' ')[1];
                try
                {
                    await Client.Group.AcceptInvite(new InviteV4(inviteCode));
                    await msg.Reply(Client, "Joined the group!");
                }
                catch (Exception e)
                {
                    await msg.Reply(Client, "That invite code seems to be invalid.");
                }
            }
            else if (msg.Body.StartsWith("!addmembers"))
            {
                var group = (GroupChat)await msg.GetChat(Client);
                var result =
                    await group.AddParticipants(Client, new[] { "number1@c.us", "number2@c.us", "number3@c.us" });
                Console.WriteLine(result);
            }
            else if (msg.Body == "!creategroup")
            {
                string[] participantsToAdd = ["number1@c.us", "number2@c.us", "number3@c.us"];
                var result = await Client.Group.CreateGroup("Group Title", participantsToAdd);
                Console.WriteLine(result);
            }
            else if (msg.Body == "!groupinfo")
            {
                var chat = await msg.GetChat(Client);
                if (chat is GroupChat groupChat)
                {
                    await msg.Reply(Client, $@"
                            *Group Details*
                            Name: {chat.Name}
                            Description: {groupChat.Description}
                            Created At: {groupChat.Creation}
                            Created By: {groupChat.Owner.User}
                            Participant count: {groupChat.Participants.Count}
                        ");
                }
                else
                {
                    await msg.Reply(Client, "This command can only be used in a group!");
                }
            }
            else if (msg.Body == "!chats")
            {
                var chats = await Client.Chat.Get();
                await Client.Message.Send(msg.From, $"The bot has {chats.Length} chats open.");
            }
            else if (msg.Body == "!info")
            {
                var info = Client.ClientInfo;
                await Client.Message.Send(msg.From, $@"
                        *Connection info*
                        User name: {info.PushName}
                        My number: {info.Id.User}
                        Platform: {info.Platform}
                    ");
            }
            else if (msg.Body == "!mediainfo" && msg.HasMedia)
            {
                var attachmentData = await msg.DownloadMedia(Client);
                await msg.Reply(Client, $@"
                        *Media info*
                        MimeType: {attachmentData.Mimetype}
                        Filename: {attachmentData.Filename}
                        Data (length): {attachmentData.Data.Length}
                    ");
            }
            else if (msg.Body == "!quoteinfo" && msg.HasQuotedMsg)
            {
                var quotedMsg = await msg.GetQuotedMessage(Client);
                await quotedMsg.Reply(Client, $@"
                        ID: {quotedMsg.Id.Id}
                        Type: {quotedMsg.Type}
                        Author: {quotedMsg.Author ?? quotedMsg.From}
                        Timestamp: {quotedMsg.Timestamp}
                        Has Media? {quotedMsg.HasMedia}
                    ");
            }
            else if (msg.Body == "!resendmedia" && msg.HasQuotedMsg)
            {
                var quotedMsg = await msg.GetQuotedMessage(Client);
                if (quotedMsg.HasMedia)
                {
                    var attachmentData = await quotedMsg.DownloadMedia(Client);
                    await Client.Message.Send(msg.From, attachmentData,
                        new MessageOptions { Caption = "Here's your requested media." });
                }

                if (quotedMsg.HasMedia && quotedMsg.Type == MessageTypes.AUDIO)
                {
                    var audio = await quotedMsg.DownloadMedia(Client);
                    await Client.Message.Send(msg.From, audio, new MessageOptions { SendAudioAsVoice = true });
                }
            }
            else if (msg.Body == "!isviewonce" && msg.HasQuotedMsg)
            {
                var quotedMsg = await msg.GetQuotedMessage(Client);
                if (quotedMsg.HasMedia)
                {
                    var media = await quotedMsg.DownloadMedia(Client);
                    await Client.Message.Send(msg.From, media, new MessageOptions { IsViewOnce = true });
                }
            }
            else if (msg.Body == "!location")
            {
                await msg.Reply(Client, new Location(37.422, -122.084));
                await msg.Reply(Client, new Location(37.422, -122.084, new LocationOptions("Googleplex")));
                await msg.Reply(Client, new Location(37.422, -122.084,
                    new LocationOptions(null, "1600 Amphitheatre Pkwy, Mountain View, CA 94043, USA")));
                await msg.Reply(Client, new Location(37.422, -122.084,
                    new LocationOptions("Googleplex", "1600 Amphitheatre Pkwy, Mountain View, CA 94043, USA")));
            }
            else if (msg.Location != null)
            {
            }
        };
    }

    private static void ConsoleWriteLineEvent(string eventType)
    {
        Console.WriteLine($"{DateTime.Now:G}: {eventType}");
    }
    private static void ConsoleWriteLineEvent(string eventType, Message msg)
    {
        Console.WriteLine($"{DateTime.Now:G}: {eventType} -> From: '{msg.From}'  To: '{msg.To}' Content: {msg.Body}");
    }

    private static void ConsoleWriteLineEvent(string eventType, GroupNotification notification)
    {
        Console.WriteLine($"{DateTime.Now:G}: {eventType} -> From: '{notification.Author}'  To: '{notification.ChatId}' Content: {notification.Body}");
    }

    private void ConsoleWriteLineEvent(string eventType, Reaction reaction)
    {
        Console.WriteLine($"{DateTime.Now:G}: {eventType} -> From: '{reaction.SenderId}'  To: '{reaction.Key.Remote}' Reaction: {reaction.Text}");
    }

    private static void ConsoleWriteLineEvent(string eventType, WAState argsState)
    {
        Console.WriteLine($"{DateTime.Now:G}: {eventType} -> Client was logged out {argsState}");
    }
    private static EventHandler<AuthenticationFailureEventArgs>? OnAuthenticationFailure()
    {
        return (_, args) => { Console.Error.WriteLine("AUTHENTICATION FAILURE " + args.Payload); };
    }

    private static EventHandler<AuthenticatedEventArg>? OnAuthenticated()
    {
        return (_, args) =>
        {
            Console.WriteLine("AUTHENTICATED");
            Console.WriteLine("USER:");
            Console.WriteLine(args.Info);
        };
    }

    private static EventHandler<QRReceivedEventArgs>? OnQRReceived()
    {
        return (_, args) =>
        {

            Console.WriteLine("QR RECEIVED " + args.Qr);

            try
            {
                if (args.Qr is null) return;
                var code = args.Qr.ToString();
                var img = QRHelper.Generate(code!);
                QRHelper.ConsoleWrite(img);
            }
            catch (Exception e)
            {

            }
        };

    }
}