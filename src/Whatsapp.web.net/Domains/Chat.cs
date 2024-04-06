namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a Chat on WhatsApp
/// </summary>
public class Chat
{
    /// <summary>
    /// ID that represents the chat
    /// </summary>
    public UserId Id { get; private set; }

    /// <summary>
    /// Title of the chat
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Indicates if the Chat is a Group Chat
    /// </summary>
    public bool IsGroup { get; private set; }

    /// <summary>
    /// Indicates if the Chat is readonly
    /// </summary>
    public bool IsReadOnly { get; private set; }

    /// <summary>
    /// Amount of messages unread
    /// </summary>
    public int UnreadCount { get; private set; }

    /// <summary>
    /// Unix timestamp for when the last activity occurred
    /// </summary>
    public DateTime? Timestamp { get; private set; }

    /// <summary>
    /// Indicates if the Chat is archived
    /// </summary>
    public bool Archived { get; private set; }

    /// <summary>
    /// Indicates if the Chat is pinned
    /// </summary>
    public bool Pinned { get; private set; }

    /// <summary>
    /// Indicates if the chat is muted or not
    /// </summary>
    public bool IsMuted { get; private set; }

    /// <summary>
    /// Unix timestamp for when the mute expires
    /// </summary>
    public long MuteExpiration { get; private set; }

    /// <summary>
    /// Last message fo chat
    /// </summary>
    public Message? LastMessage { get; private set; }


    protected Chat()
    {

    }

    public Chat(dynamic? data)
    {
        Patch(data);
    }

    protected void Patch(dynamic? data)
    {
        if (data is null) return;

        Id = UserId.Create(data.id);
        Name = data.name;
        IsGroup = data.isGroup;
        IsReadOnly = data.isReadOnly is null ? false : data.isReadOnly;
        UnreadCount = data.unreadCount;
        Timestamp = Util.ConvertToDate(data.t);
        Archived = data.archive is null ? false : bool.Parse(data.archive.ToString());
        Pinned = data.pin != null;
        IsMuted = data.muteExpiration == 0;
        MuteExpiration = data.muteExpiration;
        LastMessage = data.lastMessage != null ? new Message(data.lastMessage) : null;
    }

    public override string ToString()
    {
        return $"Chat Id: {Id}\n" +
               $"Name: {Name}\n" +
               $"IsGroup: {IsGroup}\n" +
               $"IsReadOnly: {IsReadOnly}\n" +
               $"UnreadCount: {UnreadCount}\n" +
               $"Timestamp: {Timestamp}\n" +
               $"Archived: {Archived}\n" +
               $"Pinned: {Pinned}\n" +
               $"IsMuted: {IsMuted}\n" +
               $"MuteExpiration: {MuteExpiration}\n" +
               $"LastMessage: {LastMessage?.ToString() ?? "No messages yet"}";
    }

    public static Chat? Create(dynamic data)
    {
        if (data == null) return null;
        return (bool)data.isGroup
            ? new GroupChat(data)
            : new PrivateChat(data);
    }

}