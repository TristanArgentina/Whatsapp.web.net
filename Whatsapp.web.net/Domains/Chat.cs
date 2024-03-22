namespace Whatsapp.web.net.Domains;

public class Chat
{
    public string Id { get; private set; }
    public string Name { get; set; }
    public bool IsGroup { get; private set; }
    public bool IsReadOnly { get; private set; }
    public int UnreadCount { get; private set; }
    public long Timestamp { get; private set; }
    public bool Archived { get; private set; }
    public bool Pinned { get; private set; }
    public bool IsMuted { get; private set; }
    public long MuteExpiration { get; private set; }
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

        Id = data.id;
        Name = data.formattedTitle || data.title;
        IsGroup = data.isGroup;
        IsReadOnly = data.isReadOnly;
        UnreadCount = data.unreadCount;
        Timestamp = data.t;
        Archived = data.archive;
        Pinned = data.pin != null;
        IsMuted = data.isMuted;
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
        return data.isGroup
            ? new GroupChat(data)
            : new PrivateChat(data);
    }

}