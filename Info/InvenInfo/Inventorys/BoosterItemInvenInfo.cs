using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterItemInvenInfo
{
    private int         m_nDeckUniqueID     = 0;
    private string      m_strSetName        = "";
    private Dictionary<int, BoosterItemInvenItemInfo> m_InvenItemInfoTable = new Dictionary<int, BoosterItemInvenItemInfo>();

    private int         m_nDB_Seq       = 0;    // 서버측 row의 변경이 필요한 컨텐츠나 기능시에 해당 key값을 서버에서 요청할 수도 있기에 저장을 해둠.

    public BoosterItemInvenInfo()
    {
        for (int i = 0; i < GameDefine.ms_nMaxBoosterItemCount; ++i)
        {
            m_InvenItemInfoTable.Add(i, BoosterItemInvenInfoGroup.ms_EmptyBoosterItemInvenInfo);
        }
    }

    public BoosterItemInvenInfo(int nDeckUniqueID, int nDB_Seq)
    {
        m_nDeckUniqueID = nDeckUniqueID;
        m_nDB_Seq = nDB_Seq;

        for (int i = 0; i < GameDefine.ms_nMaxBoosterItemCount; ++i)
        {
            m_InvenItemInfoTable.Add(i, BoosterItemInvenInfoGroup.ms_EmptyBoosterItemInvenInfo);
        }
    }

    public void ClearAll()
    {
        for (int i = 0; i < GameDefine.ms_nMaxBoosterItemCount; ++i)
        {
            m_InvenItemInfoTable[i] = BoosterItemInvenInfoGroup.ms_EmptyBoosterItemInvenInfo;
        }
    }

    public void SetDeckUniqueID(int nUniqueID)
    {
        m_nDeckUniqueID = nUniqueID;
    }

    public int GetDeckUniqueID()
    {
        return m_nDeckUniqueID;
    }

    public void SetDB_Seq(int nDB_Seq)
    {
        m_nDB_Seq = nDB_Seq;
    }

    public int GetDB_Seq()
    {
        return m_nDB_Seq;
    }

    public void SetSetName(string strName)
    {
        m_strSetName = strName;
    }

    public string GetSetName()
    {
        return m_strSetName;
    }

    public void AddBoosterItemInvenInfo(int nSlot, BoosterItemInvenItemInfo pItemInvenInfo)
    {
        if (m_InvenItemInfoTable.ContainsKey(nSlot) == true)
        {
            m_InvenItemInfoTable[nSlot] = pItemInvenInfo;
        }
    }

    public void RemoveBoosterItemInvenInfo(int nSlot)
    {
        if (m_InvenItemInfoTable.ContainsKey(nSlot) == true)
        {
            m_InvenItemInfoTable[nSlot] = BoosterItemInvenInfoGroup.ms_EmptyBoosterItemInvenInfo;
        }
    }

    public BoosterItemInvenItemInfo GetBoosterItemInfoInfo_bySlot(int nSlot)
    {
        if (m_InvenItemInfoTable.ContainsKey(nSlot) == true)
        {
            return m_InvenItemInfoTable[nSlot];
        }

        return null;
    }
}

public class BoosterItemInvenInfoGroup
{
    private Dictionary<int, BoosterItemInvenInfo>   m_BoosterItemInvenInfoTable     = new Dictionary<int, BoosterItemInvenInfo>();  // Key : Preset Index
    public static BoosterItemInvenItemInfo          ms_EmptyBoosterItemInvenInfo    = new BoosterItemInvenItemInfo();
    private BoosterItemInvenItemInfo                m_ADBoosterItemInvenInfo        = ms_EmptyBoosterItemInvenInfo;

    public BoosterItemInvenInfoGroup()
    {
        for (int i = 0; i < GameDefine.ms_nMaxBoosterItemSetCount; ++i)
        {
            m_BoosterItemInvenInfoTable.Add(i, new BoosterItemInvenInfo());
        }
    }

    public void ClearAll()
    {
        for (int i = 0; i < GameDefine.ms_nMaxBoosterItemSetCount; ++i)
        {
            m_BoosterItemInvenInfoTable[i].ClearAll();
        }
    }

    public void SetBoosterItemDeckUniqueID(int nSet, int nUniqueID)
    {
        if (m_BoosterItemInvenInfoTable.ContainsKey(nSet) == true)
        {
            m_BoosterItemInvenInfoTable[nSet].SetDeckUniqueID(nUniqueID);
        }
    }

    public int GetBoosterItemDeckUniqueID(int nSet)
    {
        if (m_BoosterItemInvenInfoTable.ContainsKey(nSet) == true)
        {
            return m_BoosterItemInvenInfoTable[nSet].GetDeckUniqueID();
        }

        return 0;
    }

