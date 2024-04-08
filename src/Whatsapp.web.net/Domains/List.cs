namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a message type List
/// </summary>
public class List
{
    /// <summary>
    /// The description of the message body
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// The text of the list button
    /// </summary>
    public string ButtonText { get; }

    /// <summary>
    /// The title of the message
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// The footer of the message
    /// </summary>
    public string Footer { get; }

    /// <summary>
    /// The sections of the message
    /// </summary>
    public List<ListSection> Sections { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="List"/> class.
    /// </summary>
    /// <param name="body">The description of the message body.</param>
    /// <param name="buttonText">The text of the list button.</param>
    /// <param name="sections">The sections of the message.</param>
    /// <param name="title">The title of the message.</param>
    /// <param name="footer">The footer of the message.</param>
    public List(string body, string buttonText, List<ListSection> sections, string title = null, string footer = null)
    {
        Description = body;
        ButtonText = buttonText;
        Title = title;
        Footer = footer;
        Sections = FormatSections(sections);
    }

    /// <summary>
    /// Formats the sections of the message
    /// </summary>
    /// <param name="sections">The sections to format.</param>
    /// <returns>The formatted list sections.</returns>
    private List<ListSection> FormatSections(List<ListSection> sections)
    {
        if (sections.Count == 0)
        {
            throw new Exception("[LT02] List without sections");
        }

        if (sections.Count > 1 && sections.FindAll(s => string.IsNullOrEmpty(s.Title)).Count > 1)
        {
            throw new Exception("[LT05] You can't have more than one empty title.");
        }

        return sections.ConvertAll(section => new ListSection(section));
    }
}