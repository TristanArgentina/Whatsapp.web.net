using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using Whatsapp.web.net.AuthenticationStrategies;
using Whatsapp.web.net.Domains;
using Whatsapp.web.net.Elements;

namespace Whatsapp.web.net;

public class Client : IDisposable, IAsyncDisposable
{

    const string PROGRESS = "//*[@id='app']/div/div/div[2]/progress";
    const string PROGRESS_MESSAGE = "//*[@id='app']/div/div/div[3]";

    // Define selectors
    const string INTRO_IMG_SELECTOR = "[data-icon='search']";
    const string INTRO_QRCODE_SELECTOR = "div[data-ref] canvas";

    const string QR_CONTAINER = "div[data-ref]";
    const string QR_RETRY_BUTTON = "div[data-ref] > span > button";


    private readonly IJavaScriptParser _parserInjected;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IRegisterEventService _registerEventService;
    private readonly IJavaScriptParser _parserFunctions;
    private readonly WhatsappOptions _options;
    private readonly BaseAuthStrategy _authStrategy;
    private IBrowser _pupBrowser;
    private IPage _pupPage;
    public ClientInfo ClientInfo { get; private set; }

    private readonly StreamWriter _streamWriter;

    public Client(IJavaScriptParser parserFunctions, IJavaScriptParser parserInjected, IEventDispatcher eventDispatcher, IRegisterEventService registerEventService, WhatsappOptions options)
    {
        _streamWriter = new StreamWriter("log.txt", true);
        _parserFunctions = parserFunctions;
        _parserInjected = parserInjected;
        _eventDispatcher = eventDispatcher;
        _registerEventService = registerEventService;
        _options = options;
        _authStrategy = _options.AuthStrategy;

        TaskUtils.KillProcessesByName("chrome", options.Puppeteer.ExecutablePath);
        _authStrategy.Setup(this, options);

        Util.SetFfmpegPath(options.FfmpegPath);
    }

    public async Task<Task> Initialize()
    {
        await InitializePage();

        await _authStrategy.AfterBrowserInitialized();
        await InitWebVersionCacheAsync();
        //TODO: missing
        //await PupPage.EvaluateExpressionOnNewDocumentAsync(_parserFunctions.GetMethod("modificarErrorStack"));

        await _pupPage.GoToAsync(Constants.WhatsWebURL, new NavigationOptions
        {
            WaitUntil = [WaitUntilNavigation.Load],
            Timeout = 0,
            Referer = "https://whatsapp.com/"
        });

        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getElementByXpath"));

        var lastPercent = default(int?);
        var lastPercentMessage = default(string);

        await _pupPage.ExposeFunctionAsync<int, string, bool>("onLoadingScreen", (percent, message) =>
        {
            if (lastPercent == percent && lastPercentMessage == message) return true;

            _eventDispatcher.EmitLoadingScreen(percent, message);
            lastPercent = percent;
            lastPercentMessage = message;

            return true;
        });

        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("observeProgress"), new { PROGRESS, PROGRESS_MESSAGE });

        var continueTask = await AuthenticationIfNeed();
        if (continueTask != Task.CompletedTask)
        {
            return continueTask;
        }



        // TODO: missing implementation
        // this.interface = new InterfaceController(this);

