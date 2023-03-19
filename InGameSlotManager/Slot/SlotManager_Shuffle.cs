using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SlotManager : MonoBehaviour
{
    private Dictionary<int, List<KeyValuePair<Slot, bool>>> m_MatchPossibleTable = new Dictionary<int, List<KeyValuePair<Slot, bool>>>();
    private Dictionary<int, eBlockType> m_MatchPossibleBlockTypeTable = new Dictionary<int, eBlockType>();

    private int             m_nMatchPossibleTableIndex              = 0;
    private List<Slot>      m_SuffleFixSlotList                     = new List<Slot>();

    private eBlockType      m_eBlockType_ForObstacle_OnlyMineBreak  = eBlockType.Empty;

    private bool RecursiveMatchPossibleCheck(Slot pSlot, ref List<Slot> matchPossibleCheckGroupList, ShapeNode pShapeNode, bool IsFirst = true)
    {
        int nNumChild = pShapeNode.GetChildNodeCount();

        for (int i = 0; i < nNumChild; ++i)
        {
            ShapeNode pChild = pShapeNode.GetChildNode_byIndex(i) as ShapeNode;

            if (pChild.m_eShapeData != eShapeData.Normal)
            {
                continue;
            }

            Slot pNextSlot = pSlot.GetNeighborSlot(pChild.m_eNeighbor_ToParent);
            if (pNextSlot == null || pNextSlot.GetSlotBlock() == null || IsObstacle_MatchPossible(pNextSlot) == false)
            {
                return false;
            }
            else if (pNextSlot != null && pNextSlot.GetSlotBlock() != null && (IsNormalBlock(pNextSlot) == true || IsSpecialBlock(pNextSlot) == true))
            {
                if (m_eBlockType_ForObstacle_OnlyMineBreak == eBlockType.Empty)
                {
                    if (IsObstacle_OnlyMineBreak(pNextSlot) == true)
                    {
                        m_eBlockType_ForObstacle_OnlyMineBreak = pNextSlot.GetSlotBlock().GetBlockType();
                    }
                }
                else
                {
                    if (IsObstacle_OnlyMineBreak(pNextSlot) == true && m_eBlockType_ForObstacle_OnlyMineBreak != pNextSlot.GetSlotBlock().GetBlockType())
                    {
                        return false;
                    }
                }

                matchPossibleCheckGroupList.Add(pNextSlot);

                if (RecursiveMatchPossibleCheck(pNextSlot, ref matchPossibleCheckGroupList, pChild, false) == false)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool CalculationShape(ShapeNode pShapeNode)
    {
        bool IsRes = false;
        foreach (Slot pSlot in m_ReverseSlotList)
        {
            if (pSlot.GetSlotBlock() != null && (IsNormalBlock(pSlot) == true || IsSpecialBlock(pSlot) == true) && 
                IsObstacle_OnlyMineBreak(pSlot) == false && IsObstacle_MatchPossible(pSlot) == true)
            {
                m_eBlockType_ForObstacle_OnlyMineBreak = eBlockType.Empty;

                if (IsObstacle_OnlyMineBreak(pSlot) == true)
                {
                    m_eBlockType_ForObstacle_OnlyMineBreak = pSlot.GetSlotBlock().GetBlockType();
                }

                List<Slot> matchPossibleCheckGroupList = new List<Slot>();
                matchPossibleCheckGroupList.Add(pSlot);

                if (RecursiveMatchPossibleCheck(pSlot, ref matchPossibleCheckGroupList, pShapeNode) == true)
                {
                    if (matchPossibleCheckGroupList.Count != 3)
                    {
                        OutputLog.Log("matchPossibleCheckGroupList.Count != 3 : " + pSlot.GetX() + "," + pSlot.GetY());
                        continue;
                    }

                    Helper.ShuffleList<Slot>(matchPossibleCheckGroupList);

                    Slot pInSlot = null, pOutSlot = null;
                    foreach (Slot pMatchSlot in matchPossibleCheckGroupList)
                    {
                        if (IsObstacle_OnlyMineBreak(pMatchSlot) == true)
                            continue;

						if (IsObstacle_MatchPossible(pMatchSlot) == false)
							continue;

						Slot pNeighborSlot;

                        pNeighborSlot = pMatchSlot.GetNeighborSlot(eNeighbor.Neighbor_10);
                        if (pNeighborSlot != null && pNeighborSlot.GetSlotBlock() != null && (IsNormalBlock(pNeighborSlot) == true || IsSpecialBlock(pNeighborSlot) == true) &&
                            matchPossibleCheckGroupList.Contains(pNeighborSlot) == false && IsObstacle_OnlyMineBreak(pNeighborSlot) == false && IsObstacle_MatchPossible(pNeighborSlot) == true)
                        {
                            pInSlot = pMatchSlot;
                            pOutSlot = pNeighborSlot;
                            break;
                        }

                        pNeighborSlot = pMatchSlot.GetNeighborSlot(eNeighbor.Neighbor_01);
                        if (pNeighborSlot != null && pNeighborSlot.GetSlotBlock() != null && (IsNormalBlock(pNeighborSlot) == true || IsSpecialBlock(pNeighborSlot) == true) && 
                            matchPossibleCheckGroupList.Contains(pNeighborSlot) == false && IsObstacle_OnlyMineBreak(pNeighborSlot) == false && IsObstacle_MatchPossible(pNeighborSlot) == true)
                        {
                            pInSlot = pMatchSlot;
                            pOutSlot = pNeighborSlot;
                            break;
                        }

                        pNeighborSlot = pMatchSlot.GetNeighborSlot(eNeighbor.Neighbor_21);
                        if (pNeighborSlot != null && pNeighborSlot.GetSlotBlock() != null && (IsNormalBlock(pNeighborSlot) == true || IsSpecialBlock(pNeighborSlot) == true) && 
                            matchPossibleCheckGroupList.Contains(pNeighborSlot) == false && IsObstacle_OnlyMineBreak(pNeighborSlot) == false && IsObstacle_MatchPossible(pNeighborSlot) == true)
                        {
                            pInSlot = pMatchSlot;
                            pOutSlot = pNeighborSlot;
                            break;
                        }

                        pNeighborSlot = pMatchSlot.GetNeighborSlot(eNeighbor.Neighbor_12);
                        if (pNeighborSlot != null && pNeighborSlot.GetSlotBlock() != null && (IsNormalBlock(pNeighborSlot) == true || IsSpecialBlock(pNeighborSlot) == true) && 
                            matchPossibleCheckGroupList.Contains(pNeighborSlot) == false && IsObstacle_OnlyMineBreak(pNeighborSlot) == false && IsObstacle_MatchPossible(pNeighborSlot) == true)
                        {
                            pInSlot = pMatchSlot;
                            pOutSlot = pNeighborSlot;
                            break;
                        }
                    }

                    if (pInSlot != null && pOutSlot != null)
                    {
                        foreach (Slot pMatchSlot in matchPossibleCheckGroupList)
                        {
                            OutputLog.Log("matchPossibleCheckGroupList : " + pMatchSlot.GetX() + "," + pMatchSlot.GetY());
                        }

                        List<KeyValuePair<Slot, bool>> matchPossibleCheckGroup = new List<KeyValuePair<Slot, bool>>();
                        matchPossibleCheckGroup.Add(new KeyValuePair<Slot, bool>(pInSlot, false));
                        matchPossibleCheckGroup.Add(new KeyValuePair<Slot, bool>(pOutSlot, true));

                        foreach (Slot pMatchSlot in matchPossibleCheckGroupList)
                        {
                            if (pMatchSlot != pInSlot)
                            {
                                matchPossibleCheckGroup.Add(new KeyValuePair<Slot, bool>(pMatchSlot, true));
                            }
                        }

                        m_MatchPossibleBlockTypeTable.Add(m_nMatchPossibleTableIndex, m_eBlockType_ForObstacle_OnlyMineBreak);
                        m_MatchPossibleTable.Add(m_nMatchPossibleTableIndex++, matchPossibleCheckGroup);

                        IsRes = true;
                    }

                    OutputLog.Log("--------------------------------------");
                }
            }
        }
        return IsRes;
    }

    public bool IsSwapAble()
    {
        bool IsSwapAble = false;

        CombinationShapeData pCombinationShapeData = InGameSetting.Instance.m_pInGameSettingData.GetCombinationShapeData(eSpecialItem.None);

        if (pCombinationShapeData != null)
        {
            foreach (CombinationShapeArray pShapeArray in pCombinationShapeData.m_CombinationShapeArrayList)
            {
                if (CalculationShape(pShapeArray.m_pShapeNode) == true)
                {
                    IsSwapAble = true;
                }
            }
        }

        return IsSwapAble;
    }

    private bool RecursiveMatchCheck_Shuffle(Slot pSlot, ShapeNode pShapeNode, eBlockType eType, ref List<Slot> machList, ref int nDepth)
    {
        OutputLog.Log("RecursiveMatchCheck_Shuffle Start");

        if (pSlot.GetShuffleBlockType() == eType && IsObstacle_MatchPossible(pSlot) == true)
        {
            int nNumChild = pShapeNode.GetChildNodeCount();

            for (int i = 0; i < nNumChild; ++i)
            {
                ShapeNode pChild = pShapeNode.GetChildNode_byIndex(i) as ShapeNode;

                Slot pNextSlot = pSlot.GetNeighborSlot(pChild.m_eNeighbor_ToParent);
                if (pNextSlot == null || pNextSlot.GetSlotBlock() == null || (IsNormalBlock(pNextSlot) == false && IsSpecialBlock(pNextSlot) == false))
                {
                    return false;
                }
                else if (pNextSlot != null && pNextSlot.GetSlotBlock() != null && (IsNormalBlock(pNextSlot) == true || IsSpecialBlock(pNextSlot) == true) && pNextSlot.GetShuffleBlockType() == eType)
                {
                    machList.Add(pNextSlot);
                    --nDepth;

                    if(nDepth <= 0)
                        return true;

                    if (RecursiveMatchCheck_Shuffle(pNextSlot, pChild, eType, ref machList, ref nDepth) == false)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        else if (IsObstacle_MatchPossible(pSlot) == false)
        {
            return false;
        }

        OutputLog.Log("RecursiveMatchCheck_Shuffle End");

        return true;
    }

    private bool GetShape_Shuffle(Slot pSlot, ref List<Slot> machList)
    {
        CombinationShapeData pCombinationShapeData = InGameSetting.Instance.m_pInGameSettingData.GetCombinationShapeData(eSpecialItem.None);

        foreach (CombinationShapeArray pCombinationShapeArray in pCombinationShapeData.m_CombinationShapeArrayList)
        {
            if (pCombinationShapeArray.m_IsValid == true)
            {
                int nDepth = 30;
                machList.Clear();
                machList.Add(pSlot);
                if (RecursiveMatchCheck_Shuffle(pSlot, pCombinationShapeArray.m_pShapeNode, pSlot.GetShuffleBlockType(), ref machList, ref nDepth) == true)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void RecursiveShuffle_Init(bool IsFirst, ref List<Slot> machList, int nDepth)
    {
        OutputLog.Log("RecursiveShuffle_Init Start");

        if (IsFirst == true)
        {
            foreach (Slot pSlot in m_ReverseSlotList)
            {
                if (pSlot.GetSlotBlock() != null && pSlot.GetSlotFillWay() != eSlotFillWay.Close && IsObstacle_MatchPossible(pSlot) == true)
                {
                    OutputLog.Log("RecursiveShuffle_Init GetShape_Shuffle 02");

                    machList.Clear();

                    if (GetShape_Shuffle(pSlot, ref machList) == true)
                    {
                        foreach (Slot pMatchSlot in machList)
                        {
                            if (m_SuffleFixSlotList.Contains(pMatchSlot) == false && IsSpecialBlock(pMatchSlot) == false && 
                                IsObstacle_OnlyMineBreak(pMatchSlot) == false && IsObstacle_MatchPossible(pMatchSlot) == true)
                            {
                                pMatchSlot.SetShuffleBlockType(GetRandomBlockType_ExceptBlockType(pMatchSlot.GetShuffleBlockType()));
                            }
                        }

                        --nDepth;
                        if (nDepth > 0)
                        {
                            RecursiveShuffle_Init(true, ref machList, nDepth);
                        }
                        return;
                    }
                }
            }
        }

        OutputLog.Log("RecursiveShuffle_Init End");
    }

    public eShuffleReturn OnShuffle()
    {
        InGameInfo.Instance.m_IsInGameClick = false;

        foreach (Slot pSot in m_SlotList)
        {
            pSot.OnCancelSwapPossibleDirection();
        }

        m_MatchPossibleTable.Clear();
        m_MatchPossibleBlockTypeTable.Clear();
        m_nMatchPossibleTableIndex = 0;
        m_SuffleFixSlotList.Clear();

        if (IsSwapAble() == true)
        {
            Helper.OnSoundPlay("INGAME_BLOCK_SHUFFLE", false);

            foreach (Slot pSlot in m_ReverseSlotList)
            {
                //if (pSlot.GetSlotBlock() != null && (IsNormalBlock(pSlot) == true || IsSpecialBlock(pSlot) == true) && IsObstacle_MatchPossible(pSlot) == true)
                if (pSlot.GetSlotBlock() != null && IsNormalBlock(pSlot) == true && pSlot.GetSlotFixObjectList().Count == 0)
                {
                    //pSlot.SetShuffleBlockType(pSlot.GetSlotBlock().GetBlockType());
                    pSlot.SetShuffleBlockType(GetRandomBlockType());
                }
                else if (pSlot.GetSlotBlock() != null && IsSpecialBlock(pSlot) == true && pSlot.GetSlotFixObjectList().Count == 0)
                {
                    pSlot.SetShuffleBlockType(pSlot.GetSlotBlock().GetBlockType());
                }
                else if (pSlot.GetSlotBlock() != null && pSlot.GetSlotFixObjectList().Count > 0 && IsObstacle_MatchPossible(pSlot) == true)
                {
                    pSlot.SetShuffleBlockType(pSlot.GetSlotBlock().GetBlockType());
                }
            }

            List<KeyValuePair<Slot, bool>> backup_selectMatchPossibleGroup = null;
            eBlockType backup_eMatchBlockType = eBlockType.Empty;

            OutputLog.Log("-------------------------------------------");

            List<int> indexList = new List<int>();

            foreach (KeyValuePair<int,eBlockType> item in m_MatchPossibleBlockTypeTable)
            {
                indexList.Add(item.Key);
            }

            while (m_nMatchPossibleTableIndex > 0)
            {
                m_SuffleFixSlotList.Clear();

                OutputLog.Log("while (m_nMatchPossibleTableIndex > 0) : " + m_nMatchPossibleTableIndex + "," + m_MatchPossibleTable.Count);

                int nRandom = UnityEngine.Random.Range(0, m_nMatchPossibleTableIndex);
                nRandom = indexList[nRandom];
                List<KeyValuePair<Slot, bool>> selectMatchPossibleGroup = m_MatchPossibleTable[nRandom];
                eBlockType eMatchBlockType = m_MatchPossibleBlockTypeTable[nRandom];

                backup_selectMatchPossibleGroup = selectMatchPossibleGroup;
                backup_eMatchBlockType = eMatchBlockType;

                if (eMatchBlockType == eBlockType.Empty)
                {
                    eMatchBlockType = GetRandomBlockType();
                }
                eBlockType eNoMatchBlockType = GetRandomBlockType_ExceptBlockType(eMatchBlockType);

                foreach (KeyValuePair<Slot, bool> item in selectMatchPossibleGroup)
                {
                    m_SuffleFixSlotList.Add(item.Key);

                    if (item.Value == true)
                    {
                        OutputLog.Log("OnShuffle Select True : " + item.Key.GetX() + "," + item.Key.GetY() + ", " + eMatchBlockType);

                        item.Key.SetShuffleBlockType(eMatchBlockType);
                    }
                    else
                    {
                        OutputLog.Log("OnShuffle Select False : " + item.Key.GetX() + "," + item.Key.GetY() + " , " + eNoMatchBlockType);

                        item.Key.SetShuffleBlockType(eNoMatchBlockType);
                    }
                }

                OutputLog.Log("--------------------------------- while");

                List<Slot> matchList = new List<Slot>();
                RecursiveShuffle_Init(true, ref matchList, 10);

				bool IsNext = false;
                foreach (Slot pSlot in m_ReverseSlotList)
                {
                    if (pSlot != null && pSlot.GetSlotBlock() != null && IsObstacle_MatchPossible(pSlot) == true)
                    {
                        Dictionary<int, Slot> MatchSlotTable = new Dictionary<int, Slot>();
                        Slot pCombinationSlot = m_pCombinationSlot;
                        int nIndex = GetShape_Shuffle(pSlot, ref MatchSlotTable, ref pCombinationSlot);

                        if (nIndex != -1)
                        {
                            --m_nMatchPossibleTableIndex;
                            m_MatchPossibleTable.Remove(nRandom);
                            m_MatchPossibleBlockTypeTable.Remove(nRandom);
                            indexList.Remove(nRandom);
                            IsNext = true;

                            break;
                        }
                    }
				}

				if (IsNext == true)
					continue;

				foreach (Slot pSlot in m_ReverseSlotList)
                {
                    if (pSlot.GetSlotBlock() != null && IsNormalBlock(pSlot) == true && IsSpecialBlock(pSlot) == false && 
                        IsObstacle_OnlyMineBreak(pSlot) == false && IsObstacle_MatchPossible(pSlot) == true)
                    {
                        pSlot.OnShuffleBackup();
                        pSlot.ChangeSlotBlock(pSlot.GetShuffleBlockType());
                        pSlot.OnShuffle();
                    }
                }

                return eShuffleReturn.Valid;
            }

            OutputLog.Log("OnShuffle  : ValidAndCheckRemove");

            m_SuffleFixSlotList.Clear();

            // 여기다 예외 상황 
            if (backup_eMatchBlockType == eBlockType.Empty)
            {
                backup_eMatchBlockType = GetRandomBlockType();
            }
            eBlockType ebackup_NoMatchBlockType = GetRandomBlockType_ExceptBlockType(backup_eMatchBlockType);

            foreach (KeyValuePair<Slot, bool> item in backup_selectMatchPossibleGroup)
            {
                m_SuffleFixSlotList.Add(item.Key);

                if (item.Value == true)
                {
                    OutputLog.Log("OnShuffle Select True : " + item.Key.GetX() + "," + item.Key.GetY());

                    item.Key.SetShuffleBlockType(backup_eMatchBlockType);
                }
                else
                {
                    OutputLog.Log("OnShuffle Select False : " + item.Key.GetX() + "," + item.Key.GetY());

                    item.Key.SetShuffleBlockType(ebackup_NoMatchBlockType);
                }
            }

            OutputLog.Log("--------------------------------- backup");

            List<Slot> backup_matchList = new List<Slot>();
            RecursiveShuffle_Init(true, ref backup_matchList, 10);

            foreach (Slot pSlot in m_ReverseSlotList)
            {
                if (pSlot.GetSlotBlock() != null && IsNormalBlock(pSlot) == true && IsSpecialBlock(pSlot) == false && 
                    IsObstacle_OnlyMineBreak(pSlot) == false && IsObstacle_MatchPossible(pSlot) == true)
                {
                    pSlot.OnShuffleBackup();
                    pSlot.ChangeSlotBlock(pSlot.GetShuffleBlockType());
                    pSlot.OnShuffle();
                }
            }

            return eShuffleReturn.ValidAndCheckRemove;
        }

        return eShuffleReturn.InValid;
    }

    private int GetShape_Shuffle(Slot pSlot, ref Dictionary<int, Slot> MatchSlotTable, ref Slot pCombinationSlot)
    {
        CombinationShapeData pPrevCombinationShapeData = null;

        CombinationShapeData pCombinationShapeData = InGameSetting.Instance.m_pInGameSettingData.GetCombinationShapeData(eSpecialItem.None);

        if (pPrevCombinationShapeData == null || pPrevCombinationShapeData.m_nSearchOrder < pCombinationShapeData.m_nSearchOrder)
        {
            foreach (CombinationShapeArray pCombinationShapeArray in pCombinationShapeData.m_CombinationShapeArrayList)
            {
                Dictionary<int, Slot> matchSlotTable = new Dictionary<int, Slot>();
                Slot combinationSlot = m_pCombinationSlot_Shape;

                if (pCombinationShapeArray.m_IsValid == true)
                {
                    if (MatchCheck_Shuffle(pSlot, ref matchSlotTable, ref combinationSlot, pCombinationShapeArray) == true)
                    {
                        pPrevCombinationShapeData = pCombinationShapeData;
                        MatchSlotTable.Clear();
                        MatchSlotTable = matchSlotTable;
                        pCombinationSlot = combinationSlot;
                    }
                }
            }
        }

        return pPrevCombinationShapeData != null ? pPrevCombinationShapeData.m_nIndex : -1;
    }

    private bool MatchCheck_Shuffle(Slot pSlot, ref Dictionary<int, Slot> MatchSlotTable, ref Slot pCombinationSlot, CombinationShapeArray pCombinationShapeArray)
    {
        MatchSlotTable.Add(pSlot.GetSlotIndex(), pSlot);

        if (pSlot != null && pSlot.GetSlotBlock() != null && RecursiveMatchCheck_Shuffle(pSlot, ref MatchSlotTable, pCombinationShapeArray.m_pShapeNode, pSlot.GetShuffleBlockType()) == false)
            return false;

        List<Slot> MoveSlotList = new List<Slot>();
        List<Slot> AllSlotList = new List<Slot>();
        foreach (KeyValuePair<int, Slot> item in MatchSlotTable)
        {
            if (item.Value.GetSlotBlock() != null)
            {
                if (item.Value.IsMoveFlag() == true && item.Value.GetSlotBlock().GetSpecialItem() == eSpecialItem.None)
                {
                    MoveSlotList.Add(item.Value);
                }

                if (item.Value.GetSlotBlock().GetSpecialItem() == eSpecialItem.None)
                {
                    AllSlotList.Add(item.Value);
                }
            }
        }

        if (MoveSlotList.Count == 0 && AllSlotList.Count != 0)
        {
            Helper.ShuffleList<Slot>(AllSlotList);
            pCombinationSlot = AllSlotList[0];
        }
        else if (MoveSlotList.Count != 0)
        {
            Helper.ShuffleList<Slot>(MoveSlotList);
            pCombinationSlot = MoveSlotList[0];
        }

        return true;
    }

    private bool RecursiveMatchCheck_Shuffle(Slot pSlot, ref Dictionary<int, Slot> MatchSlotTable, ShapeNode pShapeNode, eBlockType eType)
    {
        if (pSlot != null && pSlot.GetShuffleBlockType() == eType && IsCombinationBlock(pSlot) == true && IsObstacle_MatchPossible(pSlot) == true)
        {
            int nNumChild = pShapeNode.GetChildNodeCount();

            for (int i = 0; i < nNumChild; ++i)
            {
                ShapeNode pChild = pShapeNode.GetChildNode_byIndex(i) as ShapeNode;

                Slot pNextSlot = pSlot.GetNeighborSlot(pChild.m_eNeighbor_ToParent);
                if (pNextSlot == null || pNextSlot.GetSlotBlock() == null || pNextSlot.GetShuffleBlockType() != eType)
                {
                    if (pChild.m_eShapeData == eShapeData.Normal)
                    {
                        return false;
                    }
                }
                else if (pNextSlot != null && pNextSlot.GetShuffleBlockType() == eType)
                {
                    MatchSlotTable.Add(pNextSlot.GetSlotIndex(), pNextSlot);

                    if (RecursiveMatchCheck_Shuffle(pNextSlot, ref MatchSlotTable, pChild, eType) == false)
                    {
                        return false;
                    }
                }
            }
        }
        else if (pSlot != null && (IsCombinationBlock(pSlot) == false || IsObstacle_MatchPossible(pSlot) == false))
        {
            return false;
        }

        return true;
    }
}
