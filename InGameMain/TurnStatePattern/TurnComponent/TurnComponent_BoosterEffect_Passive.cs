using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent_BoosterEffect_Passive : TComponent<EventArg_Null, EventArg_Null>
{
    private Transformer_Timer   m_pTimer_Delay_NextEvent    = new Transformer_Timer();
    private bool                m_IsActive                  = false;
    private eOwner              m_eOwner                    = eOwner.My;
    public enum ePassiveEffectReturn
    {
        None,
        NoMissile,
        Missile,
    }

    public class PassiveData
    {
        public ePassiveEffectReturn                     m_ePassiveEffectReturn      = ePassiveEffectReturn.None;
        public Slot                                     m_pSrcSlot                  = null;
        public Slot                                     m_pDamageSlot               = null;
        public ExcelData_ActionInfo                     m_pActionInfo               = null;
        public ExcelData_Action_DamageTargetRangeInfo   m_pDamageTargetRangeInfo    = null;
    }

    private List<PassiveData>   m_PassiveDataList           = new List<PassiveData>();

    private Transformer_Timer   m_pTimer_Process            = new Transformer_Timer();
    private bool                m_IsScale_PlayerCharacter   = false;
    private Transformer_Scalar  m_pScale_PlayerCharacter    = new Transformer_Scalar(1);
    private GameObject          m_pGameObject_WhiteLayer    = null;
    private Plane2D             m_pPlane_WhiteLayer         = null;
    private Transformer_Scalar  m_pAlpha_WhiteLayer         = new Transformer_Scalar(0);

    public TurnComponent_BoosterEffect_Passive(eOwner eOwn)
    {
        ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);

        m_eOwner = eOwn;

        m_pTimer_Delay_NextEvent.OnReset();

        TransformerEvent_Timer eventValue;
        eventValue = new TransformerEvent_Timer(0.5f);
        m_pTimer_Delay_NextEvent.AddEvent(eventValue);
        m_pTimer_Delay_NextEvent.SetCallback(null, OnDone_Timer_Delay_NextEvent);

        GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/WhiteLayer");
        m_pGameObject_WhiteLayer = GameObject.Instantiate(ob);
        m_pGameObject_WhiteLayer.SetActive(false);
        m_pGameObject_WhiteLayer.transform.localScale *= AppInstance.Instance.m_fHeightScale;
        m_pPlane_WhiteLayer = m_pGameObject_WhiteLayer.GetComponent<Plane2D>();
    }

    public override void OnDestroy()
    {
        EventDelegateManager.Instance.OnEventInGame_PlayerCharacterPassiveProjectile_Done -= OnInGame_PlayerCharacterPassiveProjectile_Done;
        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
    }

    public override void Update()
    {
        if (m_IsScale_PlayerCharacter == true)
        {
            m_pScale_PlayerCharacter.Update(Time.deltaTime);
            float fScale = m_pScale_PlayerCharacter.GetCurScalar() * InGameInfo.Instance.m_fInGameScale;

            foreach (PassiveData pData in m_PassiveDataList)
            {
                if (pData.m_pSrcSlot != null)
                {
                    SlotFixObject_PlayerCharacter pPlayerCharacter = pData.m_pSrcSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                    if (pPlayerCharacter != null)
                    {
                        pPlayerCharacter.GetGameObject().transform.localScale = new Vector3(fScale, fScale, 1);
                    }
                }
            }
        }

        m_pTimer_Process.Update(Time.deltaTime);
        m_pAlpha_WhiteLayer.Update(Time.deltaTime);
        float fAlpha = m_pAlpha_WhiteLayer.GetCurScalar();
        m_pPlane_WhiteLayer.SetColor(new Color(1,1,1,fAlpha));

        m_pTimer_Delay_NextEvent.Update(Time.deltaTime);

    }

    public override void OnEvent(EventArg_Null Arg)
    {
        OutputLog.Log("TurnComponent_BoosterEffect_Passive : OnEvent");
        InGameTurnLog.Log("TurnComponent_BoosterEffect_Passive : OnEvent");

        m_IsActive = true;
        m_PassiveDataList.Clear();

        InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.PlayerCharacterPassiveSkill;
        EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.PlayerCharacterPassiveSkill);

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        ePassiveEffectReturn eReturnResult = ePassiveEffectReturn.None;
        InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();

        Dictionary<int, SlotFixObject_PlayerCharacter> characterTable = null;

        if (m_eOwner == eOwner.My)
        {
            characterTable = pDataStack.m_PlayerCharacterTable;
        }
        else
        {
            characterTable = pDataStack.m_OtherPlayerCharacterTable;
        }

        foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in characterTable)
        {
            if (item.Value.GetUnitInfo().m_eUnitRank >= eUnitRank.SR && item.Value.GetCharacterInvenItemInfo().m_nLevel >= 6 && item.Value.IsDead() == false &&
                item.Value.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.AttackBlock) == false)
            {
                int nActionID = item.Value.GetUnitInfo().m_nPassiveEffect_ActionTableID;

                if (nActionID != 0)
                {
                    ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(nActionID);

                    if (pActionInfo != null)
                    {
                        ePassiveEffectReturn eReturn = OnPassiveEffect(item.Value, nActionID);

                        if ((eReturn == ePassiveEffectReturn.Missile) ||
                            (eReturn == ePassiveEffectReturn.NoMissile && eReturnResult != ePassiveEffectReturn.Missile))
                        {
                            eReturnResult = eReturn;
                        }
                    }
                }
            }

            // Sub Passive
            if (item.Value.GetUnitInfo().m_eSubSkillCondition == eSubSkillCondition.OverTurn &&
                item.Value.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.AttackBlock) == false)
            {
                ePassiveEffectReturn eReturn = OnSubPassiveEffect(item.Value, item.Value.GetUnitInfo().m_nSubSkill_ActionTableID);

                if ((eReturn == ePassiveEffectReturn.Missile) ||
                    (eReturn == ePassiveEffectReturn.NoMissile && eReturnResult != ePassiveEffectReturn.Missile))
                {
                    eReturnResult = eReturn;
                }
            }
        }

        foreach (KeyValuePair<int, SlotFixObject_Minion> item in pDataStack.m_PlayerSummonUnitTable)
        {
            if (item.Value.GetOwner() == m_eOwner &&
                item.Value.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.AttackBlock) == false)
            {
                if (item.Value.GetUnitInfo().m_eSubSkillCondition == eSubSkillCondition.OverTurn)
                {
                    ePassiveEffectReturn eReturn = OnSubPassiveEffect(item.Value, item.Value.GetUnitInfo().m_nSubSkill_ActionTableID);

                    if ((eReturn == ePassiveEffectReturn.Missile) ||
                        (eReturn == ePassiveEffectReturn.NoMissile && eReturnResult != ePassiveEffectReturn.Missile))
                    {
                        eReturnResult = eReturn;
                    }
                }
            }
        }

        if (eReturnResult != ePassiveEffectReturn.None && m_PassiveDataList.Count != 0)
        {
			m_IsScale_PlayerCharacter = true;

			m_pScale_PlayerCharacter.OnReset();
			TransformerEvent_Scalar eventValue;
			eventValue = new TransformerEvent_Scalar(0.0f, 1.3f);
			m_pScale_PlayerCharacter.AddEvent(eventValue);
            m_pScale_PlayerCharacter.SetCallback(null, OnDone_Timer_Process_BigSize_Portrait);
            m_pScale_PlayerCharacter.OnPlay();
		}
        else
        {
            EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

            m_IsActive = false;
            OutputLog.Log("TurnComponent_BoosterEffect_Passive : GetNextEvent().OnEvent(EventArg_Null.Object)");
            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
    }

    private void OnDone_Timer_Process_BigSize_Portrait(TransformerEvent eventValue)
    {
        m_pGameObject_WhiteLayer.SetActive(true);

        m_pAlpha_WhiteLayer.OnReset();
        eventValue = new TransformerEvent_Scalar(0, 0);
        m_pAlpha_WhiteLayer.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(0.05f, 0.5f);
        m_pAlpha_WhiteLayer.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(0.1f, 0);
        m_pAlpha_WhiteLayer.AddEvent(eventValue);
        m_pAlpha_WhiteLayer.SetCallback(null, OnDone_Alpha_WhiteLayer);
        m_pAlpha_WhiteLayer.OnPlay();
    }

    private void OnDone_Alpha_WhiteLayer(TransformerEvent eventValue)
    {
        m_pGameObject_WhiteLayer.SetActive(false);

        m_pScale_PlayerCharacter.OnReset();
        eventValue = new TransformerEvent_Scalar(0.0f, 1.3f);
        m_pScale_PlayerCharacter.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(0.2f, 1.0f);
        m_pScale_PlayerCharacter.AddEvent(eventValue);
        m_pScale_PlayerCharacter.SetCallback(null, OnDone_Timer_Process_OriginSizePortrait);
        m_pScale_PlayerCharacter.OnPlay();
    }

    private void OnDone_Timer_Process_OriginSizePortrait(TransformerEvent eventValue)
    {
        ExcelData_SoundInfo pSoundInfo = ExcelDataManager.Instance.m_pSound.GetSoundInfo_byID("INGAME_PASSIVE_ACTIVATED");
        if (pSoundInfo != null)
        {
            SoundPlayer.Instance.Play(pSoundInfo);
        }

        m_IsScale_PlayerCharacter = false;
        m_pGameObject_WhiteLayer.SetActive(true);

        m_pAlpha_WhiteLayer.OnReset();
        eventValue = new TransformerEvent_Scalar(0, 0.5f);
        m_pAlpha_WhiteLayer.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(0.8f, 0.5f);
        m_pAlpha_WhiteLayer.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(1.0f, 0);
        m_pAlpha_WhiteLayer.AddEvent(eventValue);
        m_pAlpha_WhiteLayer.OnPlay();

        foreach (PassiveData pData in m_PassiveDataList)
        {
            if (pData.m_pSrcSlot != null)
            {
                SlotFixObject_Unit pUnit = pData.m_pSrcSlot.GetLastSlotFixObject() as SlotFixObject_Unit;

                switch (pUnit.GetObjectType())
                {
                    case eObjectType.Character:
                        {
                            SlotFixObject_PlayerCharacter pPlayerCharacter = pData.m_pSrcSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                            if (pPlayerCharacter != null)
                            {
                                pPlayerCharacter.ShowPassiveText(true);

                                pPlayerCharacter.SetHighlight();
                                Vector3 vPos = pData.m_pSrcSlot.GetPosition();
                                vPos.z = -(float)ePlaneOrder.Fx_TopLayer;
                                ParticleManager.Instance.LoadParticleSystem("FX_Character_PassiveActivated", vPos).SetScale(InGameInfo.Instance.m_fInGameScale);
                            }
                        }
                        break;

                    case eObjectType.Minion:
                        {
                            SlotFixObject_Minion pMinion = pData.m_pSrcSlot.GetLastSlotFixObject() as SlotFixObject_Minion;

                            if (pMinion != null)
                            {
                                pMinion.ShowPassiveText(true);

                                pMinion.SetHighlight();
                                Vector3 vPos = pData.m_pSrcSlot.GetPosition();
                                vPos.z = -(float)ePlaneOrder.Fx_TopLayer;
                                ParticleManager.Instance.LoadParticleSystem("FX_Character_PassiveActivated", vPos).SetScale(InGameInfo.Instance.m_fInGameScale);
                            }
                        }
                        break;
                }
            }
        }

        m_pTimer_Process.OnReset();
        eventValue = new TransformerEvent_Timer(1.0f);
        m_pTimer_Process.AddEvent(eventValue);
        m_pTimer_Process.SetCallback(null, OnDone_Timer_Process_CastEffect);
        m_pTimer_Process.OnPlay();
    }

    private void OnDone_Timer_Process_CastEffect(TransformerEvent eventValue)
    {
        m_pGameObject_WhiteLayer.SetActive(false);

        ePassiveEffectReturn eReturnResult = ePassiveEffectReturn.None;

        foreach (PassiveData pData in m_PassiveDataList)
        {
            if (pData.m_pSrcSlot != null)
            {
                SlotFixObject_Unit pUnit = pData.m_pSrcSlot.GetLastSlotFixObject() as SlotFixObject_Unit;

                switch (pUnit.GetObjectType())
                {
                    case eObjectType.Character:
                        {
                            SlotFixObject_PlayerCharacter pPlayerCharacter = pData.m_pSrcSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                            if (pPlayerCharacter != null)
                            {
                                pPlayerCharacter.ShowPassiveText(false);
                                pPlayerCharacter.ClearHighlight();
                                // 효과 발동

                                switch (pData.m_ePassiveEffectReturn)
                                {
                                    case ePassiveEffectReturn.Missile:
                                        {
                                            GameEvent_PlayerPassiveMissile pGameEvent = new GameEvent_PlayerPassiveMissile(pData.m_pSrcSlot, pData.m_pDamageSlot, pData.m_pActionInfo, pData.m_pDamageTargetRangeInfo);
                                            GameEventManager.Instance.AddGameEvent(pGameEvent);
                                        }
                                        break;

                                    case ePassiveEffectReturn.NoMissile:
                                        {
                                            MainGame_Espresso_ProcessHelper.ApplyStatusEffect(pData.m_pSrcSlot, pData.m_pDamageSlot, pData.m_pActionInfo, pData.m_pDamageTargetRangeInfo);
                                        }
                                        break;
                                }

                                if ((pData.m_ePassiveEffectReturn == ePassiveEffectReturn.Missile) ||
                                    (pData.m_ePassiveEffectReturn == ePassiveEffectReturn.NoMissile && eReturnResult != ePassiveEffectReturn.Missile))
                                {
                                    eReturnResult = pData.m_ePassiveEffectReturn;
                                }
                            }
                        }
                        break;

                    case eObjectType.Minion:
                        {
                            SlotFixObject_Minion pMinion = pData.m_pSrcSlot.GetLastSlotFixObject() as SlotFixObject_Minion;

                            if (pMinion != null)
                            {
                                pMinion.ShowPassiveText(false);
                                pMinion.ClearHighlight();
                                // 효과 발동

                                switch (pData.m_ePassiveEffectReturn)
                                {
                                    case ePassiveEffectReturn.Missile:
                                        {
                                            GameEvent_PlayerPassiveMissile pGameEvent = new GameEvent_PlayerPassiveMissile(pData.m_pSrcSlot, pData.m_pDamageSlot, pData.m_pActionInfo, pData.m_pDamageTargetRangeInfo);
                                            GameEventManager.Instance.AddGameEvent(pGameEvent);
                                        }
                                        break;

                                    case ePassiveEffectReturn.NoMissile:
                                        {
                                            MainGame_Espresso_ProcessHelper.ApplyStatusEffect(pData.m_pSrcSlot, pData.m_pDamageSlot, pData.m_pActionInfo, pData.m_pDamageTargetRangeInfo);
                                        }
                                        break;
                                }

                                if ((pData.m_ePassiveEffectReturn == ePassiveEffectReturn.Missile) ||
                                    (pData.m_ePassiveEffectReturn == ePassiveEffectReturn.NoMissile && eReturnResult != ePassiveEffectReturn.Missile))
                                {
                                    eReturnResult = pData.m_ePassiveEffectReturn;
                                }
                            }
                        }
                        break;
                }

                
            }
        }

        switch (eReturnResult)
        {
            case ePassiveEffectReturn.Missile:
                {
                    EventDelegateManager.Instance.OnEventInGame_PlayerCharacterPassiveProjectile_Done += OnInGame_PlayerCharacterPassiveProjectile_Done;
                }
                break;

            case ePassiveEffectReturn.NoMissile:
                {
                    m_pTimer_Delay_NextEvent.OnPlay();
                }
                break;
        }
    }

    private ePassiveEffectReturn OnPassiveEffect(SlotFixObject_PlayerCharacter pPlayerCharacter, int nActionID)
    {
        if (pPlayerCharacter.GetUnitInfo().m_ePassiveActionCondition == ePassiveActionCondition.None)
        {
            return ePassiveEffectReturn.None;
        }

        int nPassive_Level = pPlayerCharacter.GetCharacterInvenItemInfo().m_nPassive_Level;
        ExcelData_Action_LevelUpInfo pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(nActionID, nPassive_Level);

        if (pPlayerCharacter.GetUnitInfo().m_ePassiveActionCondition == ePassiveActionCondition.Once && pPlayerCharacter.IsApplyPassiveAction_ForOnce() == false)
        {
            pPlayerCharacter.SetApplyPassiveAction_ForOnce(true);

            float fRandom = UnityEngine.Random.Range(0.0f, 100.0f);

            if (nPassive_Level <= 5)
                return ePassiveEffectReturn.None;

            if (fRandom > (float)pActionLevelUpInfo.m_nChange_PassiveActionProbability)
            {
                return ePassiveEffectReturn.None;
            }
        }
        else if (pPlayerCharacter.GetUnitInfo().m_ePassiveActionCondition == ePassiveActionCondition.Once && pPlayerCharacter.IsApplyPassiveAction_ForOnce() == true)
        {
            return ePassiveEffectReturn.None;
        }

        pPlayerCharacter.SetApplyPassiveAction_ForOnce(true);

        if (pPlayerCharacter.GetUnitInfo().m_ePassiveActionCondition == ePassiveActionCondition.OverTurn)
        {
            float fRandom = UnityEngine.Random.Range(0.0f, 100.0f);

            if (nPassive_Level <= 5)
                return ePassiveEffectReturn.None;

            if (fRandom > (float)pActionLevelUpInfo.m_nChange_PassiveActionProbability)
            {
                return ePassiveEffectReturn.None;
            }
        }

        return OnUnitPassiveEffect(pPlayerCharacter, nActionID, pActionLevelUpInfo);
    }

    private ePassiveEffectReturn OnSubPassiveEffect(SlotFixObject_Unit pUnit, int nActionID)
    {
        int nRandomValue = UnityEngine.Random.Range(0, 100);
        if (pUnit.GetUnitInfo().m_SubSkillProbabilityList[nRandomValue] == false)
        {
            return ePassiveEffectReturn.None;
        }

        return OnUnitPassiveEffect(pUnit, nActionID, null);
    }

    private ePassiveEffectReturn OnUnitPassiveEffect(SlotFixObject_Unit pUnit, int nActionID, ExcelData_Action_LevelUpInfo pActionLevelUpInfo)
    {
        ePassiveEffectReturn eReturn = ePassiveEffectReturn.None;

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(nActionID);

        SelectTargetGenerator pSelectTargetGenerator = new SelectTargetGenerator();
        pSelectTargetGenerator.OnGenerator(nActionID, pUnit.GetSlot(), pUnit.GetOwner(), pUnit.GetObjectType(), eAttackType.Skill, pActionLevelUpInfo == null ? 1 : pActionLevelUpInfo.m_nChange_SelectTargetRangeLevel);

        int nSelectTargetAmount = pActionLevelUpInfo == null ? pActionInfo.m_nSelectTargetAmount : pActionLevelUpInfo.m_nChange_SelectTargetAmount;

        for (int i = 0; i < nSelectTargetAmount; ++i)
        {
            SelectTargetInfo pSelectTargetInfo = pSelectTargetGenerator.GetSelectTargetInfo_byIndex(i);

            if (pSelectTargetInfo != null)
            {
                int nSelect_X = pSelectTargetInfo.m_nSelectTargetSlotX;
                int nSelect_Y = pSelectTargetInfo.m_nSelectTargetSlotY;

                if (pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nSelect_X, nSelect_Y)) == true)
                {
                    Slot pSelectSlot = pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nSelect_X, nSelect_Y)];

                    DamageTargetGenerator pDamageTargetGenerator = new DamageTargetGenerator();
                    pDamageTargetGenerator.OnGenerator(pActionInfo.m_nID, pSelectSlot, pUnit.GetOwner(), pUnit.GetObjectType(), eAttackType.Skill, pActionLevelUpInfo);

                    int nNumDamageTarget = pDamageTargetGenerator.GetNumDamageTargetInfo();

                    for (int j = 0; j < nNumDamageTarget; ++j)
                    {
                        DamageTargetInfo pDamageTargetInfo = pDamageTargetGenerator.GetDamageTargetInfo_byIndex(j);

                        if (pDamageTargetInfo != null)
                        {
                            int nDamage_X = nSelect_X + pDamageTargetInfo.m_pRangeInfo.m_nPosX;
                            int nDamage_Y = nSelect_Y + pDamageTargetInfo.m_pRangeInfo.m_nPosY;

                            if (pUnit.GetOwner() == eOwner.My || pDamageTargetInfo.m_pRangeInfo.m_eRangeType == eRangeType.All)
                            {
                                nDamage_Y = nSelect_Y + pDamageTargetInfo.m_pRangeInfo.m_nPosY * -1;
                            }

                            if (pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nDamage_X, nDamage_Y)) == true)
                            {
                                Slot pDamageSlot = pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nDamage_X, nDamage_Y)];

                                if (MainGame_Espresso_ProcessHelper.IsApplyStatusEffect(pUnit.GetSlot(), pDamageSlot, pDamageTargetInfo.m_pRangeInfo) == true)
                                {
                                    if (pActionInfo.m_strEffectMissile != "0")
                                    {
                                        PassiveData pPassiveData = new PassiveData();
                                        pPassiveData.m_ePassiveEffectReturn = ePassiveEffectReturn.Missile;
                                        pPassiveData.m_pSrcSlot = pUnit.GetSlot();
                                        pPassiveData.m_pDamageSlot = pDamageSlot;
                                        pPassiveData.m_pActionInfo = pActionInfo;
                                        pPassiveData.m_pDamageTargetRangeInfo = pDamageTargetInfo.m_pRangeInfo;
                                        m_PassiveDataList.Add(pPassiveData);

                                        eReturn = ePassiveEffectReturn.Missile;
                                    }
                                    else
                                    {
                                        PassiveData pPassiveData = new PassiveData();
                                        pPassiveData.m_ePassiveEffectReturn = ePassiveEffectReturn.NoMissile;
                                        pPassiveData.m_pSrcSlot = pUnit.GetSlot();
                                        pPassiveData.m_pDamageSlot = pDamageSlot;
                                        pPassiveData.m_pActionInfo = pActionInfo;
                                        pPassiveData.m_pDamageTargetRangeInfo = pDamageTargetInfo.m_pRangeInfo;
                                        m_PassiveDataList.Add(pPassiveData);

                                        eReturn = ePassiveEffectReturn.NoMissile;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return eReturn;
    }

    public void OnInGame_PlayerCharacterPassiveProjectile_Done()
    {
        EventDelegateManager.Instance.OnEventInGame_PlayerCharacterPassiveProjectile_Done -= OnInGame_PlayerCharacterPassiveProjectile_Done;

        m_pTimer_Delay_NextEvent.OnPlay();
    }

    private void OnDone_Timer_Delay_NextEvent(TransformerEvent eventValue)
    {
        OutputLog.Log("TurnComponent_BoosterEffect_Passive : GetNextEvent().OnEvent(EventArg_Null.Object)");

        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone += OnInGame_CheckRemoveSlotDone;

        EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0.2f);
    }

    public void OnInGame_CheckRemoveSlotDone()
    {
        OutputLog.Log("TurnComponent_BoosterEffect_Passive : OnInGame_CheckRemoveSlotDone");

        EventDelegateManager.Instance.OnInGame_Unit_CheckBuffDebuffMark();
        EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;

        if (m_IsActive == true)
        {
            m_IsActive = false;
            InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();
            OutputLog.Log("TurnComponent_BoosterEffect_Passive : OnInGame_CheckRemoveSlotDone active");

            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
    }
}