        _registerEventService.Register(_pupPage);

        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("registerEventListeners"));

        _eventDispatcher.EmitReady();
        return Task.CompletedTask;
    }



    /// <summary>
    /// Asynchronously handles authentication if needed and retry.
    /// </summary>
    /// <returns></returns>
    private async Task<Task> AuthenticationIfNeed()
    {
        // Wait for either selector to appear first
        var imgSelectorTask = _pupPage.WaitForSelectorAsync(INTRO_IMG_SELECTOR, new WaitForSelectorOptions { Timeout = _options.AuthTimeoutMs });
        var qrSelectorTask = _pupPage.WaitForSelectorAsync(INTRO_QRCODE_SELECTOR, new WaitForSelectorOptions { Timeout = _options.AuthTimeoutMs });
        var needAuthentication = await Task.WhenAny(imgSelectorTask, qrSelectorTask);

        needAuthentication.Wait();

        // Check if an error occurred on the first found selector
        if (needAuthentication.IsFaulted)
        {
            // Scan-qrcode selector was found. Needs authentication
            var result = await _authStrategy.OnAuthenticationNeeded();
            if (result.Failed)
            {
                // Handle authentication failure
                // Emits authentication failure event
                _eventDispatcher.EmitAuthenticationFailure(result.FailureEventPayload);

                await Destroy();
                if (!result.Restart) return needAuthentication;

                // Session restore failed so try again without session to force new authentication
                return Initialize();
            }
        }

        // if (needAuthentication.Result.RemoteObject.ClassName == "HTMLCanvasElement") return Task.CompletedTask;
        if (needAuthentication == qrSelectorTask)
        {

            var qrRetries = 0;
            var continueObserving = true;

            // Expose qrChanged function to the page
            await _pupPage.ExposeFunctionAsync("qrChanged", (string qr) =>
            {
                // Emits QR received event
                _eventDispatcher.EmitQRReceived(qr);
                if (_options.QrMaxRetries > 0)
                {
                    qrRetries++;
                    if (qrRetries > _options.QrMaxRetries)
                    {
                        // Emits disconnected event
                        _eventDispatcher.EmitDisconnected("Max qrcode retries reached");
                        continueObserving = false;
                    }
                }

                return continueObserving;
            });


            // Observe changes in QR container
            await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("startQRCodeObserver"),
                JsonConvert.SerializeObject(new { QR_CONTAINER, QR_RETRY_BUTTON }));

            try
            {
                await _pupPage.WaitForSelectorAsync(INTRO_IMG_SELECTOR, new WaitForSelectorOptions { Timeout = 0 });
            }
            catch (Exception error)
            {
                if (!error.Message.Contains("Target closed")) throw;
                // Something has called .Destroy() while waiting
                return Task.CompletedTask;

            }
        }

        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("compareWwebVersions"));

        await _pupPage.EvaluateFunctionAsync(_parserInjected.GetMethod("registerModuleRaid"));

        // Evaluate ExposeStore 
        await _pupPage.EvaluateFunctionAsync(_parserInjected.GetMethod("exposeStore"));


        // Wait for window.Store to be defined
        await _pupPage.WaitForFunctionAsync("() => window.Store != undefined");

        // Unregister service workers
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("unregisterServiceWorkers"));

        // Load utility functions
        await _pupPage.EvaluateFunctionAsync(_parserInjected.GetMethod("loadUtils"));

        // Expose client info

        var clientInfo = await _pupPage.EvaluateFunctionAsync<object>(_parserFunctions.GetMethod("serializeConnectionAndUser"));

        ClientInfo = new ClientInfo(clientInfo);

        // Get authentication event payload
        var authEventPayload = await _authStrategy.GetAuthEventPayload();

        // Emit authenticated event
        _eventDispatcher.EmitAuthenticated(ClientInfo, authEventPayload);

        return Task.CompletedTask;
    }

    private async Task InitializePage()
    {
        IBrowser browser;
        IPage page;

        await _authStrategy.BeforeBrowserInitialized();

        if (_options.Puppeteer is { BrowserWSEndpoint: not null })
        {
            browser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                DefaultViewport = _options.Puppeteer.DefaultViewport
            });
            page = await browser.NewPageAsync();
        }
        else
        {
            var browserArgs = new List<string>(_options.Puppeteer.Args ?? Array.Empty<string>());

            // navigator.webdriver fix
            browserArgs.Add($"--disable-blink-features=AutomationControlled");


            var launchOptions = new LaunchOptions
            {
                Args = browserArgs.ToArray(),
                Headless = _options.Puppeteer.Headless,
                UserDataDir = _options.Puppeteer.UserDataDir,
                DefaultViewport = _options.Puppeteer.DefaultViewport,
                ExecutablePath = _options.Puppeteer.ExecutablePath
            };

            browser = await Puppeteer.LaunchAsync(launchOptions);
            page = (await browser.PagesAsync())[0];
        }

        if (_options.ProxyAuthentication is not null)
        {
            await page.AuthenticateAsync(_options.ProxyAuthentication);
        }

        await page.SetUserAgentAsync(_options.UserAgent);
        if (_options.BypassCSP)
        {
            await page.SetBypassCSPAsync(true);
        }

        page.Console += ConsoleWrite;

        _pupBrowser = browser;
        _pupPage = page;
    }

    private readonly List<string> mensajes = [];

    private void ConsoleWrite(object? sender, ConsoleEventArgs e)
    {
        var value = e.Message.Text;
        if (!mensajes.Contains(value))
        {
            //mensajes.Add(value);
            Console.WriteLine(value);
            _streamWriter.WriteLine(value);
        }

    }

    public async Task InitWebVersionCacheAsync()
    {
        var webCacheOptions = _options.WebVersionCache;
        var webCache = WebCacheFactory.CreateWebCache(webCacheOptions.Type, webCacheOptions);

        var requestedVersion = _options.WebVersion;
        var versionContent = await webCache.Resolve(requestedVersion);

        if (versionContent != null)
        {
            await _pupPage.SetRequestInterceptionAsync(true);
            _pupPage.Request += async (sender, e) =>
            {
                if (e.Request.Url == Constants.WhatsWebURL)
                {
                    await e.Request.RespondAsync(new ResponseData
                    {
                        Status = HttpStatusCode.OK,
                        ContentType = "text/html",
                        Body = versionContent
                    });
                }
                else
                {
                    await e.Request.ContinueAsync();
                }
            };
        }
        else
        {
            _pupPage.Response += async (sender, e) =>
            {
                if (e.Response.Ok && e.Response.Url == Constants.WhatsWebURL)
                {
                    await webCache.Persist(await e.Response.TextAsync());
                }
            };
        }
    }

    public async Task<Task<Message>> SendMessage(UserId from, string content)
    {
        return SendMessage(from.Id, content);
    }

    public async Task<Task<Message>> SendMessage(UserId from, MessageMedia attachmentData, ReplayOptions options)
    {
        return SendMessage(from.Id, attachmentData, options);
    }

    public async Task<Message> SendMessage(string fromId, object content, ReplayOptions? options = null)
    {
        var mentions = new List<UserId>();
        if (options != null && options.Mentions != null && options.Mentions.Any())
        {
            mentions = options.Mentions.OfType<Contact>().Select(c => c.Id).ToList();
        }

        var internalOptions = new Dictionary<string, object?>
        {
            { "linkPreview", options != null && options.LinkPreview && !options.LinkPreview ? null : true },
            { "sendAudioAsVoice", options != null && options.SendAudioAsVoice ? options.SendAudioAsVoice: null },
            { "sendVideoAsGif", options != null && options.SendVideoAsGif ? options.SendVideoAsGif : null },
            { "sendMediaAsSticker", options != null && options.SendMediaAsSticker ? options.SendMediaAsSticker : null },
            { "sendMediaAsDocument", options != null && options.SendMediaAsDocument ? options.SendMediaAsDocument : null },
            { "caption", options != null && !string.IsNullOrEmpty( options.Caption) ? options.Caption: null },
            { "quotedMessageId", options != null && options.QuotedMessageId is not null ? options.QuotedMessageId : null },
            { "parseVCards", options != null && options.ParseVCards && !options.ParseVCards? false : true },
            { "mentionedJidList", mentions },
            { "groupMentions", options != null && options.GroupMentions is not null && options.GroupMentions.Any() ? options.GroupMentions : null },
            { "extraOptions", options?.Extra }
        };

        var sendSeen = options != null && options.SendSeen ? options.SendSeen : true;

        if (content is MessageMedia)
        {
            internalOptions["attachment"] = content;
            content = "";
        }
        else if (options != null && options.Media is not null)
        {
            internalOptions["attachment"] = options.Media;
            internalOptions["caption"] = content;
            content = "";
        }
        else if (content is Location)
        {
            internalOptions["location"] = content;
            content = "";
        }
        else if (content is Poll)
        {
            internalOptions["poll"] = content;
            content = "";
        }
        else if (content is Contact contactCard)
        {
            internalOptions["contactCard"] = contactCard.Id;
            content = "";
        }
        else if (content is IList<Contact> contactList && contactList.Any())
        {
            internalOptions["contactCardList"] = contactList.Select(contact => ((Contact)contact).Id).ToList();
            content = "";
        }
        else if (content is Buttons buttons)
        {
            if (buttons.Type != "chat") internalOptions["attachment"] = buttons.Body;
            internalOptions["buttons"] = buttons;
            content = "";
        }
        else if (content is List)
        {
            internalOptions["list"] = content;
            content = "";
        }

        if (internalOptions.ContainsKey("sendMediaAsSticker") && internalOptions["sendMediaAsSticker"] != null && internalOptions.ContainsKey("attachment"))
        {
            internalOptions["attachment"] = await Util.FormatToWebpSticker(
                (MessageMedia)internalOptions["attachment"],
                new StickerMetadata
                {
                    Name = options != null && !string.IsNullOrEmpty(options.StickerName) ? options.StickerName : null,
                    Author = options != null && !string.IsNullOrEmpty(options.StickerAuthor) ? options.StickerAuthor : null,
                    Categories = options != null && options.StickerCategories is not null && options.StickerCategories.Any() ? options.StickerCategories.ToArray() : null
                },
                _pupPage
            );
        }

        var newMessage = await _pupPage.EvaluateFunctionAsync(
            _parserFunctions.GetMethod("sendMessageAsyncToChat"),
            fromId, content, internalOptions, sendSeen);

        return new Message(newMessage);
    }


    public async Task<InviteV4> AcceptGroupV4InviteAsync(InviteV4 inviteInfo)
    {

        if (string.IsNullOrEmpty(inviteInfo.InviteCode))
        {
            throw new ArgumentException("Invalid invite code, try passing the message.inviteV4 object");
        }

        if (inviteInfo.InviteCodeExp == 0)
        {
            throw new ArgumentException("Expired invite code");
        }

        return await _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("joinGroupViaInviteV4Async"), inviteInfo);
    }

    public async Task ReactAsync(MessageId? msgId, string reaction)
    {
        if (msgId is null) return;
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("reactToMessage"), msgId, reaction);
    }

    public async Task ForwardAsync(MessageId? msgId, string chatId)
    {
        if (msgId is null) return;
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("forwardMessages"), msgId, chatId);
    }

    public async Task<MessageMedia?> DownloadMediaAsync(MessageId? msgId, bool hasMedia)
    {
        if (msgId is null) return null;
        if (!hasMedia) return null;

        var result = await _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("retrieveAndConvertMedia"), msgId);
        return result == null ? null : new MessageMedia(result);
    }

    /// <summary>
    /// Deletes a message from the chat
    /// </summary>
    /// <param name="msgId"></param>
    /// <param name="everyone">If true and the message is sent by the current user or the user is an admin, will delete it for everyone in the chat.</param>
    /// <returns></returns>
    public async Task DeleteAsync(MessageId msgId, bool? everyone)
    {
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("deleteMessageAsyncWithPermissions"), msgId, everyone);
    }

    /// <summary>
    /// Stars this message
    /// </summary>
    /// <param name="msgId"></param>
    /// <returns></returns>
    public async Task StarAsync(MessageId msgId)
    {
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("starMessageIfAllowed"), msgId);
    }

    /// <summary>
    ///  Unstars this message
    /// </summary>
    /// <param name="msgId"></param>
    /// <returns></returns>
    public async Task UnstarAsync(MessageId msgId)
    {
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("unstarMessage"), msgId);
    }


    /// <summary>
    /// Pins the message (group admins can pin messages of all group members)
    /// </summary>
    /// <param name="duration"> The duration in seconds the message will be pinned in a chat</param>
    /// <returns>Returns true if the operation completed successfully, false otherwise</returns>
    public async Task<bool> PinAsync(MessageId msgId, int duration)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("pinMessage"), msgId, duration);
    }

    /// <summary>
    /// Unpins the message (group admins can unpin messages of all group members)
    /// </summary>
    /// <param name="msgId"></param>
    /// <returns>Returns true if the operation completed successfully, false otherwise</returns>
    public async Task<bool> UnpinAsync(MessageId msgId)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("unpinMessage"), msgId);
    }

    public async Task<MessageInfo?> GetInfoAsync(MessageId msgId)
    {
        var infoJson = await _pupPage.EvaluateFunctionAsync<string>(_parserFunctions.GetMethod("getMessageInfo"), msgId);

        return infoJson == null ? null : JsonConvert.DeserializeObject<MessageInfo>(infoJson);
    }

    public async Task<Order?> GetOrderAsync(string msgType, string orderId, string token, string chatId)
    {
        if (msgType != MessageTypes.ORDER) return null;
        var result = await _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("getOrderDetail"), orderId, token, chatId);
        return result == null ? null : new Order(result);
    }

    public async Task<Payment?> GetPayment(MessageId msgId, string msgType)
    {
        if (msgType != MessageTypes.PAYMENT) return null;
        var data = await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getMessageSerialized"), msgId);
        return new Payment(data);
    }

    public async Task<ReactionList?> GetReactionsSync(MessageId msgId, bool hasReaction)
    {
        if (!hasReaction) return null;
        var data = await _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("getReactions"), msgId);
        return new ReactionList(data);
    }

    public async Task<Message?> EditAsync(MessageId msgId, string content, ReplayOptions? options = null)
    {
        if (!msgId.FromMe) return null;

        if (options?.Mentions != null)
        {
            options.Mentions = options.Mentions.Select(m => m is Contact c ? c.Id : m).ToList();
        }

        var internalOptions = new
        {
            linkPreview = options?.LinkPreview == false ? (bool?)null : true,
            mentionedJidList = options?.Mentions ?? [],
            groupMentions = options?.GroupMentions,
            extraOptions = options?.Extra
        };


        var data = await _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("editMessage"), msgId, content, internalOptions);

        return data != null ? new Message(data) : null;
    }

    public async Task<ProductMetadata?> GetProductMetadataById(string productId)
    {
        var result = await _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("getProductMetadataById"), productId);
        return result is not null ? new ProductMetadata(result) : null;
    }

    /// <summary>
    /// Reloads this Message object's data in-place with the latest values from WhatsApp Web. 
    /// Note that the Message must still be in the web app cache for this to work, otherwise will return null.
    /// </summary>
    /// <returns></returns>
    public async Task<Message?> GetMessageById(MessageId msgId)
    {
        var newData = await _pupPage.EvaluateFunctionAsync<Message>(_parserFunctions.GetMethod("getMessageModelById"), msgId);
        return newData == null ? null : new Message(newData);
    }


    public async Task<Message?> GetQuotedMessage(MessageId msgId, bool hasQuotedMsg = true)
    {
        if (!hasQuotedMsg) return null;
        var quotedMsg = await _pupPage.EvaluateFunctionAsync<Message>(_parserFunctions.GetMethod("getQuotedMessageModel"), msgId);
        return quotedMsg == null ? null : new Message(quotedMsg);
    }

    public async Task<Contact> GetContactById(string contactId)
    {
        dynamic dataChat = _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getContactById"), contactId);

        return Contact.Create(dataChat);
    }

    public async Task<string?> GetProfilePicUrl(string contactId)
    {
        dynamic profilePic = _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getProfilePic"), contactId);

        return profilePic is not null ? profilePic.eurl : null;
    }

    public async Task<string> GetFormattedNumber(string number)
    {
        return await _pupPage.EvaluateFunctionAsync<string>(_parserFunctions.GetMethod("getFormattedNumber"), number);
    }

    public async Task<string> GetCountryCode(string number)
    {
        number = number.Replace(" ", "").Replace("+", "").Replace("@c.us", "");
        return await _pupPage.EvaluateFunctionAsync<string>(_parserFunctions.GetMethod("getCountryCode"), number);
    }

    public async Task<List<string>> GetCommonGroups(string contactId)
    {
        dynamic commonGroups = _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getCommonGroups"), contactId);
        var chats = new List<string>();
        foreach (var group in commonGroups)
        {
            chats.Add(group.Id);
        }

        return chats;
    }

    public async Task<bool> SendSeen(string chatId)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("sendSeen"), chatId);
    }

    public async Task ArchiveChat(string chatId)
    {
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("archiveChat"), chatId);
    }

    public async Task UnArchiveChat(string chatId)
    {
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("unarchiveChat"), chatId);
    }

    public async Task<bool> PinChat(string chatId)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("pinChat"), chatId);
    }

    public async Task<bool> UnpinChat(string chatId)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("unpinChat"), chatId);
    }

    public async Task MuteChat(string chatId, DateTime? unMuteDate)
    {
        var timestamp = unMuteDate.HasValue
            ? (long)unMuteDate.Value.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
            : -1;
        await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("muteChat"), chatId, timestamp);
    }

    public async Task UnmuteChat(string chatId)
    {
        await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("unmuteChat"), chatId);
    }

    public async Task MarkChatUnread(string chatId)
    {
        await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("markChatUnread"), chatId);
    }

    public async Task<List<Label>> GetChatLabels(string chatId)
    {
        var labels = await _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("getChatLabels"), chatId);
        var resultLabels = new List<Label>();
        foreach (var data in labels)
        {
            resultLabels.Add(new Label(data));
        }

        return resultLabels;
    }

    public async Task<dynamic> AddParticipants(string groupChatId, dynamic participantIds, GroupChatOptions chatOptions = null)
    {
        return await _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("addParticipantsToGroup"), groupChatId, participantIds, chatOptions);
    }

    public async Task<dynamic> RemoveParticipants(string groupChatId, List<string> participantIds)
    {
        return await _pupPage.EvaluateFunctionAsync<object>(_parserFunctions.GetMethod("removeParticipants"), groupChatId, participantIds);
    }

    public async Task<object> PromoteParticipants(string groupChatId, List<string> participantIds)
    {
        return await _pupPage.EvaluateFunctionAsync<object>(_parserFunctions.GetMethod("promoteParticipants"), groupChatId, participantIds);
    }

    public async Task<object> DemoteParticipants(string groupChatId, List<string> participantIds)
    {
        return await _pupPage.EvaluateFunctionAsync<object>(_parserFunctions.GetMethod("demoteParticipants"), groupChatId, participantIds);
    }

    public async Task<bool> SetSubject(string groupChatId, string subject)
    {
        var success = await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("setGroupSubject"), groupChatId, subject);
        return success;
    }

    public async Task<bool> SetDescription(string groupChatId, string description)
    {
        var success = await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("setGroupDescription"), groupChatId, description);
        return success;
    }

    public async Task<bool> SetMessagesAdminsOnly(string groupChatId, bool adminsOnly = true)
    {
        var success = await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("setGroupAnnouncement"), groupChatId, adminsOnly);
        return success;
    }

    public async Task<bool> SetInfoAdminsOnly(string groupChatId, bool adminsOnly = true)
    {
        var success = await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("setGroupRestrict"), groupChatId, adminsOnly);
        return success;
    }

    public async Task<bool> DeletePicture(string groupChatId)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("deletePicture"), groupChatId);
    }

    public async Task<bool> SetPicture(string groupChatId, MessageMedia media)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("setPicture"), groupChatId, media);
    }

    public async Task<string> GetInviteCode(string groupChatId)
    {
        return await _pupPage.EvaluateFunctionAsync<string>(_parserFunctions.GetMethod("getInviteCode"), groupChatId);
    }

    public async Task<string> RevokeInvite(string groupChatId)
    {
        return await _pupPage.EvaluateFunctionAsync<string>(_parserFunctions.GetMethod("revokeInvite"), groupChatId);
    }

    public async Task Leave(string groupChatId)
    {
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("sendExitGroup"), groupChatId);
    }


    public async Task AddOrRemoveLabels(List<object> labelIds, List<string> chatIds)
    {
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("manageLabelsInChats"), labelIds, chatIds);
    }

    public async Task<List<Message>> FetchMessagesChatByIdSync(string chatId, SearchOptions searchOptions)
    {
        var messages = await _pupPage.EvaluateFunctionAsync<List<dynamic>>(_parserFunctions.GetMethod("getMessagesFromChat"), chatId, searchOptions);

        return messages.ConvertAll(m => new Message(m));
    }

    public async Task<object> GetBatteryStatus()
    {
        return await _pupPage.EvaluateExpressionAsync(_parserFunctions.GetMethod("getBatteryStatus"));
    }

    private async Task Destroy()
    {
        await _pupBrowser.CloseAsync();
        await _authStrategy.Destroy();
    }

    public async Task<bool> ClearMessagesAsync(string chatId)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("clearMessagesById"), chatId);
    }

    public async Task<bool> DeleteChatByIdAsync(string chatId)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("sendDeleteChatById"), chatId);
    }

    public async Task SendStateTypingChatByIdAsync(string chatId)
    {
        await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("sendChatStateTypingById"), chatId);
    }

    public async Task SendStateRecordingChatByIdAsync(string chatId)
    {
        await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("sendChatStateRecordingById"), chatId);
    }

    public async Task ClearStateChatByIdAsync(string chatId)
    {
        await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("sendChatStateStopById"), chatId);
    }

    public async Task<bool> Block(Contact contact)
    {
        if (contact.IsGroup) return false;

        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("blockContactById"), contact.Id.Serialized);

        contact.IsBlocked = true;
        return true;
    }

    public async Task<bool> Unblock(Contact contact)
    {
        if (contact.IsGroup) return false;

        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("unblockContactById"), contact.Id.Serialized);

        contact.IsBlocked = false;
        return true;
    }

    public async Task<string?> GetAbout(Contact contact)
    {
        var about = await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getStatusContactById"), contact.Id.Serialized);

        if (about?["status"] is null || about["status"]!.Type != JTokenType.String)
        {
            return null;
        }

        return about["status"]!.Value<string>();
    }

    public void Dispose()
    {
        _pupBrowser.Dispose();
        _streamWriter.Dispose();
        _pupPage.Dispose();
    }


    public async ValueTask DisposeAsync()
    {
        await _pupBrowser.DisposeAsync();
        await _streamWriter.DisposeAsync();
        await _pupPage.DisposeAsync();
    }

    // TODO: Method duplicated
    public Chat GetChatById(string chatId)
    {
        dynamic dataChat = _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getChatById"), chatId).Result;
        return Chat.Create(dataChat);
    }


    public async Task Reject(string peerJid, string callId)
    {
         await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("rejectCall"), peerJid, callId);
    }

    public async Task<GroupChat> CreateGroup(string title, object? participants = null, GroupChatOptions options = null)
    {
        // Convierte participants a una lista si no lo es
        if (!(participants is IEnumerable<object>))
        {
            participants = new List<object> {participants};
        }

        dynamic data = _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("createGroup"), title, participants, options).Result;
        return new GroupChat(data);
    }

    public async Task<Chat[]> GetChats()
    {
        dynamic data = _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getChats")).Result;

        var dataList = new List<dynamic>(data);

        return (Chat[]) dataList.Select(d=> Chat.Create(d));
    }

    public async Task ApproveGroupMembershipRequests(string chatId, string requesterId)
    {
        var options = new MembershipRequestActionOptions
        {
            RequesterIds = [requesterId]
        };
        ApproveGroupMembershipRequests(chatId, options);
    }

    public async Task ApproveGroupMembershipRequests(string chatId, MembershipRequestActionOptions options)
    {
        _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("approveMembershipRequestAction"), chatId, options);
    }

    public async Task RejectGroupMembershipRequests(string chatId, string requesterId)
    {
        var options = new MembershipRequestActionOptions
        {
            RequesterIds = [requesterId]
        };
        RejectGroupMembershipRequests(chatId, options);
    }
    public async Task RejectGroupMembershipRequests(string chatId, MembershipRequestActionOptions options)
    {
        _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("rejectMembershipRequestAction"), chatId, options);
    }
}