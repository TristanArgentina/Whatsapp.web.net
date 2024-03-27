using Newtonsoft.Json;

namespace Whatsapp.web.net.Domains;

public class VCard
{
    public string Version { get; set; }
    public string FullName { get; set; }
    public string[] Names { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public DateTime Revision { get; set; }

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
        var lines = dataString.Split(["\r\n"], StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var parts = line.Split([':'], 2);
            var key = parts[0];
            var value = parts[1];

            switch (key)
            {
                case "BEGIN":
                case "END":
                    break;
                case "VERSION":
                    Version = value;
                    break;
                case "FN;CHARSET=UTF-8":
                    FullName = value;
                    break;
                case "N;CHARSET=UTF-8":
                    Names = value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    break;
                case "EMAIL;CHARSET=UTF-8;type=HOME,INTERNET":
                    Email = value;
                    break;
                case "TEL;TYPE=HOME,VOICE":
                    Telephone = value;
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
        return $"BEGIN:VCARD\r\nVERSION:{Version}\r\nFN;CHARSET=UTF-8:{FullName}\r\nN;CHARSET=UTF-8:{string.Join(";", Names)};;;\r\nEMAIL;CHARSET=UTF-8;type=HOME,INTERNET:{Email}\r\nTEL;TYPE=HOME,VOICE:{Telephone}\r\nREV:{Revision.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}\r\nEND:VCARD";
    }
}