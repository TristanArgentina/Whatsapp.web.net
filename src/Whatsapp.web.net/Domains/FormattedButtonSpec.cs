namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a formatted button specification
/// </summary>
public class FormattedButtonSpec
{
    public FormattedButtonSpec(ButtonSpec buttonSpec)
    {
        ButtonId = !string.IsNullOrEmpty(buttonSpec.Id) ? buttonSpec.Id : Util.GenerateHash(6);
        ButtonText = new Dictionary<string, string> {{"displayText", buttonSpec.Body}};
        Type = 1;

    }

    /// <summary>
    /// The ID of the button
    /// </summary>
    public string ButtonId { get; private set; }

    /// <summary>
    /// The type of the button
    /// </summary>
    public int Type { get; private set; }

    /// <summary>
    /// The text of the button
    /// </summary>
    public Dictionary<string, string> ButtonText { get; private set; }
}