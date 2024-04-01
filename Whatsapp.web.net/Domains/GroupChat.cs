using Newtonsoft.Json.Linq;

namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a Group Chat on WhatsApp
/// </summary>
public class GroupChat : Chat
{
    /// <summary>
    /// Gets the group owner
    /// </summary>
    public UserId Owner { get; set; }

    /// <summary>
    /// Gets the date at which the group was created
    /// </summary>
    public DateTime Creation { get; set; }

    /// <summary>
    /// Gets the group description
    /// </summary>
    public string Description { get; set; }

    public bool Announce { get; set; }

    public bool Restrict { get; set; }

    /// <summary>
    /// Gets the group participants
    /// </summary>
    public List<GroupParticipant> Participants { get; set; }

    public GroupChat(dynamic? data)
    {
        Patch(data);
        PatchGroup(data);
    }

    private void PatchGroup(dynamic data)
    {
        Owner = UserId.Create(data.groupMetadata.owner);

        var creation = (long)data.groupMetadata.creation;
        if (creation < -62135596800)
        {
            Creation = DateTime.MinValue;
        }
        else if (creation > 253402300799)
        {
            Creation = DateTime.MaxValue;
        }
        else
        {
            Creation = DateTimeOffset.FromUnixTimeSeconds(creation).UtcDateTime;
        }
        Description = data.desc;
        Participants = ((JArray)data.groupMetadata.participants).Select(p => new GroupParticipant(p)).ToList();
    }

}