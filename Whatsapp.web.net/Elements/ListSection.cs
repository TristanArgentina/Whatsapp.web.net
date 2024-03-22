namespace Whatsapp.web.net.Elements;

/// <summary>
/// Represents a section of a list message
/// </summary>
public class ListSection
{
    /// <summary>
    /// The title of the section
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The rows of the section
    /// </summary>
    public List<ListRow> Rows { get; set; }
}