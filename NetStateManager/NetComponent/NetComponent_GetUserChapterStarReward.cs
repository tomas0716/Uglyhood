using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetUserChapterStarReward : TNetComponent_SuccessOrFail<EventArg_GetUserChapterStarReward, EventArg_Null, Error>
{
    public NetComponent_GetUserChapterStarReward()
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

    public override void OnEvent(EventArg_GetUserChapterStarReward Arg)
    {
        NetLog.Log("NetComponent_GetUserChapterStarReward : OnEvent");

        MsgReqGetUserChapterStarReward pReq = new MsgReqGetUserChapterStarReward();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_ChapterId = Arg.m_nChapterID;
        pReq.I_RewardLevel = Arg.m_nRewardLevel;

        SendPacket<MsgReqGetUserChapterStarReward, MsgAnsGetUserChapterStarReward>(pReq, RecvPacket_GetUserChapterStarRewardSuccess, RecvPacket_GetUserChapterStarRewardFailure);
    }

    public void RecvPacket_GetUserChapterStarRewardSuccess(MsgReqGetUserChapterStarReward pReq, MsgAnsGetUserChapterStarReward pAns)
    {
        NetLog.Log("NetComponent_GetUserChapterStarReward : RecvPacket_GetUserChapterStarRewardSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetUserChapterStarRewardFailure(MsgReqGetUserChapterStarReward pReq, Error error)
    {
        NetLog.Log("NetComponent_GetUserChapterStarReward : RecvPacket_GetUserChapterStarRewardFailure");

        GetFailureEvent().OnEvent(error);
    }
}
