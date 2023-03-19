using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetSpecialStageList : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_GetSpecialStageList()
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
        NetLog.Log("NetComponent_GetSpecialStageList : OnEvent");

        MsgReqGetSpecialStageList pReq = new MsgReqGetSpecialStageList();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqGetSpecialStageList, MsgAnsGetSpecialStageList>(pReq, RecvPacket_GetSpecialStageListSuccess, RecvPacket_GetSpecialStageListFailure);
    }

    public void RecvPacket_GetSpecialStageListSuccess(MsgReqGetSpecialStageList pReq, MsgAnsGetSpecialStageList pAns)
    {
        NetLog.Log("NetComponent_GetSpecialStageList : RecvPacket_GetSpecialStageListSuccess");

        InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.ClearSpecialStageInfo();

        int nStartCheckTime = int.MaxValue;

        foreach(KeyValuePair<int,SpecialStageList> pInfo in pAns.M_SpecialStageList)
        {
            SpecialStageInfo pSpecialStageInfo = new SpecialStageInfo();
            pSpecialStageInfo.nModeID = pInfo.Value.I_ModeId;
            pSpecialStageInfo.nAvailablePlayerRank = pInfo.Value.I_AvailablePlayerRank;
            pSpecialStageInfo.eScheduleType = (eScheduleType_GameMode)pInfo.Value.I_ScheduleType;
            pSpecialStageInfo.nScheduleDayStart = pInfo.Value.I_ScheduleDayStart;
            pSpecialStageInfo.nScheduleDayEnd = pInfo.Value.I_ScheduleDayEnd;
            pSpecialStageInfo.nScheduleTimeStart = pInfo.Value.I_ScheduleTimeStart;
            pSpecialStageInfo.nScheduleTimeEnd = pInfo.Value.I_ScheduleTimeEnd;

            if(pSpecialStageInfo.eScheduleType == eScheduleType_GameMode.Limit)
            {
                int nTimeStamp = pSpecialStageInfo.nScheduleTimeStart;
                DateTime dateTime = new DateTime(1970, 1, 1, 9, 0, 0, 0);
                dateTime = dateTime.AddSeconds(nTimeStamp);

                //TimeSpan tsStartTime = dateTime - DateTime.Now;

                if (DateTime.Now > dateTime)
                {
                    InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.AddSpecialStageInfo(pSpecialStageInfo);
                }
                else
                {
                    if (nStartCheckTime > nTimeStamp)
                    {
                        nStartCheckTime = nTimeStamp;
                    }
                }
            }
            else
            {
                InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.AddSpecialStageInfo(pSpecialStageInfo);
            }

            //InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.AddSpecialStageInfo(pInfo.Value.I_ModeId, pInfo.Value.I_AvailablePlayerRank, (eScheduleType)pInfo.Value.I_ScheduleType, pInfo.Value.I_ScheduleDayStart, pInfo.Value.I_ScheduleDayEnd, pInfo.Value.I_ScheduleTimeStart, pInfo.Value.I_ScheduleTimeEnd);
        }

        if (nStartCheckTime != int.MaxValue)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 9, 0, 0, 0);
            dateTime = dateTime.AddSeconds(nStartCheckTime);
            TimeSpan tsStartTime = dateTime - DateTime.Now;

            //TimerManager.Instance.AddTimer(eTimerType.eGameModeStartCheck, (float)tsStartTime.TotalSeconds, 0);
            InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.SetGameModeStartCheckTimer((float)tsStartTime.TotalSeconds);
        }

        //for(int i=0; i<pAns.M_SpecialStageList.Count; i++)
        //{
        //    InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.AddSpecialStageInfo(pAns.M_SpecialStageList[i].I_ModeId, pAns.M_SpecialStageList[i].I_AvailablePlayerRank, (eScheduleType)pAns.M_SpecialStageList[i].I_ScheduleType, pAns.M_SpecialStageList[i].I_ScheduleDayStart, pAns.M_SpecialStageList[i].I_ScheduleDayEnd, pAns.M_SpecialStageList[i].I_ScheduleTimeStart, pAns.M_SpecialStageList[i].I_ScheduleTimeEnd);
        //}

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetSpecialStageListFailure(MsgReqGetSpecialStageList pReq, Error pError)
    {
        NetLog.Log("NetComponent_GetSpecialStageList : RecvPacket_GetSpecialStageListFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
