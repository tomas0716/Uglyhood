using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GameDailyStageStart : TNetComponent_SuccessOrFail<EventArg_DailyStageStart, EventArg_Null, Error>
{
    public NetComponent_GameDailyStageStart()
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

    public override void OnEvent(EventArg_DailyStageStart Arg)
    {
        NetLog.Log("NetComponent_GameDailyStageStart : OnEvent");

        MsgReqGameDailyStageStart pReq = new MsgReqGameDailyStageStart();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_ModeId = Arg.m_nModeID;
        pReq.I_Difficulty = Arg.m_nDifficulty;
        pReq.I_StageId = Arg.m_nStageID;
        pReq.I_PassKind = Arg.m_nPassKind;
        pReq.I_IsSweep = Arg.m_nIsSweep;

        SendPacket<MsgReqGameDailyStageStart, MsgAnsGameDailyStageStart>(pReq, RecvPacket_GameDailyStageStartSuccess, RecvPacket_GameDailyStageStartFailure);
    }

    public void RecvPacket_GameDailyStageStartSuccess(MsgReqGameDailyStageStart pReq, MsgAnsGameDailyStageStart pAns)
    {
        NetLog.Log("NetComponent_GameDailyStageStart : RecvPacket_GameDailyStageStartSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GameDailyStageStartFailure(MsgReqGameDailyStageStart pReq, Error pError)
    {
        NetLog.Log("NetComponent_GameDailyStageStart : RecvPacket_GameDailyStageStartFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
