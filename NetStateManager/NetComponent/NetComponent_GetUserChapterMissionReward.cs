using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetUserChapterMissionReward : TNetComponent_SuccessOrFail<EventArg_GetUserChapterMissionReward, EventArg_Null, Error>
{
    public NetComponent_GetUserChapterMissionReward()
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

    public override void OnEvent(EventArg_GetUserChapterMissionReward Arg)
    {
        NetLog.Log("NetComponent_GetUserChapterMissionReward : OnEvent");
        
        MsgReqGetUserChapterMissionReward pReq = new MsgReqGetUserChapterMissionReward();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_ChapterId = Arg.m_nChapterID;
        pReq.I_RewardLevel = Arg.m_nRewardLevel;

        SendPacket<MsgReqGetUserChapterMissionReward, MsgAnsGetUserChapterMissionReward>(pReq, RecvPacket_GetUserChapterMissionRewardSuccess, RecvPacket_getUserChapterMissionRewardFailure);
    }

    public void RecvPacket_GetUserChapterMissionRewardSuccess(MsgReqGetUserChapterMissionReward pReq, MsgAnsGetUserChapterMissionReward pAns)
    {
        NetLog.Log("NetComponent_GetUserChapterMissionReward : RecvPacket_GetUserChapterMissionRewardSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_getUserChapterMissionRewardFailure(MsgReqGetUserChapterMissionReward pReq, Error error)
    {
        NetLog.Log("NetComponent_GetUserChapterMissionReward : RecvPacket_GetUserChapterMissionRewardFailure");

        GetFailureEvent().OnEvent(error);
    }
}
