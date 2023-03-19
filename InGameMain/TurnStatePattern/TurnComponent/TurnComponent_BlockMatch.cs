using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;

public class TurnComponent_BlockMatch : TComponent<EventArg_Null, EventArg_Null>
{
    private TurnSubComponent_PlayerCharacterAttack  m_pTurnSubComponent_PlayerCharacterAttack   = null;
    private TurnSubComponent_BoosterItem            m_pTurnSubComponent_BoosterItem             = null;

    private object              m_pLockObject               = new object();
    private MainGame_DataStack  m_pDataStack                = null;
    private bool                m_IsActiveComponent         = false;

    private List<Slot>          m_SwapPossibleDirectionList = new List<Slot>();
    private Transformer_Timer   m_pTimer_SwapPossibleCheck  = new Transformer_Timer();
    private Transformer_Timer   m_pTimer_Shuffle            = new Transformer_Timer();
    private Transformer_Timer   m_pTimer_Thread             = new Transformer_Timer();

    private bool                m_IsAutoPlay                = false;
    private bool                m_IsBlockSwap               = false;

    private bool                m_IsShuffleAndCheckRemove   = false;
    private eOwner              m_eOwner                    = eOwner.My;

    public TurnComponent_BlockMatch(eOwner eOwn)
    {
        m_eOwner = eOwn;

        if ((EspressoInfo.Instance.m_eGameMode == eGameMode.PvpStage || EspressoInfo.Instance.m_eGameMode == eGameMode.EventPvpStage) && m_eOwner == eOwner.Other)
        {
            m_IsAutoPlay = true;
        }

        m_pTurnSubComponent_PlayerCharacterAttack = new TurnSubComponent_PlayerCharacterAttack(this, m_eOwner);
        m_pTurnSubComponent_BoosterItem = new TurnSubComponent_BoosterItem(this, m_eOwner);

        m_pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        TransformerEvent_Timer eventValue;
        eventValue = new TransformerEvent_Timer(2);
        m_pTimer_SwapPossibleCheck.AddEvent(eventValue);
        m_pTimer_SwapPossibleCheck.SetCallback(null, OnDone_Timer_SwapPossibleCheck);

        EventDelegateManager.Instance.OnEventInGame_BeginBlockSwap += OnInGame_BeginBlockSwap;
        EventDelegateManager.Instance.OnEventInGame_MatchTurnComplete += OnInGame_MatchTurnComplete;
        EventDelegateManager.Instance.OnEventInGame_ChangeAutoPlay += OnInGame_ChangeAutoPlay;
    }

    public override void OnDestroy()
    {
        m_pTurnSubComponent_PlayerCharacterAttack.OnDestroy();
        m_pTurnSubComponent_BoosterItem.OnDestroy();

        EventDelegateManager.Instance.OnEventInGame_BeginBlockSwap -= OnInGame_BeginBlockSwap;
        EventDelegateManager.Instance.OnEventInGame_MatchTurnComplete -= OnInGame_MatchTurnComplete;
        EventDelegateManager.Instance.OnEventInGame_ChangeAutoPlay -= OnInGame_ChangeAutoPlay;
    }

    public override void Update()
    {
        try
        {
            m_pTurnSubComponent_PlayerCharacterAttack.Update();
            m_pTurnSubComponent_BoosterItem.Update();
            m_pTimer_SwapPossibleCheck.Update(Time.deltaTime);
            m_pTimer_Shuffle.Update(Time.deltaTime);
            m_pTimer_Thread.Update(Time.deltaTime);
        }
        catch (Exception e)
        {
        }
    }

	public override void LateUpdate()
	{
        m_pTurnSubComponent_PlayerCharacterAttack.LateUpdate();
        m_pTurnSubComponent_BoosterItem.LateUpdate();
    }

	public override void OnEvent(EventArg_Null Arg)
    {
        OutputLog.Log("TurnComponent_BlockMatch : OnEvent");
        InGameTurnLog.Log("TurnComponent_BlockMatch : OnEvent");
        
        m_SwapPossibleDirectionList.Clear();

        Thread pThread = new Thread(new ThreadStart(OnThread_CalculationPossibleCheck));
        pThread.Start();
    }

    public void OnInGame_ChangeAutoPlay(bool IsAutoPlay)
    {
        if (m_eOwner == eOwner.My)
        {
            m_IsAutoPlay = IsAutoPlay;

            if (m_IsActiveComponent == true)
            {
                if (m_IsAutoPlay == true)
                {
                    OnAutoPlay();
                }
                else
                {
                }
            }
        }
    }

    public void OnPlayerCharacterAttackDone()
    {
        m_SwapPossibleDirectionList.Clear();

        Thread pThread = new Thread(new ThreadStart(OnThread_CalculationPossibleCheck));
        pThread.Start();
    }

