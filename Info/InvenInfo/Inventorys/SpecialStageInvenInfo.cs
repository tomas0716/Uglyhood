using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using sg.protocol.basic;

public class SpecialStageInfo
{
    public int nModeID = 0;
    public int nAvailablePlayerRank = 0;
    public eScheduleType_GameMode eScheduleType = eScheduleType_GameMode.infinity;
    public int nScheduleDayStart = 0;
    public int nScheduleDayEnd = 0;
    public int nScheduleTimeStart = 0;
    public int nScheduleTimeEnd = 0;

    public SpecialStageInfo()
    {
    }
}

public class DailyStageInfo
{
    public int nModeID = 0;
    public int nDifficulty = 0;
    public int nStageID = 0;
    public int nPassCountFree = 0;
    public int nFreeClearCount = 0;
    public int nPassCountPaid = 0;
    public int nPaidClearCount = 0;
    public int nPaidGemCount = 0;
    public int nPaidItemID = 0;
    public int nPaidItemCount = 0;

    public DailyStageInfo()
    {
    }
}

public class EventStageInfo
{
    public int m_nModeID = 0;
    public int m_nDifficulty = 0;
    public int m_nStageID = 0;
    public int m_nPassCountDaily = 0;
    public int m_nFreeClearCount = 0;
    public int m_nPaidClearCount = 0;
    public eEventStagePaymentType m_ePaidType = eEventStagePaymentType.Free;
    public int m_nPaidID = 0;
    public int m_nPaidCount = 0;
}

public class PvPStageInfo
{
    public int m_nLimitGameCount = 0;
    public int m_nLastUseDeckID = 0;

    public PvPStageInfo()
    {
    }
}

public class PvPUnitInfo
{
    public int m_nUnitID = 0;
    public int m_nLevel = 0;
    public int m_nSkillLevel = 0;
    public int m_nMaxHP = 0;
    public int m_nMaxSP = 0;
    public int m_nSP_ChargePerBlock = 0;
    public int m_nATK = 0;
    public int m_nCombat = 0;
}

public class PvPPlayerInfo
{
    public int m_nID = 0;
    public string m_strName_stringTable = "";
    public int m_nAvatarImage_byUnitID = 0;
    public int m_nPlayerRank = 0;

    public Dictionary<int, PvPUnitInfo> m_pPvPUnitInfo_byOrder = new Dictionary<int, PvPUnitInfo>();

    public PvPPlayerInfo()
    {
    }
}

public class SpecialStageInvenInfo
{
    private List<SpecialStageInfo> m_pSpecialStageInfoList = new List<SpecialStageInfo>();
    private List<DailyStageInfo> m_pDailyStageInfoList = new List<DailyStageInfo>();
    private List<DailyStageInfo> m_pDailyStageInfoList_Second = new List<DailyStageInfo>();

    //private List<PvPStageInfo> m_pPvPStageInfoList = new List<PvPStageInfo>();
    //private List<PvPPlayerInfo> m_pPvPPlayerInfoList = new List<PvPPlayerInfo>();

    private Dictionary<int, DailyStageInfo> m_pDailyStageInfoTable_byStageID = new Dictionary<int, DailyStageInfo>();
    private Dictionary<int, DailyStageInfo> m_pDailyStageInfoTableSecond_byStageID = new Dictionary<int, DailyStageInfo>();

    private Dictionary<int, int> m_pSpecialStageStarCount_byStageID = new Dictionary<int, int>();

    private PvPPlayerInfo m_pPvPPlayerInfo = new PvPPlayerInfo();
    private PvPStageInfo m_pPvPStageInfo = new PvPStageInfo();

    private int m_nPvPSeq = 0;

    private List<EventStageInfo> m_pEventStageInfoList = new List<EventStageInfo>();
    private Dictionary<int, EventStageInfo> m_pEventStageInfoTable_byStageID = new Dictionary<int, EventStageInfo>();

    private NetComponent_GetSpecialStageList m_pNetComponent_GetSpecialStageList = new NetComponent_GetSpecialStageList();

    private PvPStageInfo m_pEventPvpStageInfo = new PvPStageInfo();

    public SpecialStageInvenInfo()
    {
    }

    public void Init()
    {
        EventDelegateManager.Instance.OnEventTimer_Complete += OnTimer_Complete;
    }

    public void OnDestroy()
    {
        EventDelegateManager.Instance.OnEventTimer_Complete -= OnTimer_Complete;
    }

    public void ClearAll()
    {
        m_pSpecialStageInfoList.Clear();
        m_pDailyStageInfoList.Clear();
    }

