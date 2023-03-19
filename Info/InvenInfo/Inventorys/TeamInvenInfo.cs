using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamInvenInfo
{
	private int m_nTeamID		= 0;
	private int m_nTeamPower	= 0;

	private int m_nDB_Seq		= 0;    // 서버측 row의 변경이 필요한 컨텐츠나 기능시에 해당 key값을 서버에서 요청할 수도 있기에 저장을 해둠.

	private Dictionary<int, CharacterInvenItemInfo> m_InvenItemInfoTable = new Dictionary<int, CharacterInvenItemInfo>();

	public TeamInvenInfo(int nTeamID)
	{
		m_nTeamID = nTeamID;
	}

	public TeamInvenInfo(int nTeamID, int nDB_Seq)
	{
		m_nTeamID = nTeamID;
		m_nDB_Seq = nDB_Seq;
	}

	public TeamInvenInfo Copy(CharacterInvenInfo pCharacterInvenInfo)
	{
		TeamInvenInfo pTeamInvenInfo = new TeamInvenInfo(this.m_nTeamID);

		foreach (KeyValuePair<int, CharacterInvenItemInfo> item in this.m_InvenItemInfoTable)
		{
			pTeamInvenInfo.GetAllInvenItemInfoTable().Add(item.Key, pCharacterInvenInfo.GetInvenItem_byTableID(item.Value.m_nTableID));
		}

		return pTeamInvenInfo;
	}

	public int GetTeamID()
	{
		return m_nTeamID;
	}

	public int GetDB_Seq()
	{
		return m_nDB_Seq;
	}

	public void SetTeamPower(int nTeamPower)
	{
		m_nTeamPower = nTeamPower;
	}

	public int GetTeamPower()
	{
		int nTeamPower = 0;
		foreach (KeyValuePair<int, CharacterInvenItemInfo> item in this.m_InvenItemInfoTable)
		{
			nTeamPower += item.Value.GetPower();
		}

		return nTeamPower;
	}

	public void ClearInvenItemInfo()
    {
		m_InvenItemInfoTable.Clear();
    }

	public void AddInvenItemInfo(int nSlot, CharacterInvenItemInfo pInventItemInfo)
	{
		if (pInventItemInfo != null && GameDefine.ms_nTeamCharacterCount > nSlot)
		{
			if (m_InvenItemInfoTable.ContainsKey(nSlot) == true)
			{
				m_InvenItemInfoTable[nSlot].m_TeamSlotTable.Remove(m_nTeamID);
				m_InvenItemInfoTable.Remove(nSlot);
			}

			m_InvenItemInfoTable.Add(nSlot, pInventItemInfo);

			if (pInventItemInfo.m_TeamSlotTable.ContainsKey(m_nTeamID) == true)
			{
				pInventItemInfo.m_TeamSlotTable[m_nTeamID] = nSlot;
			}
			else
			{
				pInventItemInfo.m_TeamSlotTable.Add(m_nTeamID, nSlot);
			}
		}
	}

	public void DeleteInvenItemInfo(int nSlot)
    {
		if (m_InvenItemInfoTable.ContainsKey(nSlot) == true)
        {
			m_InvenItemInfoTable[nSlot].m_TeamSlotTable.Remove(m_nTeamID);
			m_InvenItemInfoTable.Remove(nSlot);
		}
	}

	public void SwapInvenItemInfo(int nFirstSlot, int nSecondSlot, CharacterInvenItemInfo pFirstInvenItemInfo, CharacterInvenItemInfo pSecondInvenItemInfo)
    {
        if (m_InvenItemInfoTable.ContainsKey(nFirstSlot) == true &&
           m_InvenItemInfoTable.ContainsKey(nSecondSlot) == true)
        {
			DeleteInvenItemInfo(nFirstSlot);
			DeleteInvenItemInfo(nSecondSlot);
			AddInvenItemInfo(nSecondSlot, pFirstInvenItemInfo);
			AddInvenItemInfo(nFirstSlot, pSecondInvenItemInfo);
		}
    }

	public CharacterInvenItemInfo GetInvenItemInfo(int nSlot)
	{
		if (m_InvenItemInfoTable.ContainsKey(nSlot) == true)
		{
			return m_InvenItemInfoTable[nSlot];
		}

		return null;
	}

	public Dictionary<int, CharacterInvenItemInfo> GetAllInvenItemInfoTable()
    {
		return m_InvenItemInfoTable;
    }

	public bool IsCharacterContain(int nUnitID)
    {
		for(int i=0; i<GameDefine.ms_nTeamCharacterCount; i++)
        {
            if (m_InvenItemInfoTable.ContainsKey(i) == true &&
				m_InvenItemInfoTable[i].m_nTableID == nUnitID)
            {
                return true;
            }
        }

		return false;
    }

	public bool IsValidTeam()
	{
		for (int i = 0; i < GameDefine.ms_nTeamCharacterCount; ++i)
		{
			if (m_InvenItemInfoTable.ContainsKey(i) == false)
			{
				return false;
			}
		}

		return true;
	}

	public bool IsEmptyTeam()
    {
		for(int i=0; i<GameDefine.ms_nTeamCharacterCount; i++)
        {
			if(m_InvenItemInfoTable.ContainsKey(i) == true)
            {
				return false;
            }
        }

		return true;
    }
}

