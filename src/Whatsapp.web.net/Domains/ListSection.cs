namespace Whatsapp.web.net.Domains;

/// <summary>
/// Represents a section of a list message
/// </summary>
public class ListSection
{
    public ListSection(ListSection section)
    {
        Title = section.Title;
        Rows = FormatRows(section.Rows);
    }

    /// <summary>
    /// The title of the section
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// The rows of the section
    /// </summary>
    public List<ListRow> Rows { get; private set; }

    /// <summary>
    /// Formats the rows of a section
    /// </summary>
    /// <param name="rows">The rows to format.</param>
    /// <returns>The formatted list rows.</returns>
    private List<ListRow> FormatRows(List<ListRow> rows)
    {
        if (rows.Count == 0)
        {
            throw new Exception("[LT03] Section without rows");
        }
        rows.ForEach(row => row.FormatRow());
        return rows;
    }
}