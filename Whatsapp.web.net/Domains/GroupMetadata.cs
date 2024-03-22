namespace Whatsapp.web.net.Domains;

public class GroupMetadata
{
    public GroupMetadata(dynamic data)
    {
        Patch(data);
    }

    protected internal void Patch(dynamic data)
    {
        Owner = UserId.Create(data.owner);
        Creation = data.creation;
        Desc = data.desc;
        Participants = ((List<dynamic>)data.participants).Select((p) => new GroupParticipant(p)).ToList();
    }

    public UserId Owner { get; set; }

    public double Creation { get; set; }

    public string Desc { get; set; }

    public bool Announce { get; set; }

    public bool Restrict { get; set; }

    public List<GroupParticipant> Participants { get; set; }

}