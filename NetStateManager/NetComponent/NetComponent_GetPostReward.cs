using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetPostReward : TNetComponent_SuccessOrFail<EventArg_GetPostReward, EventArg_Null, Error>
{
    public NetComponent_GetPostReward()
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

    public override void OnEvent(EventArg_GetPostReward Arg)
    {
        NetLog.Log("NetComponent_GetPostReward : OnEvent");

        MsgReqPostReward pReq = new MsgReqPostReward();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.L_PostSeq = new List<int>();
        for (int i = 0; i < Arg.m_nPostSeqList.Count; i++)
        {
            pReq.L_PostSeq.Add(Arg.m_nPostSeqList[i]);
        }

        SendPacket<MsgReqPostReward, MsgAnsPostReward>(pReq, RecvPacket_GetPostRewardSuccess, RecvPacket_GetPostRewardFailure);
    }

    public void RecvPacket_GetPostRewardSuccess(MsgReqPostReward pReq, MsgAnsPostReward pAns)
    {
        NetLog.Log("NetComponent_GetPostReward : GetPostRewardSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetPostRewardFailure(MsgReqPostReward pReq, Error error)
    {
        NetLog.Log("NetComponent_GetPostReward : GetPostRewardFailure");

        GetFailureEvent().OnEvent(error);
    }
}
