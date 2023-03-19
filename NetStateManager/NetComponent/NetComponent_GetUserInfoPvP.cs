using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetUserInfoPvP : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_GetUserInfoPvP()
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
        NetLog.Log("NetComponent_GetUserInfoPvP : OnEvent");

        MsgReqGetInfoUserPvp pReq = new MsgReqGetInfoUserPvp();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqGetInfoUserPvp, MsgAnsGetInfoUserPvp>(pReq, RecvPacket_GetUserInfoPvPSuccess, RecvPacket_GetUserInfoPvPFailure);
    }

    public void RecvPacket_GetUserInfoPvPSuccess(MsgReqGetInfoUserPvp pReq, MsgAnsGetInfoUserPvp pAns)
    {
        NetLog.Log("NetComponent_GetUserInfoPvP : RecvPacket_GetUserInfoPvPSuccess");

        PvPStageInfo pInfo = new PvPStageInfo();
        pInfo.m_nLimitGameCount = pAns.St_UserInfoPvp.I_LimitGameCount;
        pInfo.m_nLastUseDeckID = pAns.St_UserInfoPvp.I_LastUseDeckId;

        InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.AddPvPStageInfo(pInfo);
        InventoryInfoManager.Instance.m_pDeckInfo.SetGameModeDeckID(eGameMode.PvpStage, pInfo.m_nLastUseDeckID);

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetUserInfoPvPFailure(MsgReqGetInfoUserPvp pReq, Error error)
    {
        NetLog.Log("NetComponent_GetUserInfoPvP : RecvPacket_GetUserInfoPvPFailure");

        GetFailureEvent().OnEvent(error);
    }
}
