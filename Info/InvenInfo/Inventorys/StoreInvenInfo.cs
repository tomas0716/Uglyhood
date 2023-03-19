using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using sg.protocol.basic;
using sg.protocol.common;

public class BasicStoreInfo
{
    public int m_nID;
    public eShopPaymentType m_ePaymentType;
    public string m_strPaymentID;
    public int m_nPaymentQty;
    public eShopProductType m_eProductType;
    public int m_nProductID;
    public int m_nProductQty;

    public eItemType m_eBonusProductType = 0;
    public int m_nBonusProductID = 0;
    public int m_nBonusProductQty = 0;
    public int m_nPurchaseLimit = 0;
}

public class DailyStoreInfo
{
    public int m_nID;
    public eShopPaymentType m_ePaymentType;
    public string m_strPaymentID;
    public int m_nPaymentQty;
    public eShopProductType m_eProductType;
    public int m_nProductID;
    public int m_nProductQty;
    public bool m_IsBuy;
}

public class PackageStoreInfo
{
    public ePackageType m_ePackageKind;
    public int m_nID;
    public eShopPaymentType m_ePaymentType;
    public string m_strPaymentID;
    public int m_nPurchaseLimit;
    public int m_nRequiredChapter;
    public int m_nCycleType;
    public int m_nRankStart;
    public int m_nRankEnd;
    public int m_nRequiredUnit;
    public int m_nExpiretimeHour;
}

public class PackageGoodsInfo
{
    public int m_nID;
    public int m_nProductType;
    public int m_nProductID;
    public int m_nProductCount;
}

public class StoreInvenInfo
{
    private Dictionary<int, BasicStoreInfo> m_pBasicStoreInfoTable = new Dictionary<int, BasicStoreInfo>();
    private Dictionary<int, DailyStoreInfo> m_pDailyStoreInfoTable = new Dictionary<int, DailyStoreInfo>();
    private int m_nDailyStore_RefreshCount = 0;

    private DateTime m_dtDailyStoreRefreshTime = new DateTime();

    private Dictionary<int, PackageStoreInfo> m_pChapterInfoTable = new Dictionary<int, PackageStoreInfo>();
    private Dictionary<int, PackageStoreInfo> m_pCycleInfoTable = new Dictionary<int, PackageStoreInfo>();
    private Dictionary<int, PackageStoreInfo> m_pRankInfoTable = new Dictionary<int, PackageStoreInfo>();

    private Dictionary<int, PackageStoreInfo> m_pGrowthInfoTable = new Dictionary<int, PackageStoreInfo>();
    private Dictionary<int, PackageStoreInfo> m_pActiveCharacterInfoTable = new Dictionary<int, PackageStoreInfo>();

    private Dictionary<int, PackageStoreInfo> m_pEventLimitInfoTable = new Dictionary<int, PackageStoreInfo>();

    private Dictionary<int, List<PackageGoodsInfo>> m_pPackageGoodsInfo = new Dictionary<int, List<PackageGoodsInfo>>();

    public StoreInvenInfo()
    {
        Debug.Log("상점 생성자~");
    }

    public void Init()
    {
        EventDelegateManager.Instance.OnEventTimer_Complete += OnTimer_Complete_Store;
    }
    
    public void OnDestroy()
    {
        EventDelegateManager.Instance.OnEventTimer_Complete -= OnTimer_Complete_Store;
    }

    public void ClearBasicStoreInfoTable()
    {
        m_pBasicStoreInfoTable.Clear();
    }

    public void ClearDailyStoreInfoTable()
    {
        m_pDailyStoreInfoTable.Clear();
    }

    public void AddBasicStoreInfo(BasicStoreInfo pBasicStoreInfo)
    {
        m_pBasicStoreInfoTable.Add(pBasicStoreInfo.m_nID, pBasicStoreInfo);
    }

    public void AddDailyStoreInfo(DailyStoreInfo pDailyStoreInfo)
    {
        m_pDailyStoreInfoTable.Add(pDailyStoreInfo.m_nID, pDailyStoreInfo);

        //TimeInfo.Instance.SetTimer_ForDailyStore(nRemainTime);
        //TimerManager.Instance.AddTimer(eTimerType.eDailyStore, nRemainTime, 0);
    }

    public void SetTimer_ForDailyStore()
    {
        DateTime nowDT = DateTime.Now;

        int nRemainTime = (int)DateTime.Today.AddDays(1).Subtract(DateTime.Now).TotalSeconds;

        TimerManager.Instance.AddTimer(eTimerType.eDailyStore, nRemainTime, 0);
    }

    public void ClearChapterInfoTable()
    {
        m_pChapterInfoTable.Clear();
    }

    public void ClearCycleInfoTable()
    {
        m_pCycleInfoTable.Clear();
    }

    public void ClearRankInfoTable()
    {
        m_pRankInfoTable.Clear();
    }

    public void ClearGrowthInfoTable()
    {
        m_pGrowthInfoTable.Clear();
    }

    public void ClearActiveCharacterInfoTable()
    {
        m_pActiveCharacterInfoTable.Clear();
    }

    public void ClearEventLimitInfoTable()
    {
        m_pEventLimitInfoTable.Clear();
    }

    public void AddChapterInfoTable(PackageStoreInfo pPackageInfo)
    {
        m_pChapterInfoTable.Add(pPackageInfo.m_nID, pPackageInfo);
    }

