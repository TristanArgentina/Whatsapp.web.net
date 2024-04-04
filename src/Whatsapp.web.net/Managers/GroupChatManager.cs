using PuppeteerSharp;
using Whatsapp.web.net.Domains;
using Whatsapp.web.net.scripts;

namespace Whatsapp.web.net.Managers;

public class GroupChatManager : IGroupChatManager
{
    private readonly IJavaScriptParser _parserFunctions;
    private readonly IPage _pupPage;

    public GroupChatManager(IJavaScriptParser parserFunctions, IPage pupPage)
    {
        _parserFunctions = parserFunctions;
        _pupPage = pupPage;
    }

    public async Task<InviteV4> AcceptInvite(InviteV4 inviteInfo)
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


    public async Task<List<string>> GetCommonGroups(string contactId)
    {
        dynamic commonGroups = _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getCommonGroups"), contactId);
        var groups = new List<string>();
        foreach (var group in commonGroups)
        {
            groups.Add(group.Id);
        }

        return groups;
    }



    public async Task<dynamic> AddParticipants(UserId groupChatId, dynamic participantIds, GroupChatOptions chatOptions = null)
    {
        return await _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("addParticipantsToGroup"), groupChatId.Id, participantIds, chatOptions);
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

    public async Task<GroupChat> CreateGroup(string title, object? participants = null, GroupChatOptions options = null)
    {
        if (!(participants is IEnumerable<object>))
        {
            participants = new List<object> { participants };
        }

        dynamic data = _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("createGroup"), title, participants, options).Result;
        return new GroupChat(data);
    }
}