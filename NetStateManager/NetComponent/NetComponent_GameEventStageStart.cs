using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;
using sg.protocol.common;

public class NetComponent_GameEventStageStart : TNetComponent_SuccessOrFail<EventArg_EventStageStart, EventArg_Null, Error>
{
    public NetComponent_GameEventStageStart()
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

    public override void OnEvent(EventArg_EventStageStart Arg)
    {
        NetLog.Log("NetComponent_GameEventStageStart : OnEvent");

        MsgReqGameEventStageStart pReq = new MsgReqGameEventStageStart();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_ModeId = Arg.m_nModeID;
        pReq.I_Difficulty = Arg.m_nDifficulty;
        pReq.I_StageId = Arg.m_nStageID;
        pReq.I_IsSweep = Arg.m_nIsSweep;

        SendPacket<MsgReqGameEventStageStart, MsgAnsGameEventStageStart>(pReq, RecvPacket_GameEventStageStartSuccess, RecvPacket_GameEventStageStartFailure);
    }

    public void RecvPacket_GameEventStageStartSuccess(MsgReqGameEventStageStart pReq, MsgAnsGameEventStageStart pAns)
    {
        NetLog.Log("NetComponent_GameEventStageStart : RecvPacket_GameEventStageStartSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GameEventStageStartFailure(MsgReqGameEventStageStart pReq, Error error)
    {
        NetLog.Log("NetComponent_GameEventStageStart : RecvPacket_GameEventStageStartFailure");

        GetFailureEvent().OnEvent(error);
    }
}
