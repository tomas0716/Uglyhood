using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_PvPGameStart : TNetComponent_SuccessOrFail<EventArg_PvPGameStart, EventArg_Null, Error>
{
    public NetComponent_PvPGameStart()
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

    public override void OnEvent(EventArg_PvPGameStart Arg)
    {
        NetLog.Log("NetComponent_PvPGameStart : OnEvent");

        MsgReqGamePvpStart pReq = new MsgReqGamePvpStart();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_DeckId = Arg.m_nDeckID;
        pReq.I_StageId = Arg.m_nStageID;

        SendPacket<MsgReqGamePvpStart, MsgAnsGamePvpStart>(pReq, RecvPacket_PvPGameStartSuccess, RecvPacket_PvPGameStartFailure);
    }

    public void RecvPacket_PvPGameStartSuccess(MsgReqGamePvpStart pReq, MsgAnsGamePvpStart pAns)
    {
        NetLog.Log("NetComponent_PvPGameStart : PvPGameStartSuccess");

        InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.SetPvPSeqNumber(pAns.I_GameSeq);

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_PvPGameStartFailure(MsgReqGamePvpStart pReq, Error error)
    {
        NetLog.Log("NetComponent_PvPGameStart : PvPGameStartFailure");

        GetFailureEvent().OnEvent(error);
    }
}
