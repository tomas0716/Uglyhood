using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetDailyStageList : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_GetDailyStageList()
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
        NetLog.Log("NetComponent_GetDailyStageList : OnEvent");

        MsgReqGetDailyStageList pReq = new MsgReqGetDailyStageList();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqGetDailyStageList, MsgAnsGetDailyStageList>(pReq, RecvPacket_GetDailyStageListSuccess, RecvPacket_GetDailyStageListFailure);
    }

    public void RecvPacket_GetDailyStageListSuccess(MsgReqGetDailyStageList pReq, MsgAnsGetDailyStageList pAns)
    {
        NetLog.Log("NetComponent_GetDailyStageList : RecvPacket_GetDailyStageListSuccess");

        InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.ClearDailyStageInfo();
        InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.ClearSpecialStageStarCount();

        foreach (KeyValuePair<int,DailyStageList> pInfo in pAns.M_DailyStageList)
        {
            DailyStageInfo pDailyStageInfo = new DailyStageInfo();
            pDailyStageInfo.nModeID = pInfo.Value.I_ModeId;
            pDailyStageInfo.nDifficulty = pInfo.Value.I_Difficulty;
            pDailyStageInfo.nStageID = pInfo.Value.I_StageId;
            pDailyStageInfo.nPassCountFree = pInfo.Value.I_PassCountFree;
            pDailyStageInfo.nFreeClearCount = pInfo.Value.I_FreeClearCount;
            pDailyStageInfo.nPassCountPaid = pInfo.Value.I_PassCountPaid;
            pDailyStageInfo.nPaidClearCount = pInfo.Value.I_PaidClearCount;
            pDailyStageInfo.nPaidGemCount = pInfo.Value.I_PaidGemCount;
            pDailyStageInfo.nPaidItemID = pInfo.Value.I_PaidItemId;
            pDailyStageInfo.nPaidItemCount = pInfo.Value.I_PaidItemCount;

            if(pDailyStageInfo.nModeID >= 201 && pDailyStageInfo.nModeID <= 207)
            {
                InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.AddDailyStageInfo(pDailyStageInfo);
            }
            else
            {
                InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.AddDailyStageInfo_Second(pDailyStageInfo);
            }

            //InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.AddDailyStageInfo(pInfo.Value.I_ModeId, pInfo.Value.I_Difficulty, pInfo.Value.I_StageId, pInfo.Value.I_PassCountFree, pInfo.Value.I_FreeClearCount, pInfo.Value.I_PassCountPaid, pInfo.Value.I_PaidClearCount, pInfo.Value.I_PaidGemCount, pInfo.Value.I_PaidItemId, pInfo.Value.I_PaidItemCount);
        }
        
        foreach(KeyValuePair<int, UserDailyStageStar> pInfo in pAns.M_UserDailyStageStar)
        {
            InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.AddSpecialStageStarCount_byStageID(pInfo.Value.I_StageId, pInfo.Value.I_NowStarCount);
        }

        //for(int i=0; i<pAns.M_DailyStageList.Count; i++)
        //{
        //    InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.AddDailyStageInfo(pAns.M_DailyStageList[i].I_ModeId, pAns.M_DailyStageList[i].I_Difficulty, pAns.M_DailyStageList[i].I_StageId, pAns.M_DailyStageList[i].I_PassCountFree, pAns.M_DailyStageList[i].I_FreeClearCount, pAns.M_DailyStageList[i].I_PassCountPaid, pAns.M_DailyStageList[i].I_PaidClearCount, pAns.M_DailyStageList[i].I_PaidGemCount, pAns.M_DailyStageList[i].I_PaidItemId, pAns.M_DailyStageList[i].I_PaidItemCount);
        //}

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetDailyStageListFailure(MsgReqGetDailyStageList pReq, Error pError)
    {
        NetLog.Log("NetComponent_GetDailyStageList : RecvPacket_GetDailyStageListFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
