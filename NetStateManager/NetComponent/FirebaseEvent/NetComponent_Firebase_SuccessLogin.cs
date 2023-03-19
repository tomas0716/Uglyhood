using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;

public class NetComponent_Firebase_SuccessLogin : TNetComponent_Next<EventArg_Null, EventArg_Null>
{
    public NetComponent_Firebase_SuccessLogin()
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
        NetLog.Log("NetComponent_Firebase_SuccessLogin : OnEvent");

        Parameter[] parameters = new Parameter[2];
        parameters[0] = new Parameter("env_name", NetworkManager.Instance.m_strEnvName);
        parameters[1] = new Parameter("user_player_id", MyInfo.Instance.m_nUserIndex);

        Helper.FirebaseLogEvent("login_success", parameters);

        Debug.Log("firebase log");

        GetNextEvent().OnEvent(EventArg_Null.Object);
    }
}
