namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a GroupNotification on WhatsApp
/// </summary>
public class GroupNotification
{
    /// <summary>
    /// ID that represents the groupNotification
    /// </summary>
    public MessageId Id { get; private set; }

    /// <summary>
    /// Extra content
    /// </summary>
    public string Body { get; private set; } = "";

    /// <summary>
    /// GroupNotification type
    /// </summary>
    public string Type { get; private set; }

    /// <summary>
    /// Unix timestamp for when the groupNotification was created
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// ID for the Chat that this groupNotification was sent for.
    /// </summary>
    public UserId ChatId { get; private set; }

    /// <summary>
    /// ContactId for the user that produced the GroupNotification.
    /// </summary>
    public UserId Author { get; private set; }

    /// <summary>
    /// Contact IDs for the users that were affected by this GroupNotification.
    /// </summary>
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