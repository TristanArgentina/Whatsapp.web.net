namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a row of a list message
/// </summary>
public class ListRow
{
    /// <summary>
    /// The ID of the row
    /// </summary>
    public string RowId { get; private set; }

    /// <summary>
    /// The title of the row
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// The description of the row
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Formats the rows of a section
    /// </summary>
    /// <param name="rows">The rows to format.</param>
    /// <returns>The formatted list rows.</returns>
    public void FormatRow()
    {
        RowId = string.IsNullOrEmpty(RowId) ? Util.GenerateHash(6) : RowId;
        Description = string.IsNullOrEmpty(Description) ? "" : Description;
    }
}