using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SlotManager : MonoBehaviour
{
    private Slot m_pCombinationSlot_Shape = null;

    private int GetShape(Slot pSlot, ref Dictionary<int, Slot> MatchSlotTable, ref Slot pCombinationSlot, bool IsCheckSlotBlockMatchInPossibleAtCheckRemovePossible = false)
    {
        CombinationShapeData pPrevCombinationShapeData = null;

        foreach (CombinationShapeData pCombinationShapeData in InGameSetting.Instance.m_pInGameSettingData.m_CombinationShapeList)
        {
            if (pPrevCombinationShapeData == null || pPrevCombinationShapeData.m_nSearchOrder < pCombinationShapeData.m_nSearchOrder)
            {
                foreach (CombinationShapeArray pCombinationShapeArray in pCombinationShapeData.m_CombinationShapeArrayList)
                {
                    Dictionary<int, Slot> matchSlotTable = new Dictionary<int, Slot>();
                    Slot combinationSlot = m_pCombinationSlot_Shape;

                    if (pCombinationShapeArray.m_IsValid == true)
                    {
                        if (MatchCheck(pSlot, ref matchSlotTable, ref combinationSlot, pCombinationShapeArray, IsCheckSlotBlockMatchInPossibleAtCheckRemovePossible) == true)
                        {
                            pPrevCombinationShapeData = pCombinationShapeData;
                            MatchSlotTable.Clear();
                            MatchSlotTable = matchSlotTable;
                            pCombinationSlot = combinationSlot;
                        }
                    }
                }
            }
        }

        return pPrevCombinationShapeData != null ? pPrevCombinationShapeData.m_nIndex : -1;
    }

    private bool MatchCheck(Slot pSlot, ref Dictionary<int, Slot> MatchSlotTable, ref Slot pCombinationSlot, CombinationShapeArray pCombinationShapeArray, bool IsCheckSlotBlockMatchInPossibleAtCheckRemovePossible)
    {
        MatchSlotTable.Add(pSlot.GetSlotIndex(), pSlot);

        if (pSlot != null && pSlot.GetSlotBlock() != null && /*IsObstacle_MatchPossible(pSlot) == false &&*/
            RecursiveMatchCheck(pSlot, ref MatchSlotTable, pCombinationShapeArray.m_pShapeNode, pSlot.GetSlotBlock().GetBlockType(), IsCheckSlotBlockMatchInPossibleAtCheckRemovePossible) == false)
        {
            return false;
        }

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

    private bool RecursiveMatchCheck(Slot pSlot, ref Dictionary<int, Slot> MatchSlotTable, ShapeNode pShapeNode, eBlockType eType, bool IsCheckSlotBlockMatchInPossibleAtCheckRemovePossible)
    {
        if (pSlot != null && pSlot.GetSlotBlock().GetBlockType() == eType && IsCombinationBlock(pSlot) == true &&
            (SlotManager.IsObstacle_MatchPossible(pSlot) == true ||
            (IsCheckSlotBlockMatchInPossibleAtCheckRemovePossible == true && SlotManager.IsObstacle_MatchPossible(pSlot) == false && pSlot.IsSlotBlockMatchInPossibleAtCheckRemovePossible() == true)))
        {
            int nNumChild = pShapeNode.GetChildNodeCount();

            for (int i = 0; i < nNumChild; ++i)
            {
                ShapeNode pChild = pShapeNode.GetChildNode_byIndex(i) as ShapeNode;

                Slot pNextSlot = pSlot.GetNeighborSlot(pChild.m_eNeighbor_ToParent);
                if (pNextSlot == null || pNextSlot.GetSlotBlock() == null || pNextSlot.GetSlotBlock().GetBlockType() != eType || 
                    (IsCheckSlotBlockMatchInPossibleAtCheckRemovePossible == false && IsObstacle_MatchPossible(pNextSlot) == false) ||
                    (IsCheckSlotBlockMatchInPossibleAtCheckRemovePossible == true && IsObstacle_MatchPossible(pNextSlot) == false &&pNextSlot.IsSlotBlockMatchInPossibleAtCheckRemovePossible() == false))
                {
                    if (pChild.m_eShapeData == eShapeData.Normal)
                    {
                        return false;
                    }
                }
                else if (pNextSlot != null && pNextSlot.GetSlotBlock().GetBlockType() == eType)
                {
                    MatchSlotTable.Add(pNextSlot.GetSlotIndex(), pNextSlot);

                    if (RecursiveMatchCheck(pNextSlot, ref MatchSlotTable, pChild, eType, IsCheckSlotBlockMatchInPossibleAtCheckRemovePossible) == false)
                    {
                        return false;
                    }
                }
            }
        }
        else if (pSlot != null && 
                 (IsCombinationBlock(pSlot) == false || (IsCheckSlotBlockMatchInPossibleAtCheckRemovePossible == false && IsObstacle_MatchPossible(pSlot) == false) ||
                 (IsCheckSlotBlockMatchInPossibleAtCheckRemovePossible == true && IsObstacle_MatchPossible(pSlot) == false && pSlot.IsSlotBlockMatchInPossibleAtCheckRemovePossible() == false)))
        {
            return false;
        }

        return true;
    }
}
