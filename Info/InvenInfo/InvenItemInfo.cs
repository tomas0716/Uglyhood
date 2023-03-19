using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvenItemInfo
{
	public int m_nTableID	= 0;
	public int m_nUniqueID	= 0;

	public int m_nDB_Seq	= 0;    // 서버측 row의 변경이 필요한 컨텐츠나 기능시에 해당 key값을 서버에서 요청할 수도 있기에 저장을 해둠.
}

public class CharacterInvenItemInfo : InvenItemInfo
{
	public int m_nLevel;
	public int m_nMaxHP;
	public int m_nMaxSP;
	public int m_nSp_ChargePerBlock;
	public int m_nATK;
	public int m_nAction_Level;
	public int m_nPassive_Level;
	public int m_nPower;
	public int m_nUnit_AddTime;

	public Dictionary<int,int>	m_TeamSlotTable = new Dictionary<int, int>();		// Key : Team ID, Value : Slot

	public int MinTeam
	{
		get
		{
			List<int> keyList = new List<int>(m_TeamSlotTable.Keys);
			return keyList[0];
		}
	}

	public int GetPower()
    {
		if(m_nPower != 0)
			return m_nPower;

		ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(this.m_nTableID);

		double nPower = 0;

		if (this.m_nMaxSP != 0)
		{
			nPower = (this.m_nATK * 2.5) + (this.m_nMaxHP * 0.28) + (this.m_nMaxSP * 0.35) + (10 / this.m_nMaxSP / this.m_nSp_ChargePerBlock * 3) + (18 * (pUnitInfo.m_nRank - 1));
		}
		else
		{
			nPower = (this.m_nATK * 2.5) + (this.m_nMaxHP * 0.28) + (this.m_nMaxSP * 0.35) + (18 * (pUnitInfo.m_nRank - 1));
		}

		return Convert.ToInt32(nPower);
	}

	public int GetRank()
    {
		ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(this.m_nTableID);

		return pUnitInfo.m_nRank;
	}

	public CharacterInvenItemInfo Copy() 
	{
		CharacterInvenItemInfo pCopyObject = new CharacterInvenItemInfo();
		pCopyObject.m_nDB_Seq = this.m_nDB_Seq;
		pCopyObject.m_nTableID = this.m_nTableID;
		pCopyObject.m_nUniqueID = this.m_nUniqueID;
		pCopyObject.m_nLevel = this.m_nLevel;
		pCopyObject.m_nMaxHP = this.m_nMaxHP;
		pCopyObject.m_nMaxSP = this.m_nMaxSP;
		pCopyObject.m_nSp_ChargePerBlock = this.m_nSp_ChargePerBlock;
		pCopyObject.m_nATK = this.m_nATK;
		pCopyObject.m_nAction_Level = this.m_nAction_Level;
		pCopyObject.m_nPower = this.m_nPower;
		pCopyObject.m_nPassive_Level = this.m_nPassive_Level;
		foreach (KeyValuePair<int,int> item in this.m_TeamSlotTable)
		{
			pCopyObject.m_TeamSlotTable.Add(item.Key, item.Value);
		}
		pCopyObject.m_nUnit_AddTime = this.m_nUnit_AddTime;

		return pCopyObject;
	}
}

public class BoosterItemInvenItemInfo : InvenItemInfo
{
	public int m_nItemCount;

	public BoosterItemInvenItemInfo Copy()
	{
		BoosterItemInvenItemInfo pCopyObject = new BoosterItemInvenItemInfo();
		pCopyObject.m_nTableID = this.m_nTableID;
		pCopyObject.m_nUniqueID = this.m_nUniqueID;
		pCopyObject.m_nItemCount = this.m_nItemCount;

		return pCopyObject;
	}
}

public class ItemInvenItemInfo : InvenItemInfo
{
	public int m_nItemCount;
	public eItemType m_eItemType;

	public ItemInvenItemInfo(int nTableID, int nUniqueID, int nDB_Seq, int nItemCount, eItemType eType)
	{
		m_nTableID = nTableID;
		m_nUniqueID = nUniqueID;
		m_nDB_Seq = nDB_Seq;
		m_nItemCount = nItemCount;
		m_eItemType = eType;
	}
}