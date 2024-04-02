namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a Poll on WhatsApp
/// </summary>
public class Poll
{
    /// <summary>
    /// The name of the poll
    /// </summary>
    public string PollName { get; }

    /// <summary>
    /// The array of poll options
    /// </summary>
    public List<PollOption> PollOptions { get; }

    /// <summary>
    /// The send options for the poll
    /// </summary>
    public PollSendOptions Options { get; }

    /// <summary>
    /// Constructor for Poll class
    /// </summary>
    /// <param name="pollName">The name of the poll</param>
    /// <param name="pollOptions">The array of poll options</param>
    /// <param name="options">The send options for the poll</param>
    public Poll(string pollName, List<string> pollOptions, PollSendOptions? options = null)
    {
        PollName = pollName.Trim();
        PollOptions = [];
        for (var i = 0; i < pollOptions.Count; i++)
        {
            PollOptions.Add(new PollOption { Name = pollOptions[i].Trim(), LocalId = i });
        }
        Options = options ?? new PollSendOptions();
    }
}