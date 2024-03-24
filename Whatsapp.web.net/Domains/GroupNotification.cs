namespace Whatsapp.web.net.Domains;

public class GroupNotification
{
    public MessageId Id { get; private set; }
    public string Body { get; private set; } = "";
    public string Type { get; private set; }
    public DateTime Timestamp { get; private set; }
    public UserId ChatId { get; private set; }
    public UserId Author { get; private set; }
    public List<string> RecipientIds { get; private set; } = [];

    public GroupNotification(dynamic data)
    {
        if (data != null)
            Patch(data);
    }

    private void Patch(dynamic? data)
    {
        Id = new MessageId(data.id);
        Body = data.body ?? "";
        Type = data.subtype;
        Timestamp = DateTimeOffset.FromUnixTimeSeconds((long)data.t).UtcDateTime;
        ChatId = UserId.Create(data.id.remote);
        Author = UserId.Create(data.author);

        if (data.recipients != null)
        {
            foreach (var recipient in data.recipients)
            {
                RecipientIds.Add(recipient);
            }
        }
    }
}