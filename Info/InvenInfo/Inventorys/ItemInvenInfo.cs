using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.basic;
using sg.protocol.common;
using sg.protocol.user;

public class ItemInvenInfo
{
    // 실사용
    private Dictionary<int, ItemInvenItemInfo> m_ItemInvenTable_byTableID = new Dictionary<int, ItemInvenItemInfo>();
    private Dictionary<long, ItemInvenItemInfo> m_ItemInvenTable_byUniqueID = new Dictionary<long, ItemInvenItemInfo>();

    public ItemInvenInfo()
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
        // 재화는 남겨둠

        ItemInvenItemInfo pItemInvenItemInfo_Gem_Free = m_ItemInvenTable_byTableID.ContainsKey((int)eItemType.Gem_Free) == true ? m_ItemInvenTable_byTableID[(int)eItemType.Gem_Free] : null;
        ItemInvenItemInfo pItemInvenItemInfo_Gem_Paid = m_ItemInvenTable_byTableID.ContainsKey((int)eItemType.Gem_Paid) == true ? m_ItemInvenTable_byTableID[(int)eItemType.Gem_Paid] : null;
        ItemInvenItemInfo pItemInvenItemInfo_Gem_Gold = m_ItemInvenTable_byTableID.ContainsKey((int)eItemType.Gold) == true ? m_ItemInvenTable_byTableID[(int)eItemType.Gold] : null;
        ItemInvenItemInfo pItemInvenItemInfo_Gem_Energy = m_ItemInvenTable_byTableID.ContainsKey((int)eItemType.Energy) == true ? m_ItemInvenTable_byTableID[(int)eItemType.Energy] : null;

        m_ItemInvenTable_byTableID.Clear();
        m_ItemInvenTable_byUniqueID.Clear();

