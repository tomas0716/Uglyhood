using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInvenInfo
{
	private List<CharacterInvenItemInfo>	m_InvenItemInfoList = new List<CharacterInvenItemInfo>();

	public CharacterInvenInfo()
	{
	}

	public void ClearAll()
	{
		m_InvenItemInfoList.Clear();
	}

	public void AddInvenItemInfo(CharacterInvenItemInfo pInvenItemInfo)
	{
		m_InvenItemInfoList.Add(pInvenItemInfo);
    }

	private int CompareInvenItemInfoList(CharacterInvenItemInfo A, CharacterInvenItemInfo B)
	{
		if (A.m_TeamSlotTable.Count == 0 && B.m_TeamSlotTable.Count == 0)
		{
			return 0;
		}
		else if (A.m_TeamSlotTable.Count != 0)
		{
			return 1;
		}
		else if (B.m_TeamSlotTable.Count != 0)
		{
			return -1;
		}

		return A.MinTeam.CompareTo(B.MinTeam);
	}

	public int GetNumInvenItem()
	{
		return m_InvenItemInfoList.Count;
	}

	public CharacterInvenItemInfo GetInvenItem_byIndex(int nIndex)
	{
		if(nIndex < 0 || nIndex >= m_InvenItemInfoList.Count)
			return null;

		return m_InvenItemInfoList[nIndex];
	}

	public CharacterInvenItemInfo GetInvenItem_byTableID(int nID)
	{
		foreach (CharacterInvenItemInfo pInfo in m_InvenItemInfoList)
		{
			if(pInfo.m_nTableID == nID)
				return pInfo;
		}

		return null;
	}

	public CharacterInvenItemInfo GetInvenItem_byUniqueID(long nCharacterID)
    {
		foreach(CharacterInvenItemInfo pInfo in m_InvenItemInfoList)
        {
			if (pInfo.m_nUniqueID == nCharacterID)
				return pInfo;
        }

		return null;
    }

	public List<CharacterInvenItemInfo> GetInvenItem_All()
    {
		return m_InvenItemInfoList;
    }
}
