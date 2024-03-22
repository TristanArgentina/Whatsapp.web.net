namespace Whatsapp.web.net.Elements;

/// <summary>
/// Represents a formatted button specification
/// </summary>
public class FormattedButtonSpec
{
    /// <summary>
    /// The ID of the button
    /// </summary>
    public string ButtonId { get; set; }

    /// <summary>
    /// The type of the button
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// The text of the button
    /// </summary>
    public Dictionary<string, string> ButtonText { get; set; }
}