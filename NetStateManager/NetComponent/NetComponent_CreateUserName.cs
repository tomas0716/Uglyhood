using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;

public class NetComponent_CreateUserName : TNetComponent_SuccessOrFail<EventArg_CreateUserName, EventArg_Null, Error>
{
    private string m_strUserName = "";

    public NetComponent_CreateUserName()
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

    public string GetUserName()
    {
        return m_strUserName;
    }

    public override void OnEvent(EventArg_CreateUserName Arg)
    {
        NetLog.Log("NetComponent_CreateUserName : OnEvent");

        m_strUserName = Arg.m_strUserName;

        MsgReqCreateUserName pReq = new MsgReqCreateUserName();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.S_UserName = Arg.m_strUserName;

        SendPacket<MsgReqCreateUserName, MsgAnsCreateUserName>(pReq, RecvPacket_CreateUserNameSuccess, RecvPacket_CreateUserNameFailure);
    }

    public void RecvPacket_CreateUserNameSuccess(MsgReqCreateUserName pReq, MsgAnsCreateUserName pAns)
    {
        NetLog.Log("NetComponent_CreateUserName : RecvPacket_LoginSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_CreateUserNameFailure(MsgReqCreateUserName pReq, Error pError)
    {
        NetLog.Log("NetComponent_CreateUserName : RecvPacket_LoginFailure");

        Debug.Log(pError.Err_code);

        GetFailureEvent().OnEvent(pError);
    }
}
