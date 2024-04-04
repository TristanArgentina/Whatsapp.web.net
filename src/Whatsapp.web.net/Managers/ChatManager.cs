using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using Whatsapp.web.net.Domains;
using Whatsapp.web.net.scripts;

namespace Whatsapp.web.net.Managers;

public class ChatManager : IChatManager
{
    private readonly IJavaScriptParser _parserFunctions;
    private readonly IPage _pupPage;

    public ChatManager(IJavaScriptParser parserFunctions, IPage pupPage)
    {
        _parserFunctions = parserFunctions;
        _pupPage = pupPage;
    }

    public async Task<bool> SendSeen(string chatId)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("sendSeen"), chatId);
    }

    public async Task Archive(string chatId)
    {
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("archiveChat"), chatId);
    }

    public async Task UnArchive(string chatId)
    {
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("unarchiveChat"), chatId);
    }

    public async Task<bool> Pin(string chatId)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("pinChat"), chatId);
    }

    public async Task<bool> Unpin(string chatId)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("unpinChat"), chatId);
    }

    public async Task Mute(string chatId, DateTime? unMuteDate)
    {
        var timestamp = unMuteDate.HasValue
            ? (long)unMuteDate.Value.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
            : -1;
        await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("muteChat"), chatId, timestamp);
    }

    public async Task UnMute(string chatId)
    {
        await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("unmuteChat"), chatId);
    }

    public async Task Unread(string chatId)
    {
        await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("markChatUnread"), chatId);
    }

    public async Task<List<Label>> GetLabels(string chatId)
    {
        var labels = await _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("getChatLabels"), chatId);
        var resultLabels = new List<Label>();
        foreach (var data in labels)
        {
            resultLabels.Add(new Label(data));
        }

        return resultLabels;
    }

    public async Task AddOrRemoveLabels(List<object> labelIds, List<string> chatIds)
    {
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("manageLabelsInChats"), labelIds, chatIds);
    }

    public async Task<List<Message>> FetchMessages(string chatId, SearchOptions searchOptions)
    {
        var messages = await _pupPage.EvaluateFunctionAsync<List<dynamic>>(_parserFunctions.GetMethod("getMessagesFromChat"), chatId, searchOptions);

        return messages.ConvertAll(m => new Message(m));
    }

    public async Task<bool> ClearMessages(string chatId)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("clearMessagesById"), chatId);
    }

    public async Task<bool> Delete(string chatId)
    {
        return await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("sendDeleteChatById"), chatId);
    }

    public async Task SendStateTyping(string chatId)
    {
        await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("sendChatStateTypingById"), chatId);
    }

    public async Task SendStateRecording(string chatId)
    {
        await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("sendChatStateRecordingById"), chatId);
    }

    public async Task ClearState(string chatId)
    {
        await _pupPage.EvaluateFunctionAsync<bool>(_parserFunctions.GetMethod("sendChatStateStopById"), chatId);
    }

    public async Task<Chat> Get(string chatId)
    {
        var method = _parserFunctions.GetMethod("getChatById");
        var dataChat = await _pupPage.EvaluateFunctionAsync<dynamic>(method, chatId);
        return Chat.Create(dataChat);
    }

    public async Task<Chat[]> Get()
    {
        var data = _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("getChats")).Result;

        var dataList = new List<dynamic>((JArray)data);

        return dataList.Select(d => Chat.Create(d)).OfType<Chat>().ToArray();
    }

    public async Task ApproveMembership(string chatId, string requesterId)
    {
        var options = new MembershipRequestActionOptions
        {
            RequesterIds = [requesterId]
        };
        ApproveMembership(chatId, options);
    }

    public async Task ApproveMembership(string chatId, MembershipRequestActionOptions options)
    {
        _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("approveMembershipRequestAction"), chatId, options);
    }

    public async Task RejectMembership(string chatId, string requesterId)
    {
        var options = new MembershipRequestActionOptions
        {
            RequesterIds = [requesterId]
        };
        RejectMembership(chatId, options);
    }
    public async Task RejectMembership(string chatId, MembershipRequestActionOptions options)
    {
        _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("rejectMembershipRequestAction"), chatId, options);
    }
}