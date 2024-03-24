using Newtonsoft.Json.Linq;

namespace Whatsapp.web.net.Domains;

public class GroupChat : Chat
{
    public UserId Owner { get; set; }

    public DateTime Creation { get; set; }

    public string Description { get; set; }

    public bool Announce { get; set; }

    public bool Restrict { get; set; }

    public List<GroupParticipant> Participants { get; set; }

    public GroupChat(dynamic? data)
    {
        Patch(data);
        PatchGroup(data);
    }

    private void PatchGroup(dynamic data)
    {
        Owner = UserId.Create(data.groupMetadata.owner);
        Creation = DateTimeOffset.FromUnixTimeSeconds((long)data.groupMetadata.creation).UtcDateTime;
        Description = data.desc;
        Participants = ((JArray)data.groupMetadata.participants).Select(p => new GroupParticipant(p)).ToList();
    }

}