using Whatsapp.web.net.Domains;
using Whatsapp.web.net.Elements;

namespace Whatsapp.web.net;

public interface IChatManager : IManager
{
    Task Archive(string chatId);

    Task UnArchive(string chatId);

    Task<bool> Pin(string chatId);

    Task<bool> Unpin(string chatId);

    Task Mute(string chatId, DateTime? unMuteDate);

    Task UnMute(string chatId);

    Task Unread(string chatId);

    Task<bool> ClearMessages(string chatId);

    Task<bool> Delete(string chatId);

    Task SendStateTyping(string chatId);

    Task SendStateRecording(string chatId);

    Task ClearState(string chatId);

    Task<Chat> Get(string chatId);

    Task<Chat[]> Get();

    Task<List<Label>> GetLabels(string chatId);

    Task AddOrRemoveLabels(List<object> labelIds, List<string> chatIds);

    Task<List<Message>> FetchMessages(string chatId, SearchOptions searchOptions);

    Task<bool> SendSeen(string chatId);

    Task ApproveMembership(string chatId, string requesterId);

    Task ApproveMembership(string chatId, MembershipRequestActionOptions options);

    Task RejectMembership(string chatId, string requesterId);

    Task RejectMembership(string chatId, MembershipRequestActionOptions options);
}