namespace Whatsapp.web.net;

/// <summary>
/// Poll send options
/// </summary>
public class PollSendOptions
{
    /// <summary>
    /// If false it is a single choice poll, otherwise it is a multiple choice poll (false by default)
    /// </summary>
    public bool AllowMultipleAnswers { get; set; } = false;

    /// <summary>
    /// The custom message secret, can be used as a poll ID. NOTE: it has to be a unique vector with a length of 32
    /// </summary>
    public List<int>? MessageSecret { get; set; }

    public bool PollInvalidated { get; set; }

    public bool IsSentCagPollCreation { get; set; }
}