using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_InGameStart : TNetComponent_SuccessOrFail<EventArg_InGameStart, EventArg_Null, Error>
{
    public NetComponent_InGameStart()
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

    public override void OnEvent(EventArg_InGameStart Arg)
    {
        NetLog.Log("NetComponent_InGameStart : OnEvent");

#if UNITY_EDITOR
        if (GameConfig.Instance.m_IsInGameSuerverCommunication == false)
        {
            GetSuccessEvent().OnEvent(EventArg_Null.Object);
            return;
        }
#endif

        MsgReqGameStart pReq = new MsgReqGameStart();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_EpisodeId = Arg.m_nEpisodeID;
        pReq.I_ChapterId = Arg.m_nChapterID;
        pReq.I_StageId = Arg.m_nStageID;
        pReq.I_IsSweep = Arg.m_nIsSweep;

        SendPacket<MsgReqGameStart, MsgAnsGameStart>(pReq, RecvPacket_InGameStartSuccess, RecvPacket_InGameStartFailure);
    }

    public void RecvPacket_InGameStartSuccess(MsgReqGameStart pReq, MsgAnsGameStart pAns)
    {
        NetLog.Log("NetComponent_InGameStart : RecvPacket_InGameStartSuccess");

        ItemInvenItemInfo pInfo = InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemInfo_byTableID((int)eItemType.Energy);
        pInfo.m_nItemCount = pAns.I_Ap;

        if(InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemCount_byTableID((int)eItemType.Energy) < MyInfo.Instance.m_nMaxAP)
        {
            Debug.Log("행동력 충전 남은시간 임의 설정");
            //TimeInfo.Instance.SetTimer(MyInfo.Instance.m_nAPChargeTerm - 3);
            InventoryInfoManager.Instance.m_pItemInvenInfo.SetTimerForEnergy(MyInfo.Instance.m_nAPChargeTerm - 3);
        }

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_InGameStartFailure(MsgReqGameStart pReq, Error pError)
    {
        NetLog.Log("NetComponent_InGameStart : RecvPacket_InGameStartFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
