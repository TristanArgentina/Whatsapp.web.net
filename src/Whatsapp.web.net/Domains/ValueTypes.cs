namespace Whatsapp.web.net.Domains;

public class ValueTypes
{
    public string Value { get; set; }

    public string[] Types { get; set; }

    public string CharSet { get; set; } = "CHARSET=UTF-8";


    public ValueTypes(string key, string value)
    {
        Value = value;
        var dataSplit = key.Split(';');
        var type = dataSplit.FirstOrDefault(d => d.StartsWith("type"));
        if (type is not null)
        {
            Types = type.Split('=')[1].Split(",");
        }
    }

}