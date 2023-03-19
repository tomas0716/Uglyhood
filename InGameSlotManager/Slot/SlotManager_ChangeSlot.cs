using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SlotManager : MonoBehaviour
{
    private Transformer_Timer   m_pTimer_BlockSwap     = new Transformer_Timer();
    private Slot                m_pSlot_01              = null;
    private Slot                m_pSlot_02              = null;

    public bool IsCheckSlotPossibleMove(Slot pSlot_01, Slot pSlot_02)
    {
        if (pSlot_01.GetSlotBlock() == null)
            return false;

        if (InGameSetting.Instance.m_pInGameSettingData.m_IsPossibleEmptySlotSwap == false)
        {
            if (pSlot_02.GetSlotBlock() == null)
                return false;
        }

        if(pSlot_01.IsMoveFlag() == true || pSlot_02.IsMoveFlag() == true)
            return false;

        if (pSlot_01.IsPossibleMove() == false || pSlot_02.IsPossibleMove() == false)
            return false;

        if (pSlot_01.IsSlotBlockMatchPossible() == false || pSlot_02.IsSlotBlockMatchPossible() == false)
            return false;

        int nInterval_X = Mathf.Abs(pSlot_01.GetX() - pSlot_02.GetX());
        int nInterval_Y = Mathf.Abs(pSlot_01.GetY() - pSlot_02.GetY());
        if (!((nInterval_X == 1 && nInterval_Y == 0) || (nInterval_X == 0 && nInterval_Y == 1)))
            return false;

        return true;
    }

    public bool OnBlockSwap(Slot pSlot_01, Slot pSlot_02)
    {
        OutputLog.Log("SlotManager OnBlockSwap Begin");

        if (IsCheckSlotPossibleMove(pSlot_01, pSlot_02) == false)
        {
            OutputLog.Log("SlotManager OnBlockSwap Not Possible Move");
            return false;
        }

		if (InGameSetting.Instance.m_pInGameSettingData.m_eSpecialItemUseWay == eSpecialItemUseWay.MoveOrClick ||
            InGameSetting.Instance.m_pInGameSettingData.m_eSpecialItemUseWay == eSpecialItemUseWay.MoveOrDoubleClick)
        {
            bool IsSpecialBlock_01 = false;
            bool IsSpecialBlock_02 = false;

            if (pSlot_01.GetSlotBlock() != null && pSlot_01.GetSlotBlock().GetSpecialItem() != eSpecialItem.None)
            {
                IsSpecialBlock_01 = true;
            }

            if (pSlot_02.GetSlotBlock() != null && pSlot_02.GetSlotBlock().GetSpecialItem() != eSpecialItem.None)
            {
                IsSpecialBlock_02 = true;
            }

            if ((IsSpecialBlock_01 == true && IsSpecialBlock_02 == false) || (IsSpecialBlock_01 == false && IsSpecialBlock_02 == true))
            {
                m_pSlot_01 = pSlot_01;
                m_pSlot_02 = pSlot_02;

                OnPossibleBlockSwap(pSlot_01, pSlot_02);

                // 위에서 슬롯의 블럭을 바꿔 주므로 아래 조건식은 반대가 된다.
                Slot pSlot = IsSpecialBlock_01 == true ? pSlot_02 : pSlot_01;

                m_pTimer_BlockSwap.OnReset();
                TransformerEvent eventValue;
                eventValue = new TransformerEvent_Timer(GameDefine.ms_fBlockChnageSlotTime, pSlot);
                m_pTimer_BlockSwap.AddEvent(eventValue);
                m_pTimer_BlockSwap.SetCallback(null, OnDone_PossibleBlockSwapAction_MoveSpecialItem);
                m_pTimer_BlockSwap.OnPlay();

                return true;
            }
        }

         CombinationShapeData pCombinationShapeData = InGameSetting.Instance.m_pInGameSettingData.GetCombinationShapeData(eSpecialItem.Match_Color);
         bool IsPossible_MatchColor_Swap = true;

        if (pCombinationShapeData != null && pCombinationShapeData.m_IsIgnoreBlockType == false)
        {
            IsPossible_MatchColor_Swap = false;
        }

        if (pSlot_01.GetSlotBlock() != null && pSlot_01.GetSlotBlock().GetSpecialItem() != eSpecialItem.None &&
            pSlot_02.GetSlotBlock() != null && pSlot_02.GetSlotBlock().GetSpecialItem() != eSpecialItem.None &&
            IsPossibleCombinationSpecialItemBlockSwap(pSlot_01, pSlot_02) == true)
        {
            m_pSlot_01 = pSlot_01;
            m_pSlot_02 = pSlot_02;

            OnPossibleBlockSwap(pSlot_01, pSlot_02);

            m_pTimer_BlockSwap.OnReset();
            TransformerEvent eventValue;
            eventValue = new TransformerEvent_Timer(GameDefine.ms_fBlockChnageSlotTime);
            m_pTimer_BlockSwap.AddEvent(eventValue);
            m_pTimer_BlockSwap.SetCallback(null, OnDone_PossibleBlockSwapAction_CombinationSpecialItem);
            m_pTimer_BlockSwap.OnPlay();
        }
        else if (pSlot_01.GetSlotBlock() != null && pSlot_02.GetSlotBlock() != null && IsPossible_MatchColor_Swap == true &&
                 ((pSlot_01.GetSlotBlock().GetSpecialItem() == eSpecialItem.Match_Color && pSlot_02.GetSlotBlock().GetSpecialItem() == eSpecialItem.None) ||
                  (pSlot_01.GetSlotBlock().GetSpecialItem() == eSpecialItem.None && pSlot_02.GetSlotBlock().GetSpecialItem() == eSpecialItem.Match_Color)))
        {
            m_pSlot_01 = pSlot_01;
            m_pSlot_02 = pSlot_02;

            OnPossibleBlockSwap(pSlot_01, pSlot_02);

            Slot pSlot = pSlot_01.GetSlotBlock().GetSpecialItem() == eSpecialItem.Match_Color ? pSlot_01 : pSlot_02;

            m_pTimer_BlockSwap.OnReset();
            TransformerEvent eventValue;
            eventValue = new TransformerEvent_Timer(GameDefine.ms_fBlockChnageSlotTime, pSlot);
            m_pTimer_BlockSwap.AddEvent(eventValue);
            m_pTimer_BlockSwap.SetCallback(null, OnDone_PossibleBlockSwapAction_Match_Color);
            m_pTimer_BlockSwap.OnPlay();
        }
        else if (IsPossibleBlockSwap(pSlot_01, pSlot_02) == true || IsPossibleBlockSwap(pSlot_02, pSlot_01) == true)
        {
            OnPossibleBlockSwap(pSlot_01, pSlot_02);

            m_pTimer_BlockSwap.OnReset();
            TransformerEvent eventValue;
            eventValue = new TransformerEvent_Timer(GameDefine.ms_fBlockChnageSlotTime);
            m_pTimer_BlockSwap.AddEvent(eventValue);
            m_pTimer_BlockSwap.SetCallback(null, OnDone_PossibleBlockSwapAction);
            m_pTimer_BlockSwap.OnPlay();
        }
        else
        {
            OutputLog.Log("OnBlockSwap  Not");

            if (pSlot_01.GetSlotFixObjectList().Count != 0 || pSlot_02.GetSlotFixObjectList().Count != 0)
            {
                return false;
            }
                
            Helper.OnSoundPlay("INGAME_BLOCK_SWAP_FAIL", false);
            
            // 이동 불가
            InGameInfo.Instance.m_IsInGameClick = false;

            pSlot_01.InPossibleBlockSwapAction(pSlot_02);
            pSlot_02.InPossibleBlockSwapAction(pSlot_01);

            m_pTimer_BlockSwap.OnReset();
            TransformerEvent eventValue;
            eventValue = new TransformerEvent_Timer(0.3f);
            m_pTimer_BlockSwap.AddEvent(eventValue);
            m_pTimer_BlockSwap.SetCallback(null, OnDone_InPossibleBlockSwapAction);
            m_pTimer_BlockSwap.OnPlay();

            return false;
        }

        return true;
    }

    public bool IsBlockSwap(Slot pSlot_01, Slot pSlot_02)
    {
        OutputLog.Log("SlotManager IsBlockSwap Begin");

        if (IsCheckSlotPossibleMove(pSlot_01, pSlot_02) == false)
        {
            OutputLog.Log("SlotManager IsBlockSwap Not Possible Move");
            return false;
        }

        if (InGameSetting.Instance.m_pInGameSettingData.m_eSpecialItemUseWay == eSpecialItemUseWay.MoveOrClick ||
            InGameSetting.Instance.m_pInGameSettingData.m_eSpecialItemUseWay == eSpecialItemUseWay.MoveOrDoubleClick)
        {
            bool IsSpecialBlock_01 = false;
            bool IsSpecialBlock_02 = false;

            if (pSlot_01.GetSlotBlock() != null && pSlot_01.GetSlotBlock().GetSpecialItem() != eSpecialItem.None)
            {
                IsSpecialBlock_01 = true;
            }

            if (pSlot_02.GetSlotBlock() != null && pSlot_02.GetSlotBlock().GetSpecialItem() != eSpecialItem.None)
            {
                IsSpecialBlock_02 = true;
            }

            if ((IsSpecialBlock_01 == true && IsSpecialBlock_02 == false) || (IsSpecialBlock_01 == false && IsSpecialBlock_02 == true))
            {
                return true;
            }
        }

        CombinationShapeData pCombinationShapeData = InGameSetting.Instance.m_pInGameSettingData.GetCombinationShapeData(eSpecialItem.Match_Color);
        bool IsPossible_MatchColor_Swap = true;

        if (pCombinationShapeData != null && pCombinationShapeData.m_IsIgnoreBlockType == false)
        {
            IsPossible_MatchColor_Swap = false;
        }

        if (pSlot_01.GetSlotBlock() != null && pSlot_01.GetSlotBlock().GetSpecialItem() != eSpecialItem.None &&
            pSlot_02.GetSlotBlock() != null && pSlot_02.GetSlotBlock().GetSpecialItem() != eSpecialItem.None &&
            IsPossibleCombinationSpecialItemBlockSwap(pSlot_01, pSlot_02) == true)
        {
            return true;
        }
        else if (pSlot_01.GetSlotBlock() != null && pSlot_02.GetSlotBlock() != null && IsPossible_MatchColor_Swap == true &&
                 ((pSlot_01.GetSlotBlock().GetSpecialItem() == eSpecialItem.Match_Color && pSlot_02.GetSlotBlock().GetSpecialItem() == eSpecialItem.None) ||
                  (pSlot_01.GetSlotBlock().GetSpecialItem() == eSpecialItem.None && pSlot_02.GetSlotBlock().GetSpecialItem() == eSpecialItem.Match_Color)))
        {
            return true;
        }
        else if (IsPossibleBlockSwap(pSlot_01, pSlot_02) == true || IsPossibleBlockSwap(pSlot_02, pSlot_01) == true)
        {
            return true;
        }

        return false;
    }

    private void OnPossibleBlockSwap(Slot pSlot_01, Slot pSlot_02)
    {
        Helper.OnSoundPlay("INGAME_BLOCK_SWAP_START", false);

        EventDelegateManager.Instance.OnInGame_BeginBlockSwap();

        InGameInfo.Instance.m_IsInGameClick = false;

        pSlot_01.PossibleBlockSwapAction(pSlot_02);
        pSlot_02.PossibleBlockSwapAction(pSlot_01);

        SlotBlock pSlotBlock_01 = pSlot_01.GetSlotBlock();
        SlotBlock pSlotBlock_02 = pSlot_02.GetSlotBlock();

        pSlot_01.SetSlotBlock(pSlotBlock_02);
        pSlot_02.SetSlotBlock(pSlotBlock_01);

        if(pSlotBlock_01 != null)
            pSlotBlock_01.SetSlot(pSlot_02);
        if(pSlotBlock_02 != null)
            pSlotBlock_02.SetSlot(pSlot_01);

        pSlot_01.SetMoveFlag(true);
        pSlot_02.SetMoveFlag(true);

        InGameInfo.Instance.m_IsCheckSlotMove = true;
        m_pMainGame.OnPossibleBlockSwap(pSlot_01, pSlot_02);
    }

    private void OnDone_PossibleBlockSwapAction(TransformerEvent eventValue)
    {
        OnCheckRemoveSlot();
    }

    private void OnDone_PossibleBlockSwapAction_CombinationSpecialItem(TransformerEvent eventValue)
    {
        SpecialItemCombinationData pSpecialItemCombinationData;
        pSpecialItemCombinationData = InGameSetting.Instance.m_pInGameSettingData.GetSpecialItemCombinationData(m_pSlot_01.GetSlotBlock().GetSpecialItem(), m_pSlot_02.GetSlotBlock().GetSpecialItem());

        if (pSpecialItemCombinationData != null && pSpecialItemCombinationData.m_eSpecialItem_Value != eSpecialItem.None)
        {
            if (pSpecialItemCombinationData.m_eSpecialItem_Value != eSpecialItem.Match_Color_Trans)
            {
                m_pSlot_01.GetSlotBlock().ChangeOnlySpecialitemAttribute(eSpecialItem.None);
                RemoveSpecialItemBlock(m_pSlot_02, pSpecialItemCombinationData.m_eSpecialItem_Value, pSpecialItemCombinationData.m_pSpecialItemCustomize, pSpecialItemCombinationData);
                OnCheckRemoveSlot();
            }
            else
            {
                eBlockType eBlockType = m_pSlot_02.GetSlotBlock().GetSpecialItem() == eSpecialItem.Match_Color ? m_pSlot_01.GetSlotBlock().GetBlockType() : m_pSlot_02.GetSlotBlock().GetBlockType();
                RemoveSpecialItemBlock(m_pSlot_02, pSpecialItemCombinationData.m_eSpecialItem_Value, pSpecialItemCombinationData.m_pSpecialItemCustomize, pSpecialItemCombinationData, eBlockType);
            }
        }
    }

    private void OnDone_PossibleBlockSwapAction_MoveSpecialItem(TransformerEvent eventValue)
    {
        Slot pSlot = eventValue.m_pParameta as Slot;

        if (pSlot != null && pSlot.GetSlotBlock().GetSpecialItem() != eSpecialItem.Match_Color)
        {
            RemoveSpecialItemBlock(pSlot);
            OnCheckRemoveSlot(true);
        }
        else if (pSlot != null && pSlot.GetSlotBlock().GetSpecialItem() == eSpecialItem.Match_Color)
        {
            Slot pSlot_Differ = pSlot == m_pSlot_01 ? m_pSlot_02 : m_pSlot_01;
            CombinationShapeData pCombinationShapeData = InGameSetting.Instance.m_pInGameSettingData.GetCombinationShapeData(eSpecialItem.Match_Color);

            if (pCombinationShapeData != null && pSlot_Differ != null && pSlot_Differ.GetSlotBlock() != null)
            {
                RemoveSpecialItemBlock(pSlot, eSpecialItem.Match_Color, pCombinationShapeData.m_pSpecialItemCustomize, pSlot_Differ.GetSlotBlock().GetBlockType());
                OnCheckRemoveSlot(true);
            }
            else
            {
                OnCheckRemoveSlot();
            }
        }
    }

    private void OnDone_PossibleBlockSwapAction_Match_Color(TransformerEvent eventValue)
    {
        Slot pSlot = eventValue.m_pParameta as Slot;

        if (pSlot != null && pSlot.GetSlotBlock().GetSpecialItem() == eSpecialItem.Match_Color)
        {
            Slot pSlot_Differ = pSlot == m_pSlot_01 ? m_pSlot_02 : m_pSlot_01;
            CombinationShapeData pCombinationShapeData = InGameSetting.Instance.m_pInGameSettingData.GetCombinationShapeData(eSpecialItem.Match_Color);

            if (pCombinationShapeData != null && pSlot_Differ != null && pSlot_Differ.GetSlotBlock() != null)
            {
                RemoveSpecialItemBlock(pSlot, eSpecialItem.Match_Color, pCombinationShapeData.m_pSpecialItemCustomize, pSlot_Differ.GetSlotBlock().GetBlockType());
            }
        }
    }

    private void OnDone_InPossibleBlockSwapAction(TransformerEvent eventValue)
    {
        InGameInfo.Instance.m_IsInGameClick = true;
    }

    private bool IsPossibleBlockSwap(Slot pSlot_Pos, Slot pSlot_Check)
    {
        Dictionary<int, List<SlotBlock>> PossibleMoveSlotBlockTable = new Dictionary<int, List<SlotBlock>>();
        if (Calculation_PossibleMoveSlot(pSlot_Pos, pSlot_Check, PossibleMoveSlotBlockTable) == true)
        {
            foreach (KeyValuePair<int, List<SlotBlock>> item in PossibleMoveSlotBlockTable)
            {
                foreach (SlotBlock pSlotBlock in item.Value)
                {
                    if(pSlotBlock.GetSlot() == pSlot_Pos || pSlotBlock.GetSlot() == pSlot_Check)
                        return true;
                }
            }
        }

        return false;
    }

    private bool IsPossibleCombinationSpecialItemBlockSwap(Slot pSlot_01, Slot pSlot_02)
    {
        SpecialItemCombinationData pSpecialItemCombinationData;
        pSpecialItemCombinationData  = InGameSetting.Instance.m_pInGameSettingData.GetSpecialItemCombinationData(pSlot_01.GetSlotBlock().GetSpecialItem(), pSlot_02.GetSlotBlock().GetSpecialItem());

        if (pSpecialItemCombinationData != null && pSpecialItemCombinationData.m_eSpecialItem_Value != eSpecialItem.None)
        {
            return true;
        }

        return false;
    }
}