public class TeamInvenInfoGroup
{
	private Dictionary<int, TeamInvenInfo> m_TeamInvenInfoTable = new Dictionary<int, TeamInvenInfo>();

	public TeamInvenInfoGroup()
	{
	}

	public void ClearAll()
	{
		m_TeamInvenInfoTable.Clear();
	}

	public void ClearInvenItemInfo_byTeamID(int nTeamID)
	{
		m_TeamInvenInfoTable[nTeamID].ClearInvenItemInfo();
	}

	public void AddTeamInvenInfo(TeamInvenInfo pTeamInvenInfo)
	{
		if (GameDefine.ms_nMaxTeamCount > pTeamInvenInfo.GetTeamID())
		{
			if (m_TeamInvenInfoTable.ContainsKey(pTeamInvenInfo.GetTeamID()) == true)
			{
				m_TeamInvenInfoTable.Remove(pTeamInvenInfo.GetTeamID());
			}

			m_TeamInvenInfoTable.Add(pTeamInvenInfo.GetTeamID(), pTeamInvenInfo);
		}
	}

	public void AddTeamInvenInfo(int nTeamID, int nSlot, CharacterInvenItemInfo pInvenItemInfo)
	{
		if (GameDefine.ms_nMaxTeamCount > nTeamID)
		{
			TeamInvenInfo pTeamInvenInfo;

			if (m_TeamInvenInfoTable.ContainsKey(nTeamID) == true)
			{
				pTeamInvenInfo = m_TeamInvenInfoTable[nTeamID];
			}
			else
			{
				pTeamInvenInfo = new TeamInvenInfo(nTeamID);
				m_TeamInvenInfoTable.Add(nTeamID, pTeamInvenInfo);
			}

			pTeamInvenInfo.AddInvenItemInfo(nSlot, pInvenItemInfo);
		}
	}

	public void DeleteTeamInvenInfo(int nTeamID, int nSlot)
    {
		if(GameDefine.ms_nMaxTeamCount > nTeamID)
        {
			TeamInvenInfo pTeamInvenInfo;

			if(m_TeamInvenInfoTable.ContainsKey(nTeamID) == true)
            {
				pTeamInvenInfo = m_TeamInvenInfoTable[nTeamID];

				pTeamInvenInfo.DeleteInvenItemInfo(nSlot);
            }
        }
    }

	public void SwapTeamInvenInfo(int nTeamID, int nFirstSlot, int nSecondSlot, CharacterInvenItemInfo pFirstInvenItemInfo, CharacterInvenItemInfo pSecondInvenItemInfo)
    {
        if (GameDefine.ms_nMaxTeamCount > nTeamID)
        {
			TeamInvenInfo pTeamInvenInfo;

			if(m_TeamInvenInfoTable.ContainsKey(nTeamID) == true)
            {
				pTeamInvenInfo = m_TeamInvenInfoTable[nTeamID];

				pTeamInvenInfo.SwapInvenItemInfo(nFirstSlot, nSecondSlot, pFirstInvenItemInfo, pSecondInvenItemInfo);
            }
        }
    }

	public TeamInvenInfo GetTeamInvenInfo(int nTeamID)
	{
		if (m_TeamInvenInfoTable.ContainsKey(nTeamID) == true)
		{
			return m_TeamInvenInfoTable[nTeamID];
		}

		return null;
	}

	public Dictionary<int, TeamInvenInfo> GetAllTeamInvenInfo()
    {
		return m_TeamInvenInfoTable;
    }
}
