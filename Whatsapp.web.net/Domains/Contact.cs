namespace Whatsapp.web.net.Domains;

public class Contact
{
    public UserId Id { get; set; }
    public string Number { get; set; }
    public bool IsBusiness { get; set; }
    public bool IsEnterprise { get; set; }
    public List<object> Labels { get; set; }
    public string Name { get; set; }
    public string Pushname { get; set; }
    public object SectionHeader { get; set; }
    public string ShortName { get; set; }
    public object StatusMute { get; set; }
    public object Type { get; set; }
    public object VerifiedLevel { get; set; }
    public object VerifiedName { get; set; }
    public bool IsMe { get; set; }
    public bool IsUser { get; set; }
    public bool IsGroup { get; set; }
    public bool IsWAContact { get; set; }
    public bool IsMyContact { get; set; }
    public bool IsBlocked { get; set; }

    protected Contact()
    {
    }


    public Contact(dynamic data)
    {
        if (data != null)
        {
            Patch(data);
        }
    }

    protected void Patch(dynamic? data)
    {
        if (data is null) return;

        Id = UserId.Create(data.id);
        Number = data.userid;
        IsBusiness = data.isBusiness;
        IsEnterprise = data.isEnterprise;
        Labels = data.labels;
        Name = data.name;
        Pushname = data.pushname;
        SectionHeader = data.sectionHeader;
        ShortName = data.shortName;
        StatusMute = data.statusMute;
        Type = data.type;
        VerifiedLevel = data.verifiedLevel;
        VerifiedName = data.verifiedName;
        IsMe = data.isMe;
        IsUser = data.isUser;
        IsGroup = data.isGroup;
        IsWAContact = data.isWAContact;
        IsMyContact = data.isMyContact;
        IsBlocked = data.isBlocked;
    }

    public static Contact Create(dynamic? data)
    {
        if (data is null) return null;
        return data.isBusiness
            ? (Contact)BusinessContact.CreateBusinessContact(data)
            : (Contact)PrivateContact.CreatePrivateContact(data);
    }

}