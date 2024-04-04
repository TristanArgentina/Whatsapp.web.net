namespace Whatsapp.web.net.Domains;

public class Email : ValueTypes
{
    public Email(string value, string key = "EMAIL") : base(key, value)
    {
    }

    public override string ToString()
    {
        var result = $"EMAIL";
        if (Types is not null && Types.Any())
        {
            result += ";type=" + string.Join(',', Types);
        }
        result += $":{Value}";
        return result;
    }
}