using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.Extensions;

public static class GroupChatExtensions
{

    public static async Task<object> AddParticipants(this GroupChat groupChat, Client client, dynamic participantIds, dynamic options = null)
    {
        return await client.AddParticipants(groupChat.Id, participantIds, options);
    }

    public static async Task<object> RemoveParticipants(this GroupChat groupChat, Client client, List<string> participantIds)
    {
        return await client.RemoveParticipants(groupChat.Id, participantIds);
    }

    public static async Task<object> PromoteParticipants(this GroupChat groupChat, Client client, List<string> participantIds)
    {
        return await client.PromoteParticipants(groupChat.Id, participantIds);
    }

    public static async Task<object> DemoteParticipants(this GroupChat groupChat, Client client, List<string> participantIds)
    {
        return await client.DemoteParticipants(groupChat.Id, participantIds);
    }

    public static async Task<bool> SetSubject(this GroupChat groupChat, Client client, string subject)
    {
        var success = await client.SetSubject(groupChat.Id, subject);
        if (!success) return false;
        groupChat.Name = subject;
        return true;
    }

    public static async Task<bool> SetDescription(this GroupChat groupChat, Client client, string description)
    {
        var success = await client.SetDescription(groupChat.Id, description);
        if (!success) return false;
        groupChat.GroupMetadata.Desc = description;
        return true;
    }

    public static async Task<bool> SetMessagesAdminsOnly(this GroupChat groupChat, Client client, bool adminsOnly = true)
    {
        var success = await client.SetMessagesAdminsOnly(groupChat.Id, adminsOnly);
        if (!success) return false;

        groupChat.GroupMetadata.Announce = adminsOnly;

        return true;
    }

    public static async Task<bool> SetInfoAdminsOnly(this GroupChat groupChat, Client client, bool adminsOnly = true)
    {
        var success = await client.SetInfoAdminsOnly(groupChat.Id, adminsOnly);
        if (!success) return false;

        groupChat.GroupMetadata.Restrict = adminsOnly;

        return true;
    }

    public static async Task<bool> DeletePicture(this GroupChat groupChat, Client client)
    {
        return await client.DeletePicture(groupChat.Id);
    }

    public static async Task<bool> SetPicture(this GroupChat groupChat, Client client, MessageMedia media)
    {
        return await client.SetPicture(groupChat.Id, media);
    }

    public static async Task<string> GetInviteCode(this GroupChat groupChat, Client client)
    {
        return await client.GetInviteCode(groupChat.Id);
    }

    public static async Task<string> RevokeInvite(this GroupChat groupChat, Client client)
    {
        return await client.RevokeInvite(groupChat.Id);
    }

    public static async Task Leave(this GroupChat groupChat, Client client)
    {
        await client.Leave(groupChat.Id);
    }
}