    public void SetBoosterItemSetName(int nSet, string strName)
    {
        if (m_BoosterItemInvenInfoTable.ContainsKey(nSet) == true)
        {
            m_BoosterItemInvenInfoTable[nSet].SetSetName(strName);
        }
    }

    public string GetBoosterItemSetName(int nSet)
    {
        if (m_BoosterItemInvenInfoTable.ContainsKey(nSet) == true)
        {
            return m_BoosterItemInvenInfoTable[nSet].GetSetName();
        }

        return "";
    }

    public void SetBoosterItemDB_Seq(int nSet, int nDB_Seq)
    {
        if (m_BoosterItemInvenInfoTable.ContainsKey(nSet) == true)
        {
            m_BoosterItemInvenInfoTable[nSet].SetDB_Seq(nDB_Seq);
        }
    }

    public int GetBoosterItemDB_Seq(int nSet)
    {
        if (m_BoosterItemInvenInfoTable.ContainsKey(nSet) == true)
        {
            return m_BoosterItemInvenInfoTable[nSet].GetDB_Seq();
        }

        return 0;
    }

    public void AddBoosterItemInvenInfo(int nSet, int nSlot, BoosterItemInvenItemInfo pItemInvenInfo)
    {
        if (m_BoosterItemInvenInfoTable.ContainsKey(nSet) == true)
        {
            m_BoosterItemInvenInfoTable[nSet].AddBoosterItemInvenInfo(nSlot, pItemInvenInfo);
        }
    }

    public void RemoveBoosterItemInvenInfo(int nSet, int nSlot)
    {
        if(nSlot == (int)eBoosterItemType_bySlot.AD)
        {
            m_ADBoosterItemInvenInfo = ms_EmptyBoosterItemInvenInfo;
        }

        if (m_BoosterItemInvenInfoTable.ContainsKey(nSet) == true)
        {
            m_BoosterItemInvenInfoTable[nSet].RemoveBoosterItemInvenInfo(nSlot);
        }
    }

    public BoosterItemInvenItemInfo GetBoosterItemInfoInfo_bySlot(int nSet, int nSlot)
    {
        if(nSlot == (int)eBoosterItemType_bySlot.AD)
        {
            // 광고 슬롯
            return m_ADBoosterItemInvenInfo;
        }

        if (m_BoosterItemInvenInfoTable.ContainsKey(nSet) == true)
        {
            return m_BoosterItemInvenInfoTable[nSet].GetBoosterItemInfoInfo_bySlot(nSlot);
        }

        return null;
    }

    public void SetADBoosterItem_byItemInfo(ExcelData_ItemInfo pItemInfo)
    {
        BoosterItemInvenItemInfo pInfo = new BoosterItemInvenItemInfo();
        pInfo.m_nTableID = pItemInfo.m_nID;
        pInfo.m_nItemCount = 1;

        m_ADBoosterItemInvenInfo = pInfo;
    }

    public void SetADBoosterItemClear()
    {
        m_ADBoosterItemInvenInfo = ms_EmptyBoosterItemInvenInfo;
    }

    public bool IsADBoosterItemEmpty()
    {
        if (m_ADBoosterItemInvenInfo == ms_EmptyBoosterItemInvenInfo)
        {
            return true;
        }

        return false;
    }

    public void OnResetEquipCount()
    {
        foreach(KeyValuePair<int, BoosterItemInvenInfo> item in m_BoosterItemInvenInfoTable)
        {
            for (int i = 0; i < GameDefine.ms_nMaxBoosterItemCount; ++i)
            {
                BoosterItemInvenItemInfo pBoosterItemInvenItemInfo = item.Value.GetBoosterItemInfoInfo_bySlot(i);

                if (pBoosterItemInvenItemInfo != null && pBoosterItemInvenItemInfo != ms_EmptyBoosterItemInvenInfo)
                {
                    ItemInvenItemInfo pItemInvenItemInfo = InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemInfo_byTableID(pBoosterItemInvenItemInfo.m_nTableID);
                    ExcelData_ItemInfo pItemInfo = ExcelDataManager.Instance.m_pItem.GetItemInfo_byID(pBoosterItemInvenItemInfo.m_nTableID);

                    if (pItemInvenItemInfo != null && pItemInfo != null)
                    {
                        if (pItemInvenItemInfo.m_nItemCount > 0)
                        {
                            pBoosterItemInvenItemInfo.m_nItemCount = pItemInvenItemInfo.m_nItemCount >= pItemInfo.m_nMaxEquip ? pItemInfo.m_nMaxEquip : pItemInvenItemInfo.m_nItemCount;
                        }
                        else
                        {
                            item.Value.RemoveBoosterItemInvenInfo(i);
                        }
                    }
                    else
                    {
                        item.Value.RemoveBoosterItemInvenInfo(i);
                    }
                }
            }
        }
    }
}