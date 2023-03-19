using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;

public class TurnSubComponent_PlayerCharacterAttack
{
    private TurnComponent_BlockMatch    m_pTurnComponent_BlockMatch = null;

    private MainGame_DataStack  m_pDataStack                        = null;
    private bool                m_IsActive                          = false;

    private bool                m_IsPlayerCharacterAttackTrigger    = false;

    private eTargetSelectType   m_eCurrTargetSelectType             = eTargetSelectType.Auto;

    private GameObject          m_pGameObject_BlackLayer            = null;

    private Dictionary<Slot, InGame_Highlight>  m_SelectRange_HighlightTable        = new Dictionary<Slot, InGame_Highlight>();
    private Dictionary<Slot, InGame_Highlight>  m_SelectTarget_HighlightTable       = new Dictionary<Slot, InGame_Highlight>();
    private List<Slot>                          m_SelectTargetSlotList              = new List<Slot>();

    private InGame_Highlight                    m_pInGame_Highlight_FirstSelect     = null;
    private InGame_Highlight                    m_pInGame_Highlight_SecondSelect    = null;

    //private Dictionary<Slot, DamageTargetInfo>  m_pDamageSlotTable  = new Dictionary<Slot, DamageTargetInfo>();
    private Dictionary<DamageTargetInfo, Slot> m_pDamageSlotTable   = new Dictionary<DamageTargetInfo, Slot>();

    private GameObject          m_pGameObject_CancelArea            = null;

    private bool                m_IsAutoPlay                        = false;
    private int                 m_nCancelAreaFingerID               = 0;

    private SlotFixObject_PlayerCharacter m_pPlayerCharacter_AutoPlay = null;

    private Transformer_Timer   m_pTimer_AutoPlayStep               = new Transformer_Timer();
    private Transformer_Timer   m_pTimer_EffectCast                 = new Transformer_Timer();
    private eOwner              m_eOwner                            = eOwner.My;

    private Coroutine           m_pCoroutine_SkillTrigger           = null;

    private bool                m_IsShowSkillCutScene               = false;

    public TurnSubComponent_PlayerCharacterAttack(TurnComponent_BlockMatch pTurnComponent_BlockMatch, eOwner eOwn)
    {
        ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);

        m_pTurnComponent_BlockMatch = pTurnComponent_BlockMatch;
        m_eOwner = eOwn;

        if ((EspressoInfo.Instance.m_eGameMode == eGameMode.PvpStage || EspressoInfo.Instance.m_eGameMode == eGameMode.EventPvpStage) && m_eOwner == eOwner.Other)
        {
            m_IsAutoPlay = true;
        }

        m_pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/BlackLayer");
        m_pGameObject_BlackLayer = GameObject.Instantiate(ob);
        m_pGameObject_BlackLayer.SetActive(false);
        Helper.SetMultipleScale_XY(m_pGameObject_BlackLayer, AppInstance.Instance.m_fHeightScale);
        //Vector3 vScale = m_pGameObject_BlackLayer.transform.localScale;
        //vScale.x *= AppInstance.Instance.m_fHeightScale;
        //vScale.y *= AppInstance.Instance.m_fHeightScale;
        //m_pGameObject_BlackLayer.transform.localScale = vScale;