    public void AddSpecialStageInfo(SpecialStageInfo pSpecialStageInfo)
    {
        m_pSpecialStageInfoList.Add(pSpecialStageInfo);

        if (pSpecialStageInfo.eScheduleType == eScheduleType_GameMode.Limit)
        {
            int nTimeStamp = pSpecialStageInfo.nScheduleDayEnd;
            DateTime dateTime = new DateTime(1970, 1, 1, 9, 0, 0, 0);
            dateTime = dateTime.AddSeconds(nTimeStamp);

            int nRemainTime = pSpecialStageInfo.nScheduleTimeEnd - (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            //TimeInfo.Instance.SetTimer_ForDailyDungeon(nRemainTime);

            if (pSpecialStageInfo.nModeID < (int)eGameMode.EventStage || pSpecialStageInfo.nModeID >= (int)eGameMode.EventPvpStage)
            {
                if(pSpecialStageInfo.nModeID == (int)eGameMode.EventPvpStage)
                {
                    TimerManager.Instance.AddTimer(eTimerType.eEventPvpStage, nRemainTime, 0);
                }
                else
                {
                    TimerManager.Instance.AddTimer(eTimerType.eDailyDungeon, nRemainTime, 0, pSpecialStageInfo.nModeID);
                }
            }
            else
            {
                TimerManager.Instance.AddTimer(eTimerType.eEventStage, nRemainTime, 0);
            }
        }
        else if (pSpecialStageInfo.eScheduleType == eScheduleType_GameMode.Day_Rotation)
        {
            DateTime nowDT = DateTime.Now;

            int nRemainTime = 99999;

            if (nowDT.DayOfWeek.ToString() == ((eWeek)pSpecialStageInfo.nScheduleDayEnd).ToString())
            {
                nRemainTime = (int)DateTime.Today.AddDays(1).Subtract(DateTime.Now).TotalSeconds;
            }

            //TimeInfo.Instance.SetTimer_ForDailyDungeon(nRemainTime);

            if (pSpecialStageInfo.nModeID != (int)eGameMode.EventStage)
            {
                if (pSpecialStageInfo.nModeID == (int)eGameMode.EventPvpStage)
                {
                    TimerManager.Instance.AddTimer(eTimerType.eEventPvpStage, nRemainTime, 0);
                }
                else
                {
                    TimerManager.Instance.AddTimer(eTimerType.eDailyDungeon, nRemainTime, 0, pSpecialStageInfo.nModeID);
                }
            }
            else
            {
                TimerManager.Instance.AddTimer(eTimerType.eEventStage, nRemainTime, 0);
            }
        }
        //else
        //{
        //    int nTimeStamp = pSpecialStageInfo.nScheduleTimeEnd;
        //    DateTime dateTime = new DateTime(1970, 1, 1, 9, 0, 0, 0);
        //    dateTime = dateTime.AddSeconds(nTimeStamp);

        //    int nRemainTime = pSpecialStageInfo.nScheduleTimeEnd - (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        //    //TimeInfo.Instance.SetTimer_ForDailyDungeon(nRemainTime);

        //    TimerManager.Instance.AddTimer(eTimerType.eDailyDungeon, nRemainTime, 0, pSpecialStageInfo.nModeID);
        //}
    }

    public void SetGameModeStartCheckTimer(float fTime)
    {
        TimerManager.Instance.AddTimer(eTimerType.eGameModeStartCheck, fTime, 0);
    }

    private void OnTimer_Complete(Timer pTimer, eTimerType eType, object parameter)
    {
        if (eType == eTimerType.eDailyDungeon || eType == eTimerType.eEventStage || eType == eTimerType.eGameModeStartCheck || eType == eTimerType.eEventPvpStage)
        {
            TEventDelegate<EventArg_Null> pSuccessEvent = new TEventDelegate<EventArg_Null>();
            TEventDelegate<Error> pFailureEvent = new TEventDelegate<Error>();

            pSuccessEvent.SetFunc(OnGetSpecialStageListSuccess);
            pFailureEvent.SetFunc(OnGetSpecialStageListFailure);

            m_pNetComponent_GetSpecialStageList.SetSuccessEvent(pSuccessEvent);
            m_pNetComponent_GetSpecialStageList.SetFailureEvent(pFailureEvent);

            EventArg_Null pArg = new EventArg_Null();

            m_pNetComponent_GetSpecialStageList.OnEvent(pArg);

            AppInstance.Instance.m_pEventDelegateManager.OnCreateLoadingResponse();
        }
    }

    private void OnGetSpecialStageListSuccess(EventArg_Null Arg)
    {
        EventDelegateManager.Instance.OnLobby_RefreshSpecialGameMode();

        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    private void OnGetSpecialStageListFailure(Error error)
    {
        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    public void ClearSpecialStageInfo()
    {
        m_pSpecialStageInfoList.Clear();
    }

    public void ClearDailyStageInfo()
    {
        m_pDailyStageInfoList.Clear();
        m_pDailyStageInfoTable_byStageID.Clear();

        m_pDailyStageInfoList_Second.Clear();
        m_pDailyStageInfoTableSecond_byStageID.Clear();
    }

    public void ClearEventStageInfo()
    {
        m_pEventStageInfoList.Clear();
        m_pEventStageInfoTable_byStageID.Clear();
    }

    public void ClearSpecialStageStarCount()
    {
        m_pSpecialStageStarCount_byStageID.Clear();
    }

    public void AddDailyStageInfo(DailyStageInfo pDailyStageInfo)
    {
        m_pDailyStageInfoList.Add(pDailyStageInfo);

        m_pDailyStageInfoTable_byStageID.Add(pDailyStageInfo.nStageID, pDailyStageInfo);
    }

    public void AddDailyStageInfo_Second(DailyStageInfo pDailyStageInfo)
    {
        m_pDailyStageInfoList_Second.Add(pDailyStageInfo);
        m_pDailyStageInfoTableSecond_byStageID.Add(pDailyStageInfo.nStageID, pDailyStageInfo);
    }

    public void AddEventStageInfo(EventStageInfo pEventStageInfo)
    {
        m_pEventStageInfoList.Add(pEventStageInfo);
        m_pEventStageInfoTable_byStageID.Add(pEventStageInfo.m_nStageID, pEventStageInfo);
    }

    public void AddSpecialStageStarCount_byStageID(int nStageID, int nStarCount)
    {
        m_pSpecialStageStarCount_byStageID.Add(nStageID, nStarCount);
    }

    public void ClearPvPPlayerInfoTable()
    {
        m_pPvPPlayerInfo = new PvPPlayerInfo();
    }

    public void AddPvPPlayerInfo(PvPPlayerInfo pPvPPlayerInfo)
    {
        m_pPvPPlayerInfo = pPvPPlayerInfo;
    }

    public void ClearPvPStageInfo()
    {
        m_pPvPStageInfo = new PvPStageInfo();
    }

    public void AddPvPStageInfo(PvPStageInfo pPvPStageInfo)
    {
        m_pPvPStageInfo = pPvPStageInfo;
    }

    public void ClearEventPvpStageInfo()
    {
        m_pEventPvpStageInfo = new PvPStageInfo();
    }

    public void AddEventPvpStageInfo(PvPStageInfo pEventPvpStageInfo)
    {
        m_pEventPvpStageInfo = pEventPvpStageInfo;
    }

    public List<SpecialStageInfo> GetSpecialStageInfoList()
    {
        return m_pSpecialStageInfoList;
    }

    public List<DailyStageInfo> GetDailyStageInfoList()
    {
        return m_pDailyStageInfoList;
    }

    public List<DailyStageInfo> GetDailyStageInfoList_Second()
    {
        return m_pDailyStageInfoList_Second;
    }

    public List<EventStageInfo> GetEventStageInfoList()
    {
        return m_pEventStageInfoList;
    }

    public DailyStageInfo GetDailyStageInfo_byKey(int nGameMode, int nDifficulty, int nStageID)
    {
        foreach(DailyStageInfo Info in m_pDailyStageInfoList)
        {
            if(Info.nModeID == nGameMode && Info.nDifficulty == nDifficulty && Info.nStageID == nStageID)
            {
                return Info;
            }
        }

        foreach(DailyStageInfo Info in m_pDailyStageInfoList_Second)
        {
            if (Info.nModeID == nGameMode && Info.nDifficulty == nDifficulty && Info.nStageID == nStageID)
            {
                return Info;
            }
        }

        return null;
    }

    public DailyStageInfo GetDailyStageInfo_byStageID(int nStageID)
    {
        if (m_pDailyStageInfoTable_byStageID.ContainsKey(nStageID))
            return m_pDailyStageInfoTable_byStageID[nStageID];

        if (m_pDailyStageInfoTableSecond_byStageID.ContainsKey(nStageID))
            return m_pDailyStageInfoTableSecond_byStageID[nStageID];

        return null;
    }

    public EventStageInfo GetEventStageInfo_byStageID(int nStageID)
    {
        if (m_pEventStageInfoTable_byStageID.ContainsKey(nStageID))
            return m_pEventStageInfoTable_byStageID[nStageID];

        return null;
    }

    public SpecialStageInfo GetSpecialStageInfo_byModeID(int nModeID)
    {
        for (int i = 0; i < m_pSpecialStageInfoList.Count; i++)
        {
            if (m_pSpecialStageInfoList[i].nModeID == nModeID)
            {
                return m_pSpecialStageInfoList[i];
            }
        }

        return null;
    }

    public SpecialStageInfo GetSpecialEventStageInfo()
    {
        for(int i=0; i<m_pSpecialStageInfoList.Count; i++)
        {
            if(m_pSpecialStageInfoList[i].nModeID >= (int)eGameMode.EventStage && m_pSpecialStageInfoList[i].nModeID <= (int)eGameMode.EventStage_End)
            {
                return m_pSpecialStageInfoList[i];
            }
        }

        return null;
    }

    public int GetEventStageModeID()
    {
        for (int i = 0; i < m_pSpecialStageInfoList.Count; i++)
        {
            if (m_pSpecialStageInfoList[i].nModeID >= (int)eGameMode.EventStage && m_pSpecialStageInfoList[i].nModeID <= (int)eGameMode.EventStage_End)
            {
                return m_pSpecialStageInfoList[i].nModeID;
            }
        }

        return 0;
    }

    public PvPStageInfo GetPvPStageInfo()
    {
        return m_pPvPStageInfo;
    }

    public PvPStageInfo GetEventPvpStageInfo()
    {
        return m_pEventPvpStageInfo;
    }

    public int GetSpecialStageClearStar_byStageID(int nStageID)
    {
        if (m_pSpecialStageStarCount_byStageID.ContainsKey(nStageID))
            return m_pSpecialStageStarCount_byStageID[nStageID];

        return 0;
    }

    public bool GetRedDotIsOn_bySpecialModeID()
    {
        for (int i = 0; i < m_pSpecialStageInfoList.Count; i++)
        {
            if (m_pSpecialStageInfoList[i].nAvailablePlayerRank <= MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic.I_UserRank && m_pSpecialStageInfoList[i].nModeID != (int)eGameMode.DailyStage_Mon)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsUnlock_DailyStage()
    {
        for (int i = 0; i < m_pSpecialStageInfoList.Count; i++)
        {
            // ÀÏÀÏ ½ºÅ×ÀÌÁö °Ë»ç
            if (m_pSpecialStageInfoList[i].nModeID >= (int)eGameMode.DailyStage_Sun && m_pSpecialStageInfoList[i].nModeID <= (int)eGameMode.DailyStage_Sat)
            {
                if (m_pSpecialStageInfoList[i].nAvailablePlayerRank <= MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic.I_UserRank)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool GetRedDotIsOn_byDailyStage()
    {
        for (int i = 0; i < m_pDailyStageInfoList.Count; i++)
        {
            if (m_pDailyStageInfoList[i].nFreeClearCount < m_pDailyStageInfoList[i].nPassCountFree && m_pDailyStageInfoList[i].nModeID != (int)eGameMode.DailyStage_Mon)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsUnlock_DailyStageSecond()
    {
        for (int i = 0; i < m_pSpecialStageInfoList.Count; i++)
        {
            if (m_pSpecialStageInfoList[i].nModeID >= (int)eGameMode.DailyStage_Sun_Second && m_pSpecialStageInfoList[i].nModeID <= (int)eGameMode.DailyStage_Sat_Second)
            {
                if (m_pSpecialStageInfoList[i].nAvailablePlayerRank <= MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic.I_UserRank)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool GetRedDotIsOn_byDailyStageSecond()
    {
        for (int i = 0; i < m_pDailyStageInfoList_Second.Count; i++)
        {
            if (m_pDailyStageInfoList_Second[i].nFreeClearCount < m_pDailyStageInfoList_Second[i].nPassCountFree && m_pDailyStageInfoList_Second[i].nModeID != (int)eGameMode.DailyStage_Sun_Second && m_pDailyStageInfoList_Second[i].nModeID != (int)eGameMode.DailyStage_Sat_Second)
            {
                return true;
            }
        }

        return false;
    }

    public bool GetRedDotIsOn_byEventStage()
    {
        for (int i = 0; i < m_pEventStageInfoList.Count; i++)
        {
            ExcelData_StageInfo pInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(m_pEventStageInfoList[i].m_nStageID);

            if (pInfo.m_nRequiredEnergy <= InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemInfo_byTableID((int)eItemType.Energy).m_nItemCount)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsUnlock_EventStage()
    {
        for (int i = 0; i < m_pSpecialStageInfoList.Count; i++)
        {
            if (m_pSpecialStageInfoList[i].nModeID >= (int)eGameMode.EventStage && m_pSpecialStageInfoList[i].nModeID <= (int)eGameMode.EventStage_End)
            {
                if (m_pSpecialStageInfoList[i].nAvailablePlayerRank <= MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic.I_UserRank)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsUnlock_PvPStage()
    {
        for (int i = 0; i < m_pSpecialStageInfoList.Count; i++)
        {
            if (m_pSpecialStageInfoList[i].nModeID == (int)eGameMode.PvpStage)
            {
                if (m_pSpecialStageInfoList[i].nAvailablePlayerRank <= MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic.I_UserRank)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsUnlock_EventPvpStage()
    {
        for (int i = 0; i < m_pSpecialStageInfoList.Count; i++)
        {
            if (m_pSpecialStageInfoList[i].nModeID == (int)eGameMode.EventPvpStage)
            {
                if (m_pSpecialStageInfoList[i].nAvailablePlayerRank <= MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic.I_UserRank)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool GetRedDotIsOn()
    {

        for (int i = 0; i < m_pSpecialStageInfoList.Count; i++)
        {
            if (m_pSpecialStageInfoList[i].nModeID >= (int)eGameMode.DailyStage_Sun && m_pSpecialStageInfoList[i].nModeID <= (int)eGameMode.DailyStage_Sat)
            {
                for (int j = 0; j < m_pDailyStageInfoList.Count; j++)
                {
                    if (m_pDailyStageInfoList[j].nFreeClearCount < m_pDailyStageInfoList[j].nPassCountFree && m_pDailyStageInfoList[j].nModeID != (int)eGameMode.DailyStage_Mon &&
                       m_pSpecialStageInfoList[i].nAvailablePlayerRank <= MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic.I_UserRank)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (ExcelDataManager.Instance.m_pGameMode.GetGameModeInfo((int)eGameMode.PvpStage).m_nAvailable_PlayerRank <= MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic.I_UserRank &&
                   m_pPvPStageInfo.m_nLimitGameCount > 0)
                {
                    return true;
                }

                if(ExcelDataManager.Instance.m_pGameMode.GetGameModeInfo((int)eGameMode.EventPvpStage).m_nAvailable_PlayerRank <= MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic.I_UserRank &&
                   m_pEventPvpStageInfo.m_nLimitGameCount > 0)
                {
                    return true;
                }
            }
        }

        for (int i = 0; i < m_pSpecialStageInfoList.Count; i++)
        {
            if (m_pSpecialStageInfoList[i].nModeID >= (int)eGameMode.DailyStage_Sun_Second && m_pSpecialStageInfoList[i].nModeID <= (int)eGameMode.DailyStage_Sat_Second)
            {
                for (int j = 0; j < m_pDailyStageInfoList_Second.Count; j++)
                {
                    if (m_pSpecialStageInfoList[i].nAvailablePlayerRank <= MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic.I_UserRank &&
                        m_pDailyStageInfoList_Second[j].nFreeClearCount < m_pDailyStageInfoList_Second[j].nPassCountFree && m_pDailyStageInfoList_Second[j].nModeID != (int)eGameMode.DailyStage_Sun_Second && m_pDailyStageInfoList_Second[j].nModeID != (int)eGameMode.DailyStage_Sat_Second)
                    {
                        return true;
                    }
                }
            }
        }

        for (int i = 0; i < m_pEventStageInfoList.Count; i++)
        {
            ExcelData_StageInfo pInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(m_pEventStageInfoList[i].m_nStageID);

            for (int j = 0; j < m_pSpecialStageInfoList.Count; j++)
            {
                if (pInfo.m_nRequiredEnergy <= InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemInfo_byTableID((int)eItemType.Energy).m_nItemCount &&
                m_pSpecialStageInfoList[j].nAvailablePlayerRank <= MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic.I_UserRank && IsUnlock_EventStage())
                {
                    return true;
                }
            }
        }

        

        //if(m_pPvPStageInfo.m_nLimitGameCount > 0)
        //{
        //    return true;
        //}

        return false;
    }

    public PvPPlayerInfo GetPvPPlayerInfo()
    {
        return m_pPvPPlayerInfo;
    }

    public void SetPvPSeqNumber(int nSeqNumber)
    {
        m_nPvPSeq = nSeqNumber;
    }

    public int GetPvPSeqNumber()
    {
        return m_nPvPSeq;
    }
}
