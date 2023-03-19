using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetToastMessage : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    private string m_strToastMessage = "none";

    public NetComponent_GetToastMessage()
    {

    }

    public override void OnDestroy()
    {
    }

    public override void LateUpdate()
    {
    }

    public override void Update()
    {
    }

    private void ClearToastMessage()
    {
        m_strToastMessage = "none";
    }

    public string GetToastMessage()
    {
        return m_strToastMessage;
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        NetLog.Log("NetComponent_GetToastMessage : OnEvent");

        MsgReqGetToastMessage pReq = new MsgReqGetToastMessage();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqGetToastMessage, MsgAnsGetToastMessage>(pReq, RecvPacket_GetToastMessageSuccess, RecvPacket_GetToastMessageFailure);
    }

    public void RecvPacket_GetToastMessageSuccess(MsgReqGetToastMessage pReq, MsgAnsGetToastMessage pAns)
    {
        NetLog.Log("NetComponent_GetToastMessage : RecvPacket_GetToastMessageSuccess");

        ClearToastMessage();

        m_strToastMessage = pAns.S_Msg;

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetToastMessageFailure(MsgReqGetToastMessage pReq, Error error)
    {
        NetLog.Log("NetComponent_GetToastMessage : RecvPacket_GetToastMessageFailure");

        GetFailureEvent().OnEvent(error);
    }
}
