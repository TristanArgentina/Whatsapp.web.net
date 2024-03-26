namespace Whatsapp.web.net.Domains;

public class Label
{
    public string Id { get; private set; }
    public string Name { get; private set; }
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