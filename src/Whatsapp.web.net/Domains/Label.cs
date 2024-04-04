namespace Whatsapp.web.net.Domains;

/// <summary>
/// WhatsApp Business Label information
/// </summary>
public class Label
{
    /// <summary>
    /// Label ID
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// Label name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Label hex color
    /// </summary>
    public string HexColor { get; private set; }

    public Label(dynamic? labelData)
    {
        if (labelData != null)
            Patch(labelData);
    }

    private void Patch(dynamic data)
    {
        Id = data.id;
        Name = data.name;
        HexColor = data.hexColor;
    }

}