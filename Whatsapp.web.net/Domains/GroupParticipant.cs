namespace Whatsapp.web.net.Domains;

public class GroupParticipant
{
    public GroupParticipant(dynamic data)
    {
        Patch(data);
    }

    private void Patch(dynamic data)
    {
        Id = UserId.Create(data.id);
        IsAdmin = data.isAdmin;
        IsSuperAdmin = data.isSuperAdmin;
    }

    public UserId Id { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsSuperAdmin { get; set; }
}