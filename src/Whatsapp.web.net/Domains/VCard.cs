using Newtonsoft.Json;

namespace Whatsapp.web.net.Domains;

public class VCard
{
    public VCard(string version, string fullName, string[] names, Email email, Phone telephone, DateTime revision)
    {
        Version = version;
        FullName = fullName;
        Names = names;
        Email = email;
        Telephone = telephone;
        Revision = revision;
    }

    public string Version { get; private set; }
    public string FullName { get; private set; }
    public string[] Names { get; private set; }
    public Email Email { get; private set; }
    public Phone Telephone { get; private set; }
    public DateTime Revision { get; private set; }

    [JsonConstructor]
    public VCard(dynamic data)
    {
        Patch(data);
    }

    public VCard()
    {
    }

    private void Patch(dynamic data)
    {
        string dataString = data.ToString();
        var lines = dataString.Contains("\r\n")
            ? dataString.Split(["\r\n"], StringSplitOptions.RemoveEmptyEntries)
            : dataString.Split(["\n"], StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var parts = line.Split([':'], 2);
            if (parts.Length != 2) continue; 
            var key = parts[0].Trim();
            var value = parts[1].Trim();

            switch (key)
            {
                case "BEGIN":
                case "END":
                    break;
                case "VERSION":
                    Version = value;
                    break;
                case var fnKey when fnKey.StartsWith("FN"):
                    FullName = value;
                    break;
                case var nameKey when nameKey.StartsWith("N"):
                    Names = value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    break;
                case var emailKey when emailKey.StartsWith("EMAIL"):
                    Email = new Email(value, key);
                    break;
                case var emailKey when emailKey.StartsWith("TEL"):
                    Telephone = new Phone(value, key);
                    break;
                case "REV":
                    Revision = DateTime.Parse(value);
                    break;
                default:
                    break;
            }
        }
    }

    public override string ToString()
    {
        return $"BEGIN:VCARD\r\nVERSION:{Version}\r\nFN;CHARSET=UTF-8:{FullName}\r\nN;CHARSET=UTF-8:{string.Join(";", Names)};;;\r\n{Email}\r\n{Telephone}\r\nREV:{Revision.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}\r\nEND:VCARD";
    }
}