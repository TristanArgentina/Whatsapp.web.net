﻿using Newtonsoft.Json.Linq;

namespace Whatsapp.web.net.Domains;

public class MsgKey
{

    public string _serialized { get; private set; }

    /// <summary>
    /// Indicates if the message was sent by the current user
    /// </summary>
    public bool FromMe { get; private set; }

    /// <summary>
    /// ID that represents the message
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public UserId Remote { get; private set; }

    public string Self { get; private set; }

    public MsgKey(dynamic data)
    {
        Patch(data);
    }

    private void Patch(dynamic? data)
    {
        if (data is null) return;
        if (data.Type == JTokenType.String)
        {
            Id = data;
            _serialized = data;
        }
        else
        {
            Id = data.id;
            FromMe = data.fromMe;
            Remote = UserId.Create(data.remote);
            Self = data.self;
            _serialized = data._serialized ?? Id;
        }
    }

}