namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a message with buttons
/// </summary>
public class Buttons
{
    /// <summary>
    /// Message body
    /// </summary>
    public object Body { get; }

    /// <summary>
    /// Title of the message
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Footer of the message
    /// </summary>
    public string Footer { get; }

    /// <summary>
    /// Type of the message
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Buttons of the message
    /// </summary>
    public List<FormattedButtonSpec> ButtonsList { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Buttons"/> class.
    /// </summary>
    /// <param name="body">The message body.</param>
    /// <param name="buttons">The buttons of the message.</param>
    /// <param name="title">The title of the message.</param>
    /// <param name="footer">The footer of the message.</param>
    public Buttons(object body, List<ButtonSpec> buttons, string title = null, string footer = null)
    {
        Body = body;
        Title = title;
        Footer = footer;

        if (body is MessageMedia)
        {
            Type = "media";
            Title = "";
        }
        else
        {
            Type = "chat";
        }

        ButtonsList = FormatButtons(buttons);
        if (ButtonsList.Count == 0)
        {
            throw new Exception("[BT01] No buttons");
        }
    }

    /// <summary>
    /// Creates button array from a simple array
    /// </summary>
    /// <param name="buttons">The buttons to format.</param>
    /// <returns>The formatted button specifications.</returns>
    private List<FormattedButtonSpec> FormatButtons(List<ButtonSpec> buttons)
    {
        buttons = buttons.GetRange(0, Math.Min(buttons.Count, 3)); // Phone users can only see 3 buttons, so let's limit this
        return buttons.ConvertAll(btn => new FormattedButtonSpec(btn));
    }
}