    public void AddCycleInfoTable(PackageStoreInfo pPackageInfo)
    {
        m_pCycleInfoTable.Add(pPackageInfo.m_nID, pPackageInfo);

        // 시간 찾기

        System.Globalization.CultureInfo ciCurrent = System.Threading.Thread.CurrentThread.CurrentCulture;

        if (TimerManager.Instance.GetTimer(eTimerType.eCyclePackage_Week) == null)
        {
            DateTime dtToday = TimerManager.Instance.GetServerTime_Today();

            DayOfWeek dwFirst = ciCurrent.DateTimeFormat.FirstDayOfWeek;
            DayOfWeek dwToday = ciCurrent.Calendar.GetDayOfWeek(dtToday);

            TimeSpan tsWeek;

            if (dwToday == DayOfWeek.Sunday)
            {
                tsWeek = dtToday.AddDays(1) - TimerManager.Instance.GetServerTime_Now();
            }
            else
            {
                int iDiff = dwToday - dwFirst;
                DateTime dtFirstDayOfThisWeek = dtToday.AddDays(-iDiff + 1);
                DateTime dtLastDayOfThisWeek = dtFirstDayOfThisWeek.AddDays(7);

                tsWeek = dtLastDayOfThisWeek - TimerManager.Instance.GetServerTime_Now();
            }

            //EventDelegateManager.Instance.OnEventTimer_Complete += OnTimer_Complete_Week;
            TimerManager.Instance.AddTimer(eTimerType.eCyclePackage_Week, (float)tsWeek.TotalSeconds, 0);
        }

        if (TimerManager.Instance.GetTimer(eTimerType.eCyclePackage_Month) == null)
        {
            DateTime dtMonthFirstDay = TimerManager.Instance.GetServerTime_Today().AddDays(1 - TimerManager.Instance.GetServerTime_Today().Day);
            DateTime dtMonth = dtMonthFirstDay.AddMonths(1);

            TimeSpan tsMonth = dtMonth - TimerManager.Instance.GetServerTime_Now();

            //EventDelegateManager.Instance.OnEventTimer_Complete += OnTimer_Complete_Month;
            TimerManager.Instance.AddTimer(eTimerType.eCyclePackage_Month, (float)tsMonth.TotalSeconds, 0);
        }
    }

    //private void OnTimer_Complete_Week(Timer pTimer, eTimerType eType, object parameter)
    //{
    //    if(eType == eTimerType.eCyclePackage_Week)
    //    {
    //        EventDelegateManager.Instance.OnEventTimer_Complete -= OnTimer_Complete_Week;

    //        NetComponent_GetShopPackageProduct pNetComponent_GetShopPackageProduct = new NetComponent_GetShopPackageProduct();
    //        TEventDelegate<EventArg_Null> pSuccessEvent = new TEventDelegate<EventArg_Null>();
    //        TEventDelegate<Error> pFailureEvent = new TEventDelegate<Error>();
    //        pSuccessEvent.SetFunc(OnGetCyclePackageSuccess);
    //        pFailureEvent.SetFunc(OnGetCyclePackageFailure);

    //        pNetComponent_GetShopPackageProduct.SetSuccessEvent(pSuccessEvent);
    //        pNetComponent_GetShopPackageProduct.SetFailureEvent(pFailureEvent);

    //        EventArg_GetShopPackageProduct pArg = new EventArg_GetShopPackageProduct();
    //        pArg.m_nPackageKind = (int)ePackageType.Cycle;

    //        pNetComponent_GetShopPackageProduct.OnEvent(pArg);

    //        AppInstance.Instance.m_pEventDelegateManager.OnCreateLoadingResponse();
    //    }
    //}

    //private void OnTimer_Complete_Month(Timer pTimer, eTimerType eType, object parameter)
    //{
    //    if (eType == eTimerType.eCyclePackage_Month)
    //    {
    //        EventDelegateManager.Instance.OnEventTimer_Complete -= OnTimer_Complete_Month;

    //        NetComponent_GetShopPackageProduct pNetComponent_GetShopPackageProduct = new NetComponent_GetShopPackageProduct();
    //        TEventDelegate<EventArg_Null> pSuccessEvent = new TEventDelegate<EventArg_Null>();
    //        TEventDelegate<Error> pFailureEvent = new TEventDelegate<Error>();
    //        pSuccessEvent.SetFunc(OnGetCyclePackageSuccess);
    //        pFailureEvent.SetFunc(OnGetCyclePackageFailure);

    //        pNetComponent_GetShopPackageProduct.SetSuccessEvent(pSuccessEvent);
    //        pNetComponent_GetShopPackageProduct.SetFailureEvent(pFailureEvent);

    //        EventArg_GetShopPackageProduct pArg = new EventArg_GetShopPackageProduct();
    //        pArg.m_nPackageKind = (int)ePackageType.Cycle;

    //        pNetComponent_GetShopPackageProduct.OnEvent(pArg);

    //        AppInstance.Instance.m_pEventDelegateManager.OnCreateLoadingResponse();
    //    }
    //}

