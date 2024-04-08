namespace Whatsapp.web.net.Domains;

public class GroupParticipant
{

    public UserId Id { get; private set; }

    public bool IsAdmin { get; private set; }

    public bool IsSuperAdmin { get; private set; }

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


}