using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SlotManager : MonoBehaviour
{
    private Dictionary<int, Slot>   m_SlotDyingTable                            = new Dictionary<int, Slot>();
    private List<Slot>              m_CurrRemoveSpecialSlotDyingList            = new List<Slot>();

    private Dictionary<int, Slot>   m_CombinationTransSpecialSlotTable          = new Dictionary<int, Slot>();      // 특수유닛으로 변하는 슬롯 테이블
    private TransformerCollector    m_pTransformerCollector                     = new TransformerCollector();

    private Transformer_Timer       m_pTimer_CheckNextSpecialSlotRemove_Delay   = new Transformer_Timer();
    private Transformer_Timer       m_pTimer_SpecialSlotComplete                = new Transformer_Timer();

    private List<Slot>              m_MatchSlotList                             = new List<Slot>();
    private Transformer_Timer       m_pTimer_SlotMove                           = new Transformer_Timer();

    private Dictionary<int, Slot>   m_CombinationSlotTable                      = new Dictionary<int, Slot>();
    private GameObject              m_pGameObject_ChainLightning                = null;

    private Slot                    m_pCombinationSlot                          = null;
    private Transformer_Timer       m_pTimer_RemoveEnd                          = new Transformer_Timer();

    private List<Slot>              m_MatchColorTransSlotList                   = new List<Slot>();
    private Transformer_Timer       m_pTimer_MatchColorTransSlot                = new Transformer_Timer();
    public int                      m_nTurnCompleteCheck                        = 0;

    private void Recursive_LinkSlot(Slot pSlot, ref Dictionary<int, Slot> LinkSlotTable)
    {
        Slot pNeighborSlot;
        pNeighborSlot = pSlot.GetNeighborSlot(eNeighbor.Neighbor_10);

        if (pNeighborSlot != null &&
            pNeighborSlot.GetSlotBlock() != null &&
            pNeighborSlot.GetSlotBlock().GetBlockType() == pSlot.GetSlotBlock().GetBlockType() &&
            (SlotManager.IsObstacle_MatchPossible(pNeighborSlot) == true ||
            (SlotManager.IsObstacle_MatchPossible(pNeighborSlot) == false && pNeighborSlot.IsSlotBlockMatchInPossibleAtCheckRemovePossible() == true)))
        {
            if (LinkSlotTable.ContainsKey(pNeighborSlot.GetSlotIndex()) == false)
            {
                LinkSlotTable.Add(pNeighborSlot.GetSlotIndex(), pNeighborSlot);
                Recursive_LinkSlot(pNeighborSlot, ref LinkSlotTable);
            }
        }

        pNeighborSlot = pSlot.GetNeighborSlot(eNeighbor.Neighbor_01);

        if (pNeighborSlot != null &&
            pNeighborSlot.GetSlotBlock() != null &&
            pNeighborSlot.GetSlotBlock().GetBlockType() == pSlot.GetSlotBlock().GetBlockType() &&
            (SlotManager.IsObstacle_MatchPossible(pNeighborSlot) == true ||
            (SlotManager.IsObstacle_MatchPossible(pNeighborSlot) == false && pNeighborSlot.IsSlotBlockMatchInPossibleAtCheckRemovePossible() == true)))
        {
            if (LinkSlotTable.ContainsKey(pNeighborSlot.GetSlotIndex()) == false)
            {
                LinkSlotTable.Add(pNeighborSlot.GetSlotIndex(), pNeighborSlot);
                Recursive_LinkSlot(pNeighborSlot, ref LinkSlotTable);
            }
        }

        pNeighborSlot = pSlot.GetNeighborSlot(eNeighbor.Neighbor_21);

        if (pNeighborSlot != null &&
            pNeighborSlot.GetSlotBlock() != null &&
            pNeighborSlot.GetSlotBlock().GetBlockType() == pSlot.GetSlotBlock().GetBlockType() &&
            (SlotManager.IsObstacle_MatchPossible(pNeighborSlot) == true ||
            (SlotManager.IsObstacle_MatchPossible(pNeighborSlot) == false && pNeighborSlot.IsSlotBlockMatchInPossibleAtCheckRemovePossible() == true)))
        {
            if (LinkSlotTable.ContainsKey(pNeighborSlot.GetSlotIndex()) == false)
            {
                LinkSlotTable.Add(pNeighborSlot.GetSlotIndex(), pNeighborSlot);
                Recursive_LinkSlot(pNeighborSlot, ref LinkSlotTable);
            }
        }

        pNeighborSlot = pSlot.GetNeighborSlot(eNeighbor.Neighbor_12);

        if (pNeighborSlot != null &&
            pNeighborSlot.GetSlotBlock() != null &&
            pNeighborSlot.GetSlotBlock().GetBlockType() == pSlot.GetSlotBlock().GetBlockType() &&
            (SlotManager.IsObstacle_MatchPossible(pNeighborSlot) == true ||
            (SlotManager.IsObstacle_MatchPossible(pNeighborSlot) == false && pNeighborSlot.IsSlotBlockMatchInPossibleAtCheckRemovePossible() == true)))
        {
            if (LinkSlotTable.ContainsKey(pNeighborSlot.GetSlotIndex()) == false)
            {
                LinkSlotTable.Add(pNeighborSlot.GetSlotIndex(), pNeighborSlot);
                Recursive_LinkSlot(pNeighborSlot, ref LinkSlotTable);
            }
        }
    }

    private void OnCheckRemoveSlot(bool IgnoreNothingRemoveSlot = false)
    {
        OutputLog.Log("SlotManager : OnCheckRemoveSlot Begin");

        m_MatchSlotList.Clear();
        m_SlotDyingTable.Clear();
        m_CurrRemoveSpecialSlotDyingList.Clear();

        m_CombinationTransSpecialSlotTable.Clear();
        m_CombinationSlotTable.Clear();

        bool IsNothingRemoveSlot = true;
        bool IsRemoveSpecialItem = false;

        Dictionary<int, bool> ArleadyCheckBlockTable = new Dictionary<int, bool>();

        foreach (Slot pSlot in m_ReverseSlotList)
        {
            if (pSlot.GetSlotBlock() != null && pSlot.IsRemoveSchedule() == false && ArleadyCheckBlockTable.ContainsKey(pSlot.GetSlotIndex()) == false &&
                (SlotManager.IsObstacle_MatchPossible(pSlot) == true ||
                (SlotManager.IsObstacle_MatchPossible(pSlot) == false && pSlot.IsSlotBlockMatchInPossibleAtCheckRemovePossible() == true)))
            {
                Dictionary<int, Slot> LinkSlotTable = new Dictionary<int, Slot>();
                LinkSlotTable.Add(pSlot.GetSlotIndex(), pSlot);
                Recursive_LinkSlot(pSlot, ref LinkSlotTable);

                if (LinkSlotTable.Count >= 3)
                {
                    Dictionary<int, Slot> MatchSlotTable = new Dictionary<int, Slot>();
                    Slot pCombinationSlot = m_pCombinationSlot;
                    int nIndex = GetShape(pSlot, ref MatchSlotTable, ref pCombinationSlot, true);
                    if (nIndex != -1)
                    {
                        foreach (KeyValuePair<int, Slot> item in MatchSlotTable)
                        {
                            if (ArleadyCheckBlockTable.ContainsKey(item.Key) == false)
                            {
                                ArleadyCheckBlockTable.Add(item.Key, true);
                            }
                        }

                        if (MatchBreak(nIndex, pSlot, MatchSlotTable, pCombinationSlot) == true)
                            IsRemoveSpecialItem = true;

                        m_pMainGame.OnMatchSlot(MatchSlotTable);

                        foreach (KeyValuePair<int, Slot> item in MatchSlotTable)
                        {
                            if (m_MatchSlotList.Contains(item.Value) == false)
                            {
                                m_MatchSlotList.Add(item.Value);
                            }
                        }

                        IsNothingRemoveSlot = false;
                    }
                }
            }
        }

        if (IsNothingRemoveSlot == false && IsRemoveSpecialItem == false)
        {
            OutputLog.Log("SlotManager : OnCheckRemoveSlot OnDieBlock");

            OnDieBlock();
        }

        if (IsNothingRemoveSlot == true && IgnoreNothingRemoveSlot == false)
        {
            OutputLog.Log("SlotManager : OnCheckRemoveSlot OnNothingRemoveSlot");
            OnNothingRemoveSlot();
        }

        OutputLog.Log("SlotManager : OnCheckRemoveSlot End");
    }

    public bool MatchBreak(int nIndex, Slot pSlot, Dictionary<int, Slot> MatchSlotTable, Slot pCombinationSlot)
    {
        int nType = (int)pSlot.GetSlotBlock().GetBlockType();
        CombinationShapeData pCombinationShapeData = InGameSetting.Instance.m_pInGameSettingData.m_CombinationShapeList[nIndex];

        if ((nType >= GameDefine.ms_nBlockStart && nType < GameDefine.ms_nBlockStart + InGameSetting.Instance.m_pInGameSettingData.m_nBlockCount) && pCombinationShapeData.m_pGameObject[nType] != null)
        {
            pCombinationSlot.ChangeBlockShape_byMatch(pCombinationShapeData.m_eSpcialItem);

            if (pCombinationShapeData.m_IsIgnoreBlockType == true)
            {
                pCombinationSlot.GetSlotBlock().ChangeBlockType(eBlockType.MatchColor);
            }

            if (m_CombinationTransSpecialSlotTable.ContainsKey(pCombinationSlot.GetSlotIndex()) == false)
            {
                m_CombinationTransSpecialSlotTable.Add(pCombinationSlot.GetSlotIndex(), pCombinationSlot);
            }
        }

        bool IsRemoveSpecialItem = false;
        foreach (KeyValuePair<int, Slot> item in MatchSlotTable)
        {
            if (item.Value.GetSlotBlock() != null &&
                (((nType >= GameDefine.ms_nBlockStart && nType < GameDefine.ms_nBlockStart + InGameSetting.Instance.m_pInGameSettingData.m_nBlockCount) && pCombinationShapeData.m_pGameObject[nType] == null)
                || item.Value != pCombinationSlot))
            {
                if (item.Value.GetSlotBlock().GetSpecialItem() != eSpecialItem.None)
                {
                    SlotFixObject_Obstacle pObstacle_OnlyMineBreak = item.Value.FindFixObject_SlotDyingAtOnlyMineBreak();

                    if (pObstacle_OnlyMineBreak == null)
                    {
                        IsRemoveSpecialItem = true;
                        RemoveSpecialItemBlock(item.Value);
                    }
                    else
                    {
                        item.Value.SetSlotDying(true);
                    }
                }
                else
                {
                    item.Value.SetSlotDying(true);
                }

            }
        }

        return IsRemoveSpecialItem;
    }

    public void RemoveSpecialItemBlock(Slot pSlot)
    {
        if (pSlot.GetSlotBlock() != null && pSlot.GetSlotBlock().GetSpecialItem() != eSpecialItem.None)
        {
            eSpecialItem eItem = pSlot.GetSlotBlock().GetSpecialItem();
            CombinationShapeData pCombinationShapeData = InGameSetting.Instance.m_pInGameSettingData.GetCombinationShapeData(eItem);

            if (pCombinationShapeData != null)
            {
                RemoveSpecialItemBlock(pSlot, eItem, pCombinationShapeData.m_pSpecialItemCustomize);
            }
        }
    }

    public Dictionary<int, Slot> GetCombinationTransSpecialSlotTable()
    {
        return m_CombinationTransSpecialSlotTable;
    }

    public void OnDieBlock()
    {
        m_pTimer_RemoveEnd.OnReset();

        TransformerEvent eventValue;
        eventValue = new TransformerEvent_Timer(GameDefine.ms_fSpecialItemBlockActionTime);
        m_pTimer_RemoveEnd.AddEvent(eventValue);
        m_pTimer_RemoveEnd.SetCallback(null, OnDone_Timer_RemoveEnd);
        m_pTimer_RemoveEnd.OnPlay();
    }

    private void RemoveSpecialItemBlock(Slot pSlot, eSpecialItem eSpcialItem, SpecialItemCustomize pSpecialItemCustomize, eBlockType eBlockType_Match_Color = eBlockType.Empty)
    {
        SlotFixObject_Obstacle pObstacle_OnlyMineBreak = pSlot.FindFixObject_SlotDyingAtOnlyMineBreak();

        if (pObstacle_OnlyMineBreak == null)
        {
            RemoveSpecialItemBlock(pSlot, eSpcialItem, pSpecialItemCustomize, null, eBlockType_Match_Color);
        }
        else
        {
            pSlot.SetSlotDying(true);
        }
    }

    private void RemoveSpecialItemBlock(Slot pSlot, eSpecialItem eSpcialItem, SpecialItemCustomize pSpecialItemCustomize, SpecialItemCombinationData pSpecialItemCombinationData, eBlockType eBlockType_Match_Color = eBlockType.Empty)
    {
        m_pTimer_RemoveEnd.OnReset();

        if (pSpecialItemCustomize.m_nNumGrid == 0)
        {
            switch (eSpcialItem)
            {
                case eSpecialItem.Hor: RemoveSpecialItemBlock_Hor(pSlot); break;
                case eSpecialItem.Ver: RemoveSpecialItemBlock_Ver(pSlot); break;
                case eSpecialItem.Plus_B5: RemoveSpecialItemBlock_Plus_B5(pSlot); break;
                case eSpecialItem.Plus_L1: RemoveSpecialItemBlock_Plus_L1(pSlot); break;
                case eSpecialItem.Plus_L3: RemoveSpecialItemBlock_Plus_L3(pSlot); break;
                case eSpecialItem.Plus_L5: RemoveSpecialItemBlock_Plus_L5(pSlot); break;
                case eSpecialItem.Match_Color: RemoveSpecialItemBlock_Match_Color(pSlot, eBlockType_Match_Color); break;
                case eSpecialItem.Match_Color_Trans: RemoveSpecialItemBlock_Match_Color_Trans(pSlot, eBlockType_Match_Color, pSpecialItemCombinationData); break;
                case eSpecialItem.All_Block1: RemoveSpecialItemBlock_All_Block1(pSlot); break;
            }
        }
        else
        {
            RemoveSpecialItemBlock_Customize(pSlot, pSpecialItemCustomize);
        }

        pSlot.SetSlotDying(true);
    }

    private void RemoveSpecialItemBlock_Hor(Slot pSlot)
    {
        float fBlockTimeInterval = 0;

        Transformer_Timer pTimer_SpecialSlotRemoveAction = new Transformer_Timer();
        m_pTransformerCollector.AddTransformer(pTimer_SpecialSlotRemoveAction);

        TransformerEvent eventValue;
        Helper.OnCameraSmallShaking();

        Vector3 vPos = pSlot.GetPosition();
        vPos *= InGameInfo.Instance.m_fInGameScale;
        vPos.z = -(float)ePlaneOrder.FX_Stripe_Block;
        ParticleManager.Instance.LoadParticleSystem("FX_Stripe_Block", vPos).SetScale(InGameInfo.Instance.m_fInGameScale);

        GameEvent pGameEvent = new GameEvent_InGame_Effect_Stripe(pSlot, eSpecialItem.Hor);
        AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);

        bool IsLeftBlock = false;
        bool IsRightBlock = false;

        bool IsLeftMatchSlotBlockDie = true;
        bool IsRightMatchSlotBlockDie = true;

        Dictionary<int, Slot> MatchSlotTable = new Dictionary<int, Slot>();
        MatchSlotTable.Add(pSlot.GetSlotIndex(), pSlot);

        for (int i = 1; i < GameDefine.ms_nInGameSlot_X; ++i)
        {
            // left
            if (IsLeftBlock == false)
            {
                int x = pSlot.GetX() - i;
                if (x >= 0 && m_SlotTable.ContainsKey(Helper.GetSlotIndex(x, pSlot.GetY())) == true)
                {
                    Slot pFindSlot = m_SlotTable[Helper.GetSlotIndex(x, pSlot.GetY())];

                    if (m_CombinationTransSpecialSlotTable.ContainsKey(Helper.GetSlotIndex(x, pSlot.GetY())) == false)
                    {
                        if (pFindSlot.IsRemoveSchedule() == false)
                        {
                            pFindSlot.SetRemoveSchedule(true);

                            eventValue = new TransformerEvent_Timer(fBlockTimeInterval, new object[] { pFindSlot, eSpecialItem.Hor });
                            pTimer_SpecialSlotRemoveAction.AddEvent(eventValue);
                        }
                    }

                    if (IsLeftMatchSlotBlockDie == true && pFindSlot.GetSlotBlock() != null)
                    {
                        if (MatchSlotTable.ContainsKey(pFindSlot.GetSlotIndex()) == false && pFindSlot.IsRemoveSchedule() == false)
                            MatchSlotTable.Add(pFindSlot.GetSlotIndex(), pFindSlot);
                    }
                    else
                    {
                        IsLeftMatchSlotBlockDie = false;
                    }
                }
                else
                {
                    IsLeftMatchSlotBlockDie = false;
                }
            }

            // right
            if (IsRightBlock == false)
            {
                int x = pSlot.GetX() + i;
                if (x < GameDefine.ms_nInGameSlot_X && m_SlotTable.ContainsKey(Helper.GetSlotIndex(x, pSlot.GetY())) == true)
                {
                    Slot pFindSlot = m_SlotTable[Helper.GetSlotIndex(x, pSlot.GetY())];

                    if (m_CombinationTransSpecialSlotTable.ContainsKey(Helper.GetSlotIndex(x, pSlot.GetY())) == false)
                    {
                        if (pFindSlot.IsRemoveSchedule() == false)
                        {
                            pFindSlot.SetRemoveSchedule(true);

                            eventValue = new TransformerEvent_Timer(fBlockTimeInterval, new object[] { pFindSlot, eSpecialItem.Hor });
                            pTimer_SpecialSlotRemoveAction.AddEvent(eventValue);
                        }
                    }

                    if (IsRightMatchSlotBlockDie == true && pFindSlot.GetSlotBlock() != null)
                    {
                        if (MatchSlotTable.ContainsKey(pFindSlot.GetSlotIndex()) == false && pFindSlot.IsRemoveSchedule() == false)
                            MatchSlotTable.Add(pFindSlot.GetSlotIndex(), pFindSlot);
                    }
                    else
                    {
                        IsRightMatchSlotBlockDie = false;
                    }
                }
                else
                {
                    IsRightMatchSlotBlockDie = false;
                }
            }

            m_pMainGame.OnMatchSlot(MatchSlotTable, pSlot.GetSlotBlock().GetBlockType());

            foreach (KeyValuePair<int, Slot> item in MatchSlotTable)
            {
                if (m_MatchSlotList.Contains(item.Value) == false)
                {
                    m_MatchSlotList.Add(item.Value);
                }
            }

            fBlockTimeInterval += GameDefine.ms_fSpecialSlotStraight;
        }

        pTimer_SpecialSlotRemoveAction.SetCallback(OnOneEventDone_SpecialSlotRemoveAction, OnDone_SpecialSlotRemoveAction);
        pTimer_SpecialSlotRemoveAction.OnPlay();
    }

    private void RemoveSpecialItemBlock_Ver(Slot pSlot)
    {
        pSlot.SetSlotDying(true);
        pSlot.SetRemoveSchedule(true);

        float fBlockTimeInterval = 0;

        Transformer_Timer pTimer_SpecialSlotRemoveAction = new Transformer_Timer();
        m_pTransformerCollector.AddTransformer(pTimer_SpecialSlotRemoveAction);

        TransformerEvent eventValue;

        Helper.OnCameraSmallShaking();

        Vector3 vPos = pSlot.GetPosition();
        vPos *= InGameInfo.Instance.m_fInGameScale;
        vPos.z = -(float)ePlaneOrder.FX_Stripe_Block;
        ParticleManager.Instance.LoadParticleSystem("FX_Stripe_Block", vPos).SetScale(InGameInfo.Instance.m_fInGameScale);

        GameEvent pGameEvent = new GameEvent_InGame_Effect_Stripe(pSlot, eSpecialItem.Ver);
        AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);

        bool IsUpBlock = false;
        bool IsDownBlock = false;

        bool IsUpMatchSlotBlockDie = true;
        bool IsDownMatchSlotBlockDie = true;

        Dictionary<int, Slot> MatchSlotTable = new Dictionary<int, Slot>();
        MatchSlotTable.Add(pSlot.GetSlotIndex(), pSlot);

        for (int i = 1; i < GameDefine.ms_nInGameSlot_Y; ++i)
        {
            // up
            if (IsUpBlock == false)
            {
                int y = pSlot.GetY() - i;
                if (y >= 0 && m_SlotTable.ContainsKey(Helper.GetSlotIndex(pSlot.GetX(), y)) == true)
                {
                    Slot pFindSlot = m_SlotTable[Helper.GetSlotIndex(pSlot.GetX(), y)];

                    if (m_CombinationTransSpecialSlotTable.ContainsKey(Helper.GetSlotIndex(pSlot.GetX(), y)) == false)
                    {
                        if (pFindSlot.IsRemoveSchedule() == false)
                        {
                            pFindSlot.SetRemoveSchedule(true);

                            eventValue = new TransformerEvent_Timer(fBlockTimeInterval, new object[] { pFindSlot, eSpecialItem.Ver });
                            pTimer_SpecialSlotRemoveAction.AddEvent(eventValue);
                        }
                    }

                    if (IsUpMatchSlotBlockDie == true && pFindSlot.GetSlotBlock() != null)
                    {
                        if (MatchSlotTable.ContainsKey(pFindSlot.GetSlotIndex()) == false && pFindSlot.IsRemoveSchedule() == false)
                            MatchSlotTable.Add(pFindSlot.GetSlotIndex(), pFindSlot);
                    }
                    else
                    {
                        IsUpMatchSlotBlockDie = false;
                    }
                }
                else
                {
                    IsUpMatchSlotBlockDie = false;
                }
            }

            if (IsDownBlock == false)
            {
                // down
                int y = pSlot.GetY() + i;
                if (y < GameDefine.ms_nInGameSlot_Y && m_SlotTable.ContainsKey(Helper.GetSlotIndex(pSlot.GetX(), y)) == true)
                {
                    Slot pFindSlot = m_SlotTable[Helper.GetSlotIndex(pSlot.GetX(), y)];

                    if (m_CombinationTransSpecialSlotTable.ContainsKey(Helper.GetSlotIndex(pSlot.GetX(), y)) == false)
                    {
                        if (pFindSlot.IsRemoveSchedule() == false)
                        {
                            pFindSlot.SetRemoveSchedule(true);

                            eventValue = new TransformerEvent_Timer(fBlockTimeInterval, new object[] { pFindSlot, eSpecialItem.Ver });
                            pTimer_SpecialSlotRemoveAction.AddEvent(eventValue);
                        }
                    }

                    if (IsDownMatchSlotBlockDie == true && pFindSlot.GetSlotBlock() != null)
                    {
                        if (MatchSlotTable.ContainsKey(pFindSlot.GetSlotIndex()) == false && pFindSlot.IsRemoveSchedule() == false)
                            MatchSlotTable.Add(pFindSlot.GetSlotIndex(), pFindSlot);
                    }
                    else
                    {
                        IsDownMatchSlotBlockDie = false;
                    }
                }
                else
                {
                    IsDownMatchSlotBlockDie = false;
                }
            }

            fBlockTimeInterval += GameDefine.ms_fSpecialSlotStraight;
        }

        m_pMainGame.OnMatchSlot(MatchSlotTable, pSlot.GetSlotBlock().GetBlockType());

        foreach (KeyValuePair<int, Slot> item in MatchSlotTable)
        {
            if (m_MatchSlotList.Contains(item.Value) == false)
            {
                m_MatchSlotList.Add(item.Value);
            }
        }

        pTimer_SpecialSlotRemoveAction.SetCallback(OnOneEventDone_SpecialSlotRemoveAction, OnDone_SpecialSlotRemoveAction);
        pTimer_SpecialSlotRemoveAction.OnPlay();
    }

    private void RemoveSpecialItemBlock_Plus_B5(Slot pSlot)
    {
        Transformer_Timer pTimer_SpecialSlotRemoveAction = new Transformer_Timer();
        m_pTransformerCollector.AddTransformer(pTimer_SpecialSlotRemoveAction);

        TransformerEvent eventValue;

        Helper.OnCameraNormalShaking();

        Vector3 vPos = pSlot.GetPosition();
        vPos *= InGameInfo.Instance.m_fInGameScale;
        vPos.z = -(float)ePlaneOrder.FX_Stripe_Block;
        ParticleManager.Instance.LoadParticleSystem("FX_Cross_Normal_" + pSlot.GetSlotBlock().GetBlockType().ToString(), vPos).SetScale(InGameInfo.Instance.m_fInGameScale * 2.8f);

        Slot pFindSlot = null;

        Dictionary<int, Slot> MatchSlotTable = new Dictionary<int, Slot>();
        MatchSlotTable.Add(pSlot.GetSlotIndex(), pSlot);

        int[,] NeighborIndex = { { -1, 0 }, { 0, -1 }, { 1, 0 }, { 0, 1 } };

        for (int i = 0; i < 4; ++i)
        {
            int nSlot_X = pSlot.GetX() + NeighborIndex[i, 0];
            int nSlot_Y = pSlot.GetY() + NeighborIndex[i, 1];

            if (IsValidSlotIndex(nSlot_X, nSlot_Y) == true && m_SlotTable.ContainsKey(Helper.GetSlotIndex(nSlot_X, nSlot_Y)) == true)
            {
                pFindSlot = m_SlotTable[Helper.GetSlotIndex(nSlot_X, nSlot_Y)];

                if (m_CombinationTransSpecialSlotTable.ContainsKey(pFindSlot.GetSlotIndex()) == false)
                {
                    if (pFindSlot.IsRemoveSchedule() == false)
                    {
                        pFindSlot.SetRemoveSchedule(true);

                        eventValue = new TransformerEvent_Timer(GameDefine.ms_fSpecialSlotStraight, new object[] { pFindSlot, eSpecialItem.Plus_B5 });
                        pTimer_SpecialSlotRemoveAction.AddEvent(eventValue);
                    }

                    if (pFindSlot.GetSlotBlock() != null)
                    {
                        if (MatchSlotTable.ContainsKey(pFindSlot.GetSlotIndex()) == false && pFindSlot.IsRemoveSchedule() == false)
                            MatchSlotTable.Add(pFindSlot.GetSlotIndex(), pFindSlot);
                    }
                }
            }
        }

        m_pMainGame.OnMatchSlot(MatchSlotTable, pSlot.GetSlotBlock().GetBlockType());

        foreach (KeyValuePair<int, Slot> item in MatchSlotTable)
        {
            if (m_MatchSlotList.Contains(item.Value) == false)
            {
                m_MatchSlotList.Add(item.Value);
            }
        }

        pTimer_SpecialSlotRemoveAction.SetCallback(OnOneEventDone_SpecialSlotRemoveAction, OnDone_SpecialSlotRemoveAction);
        pTimer_SpecialSlotRemoveAction.OnPlay();
    }

    private void RemoveSpecialItemBlock_Plus_L1(Slot pSlot)
    {
        RemoveSpecialItemBlock_Hor(pSlot);
        RemoveSpecialItemBlock_Ver(pSlot);
    }

    private void RemoveSpecialItemBlock_Plus_L3(Slot pSlot)
    {
        Helper.OnCameraNormalShaking();

        RemoveSpecialItemBlock_Plus_L3_Hor(pSlot);
        RemoveSpecialItemBlock_Plus_L3_Ver(pSlot);
    }

    private void RemoveSpecialItemBlock_Plus_L3_Hor(Slot pSlot)
    {
        GameEvent pGameEvent = new GameEvent_InGame_Effect_Stripe(pSlot, eSpecialItem.Hor);
        AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);

        Dictionary<int, Slot> MatchSlotTable = new Dictionary<int, Slot>();
        MatchSlotTable.Add(pSlot.GetSlotIndex(), pSlot);

        float fBlockTimeInterval = 0;

        Transformer_Timer pTimer_SpecialSlotRemoveAction = new Transformer_Timer();
        m_pTransformerCollector.AddTransformer(pTimer_SpecialSlotRemoveAction);

        TransformerEvent eventValue;
        Vector2 vSlotPos;

        Slot pSlot_Up = pSlot.GetNeighborSlot(eNeighbor.Neighbor_10);
        if (pSlot_Up != null && pSlot_Up.GetSlotBlock() != null && pSlot_Up.IsRemoveSchedule() == false)
        {
            pSlot_Up.SetSlotDying(true);
            pSlot_Up.SetRemoveSchedule(true);
            MatchSlotTable.Add(pSlot_Up.GetSlotIndex(), pSlot_Up);
        }

        if (pSlot.IsTopSide() == false)
        {
            vSlotPos = pSlot.GetPosition();
            vSlotPos.y += InGameInfo.Instance.m_fSlotSize;
            pGameEvent = new GameEvent_InGame_Effect_Stripe(vSlotPos, eSpecialItem.Hor);
            AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
        }

        Slot pSlot_Down = pSlot.GetNeighborSlot(eNeighbor.Neighbor_12);
        if (pSlot_Down != null && pSlot_Down.GetSlotBlock() != null && pSlot_Down.IsRemoveSchedule() == false)
        {
            pSlot_Down.SetSlotDying(true);
            pSlot_Down.SetRemoveSchedule(true);
            MatchSlotTable.Add(pSlot_Down.GetSlotIndex(), pSlot_Down);
        }

        if (pSlot.IsBottomSide() == false)
        {
            vSlotPos = pSlot.GetPosition();
            vSlotPos.y -= InGameInfo.Instance.m_fSlotSize;
            pGameEvent = new GameEvent_InGame_Effect_Stripe(vSlotPos, eSpecialItem.Hor);
            AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
        }

        fBlockTimeInterval += GameDefine.ms_fSpecialSlotStraight * 2;

        for (int x = 1; x < GameDefine.ms_nInGameSlot_X; ++x)
        {
            for (int y = pSlot.GetY() - 1; y <= pSlot.GetY() + 1; ++y)
            {
                // left
                int nSlot_X = pSlot.GetX() - x;
                if (nSlot_X >= 0 && m_SlotTable.ContainsKey(Helper.GetSlotIndex(nSlot_X, y)) == true)
                {
                    Slot pFindSlot = m_SlotTable[Helper.GetSlotIndex(nSlot_X, y)];

                    if (m_CombinationTransSpecialSlotTable.ContainsKey(Helper.GetSlotIndex(nSlot_X, y)) == false)
                    {
                        if (pFindSlot.IsRemoveSchedule() == false)
                        {
                            pFindSlot.SetRemoveSchedule(true);

                            eventValue = new TransformerEvent_Timer(fBlockTimeInterval, new object[] { pFindSlot, eSpecialItem.Plus_L3 });
                            pTimer_SpecialSlotRemoveAction.AddEvent(eventValue);

                            if (MatchSlotTable.ContainsKey(pFindSlot.GetSlotIndex()) == false)
                                MatchSlotTable.Add(pFindSlot.GetSlotIndex(), pFindSlot);
                        }
                    }
                }

                // right
                nSlot_X = pSlot.GetX() + x;
                if (nSlot_X < GameDefine.ms_nInGameSlot_X && m_SlotTable.ContainsKey(Helper.GetSlotIndex(nSlot_X, y)) == true)
                {
                    Slot pFindSlot = m_SlotTable[Helper.GetSlotIndex(nSlot_X, y)];

                    if (m_CombinationTransSpecialSlotTable.ContainsKey(Helper.GetSlotIndex(nSlot_X, y)) == false)
                    {
                        if (pFindSlot.IsRemoveSchedule() == false)
                        {
                            pFindSlot.SetRemoveSchedule(true);

                            eventValue = new TransformerEvent_Timer(fBlockTimeInterval, new object[] { pFindSlot, eSpecialItem.Plus_L3 });
                            pTimer_SpecialSlotRemoveAction.AddEvent(eventValue);

                            if (MatchSlotTable.ContainsKey(pFindSlot.GetSlotIndex()) == false)
                                MatchSlotTable.Add(pFindSlot.GetSlotIndex(), pFindSlot);
                        }
                    }
                }
            }

            fBlockTimeInterval += GameDefine.ms_fSpecialSlotStraight;
        }

        m_pMainGame.OnMatchSlot(MatchSlotTable, pSlot.GetSlotBlock().GetBlockType());

        foreach (KeyValuePair<int, Slot> item in MatchSlotTable)
        {
            if (m_MatchSlotList.Contains(item.Value) == false)
            {
                m_MatchSlotList.Add(item.Value);
            }
        }

        pTimer_SpecialSlotRemoveAction.SetCallback(OnOneEventDone_SpecialSlotRemoveAction, OnDone_SpecialSlotRemoveAction);
        pTimer_SpecialSlotRemoveAction.OnPlay();
    }

    private void RemoveSpecialItemBlock_Plus_L3_Ver(Slot pSlot)
    {
        Vector3 vPos = pSlot.GetPosition();
        vPos *= InGameInfo.Instance.m_fInGameScale;
        vPos.z = -(float)ePlaneOrder.FX_Stripe_Block;
        ParticleManager.Instance.LoadParticleSystem("FX_Stripe_Block", vPos).SetScale(AppInstance.Instance.m_fMainScale * InGameInfo.Instance.m_fInGameScale);

        GameEvent pGameEvent = new GameEvent_InGame_Effect_Stripe(pSlot, eSpecialItem.Ver);
        AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);

        Dictionary<int, Slot> MatchSlotTable = new Dictionary<int, Slot>();
        MatchSlotTable.Add(pSlot.GetSlotIndex(), pSlot);

        float fBlockTimeInterval = 0;

        Transformer_Timer pTimer_SpecialSlotRemoveAction = new Transformer_Timer();
        m_pTransformerCollector.AddTransformer(pTimer_SpecialSlotRemoveAction);

        TransformerEvent eventValue;
        Vector2 vSlotPos;

        Slot pSlot_Left = pSlot.GetNeighborSlot(eNeighbor.Neighbor_01);
        if (pSlot_Left != null && pSlot_Left.GetSlotBlock() != null && pSlot_Left.IsRemoveSchedule() == false)
        {
            pSlot_Left.SetSlotDying(true);
            pSlot_Left.SetRemoveSchedule(true);

            MatchSlotTable.Add(pSlot_Left.GetSlotIndex(), pSlot_Left);
        }

        if (pSlot.IsLeftSide() == false)
        {
            vSlotPos = pSlot.GetPosition();
            vSlotPos.x -= InGameInfo.Instance.m_fSlotSize;
            pGameEvent = new GameEvent_InGame_Effect_Stripe(vSlotPos, eSpecialItem.Ver);
            AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
        }

        Slot pSlot_Right = pSlot.GetNeighborSlot(eNeighbor.Neighbor_21);
        if (pSlot_Right != null && pSlot_Right.GetSlotBlock() != null && pSlot_Right.IsRemoveSchedule() == false)
        {
            pSlot_Right.SetSlotDying(true);
            pSlot_Right.SetRemoveSchedule(true);

            MatchSlotTable.Add(pSlot_Right.GetSlotIndex(), pSlot_Right);
        }

        if (pSlot.IsRightSide() == false)
        {
            vSlotPos = pSlot.GetPosition();
            vSlotPos.x += InGameInfo.Instance.m_fSlotSize;
            pGameEvent = new GameEvent_InGame_Effect_Stripe(vSlotPos, eSpecialItem.Ver);
            AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
        }

        for (int y = 1; y < GameDefine.ms_nInGameSlot_Y; ++y)
        {
            for (int x = pSlot.GetX() - 1; x <= pSlot.GetX() + 1; ++x)
            {
                if (x < 0 || x > InGameInfo.Instance.m_nSlotEndIndex_X)
                    continue;

                // Up
                int nSlot_Y = pSlot.GetY() - y;
                if (nSlot_Y >= 0 && m_SlotTable.ContainsKey(Helper.GetSlotIndex(x, nSlot_Y)) == true)
                {
                    Slot pFindSlot = m_SlotTable[Helper.GetSlotIndex(x, nSlot_Y)];

                    if (m_CombinationTransSpecialSlotTable.ContainsKey(Helper.GetSlotIndex(x, nSlot_Y)) == false)
                    {
                        if (pFindSlot.IsRemoveSchedule() == false)
                        {
                            pFindSlot.SetRemoveSchedule(true);

                            eventValue = new TransformerEvent_Timer(fBlockTimeInterval, new object[] { pFindSlot, eSpecialItem.Plus_L3 });
                            pTimer_SpecialSlotRemoveAction.AddEvent(eventValue);

                            if (MatchSlotTable.ContainsKey(pFindSlot.GetSlotIndex()) == false)
                                MatchSlotTable.Add(pFindSlot.GetSlotIndex(), pFindSlot);
                        }
                    }
                }

                // Down
                nSlot_Y = pSlot.GetY() + y;
                if (nSlot_Y < GameDefine.ms_nInGameSlot_Y && m_SlotTable.ContainsKey(Helper.GetSlotIndex(x, nSlot_Y)) == true)
                {
                    Slot pFindSlot = m_SlotTable[Helper.GetSlotIndex(x, nSlot_Y)];

                    if (m_CombinationTransSpecialSlotTable.ContainsKey(Helper.GetSlotIndex(x, nSlot_Y)) == false)
                    {
                        if (pFindSlot.IsRemoveSchedule() == false)
                        {
                            pFindSlot.SetRemoveSchedule(true);

                            eventValue = new TransformerEvent_Timer(fBlockTimeInterval, new object[] { pFindSlot, eSpecialItem.Plus_L3 });
                            pTimer_SpecialSlotRemoveAction.AddEvent(eventValue);

                            if (MatchSlotTable.ContainsKey(pFindSlot.GetSlotIndex()) == false)
                                MatchSlotTable.Add(pFindSlot.GetSlotIndex(), pFindSlot);
                        }
                    }
                }
            }
            fBlockTimeInterval += GameDefine.ms_fSpecialSlotStraight;
        }

        m_pMainGame.OnMatchSlot(MatchSlotTable, pSlot.GetSlotBlock().GetBlockType());

        foreach (KeyValuePair<int, Slot> item in MatchSlotTable)
        {
            if (m_MatchSlotList.Contains(item.Value) == false)
            {
                m_MatchSlotList.Add(item.Value);
            }
        }

        pTimer_SpecialSlotRemoveAction.SetCallback(OnOneEventDone_SpecialSlotRemoveAction, OnDone_SpecialSlotRemoveAction);
        pTimer_SpecialSlotRemoveAction.OnPlay();
    }

    private void RemoveSpecialItemBlock_Plus_L5(Slot pSlot)
    {
        RemoveSpecialItemBlock_Plus_L5_Hor(pSlot);
        RemoveSpecialItemBlock_Plus_L5_Ver(pSlot);
    }

    private void RemoveSpecialItemBlock_Plus_L5_Hor(Slot pSlot)
    {
        GameEvent pGameEvent = new GameEvent_InGame_Effect_Stripe(pSlot, eSpecialItem.Hor);
        AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);

        Dictionary<int, Slot> MatchSlotTable = new Dictionary<int, Slot>();
        MatchSlotTable.Add(pSlot.GetSlotIndex(), pSlot);

        float fBlockTimeInterval = 0;

        Transformer_Timer pTimer_SpecialSlotRemoveAction = new Transformer_Timer();
        m_pTransformerCollector.AddTransformer(pTimer_SpecialSlotRemoveAction);

        TransformerEvent eventValue;
        Vector2 vSlotPos;

        Slot pSlot_Up = pSlot.GetNeighborSlot(eNeighbor.Neighbor_10);
        if (pSlot_Up != null && pSlot_Up.GetSlotBlock() != null && pSlot_Up.IsRemoveSchedule() == false)
        {
            pSlot_Up.SetSlotDying(true);
            pSlot_Up.SetRemoveSchedule(true);

            MatchSlotTable.Add(pSlot_Up.GetSlotIndex(), pSlot_Up);
        }

        if (pSlot.IsTopSide() == false)
        {
            vSlotPos = pSlot.GetPosition();
            vSlotPos.y += InGameInfo.Instance.m_fSlotSize;
            pGameEvent = new GameEvent_InGame_Effect_Stripe(vSlotPos, eSpecialItem.Hor);
            AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
        }

        if (pSlot_Up != null)
        {
            Slot pSlot_Up_Up = pSlot_Up.GetNeighborSlot(eNeighbor.Neighbor_10);
            if (pSlot_Up_Up != null && pSlot_Up_Up.GetSlotBlock() != null && pSlot_Up_Up.IsRemoveSchedule() == false)
            {
                pSlot_Up_Up.SetSlotDying(true);
                pSlot_Up_Up.SetRemoveSchedule(true);

                MatchSlotTable.Add(pSlot_Up_Up.GetSlotIndex(), pSlot_Up_Up);
            }
        }

        if (pSlot.IsTopSide() == false && pSlot.IsTopSecondSide() == false)
        {
            vSlotPos = pSlot.GetPosition();
            vSlotPos.y += InGameInfo.Instance.m_fSlotSize * 2;
            pGameEvent = new GameEvent_InGame_Effect_Stripe(vSlotPos, eSpecialItem.Hor);
            AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
        }

        Slot pSlot_Down = pSlot.GetNeighborSlot(eNeighbor.Neighbor_12);
        if (pSlot_Down != null && pSlot_Down.GetSlotBlock() != null && pSlot_Down.IsRemoveSchedule() == false)
        {
            pSlot_Down.SetSlotDying(true);
            pSlot_Down.SetRemoveSchedule(true);

            MatchSlotTable.Add(pSlot_Down.GetSlotIndex(), pSlot_Down);
        }

        if (pSlot.IsBottomSide() == false)
        {
            vSlotPos = pSlot.GetPosition();
            vSlotPos.y -= InGameInfo.Instance.m_fSlotSize;
            pGameEvent = new GameEvent_InGame_Effect_Stripe(vSlotPos, eSpecialItem.Hor);
            AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
        }

        if (pSlot_Down != null)
        {
            Slot pSlot_Down_Down = pSlot_Down.GetNeighborSlot(eNeighbor.Neighbor_12);
            if (pSlot_Down_Down != null && pSlot_Down_Down.GetSlotBlock() != null && pSlot_Down_Down.IsRemoveSchedule() == false)
            {
                pSlot_Down_Down.SetSlotDying(true);
                pSlot_Down_Down.SetRemoveSchedule(true);

                MatchSlotTable.Add(pSlot_Down_Down.GetSlotIndex(), pSlot_Down_Down);
            }
        }

        if (pSlot.IsBottomSide() == false && pSlot.IsBottomSecondSide() == false)
        {
            vSlotPos = pSlot.GetPosition();
            vSlotPos.y -= InGameInfo.Instance.m_fSlotSize * 2;
            pGameEvent = new GameEvent_InGame_Effect_Stripe(vSlotPos, eSpecialItem.Hor);
            AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
        }

        fBlockTimeInterval += GameDefine.ms_fSpecialSlotStraight * 2;

        for (int x = 1; x < GameDefine.ms_nInGameSlot_X; ++x)
        {
            for (int y = pSlot.GetY() - 2; y <= pSlot.GetY() + 2; ++y)
            {
                // left
                int nSlot_X = pSlot.GetX() - x;
                if (nSlot_X >= 0 && m_SlotTable.ContainsKey(Helper.GetSlotIndex(nSlot_X, y)) == true)
                {
                    Slot pFindSlot = m_SlotTable[Helper.GetSlotIndex(nSlot_X, y)];

                    if (m_CombinationTransSpecialSlotTable.ContainsKey(Helper.GetSlotIndex(nSlot_X, y)) == false)
                    {
                        if (pFindSlot.IsRemoveSchedule() == false)
                        {
                            pFindSlot.SetRemoveSchedule(true);

                            eventValue = new TransformerEvent_Timer(fBlockTimeInterval, new object[] { pFindSlot, eSpecialItem.Plus_L5 });
                            pTimer_SpecialSlotRemoveAction.AddEvent(eventValue);

                            if (MatchSlotTable.ContainsKey(pFindSlot.GetSlotIndex()) == false)
                                MatchSlotTable.Add(pFindSlot.GetSlotIndex(), pFindSlot);
                        }
                    }
                }

                // right
                nSlot_X = pSlot.GetX() + x;
                if (nSlot_X < GameDefine.ms_nInGameSlot_X && m_SlotTable.ContainsKey(Helper.GetSlotIndex(nSlot_X, y)) == true)
                {
                    Slot pFindSlot = m_SlotTable[Helper.GetSlotIndex(nSlot_X, y)];

                    if (m_CombinationTransSpecialSlotTable.ContainsKey(Helper.GetSlotIndex(nSlot_X, y)) == false)
                    {
                        if (pFindSlot.IsRemoveSchedule() == false)
                        {
                            pFindSlot.SetRemoveSchedule(true);

                            eventValue = new TransformerEvent_Timer(fBlockTimeInterval, new object[] { pFindSlot, eSpecialItem.Plus_L5 });
                            pTimer_SpecialSlotRemoveAction.AddEvent(eventValue);

                            if (MatchSlotTable.ContainsKey(pFindSlot.GetSlotIndex()) == false)
                                MatchSlotTable.Add(pFindSlot.GetSlotIndex(), pFindSlot);
                        }
                    }
                }
            }

            fBlockTimeInterval += GameDefine.ms_fSpecialSlotStraight;
        }

        m_pMainGame.OnMatchSlot(MatchSlotTable, pSlot.GetSlotBlock().GetBlockType());

        foreach (KeyValuePair<int, Slot> item in MatchSlotTable)
        {
            if (m_MatchSlotList.Contains(item.Value) == false)
            {
                m_MatchSlotList.Add(item.Value);
            }
        }

        pTimer_SpecialSlotRemoveAction.SetCallback(OnOneEventDone_SpecialSlotRemoveAction, OnDone_SpecialSlotRemoveAction);
        pTimer_SpecialSlotRemoveAction.OnPlay();
    }

    private void RemoveSpecialItemBlock_Plus_L5_Ver(Slot pSlot)
    {
        Vector3 vPos = pSlot.GetPosition();
        vPos *= InGameInfo.Instance.m_fInGameScale;
        vPos.z = -(float)ePlaneOrder.FX_Stripe_Block;
        ParticleManager.Instance.LoadParticleSystem("FX_Stripe_Block", vPos).SetScale(AppInstance.Instance.m_fMainScale * InGameInfo.Instance.m_fInGameScale);

        GameEvent pGameEvent = new GameEvent_InGame_Effect_Stripe(pSlot, eSpecialItem.Ver);
        AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);

        Dictionary<int, Slot> MatchSlotTable = new Dictionary<int, Slot>();
        MatchSlotTable.Add(pSlot.GetSlotIndex(), pSlot);

        float fBlockTimeInterval = 0;

        Transformer_Timer pTimer_SpecialSlotRemoveAction = new Transformer_Timer();
        m_pTransformerCollector.AddTransformer(pTimer_SpecialSlotRemoveAction);

        TransformerEvent eventValue;
        Vector2 vSlotPos;

        Slot pSlot_Left = pSlot.GetNeighborSlot(eNeighbor.Neighbor_01);
        if (pSlot_Left != null && pSlot_Left.GetSlotBlock() != null && pSlot_Left.IsRemoveSchedule() == false)
        {
            pSlot_Left.SetSlotDying(true);
            pSlot_Left.SetRemoveSchedule(true);

            MatchSlotTable.Add(pSlot_Left.GetSlotIndex(), pSlot_Left);
        }

        if (pSlot.IsLeftSide() == false)
        {
            vSlotPos = pSlot.GetPosition();
            vSlotPos.x -= InGameInfo.Instance.m_fSlotSize;
            pGameEvent = new GameEvent_InGame_Effect_Stripe(vSlotPos, eSpecialItem.Ver);
            AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
        }

        if (pSlot_Left != null)
        {
            Slot pSlot_Left_Left = pSlot_Left.GetNeighborSlot(eNeighbor.Neighbor_01);
            if (pSlot_Left_Left != null && pSlot_Left_Left.GetSlotBlock() != null && pSlot_Left_Left.IsRemoveSchedule() == false)
            {
                pSlot_Left_Left.SetSlotDying(true);
                pSlot_Left_Left.SetRemoveSchedule(true);

                MatchSlotTable.Add(pSlot_Left_Left.GetSlotIndex(), pSlot_Left_Left);
            }
        }

        if (pSlot.IsLeftSide() == false && pSlot.IsLeftSecondSide() == false)
        {
            vSlotPos = pSlot.GetPosition();
            vSlotPos.x -= InGameInfo.Instance.m_fSlotSize * 2;
            pGameEvent = new GameEvent_InGame_Effect_Stripe(vSlotPos, eSpecialItem.Ver);
            AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
        }

        Slot pSlot_Right = pSlot.GetNeighborSlot(eNeighbor.Neighbor_21);
        if (pSlot_Right != null && pSlot_Right.GetSlotBlock() != null && pSlot_Right.IsRemoveSchedule() == false)
        {
            pSlot_Right.SetSlotDying(true);
            pSlot_Right.SetRemoveSchedule(true);

            MatchSlotTable.Add(pSlot_Right.GetSlotIndex(), pSlot_Right);
        }

        if (pSlot.IsRightSide() == false)
        {
            vSlotPos = pSlot.GetPosition();
            vSlotPos.x += InGameInfo.Instance.m_fSlotSize;
            pGameEvent = new GameEvent_InGame_Effect_Stripe(vSlotPos, eSpecialItem.Ver);
            AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
        }

        if (pSlot_Right != null)
        {
            Slot pSlot_Right_Right = pSlot_Right.GetNeighborSlot(eNeighbor.Neighbor_21);
            if (pSlot_Right_Right != null && pSlot_Right_Right.GetSlotBlock() != null && pSlot_Right_Right.IsRemoveSchedule() == false)
            {
                pSlot_Right_Right.SetSlotDying(true);
                pSlot_Right_Right.SetRemoveSchedule(true);

                MatchSlotTable.Add(pSlot_Right_Right.GetSlotIndex(), pSlot_Right_Right);
            }
        }


        if (pSlot.IsRightSide() == false && pSlot.IsRightSecondSide() == false)
        {
            vSlotPos = pSlot.GetPosition();
            vSlotPos.x += InGameInfo.Instance.m_fSlotSize * 2;
            pGameEvent = new GameEvent_InGame_Effect_Stripe(vSlotPos, eSpecialItem.Ver);
            AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
        }

        for (int y = 1; y < GameDefine.ms_nInGameSlot_Y; ++y)
        {
            for (int x = pSlot.GetX() - 2; x <= pSlot.GetX() + 2; ++x)
            {
                if (x < 0 || x > InGameInfo.Instance.m_nSlotEndIndex_X)
                    continue;

                // Up
                int nSlot_Y = pSlot.GetY() - y;
                if (nSlot_Y >= 0 && m_SlotTable.ContainsKey(Helper.GetSlotIndex(x, nSlot_Y)) == true)
                {
                    Slot pFindSlot = m_SlotTable[Helper.GetSlotIndex(x, nSlot_Y)];

                    if (m_CombinationTransSpecialSlotTable.ContainsKey(Helper.GetSlotIndex(x, nSlot_Y)) == false)
                    {
                        if (pFindSlot.IsRemoveSchedule() == false)
                        {
                            pFindSlot.SetRemoveSchedule(true);

                            eventValue = new TransformerEvent_Timer(fBlockTimeInterval, new object[] { pFindSlot, eSpecialItem.Plus_L5 });
                            pTimer_SpecialSlotRemoveAction.AddEvent(eventValue);

                            if (MatchSlotTable.ContainsKey(pFindSlot.GetSlotIndex()) == false)
                                MatchSlotTable.Add(pFindSlot.GetSlotIndex(), pFindSlot);
                        }
                    }
                }

                // Down
                nSlot_Y = pSlot.GetY() + y;
                if (nSlot_Y < GameDefine.ms_nInGameSlot_Y && m_SlotTable.ContainsKey(Helper.GetSlotIndex(x, nSlot_Y)) == true)
                {
                    Slot pFindSlot = m_SlotTable[Helper.GetSlotIndex(x, nSlot_Y)];

                    if (m_CombinationTransSpecialSlotTable.ContainsKey(Helper.GetSlotIndex(x, nSlot_Y)) == false)
                    {
                        if (pFindSlot.IsRemoveSchedule() == false)
                        {
                            pFindSlot.SetRemoveSchedule(true);

                            eventValue = new TransformerEvent_Timer(fBlockTimeInterval, new object[] { pFindSlot, eSpecialItem.Plus_L5 });
                            pTimer_SpecialSlotRemoveAction.AddEvent(eventValue);

                            if (MatchSlotTable.ContainsKey(pFindSlot.GetSlotIndex()) == false)
                                MatchSlotTable.Add(pFindSlot.GetSlotIndex(), pFindSlot);
                        }
                    }
                }
            }
            fBlockTimeInterval += GameDefine.ms_fSpecialSlotStraight;
        }

        pTimer_SpecialSlotRemoveAction.SetCallback(OnOneEventDone_SpecialSlotRemoveAction, OnDone_SpecialSlotRemoveAction);
        pTimer_SpecialSlotRemoveAction.OnPlay();
    }

    private void RemoveSpecialItemBlock_Match_Color(Slot pSlot, eBlockType eBlockType_Match_Color)
    {
        CombinationShapeData pCombinationShapeData = InGameSetting.Instance.m_pInGameSettingData.GetCombinationShapeData(eSpecialItem.Match_Color);

        if (pCombinationShapeData != null && pCombinationShapeData.m_IsIgnoreBlockType == false)
        {
            if (pSlot.GetSlotBlock() != null)
            {
                eBlockType_Match_Color = pSlot.GetSlotBlock().GetBlockType();
            }
        }
        else
        {
            if (eBlockType_Match_Color == eBlockType.Empty)
            {
                eBlockType_Match_Color = GetRandomBlockType_CurrExistSlotTable();
            }
        }

        Dictionary<int, Slot> MatchSlotTable = new Dictionary<int, Slot>();
        MatchSlotTable.Add(pSlot.GetSlotIndex(), pSlot);

        Transformer_Timer pTimer_SpecialSlotRemoveAction = new Transformer_Timer();
        m_pTransformerCollector.AddTransformer(pTimer_SpecialSlotRemoveAction);

        TransformerEvent eventValue;

        Vector3 vPos = pSlot.transform.position;
        vPos.z = -(float)ePlaneOrder.Fx_TopLayer;

        foreach (KeyValuePair<int, Slot> item in m_SlotTable)
        {
            if (item.Value.GetSlotBlock() != null && item.Value.GetSlotBlock().GetBlockType() == eBlockType_Match_Color && 
                m_CombinationTransSpecialSlotTable.ContainsKey(item.Key) == false &&
                (SlotManager.IsObstacle_MatchPossible(item.Value) == true ||
                (SlotManager.IsObstacle_MatchPossible(item.Value) == false && item.Value.IsSlotBlockMatchInPossibleAtCheckRemovePossible() == true)))
            {
                eventValue = new TransformerEvent_Timer(GameDefine.ms_Match_Color_ChainlightningTime, new object[] { item.Value, eSpecialItem.Match_Color });
                pTimer_SpecialSlotRemoveAction.AddEvent(eventValue);

                Vector3 vPos_02 = item.Value.transform.position;
                vPos_02.z = -(float)ePlaneOrder.Fx_TopLayer;

                GameObject ob = GameObject.Instantiate(m_pGameObject_ChainLightning);
                ChainLightning pChainLightnint = ob.GetComponent<ChainLightning>();
                pChainLightnint.Init(vPos, vPos_02, GameDefine.ms_Match_Color_ChainlightningTime);

                ParticleManager.Instance.LoadParticleSystem("FX_MatchColorElectricity", vPos_02).SetScale(AppInstance.Instance.m_fMainScale * InGameInfo.Instance.m_fInGameScale);

                if (MatchSlotTable.ContainsKey(item.Value.GetSlotIndex()) == false)
                    MatchSlotTable.Add(item.Value.GetSlotIndex(), item.Value);
            }
        }

        m_pMainGame.OnMatchSlot(MatchSlotTable, pSlot.GetSlotBlock().GetBlockType());

        foreach (KeyValuePair<int, Slot> item in MatchSlotTable)
        {
            if (m_MatchSlotList.Contains(item.Value) == false)
            {
                m_MatchSlotList.Add(item.Value);
            }
        }

        pTimer_SpecialSlotRemoveAction.SetCallback(OnOneEventDone_SpecialSlotRemoveAction, OnDone_SpecialSlotRemoveAction);
        pTimer_SpecialSlotRemoveAction.OnPlay();

        Helper.OnSoundPlay("INGAME_5MATCH_ELIXIR_ACTIVATE", false);
    }

    private void RemoveSpecialItemBlock_Match_Color_Trans(Slot pSlot, eBlockType eBlockType_Match_Color, SpecialItemCombinationData pSpecialItemCombinationData)
    {
        m_MatchColorTransSlotList.Clear();

        CombinationShapeData pCombinationShapeData = InGameSetting.Instance.m_pInGameSettingData.GetCombinationShapeData(eSpecialItem.Match_Color);

        if (pCombinationShapeData != null && pCombinationShapeData.m_IsIgnoreBlockType == false)
        {
            if (pSlot.GetSlotBlock() != null)
            {
                eBlockType_Match_Color = pSlot.GetSlotBlock().GetBlockType();
            }
        }
        else
        {
            if (!((int)eBlockType_Match_Color >= GameDefine.ms_nBlockStart && (int)eBlockType_Match_Color < GameDefine.ms_nBlockStart + InGameSetting.Instance.m_pInGameSettingData.m_nBlockCount))
            {
                eBlockType_Match_Color = GetRandomBlockType_CurrExistSlotTable();
            }
        }

        Dictionary<int, Slot> MatchSlotTable = new Dictionary<int, Slot>();
        MatchSlotTable.Add(pSlot.GetSlotIndex(), pSlot);

        Transformer_Timer pTimer_SpecialSlotRemoveAction = new Transformer_Timer();
        m_pTransformerCollector.AddTransformer(pTimer_SpecialSlotRemoveAction);

        TransformerEvent eventValue;

        Vector3 vPos = pSlot.transform.position;
        vPos.z = -(float)ePlaneOrder.Fx_TopLayer;

        foreach (KeyValuePair<int, Slot> item in m_SlotTable)
        {
            if (item.Value.GetSlotBlock() != null && item.Value.GetSlotBlock().GetBlockType() == eBlockType_Match_Color &&
                m_CombinationTransSpecialSlotTable.ContainsKey(item.Key) == false &&
                (SlotManager.IsObstacle_MatchPossible(item.Value) == true ||
                (SlotManager.IsObstacle_MatchPossible(item.Value) == false && item.Value.IsSlotBlockMatchInPossibleAtCheckRemovePossible() == true)))
            {
                eventValue = new TransformerEvent_Timer(GameDefine.ms_Match_Color_ChainlightningTime, new object[] { item.Value, eSpecialItem.Match_Color_Trans, pSpecialItemCombinationData, eBlockType_Match_Color });
                pTimer_SpecialSlotRemoveAction.AddEvent(eventValue);

                Vector3 vPos_02 = item.Value.transform.position;
                vPos_02.z = -(float)ePlaneOrder.Fx_TopLayer;

                GameObject ob = GameObject.Instantiate(m_pGameObject_ChainLightning);
                ChainLightning pChainLightnint = ob.GetComponent<ChainLightning>();
                pChainLightnint.Init(vPos, vPos_02, GameDefine.ms_Match_Color_ChainlightningTime);

                ParticleManager.Instance.LoadParticleSystem("FX_MatchColorElectricity", vPos_02).SetScale(AppInstance.Instance.m_fMainScale * InGameInfo.Instance.m_fInGameScale);
            }
        }

        m_pMainGame.OnMatchSlot(MatchSlotTable, pSlot.GetSlotBlock().GetBlockType());

        foreach (KeyValuePair<int, Slot> item in MatchSlotTable)
        {
            if (m_MatchSlotList.Contains(item.Value) == false)
            {
                m_MatchSlotList.Add(item.Value);
            }
        }

        pTimer_SpecialSlotRemoveAction.SetCallback(OnOneEventDone_SpecialSlotRemoveAction, OnDone_SpecialSlotRemoveAction);
        pTimer_SpecialSlotRemoveAction.OnPlay();

        Helper.OnSoundPlay("INGAME_5MATCH_ELIXIR_ACTIVATE", false);
    }

    private void RemoveSpecialItemBlock_All_Block1(Slot pSlot)
    {
        foreach (KeyValuePair<int, Slot> item in m_SlotTable)
        {
            item.Value.SetSlotDying(true);
        }
    }

    private void RemoveSpecialItemBlock_Customize(Slot pSlot, SpecialItemCustomize pSpecialItemCustomize)
    {
        pSlot.SetSlotDying(true);

        Slot pTempSlot;
        bool bActive_Timer_RemoveEnd = true;

        Dictionary<int, Slot> MatchSlotTable = new Dictionary<int, Slot>();
        MatchSlotTable.Add(pSlot.GetSlotIndex(), pSlot);

        int nStartX = pSlot.GetX() - pSpecialItemCustomize.m_nNumGrid / 2;
        int nStartY = pSlot.GetY() - pSpecialItemCustomize.m_nNumGrid / 2;

        int nShapeIndex = 0;

        for (int j = nStartY; j < nStartY + pSpecialItemCustomize.m_nNumGrid; j++)
        {
            for (int i = nStartX; i < nStartX + pSpecialItemCustomize.m_nNumGrid; i++)
            {
                if (IsValidSlotIndex(i, j) == true && pSpecialItemCustomize.m_Shapes[nShapeIndex] == true)
                {
                    int nSlotIndex = Helper.GetSlotIndex(i, j);
                    if (m_SlotTable.ContainsKey(nSlotIndex) == true)
                    {
                        pTempSlot = m_SlotTable[nSlotIndex];

                        if (pTempSlot != null && pTempSlot.IsRemoveSchedule() == false && m_CombinationTransSpecialSlotTable.ContainsKey(pTempSlot.GetSlotIndex()) == false)
                        {
                            if (pTempSlot.GetSlotBlock() != null && pTempSlot.GetSlotBlock().GetSpecialItem() != eSpecialItem.None && pTempSlot.IsSlotDying() == false)
                            {
                                SlotFixObject_Obstacle pObstacle_OnlyMineBreak = pTempSlot.FindFixObject_SlotDyingAtOnlyMineBreak();

                                if (pObstacle_OnlyMineBreak == null)
                                {
                                    bActive_Timer_RemoveEnd = false;
                                    RemoveSpecialItemBlock(pTempSlot);
                                }
                                else
                                {
                                    pTempSlot.SetSlotDying(true);
                                }
                            }
                            else
                            {
                                pTempSlot.SetSlotDying(true);

                                if (MatchSlotTable.ContainsKey(pTempSlot.GetSlotIndex()) == false)
                                    MatchSlotTable.Add(pTempSlot.GetSlotIndex(), pTempSlot);
                            }
                        }
                    }
                }

                nShapeIndex++;
            }
        }

        m_pMainGame.OnMatchSlot(MatchSlotTable, pSlot.GetSlotBlock().GetBlockType());

        foreach (KeyValuePair<int, Slot> item in MatchSlotTable)
        {
            if (m_MatchSlotList.Contains(item.Value) == false)
            {
                m_MatchSlotList.Add(item.Value);
            }
        }

        if (bActive_Timer_RemoveEnd == true)
        {
            m_pTimer_RemoveEnd.OnReset();

            TransformerEvent_Timer eventValue;
            eventValue = new TransformerEvent_Timer(GameDefine.ms_fSpecialItemBlockActionTime);
            m_pTimer_RemoveEnd.AddEvent(eventValue);
            m_pTimer_RemoveEnd.SetCallback(null, OnDone_Timer_RemoveEnd);
            m_pTimer_RemoveEnd.OnPlay();
        }
    }

    private void OnOneEventDone_SpecialSlotRemoveAction(int nIndex, TransformerEvent eventValue)
    {
        if (eventValue.m_pParametas != null)
        {
            Slot pSlot = eventValue.m_pParametas[0] as Slot;
            eSpecialItem eSpecialItem = (eSpecialItem)eventValue.m_pParametas[1];

            if (eSpecialItem == eSpecialItem.Match_Color_Trans)
            {
                SpecialItemCombinationData pSpecialItemCombinationData = (SpecialItemCombinationData)eventValue.m_pParametas[2];
                eBlockType eBlockType_TransColor = (eBlockType)eventValue.m_pParametas[3];

                if (!((int)eBlockType_TransColor >= GameDefine.ms_nBlockStart && (int)eBlockType_TransColor < GameDefine.ms_nBlockStart + InGameSetting.Instance.m_pInGameSettingData.m_nBlockCount))
                {
                    eBlockType_TransColor = GetRandomBlockType_CurrExistSlotTable();
                }

                int nNumTrans = pSpecialItemCombinationData.m_MatchColorTransList.Count;
                int nRandomValue = UnityEngine.Random.Range(0, nNumTrans);
                eSpecialItem eSpecialItem_Trans = pSpecialItemCombinationData.m_MatchColorTransList[nRandomValue];
                SpecialItemCustomize pSpecialItemCustomize = pSpecialItemCombinationData.m_MatchColorTransSpecialItemCustomizeList[nRandomValue];

                CombinationShapeData pCombinationShapeData = InGameSetting.Instance.m_pInGameSettingData.GetCombinationShapeData(eSpecialItem_Trans);

                if (pSlot != null && pSlot.GetSlotBlock() != null)
                {
                    pSlot.GetSlotBlock().ChangeBlockSpecialItem(pCombinationShapeData.m_pGameObject[(int)eBlockType_TransColor], eSpecialItem_Trans, pSpecialItemCustomize);
                    m_MatchColorTransSlotList.Add(pSlot);
                }
            }
            else if (pSlot != null)
            {
                if (pSlot.IsSlotDying() == false && pSlot.GetSlotBlock() != null && m_CombinationTransSpecialSlotTable.ContainsKey(pSlot.GetSlotIndex()) == false)
                {
                    if (pSlot.GetSlotBlock().GetSpecialItem() != eSpecialItem.None)
                    {
                        SlotFixObject_Obstacle pObstacle_OnlyMineBreak = pSlot.FindFixObject_SlotDyingAtOnlyMineBreak();

                        if (pObstacle_OnlyMineBreak == null)
                        {
                            RemoveSpecialItemBlock(pSlot);
                        }
                        else
                        {
                            pSlot.SetSlotDying(true);
                        }
                    }
                    else
                    {
                        pSlot.SetSlotDying(true);
                    }
                }
            }

            if (pSlot != null)
            {
                Vector3 vPos = pSlot.GetPosition();
                vPos *= InGameInfo.Instance.m_fInGameScale;
                vPos.z = -(float)ePlaneOrder.FX_Stripe_Block;

                switch (eSpecialItem)
                {
                    case eSpecialItem.Hor:
                    case eSpecialItem.Ver:
                        {
                            ParticleManager.Instance.LoadParticleSystem("FX_Stripe_Block", vPos).SetScale(AppInstance.Instance.m_fMainScale * InGameInfo.Instance.m_fInGameScale);
                        }
                        break;
                }
            }
        }
    }

    private void OnDone_SpecialSlotRemoveAction(TransformerEvent eventValue)
    {
        if (eventValue != null && eventValue.m_pParametas != null)
        {
            bool bActive_Timer_RemoveEnd = true;

            Slot pSlot = eventValue.m_pParametas[0] as Slot;
            eSpecialItem eSpecialItem = (eSpecialItem)eventValue.m_pParametas[1];

            if (eSpecialItem == eSpecialItem.Match_Color_Trans)
            {
                SpecialItemCombinationData pSpecialItemCombinationData = (SpecialItemCombinationData)eventValue.m_pParametas[2];
                eBlockType eBlockType_TransColor = (eBlockType)eventValue.m_pParametas[3];

                if (!((int)eBlockType_TransColor >= GameDefine.ms_nBlockStart && (int)eBlockType_TransColor < GameDefine.ms_nBlockStart + InGameSetting.Instance.m_pInGameSettingData.m_nBlockCount))
                {
                    eBlockType_TransColor = GetRandomBlockType_CurrExistSlotTable();
                }

                int nNumTrans = pSpecialItemCombinationData.m_MatchColorTransList.Count;
                int nRandomValue = UnityEngine.Random.Range(0, nNumTrans);
                eSpecialItem eSpecialItem_Trans = pSpecialItemCombinationData.m_MatchColorTransList[nRandomValue];
                SpecialItemCustomize pSpecialItemCustomize = pSpecialItemCombinationData.m_MatchColorTransSpecialItemCustomizeList[nRandomValue];

                CombinationShapeData pCombinationShapeData = InGameSetting.Instance.m_pInGameSettingData.GetCombinationShapeData(eSpecialItem_Trans);

                if (pSlot != null && pSlot.GetSlotBlock() != null)
                {
                    pSlot.GetSlotBlock().ChangeBlockSpecialItem(pCombinationShapeData.m_pGameObject[(int)eBlockType_TransColor], eSpecialItem_Trans, pSpecialItemCustomize);
                    m_MatchColorTransSlotList.Add(pSlot);
                }

                m_pTimer_MatchColorTransSlot.OnReset();
                eventValue = new TransformerEvent_Timer(0.4f);
                m_pTimer_MatchColorTransSlot.AddEvent(eventValue);
                m_pTimer_MatchColorTransSlot.SetCallback(null, OnDone_Timer_MatchColorTransSlot);
                m_pTimer_MatchColorTransSlot.OnPlay();

                bActive_Timer_RemoveEnd = false;
            }
            else if (pSlot != null)
            {
                if (pSlot.IsSlotDying() == false && pSlot.GetSlotBlock() != null && m_CombinationTransSpecialSlotTable.ContainsKey(pSlot.GetSlotIndex()) == false)
                {
                    if (pSlot.GetSlotBlock().GetSpecialItem() != eSpecialItem.None)
                    {
                        SlotFixObject_Obstacle pObstacle_OnlyMineBreak = pSlot.FindFixObject_SlotDyingAtOnlyMineBreak();

                        if (pObstacle_OnlyMineBreak == null)
                        {
                            bActive_Timer_RemoveEnd = false;
                            RemoveSpecialItemBlock(pSlot);
                        }
                        else
                        {
                            pSlot.SetSlotDying(true);
                        }
                    }
                    else
                    {
                        pSlot.SetSlotDying(true);
                    }
                }
            }

            if (pSlot != null)
            {
                Vector3 vPos = pSlot.GetPosition();
                vPos *= InGameInfo.Instance.m_fInGameScale;
                vPos.z = -(float)ePlaneOrder.FX_Stripe_Block;

                switch (eSpecialItem)
                {
                    case eSpecialItem.Hor:
                    case eSpecialItem.Ver:
                        {
                            ParticleManager.Instance.LoadParticleSystem("FX_Stripe_Block", vPos).SetScale(AppInstance.Instance.m_fMainScale * InGameInfo.Instance.m_fInGameScale);
                        }
                        break;
                }
            }

            if (bActive_Timer_RemoveEnd == true)
            {
                m_pTimer_RemoveEnd.OnReset();

                eventValue = new TransformerEvent_Timer(GameDefine.ms_fSpecialItemBlockActionTime);
                m_pTimer_RemoveEnd.AddEvent(eventValue);
                m_pTimer_RemoveEnd.SetCallback(null, OnDone_Timer_RemoveEnd);
                m_pTimer_RemoveEnd.OnPlay();
            }
        }
    }

    private void OnDone_Timer_MatchColorTransSlot(TransformerEvent eventValue)
    {
        foreach (Slot pSlot in m_MatchColorTransSlotList)
        {
            if (pSlot.IsSlotDying() == false && pSlot.IsRemoveSchedule() == false)
            {
                RemoveSpecialItemBlock(pSlot, pSlot.GetSlotBlock().GetSpecialItem(), pSlot.GetSlotBlock().GetSpecialItemCustomize());
            }
        }

        m_MatchColorTransSlotList.Clear();
    }

    private void OnBreakSlotBlockCheck(Slot pSlot)
    {
        if (pSlot != null && pSlot.GetSlotBlock() != null &&
            pSlot.GetSlotBlock().GetBlockType() == eBlockType.Custom && pSlot.GetSlotBlock().GetMapSlotItem() != eMapSlotItem.None)
        {
            switch (pSlot.GetSlotBlock().GetMapSlotItem())
            {
                case eMapSlotItem.Tire:
                    {
                        pSlot.SetSlotDying(true);
                    }
                    break;
            }
        }
    }

    private void OnBreakSlotCheck(Slot pSlot)
    {
        if (pSlot != null)
        {
            SlotFixObject_Obstacle pObstacle = pSlot.FindFixObject_NeighborSlotBlockDyingAtBreak();

            if (pObstacle != null)
            {
                pSlot.SetSlotDying(true);
            }
        }
    }

    private void OnBreakNeighborSlotCheck(Slot pSlot)
    {
        Slot pNeighbor;

        pNeighbor = pSlot.GetNeighborSlot(eNeighbor.Neighbor_10);
        OnBreakSlotBlockCheck(pNeighbor);
        OnBreakSlotCheck(pNeighbor);

        pNeighbor = pSlot.GetNeighborSlot(eNeighbor.Neighbor_01);
        OnBreakSlotBlockCheck(pNeighbor);
        OnBreakSlotCheck(pNeighbor);

        pNeighbor = pSlot.GetNeighborSlot(eNeighbor.Neighbor_21);
        OnBreakSlotBlockCheck(pNeighbor);
        OnBreakSlotCheck(pNeighbor);

        pNeighbor = pSlot.GetNeighborSlot(eNeighbor.Neighbor_12);
        OnBreakSlotBlockCheck(pNeighbor);
        OnBreakSlotCheck(pNeighbor);
    }

    private void OnDone_Timer_RemoveEnd(TransformerEvent eventValue)
    {
        foreach (Slot pSlot in m_MatchSlotList)
        {
            if (pSlot.GetLastSlotFixObject() == null)
            {
                OnBreakNeighborSlotCheck(pSlot);
            }
        }

        m_pMainGame.OnDoneCheckRemoveSlot();

        foreach (KeyValuePair<int, Slot> item in m_SlotTable)
        {
            Slot pSlot = item.Value;
            pSlot.OnSlotDying();
        }

        m_pMainGame.OnDoneAllBlockDying();

        foreach (KeyValuePair<int, Slot> item in m_SlotTable)
        {
            Slot pSlot = item.Value;
            pSlot.SetRemoveSchedule(false);
            pSlot.SetMoveFlag(false);
            pSlot.SetSlotDying(false);
        }
    }

    private void OnNothingRemoveSlot()
    {
        foreach (KeyValuePair<int, Slot> item in m_SlotTable)
        {
            Slot pSlot = item.Value;
            pSlot.SetRemoveSchedule(false);
            item.Value.SetMoveFlag(false);
            item.Value.SetSlotDying(false);
        }

        if (false)  // 미션 성공 여부
        {
        }
        else if (InGameInfo.Instance.m_IsCheckSlotMove == true && CheckSlotMove() == true)
        {
            OutputLog.Log("SlotManager : OnNothingRemoveSlot InGameInfo.Instance.m_IsCheckSlotMove == true && CheckSlotMove() == true");

            m_pTimer_SlotMove.OnReset();
            TransformerEvent_Timer eventValue;
            eventValue = new TransformerEvent_Timer(GameDefine.ms_fBlockSwapMoveTime + 0.01f);
            m_pTimer_SlotMove.AddEvent(eventValue);
            m_pTimer_SlotMove.SetCallback(null, OnDone_Timer_SlotMove);
            m_pTimer_SlotMove.OnPlay();
        }
        else
        {
            OutputLog.Log("SlotManager : OnNothingRemoveSlot");

            if (m_nTurnCompleteCheck == 0)
            {
                OutputLog.Log("SlotManager : OnNothingRemoveSlot OnMatchTurnComplete");

                m_pMainGame.OnMatchTurnComplete();
            }

            EventDelegateManager.Instance.OnInGame_CheckRemoveSlotDone();

            StartCoroutine(CheckSlotBlock());
        }
    }

    private IEnumerator CheckSlotBlock()
    {
        yield return new WaitForEndOfFrame();

        foreach (KeyValuePair<int, Slot> item in m_SlotTable)
        {
            GameObject ob = Helper.FindChildGameObject(gameObject, "Slot_" + item.Key);

            if (ob != null)
            {
                List<GameObject> list = new List<GameObject>();
                int nNumSlotBlock = Helper.GetNumChildGameObject(ob, "SlotBlock", ref list);

                if (nNumSlotBlock > 1 && list.Count > 1)
                {
                    //Debug.LogError("Two SlotBlock " + item.Key + " : " + "X : " + item.Value.GetX() + ", Y : " + item.Value.GetY());
                    //Debug.Break();
                    foreach (GameObject child in list)
                    {
                        if (item.Value.GetSlotBlock() != null && child != item.Value.GetSlotBlock().GetGameObject())
                        {
                            //GameObject.Destroy(child);
                        }
                    }
                }
            }
        }
    }

    private void OnDone_Timer_SlotMove(TransformerEvent eventValue)
    {
        m_pTimer_SlotMoveAndCreate.OnReset();
        eventValue = new TransformerEvent_Timer(GameDefine.ms_fMoveAndCreateTime);
        m_pTimer_SlotMoveAndCreate.AddEvent(eventValue);
        m_pTimer_SlotMoveAndCreate.SetCallback(null, OnDoneTimer_SlotMoveAndCreate);
        m_pTimer_SlotMoveAndCreate.OnPlay();
    }
}
