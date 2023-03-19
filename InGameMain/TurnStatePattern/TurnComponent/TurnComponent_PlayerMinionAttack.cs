using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent_PlayerMinionAttack : TComponent<EventArg_Null, EventArg_Null>
{
    private MainGame_DataStack      m_pDataStack                    = null;
    private Transformer_Timer       m_pTimer_EffectCast             = new Transformer_Timer();
    private Transformer_Timer       m_pTimer_AttackCast             = new Transformer_Timer();
    private Transformer_Timer       m_pTimer_AttackCompleteDelay    = new Transformer_Timer();

    private bool                    m_IsActive                      = false;
    private eOwner                  m_eOwner                        = eOwner.My;

    private Dictionary<SlotFixObject_Minion, bool> m_pAttackMinionTable = new Dictionary<SlotFixObject_Minion, bool>();    // Value : False > Attack, True > Skill

    public TurnComponent_PlayerMinionAttack(eOwner eOwn)
    {
        m_eOwner = eOwn;

        m_pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        EventDelegateManager.Instance.OnEventInGame_EnemyMinionProjectile_Done += OnInGame_EnemyMinionProjectile_Done;
        EventDelegateManager.Instance.OnEventInGame_ReflectDamageProjectile_Done += OnInGame_ReflectDamageProjectile_Done;
    }

    public override void OnDestroy()
    {
        EventDelegateManager.Instance.OnEventInGame_EnemyMinionProjectile_Done -= OnInGame_EnemyMinionProjectile_Done;
        EventDelegateManager.Instance.OnEventInGame_ReflectDamageProjectile_Done -= OnInGame_ReflectDamageProjectile_Done;

        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
    }

    public override void Update()
    {
        m_pTimer_EffectCast.Update(Time.deltaTime);
        m_pTimer_AttackCast.Update(Time.deltaTime);
        m_pTimer_AttackCompleteDelay.Update(Time.deltaTime);
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        OutputLog.Log("TurnComponent_PlayerMinionAttack : OnEvent");
        InGameTurnLog.Log("TurnComponent_PlayerMinionAttack : OnEvent");

        InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.PlayerMinionAttack;
        EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.PlayerMinionAttack);

        m_IsActive = true;
        m_pDataStack.m_EnemyColonyCreateTable.Clear();
        m_pAttackMinionTable.Clear();
        InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();

        foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pDataStack.m_PlayerSummonUnitTable)
        {
            SlotFixObject_Minion pMinion = item.Value;

            if (pMinion != null && pMinion.GetOwner() == m_eOwner)
            {
                if (pMinion.IsFull_SP() == true)
                {
                    if (MainGame_Espresso_ProcessHelper.IsMinionPossibleAction(pMinion.GetUnitInfo().m_nActiveSkill_ActionTableID, pMinion.GetSlot(), pMinion.GetOwner(), eAttackType.Skill) == true)
                    {
                        if (pMinion.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.AttackBlock) == false)
                        {
                            m_pAttackMinionTable.Add(pMinion, true);
                        }

                        if (pMinion.GetDelayTurn() > 1)
                        {
                            pMinion.DecreaseTurn();
                        }
                    }
                    else if (pMinion.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.AttackBlock) == false)
                    {
                        if (pMinion.DecreaseTurn() == true)
                        {
                            if (MainGame_Espresso_ProcessHelper.IsMinionPossibleAction(pMinion.GetUnitInfo().m_nAttack_ActionTableID, pMinion.GetSlot(), pMinion.GetOwner(), eAttackType.Skill) == true)
                            {
                                m_pAttackMinionTable.Add(pMinion, false);
                            }
                        }
                    }
                }
                else
                {
                    int nChangeSP = pMinion.GetChargePerTurn_SP() + pMinion.GetSP();
                    pMinion.ChangeSP(nChangeSP);

                    if (pMinion.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.AttackBlock) == false)
                    {
                        if (pMinion.DecreaseTurn() == true)
                        {
                            if (MainGame_Espresso_ProcessHelper.IsMinionPossibleAction(pMinion.GetUnitInfo().m_nAttack_ActionTableID, pMinion.GetSlot(), pMinion.GetOwner(), eAttackType.Attack) == true)
                            {
                                m_pAttackMinionTable.Add(pMinion, false);
                            }
                        }
                    }
                }
            }
        }

        if (m_pAttackMinionTable.Count == 0)
        {
            EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);
            m_IsActive = false;
            OutputLog.Log("TurnComponent_PlayerMinionAttack : GetNextEvent().OnEvent(EventArg_Null.Object)");
            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
        else
        {
            OutputLog.Log("TurnComponent_PlayerMinionAttack : m_pAttackMinionTable.Count != 0");

            m_pTimer_EffectCast.OnReset();

            TransformerEvent_Timer eventValue;
            eventValue = new TransformerEvent_Timer(0.5f);
            m_pTimer_EffectCast.AddEvent(eventValue);
            m_pTimer_EffectCast.SetCallback(null, OnDone_Timer_EffectCast);
            m_pTimer_EffectCast.OnPlay();

            foreach (KeyValuePair<SlotFixObject_Minion, bool> item in m_pAttackMinionTable)
            {
                ExcelData_ActionInfo pActionInfo = null;

                if (item.Value == true)     // Skill
                {
                    int nAttackActionID = item.Key.GetUnitInfo().m_nActiveSkill_ActionTableID;
                    pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(nAttackActionID);
                }
                else                        // Attack
                {
                    int nAttackActionID = item.Key.GetUnitInfo().m_nAttack_ActionTableID;
                    pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(nAttackActionID);
                }

                if (pActionInfo != null && pActionInfo.m_strEffectCast != "0")
                {
                    Vector3 vPos_Target = item.Key.GetSlot().GetPosition();
                    vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                    ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectCast, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                }
            }
        }
    }

    private void OnDone_Timer_EffectCast(TransformerEvent eventValue)
    {
        OutputLog.Log("TurnComponent_PlayerMinionAttack : OnDone_Timer_EffectCast");

        foreach (KeyValuePair<SlotFixObject_Minion, bool> item in m_pAttackMinionTable)
        {
            if (m_pDataStack.m_PlayerSummonUnitTable.ContainsValue(item.Key) == true)
            {
                item.Key.OnAttackCast();
            }
        }

        m_pTimer_AttackCast.OnReset();
        eventValue = new TransformerEvent_Timer(0.05f);
        m_pTimer_AttackCast.AddEvent(eventValue);
        m_pTimer_AttackCast.SetCallback(null, OnDone_Timer_AttackCast);
        m_pTimer_AttackCast.OnPlay();
    }

    private void OnDone_Timer_AttackCast(TransformerEvent eventValue)
    {
        OutputLog.Log("TurnComponent_PlayerMinionAttack : OnDone_Timer_AttackCast");

        foreach (KeyValuePair<SlotFixObject_Minion, bool> item in m_pAttackMinionTable)
        {
            if (m_pDataStack.m_PlayerSummonUnitTable.ContainsValue(item.Key) == true)
            {
                if (item.Value == true)
                {
                    item.Key.ChangeSP(0);
                    MainGame_Espresso_ProcessHelper.OnMinionSkillActionDamage(item.Key);
                }
                else
                {
                    MainGame_Espresso_ProcessHelper.OnMinionAttackActionDamage(item.Key);
                }
            }
        }

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        if (pDataStack.m_nEnemyMinionProjectileCount == 0 && pDataStack.m_nReflectDamageProjectileCount == 0)
        {
            m_pTimer_AttackCompleteDelay.OnReset();
            eventValue = new TransformerEvent_Timer(0.01f);
            m_pTimer_AttackCompleteDelay.AddEvent(eventValue);
            m_pTimer_AttackCompleteDelay.SetCallback(null, OnDone_Timer_AttackCompleteDelay);
            m_pTimer_AttackCompleteDelay.OnPlay();
        }
    }

    public void OnInGame_EnemyMinionProjectile_Done()
    {
        OutputLog.Log("TurnComponent_PlayerMinionAttack : OnInGame_EnemyMinionProjectile_Done");

        if (m_IsActive == true)
        {
            MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
            if (pDataStack.m_nEnemyMinionProjectileCount == 0 && pDataStack.m_nReflectDamageProjectileCount == 0)
            {
                m_pTimer_AttackCompleteDelay.OnReset();

                TransformerEvent_Timer eventValue;
                eventValue = new TransformerEvent_Timer(0.01f);
                m_pTimer_AttackCompleteDelay.AddEvent(eventValue);
                m_pTimer_AttackCompleteDelay.SetCallback(null, OnDone_Timer_AttackCompleteDelay);
                m_pTimer_AttackCompleteDelay.OnPlay();
            }
        }
    }

    public void OnInGame_ReflectDamageProjectile_Done()
    {
        if (m_IsActive == true)
        {
            MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
            if (pDataStack.m_nEnemyMinionProjectileCount == 0 && pDataStack.m_nReflectDamageProjectileCount == 0)
            {
                m_pTimer_AttackCompleteDelay.OnReset();

                TransformerEvent_Timer eventValue;
                eventValue = new TransformerEvent_Timer(0.01f);
                m_pTimer_AttackCompleteDelay.AddEvent(eventValue);
                m_pTimer_AttackCompleteDelay.SetCallback(null, OnDone_Timer_AttackCompleteDelay);
                m_pTimer_AttackCompleteDelay.OnPlay();
            }
        }
    }

    private void OnDone_Timer_AttackCompleteDelay(TransformerEvent eventValue)
    {
        EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

        OutputLog.Log("TurnComponent_PlayerMinionAttack : GetNextEvent().OnEvent(EventArg_Null.Object)");

        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone += OnInGame_CheckRemoveSlotDone;

        EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0.4f);
    }

    public void OnInGame_CheckRemoveSlotDone()
    {
        OutputLog.Log("TurnComponent_PlayerMinionAttack : OnInGame_CheckRemoveSlotDone");

        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;

        if (m_IsActive == true)
        {
            m_IsActive = false;
            OutputLog.Log("TurnComponent_PlayerMinionAttack : OnInGame_CheckRemoveSlotDone active");

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
                                OutputLog.Log("TurnComponent_PlayerMinionAttack : OnInGame_CheckRemoveSlotDone eGameMode.PvpStage 01");
                                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();

                                GetNextEvent().OnEvent(EventArg_Null.Object);
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
                                OutputLog.Log("TurnComponent_PlayerMinionAttack : OnInGame_CheckRemoveSlotDone eGameMode.PvpStage 02");
                                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();

                                GetNextEvent().OnEvent(EventArg_Null.Object);
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
                                OutputLog.Log("TurnComponent_PlayerMinionAttack : OnInGame_CheckRemoveSlotDone eGameMode.PvpStage 01");
                                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();

                                GetNextEvent().OnEvent(EventArg_Null.Object);
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
                                OutputLog.Log("TurnComponent_PlayerMinionAttack : OnInGame_CheckRemoveSlotDone eGameMode.PvpStage 02");
                                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();

                                GetNextEvent().OnEvent(EventArg_Null.Object);
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
                            OutputLog.Log("TurnComponent_PlayerMinionAttack : OnInGame_CheckRemoveSlotDone default");
                            InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();

                            GetNextEvent().OnEvent(EventArg_Null.Object);
                        }
                    }
                    break;
            }
        }
    }
}
