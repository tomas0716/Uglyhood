using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetEventStageList : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_GetEventStageList()
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
        NetLog.Log("NetComponent_GetEventStageList : OnEvent");

        MsgReqGetEventStageList pReq = new MsgReqGetEventStageList();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqGetEventStageList, MsgAnsGetEventStageList>(pReq, RecvPacket_GetEventStageListSuccess, RecvPacket_GetEventStageListFailure);
    }

    public void RecvPacket_GetEventStageListSuccess(MsgReqGetEventStageList pReq, MsgAnsGetEventStageList pAns)
    {
        NetLog.Log("NetComponent_GetEventStageList : RecvPacket_GetEventStageListSuccess");

        InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.ClearEventStageInfo();

        foreach(KeyValuePair<int,EventStageList> pInfo in pAns.M_EventStageList)
        {
            EventStageInfo pEventStageInfo = new EventStageInfo();
            pEventStageInfo.m_nModeID = pInfo.Value.I_ModeId;
            pEventStageInfo.m_nDifficulty = pInfo.Value.I_Difficulty;
            pEventStageInfo.m_nStageID = pInfo.Value.I_StageId;
            pEventStageInfo.m_nPassCountDaily = pInfo.Value.I_PassCountDaily;
            pEventStageInfo.m_nFreeClearCount = pInfo.Value.I_FreeClearCount;
            pEventStageInfo.m_nPaidClearCount = pInfo.Value.I_PaidClearCount;
            pEventStageInfo.m_ePaidType = (eEventStagePaymentType)pInfo.Value.I_PaidType;
            pEventStageInfo.m_nPaidID = pInfo.Value.I_PaidId;
            pEventStageInfo.m_nPaidCount = pInfo.Value.I_PaidCount;

            InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.AddEventStageInfo(pEventStageInfo);
        }

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetEventStageListFailure(MsgReqGetEventStageList pReq, Error error)
    {
        NetLog.Log("NetComponent_GetEventStageList : RecvPacket_GetEventStageListFailure");

        GetFailureEvent().OnEvent(error);
    }
}
