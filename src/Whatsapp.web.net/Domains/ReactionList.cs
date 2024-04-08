namespace Whatsapp.web.net.Domains;

public class ReactionList
{
    public ReactionList(dynamic data)
    {
        if (data is not null)
        {
            Patch(data);
        }
    }

    private void Patch(dynamic data)
    {
        Id = data.id;
        AggregateEmoji = data.aggregateEmoji;
        HasReactionByMe = data.hasReactionByMe;
        foreach (var sender in data.senders)
        {
            Senders.Add(new Reaction(sender));
        }
    }

    public string Id { get; private set; }
    public string AggregateEmoji { get; private set; }
    public bool HasReactionByMe { get; private set; }
    public List<Reaction> Senders { get; private set; } = [];
}