        EventDelegateManager.Instance.OnEventInGame_Request_ActionTrigger_Open += OnInGame_Request_ActionTrigger_Open;
        EventDelegateManager.Instance.OnEventInGame_Request_ActionTrigger_On += OnInGame_Request_ActionTrigger_On;
        EventDelegateManager.Instance.OnEventInGame_Request_ActionTrigger_Cancel += OnInGame_Request_ActionTrigger_Cancel;
        EventDelegateManager.Instance.OnEventInGame_PlayerCharacterSkill_CutScene_UIDone += OnInGame_PlayerCharacterSkill_CutScene_UIDone;
        EventDelegateManager.Instance.OnEventInGame_Slot_Click += OnInGame_Slot_Click;
        EventDelegateManager.Instance.OnEventInGame_Projectile_Block_Done += OnInGame_Projectile_Block_Done;
        EventDelegateManager.Instance.OnEventInGame_PlayerCharacterProjectile_Done += OnInGame_PlayerCharacterProjectile_Done;
        EventDelegateManager.Instance.OnEventInGame_Projectile_SP_Charge_Done += OnInGame_Projectile_SP_Charge_Done;
        EventDelegateManager.Instance.OnEventInGame_ChangeAutoPlay += OnInGame_ChangeAutoPlay;
        EventDelegateManager.Instance.OnEventInGame_Tooltip_DestroyCancelArea += OnInGame_Tooltip_DestroyCancelArea;
    }

    public void OnDestroy()
    {
        if (m_pGameObject_BlackLayer != null)
        {
            GameObject.Destroy(m_pGameObject_BlackLayer);
            m_pGameObject_BlackLayer = null;
        }

        if(m_pGameObject_CancelArea != null)
        {
            GameObject.Destroy(m_pGameObject_CancelArea);
            m_pGameObject_CancelArea = null;
        }

        if (AppInstance.Instance != null && m_pCoroutine_SkillTrigger != null)
        {
            AppInstance.Instance.StopCoroutine(m_pCoroutine_SkillTrigger);
            m_pCoroutine_SkillTrigger = null;
        }

        EventDelegateManager.Instance.OnEventInGame_Request_ActionTrigger_Open -= OnInGame_Request_ActionTrigger_Open;
        EventDelegateManager.Instance.OnEventInGame_Request_ActionTrigger_On -= OnInGame_Request_ActionTrigger_On;
        EventDelegateManager.Instance.OnEventInGame_Request_ActionTrigger_Cancel -= OnInGame_Request_ActionTrigger_Cancel;
        EventDelegateManager.Instance.OnEventInGame_PlayerCharacterSkill_CutScene_UIDone -= OnInGame_PlayerCharacterSkill_CutScene_UIDone;
        EventDelegateManager.Instance.OnEventInGame_Slot_Click -= OnInGame_Slot_Click;
        EventDelegateManager.Instance.OnEventInGame_Projectile_Block_Done -= OnInGame_Projectile_Block_Done;
        EventDelegateManager.Instance.OnEventInGame_PlayerCharacterProjectile_Done -= OnInGame_PlayerCharacterProjectile_Done;
        EventDelegateManager.Instance.OnEventInGame_Projectile_SP_Charge_Done -= OnInGame_Projectile_SP_Charge_Done;
        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
        EventDelegateManager.Instance.OnEventInGame_ChangeAutoPlay -= OnInGame_ChangeAutoPlay;
        EventDelegateManager.Instance.OnEventInGame_BlockShuffleSkill -= OnInGame_BlockShuffleSkill;
        EventDelegateManager.Instance.OnEventInGame_Tooltip_DestroyCancelArea -= OnInGame_Tooltip_DestroyCancelArea;
    }

    public void Update()
    {
        m_pTimer_AutoPlayStep.Update(Time.deltaTime);
        m_pTimer_EffectCast.Update(Time.deltaTime);
    }

    public void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0) == true)
        {
            OutputLog.Log("TurnComponent_PlayerCharacterAttack LateUpdate MouseButtonUp");
        }
    }

    public void SetActive(bool IsActive)
    {
        InGameTurnLog.Log("TurnComponent_PlayerCharacterAttack : SetActive : " + IsActive);

        m_IsActive = IsActive;
        m_IsShowSkillCutScene = false;

        InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();

        if (m_IsActive == true)
        {
			if (m_IsAutoPlay == true)
				OnAutoPlay();
		}
        else
        {
            m_pPlayerCharacter_AutoPlay = null;
            m_pTimer_AutoPlayStep.OnStop();

            ClearHighlight();
        }
    }

    public void OnInGame_ChangeAutoPlay(bool IsAutoPlay)
    {
        if (m_eOwner == eOwner.My)
        {
            m_IsAutoPlay = IsAutoPlay;

            if (m_IsActive == true)
            {
                if (m_IsAutoPlay == true && m_IsShowSkillCutScene == false)
                {
                    OnAutoPlay();
                }
            }

            if (m_IsAutoPlay == false)
            {
                m_pPlayerCharacter_AutoPlay = null;

                if (m_IsShowSkillCutScene == false)
                {
                    OnCancelAutoPlay();
                }
            }
        }
	}

    public void OnAutoPlay()
    {
        if (InGameInfo.Instance.m_IsInGameClick == true && EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger == false)
        {
            switch (EspressoInfo.Instance.m_eGameMode)
            {
                case eGameMode.PvpStage:
                    {
                        if (m_eOwner == eOwner.My)
                        {
                            foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_PlayerCharacterTable.Reverse())
                            {
                                if (item.Value.IsFull_SP() == true && item.Value.IsDead() == false && MainGame_Espresso_ProcessHelper.IsPlayerCharacterSkillUsePossible(item.Value) == true)
                                {
                                    OnAutoPlay(item.Value);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_OtherPlayerCharacterTable.Reverse())
                            {
                                if (item.Value.IsFull_SP() == true && item.Value.IsDead() == false && MainGame_Espresso_ProcessHelper.IsPlayerCharacterSkillUsePossible(item.Value) == true)
                                {
                                    OnAutoPlay(item.Value);
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
                            foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_PlayerCharacterTable.Reverse())
                            {
                                if (item.Value.IsFull_SP() == true && item.Value.IsDead() == false && MainGame_Espresso_ProcessHelper.IsPlayerCharacterSkillUsePossible(item.Value) == true)
                                {
                                    OnAutoPlay(item.Value);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_OtherPlayerCharacterTable.Reverse())
                            {
                                if (item.Value.IsFull_SP() == true && item.Value.IsDead() == false && MainGame_Espresso_ProcessHelper.IsPlayerCharacterSkillUsePossible(item.Value) == true)
                                {
                                    OnAutoPlay(item.Value);
                                    break;
                                }
                            }
                        }
                    }
                    break;

                default:
                    {
                        foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_PlayerCharacterTable.Reverse())
                        {
                            if (item.Value.IsFull_SP() == true && item.Value.IsDead() == false && MainGame_Espresso_ProcessHelper.IsPlayerCharacterSkillUsePossible(item.Value) == true)
                            {
                                OnAutoPlay(item.Value);
                                break;
                            }
                        }
                    }
                    break;
            }
        }
    }

    private void OnAutoPlay(SlotFixObject_PlayerCharacter pPlayerCharacter_AutoPlay)
    {
        m_pPlayerCharacter_AutoPlay = pPlayerCharacter_AutoPlay;
        m_pDataStack.m_pPlayerCharacter_SkillTrigger = m_pPlayerCharacter_AutoPlay;

        m_pTimer_AutoPlayStep.OnReset();
        TransformerEvent_Timer eventValue;
        eventValue = new TransformerEvent_Timer(0.5f);
        m_pTimer_AutoPlayStep.AddEvent(eventValue);
        m_pTimer_AutoPlayStep.SetCallback(null, DoDone_Timer_AutoPlay_ActionTrigger_Open);
        m_pTimer_AutoPlayStep.OnPlay();
    }

    private void DoDone_Timer_AutoPlay_ActionTrigger_Open(TransformerEvent eventValue)
    {
        OnInGame_Request_ActionTrigger_Open();

        m_pTimer_AutoPlayStep.OnReset();
        eventValue = new TransformerEvent_Timer(1.5f);
        m_pTimer_AutoPlayStep.AddEvent(eventValue);
        m_pTimer_AutoPlayStep.SetCallback(null, DoDone_Timer_AutoPlayStep_ActionTrigger_On);
        m_pTimer_AutoPlayStep.OnPlay();
    }

    private void DoDone_Timer_AutoPlayStep_ActionTrigger_On(TransformerEvent eventValue)
    {
        if (m_pDataStack.m_pPlayerCharacter_SkillTrigger != null)
        {
            ExcelData_UnitInfo pUnitInfo = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetUnitInfo();

            if (pUnitInfo != null)
            {
                ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(pUnitInfo.m_nActiveSkill_ActionTableID);

                switch (pActionInfo.m_eDamageType)
                {
                    case eDamageEffectType.CharacterSwap:
                        {
                            foreach (KeyValuePair<Slot, InGame_Highlight> item in m_SelectTarget_HighlightTable)
                            {
                                OnInGame_Slot_Click(item.Key);

                                m_pTimer_AutoPlayStep.OnReset();
                                eventValue = new TransformerEvent_Timer(1.0f);
                                m_pTimer_AutoPlayStep.AddEvent(eventValue);
                                m_pTimer_AutoPlayStep.SetCallback(null, DoDone_Timer_AutoPlayStep_SecondSlotClick);
                                m_pTimer_AutoPlayStep.OnPlay();
                                break;
                            }
                        }
                        break;

                    default:
                        {
                            OnInGame_Request_ActionTrigger_On();
                        }
                        break;
                }
            }
        }
        else
        {
            OnInGame_Request_ActionTrigger_On();
        }
    }

    private void DoDone_Timer_AutoPlayStep_SecondSlotClick(TransformerEvent eventValue)
    {
        List<Slot> highlightList = new List<Slot>();
        foreach (KeyValuePair<Slot, InGame_Highlight> item in m_SelectRange_HighlightTable)
        {
            if (m_pInGame_Highlight_FirstSelect != null && m_pInGame_Highlight_FirstSelect.GetSlot() != item.Value.GetSlot())
            {
                highlightList.Add(item.Key);
            }
        }

        if (highlightList.Count != 0)
        {
            Helper.ShuffleList_UnityEngineRandom(highlightList);
            Helper.ShuffleList_UnityEngineRandom(highlightList);
            int nRandom = UnityEngine.Random.Range(0, highlightList.Count);
            Slot pNextTarget = highlightList[nRandom];
            OnInGame_Slot_Click(pNextTarget);
        }

        m_pTimer_AutoPlayStep.OnReset();
        eventValue = new TransformerEvent_Timer(1.0f);
        m_pTimer_AutoPlayStep.AddEvent(eventValue);
        m_pTimer_AutoPlayStep.SetCallback(null, DoDone_Timer_AutoPlayStep_SecondActionTrigger_On);
        m_pTimer_AutoPlayStep.OnPlay();
    }

    private void DoDone_Timer_AutoPlayStep_SecondActionTrigger_On(TransformerEvent eventValue)
    {
        foreach (KeyValuePair<Slot, InGame_Highlight> item in m_SelectTarget_HighlightTable)
        {
            OnInGame_Slot_Click(item.Key);
            break;
        }
    }

    private void OnCancelAutoPlay()
    {
        m_pTimer_AutoPlayStep.OnStop();

        if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner != m_eOwner)
            return;

        if (EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger == true &&
            EspressoInfo.Instance.m_IsInGame_SkillTriggerDrag == false && m_IsAutoPlay == false &&
            EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.PlayerCharacter &&
            EspressoInfo.Instance.m_IsActionAndUnitDetailCloseBlock == false)
        {
            EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Cancel();
            EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Close();
            EventDelegateManager.Instance.OnInGame_UnitDetail_Close();

            if (m_pGameObject_CancelArea != null)
            {
                GameObject.Destroy(m_pGameObject_CancelArea);
                m_pGameObject_CancelArea = null;
            }

            if (m_pDataStack.m_IsCurrRefillGuideState == true && m_eOwner == eOwner.My)
            {
                GameObject ob_RefillGuide = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGameRefillGuide");
                GameObject.Instantiate(ob_RefillGuide);
            }
        }

        EspressoInfo.Instance.m_IsActionAndUnitDetailCloseBlock = false;
    }

    private void ClearHighlight()
    {
        foreach (KeyValuePair<Slot, InGame_Highlight> item in m_SelectRange_HighlightTable)
        {
            item.Key.ClearHighlight();
            GameObject.Destroy(item.Value.gameObject);
        }
        m_SelectRange_HighlightTable.Clear();

        foreach (KeyValuePair<Slot, InGame_Highlight> item in m_SelectTarget_HighlightTable)
        {
            item.Key.ClearHighlight();
            GameObject.Destroy(item.Value.gameObject);
        }
        m_SelectTarget_HighlightTable.Clear();

        foreach (KeyValuePair<InGame_Highlight, Slot> item in m_pDataStack.m_DamageTarget_HighlightTable)
        {
            item.Value.ClearHighlight();
            GameObject.Destroy(item.Key.gameObject);
        }
        m_pDataStack.m_DamageTarget_HighlightTable.Clear();
    }

	private void OnInGame_Request_ActionTrigger_Open()
    {
        if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner != m_eOwner)
            return;

        OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnInGame_Request_ActionTrigger_Open");

        EspressoInfo.Instance.m_eActionTriggerSubject = eActionTriggerSubject.PlayerCharacter;

        GameObject ob;

        if (InGameInfo.Instance.m_pSelectModeSlot != null)
        {
            InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
            InGameInfo.Instance.m_pSelectModeSlot = null;
        }

        ClearHighlight();
        m_SelectTargetSlotList.Clear();
        m_pInGame_Highlight_FirstSelect = null;
        m_pInGame_Highlight_SecondSelect = null;

        if (m_IsActive == true && m_IsPlayerCharacterAttackTrigger == false && 
            m_pDataStack.m_pPlayerCharacter_SkillTrigger != null && m_pDataStack.m_pPlayerCharacter_SkillTrigger.IsFull_SP() == true &&
            m_pDataStack.m_pPlayerCharacter_SkillTrigger.IsDead() == false && m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetOwner() == m_eOwner &&
            EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == m_eOwner &&
            m_pDataStack.m_nBlockProjectileCount == 0 && InGameInfo.Instance.m_IsInGameClick == true &&
            m_pDataStack.m_pPlayerCharacter_SkillTrigger.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.AttackBlock) == false)
        {
            OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnInGame_Request_ActionTrigger_Open Opened");

            bool IsAbleSkill = false;

            ExcelData_UnitInfo pUnitInfo = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetUnitInfo();

            if (pUnitInfo != null)
            {
                Dictionary<bool,int> chekcSkillTable = new Dictionary<bool,int>();    // Key : Is SubSkill, Value : Action ID
                chekcSkillTable.Add(false, pUnitInfo.m_nActiveSkill_ActionTableID);
                if (pUnitInfo.m_eSubSkillCondition == eSubSkillCondition.ActiveSkill)
                {
                    List<bool> probabilityList = new List<bool>();
                    probabilityList = pUnitInfo.m_SubSkillProbabilityList.ToList<bool>();

                    Helper.ShuffleList(probabilityList, EspressoInfo.Instance.m_nDamageTargetRandomSeed);
                    Helper.ShuffleList(probabilityList, EspressoInfo.Instance.m_nDamageTargetRandomSeed);

                    if (probabilityList[0] == true)
                    {
                        chekcSkillTable.Add(true, pUnitInfo.m_nSubSkill_ActionTableID);
                    }
                }

                foreach (KeyValuePair<bool, int> item in chekcSkillTable)
                {
                    ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(item.Value);

                    if (pActionInfo != null)
                    {
                        switch (pActionInfo.m_eDamageType)
                        {
                            case eDamageEffectType.ShieldPointCharge:
                                {
                                    if (EspressoInfo.Instance.m_eGameMode != eGameMode.PvpStage && EspressoInfo.Instance.m_eGameMode != eGameMode.EventPvpStage)
                                    {
                                        EventDelegateManager.Instance.OnInGame_ShieldSelecTarget(true);

                                        Slot pSelectSlot = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetSlot();

                                        if (m_SelectTarget_HighlightTable.ContainsKey(pSelectSlot) == false && item.Key == false)
                                        {
                                            GameObject ob_Highlight = null;

                                            SlotFixObject_PlayerCharacter pPlayerCharacter = pSelectSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                            if (pPlayerCharacter != null)
                                            {
                                                string strPos = "_Center";
                                                if (pSelectSlot.GetX() == 0) strPos = "_Right";
                                                else if (pSelectSlot.GetX() == InGameInfo.Instance.m_nSlotCount_X - 1) strPos = "_Left";

                                                if (m_eOwner == pPlayerCharacter.GetOwner())
                                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Green" + strPos);
                                                else
                                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Red" + strPos);

                                                ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                            }
                                            else
                                            {
                                                if (pSelectSlot.GetSlotBlock() != null)
                                                {
                                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                    ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                    //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                    Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                }
                                                else
                                                {
                                                    SlotFixObject_Espresso pSlotFixObject = pSelectSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                                                    if (pSlotFixObject != null && pSlotFixObject.GetOwner() == m_eOwner)
                                                    {
                                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Green");
                                                        ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                        //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                        Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                    }
                                                    else
                                                    {
                                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                        ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                        //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                        Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                    }
                                                }
                                            }

                                            InGame_Highlight pHighlight = ob_Highlight.GetComponent<InGame_Highlight>();
                                            pHighlight.Initialize(ePlayerCharacterHighlight.SelectTarget, pSelectSlot, null);
                                            pHighlight.SetSubSkillStatusEffect(item.Key);

                                            m_SelectTarget_HighlightTable.Add(pSelectSlot, pHighlight);
                                            pSelectSlot.SetHighlight();

                                            m_SelectTargetSlotList.Add(pSelectSlot);
                                        }

                                        m_eCurrTargetSelectType = eTargetSelectType.Auto;
                                        IsAbleSkill = true;
                                    }
                                }
                                break;

                            default:
                                {
                                    CharacterInvenItemInfo pCharacterInfo = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetCharacterInvenItemInfo();
                                    ExcelData_Action_LevelUpInfo pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(pActionInfo.m_nID, pCharacterInfo.m_nAction_Level);

                                    SelectTargetGenerator pSelectTargetGenerator = new SelectTargetGenerator();
                                    pSelectTargetGenerator.OnGenerator(pActionInfo.m_nID, m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetSlot(), m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetOwner(),
                                                                        m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetObjectType(), eAttackType.Skill, pActionLevelUpInfo.m_nChange_SelectTargetRangeLevel);

                                    if (item.Key == false)
                                    {
                                        m_eCurrTargetSelectType = pActionInfo.m_eTargetSelectType;
                                    }

                                    switch (pActionInfo.m_eTargetSelectType)
                                    {
                                        case eTargetSelectType.Auto:
                                            {
                                                for (int i = 0; i < pActionLevelUpInfo.m_nChange_SelectTargetAmount; ++i)
                                                {
                                                    SelectTargetInfo pSelectTargetInfo = pSelectTargetGenerator.GetSelectTargetInfo_byIndex(i);

                                                    if (pSelectTargetInfo != null)
                                                    {
                                                        int nSelect_X = pSelectTargetInfo.m_nSelectTargetSlotX;
                                                        int nSelect_Y = pSelectTargetInfo.m_nSelectTargetSlotY;

                                                        if (m_pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nSelect_X, nSelect_Y)) == true)
                                                        {
                                                            Slot pSelectSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nSelect_X, nSelect_Y)];

                                                            if (m_SelectTarget_HighlightTable.ContainsKey(pSelectSlot) == false && item.Key == false)
                                                            {
                                                                GameObject ob_Highlight = null;

                                                                SlotFixObject_PlayerCharacter pPlayerCharacter = pSelectSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                                                if (pPlayerCharacter != null)
                                                                {
                                                                    string strPos = "_Center";
                                                                    if (nSelect_X == 0) strPos = "_Right";
                                                                    else if (nSelect_X == InGameInfo.Instance.m_nSlotCount_X - 1) strPos = "_Left";

                                                                    if (m_eOwner == pPlayerCharacter.GetOwner())
                                                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Green" + strPos);
                                                                    else
                                                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Red" + strPos);

                                                                    ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                    //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                    Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                }
                                                                else
                                                                {
                                                                    if (pSelectSlot.GetSlotBlock() != null)
                                                                    {
                                                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                                        ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                        //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                        Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                    }
                                                                    else
                                                                    {
                                                                        SlotFixObject_Espresso pSlotFixObject = pSelectSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                                                                        if (pSlotFixObject != null && pSlotFixObject.GetOwner() == m_eOwner)
                                                                        {
                                                                            ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Green");
                                                                            ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                            //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                            Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                        }
                                                                        else
                                                                        {
                                                                            ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                                            ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                            //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                            Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                        }
                                                                    }
                                                                }

                                                                InGame_Highlight pHighlight = ob_Highlight.GetComponent<InGame_Highlight>();
                                                                pHighlight.Initialize(ePlayerCharacterHighlight.SelectTarget, pSelectSlot, null);
                                                                pHighlight.SetSubSkillStatusEffect(item.Key);

                                                                m_SelectTarget_HighlightTable.Add(pSelectSlot, pHighlight);
                                                                pSelectSlot.SetHighlight();

                                                                m_SelectTargetSlotList.Add(pSelectSlot);
                                                            }

                                                            DamageTargetGenerator pDamageTargetGenerator = new DamageTargetGenerator();
                                                            pDamageTargetGenerator.OnGenerator(pActionInfo.m_nID, pSelectSlot, m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetOwner(),
                                                                                               m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetObjectType(), eAttackType.Skill, pActionLevelUpInfo);

                                                            int nNumDamageTarget = pDamageTargetGenerator.GetNumDamageTargetInfo();

                                                            for (int j = 0; j < nNumDamageTarget; ++j)
                                                            {
                                                                DamageTargetInfo pDamageTargetInfo = pDamageTargetGenerator.GetDamageTargetInfo_byIndex(j);

                                                                if (pDamageTargetInfo != null)
                                                                {
                                                                    int nDamage_X = nSelect_X + pDamageTargetInfo.m_pRangeInfo.m_nPosX;
                                                                    int nDamage_Y = nSelect_Y + pDamageTargetInfo.m_pRangeInfo.m_nPosY;

                                                                    if (m_eOwner == eOwner.My || pDamageTargetInfo.m_pRangeInfo.m_eRangeType == eRangeType.All)
                                                                    {
                                                                        nDamage_Y = nSelect_Y + pDamageTargetInfo.m_pRangeInfo.m_nPosY * -1;
                                                                    }

                                                                    if (m_pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nDamage_X, nDamage_Y)) == true)
                                                                    {
                                                                        Slot pDamageSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nDamage_X, nDamage_Y)];

                                                                        GameObject ob_Highlight = null;

                                                                        SlotFixObject_PlayerCharacter pPlayerCharacter = pDamageSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                                                        if (pPlayerCharacter != null)
                                                                        {
                                                                            string strPos = "_Center";
                                                                            if (nSelect_X == 0) strPos = "_Right";
                                                                            else if (nSelect_X == InGameInfo.Instance.m_nSlotCount_X - 1) strPos = "_Left";

                                                                            if (m_eOwner == pPlayerCharacter.GetOwner())
                                                                                ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Green" + strPos);
                                                                            else
                                                                                ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Red" + strPos);

                                                                            ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                            //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                            Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                        }
                                                                        else
                                                                        {
                                                                            if (pDamageSlot.GetSlotBlock() != null)
                                                                            {
                                                                                ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                                                ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                                //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                                Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                            }
                                                                            else
                                                                            {
                                                                                SlotFixObject_Espresso pSlotFixObject = pDamageSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                                                                                if (pSlotFixObject != null && pSlotFixObject.GetOwner() == m_eOwner)
                                                                                {
                                                                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Green");
                                                                                    ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                                    //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                                    Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                                }
                                                                                else
                                                                                {
                                                                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                                                    ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                                    //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                                    Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                                }
                                                                            }
                                                                        }

                                                                        InGame_Highlight pHighlight = ob_Highlight.GetComponent<InGame_Highlight>();
                                                                        pHighlight.Initialize(ePlayerCharacterHighlight.DamageTarget, pDamageSlot, pDamageTargetInfo);
                                                                        pHighlight.SetSubSkillStatusEffect(item.Key);

                                                                        m_pDataStack.m_DamageTarget_HighlightTable.Add(pHighlight, pDamageSlot);
                                                                        pDamageSlot.SetHighlight();

                                                                        IsAbleSkill = true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                if (m_pDataStack.m_DamageTarget_HighlightTable.Count == 0)
                                                {
                                                    ClearHighlight();
                                                    IsAbleSkill = false;
                                                }
                                            }
                                            break;

                                        case eTargetSelectType.ManualAndAuto:
                                            {
                                                int nRealSelectTarget = 0;
                                                int nNumSelectTarget = pSelectTargetGenerator.GetNumSelectTargetInfo();
                                                for (int i = 0; i < nNumSelectTarget; ++i)
                                                {
                                                    SelectTargetInfo pSelectTargetInfo = pSelectTargetGenerator.GetSelectTargetInfo_byIndex(i);

                                                    if (pSelectTargetInfo != null)
                                                    {
                                                        int nSelect_X = pSelectTargetInfo.m_nSelectTargetSlotX;
                                                        int nSelect_Y = pSelectTargetInfo.m_nSelectTargetSlotY;

                                                        if (m_pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nSelect_X, nSelect_Y)) == true)
                                                        {
                                                            Slot pSelectSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nSelect_X, nSelect_Y)];

                                                            if (m_SelectRange_HighlightTable.ContainsKey(pSelectSlot) == false && item.Key == false)
                                                            {
                                                                GameObject ob_Highlight = null;

                                                                SlotFixObject_PlayerCharacter pPlayerCharacter = pSelectSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                                                if (pPlayerCharacter != null)
                                                                {
                                                                    string strPos = "_Center";
                                                                    if (nSelect_X == 0) strPos = "_Right";
                                                                    else if (nSelect_X == InGameInfo.Instance.m_nSlotCount_X - 1) strPos = "_Left";

                                                                    if (m_eOwner == pPlayerCharacter.GetOwner())
                                                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Green" + strPos);
                                                                    else
                                                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Red" + strPos);

                                                                    ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                    //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                    Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                }
                                                                else
                                                                {
                                                                    if (pSelectSlot.GetSlotBlock() != null)
                                                                    {
                                                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                                        ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                        //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                        Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                    }
                                                                    else
                                                                    {
                                                                        SlotFixObject_Espresso pSlotFixObject = pSelectSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                                                                        if (pSlotFixObject != null && pSlotFixObject.GetOwner() == m_eOwner)
                                                                        {
                                                                            ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Green");
                                                                            ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                            //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                            Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                        }
                                                                        else
                                                                        {
                                                                            ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                                            ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                            //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                            Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                        }
                                                                    }
                                                                }

                                                                InGame_Highlight pHighlight = ob_Highlight.GetComponent<InGame_Highlight>();
                                                                pHighlight.Initialize(ePlayerCharacterHighlight.SelectRange, pSelectSlot, null);

                                                                m_SelectRange_HighlightTable.Add(pSelectSlot, pHighlight);
                                                                pSelectSlot.SetHighlight();

                                                                ++nRealSelectTarget;
                                                                IsAbleSkill = true;
                                                            }
                                                        }
                                                    }
                                                }

                                                List<SelectTargetInfo> firstPrioritySelectTargetInfoList = pSelectTargetGenerator.GetFirstPrioritySelectTargetInfoList();

                                                if (firstPrioritySelectTargetInfoList.Count != 0)
                                                {
                                                    int nCount = firstPrioritySelectTargetInfoList.Count;
                                                    int nRandom = UnityEngine.Random.Range(0, nCount);

                                                    SelectTargetInfo pFirstSelectTargetInfo = firstPrioritySelectTargetInfoList[nRandom];

                                                    if (pFirstSelectTargetInfo != null)
                                                    {
                                                        int nSlotIndex = Helper.GetSlotIndex(pFirstSelectTargetInfo.m_nSelectTargetSlotX, pFirstSelectTargetInfo.m_nSelectTargetSlotY);

                                                        if (m_pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(nSlotIndex) == true)
                                                        {
                                                            m_IsPlayerCharacterAttackTrigger = true;
                                                            OnInGame_Slot_Click(m_pDataStack.m_pSlotManager.GetSlotTable()[nSlotIndex]);

                                                            if (m_pDataStack.m_DamageTarget_HighlightTable.Count == 0)
                                                            {
                                                                ClearHighlight();
                                                                IsAbleSkill = false;
                                                            }
                                                        }
                                                    }
                                                }

                                                if (pActionInfo.m_eDamageType == eDamageEffectType.CharacterSwap && item.Key == false)
                                                {
                                                    if (nRealSelectTarget < 2)
                                                    {
                                                        IsAbleSkill = false;
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }

                    if (item.Key == false && IsAbleSkill == false)
                    {
                        break;
                    }
                }
            }

			if (IsAbleSkill == false && m_pDataStack.m_pPlayerCharacter_SkillTrigger != null && m_pDataStack.m_pPlayerCharacter_SkillTrigger.IsFull_SP() == true &&
			    m_pDataStack.m_pPlayerCharacter_SkillTrigger.IsDead() == false)
			{
				GameObject ob_Ms = Resources.Load<GameObject>("GUI/Prefabs/Common/CommonMessageBox");
				ob_Ms = GameObject.Instantiate(ob_Ms);
				CommonMessageBox_UI pScript = ob_Ms.GetComponent<CommonMessageBox_UI>();
				pScript.Init(ExcelDataHelper.GetString("ACTION_MESSAGE_NO_VAILD_TARGET"), 0, 0.7f, 0.2f);
			}
        }

        if (m_IsAutoPlay == false)
        {
            if (m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetUnitInfo().m_nActiveSkill_ActionTableID != 0 && m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetMaxSP() != 0)
            {
                EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Close();
                ob = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGameActionDetail");
                GameObject.Instantiate(ob);
            }

            EventDelegateManager.Instance.OnInGame_UnitDetail_Close();
            GameObject ob_unit_detail = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGameUnitDetail");
            ob_unit_detail = GameObject.Instantiate(ob_unit_detail);
            InGame_UnitDetail_UI pScript = ob_unit_detail.GetComponent<InGame_UnitDetail_UI>();
            pScript.Init(m_pDataStack.m_pPlayerCharacter_SkillTrigger);

            if (m_IsActive == true && m_IsPlayerCharacterAttackTrigger == false &&
                m_pDataStack.m_pPlayerCharacter_SkillTrigger != null && m_pDataStack.m_pPlayerCharacter_SkillTrigger.IsFull_SP() == true &&
                m_pDataStack.m_pPlayerCharacter_SkillTrigger.IsDead() == false && m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetOwner() == m_eOwner &&
                EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == m_eOwner &&
                m_pDataStack.m_nBlockProjectileCount == 0 && InGameInfo.Instance.m_IsInGameClick == true &&
                m_pDataStack.m_pPlayerCharacter_SkillTrigger.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.AttackBlock) == true)
            {
                GameObject ob_Ms = Resources.Load<GameObject>("GUI/Prefabs/Common/CommonMessageBox");
                ob_Ms = GameObject.Instantiate(ob_Ms);
                CommonMessageBox_UI pScript_Mess = ob_Ms.GetComponent<CommonMessageBox_UI>();
                pScript_Mess.Init(ExcelDataHelper.GetString("ACTION_MESSAGE_ATTACK_HOLD_BY_STATUS_EFFECT"), 0, 0.7f, 0.2f);
            }
        }

        EventDelegateManager.Instance.OnInGame_RefillGuide_UI_Destroy();

        if (m_pDataStack.m_pPlayerCharacter_SkillTrigger != null)
        {
            m_pDataStack.m_pPlayerCharacter_SkillTrigger.SetHighlight();
        }

        if (m_pGameObject_BlackLayer != null)
            m_pGameObject_BlackLayer.SetActive(true);

        EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger = true;
        m_IsPlayerCharacterAttackTrigger = true;

        if (m_pGameObject_CancelArea != null) GameObject.Destroy(m_pGameObject_CancelArea);
        ob = Resources.Load<GameObject>("2D/Prefabs/Character/PlayerCharacterAttackCancelArea");
        m_pGameObject_CancelArea = GameObject.Instantiate(ob);
        Plane2D pPlane_CancelArea = m_pGameObject_CancelArea.GetComponent<Plane2D>();
        pPlane_CancelArea.AddCallback_LButtonDown(OnCallback_LButtonDown_CancelArea);
        pPlane_CancelArea.AddCallback_LButtonUp(OnCallback_LButtonUp_CancelArea);
    }

    public void OnInGame_Slot_Click(Slot pSlot)
    {
        if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner != m_eOwner)
            return;

        if(m_IsActive == false)
            return;

        OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnInGame_Slot_Click");

        if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.PlayerCharacter)
        {
            GameObject ob;

            if (m_IsPlayerCharacterAttackTrigger == true && m_IsActive == true)
            {
                ExcelData_UnitInfo pUnitInfo = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetUnitInfo();

                if (pUnitInfo != null)
                {
                    ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(pUnitInfo.m_nActiveSkill_ActionTableID);

                    if (pActionInfo != null)
                    {
                        if (m_SelectTarget_HighlightTable.ContainsKey(pSlot) == true || m_pInGame_Highlight_FirstSelect != null)
                        {
                            switch (pActionInfo.m_eDamageType)
                            {
                                case eDamageEffectType.CharacterSwap:
                                    {
                                        if (m_pInGame_Highlight_FirstSelect == null)
                                        {
                                            Helper.OnSoundPlay("UI_CHARACTER_TOUCH", false);
                                            m_pInGame_Highlight_FirstSelect = m_SelectTarget_HighlightTable[pSlot];

                                            // 첫 선택 타겟 처리
                                            SlotFixObject_PlayerCharacter pPlayerCharacter = pSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                            if (pPlayerCharacter != null)
                                            {
                                                pPlayerCharacter.SetSkillFirstSelect(true);
                                            }

                                            m_pInGame_Highlight_FirstSelect.gameObject.SetActive(false);
                                            m_SelectTarget_HighlightTable.Remove(pSlot);

                                            List<Slot> highlightList = new List<Slot>();
                                            foreach (KeyValuePair<Slot, InGame_Highlight> item in m_SelectRange_HighlightTable)
                                            {
                                                if (m_pInGame_Highlight_FirstSelect != null && m_pInGame_Highlight_FirstSelect.GetSlot() != item.Value.GetSlot())
                                                {
                                                    highlightList.Add(item.Key);
                                                }
                                            }

                                            if (highlightList.Count != 0)
                                            {
                                                Helper.ShuffleList_UnityEngineRandom(highlightList);
                                                Helper.ShuffleList_UnityEngineRandom(highlightList);
                                                int nRandom = UnityEngine.Random.Range(0, highlightList.Count);
                                                Slot pNextTarget = highlightList[nRandom];
                                                OnInGame_Slot_Click(pNextTarget);
                                            }

                                            return;
                                        }
                                        else if (m_pInGame_Highlight_FirstSelect.GetSlot() == pSlot)
                                        {
                                            Helper.OnSoundPlay("UI_CHARACTER_TOUCH", false);
                                            // 첫 선택 타겟 해제 처리
                                            Slot pSlot_FirstSelect = m_pInGame_Highlight_FirstSelect.GetSlot();
                                            SlotFixObject_PlayerCharacter pPlayerCharacter = pSlot_FirstSelect.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                            if (pPlayerCharacter != null)
                                            {
                                                pPlayerCharacter.SetSkillFirstSelect(false);
                                            }

                                            if (m_SelectTarget_HighlightTable.ContainsKey(pSlot) == true)
                                            {
                                                GameObject.Destroy(m_pInGame_Highlight_FirstSelect.gameObject);
                                                m_SelectTarget_HighlightTable.Remove(pSlot);
                                            }

                                            m_pInGame_Highlight_FirstSelect = null;
                                        }
                                        else if(m_SelectTarget_HighlightTable.ContainsKey(pSlot) == true)
                                        {
                                            Helper.OnSoundPlay("UI_CHARACTER_TOUCH", false);
                                            m_pInGame_Highlight_SecondSelect = m_SelectTarget_HighlightTable[pSlot];

                                            Slot pSlot_FirstSelect = m_pInGame_Highlight_FirstSelect.GetSlot();
                                            SlotFixObject_PlayerCharacter pPlayerCharacter = pSlot_FirstSelect.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                            if (pPlayerCharacter != null)
                                            {
                                                pPlayerCharacter.SetSkillFirstSelect(false);
                                            }

                                            EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Close();
                                            EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_On();
                                            EventDelegateManager.Instance.OnInGame_UnitDetail_Close();
                                            return;
                                        }
                                    }
                                    break;

                                default:
                                    {
                                        EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Close();
                                        EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_On();
                                        EventDelegateManager.Instance.OnInGame_UnitDetail_Close();
                                        return;
                                    }
                            }

                            EventDelegateManager.Instance.OnInGame_ShieldSelecTarget(false);
                        }

                        if (m_SelectRange_HighlightTable.ContainsKey(pSlot) == true)
                        {
                            OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnInGame_Slot_Click In");

                            CharacterInvenItemInfo pCharacterInfo = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetCharacterInvenItemInfo();
                            ExcelData_Action_LevelUpInfo pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(pActionInfo.m_nID, pCharacterInfo.m_nAction_Level);

                            if (m_eCurrTargetSelectType == eTargetSelectType.ManualAndAuto)
                            {
                                foreach (KeyValuePair<Slot, InGame_Highlight> item in m_SelectTarget_HighlightTable)
                                {
                                    GameObject.Destroy(item.Value.gameObject);
                                }
                                m_SelectTarget_HighlightTable.Clear();
                                m_SelectTargetSlotList.Clear();

                                Dictionary<InGame_Highlight, Slot> removeInGameHighlightTable = new Dictionary<InGame_Highlight, Slot>();
                                foreach (KeyValuePair<InGame_Highlight, Slot> item in m_pDataStack.m_DamageTarget_HighlightTable)
                                {
                                    if (item.Key.IsSubSkillStatusEffect() == false)
                                    {
                                        GameObject.Destroy(item.Key.gameObject);
                                        removeInGameHighlightTable.Add(item.Key, item.Value);
                                    }
                                }

                                foreach (KeyValuePair<InGame_Highlight, Slot> item in removeInGameHighlightTable)
                                {
                                    m_pDataStack.m_DamageTarget_HighlightTable.Remove(item.Key);
                                }

                                GameObject ob_Highlight = null;

                                SlotFixObject_PlayerCharacter pPlayerCharacter = pSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                if (pPlayerCharacter != null)
                                {
                                    string strPos = "_Center";
                                    if (pSlot.GetX() == 0) strPos = "_Right";
                                    else if (pSlot.GetX() == InGameInfo.Instance.m_nSlotCount_X - 1) strPos = "_Left";

                                    if (m_eOwner == pPlayerCharacter.GetOwner())
                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Green" + strPos);
                                    else
                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Red" + strPos);

                                    ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                    //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                    Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                }
                                else
                                {
                                    if (pSlot.GetSlotBlock() != null)
                                    {
                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                        ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                        //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                        Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                    }
                                    else
                                    {
                                        SlotFixObject_Espresso pSlotFixObject = pSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                                        if (pSlotFixObject != null && pSlotFixObject.GetOwner() == m_eOwner)
                                        {
                                            ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Green");
                                            ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                            //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                            Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                        }
                                        else
                                        {
                                            ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                            ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                            //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                            Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                        }
                                    }
                                }

                                InGame_Highlight pHighlight = ob_Highlight.GetComponent<InGame_Highlight>();
                                pHighlight.Initialize(ePlayerCharacterHighlight.SelectTarget, pSlot, null);

                                m_SelectTarget_HighlightTable.Add(pSlot, pHighlight);
                                pSlot.SetHighlight();
                                m_SelectTargetSlotList.Add(pSlot);

                                foreach (KeyValuePair<Slot, InGame_Highlight> item in m_SelectRange_HighlightTable)
                                {
                                    item.Value.gameObject.SetActive(true);
                                }

                                Helper.OnSoundPlay("INGAME_SKILL_TARGET_SELECT", false);

                                DamageTargetGenerator pDamageTargetGenerator = new DamageTargetGenerator();
                                pDamageTargetGenerator.OnGenerator(pActionInfo.m_nID, pSlot, m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetOwner(),
                                                                   m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetObjectType(), eAttackType.Skill, pActionLevelUpInfo);

                                int nNumDamageTarget = pDamageTargetGenerator.GetNumDamageTargetInfo();

                                for (int j = 0; j < nNumDamageTarget; ++j)
                                {
                                    DamageTargetInfo pDamageTargetInfo = pDamageTargetGenerator.GetDamageTargetInfo_byIndex(j);

                                    if (pDamageTargetInfo != null)
                                    {
                                        int nDamage_X = pSlot.GetX() + pDamageTargetInfo.m_pRangeInfo.m_nPosX;
                                        int nDamage_Y = pSlot.GetY() + pDamageTargetInfo.m_pRangeInfo.m_nPosY;

                                        if (m_eOwner == eOwner.My || pDamageTargetInfo.m_pRangeInfo.m_eRangeType == eRangeType.All)
                                        {
                                            nDamage_Y = pSlot.GetY() + pDamageTargetInfo.m_pRangeInfo.m_nPosY * -1;
                                        }

                                        if (m_pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nDamage_X, nDamage_Y)) == true)
                                        {
                                            Slot pDamageSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nDamage_X, nDamage_Y)];

                                            //if (m_pDataStack.m_DamageTarget_HighlightTable.ContainsKey(pDamageSlot) == false)
                                            {
                                                pPlayerCharacter = pDamageSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                                if (pPlayerCharacter != null)
                                                {
                                                    string strPos = "_Center";
                                                    if (pDamageSlot.GetX() == 0) strPos = "_Right";
                                                    else if (pDamageSlot.GetX() == InGameInfo.Instance.m_nSlotCount_X - 1) strPos = "_Left";

                                                    if (m_eOwner == pPlayerCharacter.GetOwner())
                                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Green" + strPos);
                                                    else
                                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Red" + strPos);

                                                    ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                    //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                    Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                }
                                                else
                                                {
                                                    if (pDamageSlot.GetSlotBlock() != null)
                                                    {
                                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                        ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                        //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                        Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                    }
                                                    else
                                                    {
                                                        SlotFixObject_Espresso pSlotFixObject = pDamageSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                                                        if (pSlotFixObject != null && pSlotFixObject.GetOwner() == m_eOwner)
                                                        {
                                                            ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Green");
                                                            ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                            //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                            Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                        }
                                                        else
                                                        {
                                                            ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                            ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                            //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                            Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                        }
                                                    }
                                                }

                                                pHighlight = ob_Highlight.GetComponent<InGame_Highlight>();
                                                pHighlight.Initialize(ePlayerCharacterHighlight.DamageTarget, pDamageSlot, pDamageTargetInfo);

                                                m_pDataStack.m_DamageTarget_HighlightTable.Add(pHighlight, pDamageSlot);
                                                pDamageSlot.SetHighlight();

                                                if (m_SelectRange_HighlightTable.ContainsKey(pDamageSlot) == true)
                                                {
                                                    m_SelectRange_HighlightTable[pDamageSlot].gameObject.SetActive(false);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnInGame_Request_ActionTrigger_On()
    {
        if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner != m_eOwner)
            return;

        OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnInGame_Request_ActionTrigger_On");

        if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.PlayerCharacter)
        {
            if (m_pGameObject_CancelArea != null)
            {
                GameObject.Destroy(m_pGameObject_CancelArea);
                m_pGameObject_CancelArea = null;
            }

            GameObject ob = Resources.Load("GUI/Prefabs/InGame/InGameCutScene") as GameObject;
            ob = GameObject.Instantiate(ob);
            m_IsShowSkillCutScene = true;

            m_pDamageSlotTable.Clear();

            ExcelData_UnitInfo pUnitInfo = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetUnitInfo();

            if (pUnitInfo != null)
            {
                ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(pUnitInfo.m_nActiveSkill_ActionTableID);

                if (pActionInfo != null)
                {
                    switch (pActionInfo.m_eDamageType)
                    {
                        case eDamageEffectType.CharacterSwap:
                            {
                                CharacterInvenItemInfo pCharacterInfo = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetCharacterInvenItemInfo();
                                ExcelData_Action_LevelUpInfo pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(pActionInfo.m_nID, pCharacterInfo.m_nAction_Level);

                                DamageTargetGenerator pDamageTargetGenerator;

                                pDamageTargetGenerator = new DamageTargetGenerator();
                                pDamageTargetGenerator.OnGenerator(pActionInfo.m_nID, m_pInGame_Highlight_FirstSelect.GetSlot(), m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetOwner(),
                                                                   m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetObjectType(), eAttackType.Skill, pActionLevelUpInfo);

                                int nNumDamageTarget = pDamageTargetGenerator.GetNumDamageTargetInfo();
                                if (nNumDamageTarget != 0)
                                {
                                    DamageTargetInfo pDamageTargetInfo = pDamageTargetGenerator.GetDamageTargetInfo_byIndex(0);
                                    m_pDamageSlotTable.Add(pDamageTargetInfo, m_pInGame_Highlight_FirstSelect.GetSlot());
                                }

                                pDamageTargetGenerator = new DamageTargetGenerator();
                                pDamageTargetGenerator.OnGenerator(pActionInfo.m_nID, m_pInGame_Highlight_SecondSelect.GetSlot(), m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetOwner(),
                                                                   m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetObjectType(), eAttackType.Skill, pActionLevelUpInfo);

                                nNumDamageTarget = pDamageTargetGenerator.GetNumDamageTargetInfo();
                                if (nNumDamageTarget != 0)
                                {
                                    DamageTargetInfo pDamageTargetInfo = pDamageTargetGenerator.GetDamageTargetInfo_byIndex(0);
                                    m_pDamageSlotTable.Add(pDamageTargetInfo, m_pInGame_Highlight_SecondSelect.GetSlot());
                                }
                            }
                            break;

                        case eDamageEffectType.ShieldPointCharge:
                            {
                                EventDelegateManager.Instance.OnInGame_ShieldSelecTarget(false);

                                foreach (KeyValuePair<InGame_Highlight, Slot> item in m_pDataStack.m_DamageTarget_HighlightTable)
                                {
                                    m_pDamageSlotTable.Add(item.Key.GetAction_DamageTargetInfo(), item.Value);
                                }
                            }
                            break;

                        default:
                            {
                                foreach (KeyValuePair<InGame_Highlight, Slot> item in m_pDataStack.m_DamageTarget_HighlightTable)
                                {
                                    m_pDamageSlotTable.Add(item.Key.GetAction_DamageTargetInfo(), item.Value);
                                }
                            }
                            break;
                    }
                }
            }

            ClearHighlight();

            m_pDataStack.m_pPlayerCharacter_SkillTrigger.ClearHighlight();

            if (m_pGameObject_BlackLayer != null)
                m_pGameObject_BlackLayer.SetActive(false);

            InGameInfo.Instance.m_nClickSlotFingerID = -1;
            InGameInfo.Instance.m_pClickSlot = null;

            Helper.OnSoundPlay("INGAME_SKILL_CUTSCENE_START", false);
        }
    }

    private void OnInGame_Request_ActionTrigger_Cancel()
    {
        if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner != m_eOwner)
            return;

        m_IsShowSkillCutScene = false;
        InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();

        OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnInGame_Request_ActionTrigger_Cancel");

        if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.PlayerCharacter)
        {
            EspressoInfo.Instance.m_eActionTriggerSubject = eActionTriggerSubject.None;

            if (m_pInGame_Highlight_FirstSelect != null)
            {
                Slot pSlot_FirstSelect = m_pInGame_Highlight_FirstSelect.GetSlot();
                SlotFixObject_PlayerCharacter pPlayerCharacter = pSlot_FirstSelect.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                if (pPlayerCharacter != null)
                {
                    pPlayerCharacter.SetSkillFirstSelect(false);
                }

                m_pInGame_Highlight_FirstSelect = null;
            }

            ClearHighlight();

            m_IsPlayerCharacterAttackTrigger = false;
            InGameInfo.Instance.m_IsInGameClick = true;
            EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger = false;

            m_pDataStack.m_pPlayerCharacter_SkillTrigger.ClearHighlight();
            m_pDataStack.m_pPlayerCharacter_SkillTrigger = null;
            if (m_pGameObject_BlackLayer != null)
                m_pGameObject_BlackLayer.SetActive(false);

            EventDelegateManager.Instance.OnInGame_ShieldSelecTarget(false);
        }
    }

    public void OnInGame_PlayerCharacterSkill_CutScene_UIDone()
    {
        if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner != m_eOwner)
            return;

        OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnInGame_PlayerCharacterSkill_CutScene_UIDone");

        if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.PlayerCharacter)
        {
            if (m_pDataStack.m_pPlayerCharacter_SkillTrigger == null)
                return;

            InGameInfo.Instance.m_IsPlayerCharacterSkillToSpeicalBlockAttack = false;
            ExcelData_UnitInfo pUnitInfo = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetUnitInfo();
            Slot pSlot = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetSlot();

            if (pUnitInfo != null)
            {
                ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(pUnitInfo.m_nActiveSkill_ActionTableID);

                if (pActionInfo != null)
                {
                    if (pActionInfo.m_strEffectCast != "0")
                    {
                        m_pTimer_EffectCast.OnReset();
                        TransformerEvent_Timer eventValue;
                        eventValue = new TransformerEvent_Timer(0.5f);
                        m_pTimer_EffectCast.AddEvent(eventValue);
                        m_pTimer_EffectCast.SetCallback(null, OnDone_Timer_EffectCast);
                        m_pTimer_EffectCast.OnPlay();

                        if (m_pDataStack.m_pPlayerCharacter_SkillTrigger != null)
                        {
                            Vector3 vPos_Target = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetSlot().GetPosition();
                            vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;
                            ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectCast, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                        }
                    }
                    else
                    {
                        OnPlayerCharacterSkillTrigger();
                    }
                }
            }

            m_pDataStack.m_nPlayerSkillCount++;
            EventDelegateManager.Instance.OnInGameUI_ChallengeModeCheck();
        }
    }

    private void OnDone_Timer_EffectCast(TransformerEvent eventValue)
    {
        OnPlayerCharacterSkillTrigger();
    }

    private void OnPlayerCharacterSkillTrigger()
    {
        if (m_pCoroutine_SkillTrigger != null)
        {
            AppInstance.Instance.StopCoroutine(m_pCoroutine_SkillTrigger);
            m_pCoroutine_SkillTrigger = null;
        }

        m_pCoroutine_SkillTrigger = AppInstance.Instance.StartCoroutine(Co_OnPlayerCharacterSkillTrigger());
    }

    public IEnumerator Co_OnPlayerCharacterSkillTrigger()
    {
        if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == m_eOwner)
        {
            OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnPlayerCharacterSkillTrigger");

            if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.PlayerCharacter && m_pDataStack.m_pPlayerCharacter_SkillTrigger != null)
            {
                InGameInfo.Instance.m_IsPlayerCharacterSkillToSpeicalBlockAttack = false;
                ExcelData_UnitInfo pUnitInfo = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetUnitInfo();
                Slot pSlot = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetSlot();

                if (pUnitInfo != null)
                {
                    ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(pUnitInfo.m_nActiveSkill_ActionTableID);

                    if (pActionInfo != null)
                    {
                        m_pDataStack.m_nPlayerCharacterProjectileCount = 0;
                        m_pDataStack.m_fKnockBackDelayTime = 0;

                        m_pDataStack.m_pPlayerCharacter_SkillTrigger.ChangeSP(0);

                        switch (pActionInfo.m_eDamageType)
                        {
                            case eDamageEffectType.CharacterSwap:
                                {
                                    Helper.OnSoundPlay(pActionInfo.m_strEffectHit_Sound, false);

                                    float fDelayTime = 0;
                                    foreach (KeyValuePair<DamageTargetInfo, Slot> item in m_pDamageSlotTable)
                                    {
                                        if ((pActionInfo.m_strEffectHit_Target != "0" || pActionInfo.m_strEffectHit_Center != "0") && m_SelectTargetSlotList.Contains(item.Value) == true)
                                        {
                                            Vector3 vPos_Target = item.Value.GetPosition();
                                            vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                                            if (pActionInfo.m_strEffectHit_Target != "0" &&
                                                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Contains(pActionInfo.m_strEffectHit_Target) == false)
                                            {
                                                ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit_Target, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                                                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Add(pActionInfo.m_strEffectHit_Target);
                                            }

                                            if (pActionInfo.m_strEffectHit_Center != "0" &&
                                                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Contains(pActionInfo.m_strEffectHit_Center) == false)
                                            {
                                                ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit_Center, InGameInfo.Instance.m_vSlotGridCenter).SetScale(InGameInfo.Instance.m_fInGameScale);
                                                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Add(pActionInfo.m_strEffectHit_Center);
                                            }

                                            float fDelay = pActionInfo.m_fEffectHit_Target_Delay;

                                            if (fDelay > fDelayTime)
                                            {
                                                fDelayTime = fDelay;
                                            }
                                        }
                                    }

                                    yield return new WaitForSeconds(fDelayTime);

                                    foreach (KeyValuePair<DamageTargetInfo, Slot> item in m_pDamageSlotTable)
                                    {
                                        if (pActionInfo.m_strEffectHit != "0")
                                        {
                                            Vector3 vPos_Target = item.Value.GetPosition();
                                            vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                                            ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                                        }

                                        MainGame_Espresso_ProcessHelper.ApplyStatusEffect(pSlot, item.Value, pActionInfo, item.Key.m_pRangeInfo);
                                    }

                                    switch (EspressoInfo.Instance.m_eGameMode)
                                    {
                                        case eGameMode.PvpStage:
                                            {
                                                if (m_eOwner == eOwner.My)
                                                {
                                                    Slot pSlot_01 = m_pInGame_Highlight_FirstSelect.GetSlot();
                                                    Slot pSlot_02 = m_pInGame_Highlight_SecondSelect.GetSlot();
                                                    SlotFixObject_PlayerCharacter pPlayerCharacter_01 = pSlot_01.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;
                                                    SlotFixObject_PlayerCharacter pPlayerCharacter_02 = pSlot_02.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;
                                                    m_pDataStack.m_PlayerCharacterTable.Remove(pSlot_01.GetSlotIndex());
                                                    m_pDataStack.m_PlayerCharacterTable.Remove(pSlot_02.GetSlotIndex());

                                                    pSlot_01.PopSlotFixObject(pPlayerCharacter_01);
                                                    pSlot_02.PopSlotFixObject(pPlayerCharacter_02);
                                                    pSlot_01.AddSlotFixObject(pPlayerCharacter_02);
                                                    pSlot_02.AddSlotFixObject(pPlayerCharacter_01);
                                                    pPlayerCharacter_01.SetSlot(pSlot_02);
                                                    pPlayerCharacter_02.SetSlot(pSlot_01);
                                                    m_pDataStack.m_PlayerCharacterTable.Add(pSlot_01.GetSlotIndex(), pPlayerCharacter_02);
                                                    m_pDataStack.m_PlayerCharacterTable.Add(pSlot_02.GetSlotIndex(), pPlayerCharacter_01);
                                                }
                                                else
                                                {
                                                    Slot pSlot_01 = m_pInGame_Highlight_FirstSelect.GetSlot();
                                                    Slot pSlot_02 = m_pInGame_Highlight_SecondSelect.GetSlot();
                                                    SlotFixObject_PlayerCharacter pPlayerCharacter_01 = pSlot_01.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;
                                                    SlotFixObject_PlayerCharacter pPlayerCharacter_02 = pSlot_02.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;
                                                    m_pDataStack.m_OtherPlayerCharacterTable.Remove(pSlot_01.GetSlotIndex());
                                                    m_pDataStack.m_OtherPlayerCharacterTable.Remove(pSlot_02.GetSlotIndex());

                                                    pSlot_01.PopSlotFixObject(pPlayerCharacter_01);
                                                    pSlot_02.PopSlotFixObject(pPlayerCharacter_02);
                                                    pSlot_01.AddSlotFixObject(pPlayerCharacter_02);
                                                    pSlot_02.AddSlotFixObject(pPlayerCharacter_01);
                                                    pPlayerCharacter_01.SetSlot(pSlot_02);
                                                    pPlayerCharacter_02.SetSlot(pSlot_01);
                                                    m_pDataStack.m_OtherPlayerCharacterTable.Add(pSlot_01.GetSlotIndex(), pPlayerCharacter_02);
                                                    m_pDataStack.m_OtherPlayerCharacterTable.Add(pSlot_02.GetSlotIndex(), pPlayerCharacter_01);
                                                }
                                            }
                                            break;

                                        case eGameMode.EventPvpStage:
                                            {
                                                if (m_eOwner == eOwner.My)
                                                {
                                                    Slot pSlot_01 = m_pInGame_Highlight_FirstSelect.GetSlot();
                                                    Slot pSlot_02 = m_pInGame_Highlight_SecondSelect.GetSlot();
                                                    SlotFixObject_PlayerCharacter pPlayerCharacter_01 = pSlot_01.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;
                                                    SlotFixObject_PlayerCharacter pPlayerCharacter_02 = pSlot_02.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;
                                                    m_pDataStack.m_PlayerCharacterTable.Remove(pSlot_01.GetSlotIndex());
                                                    m_pDataStack.m_PlayerCharacterTable.Remove(pSlot_02.GetSlotIndex());

                                                    pSlot_01.PopSlotFixObject(pPlayerCharacter_01);
                                                    pSlot_02.PopSlotFixObject(pPlayerCharacter_02);
                                                    pSlot_01.AddSlotFixObject(pPlayerCharacter_02);
                                                    pSlot_02.AddSlotFixObject(pPlayerCharacter_01);
                                                    pPlayerCharacter_01.SetSlot(pSlot_02);
                                                    pPlayerCharacter_02.SetSlot(pSlot_01);
                                                    m_pDataStack.m_PlayerCharacterTable.Add(pSlot_01.GetSlotIndex(), pPlayerCharacter_02);
                                                    m_pDataStack.m_PlayerCharacterTable.Add(pSlot_02.GetSlotIndex(), pPlayerCharacter_01);
                                                }
                                                else
                                                {
                                                    Slot pSlot_01 = m_pInGame_Highlight_FirstSelect.GetSlot();
                                                    Slot pSlot_02 = m_pInGame_Highlight_SecondSelect.GetSlot();
                                                    SlotFixObject_PlayerCharacter pPlayerCharacter_01 = pSlot_01.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;
                                                    SlotFixObject_PlayerCharacter pPlayerCharacter_02 = pSlot_02.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;
                                                    m_pDataStack.m_OtherPlayerCharacterTable.Remove(pSlot_01.GetSlotIndex());
                                                    m_pDataStack.m_OtherPlayerCharacterTable.Remove(pSlot_02.GetSlotIndex());

                                                    pSlot_01.PopSlotFixObject(pPlayerCharacter_01);
                                                    pSlot_02.PopSlotFixObject(pPlayerCharacter_02);
                                                    pSlot_01.AddSlotFixObject(pPlayerCharacter_02);
                                                    pSlot_02.AddSlotFixObject(pPlayerCharacter_01);
                                                    pPlayerCharacter_01.SetSlot(pSlot_02);
                                                    pPlayerCharacter_02.SetSlot(pSlot_01);
                                                    m_pDataStack.m_OtherPlayerCharacterTable.Add(pSlot_01.GetSlotIndex(), pPlayerCharacter_02);
                                                    m_pDataStack.m_OtherPlayerCharacterTable.Add(pSlot_02.GetSlotIndex(), pPlayerCharacter_01);
                                                }
                                            }
                                            break;

                                        default:
                                            {
                                                Slot pSlot_01 = m_pInGame_Highlight_FirstSelect.GetSlot();
                                                Slot pSlot_02 = m_pInGame_Highlight_SecondSelect.GetSlot();
                                                SlotFixObject_PlayerCharacter pPlayerCharacter_01 = pSlot_01.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;
                                                SlotFixObject_PlayerCharacter pPlayerCharacter_02 = pSlot_02.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;
                                                m_pDataStack.m_PlayerCharacterTable.Remove(pSlot_01.GetSlotIndex());
                                                m_pDataStack.m_PlayerCharacterTable.Remove(pSlot_02.GetSlotIndex());

                                                pSlot_01.PopSlotFixObject(pPlayerCharacter_01);
                                                pSlot_02.PopSlotFixObject(pPlayerCharacter_02);
                                                pSlot_01.AddSlotFixObject(pPlayerCharacter_02);
                                                pSlot_02.AddSlotFixObject(pPlayerCharacter_01);
                                                pPlayerCharacter_01.SetSlot(pSlot_02);
                                                pPlayerCharacter_02.SetSlot(pSlot_01);
                                                m_pDataStack.m_PlayerCharacterTable.Add(pSlot_01.GetSlotIndex(), pPlayerCharacter_02);
                                                m_pDataStack.m_PlayerCharacterTable.Add(pSlot_02.GetSlotIndex(), pPlayerCharacter_01);
                                            }
                                            break;
                                    }

                                    EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
                                    EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone += OnInGame_CheckRemoveSlotDone;

                                    m_IsPlayerCharacterAttackTrigger = false;

                                    EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate = false;

                                    EventDelegateManager.Instance.OnInGame_Projectile_Block_Done();
                                    EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0.4f);
                                    InGameInfo.Instance.m_IsInGameClick = false;
                                    m_pDataStack.m_pPlayerCharacter_SkillTrigger = null;
                                    if (m_pGameObject_BlackLayer != null)
                                        m_pGameObject_BlackLayer.SetActive(false);
                                }
                                break;

                            default:
                                {
                                    if (pActionInfo.m_eDamageType == eDamageEffectType.ShieldPointCharge)
                                    {
                                        CharacterInvenItemInfo pCharacterInfo = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetCharacterInvenItemInfo();
                                        EventDelegateManager.Instance.OnInGame_ShieldSelecTarget(false);
                                        m_pDataStack.m_nCurrShildPoint += pActionInfo.m_nDamageBaseRate;
                                        EventDelegateManager.Instance.OnInGame_Action_ShieldCharge(pActionInfo, m_pDataStack.m_nCurrShildPoint, pCharacterInfo.m_nAction_Level);
                                    }

                                    bool IsProjectile = false;
                                    float fDelayTime = 0;

                                    if (pActionInfo.m_strEffectMissile == "0")
                                    {
                                        Helper.OnSoundPlay(pActionInfo.m_strEffectHit_Sound, false);

                                        string strEffectHit_Target = pActionInfo.m_strEffectHit_Target;
                                        if (m_pDataStack.m_pPlayerCharacter_SkillTrigger != null)
                                        {
                                            CharacterInvenItemInfo pCharacterInfo = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetCharacterInvenItemInfo();

                                            ExcelData_Action_LevelUpInfo pActionLevelUpInfo;
                                            pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(pActionInfo.m_nID, pCharacterInfo.m_nAction_Level);

                                            if (pActionLevelUpInfo != null && pActionLevelUpInfo.m_strChange_EffectHit_Target_Prefab != "0")
                                            {
                                                strEffectHit_Target = pActionLevelUpInfo.m_strChange_EffectHit_Target_Prefab;
                                            }
                                        }

                                        if ((strEffectHit_Target != "0" || pActionInfo.m_strEffectHit_Center != "0") && m_SelectTargetSlotList.Count != 0)
                                        {
                                            Slot pSelectTargetSlot = m_SelectTargetSlotList[0];

                                            Vector3 vPos_Target = pSelectTargetSlot.GetPosition();
                                            vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                                            if (strEffectHit_Target != "0" &&
                                                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Contains(strEffectHit_Target) == false)
                                            {
                                                ParticleManager.Instance.LoadParticleSystem(strEffectHit_Target, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                                                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Add(strEffectHit_Target);
                                            }

                                            if (pActionInfo.m_strEffectHit_Center != "0" &&
                                                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Contains(pActionInfo.m_strEffectHit_Center) == false)
                                            {
                                                ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit_Center, InGameInfo.Instance.m_vSlotGridCenter).SetScale(InGameInfo.Instance.m_fInGameScale);
                                                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Add(pActionInfo.m_strEffectHit_Center);
                                            }

                                            float fDelay = pActionInfo.m_fEffectHit_Target_Delay;

                                            if (fDelay > fDelayTime)
                                            {
                                                fDelayTime = fDelay;
                                            }
                                        }
                                    }

                                    foreach (KeyValuePair<DamageTargetInfo, Slot> item in m_pDamageSlotTable)
                                    {
                                        pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(item.Key.m_nActionID);

                                        float fDamageRate = ((float)item.Key.m_pRangeInfo.m_nRelativeDamageRate) / 100.0f;
                                        if (pActionInfo.m_strEffectMissile == "0")
                                        {
                                            Helper.OnSoundPlay(pActionInfo.m_strEffectHit_Sound, false);
                                        }
                                        else
                                        {
                                            OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnPlayerCharacterSkillTrigger Missile Attack");

                                            IsProjectile = true;

                                            if (pActionInfo.m_eDamageType == eDamageEffectType.BlockShuffle)
                                            {
                                                EventDelegateManager.Instance.OnEventInGame_BlockShuffleSkill -= OnInGame_BlockShuffleSkill;
                                                EventDelegateManager.Instance.OnEventInGame_BlockShuffleSkill += OnInGame_BlockShuffleSkill;
                                            }

                                            if (item.Key.m_IsSubSkillStatusEffect == false)
                                            {
                                                GameEvent_PlayerCharacterAttackMissile pGameEvent = new GameEvent_PlayerCharacterAttackMissile(pSlot, item.Value, pActionInfo, fDamageRate, m_SelectTargetSlotList.Contains(item.Value), item.Key);
                                                GameEventManager.Instance.AddGameEvent(pGameEvent);
                                            }
                                            else
                                            {
                                                GameEvent_PlayerCharacterSubSkillMissile pGameEvent = new GameEvent_PlayerCharacterSubSkillMissile(pSlot, item.Value, pActionInfo, fDamageRate, m_SelectTargetSlotList.Contains(item.Value), item.Key);
                                                GameEventManager.Instance.AddGameEvent(pGameEvent);
                                            }
                                        }
                                    }

                                    yield return new WaitForSeconds(fDelayTime);

                                    foreach (KeyValuePair<DamageTargetInfo, Slot> item in m_pDamageSlotTable)
                                    {
                                        pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(item.Key.m_nActionID);

                                        float fDamageRate = ((float)item.Key.m_pRangeInfo.m_nRelativeDamageRate) / 100.0f;
                                        if (pActionInfo.m_strEffectMissile == "0")
                                        {
                                            CharacterInvenItemInfo pCharacterInfo = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetCharacterInvenItemInfo();
                                            ExcelData_Action_LevelUpInfo pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(pActionInfo.m_nID, pCharacterInfo.m_nAction_Level);

                                            if (pActionInfo.m_strEffectHit != "0")
                                            {
                                                Vector3 vPos_Target = item.Value.GetPosition();
                                                vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                                                ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                                            }

                                            if (pActionInfo.m_eDamageType == eDamageEffectType.BlockShuffle)
                                            {
                                                EventDelegateManager.Instance.OnEventInGame_BlockShuffleSkill -= OnInGame_BlockShuffleSkill;
                                                EventDelegateManager.Instance.OnEventInGame_BlockShuffleSkill += OnInGame_BlockShuffleSkill;
                                            }

                                            if (item.Key.m_IsSubSkillStatusEffect == false)
                                            {
                                                MainGame_Espresso_ProcessHelper.OnSkillActionDamage(pSlot, item.Value, pActionInfo, pActionInfo.m_eDamageType, pActionInfo.m_eDamageBaseUnitRole, pActionInfo.m_eDamageBaseUnitProperty,
                                                                                                    pActionLevelUpInfo.m_nChange_DamageBaseRate, fDamageRate, item.Key.m_pConditionInfo.m_fRelativeDamageRate);
                                            }

                                            MainGame_Espresso_ProcessHelper.ApplyStatusEffect(pSlot, item.Value, pActionInfo, item.Key.m_pRangeInfo);
                                        }
                                    }

                                    if (IsProjectile == false)
                                    {
                                        OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnPlayerCharacterSkillTrigger Immediate Attack To SlotMoveAndCreate");

                                        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
                                        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone += OnInGame_CheckRemoveSlotDone;

                                        m_IsPlayerCharacterAttackTrigger = false;

                                        EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate = true;

                                        if (InGameInfo.Instance.m_IsPlayerCharacterSkillToSpeicalBlockAttack == false && pActionInfo.m_eDamageType != eDamageEffectType.BlockShuffle)
                                        {
                                            EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0.4f + m_pDataStack.m_fKnockBackDelayTime);
                                            m_pDataStack.m_fKnockBackDelayTime = 0;
                                        }
                                    }

                                    InGameInfo.Instance.m_IsInGameClick = false;
                                    m_pDataStack.m_pPlayerCharacter_SkillTrigger = null;
                                    if (m_pGameObject_BlackLayer != null)
                                        m_pGameObject_BlackLayer.SetActive(false);
                                }
                                break;
                        }
                    }
                }
            }
        }
    }

    public void OnInGame_PlayerCharacterProjectile_Done(ExcelData_ActionInfo pActionInfo)
    {
        if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner != m_eOwner)
            return;

        OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnInGame_PlayerCharacterProjectile_Done");

        if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.PlayerCharacter)
        {
            EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
            EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone += OnInGame_CheckRemoveSlotDone;

            m_IsPlayerCharacterAttackTrigger = false;

            EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate = true;

            if (InGameInfo.Instance.m_IsPlayerCharacterSkillToSpeicalBlockAttack == false && pActionInfo.m_eDamageType != eDamageEffectType.BlockShuffle)
            {
                EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0.4f + m_pDataStack.m_fKnockBackDelayTime);
                m_pDataStack.m_fKnockBackDelayTime = 0;
            }
        }
    }

    public void OnInGame_Projectile_Block_Done()
    {
        if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner != m_eOwner)
            return;

        OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnInGame_Projectile_Block_Done");

        if (m_IsActive == true)
        {
        }
    }

    public void OnInGame_Projectile_SP_Charge_Done()
    {
        if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner != m_eOwner)
            return;

        OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnInGame_Projectile_SP_Charge_Done");

        if (m_IsActive == true)
        {
        }
    }

    public void OnInGame_BlockShuffleSkill(SlotFixObject_Unit pUnit, ExcelData_ActionInfo pActionInfo)
    {
        if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner != m_eOwner)
            return;

        if (pUnit != null && pUnit.GetObjectType() == eObjectType.Character)
        {
            if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.PlayerCharacter)
            {
                EspressoInfo.Instance.m_eActionTriggerSubject = eActionTriggerSubject.None;

                EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;

                EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate = false;
                InGameInfo.Instance.m_IsInGameClick = true;

                if (m_IsActive == true)
                {
                    OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnInGame_CheckRemoveSlotDone active");

                    switch (EspressoInfo.Instance.m_eGameMode)
                    {
                        case eGameMode.PvpStage:
                            {
                                if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
                                {
                                    if (m_pDataStack.IsPlayerCharacterAllDead(eOwner.Other) == true)
                                    {
                                        EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                                    }
                                    else
                                    {
                                        EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger = false;
                                        InGameInfo.Instance.m_IsInGameClick = false;
                                        m_pTurnComponent_BlockMatch.OnPlayerCharacterAttackDone();

                                        if (m_IsAutoPlay == true)
                                            OnAutoPlay();
                                    }
                                }
                                else
                                {
                                    if (m_pDataStack.IsPlayerCharacterAllDead(eOwner.My) == true)
                                    {
                                        EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Fail);
                                    }
                                    else
                                    {
                                        EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger = false;
                                        InGameInfo.Instance.m_IsInGameClick = false;
                                        m_pTurnComponent_BlockMatch.OnPlayerCharacterAttackDone();

                                        if (m_IsAutoPlay == true)
                                            OnAutoPlay();
                                    }
                                }
                            }
                            break;

                        case eGameMode.EventPvpStage:
                            {
                                if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
                                {
                                    if (m_pDataStack.IsPlayerCharacterAllDead(eOwner.Other) == true)
                                    {
                                        EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                                    }
                                    else
                                    {
                                        EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger = false;
                                        InGameInfo.Instance.m_IsInGameClick = false;
                                        m_pTurnComponent_BlockMatch.OnPlayerCharacterAttackDone();

                                        if (m_IsAutoPlay == true)
                                            OnAutoPlay();
                                    }
                                }
                                else
                                {
                                    if (m_pDataStack.IsPlayerCharacterAllDead(eOwner.My) == true)
                                    {
                                        EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Fail);
                                    }
                                    else
                                    {
                                        EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger = false;
                                        InGameInfo.Instance.m_IsInGameClick = false;
                                        m_pTurnComponent_BlockMatch.OnPlayerCharacterAttackDone();

                                        if (m_IsAutoPlay == true)
                                            OnAutoPlay();
                                    }
                                }
                            }
                            break;

                        default:
                            {
                                if (m_pDataStack.m_nCurrShildPoint > 0 && m_pDataStack.m_EnemyMinionTable.Count == 0 && m_pDataStack.m_nCurrObjectiveCount <= 0)
                                {
                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                                }
                                else
                                {
                                    EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger = false;
                                    InGameInfo.Instance.m_IsInGameClick = false;
                                    m_pTurnComponent_BlockMatch.OnPlayerCharacterAttackDone();

                                    if (m_IsAutoPlay == true)
                                        OnAutoPlay();
                                }
                            }
                            break;
                    }
                }
            }
        }
    }

    public void OnInGame_CheckRemoveSlotDone()
    {
        if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner != m_eOwner)
            return;

        OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnInGame_CheckRemoveSlotDone");

        if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.PlayerCharacter)
        {
            EspressoInfo.Instance.m_eActionTriggerSubject = eActionTriggerSubject.None;

            EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;

            InGameInfo.Instance.m_IsInGameClick = true;
            m_IsShowSkillCutScene = false;

            InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();

            if (m_IsActive == true)
            {
                OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnInGame_CheckRemoveSlotDone active");

                switch (EspressoInfo.Instance.m_eGameMode)
                {
                    case eGameMode.PvpStage:
                        {
                            if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
                            {
                                if (m_pDataStack.IsPlayerCharacterAllDead(eOwner.Other) == true)
                                {
                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                                }
                                else
                                {
                                    EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger = false;
                                    InGameInfo.Instance.m_IsInGameClick = false;
                                    m_pTurnComponent_BlockMatch.OnPlayerCharacterAttackDone();

                                    if (m_IsAutoPlay == true)
                                        OnAutoPlay();
                                }
                            }
                            else
                            {
                                if (m_pDataStack.IsPlayerCharacterAllDead(eOwner.My) == true)
                                {
                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Fail);
                                }
                                else
                                {
                                    EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger = false;
                                    InGameInfo.Instance.m_IsInGameClick = false;
                                    m_pTurnComponent_BlockMatch.OnPlayerCharacterAttackDone();

                                    if (m_IsAutoPlay == true)
                                        OnAutoPlay();
                                }
                            }
                        }
                        break;

                    case eGameMode.EventPvpStage:
                        {
                            if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
                            {
                                if (m_pDataStack.IsPlayerCharacterAllDead(eOwner.Other) == true)
                                {
                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                                }
                                else
                                {
                                    EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger = false;
                                    InGameInfo.Instance.m_IsInGameClick = false;
                                    m_pTurnComponent_BlockMatch.OnPlayerCharacterAttackDone();

                                    if (m_IsAutoPlay == true)
                                        OnAutoPlay();
                                }
                            }
                            else
                            {
                                if (m_pDataStack.IsPlayerCharacterAllDead(eOwner.My) == true)
                                {
                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Fail);
                                }
                                else
                                {
                                    EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger = false;
                                    InGameInfo.Instance.m_IsInGameClick = false;
                                    m_pTurnComponent_BlockMatch.OnPlayerCharacterAttackDone();

                                    if (m_IsAutoPlay == true)
                                        OnAutoPlay();
                                }
                            }
                        }
                        break;

                    default:
                        {
                            if (m_pDataStack.m_nCurrShildPoint > 0 && m_pDataStack.m_EnemyMinionTable.Count == 0 && m_pDataStack.m_nCurrObjectiveCount <= 0)
                            {
                                EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                            }
                            else
                            {
                                EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger = false;
                                InGameInfo.Instance.m_IsInGameClick = false;
                                m_pTurnComponent_BlockMatch.OnPlayerCharacterAttackDone();

                                if (m_IsAutoPlay == true)
                                    OnAutoPlay();
                            }
                        }
                        break;
                }
            }
        }
    }

    protected void OnCallback_LButtonDown_CancelArea(GameObject gameObject, Vector3 vPos, object ob, int nFingerID)
    {
        if (m_IsAutoPlay == false)
        {
            m_nCancelAreaFingerID = nFingerID;
        }
    }

    protected void OnCallback_LButtonUp_CancelArea(GameObject gameObject, Vector3 vPos, object ob, int nFingerID, bool IsDown)
    {
        if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner != m_eOwner)
            return;

        if (IsDown == true && m_nCancelAreaFingerID == nFingerID && EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger == true && 
            EspressoInfo.Instance.m_IsInGame_SkillTriggerDrag == false && m_IsAutoPlay == false &&
            EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.PlayerCharacter &&
            EspressoInfo.Instance.m_IsActionAndUnitDetailCloseBlock == false)
        {
            EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Cancel();
            EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Close();
            EventDelegateManager.Instance.OnInGame_UnitDetail_Close();

            if (m_pGameObject_CancelArea != null)
            {
                GameObject.Destroy(m_pGameObject_CancelArea);
                m_pGameObject_CancelArea = null;
            }

            if (m_pDataStack.m_IsCurrRefillGuideState == true && m_eOwner == eOwner.My)
            {
                GameObject ob_RefillGuide = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGameRefillGuide");
                GameObject.Instantiate(ob_RefillGuide);
            }
        }

        EspressoInfo.Instance.m_IsActionAndUnitDetailCloseBlock = false;
        InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();

        EventDelegateManager.Instance.OnInGame_ShieldSelecTarget(false);
    }

    public void OnInGame_Tooltip_DestroyCancelArea()
    {
        if (m_pGameObject_CancelArea != null)
        {
            GameObject.Destroy(m_pGameObject_CancelArea);
            m_pGameObject_CancelArea = null;
        }
    }
}
