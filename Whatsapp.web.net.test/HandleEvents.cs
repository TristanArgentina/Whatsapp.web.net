using System.Collections.Concurrent;
using System.Drawing;
using ChatbotAI.net;
using Whatsapp.web.net.Domains;
using Whatsapp.web.net.EventArgs;
using Whatsapp.web.net.Extensions;

namespace Whatsapp.web.net.test;

public class HandleEvents
{
    private readonly Client _client;
    private readonly IEventDispatcher? _eventDispatcher;
    private readonly OpenAIOptions _openAiOptions;
    private readonly ConcurrentDictionary<string, IChatBotAI> _ChatBots = new();

    public HandleEvents(Client client, IEventDispatcher? eventDispatcher, OpenAIOptions openAiOptions)
    {
        _client = client;
        _eventDispatcher = eventDispatcher;
        _openAiOptions = openAiOptions;
    }

    public void SetHandle()
    {
        // Define event handlers
        _eventDispatcher.LoadingScreenEvent += OnLoadingScreenEvent();
        _eventDispatcher.QRReceivedEvent += OnQRReceivedEvent();
        _eventDispatcher.AuthenticatedEvent += OnAuthenticatedEvent();
        _eventDispatcher.AuthenticationFailureEvent += OnAuthenticationFailureEvent();
        _eventDispatcher.ReadyEvent += OnReadyEvent();
        _eventDispatcher.MessageReceivedEvent += OnMessageReceivedEvent();
        _eventDispatcher.MessageCreateEvent += OnMessageCreateEvent();
        _eventDispatcher.MessageCiphertextEvent += OnMessageCiphertextEvent();
        _eventDispatcher.RevokedEveryoneEvent += OnRevokedEveryoneEvent();
        _eventDispatcher.RevokedMeEvent += OnRevokedMeEvent();
        _eventDispatcher.MessageACKEvent += OnMessageACKEvent();
        _eventDispatcher.GroupJoinEvent += OnGroupJoinEvent();
        _eventDispatcher.GroupLeaveEvent += OnGroupLeaveEvent();
        _eventDispatcher.GroupUpdateEvent += OnGroupUpdateEvent();
        _eventDispatcher.StateChangedEvent += OnStateChangedEvent();

        // Change to false if you don't want to reject incoming calls
        var rejectCalls = true;

        _eventDispatcher.IncomingCallEvent += OnIncomingCallEvent(rejectCalls);
        _eventDispatcher.DisconnectedEvent += OnDisconnectedEvent();
        _eventDispatcher.ContactChangedEvent += OnContactChangedEvent();
        _eventDispatcher.GroupAdminChangedEvent += OnGroupAdminChangedEvent();
        _eventDispatcher.GroupMembershipRequestEvent += OnGroupMembershipRequestEvent();

    }

    private IChatBotAI GetChatBotAI(string key)
    {
        return _ChatBots.GetOrAdd(key, s =>
        {
            var kernel = KernelFactory.Create(_openAiOptions);
            var ai = new ChatBotAi(_openAiOptions, kernel);
            return ai;
        });
    }

    private static EventHandler<ReadyEventArgs>? OnReadyEvent()
    {
        return (sender, args) => { Console.WriteLine("READY"); };
    }


    private EventHandler<GroupMembershipRequestEventArgs>? OnGroupMembershipRequestEvent()
    {
        return async (sender, args) =>
        {
            /**
             * The example of the {@link notification} output:
             * {
             *     Id: {
             *         FromMe: false,
             *         Remote: "groupId@g.us",
             *         Id: "123123123132132132",
             *         Participant: "number@c.us",
             *         Serialized: "false_groupId@g.us_123123123132132132_number@c.us"
             *     },
             *     Body: "",
             *     Type: "created_membership_requests",
             *     Timestamp: 1694456538,
             *     ChatId: "groupId@g.us",
             *     Author: "number@c.us",
             *     RecipientIds: []
             * }
             *
             */
            Console.WriteLine(args.Notification);
            /** You can approve or reject the newly appeared membership request: */
            await _client.Chat.ApproveMembership(args.Notification.ChatId.Id, args.Notification.Author.Id);
            await _client.Chat.RejectMembership(args.Notification.ChatId.Id, args.Notification.Author.Id);
        };
    }

