namespace Whatsapp.web.net.Domains;

public class Phone : ValueTypes
{
    public Phone(string value, string key = "TEL"): base(key, value)
    {
        var dataSplit = key.Split(';');
        var wait = dataSplit.FirstOrDefault(d => d.StartsWith("waid"));
        if (wait is not null)
        {
            Waid = wait.Split('=')[1];
        }
    }

    public string Waid { get; set; }

    public override string ToString()
    {
        var result = $"TEL";
        if (Types is not null && Types.Any())
        {
            result += ";type=" + string.Join(',',Types);
        }

        if (string.IsNullOrEmpty(Waid))
        {
            result += $"waid={Waid}";
        }

        result += $":{Value}";
        return result;
    }
}