using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using sg.protocol.basic;

public class OfflineRewardInvenInfo
{
    public int m_nRewardID = 0;
    public int m_nRewardCount = 0;
}

public class OfflineRewardInfo
{
    private int m_nAccrueTime, m_nFastAdLimit, m_nFastGemLimit, m_nNextStartTime = 0;
    private int m_nRewardGold = 0;
    private List<OfflineRewardInvenInfo> m_pRewardInfoList = new List<OfflineRewardInvenInfo>();
    private Dictionary<int, OfflineRewardInvenInfo> m_pRewardInfoTable_Seq = new Dictionary<int, OfflineRewardInvenInfo>();

    private NetComponent_UserInfoOfflineReward m_pNetComponent_UserInfoOfflineReward = new NetComponent_UserInfoOfflineReward();

    public void Init()
    {
        EventDelegateManager.Instance.OnEventTimer_Complete += OnTimer_Complete;
    }

    public void OnDestroy()
    {
        EventDelegateManager.Instance.OnEventTimer_Complete -= OnTimer_Complete;
    }

    public void ClearInfo()
    {
        m_nAccrueTime = 0;
        m_nFastAdLimit = 0;
        m_nFastGemLimit = 0;
        m_nNextStartTime = 0;
        m_nRewardGold = 0;
        m_pRewardInfoList.Clear();
        m_pRewardInfoTable_Seq.Clear();
    }

    public void SetTimeInfo(int nAccureTime, int nFastAdLimit, int nFastGemLimit, int nNextStartTime)
    {
        m_nAccrueTime = nAccureTime;
        m_nFastAdLimit = nFastAdLimit;
        m_nFastGemLimit = nFastGemLimit;
        m_nNextStartTime = nNextStartTime;

        int nRemainTime = 9 * 60 * 60 - nAccureTime;

        Debug.Log("오프라인 남은 시간 : " + nRemainTime);

        //TimeInfo.Instance.SetTimer_ForOfflineRewardRefresh(nRemainTime, m_nAccrueTime);
        if (nRemainTime > 0)
        {
            TimerManager.Instance.AddTimer(eTimerType.eOfflineReward, nRemainTime, 0);
            TimerManager.Instance.GetTimer(eTimerType.eOfflineReward).SetStartTime(nAccureTime);

            //NotificationManager.Instance.SetNotification(eNotificationType.eOffline, (float)nRemainTime);
        }

        if (nNextStartTime != 0)
        {
            //TimeInfo.Instance.SetTimer_ForOfflineClaimRefresh(nNextStartTime);

            TimerManager.Instance.AddTimer(eTimerType.eOfflineRewardClaim, nNextStartTime, 0);
        }

    }

    private void OnTimer_Complete(Timer pTimer, eTimerType eType, object parameter)
    {
        if (eType == eTimerType.eOfflineReward)
        {

        }

        if (eType == eTimerType.eOfflineRewardClaim)
        {

        }
    }

    public void SetRewardGold(int nRewardGold)
    {
        m_nRewardGold = nRewardGold;
    }

    public void SetRewardInfo(int nRewardID, int nRewardCount, int nSeq)
    {
        if(nRewardID != 0)
        {
            OfflineRewardInvenInfo pInfo = new OfflineRewardInvenInfo();
            pInfo.m_nRewardID = nRewardID;
            pInfo.m_nRewardCount = nRewardCount;

            Debug.Log(nSeq);

            m_pRewardInfoList.Add(pInfo);
            m_pRewardInfoTable_Seq.Add(nSeq, pInfo);
        }
    }

    public List<OfflineRewardInvenInfo> GetRewardInvenInfoList()
    {
        List<OfflineRewardInvenInfo> pRewardList = new List<OfflineRewardInvenInfo>();

        for(int i=0; i<m_pRewardInfoList.Count; i++)
        {
            pRewardList.Add(m_pRewardInfoList[i]);
        }

        return pRewardList;
    }

    public OfflineRewardInvenInfo GetRewardItemCount_byID(int nSeq)
    {
        if (m_pRewardInfoTable_Seq.ContainsKey(nSeq)) 
        {
            return m_pRewardInfoTable_Seq[nSeq];
        }

        return null;
    }

    public int GetRewardGoldCount()
    {
        return m_nRewardGold;
    }

    public int GetClaimTime()
    {
        return m_nNextStartTime;
    }

    public bool IsFullOfflineReward()
    {
        if(m_nAccrueTime == 9 * 60 * 60)
        {
            return true;
        }

        return false;
    }

    public bool IsAdvancedClaimAD()
    {
        if(m_nFastAdLimit != 0)
        {
            return true;
        }

        return false;
    }

    public bool IsRemainAdvancedClaim()
    {
        if(m_nFastGemLimit != 0 || m_nFastAdLimit != 0)
        {
            return true;
        }

        return false;
    }

    public int GetAdvancedClaimCount()
    {
        return m_nFastGemLimit;
    }
}
