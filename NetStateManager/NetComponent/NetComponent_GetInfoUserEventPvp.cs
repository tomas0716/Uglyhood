using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetInfoUserEventPvp : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_GetInfoUserEventPvp()
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
        NetLog.Log("NetComponent_GetInfoUserEventPvp : OnEvent");

        MsgReqGetInfoUserEventPvp pReq = new MsgReqGetInfoUserEventPvp();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqGetInfoUserEventPvp, MsgAnsGetInfoUserEventPvp>(pReq, RecvPacket_GetInfoUserEventPvpSuccess, RecvPacket_GetInfoUserEventPvpFailure);
    }

    public void RecvPacket_GetInfoUserEventPvpSuccess(MsgReqGetInfoUserEventPvp pReq, MsgAnsGetInfoUserEventPvp pAns)
    {
        NetLog.Log("NetComponent_GetInfoUserEventPvp : RecvPacket_GetInfoUserEventPvpSuccess");

        PvPStageInfo pInfo = new PvPStageInfo();
        pInfo.m_nLimitGameCount = pAns.St_UserInfoPvp.I_LimitGameCount;
        pInfo.m_nLastUseDeckID = pAns.St_UserInfoPvp.I_LastUseDeckId;

        InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.AddEventPvpStageInfo(pInfo);

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetInfoUserEventPvpFailure(MsgReqGetInfoUserEventPvp pReq, Error error)
    {
        NetLog.Log("NetComponent_GetInfoUserEventPvp : RecvPacket_GetInfoUserEventPvpFailure");

        GetFailureEvent().OnEvent(error);
    }
}