        SetItemInvenItemInfo(pItemInvenItemInfo_Gem_Free);
        SetItemInvenItemInfo(pItemInvenItemInfo_Gem_Paid);
        SetItemInvenItemInfo(pItemInvenItemInfo_Gem_Gold);
        SetItemInvenItemInfo(pItemInvenItemInfo_Gem_Energy);
    }

    public void SetItemInvenItemInfo(ItemInvenItemInfo pItemInvenItemInfo)
    {
        if(pItemInvenItemInfo == null)
            return;

        if (m_ItemInvenTable_byTableID.ContainsKey(pItemInvenItemInfo.m_nTableID) == true)
        {
            m_ItemInvenTable_byTableID[pItemInvenItemInfo.m_nTableID].m_nItemCount = pItemInvenItemInfo.m_nItemCount;
        }
        else
        {
            m_ItemInvenTable_byTableID.Add(pItemInvenItemInfo.m_nTableID, pItemInvenItemInfo);
        }

        if (m_ItemInvenTable_byUniqueID.ContainsKey(pItemInvenItemInfo.m_nUniqueID) == true)
        {
            m_ItemInvenTable_byUniqueID[pItemInvenItemInfo.m_nUniqueID].m_nItemCount = pItemInvenItemInfo.m_nItemCount;
        }
        else
        {
            m_ItemInvenTable_byUniqueID.Add(pItemInvenItemInfo.m_nUniqueID, pItemInvenItemInfo);
        }
    }

    public void CheckTimerForEnergy()
    {
        if(InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemInfo_byTableID((int)eItemType.Energy).m_nItemCount >= MyInfo.Instance.m_nMaxAP)
        {
            EventDelegateManager.Instance.OnLobby_SetRemainTimeFalse();
        }
    }

    public void SetTimerForEnergy(float nRemainTime)
    {
        if(InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemInfo_byTableID((int)eItemType.Energy).m_nItemCount < MyInfo.Instance.m_nMaxAP)
        {
            EventDelegateManager.Instance.OnLobby_RefreshEnergy();

            TimerManager.Instance.AddTimer(eTimerType.eEnergy, nRemainTime, 0);
            //NotificationManager.Instance.SetNotification(eNotificationType.eEnergy, nRemainTime);
        }
        else
        {
            EventDelegateManager.Instance.OnLobby_SetRemainTimeFalse();
            if(TimerManager.Instance.GetTimer(eTimerType.eEnergy) != null)
            {
                TimerManager.Instance.OnDone_Timer(TimerManager.Instance.GetTimer(eTimerType.eEnergy));
            }
        }
    }

    private void OnTimer_Complete(Timer pTimer, eTimerType eType, object parameter)
    {
        if(eType == eTimerType.eEnergy)
        {
            if (eType == eTimerType.eEnergy)
            {
                ItemInvenItemInfo pEnergy = InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemInfo_byTableID((int)eItemType.Energy);

                if (pEnergy.m_nItemCount < MyInfo.Instance.m_nMaxAP)
                {
                    pEnergy.m_nItemCount++;
                    InventoryInfoManager.Instance.m_pItemInvenInfo.SetItemInvenItemInfo(pEnergy);

                    SetTimerForEnergy(MyInfo.Instance.m_nAPChargeTerm);
                }
                else
                {
                    EventDelegateManager.Instance.OnLobby_SetRemainTimeFalse();
                }

                EventDelegateManager.Instance.OnLobby_RefreshEnergy();
            }
        }
    }

    public int GetNumInvenItemInfo()
    {
        return m_ItemInvenTable_byTableID.Count;
    }

    public ItemInvenItemInfo GetInvenItemInfo_byIndex(int nIndex)
    {
        int i = 0;

        foreach (KeyValuePair<int, ItemInvenItemInfo> pInfo in m_ItemInvenTable_byTableID)
        {
            if(i == nIndex)
                return pInfo.Value;

            ++i;
        }

        return null;
    }

    public ItemInvenItemInfo GetInvenItemInfo_byTableID(int nID)
    {
        if (m_ItemInvenTable_byTableID.ContainsKey(nID))
            return m_ItemInvenTable_byTableID[nID];

        return null;
    }

    public ItemInvenItemInfo GetInvenItemInfo_byUniqueID(long nID)
    {
        if (m_ItemInvenTable_byUniqueID.ContainsKey(nID))
            return m_ItemInvenTable_byUniqueID[nID];

        return null;
    }

    public void RemoveItemInfo(ItemInvenItemInfo pItemInfo)
    {
        if (m_ItemInvenTable_byTableID.ContainsKey(pItemInfo.m_nTableID))
        {
            m_ItemInvenTable_byTableID.Remove(pItemInfo.m_nTableID);
        }

        if (m_ItemInvenTable_byUniqueID.ContainsKey(pItemInfo.m_nUniqueID))
        {
            m_ItemInvenTable_byUniqueID.Remove(pItemInfo.m_nUniqueID);
        }
    }

    public void RemvoeItemInfo_byTableID(int nTableID)
    {
        ItemInvenItemInfo pItemInfo = GetInvenItemInfo_byTableID(nTableID);

        if (pItemInfo != null)
        {
            RemoveItemInfo(pItemInfo);
        }
    }

    public void RemoveItemInfo_byUniqueID(long nUniqueID)
    {
        ItemInvenItemInfo pItemInfo = GetInvenItemInfo_byUniqueID(nUniqueID);

        if (pItemInfo != null)
        {
            RemoveItemInfo(pItemInfo);
        }
    }

    public int GetInvenItemCount_byTableID(int nID)
    {
        if (m_ItemInvenTable_byTableID.ContainsKey(nID))
            return m_ItemInvenTable_byTableID[nID].m_nItemCount;

        return 0;
    }

    public Dictionary<int, int> GetAllItem()
    {
        Dictionary<int, int> pTable = new Dictionary<int, int>();

        foreach(KeyValuePair<int,ItemInvenItemInfo> pInfo in m_ItemInvenTable_byTableID)
        {
            pTable.Add(pInfo.Key, pInfo.Value.m_nItemCount);
        }

        return pTable;
    }

    public int GetGemCount()
    {
        int nGem = 0;

        foreach(KeyValuePair<int,ItemInvenItemInfo> pInfo in m_ItemInvenTable_byTableID)
        {
            if (pInfo.Value.m_eItemType == eItemType.Gem_Free || pInfo.Value.m_eItemType == eItemType.Gem_Paid)
                nGem += pInfo.Value.m_nItemCount;
        }

        return nGem;
    }

    public List<int> GetEnergyItemIDList()
    {
        Dictionary<int, int> pItemIDOrderDic = new Dictionary<int, int>();
        List<int> pOrderList = new List<int>();
        Dictionary<int, int> pItemIDCustomOrderDoneDic = new Dictionary<int, int>();

        List<int> pItemIDList = new List<int>();

        foreach(KeyValuePair<int,ItemInvenItemInfo> pInfo in m_ItemInvenTable_byTableID)
        {
            if(pInfo.Value.m_eItemType == eItemType.EnergyCharger)
            {
                int nOrder = ExcelDataManager.Instance.m_pItem.GetItemInfo_byID(pInfo.Value.m_nTableID).m_nCustomOrder;
                pItemIDOrderDic.Add(nOrder, pInfo.Value.m_nTableID);
                pOrderList.Add(nOrder);
            }
        }

        pOrderList.Sort();

        foreach(int pOrder in pOrderList)
        {
            pItemIDCustomOrderDoneDic.Add(pOrder, pItemIDOrderDic[pOrder]);
        }

        foreach(KeyValuePair<int,int> pInfo in pItemIDCustomOrderDoneDic)
        {
            pItemIDList.Add(pInfo.Value);
        }

        return pItemIDList;
    }
}