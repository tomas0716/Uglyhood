using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using sg.protocol.basic;
using sg.protocol.common;

public class SummonInfo
{
    public int nGroupID;
    public int nProductID;
    public int nPriceID;
    public int nPaymentType;
    public int nPaymentID;
    public int nPaymentCount;
    public bool IsBadge;
    public int nRotationType;
    public int nStartDay;
    public int nEndDay;
    public int nStartTime;
    public int nEndTime;
}

public class SummonShopInfo
{
    private Dictionary<int, SummonInfo> m_pSummonShopInfoTable_byPriceID = new Dictionary<int, SummonInfo>();
    private Dictionary<int, SummonInfo> m_pSummonShopInfoTable_byGroupID = new Dictionary<int, SummonInfo>();

    private NetComponent_GetSummonShopList m_pNetComponent_GetSummonShopList = new NetComponent_GetSummonShopList();

    public SummonShopInfo()
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

    public void SummonShopInfoTableClear()
    {
        m_pSummonShopInfoTable_byPriceID.Clear();
    }

    public void InitSummonInfo(Dictionary<int, SummonShopList> pTable)
    {
        Dictionary<int, int> pEndTimeTable = new Dictionary<int, int>();

        foreach(KeyValuePair<int, SummonShopList> pSummonInfo in pTable)
        {
            SummonInfo pInfo = new SummonInfo();

            pInfo.nGroupID = pSummonInfo.Value.I_GroupId;
            pInfo.nProductID = pSummonInfo.Value.I_ProductId;
            pInfo.nPriceID = pSummonInfo.Value.I_PriceId;
            pInfo.nPaymentType = pSummonInfo.Value.I_PaymentType;
            pInfo.nPaymentID = pSummonInfo.Value.I_PaymentId;
            pInfo.nPaymentCount = pSummonInfo.Value.I_PaymentCount;
            
            if(pSummonInfo.Value.I_IsBadge == 0)
            {
                pInfo.IsBadge = false;
            }
            else
            {
                pInfo.IsBadge = true;
            }

            pInfo.nRotationType = pSummonInfo.Value.I_RotationType;
            pInfo.nStartDay = pSummonInfo.Value.I_StartDay;
            pInfo.nEndDay = pSummonInfo.Value.I_EndDay;
            pInfo.nStartTime = pSummonInfo.Value.I_StartTime;
            pInfo.nEndTime = pSummonInfo.Value.I_EndTime;

            m_pSummonShopInfoTable_byPriceID.Add(pInfo.nPriceID, pInfo);

            if (pInfo.nRotationType == 1)
            {
                if (!pEndTimeTable.ContainsKey(pInfo.nGroupID))
                {
                    // 요일 게시
                    // 수정 필요?
                    DateTime nowDT = DateTime.Now;

                    int nRemainDay = ((int)pInfo.nEndDay + 1) - ((int)nowDT.DayOfWeek + 1);

                    if (nRemainDay < 0)
                    {
                        nRemainDay += 7;
                    }

                    int nRemainTime = (int)DateTime.Today.AddDays(nRemainDay).Subtract(DateTime.Now).TotalSeconds;

                    pEndTimeTable.Add(pInfo.nProductID, nRemainTime);
                }
            }

            if (pInfo.nRotationType == 2)
            {
                // 특정 기간 게시
                if (!pEndTimeTable.ContainsKey(pInfo.nGroupID))
                {
                    int nTimeStamp = pInfo.nEndTime;
                    DateTime dateTime = new DateTime(1970, 1, 1, 9, 0, 0, 0);
                    dateTime = dateTime.AddSeconds(nTimeStamp);

                    int nRemainTime = pInfo.nEndTime - (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

                    pEndTimeTable.Add(pInfo.nProductID, nRemainTime);
                }
            }
        }

        SetTimeInfo(pEndTimeTable);

        ExcelDataManager.Instance.m_pCharacterSummon_ProductGroup.CheckSummonListStartTime();
    }

    private void SetTimeInfo(Dictionary<int,int> pEndTimeTable)
    {
        foreach(KeyValuePair<int, int> pInfo in pEndTimeTable)
        {
            //TimeInfo.Instance.SetTimer_ForSummonList(pInfo.Key, pInfo.Value + 3);
            TimerManager.Instance.AddTimer(eTimerType.eSummonList, pInfo.Value, 0, pInfo.Key);
        }

        //TimeInfo.Instance.SetRemainTimer_ForSummerList();
    }

    private void OnTimer_Complete(Timer pTimer, eTimerType eType, object parameter)
    {
        if(eType == eTimerType.eSummonList)
        {
            TEventDelegate<EventArg_Null> pSuccessEvent = new TEventDelegate<EventArg_Null>();
            TEventDelegate<Error> pFailureEvent = new TEventDelegate<Error>();

            pSuccessEvent.SetFunc(OnGetSummonShopListSuccess);
            pFailureEvent.SetFunc(OnGetSummonShopListFailure);

            m_pNetComponent_GetSummonShopList.SetSuccessEvent(pSuccessEvent);
            m_pNetComponent_GetSummonShopList.SetFailureEvent(pFailureEvent);

            EventArg_Null pArg = new EventArg_Null();

            m_pNetComponent_GetSummonShopList.OnEvent(pArg);

            AppInstance.Instance.m_pEventDelegateManager.OnCreateLoadingResponse();
        }
    }

    private void OnGetSummonShopListSuccess(EventArg_Null Arg)
    {
        EventDelegateManager.Instance.OnLobby_RefreshGachaList();

        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    private void OnGetSummonShopListFailure(Error error)
    {
        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    public SummonInfo GetSummonShopInfo_byPriceID(int nPriceID)
    {
        if (m_pSummonShopInfoTable_byPriceID.ContainsKey(nPriceID))
            return m_pSummonShopInfoTable_byPriceID[nPriceID];

        return null;
    }

    public List<int> GetSummonShopProductIDList()
    {
        List<int> pProductIDList = new List<int>();

        foreach(KeyValuePair<int,SummonInfo> pInfo in m_pSummonShopInfoTable_byPriceID)
        {
            if (!pProductIDList.Contains(pInfo.Value.nProductID))
            {
                pProductIDList.Add(pInfo.Value.nProductID);
            }
        }

        return pProductIDList;
    }

    public List<int> GetSummonShopPriceIDList_byProductID(int nProductID)
    {
        List<int> pPriceIDList = new List<int>();

        foreach(KeyValuePair<int, SummonInfo> pInfo in m_pSummonShopInfoTable_byPriceID)
        {
            if(pInfo.Value.nProductID == nProductID)
            {
                pPriceIDList.Add(pInfo.Value.nPriceID);
            }
        }

        return pPriceIDList;
    }

    public SummonInfo GetSummonShopInfo_byGroupID(int nGroupID)
    {
        if (m_pSummonShopInfoTable_byGroupID.ContainsKey(nGroupID))
            return m_pSummonShopInfoTable_byGroupID[nGroupID];

        return null;
    }

    public bool GetIsBadgeOn_byProductID(int nProductID)
    {
        foreach(KeyValuePair<int, SummonInfo> pInfo in m_pSummonShopInfoTable_byPriceID)
        {
            if(pInfo.Value.nProductID == nProductID && pInfo.Value.IsBadge == true)
            {
                return true;
            }
        }

        return false;
    }
}
