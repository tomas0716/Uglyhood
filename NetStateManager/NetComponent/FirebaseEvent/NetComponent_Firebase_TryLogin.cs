using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;

public class NetComponent_Firebase_TryLogin : TNetComponent_Next<EventArg_Null, EventArg_Null>
{
    public NetComponent_Firebase_TryLogin()
    {
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
    }

    public override void LateUpdate()
    {
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        NetLog.Log("NetComponent_Firebase_TryLogin : OnEvent");

        Helper.FirebaseLogEvent("login");

        GetNextEvent().OnEvent(EventArg_Null.Object);
    }
}
