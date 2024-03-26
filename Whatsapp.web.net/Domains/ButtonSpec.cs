namespace Whatsapp.web.net.Domains;

/// <summary>
/// Button specification used in the Buttons constructor
/// </summary>
public class ButtonSpec
{
    /// <summary>
    /// Custom ID to set on the button. A random one will be generated if one is not passed.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The text to show on the button.
    /// </summary>
    public string Body { get; set; }
}