    private void OnThread_CalculationPossibleCheck()
    {
        lock (m_pLockObject)
        {
            bool IsPossibleMoveSlot = m_pDataStack.m_pSlotManager.Calculation_PossibleMoveSlot();

            m_pTimer_Thread.OnReset();
            TransformerEvent_Timer eventValue;
            eventValue = new TransformerEvent_Timer(0.1f, IsPossibleMoveSlot);
            m_pTimer_Thread.AddEvent(eventValue);
            m_pTimer_Thread.SetCallback(null, OnDone_Timer_Thread);
            m_pTimer_Thread.OnPlay();
        }
    }

    private void OnDone_Timer_Thread(TransformerEvent eventValue)
    {
        m_SwapPossibleDirectionList.Clear();

        bool IsPossibleMoveSlot = (bool)eventValue.m_pParameta;
        bool IsExistSpecialSlot = m_pDataStack.m_pSlotManager.IsExistSpecialSlot();
        m_IsActiveComponent = true;
        m_pDataStack.m_IsCurrRefillGuideState = false;

        if (IsPossibleMoveSlot == false && IsExistSpecialSlot == false)
        {
            OutputLog.Log("TurnComponent_BlockMatch : OnEvent Shuffle");

            InGameInfo.Instance.m_IsInGameClick = false;

            switch (m_pDataStack.m_pSlotManager.OnShuffle())
            {
                case eShuffleReturn.Valid:
                    {
                        m_pTimer_Shuffle.OnReset();
                        eventValue = new TransformerEvent_Timer(0.85f, IsExistSpecialSlot);
                        m_pTimer_Shuffle.AddEvent(eventValue);
                        m_pTimer_Shuffle.SetCallback(null, OnDone_Timer_Shuffle);
                        m_pTimer_Shuffle.OnPlay();
                    }
                    break;

                case eShuffleReturn.ValidAndCheckRemove:
                    {
                        m_IsShuffleAndCheckRemove = true;
                        EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0.4f);
                    }
                    break;

                case eShuffleReturn.InValid:
                    {
                        bool isPlayerCharacterActiveSkill = false;

                        switch (EspressoInfo.Instance.m_eGameMode)
                        {
                            case eGameMode.PvpStage:
                                {
                                    if (m_eOwner == eOwner.My)
                                    {
                                        foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_PlayerCharacterTable)
                                        {
                                            if (item.Value.IsFull_SP() == true && item.Value.IsDead() == false && MainGame_Espresso_ProcessHelper.IsPlayerCharacterSkillUsePossible(item.Value) == true)
                                            {
                                                isPlayerCharacterActiveSkill = true;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_OtherPlayerCharacterTable)
                                        {
                                            if (item.Value.IsFull_SP() == true && item.Value.IsDead() == false && MainGame_Espresso_ProcessHelper.IsPlayerCharacterSkillUsePossible(item.Value) == true)
                                            {
                                                isPlayerCharacterActiveSkill = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                break;

                            case eGameMode.EventPvpStage:
                                {
                                    if (m_eOwner == eOwner.My)
                                    {
                                        foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_PlayerCharacterTable)
                                        {
                                            if (item.Value.IsFull_SP() == true && item.Value.IsDead() == false && MainGame_Espresso_ProcessHelper.IsPlayerCharacterSkillUsePossible(item.Value) == true)
                                            {
                                                isPlayerCharacterActiveSkill = true;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_OtherPlayerCharacterTable)
                                        {
                                            if (item.Value.IsFull_SP() == true && item.Value.IsDead() == false && MainGame_Espresso_ProcessHelper.IsPlayerCharacterSkillUsePossible(item.Value) == true)
                                            {
                                                isPlayerCharacterActiveSkill = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                break;

                            default:
                                {
                                    foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_PlayerCharacterTable)
                                    {
                                        if (item.Value.IsFull_SP() == true && item.Value.IsDead() == false && MainGame_Espresso_ProcessHelper.IsPlayerCharacterSkillUsePossible(item.Value) == true)
                                        {
                                            isPlayerCharacterActiveSkill = true;
                                            break;
                                        }
                                    }
                                }
                                break;
                        }

                        if (m_IsAutoPlay == true)
                        {
                            if (isPlayerCharacterActiveSkill == false)
                            {
                                switch (EspressoInfo.Instance.m_eGameMode)
                                {
                                    case eGameMode.PvpStage:
                                        {
                                            if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
                                            {
                                                if (m_pDataStack.m_pCloudManager.IsExistCloud(eOwner.Other) == false)
                                                {
                                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Fail);
                                                }
                                                else
                                                {
                                                    m_IsActiveComponent = false;
                                                    m_IsBlockSwap = false;

                                                    EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

                                                    OutputLog.Log("TurnComponent_BlockMatch : GetNextEvent().OnEvent(EventArg_Null.Object)");
                                                    GetNextEvent().OnEvent(EventArg_Null.Object);
                                                }
                                            }
                                            else
                                            {
                                                if (m_pDataStack.m_pCloudManager.IsExistCloud(eOwner.My) == false)
                                                {
                                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                                                }
                                                else
                                                {
                                                    m_IsActiveComponent = false;
                                                    m_IsBlockSwap = false;

                                                    EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

                                                    OutputLog.Log("TurnComponent_BlockMatch : GetNextEvent().OnEvent(EventArg_Null.Object)");
                                                    GetNextEvent().OnEvent(EventArg_Null.Object);
                                                }
                                            }
                                        }
                                        break;

                                    case eGameMode.EventPvpStage:
                                        {
                                            if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
                                            {
                                                if (m_pDataStack.m_pCloudManager.IsExistCloud(eOwner.Other) == false)
                                                {
                                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Fail);
                                                }
                                                else
                                                {
                                                    m_IsActiveComponent = false;
                                                    m_IsBlockSwap = false;

                                                    EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

                                                    OutputLog.Log("TurnComponent_BlockMatch : GetNextEvent().OnEvent(EventArg_Null.Object)");
                                                    GetNextEvent().OnEvent(EventArg_Null.Object);
                                                }
                                            }
                                            else
                                            {
                                                if (m_pDataStack.m_pCloudManager.IsExistCloud(eOwner.My) == false)
                                                {
                                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                                                }
                                                else
                                                {
                                                    m_IsActiveComponent = false;
                                                    m_IsBlockSwap = false;

                                                    EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

                                                    OutputLog.Log("TurnComponent_BlockMatch : GetNextEvent().OnEvent(EventArg_Null.Object)");
                                                    GetNextEvent().OnEvent(EventArg_Null.Object);
                                                }
                                            }
                                        }
                                        break;

                                    default:
                                        {
                                            if (m_pDataStack.m_pCloudManager.IsExistCloud(eOwner.Other) == false)
                                            {
                                                EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Fail);
                                            }
                                            else
                                            {
                                                m_IsActiveComponent = false;
                                                m_IsBlockSwap = false;

                                                EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

                                                OutputLog.Log("TurnComponent_BlockMatch : GetNextEvent().OnEvent(EventArg_Null.Object)");
                                                GetNextEvent().OnEvent(EventArg_Null.Object);
                                            }
                                        }
                                        break;
                                }

                                InGameInfo.Instance.m_IsInGameClick = false;
                            }
                            else
                            {
                                // Refill Guide Open
                                m_pDataStack.m_IsCurrRefillGuideState = true;
                                InGameInfo.Instance.m_IsInGameClick = true;
                                m_pTurnSubComponent_PlayerCharacterAttack.SetActive(true);
                                m_pTurnSubComponent_BoosterItem.SetActive(true);

                                if (m_eOwner == eOwner.My)
                                {
                                    m_pDataStack.m_pMainGame.SetColor_RndSlotBlock(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.0f, 0.2f);

                                    GameObject ob = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGameRefillGuide");
                                    GameObject.Instantiate(ob);
                                }
                            }
                        }
                        else
                        {
                            bool isExistUseItem = false;

                            if (EspressoInfo.Instance.m_eGameMode != eGameMode.PvpStage && EspressoInfo.Instance.m_eGameMode != eGameMode.EventPvpStage)
                            {
                                int nTeamDeckID = InventoryInfoManager.Instance.m_pDeckInfo.GetGameModeDeckID(EspressoInfo.Instance.m_eGameMode);
                                int nBoosterItemDeckID = InventoryInfoManager.Instance.m_pDeckInfo.GetBoosterItemDeckID(nTeamDeckID);

                                BoosterItemInvenItemInfo pInvenItemInfo = InventoryInfoManager.Instance.m_pBoosterItemInvenInfoGroup.GetBoosterItemInfoInfo_bySlot(nBoosterItemDeckID, (int)eBoosterItemType_bySlot.AD);

                                if (pInvenItemInfo != BoosterItemInvenInfoGroup.ms_EmptyBoosterItemInvenInfo)
                                {
                                    isExistUseItem = true;
                                }

                                for (int i = 0; i < GameDefine.ms_nMaxBoosterItemCount; ++i)
                                {
                                    pInvenItemInfo = InventoryInfoManager.Instance.m_pBoosterItemInvenInfoGroup.GetBoosterItemInfoInfo_bySlot(nBoosterItemDeckID, i);

                                    if (pInvenItemInfo != BoosterItemInvenInfoGroup.ms_EmptyBoosterItemInvenInfo && pInvenItemInfo.m_nItemCount != 0)
                                    {
                                        isExistUseItem = true;
                                    }
                                }
                            }

                            if (isPlayerCharacterActiveSkill == false && isExistUseItem == false)
                            {
                                switch (EspressoInfo.Instance.m_eGameMode)
                                {
                                    case eGameMode.PvpStage:
                                        {
                                            if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
                                            {
                                                if (m_pDataStack.m_pCloudManager.IsExistCloud(eOwner.Other) == false)
                                                {
                                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Fail);
                                                }
                                                else
                                                {
                                                    m_IsActiveComponent = false;
                                                    m_IsBlockSwap = false;

                                                    EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

                                                    OutputLog.Log("TurnComponent_BlockMatch : GetNextEvent().OnEvent(EventArg_Null.Object)");
                                                    GetNextEvent().OnEvent(EventArg_Null.Object);
                                                }
                                            }
                                            else
                                            {
                                                if (m_pDataStack.m_pCloudManager.IsExistCloud(eOwner.My) == false)
                                                {
                                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                                                }
                                                else
                                                {
                                                    m_IsActiveComponent = false;
                                                    m_IsBlockSwap = false;

                                                    EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

                                                    OutputLog.Log("TurnComponent_BlockMatch : GetNextEvent().OnEvent(EventArg_Null.Object)");
                                                    GetNextEvent().OnEvent(EventArg_Null.Object);
                                                }
                                            }
                                        }
                                        break;

                                    case eGameMode.EventPvpStage:
                                        {
                                            if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
                                            {
                                                if (m_pDataStack.m_pCloudManager.IsExistCloud(eOwner.Other) == false)
                                                {
                                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Fail);
                                                }
                                                else
                                                {
                                                    m_IsActiveComponent = false;
                                                    m_IsBlockSwap = false;

                                                    EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

                                                    OutputLog.Log("TurnComponent_BlockMatch : GetNextEvent().OnEvent(EventArg_Null.Object)");
                                                    GetNextEvent().OnEvent(EventArg_Null.Object);
                                                }
                                            }
                                            else
                                            {
                                                if (m_pDataStack.m_pCloudManager.IsExistCloud(eOwner.My) == false)
                                                {
                                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                                                }
                                                else
                                                {
                                                    m_IsActiveComponent = false;
                                                    m_IsBlockSwap = false;

                                                    EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

                                                    OutputLog.Log("TurnComponent_BlockMatch : GetNextEvent().OnEvent(EventArg_Null.Object)");
                                                    GetNextEvent().OnEvent(EventArg_Null.Object);
                                                }
                                            }
                                        }
                                        break;

                                    default:
                                        {
                                            if (m_pDataStack.m_pCloudManager.IsExistCloud(eOwner.Other) == false)
                                            {
                                                EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Fail);
                                            }
                                            else
                                            {
                                                m_IsActiveComponent = false;
                                                m_IsBlockSwap = false;

                                                EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

                                                OutputLog.Log("TurnComponent_BlockMatch : GetNextEvent().OnEvent(EventArg_Null.Object)");
                                                GetNextEvent().OnEvent(EventArg_Null.Object);
                                            }
                                        }
                                        break;
                                }

                                InGameInfo.Instance.m_IsInGameClick = false;
                            }
                            else
                            {
                                // Refill Guide Open
                                m_pDataStack.m_IsCurrRefillGuideState = true;
                                InGameInfo.Instance.m_IsInGameClick = true;
                                m_pTurnSubComponent_PlayerCharacterAttack.SetActive(true);
                                m_pTurnSubComponent_BoosterItem.SetActive(true);

                                if (m_eOwner == eOwner.My)
                                {
                                    m_pDataStack.m_pMainGame.SetColor_RndSlotBlock(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.0f, 0.2f);

                                    GameObject ob = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGameRefillGuide");
                                    GameObject.Instantiate(ob);
                                }
                            }
                        }
                    }
                    break;
            }
        }
        else if (IsPossibleMoveSlot == true)
        {
            InGameInfo.Instance.m_IsInGameClick = true;
            m_pTurnSubComponent_PlayerCharacterAttack.SetActive(true);
            m_pTurnSubComponent_BoosterItem.SetActive(true);

            if (m_eOwner == eOwner.My)
            {
                m_pDataStack.m_pMainGame.SetColor_RndSlotBlock(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.0f, 0.2f);
            }

            Slot pSlot_01 = m_pDataStack.m_pSlotManager.GetSlot_SwapPossible_01();
            Slot pSlot_02 = m_pDataStack.m_pSlotManager.GetSlot_SwapPossible_02();

            m_SwapPossibleDirectionList.Add(pSlot_01);
            m_SwapPossibleDirectionList.Add(pSlot_02);

            m_pTimer_SwapPossibleCheck.OnPlay();

            if (m_IsAutoPlay == true) 
                OnAutoPlay();

            if (InGameInfo.Instance.m_eTurnComponentType != eTurnComponentType.BlockMatch)
            {
                InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.BlockMatch;
                EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.BlockMatch);
            }
        }
        else if (IsExistSpecialSlot == true)
        {
            InGameInfo.Instance.m_IsInGameClick = true;
            m_pTurnSubComponent_PlayerCharacterAttack.SetActive(true);
            m_pTurnSubComponent_BoosterItem.SetActive(true);

            if (m_eOwner == eOwner.My)
            {
                m_pDataStack.m_pMainGame.SetColor_RndSlotBlock(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.0f, 0.2f);
            }

            Slot pSlot = m_pDataStack.m_pSlotManager.GetRandomSpecialSlot();

            m_SwapPossibleDirectionList.Add(pSlot);

            m_pTimer_SwapPossibleCheck.OnPlay();

            if (m_IsAutoPlay == true)
                OnAutoPlay();

            if (InGameInfo.Instance.m_eTurnComponentType != eTurnComponentType.BlockMatch)
            {
                InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.BlockMatch;
                EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.BlockMatch);
            }
        }
    }

    private void OnDone_Timer_Shuffle(TransformerEvent eventValue)
    {
        Thread pThread = new Thread(new ThreadStart(OnThread_ShuffleAfterCalculationPossibleCheck));
        pThread.Start();
    }

    private void OnThread_ShuffleAfterCalculationPossibleCheck()
    {
        lock (m_pLockObject)
        {
            bool IsPossibleMoveSlot = m_pDataStack.m_pSlotManager.Calculation_PossibleMoveSlot();

            m_pTimer_Thread.OnReset();
            TransformerEvent_Timer eventValue;
            eventValue = new TransformerEvent_Timer(0.1f, IsPossibleMoveSlot);
            m_pTimer_Thread.AddEvent(eventValue);
            m_pTimer_Thread.SetCallback(null, OnDone_Timer_SwapPossible);
            m_pTimer_Thread.OnPlay();
        }
    }

    private void OnDone_Timer_SwapPossible(TransformerEvent eventValue)
    {
		InGameInfo.Instance.m_IsInGameClick = true;
        m_pTurnSubComponent_PlayerCharacterAttack.SetActive(true);
        m_pTurnSubComponent_BoosterItem.SetActive(true);

        if (m_eOwner == eOwner.My)
        {
            m_pDataStack.m_pMainGame.SetColor_RndSlotBlock(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.0f, 0.2f);
        }

        Slot pSlot_01 = m_pDataStack.m_pSlotManager.GetSlot_SwapPossible_01();
        Slot pSlot_02 = m_pDataStack.m_pSlotManager.GetSlot_SwapPossible_02();

        m_SwapPossibleDirectionList.Add(pSlot_01);
        m_SwapPossibleDirectionList.Add(pSlot_02);

        m_pTimer_SwapPossibleCheck.OnPlay();

        if (m_IsAutoPlay == true)
            OnAutoPlay();

        if (InGameInfo.Instance.m_eTurnComponentType != eTurnComponentType.BlockMatch)
        {
            InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.BlockMatch;
            EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.BlockMatch);
        }
    }

    private void OnAutoPlay()
    {
        OutputLog.Log("TurnComponent_BlockMatch : OnAutoPlay");

        if (InGameInfo.Instance.m_IsInGameClick == true)
        {
            bool isPlayerCharacterActiveSkill = false;

            switch (EspressoInfo.Instance.m_eGameMode)
            {
                case eGameMode.PvpStage:
                    {
                        if (m_eOwner == eOwner.My)
                        {
                            foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_PlayerCharacterTable)
                            {
                                if (item.Value.IsFull_SP() == true && item.Value.IsDead() == false && MainGame_Espresso_ProcessHelper.IsPlayerCharacterSkillUsePossible(item.Value) == true)
                                {
                                    isPlayerCharacterActiveSkill = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_OtherPlayerCharacterTable)
                            {
                                if (item.Value.IsFull_SP() == true && item.Value.IsDead() == false && MainGame_Espresso_ProcessHelper.IsPlayerCharacterSkillUsePossible(item.Value) == true)
                                {
                                    isPlayerCharacterActiveSkill = true;
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case eGameMode.EventPvpStage:
                    {
                        if (m_eOwner == eOwner.My)
                        {
                            foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_PlayerCharacterTable)
                            {
                                if (item.Value.IsFull_SP() == true && item.Value.IsDead() == false && MainGame_Espresso_ProcessHelper.IsPlayerCharacterSkillUsePossible(item.Value) == true)
                                {
                                    isPlayerCharacterActiveSkill = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_OtherPlayerCharacterTable)
                            {
                                if (item.Value.IsFull_SP() == true && item.Value.IsDead() == false && MainGame_Espresso_ProcessHelper.IsPlayerCharacterSkillUsePossible(item.Value) == true)
                                {
                                    isPlayerCharacterActiveSkill = true;
                                    break;
                                }
                            }
                        }
                    }
                    break;

                default:
                    {
                        foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_PlayerCharacterTable)
                        {
                            if (item.Value.IsFull_SP() == true && item.Value.IsDead() == false && MainGame_Espresso_ProcessHelper.IsPlayerCharacterSkillUsePossible(item.Value) == true)
                            {
                                isPlayerCharacterActiveSkill = true;
                                break;
                            }
                        }
                    }
                    break;
            }

            if (isPlayerCharacterActiveSkill == true)
            {
                return;
            }
            else
            {
                List<Slot> BombSlotList = new List<Slot>();
                List<Slot> ElixirSlotList = new List<Slot>();

                foreach (KeyValuePair<int, Slot> item in m_pDataStack.m_pSlotManager.GetSlotTable())
                {
                    if (item.Value.GetSlotBlock() != null)
                    {
                        SlotFixObject_Obstacle pObstacle_OnlyMineBreak = item.Value.FindFixObject_SlotDyingAtOnlyMineBreak();

                        if (pObstacle_OnlyMineBreak == null && item.Value.IsSlotBlockNoSwap() == false)
                        {
                            eSpecialItem eItem = item.Value.GetSlotBlock().GetSpecialItem();

                            switch (eItem)
                            {
                                case eSpecialItem.Square3: BombSlotList.Add(item.Value); break;
                                case eSpecialItem.Match_Color: ElixirSlotList.Add(item.Value); break;
                            }
                        }
                    }
                }

                Dictionary<int, List<SlotBlock>> PossibleMoveSlotBlockTable = m_pDataStack.m_pSlotManager.GetPossibleMoveSlotBlockTable();
                Dictionary<int, SlotBlock> PossibleMoveSlotBlockSwapDestTable = m_pDataStack.m_pSlotManager.GetPossibleMoveSlotBlockSwapDestTable();

                List<SlotFixObject_Unit> unitList = new List<SlotFixObject_Unit>();

                if (m_eOwner == eOwner.My)
                {
                    foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pDataStack.m_EnemyMinionTable)
                    {
                        unitList.Add(item.Value);
                    }

                    foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pDataStack.m_PlayerSummonUnitTable)
                    {
                        if (item.Value.GetOwner() == eOwner.Other)
                        {
                            unitList.Add(item.Value);
                        }
                    }

                    foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pDataStack.m_EnemySummonUnitTable)
                    {
                        if (item.Value.GetOwner() == eOwner.Other)
                        {
                            unitList.Add(item.Value);
                        }
                    }

                    foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_OtherPlayerCharacterTable)
                    {
                        if(item.Value.IsDead() == false)
                            unitList.Add(item.Value);
                    }
                }
                else
                {
                    foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pDataStack.m_PlayerSummonUnitTable)
                    {
                        if (item.Value.GetOwner() == eOwner.My)
                        {
                            unitList.Add(item.Value);
                        }
                    }

                    foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pDataStack.m_EnemySummonUnitTable)
                    {
                        if (item.Value.GetOwner() == eOwner.My)
                        {
                            unitList.Add(item.Value);
                        }
                    }

                    foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_PlayerCharacterTable)
                    {
                        if (item.Value.IsDead() == false)
                            unitList.Add(item.Value);
                    }
                }

                Helper.ShuffleList(unitList);
                Helper.ShuffleList(unitList);

                Dictionary<int, int>[] highOrderTable = new Dictionary<int, int>[InGameInfo.Instance.m_nSlotCount_Y];    // Key : X√‡, Value : Y√‡
                for (int i = 0; i < InGameInfo.Instance.m_nSlotCount_Y; ++i)
                {
                    highOrderTable[i] = new Dictionary<int, int>();
                }

                foreach (SlotFixObject_Unit pUnit in unitList)
                {
                    int nX = pUnit.GetSlot().GetX();
                    int nY = pUnit.GetSlot().GetY();

                    if (m_eOwner == eOwner.My)
                    {
                        highOrderTable[nY].Add(nX, nY);
                    }
                    else
                    {
                        highOrderTable[InGameInfo.Instance.m_nSlotCount_Y - nY - 1].Add(nX, nY);
                    }
                }

                for (int order = 0; order < InGameInfo.Instance.m_nSlotCount_Y; ++order)
                {
                    foreach (KeyValuePair<int, int> item in highOrderTable[order])
                    {
                        for (int i = 0; i < BombSlotList.Count; ++i)
                        {
                            Slot pSlot = BombSlotList[i];

                            if (m_eOwner == eOwner.My ? pSlot.GetY() < item.Value : pSlot.GetY() > item.Value && (pSlot.GetX() == item.Key || pSlot.GetX() - 1 == item.Key || pSlot.GetX() + 1 == item.Key))
                            {
                                if (InGameInfo.Instance.m_pSelectModeSlot != null)
                                {
                                    InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
                                    InGameInfo.Instance.m_pSelectModeSlot = null;
                                }

                                if (m_eOwner == eOwner.My)
                                {
                                    m_pDataStack.m_pSlotManager.RemoveSpecialItemBlock(pSlot);
                                    EventDelegateManager.Instance.OnInGame_BeginBlockSwap();
                                }
                                else
                                {
                                    GameEvent_OtherPlayer_BlockMatch_Finger pGameEvent = new GameEvent_OtherPlayer_BlockMatch_Finger(pSlot);
                                    GameEventManager.Instance.AddGameEvent(pGameEvent);
                                }

                                OutputLog.Log("TurnComponent_BlockMatch : OnAutoPlay Bomb");

                                return;
                            }
                        }
                    }

                    foreach (KeyValuePair<int, int> item in highOrderTable[order])
                    {
                        for (int i = 0; i < ElixirSlotList.Count; ++i)
                        {
                            Slot pSlot = ElixirSlotList[i];

                            if (m_eOwner == eOwner.My ? pSlot.GetY() < item.Value : pSlot.GetY() > item.Value && pSlot.GetX() == item.Key)
                            {
                                if (InGameInfo.Instance.m_pSelectModeSlot != null)
                                {
                                    InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
                                    InGameInfo.Instance.m_pSelectModeSlot = null;
                                }

                                if (m_eOwner == eOwner.My)
                                {
                                    m_pDataStack.m_pSlotManager.RemoveSpecialItemBlock(pSlot);
                                    EventDelegateManager.Instance.OnInGame_BeginBlockSwap();
                                }
                                else
                                {
                                    GameEvent_OtherPlayer_BlockMatch_Finger pGameEvent = new GameEvent_OtherPlayer_BlockMatch_Finger(pSlot);
                                    GameEventManager.Instance.AddGameEvent(pGameEvent);
                                }

                                OutputLog.Log("TurnComponent_BlockMatch : OnAutoPlay Elixir");

                                return;
                            }
                        }
                    }

                    foreach (KeyValuePair<int, int> item in highOrderTable[order])
                    {
                        foreach (KeyValuePair<int, List<SlotBlock>> item_possibleMove in PossibleMoveSlotBlockTable)
                        {
                            if (PossibleMoveSlotBlockSwapDestTable.ContainsKey(item_possibleMove.Key) == true)
                            {
                                SlotBlock pDifferSlotBlock = null;
                                SlotBlock pDestSlotBlock = PossibleMoveSlotBlockSwapDestTable[item_possibleMove.Key];

                                eBlockType eType = pDestSlotBlock.GetBlockType();
                                bool IsSwapAble = false;
                                foreach (SlotBlock pSlotBlock in item_possibleMove.Value)
                                {
                                    if (pSlotBlock != null)
                                    {
                                        if (pSlotBlock.GetBlockType() != eType)
                                        {
                                            pDifferSlotBlock = pSlotBlock;
                                        }

                                        if (pSlotBlock != pDestSlotBlock)
                                        {
                                            Slot pSlot = pSlotBlock.GetSlot();

                                            if (m_eOwner == eOwner.My)
                                            {
                                                if (pSlot.GetY() < item.Value && pSlot.GetX() == item.Key)
                                                {
                                                    IsSwapAble = true;
                                                }
                                            }
                                            else
                                            {
                                                if (pSlot.GetY() > item.Value && pSlot.GetX() == item.Key)
                                                {
                                                    IsSwapAble = true;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (IsSwapAble == true && pDifferSlotBlock != null && pDifferSlotBlock.GetSlot() != null)
                                {
                                    if (m_pDataStack.m_pSlotManager.IsBlockSwap(pDifferSlotBlock.GetSlot(), pDestSlotBlock.GetSlot()) == true)
                                    {
                                        InGameInfo.Instance.m_nClickSlotFingerID = -1;
                                        InGameInfo.Instance.m_pClickSlot = null;

                                        if (m_eOwner == eOwner.My)
                                        {
                                            m_pDataStack.m_pSlotManager.OnBlockSwap(pDifferSlotBlock.GetSlot(), pDestSlotBlock.GetSlot());

                                            if (InGameInfo.Instance.m_pSelectModeSlot != null)
                                            {
                                                InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
                                                InGameInfo.Instance.m_pSelectModeSlot = null;
                                            }

                                            OutputLog.Log("TurnComponent_BlockMatch : OnAutoPlay Block");
                                        }
                                        else
                                        {
                                            GameEvent_OtherPlayer_BlockMatch_Finger pGameEvent = new GameEvent_OtherPlayer_BlockMatch_Finger(pDestSlotBlock.GetSlot(), pDifferSlotBlock.GetSlot());
                                            GameEventManager.Instance.AddGameEvent(pGameEvent);
                                        }

                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                if (BombSlotList.Count != 0)
                {
                    Helper.ShuffleList<Slot>(BombSlotList);
                    Slot pSlot = BombSlotList[0];
                    if (InGameInfo.Instance.m_pSelectModeSlot != null)
                    {
                        InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
                        InGameInfo.Instance.m_pSelectModeSlot = null;
                    }

                    if (m_eOwner == eOwner.My)
                    {
                        m_pDataStack.m_pSlotManager.RemoveSpecialItemBlock(pSlot);
                        EventDelegateManager.Instance.OnInGame_BeginBlockSwap();
                    }
                    else
                    {
                        GameEvent_OtherPlayer_BlockMatch_Finger pGameEvent = new GameEvent_OtherPlayer_BlockMatch_Finger(pSlot);
                        GameEventManager.Instance.AddGameEvent(pGameEvent);
                    }

                    OutputLog.Log("TurnComponent_BlockMatch : OnAutoPlay Extra Bomb");

                    return;
                }

                if (ElixirSlotList.Count != 0)
                {
                    Helper.ShuffleList<Slot>(ElixirSlotList);
                    Slot pSlot = ElixirSlotList[0];
                    if (InGameInfo.Instance.m_pSelectModeSlot != null)
                    {
                        InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
                        InGameInfo.Instance.m_pSelectModeSlot = null;
                    }

                    if (m_eOwner == eOwner.My)
                    {
                        m_pDataStack.m_pSlotManager.RemoveSpecialItemBlock(pSlot);
                        EventDelegateManager.Instance.OnInGame_BeginBlockSwap();
                    }
                    else
                    {
                        GameEvent_OtherPlayer_BlockMatch_Finger pGameEvent = new GameEvent_OtherPlayer_BlockMatch_Finger(pSlot);
                        GameEventManager.Instance.AddGameEvent(pGameEvent);
                    }

                    OutputLog.Log("TurnComponent_BlockMatch : OnAutoPlay Extra Elixir");

                    return;
                }

                int nTryCount = 30;
                while (nTryCount != 0)
                {
                    int nCount = PossibleMoveSlotBlockTable.Count;
                    int nRandom = UnityEngine.Random.Range(0, nCount);

                    if (PossibleMoveSlotBlockTable.ContainsKey(nRandom) == true && PossibleMoveSlotBlockSwapDestTable.ContainsKey(nRandom) == true)
                    {
                        List<SlotBlock> slotBlockList = PossibleMoveSlotBlockTable[nRandom];

                        SlotBlock pDifferSlotBlock = null;
                        SlotBlock pDestSlotBlock = PossibleMoveSlotBlockSwapDestTable[nRandom];

                        eBlockType eType = pDestSlotBlock.GetBlockType();
                        foreach (SlotBlock pSlotBlock in slotBlockList)
                        {
                            if (pSlotBlock.GetBlockType() != eType)
                            {
                                pDifferSlotBlock = pSlotBlock;
                            }

                            if (pSlotBlock != pDestSlotBlock)
                            {
                                Slot pSlot = pSlotBlock.GetSlot();
                            }
                        }

                        if (pDifferSlotBlock != null && pDifferSlotBlock.GetSlot() != null)
                        {
                            if (m_pDataStack.m_pSlotManager.IsBlockSwap(pDifferSlotBlock.GetSlot(), pDestSlotBlock.GetSlot()) == true)
                            {
                                InGameInfo.Instance.m_nClickSlotFingerID = -1;
                                InGameInfo.Instance.m_pClickSlot = null;

                                if (m_eOwner == eOwner.My)
                                {
                                    m_pDataStack.m_pSlotManager.OnBlockSwap(pDifferSlotBlock.GetSlot(), pDestSlotBlock.GetSlot());

                                    if (InGameInfo.Instance.m_pSelectModeSlot != null)
                                    {
                                        InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
                                        InGameInfo.Instance.m_pSelectModeSlot = null;
                                    }

                                    OutputLog.Log("TurnComponent_BlockMatch : OnAutoPlay Extra Block");
                                }
                                else
                                {
                                    GameEvent_OtherPlayer_BlockMatch_Finger pGameEvent = new GameEvent_OtherPlayer_BlockMatch_Finger(pDestSlotBlock.GetSlot(), pDifferSlotBlock.GetSlot());
                                    GameEventManager.Instance.AddGameEvent(pGameEvent);
                                }

                                return;
                            }
                        }
                    }

                    --nTryCount;
                }
            }

            OutputLog.Log("TurnComponent_BlockMatch : OnAutoPlay CalculationPossibleCheck");

            m_pTimer_Thread.OnReset();
            m_SwapPossibleDirectionList.Clear();
            Thread pThread = new Thread(new ThreadStart(OnThread_CalculationPossibleCheck));
            pThread.Start();
        }
    }

    public void OnInGame_BeginBlockSwap()
    {
        if (m_IsActiveComponent == true)
        {
            OutputLog.Log("TurnComponent_BlockMatch : OnInGame_BeginBlockSwap");

            m_IsBlockSwap = true;
            InGameInfo.Instance.m_IsInGameClick = false;
            m_pTurnSubComponent_PlayerCharacterAttack.SetActive(false);
            m_pTurnSubComponent_BoosterItem.SetActive(false);
            m_pTimer_SwapPossibleCheck.OnStop();

            foreach (Slot pSot in m_SwapPossibleDirectionList)
            {
                pSot.OnCancelSwapPossibleDirection();
            }
            m_SwapPossibleDirectionList.Clear();
        }
    }

    public void OnDone_Timer_SwapPossibleCheck(TransformerEvent eventValue)
    {
        foreach (Slot pSot in m_SwapPossibleDirectionList)
        {
            pSot.OnSwapPossibleDirection();
        }

        m_pTimer_SwapPossibleCheck.OnPlay();
    }

    private void OnInGame_MatchTurnComplete()
    {
        OutputLog.Log("TurnComponent_BlockMatch : OnInGame_MatchTurnComplete");

        if (m_IsActiveComponent == true)
        {
            if (m_IsBlockSwap == true)
            {
                m_IsActiveComponent = false;
                m_IsBlockSwap = false;
                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();

                EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

                OutputLog.Log("TurnComponent_BlockMatch : GetNextEvent().OnEvent(EventArg_Null.Object)");
                GetNextEvent().OnEvent(EventArg_Null.Object);
            }
            else if (m_IsShuffleAndCheckRemove == true)
            {
                m_IsShuffleAndCheckRemove = false;

                m_pTimer_Thread.OnReset();
                m_SwapPossibleDirectionList.Clear();
                Thread pThread = new Thread(new ThreadStart(OnThread_CalculationPossibleCheck));
                pThread.Start();
            }
        }
    }
}
