namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a Contact on WhatsApp
/// </summary>
public class Contact
{
    /// <summary>
    /// ID that represents the contact
    /// </summary>
    public UserId Id { get; set; }

    /// <summary>
    /// Contact's phone number
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    /// Indicates if the contact is a business contact
    /// </summary>
    public bool IsBusiness { get; set; }

    /// <summary>
    /// Indicates if the contact is an enterprise contact
    /// </summary>
    public bool IsEnterprise { get; set; }

    /// <summary>
    /// The contact's name, as saved by the current user
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The name that the contact has configured to be shown publically
    /// </summary>
    public string Pushname { get; set; }

    /// <summary>
    /// A shortened version of name
    /// </summary>
    public string ShortName { get; set; }

    /// <summary>
    /// Indicates if the contact is the current user's contact
    /// </summary>
    public bool IsMe { get; set; }

    /// <summary>
    /// Indicates if the contact is a user contact
    /// </summary>
    public bool IsUser { get; set; }

    /// <summary>
    /// Indicates if the contact is a group contact
    /// </summary>
    public bool IsGroup { get; set; }

    /// <summary>
    /// Indicates if the number is registered on WhatsApp
    /// </summary>
    public bool IsWAContact { get; set; }

    /// <summary>
    /// Indicates if the number is saved in the current phone's contacts
    /// </summary>
    public bool IsMyContact { get; set; }

    /// <summary>
    /// Indicates if you have blocked this contact
    /// </summary>
    public bool IsBlocked { get; set; }

    public object StatusMute { get; set; }
    public object Type { get; set; }
    public object SectionHeader { get; set; }

    public List<object> Labels { get; set; }

    public object VerifiedLevel { get; set; }
    public object VerifiedName { get; set; }

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