using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.Extensions;

public static class GroupChatExtensions
{

    public static async Task<object> AddParticipants(this GroupChat groupChat, Client client, dynamic participantIds, dynamic options = null)
    {
        return await client.Group.AddParticipants(groupChat.Id, participantIds, options);
    }

    public static async Task<object> RemoveParticipants(this GroupChat groupChat, Client client, List<string> participantIds)
    {
        return await client.Group.RemoveParticipants(groupChat.Id.Id, participantIds);
    }

    public static async Task<object> PromoteParticipants(this GroupChat groupChat, Client client, List<string> participantIds)
    {
        return await client.Group.PromoteParticipants(groupChat.Id.Id, participantIds);
    }

    public static async Task<object> DemoteParticipants(this GroupChat groupChat, Client client, List<string> participantIds)
    {
        return await client.Group.DemoteParticipants(groupChat.Id.Id, participantIds);
    }

    public static async Task<bool> SetSubject(this GroupChat groupChat, Client client, string subject)
    {
        var success = await client.Group.SetSubject(groupChat.Id.Id, subject);
        if (!success) return false;
        groupChat.Name = subject;
        return true;
    }

    public static async Task<bool> SetDescription(this GroupChat groupChat, Client client, string description)
    {
        var success = await client.Group.SetDescription(groupChat.Id.Id, description);
        if (!success) return false;
        groupChat.Description = description;
        return true;
    }

    public static async Task<bool> SetMessagesAdminsOnly(this GroupChat groupChat, Client client, bool adminsOnly = true)
    {
        var success = await client.Group.SetMessagesAdminsOnly(groupChat.Id.Id, adminsOnly);
        if (!success) return false;

        groupChat.Announce = adminsOnly;

        return true;
    }

    public static async Task<bool> SetInfoAdminsOnly(this GroupChat groupChat, Client client, bool adminsOnly = true)
    {
        var success = await client.Group.SetInfoAdminsOnly(groupChat.Id.Id, adminsOnly);
        if (!success) return false;

        groupChat.Restrict = adminsOnly;

        return true;
    }

    public static async Task<bool> DeletePicture(this GroupChat groupChat, Client client)
    {
        return await client.Group.DeletePicture(groupChat.Id._serialized);
    }

    public static async Task<bool> SetPicture(this GroupChat groupChat, Client client, MessageMedia media)
    {
        return await client.Group.SetPicture(groupChat.Id._serialized, media);
    }

    public static async Task<string> GetInviteCode(this GroupChat groupChat, Client client)
    {
        return await client.Group.GetInviteCode(groupChat.Id.Id);
    }

    public static async Task<string> RevokeInvite(this GroupChat groupChat, Client client)
    {
        return await client.Group.RevokeInvite(groupChat.Id.Id);
    }

    public static async Task Leave(this GroupChat groupChat, Client client)
    {
        await client.Group.Leave(groupChat.Id.Id);
    }
}