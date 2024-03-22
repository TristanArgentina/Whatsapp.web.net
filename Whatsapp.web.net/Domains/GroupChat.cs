namespace Whatsapp.web.net.Domains;

public class GroupChat : Chat
{

    public UserId Owner => GroupMetadata.Owner;

    public GroupMetadata GroupMetadata { get; private set; }

    public DateTime CreatedAt => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        .AddSeconds((long)GroupMetadata.Creation);

    public string Description => GroupMetadata.Desc;

    public List<GroupParticipant> Participants => GroupMetadata.Participants;


    private void PatchGroupMetadata(dynamic? data)
    {
        if (data == null) return;
        GroupMetadata = new GroupMetadata(data.groupMetadata);
        GroupMetadata.Patch(data);
    }

    public GroupChat(dynamic? data)
    {
        Patch(data);
        PatchGroupMetadata(data);
    }
}