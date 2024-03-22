namespace Whatsapp.web.net.Domains;

public class GroupParticipant
{
    public GroupParticipant(dynamic data)
    {
        Patch(data);
    }

    private void Patch(dynamic data)
    {
        Id = data.id;
        IsAdmin = data.isAdmin;
        IsSuperAdmin = data.isSuperAdmin;
    }

    public string Id { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsSuperAdmin { get; set; }
}