    private static EventHandler<GroupAdminChangedEventArgs>? OnGroupAdminChangedEvent()
    {
        return (sender, args) =>
        {
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

    private EventHandler<ContactChangedEventArg>? OnContactChangedEvent()
    {
        return async (sender, args) =>
        {
            var message = args.Message;
            var oldId = args.OldId;
            var newId = args.NewId;
            var isContact = args.IsContact;

            // The time the event occurred.
            var eventTime = message.Timestamp.Value;

            var fromId = message.To?.Id ?? message.From?.Id;
            Console.WriteLine(
                $"The contact {oldId.Substring(0, oldId.Length - 5)}" +
                $"{(!isContact ? " that participates in group " +
                                 $"{_client.Chat.Get(fromId).Result.Name} " : " ")}" +
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

    private static EventHandler<DisconnectedEventArgs>? OnDisconnectedEvent()
    {
        return (sender, args) => { Console.WriteLine("Client was logged out " + args.State); };
    }

    private EventHandler<IncomingCallEventArgs>? OnIncomingCallEvent(bool rejectCalls)
    {
        return async (sender, args) =>
        {
            Console.WriteLine("Call received, rejecting. GOTO Line 261 to disable " + args.Call);
            if (rejectCalls) args.Call.Reject(_client);
            await _client.Message.Send(args.Call.From,
                $"[{(args.Call.FromMe ? "Outgoing" : "Incoming")}] Phone call from {args.Call.From}, type {(args.Call.IsGroup ? "group" : "")} {(args.Call.IsVideo ? "video" : "audio")} call. {(rejectCalls ? "This call was automatically rejected by the script." : "")}");
        };
    }

    private static EventHandler<StateChangedEventArg>? OnStateChangedEvent()
    {
        return (sender, args) => { Console.WriteLine("CHANGE STATE " + args.State); };
    }

    private static EventHandler<GroupUpdateEventArgs>? OnGroupUpdateEvent()
    {
        return (sender, args) =>
        {
            // Group picture, subject or description has been updated.
            Console.WriteLine("update " + args.Notification);
        };
    }

    private EventHandler<GroupLeaveEventArgs>? OnGroupLeaveEvent()
    {
        return (sender, args) =>
        {
            // User has left or been kicked from the group.
            Console.WriteLine("leave " + args.Notification);
            args.Notification.Reply(_client, "User left.");
        };
    }

    private EventHandler<GroupJoinEventArgs>? OnGroupJoinEvent()
    {
        return (sender, args) =>
        {
            // User has joined or been added to the group.
            Console.WriteLine("join " + args.Notification);
            args.Notification.Reply(_client, "User joined.");
        };
    }

    private EventHandler<MessageACKEventArg>? OnMessageACKEvent()
    {
        return (sender, args) =>
        {
            if (args.MessageAsk == MessageAck.ACK_READ)
            {
                // The message was read
            }
        };
    }

    private EventHandler<RevokedMeEventArg>? OnRevokedMeEvent()
    {
        return (sender, args) =>
        {
            // Fired whenever a message is only deleted in your own view.
            Console.WriteLine(args.Message.Body); // message before it was deleted.
        };
    }

    private EventHandler<RevokedEveryoneEventArg>? OnRevokedEveryoneEvent()
    {
        return async (sender, args) =>
        {
            // Fired whenever a message is deleted by anyone (including you)
            Console.WriteLine(args.Message); // message after it was deleted.
            if (args.RevokedMsg != null)
            {
                Console.WriteLine(args.RevokedMsg); // message before it was deleted.
            }
        };
    }

    private EventHandler<MessageCiphertextEventArgs>? OnMessageCiphertextEvent()
    {
        return (sender, args) =>
        {
            var msg = args.Message;
            // Receiving new incoming messages that have been encrypted
            // msg.Type == "ciphertext"
            msg.Body = "Waiting for this message. Check your phone.";

            // do stuff here
        };
    }

    private EventHandler<MessageCreateEventArgs>? OnMessageCreateEvent()
    {
        return async (sender, args) =>
        {
            var msg = args.Message;

            Console.WriteLine($"MESSAGE CREATE: {msg}");

            // Fired on all message creations, including your own
            if (msg.Id.FromMe)
            {
                if (msg.Type == MessageTypes.VOICE)
                {
                    var downloadMedia = await msg.DownloadMedia(_client);
                    var audioBytes = Convert.FromBase64String(downloadMedia.Data);
                    var text = GetChatBotAI(msg.From.Id).GetAudioTranscription(audioBytes);
                    msg.Reply(_client, text);
                }
            }

            if (msg.Body.StartsWith("AI:Convertir a audio:"))
            {
                var audioBytes = GetChatBotAI(msg.From.Id).GenerateSpeechFromText(msg.Body.Substring(21));
                var audioBase64 = Convert.ToBase64String(audioBytes);
                var messageMedia = new MessageMedia("audio", audioBase64);
                await msg.Reply(_client, messageMedia);
            } else if (msg.Body.StartsWith("AI:"))
            {
                var response = GetChatBotAI(msg.From.Id).Ask(msg.From.Id, msg.Body.Substring(3)).Result;
                await msg.Reply(_client, response);
            }

            // Unpins a message
            if (msg.Id.FromMe && msg.Body.StartsWith("!unpin"))
            {
                var pinnedMsg = await msg.GetQuotedMessage(_client);
                if (pinnedMsg != null)
                {
                    // Will unpin a message
                    var result = await pinnedMsg.Unpin(_client);
                    Console.WriteLine(result); // True if the operation completed successfully, false otherwise
                }
            }
        };
    }

    private static EventHandler<LoadingScreenEventArg>? OnLoadingScreenEvent()
    {
        return (sender, args) => { Console.WriteLine($"LOADING SCREEN {args.Percent}% {args.Message}"); };
    }

    private EventHandler<MessageReceivedEventArgs>? OnMessageReceivedEvent()
    {
        return async (sender, args) =>
        {
            var msg = args.Message;
            Console.WriteLine("MESSAGE RECEIVED " + msg);

            //if (msg.Body.StartsWith("AI:"))
            //{
            //    var response = _ai.Ask(msg.From.Id, msg.Body.Substring(3)).Result;
            //    await msg.Reply(_client, response);
            //}

            if (msg.Body == "!ping reply")
            {
                await msg.Reply(_client, "pong");
            }
            else if (msg.Body == "!ping")
            {
                await _client.Message.Send(msg.From, "pong");
            }
            else if (msg.Body.StartsWith("!sendto "))
            {
                var parts = msg.Body.Split(' ');
                var number = parts[1];
                var messageIndex = msg.Body.IndexOf(number) + number.Length;
                var message = msg.Body.Substring(messageIndex);
                number = number.Contains("@c.us") ? number : $"{number}@c.us";
                var chat = await msg.GetChat(_client);
                chat.SendSeen(_client);
                await _client.Message.Send(number, message);
            }
            else if (msg.Body.StartsWith("!subject "))
            {
                var chat = await msg.GetChat(_client);
                if (chat is GroupChat groupChat)
                {
                    var newSubject = msg.Body.Substring(9);
                    groupChat.SetSubject(_client, newSubject);
                }
                else
                {
                    await msg.Reply(_client, "This command can only be used in a group!");
                }
            }
            else if (msg.Body.StartsWith("!echo "))
            {
                await msg.Reply(_client, msg.Body.Substring(6));
            }
            else if (msg.Body.StartsWith("!preview "))
            {
                var text = msg.Body.Substring(9);
                await msg.Reply(_client, text, null, new MessageOptions {LinkPreview = true});
            }
            else if (msg.Body.StartsWith("!desc "))
            {
                var chat = await msg.GetChat(_client);
                if (chat is GroupChat groupChat)
                {
                    var newDescription = msg.Body.Substring(6);
                    groupChat.SetDescription(_client, newDescription);
                }
                else
                {
                    await msg.Reply(_client, "This command can only be used in a group!");
                }
            }
            else if (msg.Body == "!leave")
            {
                var chat = await msg.GetChat(_client);
                if (chat is GroupChat groupChat)
                {
                    groupChat.Leave(_client);
                }
                else
                {
                    await msg.Reply(_client, "This command can only be used in a group!");
                }
            }
            else if (msg.Body.StartsWith("!join "))
            {
                var inviteCode = msg.Body.Split(' ')[1];
                try
                {
                    await _client.Group.AcceptInvite(new InviteV4(inviteCode));
                    await msg.Reply(_client, "Joined the group!");
                }
                catch (Exception e)
                {
                    await msg.Reply(_client, "That invite code seems to be invalid.");
                }
            }
            else if (msg.Body.StartsWith("!addmembers"))
            {
                var group = (GroupChat) await msg.GetChat(_client);
                var result =
                    await group.AddParticipants(_client, new[] {"number1@c.us", "number2@c.us", "number3@c.us"});
                Console.WriteLine(result);
            }
            else if (msg.Body == "!creategroup")
            {
                string[] participantsToAdd = ["number1@c.us", "number2@c.us", "number3@c.us"];
                var result = await _client.Group.CreateGroup("Group Title", participantsToAdd);
                Console.WriteLine(result);
            }
            else if (msg.Body == "!groupinfo")
            {
                var chat = await msg.GetChat(_client);
                if (chat is GroupChat groupChat)
                {
                    await msg.Reply(_client, $@"
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
                    await msg.Reply(_client, "This command can only be used in a group!");
                }
            }
            else if (msg.Body == "!chats")
            {
                var chats = await _client.Chat.Get();
                await _client.Message.Send(msg.From, $"The bot has {chats.Length} chats open.");
            }
            else if (msg.Body == "!info")
            {
                var info = _client.ClientInfo;
                await _client.Message.Send(msg.From, $@"
                        *Connection info*
                        User name: {info.PushName}
                        My number: {info.Id.User}
                        Platform: {info.Platform}
                    ");
            }
            else if (msg.Body == "!mediainfo" && msg.HasMedia)
            {
                var attachmentData = await msg.DownloadMedia(_client);
                await msg.Reply(_client, $@"
                        *Media info*
                        MimeType: {attachmentData.Mimetype}
                        Filename: {attachmentData.Filename}
                        Data (length): {attachmentData.Data.Length}
                    ");
            }
            else if (msg.Body == "!quoteinfo" && msg.HasQuotedMsg)
            {
                var quotedMsg = await msg.GetQuotedMessage(_client);
                await quotedMsg.Reply(_client, $@"
                        ID: {quotedMsg.Id.Id}
                        Type: {quotedMsg.Type}
                        Author: {quotedMsg.Author ?? quotedMsg.From}
                        Timestamp: {quotedMsg.Timestamp}
                        Has Media? {quotedMsg.HasMedia}
                    ");
            }
            else if (msg.Body == "!resendmedia" && msg.HasQuotedMsg)
            {
                var quotedMsg = await msg.GetQuotedMessage(_client);
                if (quotedMsg.HasMedia)
                {
                    var attachmentData = await quotedMsg.DownloadMedia(_client);
                    await _client.Message.Send(msg.From, attachmentData,
                        new MessageOptions {Caption = "Here's your requested media."});
                }

                if (quotedMsg.HasMedia && quotedMsg.Type == "audio")
                {
                    var audio = await quotedMsg.DownloadMedia(_client);
                    await _client.Message.Send(msg.From, audio, new MessageOptions {SendAudioAsVoice = true});
                }
            }
            else if (msg.Body == "!isviewonce" && msg.HasQuotedMsg)
            {
                var quotedMsg = await msg.GetQuotedMessage(_client);
                if (quotedMsg.HasMedia)
                {
                    var media = await quotedMsg.DownloadMedia(_client);
                    await _client.Message.Send(msg.From, media, new MessageOptions {IsViewOnce = true});
                }
            }
            else if (msg.Body == "!location")
            {
                await msg.Reply(_client, new Location(37.422, -122.084));
                await msg.Reply(_client, new Location(37.422, -122.084, new LocationOptions {Name = "Googleplex"}));
                await msg.Reply(_client, new Location(37.422, -122.084, new LocationOptions
                {
                    Address = "1600 Amphitheatre Pkwy, Mountain View, CA 94043, USA"
                }));
                await msg.Reply(_client, new Location(37.422, -122.084, new LocationOptions
                {
                    Name = "Googleplex",
                    Address = "1600 Amphitheatre Pkwy, Mountain View, CA 94043, USA",

                }));
            }
            else if (msg.Location != null)
            {
            }
        };
    }



    private static EventHandler<AuthenticationFailureEventArgs>? OnAuthenticationFailureEvent()
    {
        return (sender, args) => { Console.Error.WriteLine("AUTHENTICATION FAILURE " + args.Payload); };
    }

    private static EventHandler<AuthenticatedEventArg>? OnAuthenticatedEvent()
    {
        return (sender, args) =>
        {
            Console.WriteLine("AUTHENTICATED");
            Console.WriteLine("USER:");
            Console.WriteLine(args.Info);
        };
    }

    private static EventHandler<QRReceivedEventArgs>? OnQRReceivedEvent()
    {
        return (sender, args) =>
        {

            Console.WriteLine("QR RECEIVED " + args.Qr);

            try
            {
                var qr = new GenerateQR();
                var ms = qr.Generate(args.Qr.ToString());
                var sw = Image.FromStream(ms.Result);
                sw.Save("testQR.png");
            }
            catch (Exception e)
            {

            }
        };

    }

    
   
}