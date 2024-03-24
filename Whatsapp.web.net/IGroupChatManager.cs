using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net;

public interface IGroupChatManager : IManager
{
    Task<InviteV4> AcceptInvite(InviteV4 inviteInfo);
    Task<List<string>> GetCommonGroups(string contactId);

    Task<dynamic> AddParticipants(UserId groupChatId, dynamic participantIds, GroupChatOptions chatOptions = null);

    Task<dynamic> RemoveParticipants(string groupChatId, List<string> participantIds);
    Task<object> PromoteParticipants(string groupChatId, List<string> participantIds);
    Task<object> DemoteParticipants(string groupChatId, List<string> participantIds);
    Task<bool> SetSubject(string groupChatId, string subject);
    Task<bool> SetDescription(string groupChatId, string description);
    Task<bool> SetMessagesAdminsOnly(string groupChatId, bool adminsOnly = true);
    Task<bool> SetInfoAdminsOnly(string groupChatId, bool adminsOnly = true);
    Task<bool> DeletePicture(string groupChatId);
    Task<bool> SetPicture(string groupChatId, MessageMedia media);
    Task<string> GetInviteCode(string groupChatId);
    Task<string> RevokeInvite(string groupChatId);
    Task Leave(string groupChatId);
    Task<GroupChat> CreateGroup(string title, object? participants = null, GroupChatOptions options = null);
}