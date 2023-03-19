using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum eTimerType
{
    eNone,
    eLoginAfterTime,
    eBattlePass,
    eBattlePassStartCheck,

    eCyclePackage_Week,
    eCyclePackage_Month,
    eCharacterPackage,

    eEventLimitPackage,

    eToastMessage,
    eOfflineReward,
    eOfflineRewardClaim,
    eAttendance,
    ePost,

    eEnergy,
    eSummonList,        // 파라미터, 한번의 리셋
    eDailyDungeon,      // 파라미터, 한번의 리셋
    eDailyStore,
    eDailyStoreRefresh,

    eEventStage,
    eEventPvpStage,

    eGameModeStartCheck,

    ePurchase,
}

public class TimerManager : Singleton<TimerManager>
{
    private Dictionary<eTimerType, Timer>   m_TimerTable                    = new Dictionary<eTimerType, Timer>();
    private Dictionary<int, Timer>          m_TimerTable_ForParameter       = new Dictionary<int, Timer>();

    private Dictionary<eTimerType, Timer>   m_RemoveTimerTable              = new Dictionary<eTimerType, Timer>();
    private Dictionary<object, Timer>       m_RemoveTimerTable_byParameter  = new Dictionary<object, Timer>();
    // 파라미터 에러 발생

    private TimerManager()
    {
    }

    public void Update()
    {
        foreach (KeyValuePair<eTimerType, Timer> pRemove in m_RemoveTimerTable)
        {
            if (m_TimerTable.ContainsKey(pRemove.Key) == true)
            {
                m_TimerTable.Remove(pRemove.Key);
            }

            //if (pRemove.Value.GetParameter() != null && m_TimerTable_ForParameter.ContainsKey((int)pRemove.Value.GetParameter()))
            //{
            //    m_TimerTable_ForParameter.Remove((int)pRemove.Value.GetParameter());
            //}
        }

        foreach(KeyValuePair<object,Timer> pRemove in m_RemoveTimerTable_byParameter)
        {
            if (pRemove.Value.GetParameter() != null && m_TimerTable_ForParameter.ContainsKey((int)pRemove.Value.GetParameter()))
            {
                m_TimerTable_ForParameter.Remove((int)pRemove.Value.GetParameter());
            }
        }

        m_RemoveTimerTable.Clear();
        m_RemoveTimerTable_byParameter.Clear();

        foreach (KeyValuePair<eTimerType, Timer> item in m_TimerTable)
        {
            item.Value.Update();
        }

        foreach(KeyValuePair<int,Timer> item in m_TimerTable_ForParameter)
        {
            item.Value.Update();
        }
    }

    public void AddTimer(eTimerType eType, float fTime, float fInterval, bool IsCompleteToDestroy = true)
    {
        OutputLog.Log("TimerManager : AddTime - Type : " + eType.ToString() + " Time : " + ((int)fTime).ToString());

        if (m_TimerTable.ContainsKey(eType) == true)
        {
            m_TimerTable.Remove(eType);
        }

        if (m_RemoveTimerTable.ContainsKey(eType) == true)
        {
            m_RemoveTimerTable.Remove(eType);
        }

        Timer pTimer = new Timer(this, eType, fTime, fInterval, null, IsCompleteToDestroy);
        m_TimerTable.Add(eType, pTimer);
    }

    public void AddTimer(eTimerType eType, float fTime, float fInterval, object parameter, bool IsCompleteToDestroy = true)
    {
        if(parameter == null)
        {
            if (m_TimerTable.ContainsKey(eType) == true)
            {
                m_TimerTable.Remove(eType);
            }

            if (m_RemoveTimerTable.ContainsKey(eType) == true)
            {
                m_RemoveTimerTable.Remove(eType);
            }

            Timer pTimer = new Timer(this, eType, fTime, fInterval, parameter, IsCompleteToDestroy);
            m_TimerTable.Add(eType, pTimer);
        }
        else
        {
            if (m_TimerTable_ForParameter.ContainsKey((int)parameter))
            {
                m_TimerTable_ForParameter.Remove((int)parameter);
            }

            if (m_RemoveTimerTable.ContainsKey(eType) == true)
            {
                m_RemoveTimerTable.Remove(eType);
            }

            if (m_RemoveTimerTable_byParameter.ContainsKey(parameter) == true)
            {
                m_RemoveTimerTable_byParameter.Remove(parameter);
            }

            Timer pTimer = new Timer(this, eType, fTime, fInterval, parameter, IsCompleteToDestroy);
            m_TimerTable_ForParameter.Add((int)parameter, pTimer);
        }
    }

    public Timer GetTimer(eTimerType eType, object parameter = null)
    {
        if(parameter == null)
        {
            if (m_TimerTable.ContainsKey(eType) == true)
            {
                return m_TimerTable[eType];
            }
        }
        else
        {
            if (m_TimerTable_ForParameter.ContainsKey((int)parameter))
            {
                return m_TimerTable_ForParameter[(int)parameter];
            }
        }

        return null;
    }

    public void OnDone_Timer(Timer pTimer)
    {

        if(pTimer.GetParameter() == null)
        {
            Debug.Log("타이머 종료 >> " + pTimer.GetTimerType());
            if (!m_RemoveTimerTable.ContainsKey(pTimer.GetTimerType()))
            {
                m_RemoveTimerTable.Add(pTimer.GetTimerType(), pTimer);
            }
        }
        else
        {
            Debug.Log("타이머 종료 >> " + pTimer.GetTimerType() + " 파라미터 >> " + pTimer.GetParameter());
            if (!m_RemoveTimerTable_byParameter.ContainsKey(pTimer.GetParameter()))
            {
                m_RemoveTimerTable_byParameter.Add(pTimer.GetParameter(), pTimer);
            }
        }

        //m_RemoveTimerTable.Add(pTimer.GetTimerType(), pTimer);
        // 12시 리프레시 안됨
        // ArgumentException: An item with the same key has already been added. Key: eDailyStore
    }

    public DateTime GetServerTime_Now()
    {
        DateTime dtServerTime = EspressoInfo.Instance.m_pConnectDateTime.AddSeconds(TimerManager.Instance.GetTimer(eTimerType.eLoginAfterTime).GetElapsedTime());

        return dtServerTime;
    }
    
    public DateTime GetServerTime_Today()
    {
        DateTime dtServerTime = TimerManager.Instance.GetServerTime_Now();

        DateTime dtToday = dtServerTime.AddHours(-1 * dtServerTime.Hour).AddMinutes(-1 * dtServerTime.Minute).AddSeconds(-1 * dtServerTime.Second);

        return dtToday;
    }
}