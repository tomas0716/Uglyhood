using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_PostConfirm : TNetComponent_SuccessOrFail<EventArg_PostConfirm, EventArg_Null, Error>
{
    public NetComponent_PostConfirm()
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

    public override void OnEvent(EventArg_PostConfirm Arg)
    {
        NetLog.Log("NetComponent_PostConfirm : OnEvent");

        MsgReqPostConfirm pReq = new MsgReqPostConfirm();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.L_PostSeq = new List<int>();
        for(int i=0; i<Arg.m_nPostSeqList.Count; i++)
        {
            pReq.L_PostSeq.Add(Arg.m_nPostSeqList[i]);
        }

        SendPacket<MsgReqPostConfirm, MsgAnsPostConfirm>(pReq, RecvPacket_PostConfirmSuccess, RecvPacket_PostConfirmFailure);
    }

    public void RecvPacket_PostConfirmSuccess(MsgReqPostConfirm pReq, MsgAnsPostConfirm pAns)
    {
        NetLog.Log("NetComponent_PostConfirm : PostConfirmSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_PostConfirmFailure(MsgReqPostConfirm pReq, Error error)
    {
        NetLog.Log("NetComponent_PostConfirm : RecvPacket_PostConfirmFailure");

        GetFailureEvent().OnEvent(error);
    }
}
