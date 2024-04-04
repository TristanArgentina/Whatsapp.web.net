﻿namespace Whatsapp.web.net.Domains;

/// <summary>
/// Current connection information
/// </summary>
public class ClientInfo
{
    /// <summary>
    /// Current user ID
    /// </summary>
    public UserId Id { get; set; }

    /// <summary>
    /// Name configured to be shown in push notifications
    /// </summary>
    public string PushName { get; set; }

    /// <summary>
    /// Platform WhatsApp is running on
    /// </summary>
    public string Platform { get; set; }

    public string Ref { get; set; }

    public string RefTTL { get; set; }
    
    public string SmbTos { get; set; }

    public ClientInfo(dynamic? data)
    {
        Patch(data);
    }

    protected void Patch(dynamic? data)
    {
        if (data is null) { return; }

        Id = UserId.Create(data.wid);
        PushName = data.pushname;
        Platform = data.platform;
        Ref = data.@ref;
        RefTTL = data.refTTL;
        SmbTos = data.smbTos;
    }



    public override string ToString()
    {
        return $"PushName: {PushName}\n" +
               $"Wid: {Id}\n" +
               $"Ref: {Ref}\n" +
               $"Platform: {Platform}";
    }
}