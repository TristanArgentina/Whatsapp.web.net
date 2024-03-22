namespace Whatsapp.web.net.Domains;

public class ClientInfo
{
    public ClientInfo(dynamic? data)
    {
        Patch(data);
    }

    public string Id { get; set; }

    public string Ref { get; set; }

    public string RefTTL { get; set; }

    public string PushName { get; set; }

    public UserId Wid { get; set; }

    public string Platform { get; set; }

    public string SmbTos { get; set; }

    protected void Patch(dynamic? data)
    {
        if (data is null) { return; }

        Id = data.id;
        Ref = data.@ref;
        RefTTL = data.refTTL;
        PushName = data.pushname;
        Wid = UserId.Create(data.wid);
        Platform = data.platform;
        SmbTos = data.smbTos;
    }



    public override string ToString()
    {
        return $"PushName: {PushName}\n" +
               $"Wid: {Wid}\n" +
               $"Ref: {Ref}\n" +
               $"Platform: {Platform}";
    }
}