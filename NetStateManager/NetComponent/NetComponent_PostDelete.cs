using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_PostDelete : TNetComponent_SuccessOrFail<EventArg_PostDelete, EventArg_Null, Error>
{
    public NetComponent_PostDelete()
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

    public override void OnEvent(EventArg_PostDelete Arg)
    {
        NetLog.Log("NetComponent_PostDelete : OnEvent");

        MsgReqPostDelete pReq = new MsgReqPostDelete();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.L_PostSeq = new List<int>();
        for (int i = 0; i < Arg.m_nPostSeqList.Count; i++)
        {
            pReq.L_PostSeq.Add(Arg.m_nPostSeqList[i]);
        }

        SendPacket<MsgReqPostDelete, MsgAnsPostDelete>(pReq, RecvPacket_PostDeleteSuccess, RecvPacket_PostDeleteFailure);
    }

    public void RecvPacket_PostDeleteSuccess(MsgReqPostDelete pReq, MsgAnsPostDelete pAns)
    {
        NetLog.Log("NetComponent_PostDelete : PostDeleteSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_PostDeleteFailure(MsgReqPostDelete pReq, Error error)
    {
        NetLog.Log("NetComponent_PostDelete : PostDeleteFailure");

        GetFailureEvent().OnEvent(error);
    }
}
