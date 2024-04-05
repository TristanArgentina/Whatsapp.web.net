using Newtonsoft.Json.Linq;

namespace Whatsapp.web.net.Domains;

public class UserId
{
    protected UserId(dynamic? data)
    {
        Patch(data);
    }

    protected UserId()
    {
    }

    protected UserId(string user, string server)
    {
        User = user;
        Server = server;
        Id = $"{User}@{Server}";
        _serialized = Id;
    }

    protected void Patch(dynamic? data)
    {
        if (data is null) return;
        User = data.user;
        Server = data.server;
        _serialized = data._serialized;
        Id = $"{User}@{Server}";
    }

    public string Id { get; private set; }

    public string Server { get; private set; }

    public string User { get; private set; }

    public string _serialized { get; set; }

    public override string ToString()
    {
        return $"{Id}";
    }

    public static UserId? Create(dynamic? data)
    {
        if (data is null) return null;
        if (data is not string && data.Type == JTokenType.Null) return null;
        if (data is string || data.Type == JTokenType.String)
        {
            var dataString = data!.ToString();
            if (string.IsNullOrEmpty(dataString)) return null;

            var dataSplit = dataString.Split("@");
            var user = dataSplit[0];
            var server = dataSplit[1];

            if (server == "c.us")
            {
                return new ContactId(user, server);
            }

            if (server == "g.us")
            {
                return new GroupId(user, server);
            }
            return new UserId(user, server);

        }

        if (data.server == "c.us")
        {
            return ContactId.Create(data);
        }
        if (data.server == "g.us")
        {
            return GroupId.Create(data);
        }
        return new UserId(data);

    }

}