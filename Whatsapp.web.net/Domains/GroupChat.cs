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
        Owner = UserId.Create(data.owner);
        Creation = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((long)data.Creation);
        Description = data.desc;
        Participants = ((List<dynamic>)data.participants).Select(p => new GroupParticipant(p)).ToList();
    }

}