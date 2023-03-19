using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;

public class NetComponent_UserAccountDelete : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_UserAccountDelete()
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
        NetLog.Log("NetComponent_UserAccountDelete : OnEvent");

        MsgReqSetUserDelete pReq = new MsgReqSetUserDelete();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqSetUserDelete, MsgAnsSetUserDelete>(pReq, RecvPacket_UserAccountDeleteSuccess, RecvPacket_UserAccountDeleteFailure);
    }

    public void RecvPacket_UserAccountDeleteSuccess(MsgReqSetUserDelete pReq, MsgAnsSetUserDelete pAns)
    {
        NetLog.Log("NetComponent_UserAccountDelete : RecvPacket_UserAccountDeleteSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_UserAccountDeleteFailure(MsgReqSetUserDelete pReq, Error pError)
    {
        NetLog.Log("NetComponent_UserAccountDelete : RecvPacket_UserAccountDeleteFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