    private void OnGetCyclePackageSuccess(EventArg_Null Arg)
    {
        // Cycle 패키지 리프레시
        EventDelegateManager.Instance.OnPackage_RefreshCyclePackage();

        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    private void OnGetCyclePackageFailure(Error error)
    {
        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    public void AddRankInfoTable(PackageStoreInfo pPackageInfo)
    {
        m_pRankInfoTable.Add(pPackageInfo.m_nID, pPackageInfo);
    }

    public void AddGrowthInfoTable(PackageStoreInfo pPackageInfo)
    {
        m_pGrowthInfoTable.Add(pPackageInfo.m_nID, pPackageInfo);

        // 유닛 얻은 시간과 패키지 테이블 비교후 타이머 추가
    }

    public void AddEventLimitInfoTable(PackageStoreInfo pPackageInfo)
    {
        m_pEventLimitInfoTable.Add(pPackageInfo.m_nID, pPackageInfo);

        //ExcelData_Shop_PackageInfo pInfo = ExcelDataManager.Instance.m_pShop_Package.GetPackageInfo_byID(pPackageInfo.m_nID);

        //DateTime dtNow = DateTime.Now;

        //if(pInfo.m_dtScheduleTime_Start <= dtNow && pInfo.m_dtScheduleTime_End >= dtNow)
        //{
        //    TimeSpan tsRemainTime = pInfo.m_dtScheduleTime_End - dtNow;

        //    if (TimerManager.Instance.GetTimer(eTimerType.eEventLimitPackage) == null)
        //    {
        //        TimerManager.Instance.AddTimer(eTimerType.eEventLimitPackage, (float)tsRemainTime.TotalSeconds, 0);
        //    }
        //}

        //Debug.Log("이벤트 패키지 카운트 >> " + m_pEventLimitInfoTable.Count);
    }

    private void OnGetEventLimitPackageSuccess(EventArg_Null Arg)
    {
        // 사이드 메뉴 이벤트 제외
        EventDelegateManager.Instance.OnSideEvent_RefreshPackage();
        // 화면 켜져있는 상태일때 끄기
        EventDelegateManager.Instance.OnPackage_RefreshEventLimitPackage();


        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    private void OnGetEventLimitPackageFailure(Error error)
    {
        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    public void CharacterInfoTimeCheck()
    {
        //CoroutineHelper.StartCoroutine(WaitForCharacterInfoTimeCheck());
        ClearActiveCharacterInfoTable();

        foreach (KeyValuePair<int, PackageStoreInfo> pInfo in m_pGrowthInfoTable)
        {
            ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(pInfo.Value.m_nRequiredUnit);
            ExcelData_Shop_PackageInfo pPackageInfo = ExcelDataManager.Instance.m_pShop_Package.GetPackageInfo_byID(pInfo.Value.m_nID);
            CharacterInvenItemInfo pCharacterInfo = InventoryInfoManager.Instance.m_pCharacterInvenInfo.GetInvenItem_byTableID(pInfo.Value.m_nRequiredUnit);

            if (pCharacterInfo != null && pUnitInfo != null && pPackageInfo != null)
            {
                int nTimeStamp = pPackageInfo.m_nExpireTime_Hour;

                DateTime dateTime = new DateTime(1970, 1, 1, 9, 0, 0, 0);
                dateTime = dateTime.AddSeconds(pCharacterInfo.m_nUnit_AddTime);

                DateTime dtFinish = dateTime.AddHours(nTimeStamp);

                TimeSpan tsActiveTime = dtFinish - DateTime.Now;

                if (tsActiveTime.TotalSeconds > 0)
                {
                    TimerManager.Instance.AddTimer(eTimerType.eCharacterPackage, (float)tsActiveTime.TotalSeconds, 0, pInfo.Value.m_nRequiredUnit);

                    m_pActiveCharacterInfoTable.Add(pInfo.Key, pInfo.Value);
                }
            }
        }
    }

    //IEnumerator WaitForCharacterInfoTimeCheck()
    //{
    //    yield return new WaitForEndOfFrame();

    //    ClearActiveCharacterInfoTable();

    //    try
    //    {
    //        foreach (KeyValuePair<int, PackageStoreInfo> pInfo in m_pGrowthInfoTable)
    //        {
    //            ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(pInfo.Value.m_nRequiredUnit);
    //            ExcelData_Shop_PackageInfo pPackageInfo = ExcelDataManager.Instance.m_pShop_Package.GetPackageInfo_byID(pInfo.Value.m_nID);
    //            CharacterInvenItemInfo pCharacterInfo = InventoryInfoManager.Instance.m_pCharacterInvenInfo.GetInvenItem_byTableID(pInfo.Value.m_nRequiredUnit);

    //            if (pCharacterInfo != null && pUnitInfo != null && pPackageInfo != null)
    //            {
    //                int nTimeStamp = pPackageInfo.m_nExpireTime_Hour;

    //                DateTime dateTime = new DateTime(1970, 1, 1, 9, 0, 0, 0);
    //                dateTime = dateTime.AddSeconds(pCharacterInfo.m_nUnit_AddTime);

    //                DateTime dtFinish = dateTime.AddHours(nTimeStamp);

    //                TimeSpan tsActiveTime = dtFinish - DateTime.Now;

    //                if (tsActiveTime.TotalSeconds > 0)
    //                {
    //                    TimerManager.Instance.AddTimer(eTimerType.eCharacterPackage, (float)tsActiveTime.TotalSeconds, 0, pInfo.Value.m_nRequiredUnit);

    //                    m_pActiveCharacterInfoTable.Add(pInfo.Key, pInfo.Value);
    //                }
    //            }
    //        }
    //    }
    //    catch (ArgumentException e)
    //    {
    //        Debug.Log("ArgumentException >> " + e.Message);
    //    }
    //    catch(IndexOutOfRangeException e)
    //    {
    //        Debug.Log("IndexOutOfRangeException >> " + e.Message);
    //    }
    //    catch(NullReferenceException e)
    //    {
    //        Debug.Log("NullRefrenceException >> " + e.Message);
    //    }
    //}

    private void OnTimer_Complete_Store(Timer pTimer, eTimerType eTimerType, object parameter)
    {
        if (eTimerType == eTimerType.eCyclePackage_Week)
        {
            NetComponent_GetShopPackageProduct pNetComponent_GetShopPackageProduct = new NetComponent_GetShopPackageProduct();
            TEventDelegate<EventArg_Null> pSuccessEvent = new TEventDelegate<EventArg_Null>();
            TEventDelegate<Error> pFailureEvent = new TEventDelegate<Error>();
            pSuccessEvent.SetFunc(OnGetCyclePackageSuccess);
            pFailureEvent.SetFunc(OnGetCyclePackageFailure);

            pNetComponent_GetShopPackageProduct.SetSuccessEvent(pSuccessEvent);
            pNetComponent_GetShopPackageProduct.SetFailureEvent(pFailureEvent);

            EventArg_GetShopPackageProduct pArg = new EventArg_GetShopPackageProduct();
            pArg.m_nPackageKind = (int)ePackageType.Cycle;

            pNetComponent_GetShopPackageProduct.OnEvent(pArg);

            AppInstance.Instance.m_pEventDelegateManager.OnCreateLoadingResponse();
        }

        if (eTimerType == eTimerType.eCyclePackage_Month)
        {
            NetComponent_GetShopPackageProduct pNetComponent_GetShopPackageProduct = new NetComponent_GetShopPackageProduct();
            TEventDelegate<EventArg_Null> pSuccessEvent = new TEventDelegate<EventArg_Null>();
            TEventDelegate<Error> pFailureEvent = new TEventDelegate<Error>();
            pSuccessEvent.SetFunc(OnGetCyclePackageSuccess);
            pFailureEvent.SetFunc(OnGetCyclePackageFailure);

            pNetComponent_GetShopPackageProduct.SetSuccessEvent(pSuccessEvent);
            pNetComponent_GetShopPackageProduct.SetFailureEvent(pFailureEvent);

            EventArg_GetShopPackageProduct pArg = new EventArg_GetShopPackageProduct();
            pArg.m_nPackageKind = (int)ePackageType.Cycle;

            pNetComponent_GetShopPackageProduct.OnEvent(pArg);

            AppInstance.Instance.m_pEventDelegateManager.OnCreateLoadingResponse();
        }

        if (eTimerType == eTimerType.eCharacterPackage)
        {
            NetComponent_GetShopPackageProduct pNetComponent_GetShopPackageProduct = new NetComponent_GetShopPackageProduct();
            TEventDelegate<EventArg_Null> pSuccessEvent = new TEventDelegate<EventArg_Null>();
            TEventDelegate<Error> pFailureEvent = new TEventDelegate<Error>();
            pSuccessEvent.SetFunc(OnGetCharacterPackageSuccess);
            pFailureEvent.SetFunc(OnGetCharacterPackageFailure);

            pNetComponent_GetShopPackageProduct.SetSuccessEvent(pSuccessEvent);
            pNetComponent_GetShopPackageProduct.SetFailureEvent(pFailureEvent);

            EventArg_GetShopPackageProduct pArg = new EventArg_GetShopPackageProduct();
            pArg.m_nPackageKind = (int)ePackageType.Growth;

            pNetComponent_GetShopPackageProduct.OnEvent(pArg);

            AppInstance.Instance.m_pEventDelegateManager.OnCreateLoadingResponse();
        }

        if(eTimerType == eTimerType.eDailyStore)
        {
            NetComponent_GetShopDailyProduct pNetComponent_GetShopDailyProduct = new NetComponent_GetShopDailyProduct();
            TEventDelegate<EventArg_Null> pSuccessEvent = new TEventDelegate<EventArg_Null>();
            TEventDelegate<Error> pFailureEvent = new TEventDelegate<Error>();

            pSuccessEvent.SetFunc(OnGetShopDailyProductSuccess);
            pFailureEvent.SetFunc(OnGetShopDailyProductFailure);

            pNetComponent_GetShopDailyProduct.SetSuccessEvent(pSuccessEvent);
            pNetComponent_GetShopDailyProduct.SetFailureEvent(pFailureEvent);

            EventArg_Null pArg = new EventArg_Null();

            pNetComponent_GetShopDailyProduct.OnEvent(pArg);

            AppInstance.Instance.m_pEventDelegateManager.OnCreateLoadingResponse();
        }

        if(eTimerType == eTimerType.eDailyStoreRefresh)
        {
            InventoryInfoManager.Instance.m_pStoreInvenInfo.SetDailyStoreRefreshRemainTime_byTimeInfo(DateTime.Now);

            EventDelegateManager.Instance.OnStore_RefreshDailyStoreButtonActive(true);
        }

        if (eTimerType == eTimerType.eEventLimitPackage)
        {
            NetComponent_GetShopPackageProduct pNetComponent_GetShopPackageProduct = new NetComponent_GetShopPackageProduct();
            TEventDelegate<EventArg_Null> pSuccessEvent = new TEventDelegate<EventArg_Null>();
            TEventDelegate<Error> pFailureEvent = new TEventDelegate<Error>();

            pSuccessEvent.SetFunc(OnGetEventLimitPackageSuccess);
            pFailureEvent.SetFunc(OnGetEventLimitPackageFailure);

            pNetComponent_GetShopPackageProduct.SetSuccessEvent(pSuccessEvent);
            pNetComponent_GetShopPackageProduct.SetFailureEvent(pFailureEvent);

            EventArg_GetShopPackageProduct pArg = new EventArg_GetShopPackageProduct();
            pArg.m_nPackageKind = (int)ePackageType.Limit;

            pNetComponent_GetShopPackageProduct.OnEvent(pArg);

            AppInstance.Instance.m_pEventDelegateManager.OnCreateLoadingResponse();
        }
    }

    private void OnGetCharacterPackageSuccess(EventArg_Null Arg)
    {
        //Refresh
        EventDelegateManager.Instance.OnSideEvent_RefreshPackage();
        EventDelegateManager.Instance.OnPackage_RefreshCharacterPackage();

        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    private void OnGetCharacterPackageFailure(Error error)
    {
        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    private void OnGetShopDailyProductSuccess(EventArg_Null Arg)
    {
        EventDelegateManager.Instance.OnStore_RefreshStoreList(true);

        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    private void OnGetShopDailyProductFailure(Error error)
    {
        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    public void AddPackageGoodsInfoTable(Dictionary<int, List<PackageGoodsInfo>> pInfo)
    {
        Dictionary<int, List<PackageGoodsInfo>> pInfoTable_byOrder = new Dictionary<int, List<PackageGoodsInfo>>();

        foreach (KeyValuePair<int, List<PackageGoodsInfo>> Info in pInfo)
        {
            List<PackageGoodsInfo> pInfoList_byOrder = new List<PackageGoodsInfo>();
            Dictionary<int, PackageGoodsInfo> pInfoTable = new Dictionary<int, PackageGoodsInfo>();

            List<ExcelData_Shop_Package_GoodsInfo> pGoodsInfoList = ExcelDataManager.Instance.m_pShop_Package_Goods.GetPackageGoodsInfoList_byID(Info.Key);
            List<int> nKeyList = new List<int>();

            for(int i=0; i<Info.Value.Count; i++)
            {
                for(int j=0; j<pGoodsInfoList.Count; j++)
                {
                    if(Info.Value[i].m_nProductType == (int)eShopProductType.Item)
                    {
                        if(Info.Value[i].m_nProductID == pGoodsInfoList[j].m_nProductID)
                        {
                            pInfoTable.Add(pGoodsInfoList[j].m_nOrder, Info.Value[i]);
                            nKeyList.Add(pGoodsInfoList[j].m_nOrder);
                            break;
                        }
                    }
                    else
                    {
                        if(Info.Value[i].m_nProductType == (int)pGoodsInfoList[j].m_eProductType)
                        {
                            pInfoTable.Add(pGoodsInfoList[j].m_nOrder, Info.Value[i]);
                            nKeyList.Add(pGoodsInfoList[j].m_nOrder);
                            break;
                        }
                    }
                }
            }

            nKeyList.Sort();

            foreach(int pKey in nKeyList)
            {
                if (pInfoTable.ContainsKey(pKey))
                {
                    pInfoList_byOrder.Add(pInfoTable[pKey]);
                }
            }

            //foreach(KeyValuePair<int,PackageGoodsInfo> pGoodsInfo in pInfoTable)
            //{
            //    pInfoList_byOrder.Add(pGoodsInfo.Value);
            //}

            pInfoTable_byOrder.Add(Info.Key, pInfoList_byOrder);
        }

        foreach (KeyValuePair<int, List<PackageGoodsInfo>> Info in pInfoTable_byOrder)
        {
            if (m_pPackageGoodsInfo.ContainsKey(Info.Key))
            {
                m_pPackageGoodsInfo.Remove(Info.Key);
                m_pPackageGoodsInfo.Add(Info.Key, Info.Value);
            }
            else
            {
                m_pPackageGoodsInfo.Add(Info.Key, Info.Value);
            }
        }
    }

    public PackageStoreInfo GetPackageStoreInfo_byID(int nID)
    {
        foreach(KeyValuePair<int,PackageStoreInfo> pInfo in m_pChapterInfoTable)
        {
            if(pInfo.Value.m_nID == nID)
            {
                return pInfo.Value;
            }
        }

        foreach (KeyValuePair<int, PackageStoreInfo> pInfo in m_pCycleInfoTable)
        {
            if (pInfo.Value.m_nID == nID)
            {
                return pInfo.Value;
            }
        }

        foreach (KeyValuePair<int, PackageStoreInfo> pInfo in m_pRankInfoTable)
        {
            if (pInfo.Value.m_nID == nID)
            {
                return pInfo.Value;
            }
        }

        foreach (KeyValuePair<int, PackageStoreInfo> pInfo in m_pActiveCharacterInfoTable)
        {
            if (pInfo.Value.m_nID == nID)
            {
                return pInfo.Value;
            }
        }

        foreach(KeyValuePair<int, PackageStoreInfo> pInfo in m_pEventLimitInfoTable)
        {
            if(pInfo.Value.m_nID == nID)
            {
                return pInfo.Value;
            }
        }

        return null;
    }

    public void SetDailyStore_RefreshCount(int nRefreshCount)
    {
        m_nDailyStore_RefreshCount = nRefreshCount;
    }

    public Dictionary<int, BasicStoreInfo> GetBasicStoreInfoTable()
    {
        return m_pBasicStoreInfoTable;
    }

    public Dictionary<int, DailyStoreInfo> GetDailyStoreInfoTable()
    {
        return m_pDailyStoreInfoTable;
    }

    public Dictionary<int, PackageStoreInfo> GetPackageStoreInfoTable_byPackageType(ePackageType ePackageType)
    {
        if (ePackageType == ePackageType.Chapter)
        {
            return m_pChapterInfoTable;
        }
        else if (ePackageType == ePackageType.Cycle)
        {
            return m_pCycleInfoTable;
        }
        else if (ePackageType == ePackageType.Rank)
        {
            return m_pRankInfoTable;
        }
        else
        {
            return m_pActiveCharacterInfoTable;
        }
    }

    public PackageStoreInfo GetRankPackagetStoreInfoTabel_byID(int nID)
    {
        if (m_pRankInfoTable.ContainsKey(nID))
        {
            return m_pRankInfoTable[nID];
        }

        return null;
    }

    public Dictionary<int, PackageStoreInfo> GetCyclePackageInfoTable_byCycle(bool IsWeekly)
    {
        Dictionary<int, PackageStoreInfo> pCycleTable = new Dictionary<int, PackageStoreInfo>();

        if (IsWeekly)
        {
            foreach(KeyValuePair<int,PackageStoreInfo> pInfo in m_pCycleInfoTable)
            {
                if(pInfo.Value.m_nCycleType == 1)
                {
                    pCycleTable.Add(pInfo.Key, pInfo.Value);
                }
            }
        }
        else
        {
            foreach (KeyValuePair<int, PackageStoreInfo> pInfo in m_pCycleInfoTable)
            {
                if (pInfo.Value.m_nCycleType == 2)
                {
                    pCycleTable.Add(pInfo.Key, pInfo.Value);
                }
            }
        }

        return pCycleTable;
    }

    public PackageStoreInfo GetRankPackageInfo_byOrder()
    {
        Dictionary<int, PackageStoreInfo> pPackageInfo = new Dictionary<int, PackageStoreInfo>();

        foreach(KeyValuePair<int,PackageStoreInfo> pInfo in m_pRankInfoTable)
        {
            if(pInfo.Value.m_nRankStart <= MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic.I_UserRank && pInfo.Value.m_nRankEnd >= MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic.I_UserRank)
            {
                ExcelData_Shop_PackageInfo pTableInfo = ExcelDataManager.Instance.m_pShop_Package.GetPackageInfo_byID(pInfo.Value.m_nID);

                pPackageInfo.Add(pTableInfo.m_nOrder, pInfo.Value);
            }
        }

        foreach(KeyValuePair<int,PackageStoreInfo> pInfo in pPackageInfo)
        {
            return pInfo.Value;
        }

        return null;
    }

    public Dictionary<int,PackageStoreInfo> GetCharacterPackageInfo()
    {
        return m_pActiveCharacterInfoTable;
    }

    public List<PackageStoreInfo> GetPackageInfo_byPaymentTypeIAP()
    {
        List<PackageStoreInfo> pStoreInfo = new List<PackageStoreInfo>();

        foreach(KeyValuePair<int,PackageStoreInfo> pInfo in m_pChapterInfoTable)
        {
            if(pInfo.Value.m_ePaymentType == eShopPaymentType.IAP)
            {
                pStoreInfo.Add(pInfo.Value);
            }
        }

        foreach (KeyValuePair<int, PackageStoreInfo> pInfo in m_pCycleInfoTable)
        {
            if (pInfo.Value.m_ePaymentType == eShopPaymentType.IAP)
            {
                pStoreInfo.Add(pInfo.Value);
            }
        }

        foreach (KeyValuePair<int, PackageStoreInfo> pInfo in m_pRankInfoTable)
        {
            if (pInfo.Value.m_ePaymentType == eShopPaymentType.IAP)
            {
                pStoreInfo.Add(pInfo.Value);
            }
        }

        foreach (KeyValuePair<int, PackageStoreInfo> pInfo in m_pGrowthInfoTable)
        {
            if (pInfo.Value.m_ePaymentType == eShopPaymentType.IAP)
            {
                pStoreInfo.Add(pInfo.Value);
            }
        }

        return pStoreInfo;
    }

    public Dictionary<int, PackageStoreInfo> GetEventLimitPackageInfo()
    {
        return m_pEventLimitInfoTable;
    }

    public List<PackageGoodsInfo> GetPackageGoodsList_byID(int nID)
    {
        return m_pPackageGoodsInfo[nID];
    }
    
    public int GetDailyStore_RefeshCount()
    {
        return m_nDailyStore_RefreshCount;
    }

    public List<BasicStoreInfo> GetBasicStoreInfoList_byCategory(eShopCategory pCategory)
    {
        Dictionary<int, BasicStoreInfo> pInfoTable = new Dictionary<int, BasicStoreInfo>();     //Key slot
        List<BasicStoreInfo> pInfoList = new List<BasicStoreInfo>();

        foreach (KeyValuePair<int, BasicStoreInfo> pInfo in m_pBasicStoreInfoTable)
        {
            ExcelData_Shop_BasicInfo pShopInfo = ExcelDataManager.Instance.m_pShop_Basic.GetShop_BasicInfo_byID(pInfo.Value.m_nID);

            if(pCategory == pShopInfo.m_eShopCategory)
            {
                if (!pInfoTable.ContainsKey(pShopInfo.m_nSlot))
                {
                    pInfoTable.Add(pShopInfo.m_nSlot, pInfo.Value);
                }
                else
                {
                    ExcelData_Shop_BasicInfo pCheckInfo = ExcelDataManager.Instance.m_pShop_Basic.GetShop_BasicInfo_byID(pInfoTable[pShopInfo.m_nSlot].m_nID);

                    if (pShopInfo.m_nOrder < pCheckInfo.m_nOrder)
                    {
                        pInfoTable.Remove(pShopInfo.m_nSlot);
                        pInfoTable.Add(pShopInfo.m_nSlot, pInfo.Value);
                    }
                }
            }
        }

        foreach (KeyValuePair<int, BasicStoreInfo> pInfo in pInfoTable)
        {
            pInfoList.Add(pInfo.Value);
        }

        return pInfoList;

        //List<BasicStoreInfo> pInfoList = new List<BasicStoreInfo>();

        //foreach (KeyValuePair<int,BasicStoreInfo> pInfo in m_pBasicStoreInfoTable)
        //{
        //    if(ExcelDataManager.Instance.m_pShop_Basic.GetShop_BasicCategoryInfo_byID(pInfo.Key) == pCategory)
        //    {
        //        pInfoList.Add(pInfo.Value);
        //    }
        //}

        //return pInfoList;
    }

    public List<DailyStoreInfo> GetDailyStoreInfoList()
    {
        List<DailyStoreInfo> pInfoList = new List<DailyStoreInfo>();

        foreach(KeyValuePair<int,DailyStoreInfo> pInfo in m_pDailyStoreInfoTable)
        {
            pInfoList.Add(pInfo.Value);
        }

        return pInfoList;
    }

    public BasicStoreInfo GetBasicStoreInfo_byID(int nID)
    {
        if (m_pBasicStoreInfoTable.ContainsKey(nID))
            return m_pBasicStoreInfoTable[nID];

        return null;
    }

    public List<BasicStoreInfo> GetBasicStoreInfo_GemProduct()
    {
        List<BasicStoreInfo> pInfoList = new List<BasicStoreInfo>();

        foreach(KeyValuePair<int,BasicStoreInfo> pInfo in m_pBasicStoreInfoTable)
        {
            if(pInfo.Value.m_ePaymentType == eShopPaymentType.IAP)
            {
                pInfoList.Add(pInfo.Value);
            }
        }

        return pInfoList;
    }

    public List<BasicStoreInfo> GetAllBasicStoreInfo()
    {
        List<BasicStoreInfo> pInfoList = new List<BasicStoreInfo>();

        foreach (KeyValuePair<int, BasicStoreInfo> pInfo in m_pBasicStoreInfoTable)
        {
            pInfoList.Add(pInfo.Value);
        }

        return pInfoList;
    }

    public List<BasicStoreInfo> GetBasicStoreInfo_byOrder()
    {
        Dictionary<int, BasicStoreInfo> pInfoTable = new Dictionary<int, BasicStoreInfo>();     //Key slot
        List<BasicStoreInfo> pInfoList = new List<BasicStoreInfo>();

        foreach(KeyValuePair<int, BasicStoreInfo> pInfo in m_pBasicStoreInfoTable)
        {
            ExcelData_Shop_BasicInfo pShopInfo = ExcelDataManager.Instance.m_pShop_Basic.GetShop_BasicInfo_byID(pInfo.Value.m_nID);

            if (!pInfoTable.ContainsKey(pShopInfo.m_nSlot))
            {
                pInfoTable.Add(pShopInfo.m_nSlot, pInfo.Value);
            }
            else
            {
                ExcelData_Shop_BasicInfo pCheckInfo = ExcelDataManager.Instance.m_pShop_Basic.GetShop_BasicInfo_byID(pInfoTable[pShopInfo.m_nSlot].m_nID);

                if(pShopInfo.m_nOrder < pCheckInfo.m_nOrder)
                {
                    pInfoTable.Remove(pShopInfo.m_nSlot);
                    pInfoTable.Add(pShopInfo.m_nSlot, pInfo.Value);
                }
            }
        }

        foreach(KeyValuePair<int,BasicStoreInfo> pInfo in pInfoTable)
        {
            pInfoList.Add(pInfo.Value);
        }

        return pInfoList;
    }

    public DailyStoreInfo GetDailyStoreInfo_byID(int nID)
    {
        if (m_pDailyStoreInfoTable.ContainsKey(nID))
            return m_pDailyStoreInfoTable[nID];

        return null;
    }

    public bool IsStorePageRedDotOn()
    {
        foreach(KeyValuePair<int, DailyStoreInfo> pInfo in m_pDailyStoreInfoTable)
        {
            ExcelData_Shop_DailyInfo pDailyInfo = ExcelDataManager.Instance.m_pShop_Daily.GetShop_DailyInfo_byID(pInfo.Value.m_nID);

            if (!pInfo.Value.m_IsBuy && pDailyInfo.m_IsNotification)
                return true;
        }

        if(m_nDailyStore_RefreshCount > 0)
        {
            DateTime dtNow = new DateTime();
            dtNow = DateTime.Now;

            if (m_dtDailyStoreRefreshTime < dtNow)
            {
                return true;
            }
        }

        if (IsChapterPackageRedDot() == true)
            return true;

        return false;
    }

    public bool IsDailyStoreRedDotOn()
    {
        foreach (KeyValuePair<int, DailyStoreInfo> pInfo in m_pDailyStoreInfoTable)
        {
            ExcelData_Shop_DailyInfo pDailyInfo = ExcelDataManager.Instance.m_pShop_Daily.GetShop_DailyInfo_byID(pInfo.Value.m_nID);

            if (!pInfo.Value.m_IsBuy && pDailyInfo.m_IsNotification)
                return true;
        }

        if(m_nDailyStore_RefreshCount > 0)
        {
            DateTime dtNow = new DateTime();
            dtNow = DateTime.Now;

            if(m_dtDailyStoreRefreshTime < dtNow)
            {
                return true;
            }
        }

        return false;
    }

    public void SaveDailyStoreRefreshRemainTime()
    {
        DateTime dtNow = new DateTime();
        dtNow = DateTime.Now;
        m_dtDailyStoreRefreshTime = dtNow.AddMinutes(30);

        SaveDataInfo.Instance.DailyStoreRemainSave(m_dtDailyStoreRefreshTime);

        if(m_nDailyStore_RefreshCount >= 1)
        {
            TimeSpan tsRemainTime = new TimeSpan();

            tsRemainTime = m_dtDailyStoreRefreshTime - DateTime.Now;

            //TimeInfo.Instance.SetTimer_ForDailyStoreRefresh((int)tsRemainTime.TotalSeconds);

            TimerManager.Instance.AddTimer(eTimerType.eDailyStoreRefresh, (int)tsRemainTime.TotalSeconds, 0);
            //NotificationManager.Instance.SetNotification(eNotificationType.eDailyStore, (float)tsRemainTime.TotalSeconds);
        }
    }

    public void SetDailyStoreRefreshRemainTime_bySaveDataInfo(DateTime dtDailyStoreRefreshTime)
    {
        m_dtDailyStoreRefreshTime = dtDailyStoreRefreshTime;

        if(m_dtDailyStoreRefreshTime > DateTime.Now)
        {
            TimeSpan tsRemainTime = new TimeSpan();

            tsRemainTime = m_dtDailyStoreRefreshTime - DateTime.Now;

            //TimeInfo.Instance.SetTimer_ForDailyStoreRefresh((int)tsRemainTime.TotalSeconds);

            TimerManager.Instance.AddTimer(eTimerType.eDailyStoreRefresh, (int)tsRemainTime.TotalSeconds, 0);
            //NotificationManager.Instance.SetNotification(eNotificationType.eDailyStore, (float)tsRemainTime.TotalSeconds);
        }
    }

    public void SetDailyStoreRefreshRemainTime_byTimeInfo(DateTime dtDailyStoreRefreshRemainTime)
    {
        m_dtDailyStoreRefreshTime = dtDailyStoreRefreshRemainTime;
    }

    public bool IsDailyStoreRefreshRemainTimeActive()
    {
        if(m_dtDailyStoreRefreshTime > DateTime.Now)
        {
            return true;
        }

        return false;
    }

    //public void SetDailyStoreRefreshRemainTimeInfo_byOutFocus()
    //{
    //    if(m_dtDailyStoreRefreshTime > DateTime.Now)
    //    {
    //        TimeSpan tsRemainTime = new TimeSpan();

    //        tsRemainTime = m_dtDailyStoreRefreshTime - DateTime.Now;

    //        //TimeInfo.Instance.SetTimer_ForDailyStoreRefresh((int)tsRemainTime.TotalSeconds);

    //        TimerManager.Instance.AddTimer(eTimerType.eDailyStoreRefresh, (int)tsRemainTime.TotalSeconds, 0);
    //        NotificationManager.Instance.SetAndroidNotification(eNotificationType.eDailyStore, (float)tsRemainTime.TotalSeconds);
    //    }
    //}

    public bool IsChapterPackageRedDot()
    {
        foreach(KeyValuePair<int, PackageStoreInfo> pInfo in m_pChapterInfoTable)
        {
            if (SaveDataInfo.Instance.GetChapterPackageIsRedDot(pInfo.Value.m_nRequiredChapter))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsCyclePackageRedDot()
    {
        foreach (KeyValuePair<int, PackageStoreInfo> pInfo in m_pCycleInfoTable)
        {
            if(pInfo.Value.m_ePaymentType == eShopPaymentType.Reward_AD && pInfo.Value.m_nPurchaseLimit != 0)
            {
                return true;
            }
        }

        return false;
    }
}
