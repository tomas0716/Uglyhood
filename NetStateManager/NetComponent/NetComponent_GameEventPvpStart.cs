using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GameEventPvpStart : TNetComponent_SuccessOrFail<EventArg_GameEventPvpStart, EventArg_Null, Error>
{
    public NetComponent_GameEventPvpStart()
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

    public override void OnEvent(EventArg_GameEventPvpStart Arg)
    {
        NetLog.Log("NetComponent_GameEventPvpStart : OnEvent");

        MsgReqGameEventPvpStart pReq = new MsgReqGameEventPvpStart();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_DeckId = Arg.m_nDeckID;
        pReq.I_StageId = Arg.m_nStageID;

        SendPacket<MsgReqGameEventPvpStart, MsgAnsGameEventPvpStart>(pReq, RecvPacket_GameEventPvpStartSuccess, RecvPacket_GameEventPvpStartFailure);
    }

    public void RecvPacket_GameEventPvpStartSuccess(MsgReqGameEventPvpStart pReq, MsgAnsGameEventPvpStart pAns)
    {
        NetLog.Log("NetComponent_GameEventPvpStart : RecvPacket_GameEventPvpStartSuccess");

        EspressoInfo.Instance.m_nEventPVPGameSeq = pAns.I_GameSeq;

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GameEventPvpStartFailure(MsgReqGameEventPvpStart pReq, Error error)
    {
        NetLog.Log("NetComponent_GameEventPvpStart : RecvPacket_GameEventPvpStartFailure");

        GetFailureEvent().OnEvent(error);
    }
}
