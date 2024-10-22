﻿using Newtonsoft.Json.Linq;

namespace Whatsapp.web.net.Domains;

public class MessageInfo
{
    public DeliveryInfo[] Delivery { get; private set; }

    public int DeliveryRemaining { get; private set; }

    public PlayedInfo[] Played { get; private set; }

    public int PlayedRemaining { get; private set; }

    public ReadInfo[] Read { get; private set; }

    public int ReadRemaining { get; private set; }

    public MessageInfo(dynamic? data)
    {
        Patch(data);
    }

    private void Patch(dynamic? data)
    {
        if (data is null) return;
        if (data.Type == JTokenType.Null) return;

        Delivery = data.delivery is not null && data.delivery.Type != JTokenType.Null
            ? ((JArray)data.delivery).Select(d => new DeliveryInfo(d)).ToArray()
            : [];
        DeliveryRemaining = int.Parse(data.deliveryRemaining.ToString());
        Played = data.played is not null && data.played.Type != JTokenType.Null
            ? ((JArray)data.played).Select(d => new PlayedInfo(d)).ToArray()
            : [];
        PlayedRemaining = int.Parse(data.playedRemaining.ToString());
        Read = data.read is not null && data.read.Type != JTokenType.Null
            ? ((JArray)data.read).Select(d => new ReadInfo(d)).ToArray()
            : [];
        ReadRemaining = int.Parse(data.readRemaining.ToString());
    }


}