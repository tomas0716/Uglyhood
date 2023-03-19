using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetUserBattlePass : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_GetUserBattlePass()
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
        NetLog.Log("NetComponent_GetUserBattlePass : OnEvent");

		MsgReqGetUserBattlePass pReq = new MsgReqGetUserBattlePass();
		pReq.Uid = MyInfo.Instance.m_nUserIndex;

		SendPacket<MsgReqGetUserBattlePass, MsgAnsGetUserBattlePass>(pReq, RecvPacket_GetUserBattlePassSuccess, RecvPacket_GetUserBattlePassFailure);
	}

    public void RecvPacket_GetUserBattlePassSuccess(MsgReqGetUserBattlePass pReq, MsgAnsGetUserBattlePass pAns)
    {
        NetLog.Log("NetComponent_GetUserBattlePass : RecvPacket_GetUserBattlePassSuccess");

        BattlePassInfo.Instance.m_nSeasonID = pAns.I_SeasonId;

        if (BattlePassInfo.Instance.m_nSeasonID != 0)
        {
            BattlePassInfo.Instance.m_nLevel = pAns.I_FreeNowLevel; // 프리만 가지고 한다.
            BattlePassInfo.Instance.m_nPoint = pAns.I_Point;
            BattlePassInfo.Instance.m_IsPassTicketBuy = pAns.I_IsPaid == 0 ? false : true;
            BattlePassInfo.Instance.m_strBattlePassProductID = pAns.S_ProductId;
            BattlePassInfo.Instance.m_nRecvRewardLevel_Free = pAns.I_FreeRecvLevel;
            BattlePassInfo.Instance.m_nRecvRewardLevel_Paid = pAns.I_PaidRecvLevel;

            int nMaxLevel = ExcelDataManager.Instance.m_pBattlePass_RewardLevel.GetMaxLevel(BattlePassInfo.Instance.m_nSeasonID);

            for (int i = 1; i <= pAns.I_FreeRecvLevel; ++i)
            {
                ExcelData_BattlePass_RewardLevelInfo pRewardLevelInfo = ExcelDataManager.Instance.m_pBattlePass_RewardLevel.GetRewardLevelInfo(BattlePassInfo.Instance.m_nSeasonID, i);

                if (pRewardLevelInfo != null)
                {
                    pRewardLevelInfo.m_IsClientData_FreeRewardGet = true;
                }
            }

            for (int i = pAns.I_FreeRecvLevel + 1; i <= nMaxLevel; ++i)
            {
                ExcelData_BattlePass_RewardLevelInfo pRewardLevelInfo = ExcelDataManager.Instance.m_pBattlePass_RewardLevel.GetRewardLevelInfo(BattlePassInfo.Instance.m_nSeasonID, i);

                if (pRewardLevelInfo != null)
                {
                    pRewardLevelInfo.m_IsClientData_FreeRewardGet = false;
                }
            }

            for (int i = 1; i <= pAns.I_PaidRecvLevel; ++i)
            {
                ExcelData_BattlePass_RewardLevelInfo pRewardLevelInfo = ExcelDataManager.Instance.m_pBattlePass_RewardLevel.GetRewardLevelInfo(BattlePassInfo.Instance.m_nSeasonID, i);

                if (pRewardLevelInfo != null)
                {
                    pRewardLevelInfo.m_IsClientData_PaidRewardGet = true;
                }
            }

            for (int i = pAns.I_PaidRecvLevel + 1; i <= nMaxLevel; ++i)
            {
                ExcelData_BattlePass_RewardLevelInfo pRewardLevelInfo = ExcelDataManager.Instance.m_pBattlePass_RewardLevel.GetRewardLevelInfo(BattlePassInfo.Instance.m_nSeasonID, i);

                if (pRewardLevelInfo != null)
                {
                    pRewardLevelInfo.m_IsClientData_PaidRewardGet = false;
                }
            }

            int nTimeStamp = pAns.I_SeasonEnd;
            DateTime dateTime = new DateTime(1970, 1, 1, 9, 0, 0, 0);
            BattlePassInfo.Instance.OnTimerAction(dateTime.AddSeconds(nTimeStamp));
        }
        else
        {
            // 현재 진행중인 배틀패스 없음.
            // 다음 배틀패스 계산
            Timer pTimer = TimerManager.Instance.GetTimer(eTimerType.eLoginAfterTime);
            float fElapsedTime = pTimer.GetElapsedTime();
            TimeSpan pTimeSpan = TimeSpan.FromSeconds((double)fElapsedTime);
            DateTime pCurrDateTime = EspressoInfo.Instance.m_pConnectDateTime + pTimeSpan;

            int nNumData = ExcelDataManager.Instance.m_pBattlePass_Season.GetNumSeasonInfo();
            DateTime pFindDateTime = new DateTime(4000, 1, 1, 9, 0, 0, 0);
            ExcelData_BattlePass_SeasonInfo pNextBattlePass_SeasonInfo = null;

            for (int i = 0; i < nNumData; ++i)
            {
                ExcelData_BattlePass_SeasonInfo pBattlePass_SeasonInfo = ExcelDataManager.Instance.m_pBattlePass_Season.GetSeasonInfo_byIndex(i);

                if (pBattlePass_SeasonInfo != null && pBattlePass_SeasonInfo.m_Schedule_Start > pCurrDateTime && pFindDateTime > pBattlePass_SeasonInfo.m_Schedule_Start)
                {
                    pFindDateTime = pBattlePass_SeasonInfo.m_Schedule_Start;
                    pNextBattlePass_SeasonInfo = pBattlePass_SeasonInfo;
                }
            }

            if (pNextBattlePass_SeasonInfo != null)
            {
                BattlePassInfo.Instance.OnTimerAction_StartCheck(pNextBattlePass_SeasonInfo.m_Schedule_Start);
            }
        }

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetUserBattlePassFailure(MsgReqGetUserBattlePass pReq, Error error)
    {
        NetLog.Log("NetComponent_GetUserBattlePass : RecvPacket_GetUserBattlePassFailure");

        GetFailureEvent().OnEvent(error);
    }
}
