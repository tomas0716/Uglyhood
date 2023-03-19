using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;

public class NetComponent_Firebase_Register : TNetComponent_Next<EventArg_Null, EventArg_Null>
{
    public NetComponent_Firebase_Register()
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
        NetLog.Log("NetComponent_Firebase_Register : OnEvent");

        string strDate = string.Format("{0}{1}{2}", EspressoInfo.Instance.m_pConnectDateTime.Year, EspressoInfo.Instance.m_pConnectDateTime.Month.ToString("D2"), EspressoInfo.Instance.m_pConnectDateTime.Day.ToString("D2"));

        Helper.FirebaseLogEvent("user_register", "date", strDate);

        GetNextEvent().OnEvent(EventArg_Null.Object);
    }
}
