using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SlotManager : MonoBehaviour
{
    private int         m_nMaxSwwaPossibleCount     = 20;
    private Slot        m_pSlot_SwapPossible_01     = null;
    private Slot        m_pSlot_SwapPossible_02     = null;

    private Dictionary<int, List<SlotBlock>>                m_PossibleMoveSlotBlockTable            = new Dictionary<int, List<SlotBlock>>();
    private Dictionary<int, SlotBlock>                      m_PossibleMoveSlotBlockSwapDestTable    = new Dictionary<int, SlotBlock>();
    private int                                             m_nPossibleMoveSlotBlockTableIndex      = 0;
    private Dictionary<int, List<KeyValuePair<Slot, Slot>>> m_SwapPossibleTable                     = new Dictionary<int, List<KeyValuePair<Slot, Slot>>>();

    private enum ePossibleMoveSlotResult
    {
        Possible,
        InPossible,
        HideShowIncludePossible,
    }

    public bool Calculation_PossibleMoveSlot()
    {
        bool IsPossible = false;

        m_PossibleMoveSlotBlockList.Clear();
        m_PossibleMoveSlotBlockTable.Clear();
        m_PossibleMoveSlotBlockSwapDestTable.Clear();
        m_nPossibleMoveSlotBlockTableIndex = 0;

        for (int i = 0; i < m_nMaxSwwaPossibleCount; ++i)
        {
            m_SwapPossibleTable[i].Clear();
        }

        ePossibleMoveSlotResult eResult;
		eResult = Calculation_PossibleMoveSlot_Normal();

        if (eResult == ePossibleMoveSlotResult.InPossible)
        {
            if (InGameSetting.Instance.m_pInGameSettingData.m_eSpecialItemUseWay == eSpecialItemUseWay.Match)
            {
                if (Calculation_PossibleMoveSlot_SpecialSlot() == false)
                {
                    if (Calculation_PossibleMoveSlot_MatchColor() == false)
                    {
                        IsPossible = true;
                    }
                }
            }
        }
        else if (eResult == ePossibleMoveSlotResult.Possible)
        {
            IsPossible = true;
        }

        return IsPossible;
    }

    private ePossibleMoveSlotResult Calculation_PossibleMoveSlot_Normal()
    {
        Dictionary<int, List<SlotBlock>> PossibleMoveSlotBlockTable = new Dictionary<int, List<SlotBlock>>();

        foreach (Slot pSlot in m_ReverseSlotList)
        {
            if (pSlot.GetSlotBlock() != null && (IsNormalBlock(pSlot) == true || IsSpecialBlock(pSlot) == true) &&
                IsObstacle_OnlyMineBreak(pSlot) == false && IsObstacle_MatchPossible(pSlot) == true)
            {
                Calculation_PossibleMoveSlot(pSlot, eNeighbor.Neighbor_10, PossibleMoveSlotBlockTable);
                Calculation_PossibleMoveSlot(pSlot, eNeighbor.Neighbor_01, PossibleMoveSlotBlockTable);
                Calculation_PossibleMoveSlot(pSlot, eNeighbor.Neighbor_21, PossibleMoveSlotBlockTable);
                Calculation_PossibleMoveSlot(pSlot, eNeighbor.Neighbor_12, PossibleMoveSlotBlockTable);
            }
        }

        int nTableCount = PossibleMoveSlotBlockTable.Count;

        if (nTableCount > 0)
        {
            List<int> PossibleList = new List<int>();
            foreach (KeyValuePair<int, List<SlotBlock>> item in PossibleMoveSlotBlockTable)
            {
                PossibleList.Add(item.Key);
            }

            if (PossibleList.Count == 0)
            {
                return ePossibleMoveSlotResult.HideShowIncludePossible;
            }
            else
            {
                System.Random random = new System.Random();

                for (int i = 3; i < m_nMaxSwwaPossibleCount; ++i)
                {
                    if (m_SwapPossibleTable[i].Count != 0)
                    {
                        int nRandom = random.Next(0, m_SwapPossibleTable[i].Count);
                        KeyValuePair<Slot,Slot> keyValue = m_SwapPossibleTable[i][nRandom];

                        m_pSlot_SwapPossible_01 = keyValue.Key;
                        m_pSlot_SwapPossible_02 = keyValue.Value;

                        break;
                    }
                }

				return ePossibleMoveSlotResult.Possible;
            }
        }

        return ePossibleMoveSlotResult.InPossible;
    }

    private bool Calculation_PossibleMoveSlot(Slot pSlot, eNeighbor eChangeNeighbor, Dictionary<int, List<SlotBlock>> PossibleMoveSlotBlockTable)
    {
        Slot pNeighbor = pSlot.GetNeighborSlot(eChangeNeighbor);

        if (pNeighbor == null)
        {
            return false;
        }

        if (pNeighbor.GetSlotBlock() == null)
            return false;

        if(IsNormalBlock(pNeighbor) == false && IsSpecialBlock(pNeighbor) == false)
            return false;

        return Calculation_PossibleMoveSlot(pSlot, pNeighbor, PossibleMoveSlotBlockTable);
    }

    private bool Calculation_PossibleMoveSlot(Slot pSlot_01, Slot pSlot_02, Dictionary<int, List<SlotBlock>> PossibleMoveSlotBlockTable)
    {
        bool IsRes = false;
        int nCount = PossibleMoveSlotBlockTable.Count;
        SlotBlock pBaseSlotBlock = pSlot_01.GetSlotBlock();

        if (pSlot_02 != null && pSlot_02.GetSlotBlock() != null && IsCheckSlotPossibleMove(pSlot_01, pSlot_02) == true)
        {
            SlotBlock pTargetSlotBlock = pSlot_02.GetSlotBlock();

            pSlot_01.SetTempSlotBlock(pTargetSlotBlock);
            pSlot_02.SetTempSlotBlock(pBaseSlotBlock);

            Dictionary<int, bool> ArleadyCheckBlockTable = new Dictionary<int, bool>();

            foreach (Slot pTempSlot in m_ReverseSlotList)
            {
                if (pTempSlot.GetSlotBlock() != null && ArleadyCheckBlockTable.ContainsKey(pTempSlot.GetSlotIndex()) == false)
                {
                    Dictionary<int, Slot> MatchSlotTable = new Dictionary<int, Slot>();
                    Slot pCombinationSlot = null;
                    int nIndex = GetShape(pTempSlot, ref MatchSlotTable, ref pCombinationSlot);
                    if (nIndex != -1)
                    {
                        List<SlotBlock> MatchSlotBlockList = new List<SlotBlock>();
                        foreach (KeyValuePair<int, Slot> item in MatchSlotTable)
                        {
                            MatchSlotBlockList.Add(item.Value.GetSlotBlock());

                            if (ArleadyCheckBlockTable.ContainsKey(item.Key) == false)
                            {
                                ArleadyCheckBlockTable.Add(item.Key, item.Value);
                            }
                        }

                        PossibleMoveSlotBlockTable.Add(nCount++, MatchSlotBlockList);
                        m_SwapPossibleTable[MatchSlotTable.Count].Add(new KeyValuePair<Slot,Slot>(pSlot_01, pSlot_02));

                        if (MatchSlotBlockList.Contains(pTargetSlotBlock) == false)
                        {
                            MatchSlotBlockList.Add(pTargetSlotBlock);
                            if (m_PossibleMoveSlotBlockSwapDestTable.ContainsKey(m_nPossibleMoveSlotBlockTableIndex) == false)
                            {
                                m_PossibleMoveSlotBlockSwapDestTable.Add(m_nPossibleMoveSlotBlockTableIndex, pBaseSlotBlock);
                            }
                        }
                        else if (MatchSlotBlockList.Contains(pBaseSlotBlock) == false)
                        {
                            MatchSlotBlockList.Add(pBaseSlotBlock);
                            if (m_PossibleMoveSlotBlockSwapDestTable.ContainsKey(m_nPossibleMoveSlotBlockTableIndex) == false)
                            {
                                m_PossibleMoveSlotBlockSwapDestTable.Add(m_nPossibleMoveSlotBlockTableIndex, pTargetSlotBlock);
                            }
                        }

                        if (m_PossibleMoveSlotBlockTable.ContainsKey(m_nPossibleMoveSlotBlockTableIndex) == false)
                        {
                            m_PossibleMoveSlotBlockTable.Add(m_nPossibleMoveSlotBlockTableIndex, MatchSlotBlockList);
                        }
                        ++m_nPossibleMoveSlotBlockTableIndex;

                        IsRes = true;
                    }
                }
            }

            pSlot_01.SetTempSlotBlock(pBaseSlotBlock);
            pSlot_02.SetTempSlotBlock(pTargetSlotBlock);
        }

        return IsRes;
    }

    private bool Calculation_PossibleMoveSlot_SpecialSlot()
    {
        int nCount = 0;
        Dictionary<int, List<SlotBlock>> PossibleMoveSlotBlockTable = new Dictionary<int, List<SlotBlock>>();
        bool IsPossible = false;

        foreach (Slot pSlot in m_ReverseSlotList)
        {
            if (pSlot.GetSlotBlock() != null && IsSpecialBlock(pSlot) == true && IsObstacle_MatchPossible(pSlot) == true)
            {
                Slot pNeighbor;
                pNeighbor = pSlot.GetNeighborSlot(eNeighbor.Neighbor_10);
                if (pNeighbor != null && pNeighbor.GetSlotBlock() != null && IsSpecialBlock(pNeighbor) == true && IsObstacle_MatchPossible(pNeighbor) == true)
                {
                    List<SlotBlock> MatchSlotBlockList = new List<SlotBlock>();
                    MatchSlotBlockList.Add(pSlot.GetSlotBlock());
                    MatchSlotBlockList.Add(pNeighbor.GetSlotBlock());
                    PossibleMoveSlotBlockTable.Add(nCount++, MatchSlotBlockList);

                    IsPossible = true;
                }

                pNeighbor = pSlot.GetNeighborSlot(eNeighbor.Neighbor_01);
                if (pNeighbor != null && pNeighbor.GetSlotBlock() != null && IsSpecialBlock(pNeighbor) == true && IsObstacle_MatchPossible(pNeighbor) == true)
                {
                    List<SlotBlock> MatchSlotBlockList = new List<SlotBlock>();
                    MatchSlotBlockList.Add(pSlot.GetSlotBlock());
                    MatchSlotBlockList.Add(pNeighbor.GetSlotBlock());
                    PossibleMoveSlotBlockTable.Add(nCount++, MatchSlotBlockList);

                    IsPossible = true;
                }

                pNeighbor = pSlot.GetNeighborSlot(eNeighbor.Neighbor_21);
                if (pNeighbor != null && pNeighbor.GetSlotBlock() != null && IsSpecialBlock(pNeighbor) == true && IsObstacle_MatchPossible(pNeighbor) == true)
                {
                    List<SlotBlock> MatchSlotBlockList = new List<SlotBlock>();
                    MatchSlotBlockList.Add(pSlot.GetSlotBlock());
                    MatchSlotBlockList.Add(pNeighbor.GetSlotBlock());
                    PossibleMoveSlotBlockTable.Add(nCount++, MatchSlotBlockList);

                    IsPossible = true;
                }

                pNeighbor = pSlot.GetNeighborSlot(eNeighbor.Neighbor_12);
                if (pNeighbor != null && pNeighbor.GetSlotBlock() != null && IsSpecialBlock(pNeighbor) == true && IsObstacle_MatchPossible(pNeighbor) == true)
                {
                    List<SlotBlock> MatchSlotBlockList = new List<SlotBlock>();
                    MatchSlotBlockList.Add(pSlot.GetSlotBlock());
                    MatchSlotBlockList.Add(pNeighbor.GetSlotBlock());
                    PossibleMoveSlotBlockTable.Add(nCount++, MatchSlotBlockList);

                    IsPossible = true;
                }
            }
        }

        int nTableCount = PossibleMoveSlotBlockTable.Count;

        if (nTableCount > 0)
        {
            int nRandom = UnityEngine.Random.Range(0, nTableCount);
            foreach (SlotBlock pSlotBlock in PossibleMoveSlotBlockTable[nRandom])
            {
                m_PossibleMoveSlotBlockList.Add(pSlotBlock);
            }
        }

        return IsPossible;
    }

    private bool Calculation_PossibleMoveSlot_MatchColor()
    {
        int nCount = 0;
        Dictionary<int, List<SlotBlock>> PossibleMoveSlotBlockTable = new Dictionary<int, List<SlotBlock>>();
        bool IsPossible = false;

        foreach (Slot pSlot in m_ReverseSlotList)
        {
            if (pSlot.GetSlotBlock() != null && pSlot.GetSlotBlock().GetBlockType() == eBlockType.MatchColor && IsObstacle_MatchPossible(pSlot) == true)
            {
                Slot pNeighbor;
                pNeighbor = pSlot.GetNeighborSlot(eNeighbor.Neighbor_10);
                if (pNeighbor != null && pNeighbor.GetSlotBlock() != null && IsObstacle_MatchPossible(pNeighbor) == true)
                {
                    List<SlotBlock> MatchSlotBlockList = new List<SlotBlock>();
                    MatchSlotBlockList.Add(pSlot.GetSlotBlock());
                    MatchSlotBlockList.Add(pNeighbor.GetSlotBlock());
                    PossibleMoveSlotBlockTable.Add(nCount++, MatchSlotBlockList);

                    IsPossible = true;
                }

                pNeighbor = pSlot.GetNeighborSlot(eNeighbor.Neighbor_01);
                if (pNeighbor != null && pNeighbor.GetSlotBlock() != null && IsObstacle_MatchPossible(pNeighbor) == true)
                {
                    List<SlotBlock> MatchSlotBlockList = new List<SlotBlock>();
                    MatchSlotBlockList.Add(pSlot.GetSlotBlock());
                    MatchSlotBlockList.Add(pNeighbor.GetSlotBlock());
                    PossibleMoveSlotBlockTable.Add(nCount++, MatchSlotBlockList);

                    IsPossible = true;
                }

                pNeighbor = pSlot.GetNeighborSlot(eNeighbor.Neighbor_21);
                if (pNeighbor != null && pNeighbor.GetSlotBlock() != null && IsObstacle_MatchPossible(pNeighbor) == true)
                {
                    List<SlotBlock> MatchSlotBlockList = new List<SlotBlock>();
                    MatchSlotBlockList.Add(pSlot.GetSlotBlock());
                    MatchSlotBlockList.Add(pNeighbor.GetSlotBlock());
                    PossibleMoveSlotBlockTable.Add(nCount++, MatchSlotBlockList);

                    IsPossible = true;
                }

                pNeighbor = pSlot.GetNeighborSlot(eNeighbor.Neighbor_12);
                if (pNeighbor != null && pNeighbor.GetSlotBlock() != null && IsObstacle_MatchPossible(pNeighbor) == true)
                {
                    List<SlotBlock> MatchSlotBlockList = new List<SlotBlock>();
                    MatchSlotBlockList.Add(pSlot.GetSlotBlock());
                    MatchSlotBlockList.Add(pNeighbor.GetSlotBlock());
                    PossibleMoveSlotBlockTable.Add(nCount++, MatchSlotBlockList);

                    IsPossible = true;
                }
            }
        }

        int nTableCount = PossibleMoveSlotBlockTable.Count;

        if (nTableCount > 0)
        {
            int nRandom = UnityEngine.Random.Range(0, nTableCount);
            foreach (SlotBlock pSlotBlock in PossibleMoveSlotBlockTable[nRandom])
            {
                m_PossibleMoveSlotBlockList.Add(pSlotBlock);
            }
        }

        return IsPossible;
    }

    public bool IsExistSpecialSlot()
    {
        foreach (KeyValuePair<int, Slot> item in m_SlotTable)
        {
            if (item.Value.GetSlotBlock() != null && item.Value.GetSlotBlock().GetSpecialItem() != eSpecialItem.None && 
                IsObstacle_OnlyMineBreak(item.Value) == false && IsObstacle_MatchPossible(item.Value) == true)
            {
                if (item.Value.IsSlotBlockNoSwap() == false)
                {
                    return true;
                }
            }
        }

        return false;
    }

	public Slot GetRandomSpecialSlot()
	{
		List<Slot> slotList = new List<Slot>();

		foreach (KeyValuePair<int, Slot> item in m_SlotTable)
		{
			if (item.Value.GetSlotBlock() != null && item.Value.GetSlotBlock().GetSpecialItem() != eSpecialItem.None &&
                IsObstacle_OnlyMineBreak(item.Value) == false && IsObstacle_MatchPossible(item.Value) == true)
			{
                if (item.Value.IsSlotBlockNoSwap() == false)
                {
                    slotList.Add(item.Value);
                }
			}
		}

		if (slotList.Count == 0)
			return null;

		int nRandom = UnityEngine.Random.Range(0, slotList.Count);

		return slotList[nRandom];
	}

	public Slot GetSlot_SwapPossible_01()
    {
        return m_pSlot_SwapPossible_01;
    }

    public Slot GetSlot_SwapPossible_02()
    {
        return m_pSlot_SwapPossible_02;
    }

    public Dictionary<int, List<SlotBlock>> GetPossibleMoveSlotBlockTable()
    {
        return m_PossibleMoveSlotBlockTable;
    }

    public Dictionary<int, SlotBlock> GetPossibleMoveSlotBlockSwapDestTable()
    {
        return m_PossibleMoveSlotBlockSwapDestTable;
    }
}
