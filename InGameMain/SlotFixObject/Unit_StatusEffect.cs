using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyStatusEffect
{
    public ExcelData_StatusEffectInfo   m_pStatusEffectInfo     = null;
    public ExcelData_ActionInfo         m_pActionInfo           = null;
    public int                          m_nApplyValue           = 0;
    public int                          m_nRenameTurn           = 0;

    public int                          m_nChangeValue          = 0;

    public eUnitDamage                  m_eUnitDamage           = eUnitDamage.Damage;
    public eOwner                       m_eOwner_Subject        = eOwner.My;

    public ParticleInfo                 m_pParticleInfo         = null;
}

public class Unit_StatusEffect
{
    private SlotFixObject_Unit      m_pSlotFixObject_Unit       = null;
    private List<ApplyStatusEffect> m_ApplyStatusEffectList     = new List<ApplyStatusEffect>();

    public Unit_StatusEffect(SlotFixObject_Unit pUnit)
    {
        m_pSlotFixObject_Unit = pUnit;

        EventDelegateManager.Instance.OnEventUpdate += OnUpdate;
    }

    public void OnDestroy()
    {
        foreach (ApplyStatusEffect pApplyStatusEffect in m_ApplyStatusEffectList)
        {
            if (pApplyStatusEffect.m_pParticleInfo != null)
            {
                ParticleManager.Instance.RemoveParticleInfo(pApplyStatusEffect.m_pParticleInfo);
            }
        }

        m_ApplyStatusEffectList.Clear();

        EventDelegateManager.Instance.OnEventUpdate -= OnUpdate;
    }

    public void OnUpdate()
    {
        foreach (ApplyStatusEffect pApplyStatusEffect in m_ApplyStatusEffectList)
        {
            if (pApplyStatusEffect.m_pParticleInfo != null)
            {
                Vector3 vPos = m_pSlotFixObject_Unit.GetGameObject().transform.position;
                vPos.z = -(float)ePlaneOrder.Fx_TopLayer;

                pApplyStatusEffect.m_pParticleInfo.SetPosition(vPos);
            }
        }
    }

    private bool ActiveStatusEffect(ApplyStatusEffect pApplyStatusEffect, SlotFixObject_Unit pUnit_Actor)
    {
        bool IsRes = false;
        switch (pApplyStatusEffect.m_pStatusEffectInfo.m_eEffectType)
        {
            case eDamageEffectType.ElementDamage:
                {
                    int nHP = m_pSlotFixObject_Unit.GetHP();
                    int nDamage = pApplyStatusEffect.m_nApplyValue;

                    if (m_pSlotFixObject_Unit.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageChange) == true)
                    {
                        nDamage = MainGame_Espresso_ProcessHelper.CalculationDamageChange(m_pSlotFixObject_Unit, nDamage);
                    }

                    if (pUnit_Actor != null && m_pSlotFixObject_Unit.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageReflect) == true)
                    {
                        MainGame_Espresso_ProcessHelper.OnDamageReflect(pUnit_Actor, m_pSlotFixObject_Unit, pApplyStatusEffect.m_pActionInfo, nDamage);
                    }

                    m_pSlotFixObject_Unit.ChangeHP(nHP - nDamage, GameDefine.ms_UnitDamageColors[(int)pApplyStatusEffect.m_eUnitDamage]);
                    m_pSlotFixObject_Unit.ResetVirtualHP();

                    if (pApplyStatusEffect.m_eUnitDamage != eUnitDamage.Damage)
                    {
                        m_pSlotFixObject_Unit.OnDamageText(pApplyStatusEffect.m_eUnitDamage);
                    }

                    if (m_pSlotFixObject_Unit.GetHP() <= 0)
                    {
                        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

                        switch (m_pSlotFixObject_Unit.GetObjectType())
                        {
                            case eObjectType.Minion:
                            case eObjectType.MinionBoss:
                                {
                                    SlotFixObject_Minion pMinion = m_pSlotFixObject_Unit as SlotFixObject_Minion;

                                    if (pMinion != null)
                                    {
                                        switch (pMinion.GetMinionType())
                                        {
                                            case eMinionType.EnemyMinion:
                                                {
                                                    pDataStack.m_EnemyMinionTable.Remove(pMinion.GetSlot().GetSlotIndex());
                                                }
                                                break;

                                            case eMinionType.PlayerSummonUnit:
                                                {
                                                    pDataStack.m_PlayerSummonUnitTable.Remove(pMinion.GetSlot().GetSlotIndex());
                                                }
                                                break;

                                            case eMinionType.EnemySummonUnit:
                                                {
                                                    pDataStack.m_EnemySummonUnitTable.Remove(pMinion.GetSlot().GetSlotIndex());
                                                }
                                                break;
                                        }
                                    }

                                    IsRes = true;
                                }
                                break;

                            case eObjectType.Character:
                                {
                                    if (pDataStack.m_PlayerCharacterTable.ContainsKey(m_pSlotFixObject_Unit.GetSlot().GetSlotIndex()) == true)
                                    {
                                        //pDataStack.m_PlayerCharacterTable.Remove(m_pSlotFixObject_Unit.GetSlot().GetSlotIndex());
                                    }
                                }
                                break;
                        }

                        m_pSlotFixObject_Unit.OnDead();
                    }
                }
                break;

            case eDamageEffectType.Damage:
                {
                    int nHP = m_pSlotFixObject_Unit.GetHP();

                    int nDamage = pApplyStatusEffect.m_nApplyValue;

                    if (m_pSlotFixObject_Unit.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageChange) == true)
                    {
                        nDamage = MainGame_Espresso_ProcessHelper.CalculationDamageChange(m_pSlotFixObject_Unit, nDamage);
                    }

                    if (pUnit_Actor != null && m_pSlotFixObject_Unit.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageReflect) == true)
                    {
                        MainGame_Espresso_ProcessHelper.OnDamageReflect(pUnit_Actor, m_pSlotFixObject_Unit, pApplyStatusEffect.m_pActionInfo, nDamage);
                    }

                    m_pSlotFixObject_Unit.ChangeHP(nHP - nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage]);
                    m_pSlotFixObject_Unit.ResetVirtualHP();

                    if (m_pSlotFixObject_Unit.GetHP() <= 0)
                    {
                        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

                        switch (m_pSlotFixObject_Unit.GetObjectType())
                        {
                            case eObjectType.Minion:
                            case eObjectType.MinionBoss:
                                {
                                    SlotFixObject_Minion pMinion = m_pSlotFixObject_Unit as SlotFixObject_Minion;

                                    if (pMinion != null)
                                    {
                                        switch (pMinion.GetMinionType())
                                        {
                                            case eMinionType.EnemyMinion:
                                                {
                                                    pDataStack.m_EnemyMinionTable.Remove(pMinion.GetSlot().GetSlotIndex());
                                                }
                                                break;

                                            case eMinionType.PlayerSummonUnit:
                                                {
                                                    pDataStack.m_PlayerSummonUnitTable.Remove(pMinion.GetSlot().GetSlotIndex());
                                                }
                                                break;

                                            case eMinionType.EnemySummonUnit:
                                                {
                                                    pDataStack.m_EnemySummonUnitTable.Remove(pMinion.GetSlot().GetSlotIndex());
                                                }
                                                break;
                                        }
                                    }

                                    IsRes = true;
                                }
                                break;

                            case eObjectType.Character:
                                {
                                    if (pDataStack.m_PlayerCharacterTable.ContainsKey(m_pSlotFixObject_Unit.GetSlot().GetSlotIndex()) == true)
                                    {
                                        //pDataStack.m_PlayerCharacterTable.Remove(m_pSlotFixObject_Unit.GetSlot().GetSlotIndex());
                                    }
                                }
                                break;
                        }

                        m_pSlotFixObject_Unit.OnDead();
                    }
                }
                break;

            case eDamageEffectType.Heal:
                {
                    int nHP = m_pSlotFixObject_Unit.GetHP();
                    m_pSlotFixObject_Unit.ChangeHP(nHP + pApplyStatusEffect.m_nApplyValue, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.HP_Recovery]);
                    m_pSlotFixObject_Unit.ResetVirtualHP();

                    m_pSlotFixObject_Unit.OnDamageText(eUnitDamage.HP_Recovery);
                }
                break;

            case eDamageEffectType.SPCharge:
                {
                    int nSP = m_pSlotFixObject_Unit.GetSP();
                    m_pSlotFixObject_Unit.ChangeSP(nSP + pApplyStatusEffect.m_nApplyValue, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.SP_Recharge]);
                    m_pSlotFixObject_Unit.OnDamageText(eUnitDamage.SP_Recharge);
                }
                break;

            case eDamageEffectType.ATKChange:
                {
                    m_pSlotFixObject_Unit.PlusAdd_ATK(pApplyStatusEffect.m_nApplyValue);
                    pApplyStatusEffect.m_nChangeValue += pApplyStatusEffect.m_nApplyValue;

                    if (m_pSlotFixObject_Unit.GetObjectType() == eObjectType.Character)
                    {
                        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
                        pDataStack.m_nPlayerCharacterAttackValue[(int)m_pSlotFixObject_Unit.GetUnitInfo().m_eElement] += pApplyStatusEffect.m_nApplyValue;
                    }
                }
                break;

            case eDamageEffectType.MaxHPChange:
                {
                    m_pSlotFixObject_Unit.PlusAdd_MaxHP(pApplyStatusEffect.m_nApplyValue);   
                    pApplyStatusEffect.m_nChangeValue += pApplyStatusEffect.m_nApplyValue;
                }
                break;

            case eDamageEffectType.SPChargeChange:
                {
                    m_pSlotFixObject_Unit.PlusAdd_ChargePerBlock_SP(pApplyStatusEffect.m_nApplyValue);
                    m_pSlotFixObject_Unit.PlusAdd_ChargePerTurn_SP(pApplyStatusEffect.m_nApplyValue);
                    pApplyStatusEffect.m_nChangeValue += pApplyStatusEffect.m_nApplyValue;
                }
                break;

            case eDamageEffectType.ElementResist:
                {
                    m_pSlotFixObject_Unit.PlusAdd_ElementResist(pApplyStatusEffect.m_pStatusEffectInfo.m_eElement, pApplyStatusEffect.m_nApplyValue);
                    pApplyStatusEffect.m_nChangeValue += pApplyStatusEffect.m_nApplyValue;

                    switch (pApplyStatusEffect.m_pStatusEffectInfo.m_eElement)
                    {
                        case eElement.Water: m_pSlotFixObject_Unit.OnDamageText(ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_WaterElementResistUp")); break;
                        case eElement.Fire: m_pSlotFixObject_Unit.OnDamageText(ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_FireElementResistUp")); break;
                        case eElement.Wind: m_pSlotFixObject_Unit.OnDamageText(ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_WindElementResistUp")); break;
                        case eElement.Light: m_pSlotFixObject_Unit.OnDamageText(ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_LightElementResistUp")); break;
                        case eElement.Dark: m_pSlotFixObject_Unit.OnDamageText(ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_DarkElementResistUp")); break;
                    }
                }
                break;

            case eDamageEffectType.DamageReflect:
                {
                    // 구현 완료
                }
                break;

            case eDamageEffectType.MoveHold:
                {
                    // 구현 완료
                }
                break;

            case eDamageEffectType.AttackBlock:
                {
                    // 구현 완료
                }
                break;

            case eDamageEffectType.DamageChange:
                {
                    // 구현 완료
                }
                break;

            case eDamageEffectType.BuffDispel:      // 버프 해제
                {
                    // 구현 완료
                }
                break;

            case eDamageEffectType.DebuffDispel:    // 디버프 해제
                {
                    // 구현 완료
                }
                break;

            case eDamageEffectType.BuffImmune:      // 버프 면역
                {
                    // 구현 완료
                }
                break;

            case eDamageEffectType.DeuffImmune:     // 디버프 면역
                {
                    // 구현 완료
                }
                break;

            case eDamageEffectType.KnockBack:
                {
                    SlotFixObject_Minion pMinion = m_pSlotFixObject_Unit as SlotFixObject_Minion;

                    if (pMinion != null)
                    {
                        eKnockBackStatusEffect_Direct eDirect = eKnockBackStatusEffect_Direct.Up;

                        switch (pApplyStatusEffect.m_eOwner_Subject)
                        {
                            case eOwner.My:
                                {
                                    if (pApplyStatusEffect.m_nApplyValue > 0)
                                    {
                                        eDirect = eKnockBackStatusEffect_Direct.Up;
                                    }
                                    else
                                    {
                                        eDirect = eKnockBackStatusEffect_Direct.Down;
                                    }
                                }
                                break;

                            case eOwner.Other:
                                {
                                    if (pApplyStatusEffect.m_nApplyValue > 0)
                                    {
                                        eDirect = eKnockBackStatusEffect_Direct.Down;
                                    }
                                    else
                                    {
                                        eDirect = eKnockBackStatusEffect_Direct.Up;
                                    }
                                }
                                break;
                        }

                        if (pApplyStatusEffect.m_nApplyValue != 0)
                        {
                            GameEvent_EnemyMinion_KnockBack pGameEvent = new GameEvent_EnemyMinion_KnockBack(pMinion, eDirect, pApplyStatusEffect.m_nApplyValue);
                            GameEventManager.Instance.AddGameEvent(pGameEvent);
                        }
                    }

                    int nHP = m_pSlotFixObject_Unit.GetHP();
                    m_pSlotFixObject_Unit.ChangeHP(nHP - pApplyStatusEffect.m_nApplyValue, GameDefine.ms_UnitDamageColors[(int)pApplyStatusEffect.m_eUnitDamage]);
                    m_pSlotFixObject_Unit.ResetVirtualHP();

                    if (pApplyStatusEffect.m_eUnitDamage != eUnitDamage.Damage)
                    {
                        m_pSlotFixObject_Unit.OnDamageText(pApplyStatusEffect.m_eUnitDamage);
                    }

                    if (m_pSlotFixObject_Unit.GetHP() <= 0)
                    {
                        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

                        switch (m_pSlotFixObject_Unit.GetObjectType())
                        {
                            case eObjectType.Minion:
                            case eObjectType.MinionBoss:
                                {
                                    pMinion = m_pSlotFixObject_Unit as SlotFixObject_Minion;

                                    if (pMinion != null)
                                    {
                                        switch (pMinion.GetMinionType())
                                        {
                                            case eMinionType.EnemyMinion:
                                                {
                                                    pDataStack.m_EnemyMinionTable.Remove(pMinion.GetSlot().GetSlotIndex());
                                                }
                                                break;

                                            case eMinionType.PlayerSummonUnit:
                                                {
                                                    pDataStack.m_PlayerSummonUnitTable.Remove(pMinion.GetSlot().GetSlotIndex());
                                                }
                                                break;

                                            case eMinionType.EnemySummonUnit:
                                                {
                                                    pDataStack.m_EnemySummonUnitTable.Remove(pMinion.GetSlot().GetSlotIndex());
                                                }
                                                break;
                                        }
                                    }

                                    IsRes = true;
                                }
                                break;

                            case eObjectType.Character:
                                {
                                    if (pDataStack.m_PlayerCharacterTable.ContainsKey(m_pSlotFixObject_Unit.GetSlot().GetSlotIndex()) == true)
                                    {
                                        //pDataStack.m_PlayerCharacterTable.Remove(m_pSlotFixObject_Unit.GetSlot().GetSlotIndex());
                                    }
                                }
                                break;
                        }

                        m_pSlotFixObject_Unit.OnDead();
                    }
                }
                break;
        }

        Vector3 vPos_Target = m_pSlotFixObject_Unit.GetSlot().GetPosition();
        vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

        ParticleManager.Instance.LoadParticleSystem(pApplyStatusEffect.m_pStatusEffectInfo.m_strEffectHit_Prefab, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
        Helper.OnSoundPlay(pApplyStatusEffect.m_pStatusEffectInfo.m_strEffectHit_Sound, false);

        return IsRes;
    }

    public void AddApplyStatusEffect(ExcelData_StatusEffectInfo pStatusEffectInfo, ExcelData_ActionInfo pActionInfo, int nApplyValue, eOwner eOwner_Subject, SlotFixObject_Unit pUnit_Actor, eUnitDamage eUnitDamage = eUnitDamage.Damage)
    {
        if (IsExistApplyStatusEffect_byEffectType(eDamageEffectType.BuffImmune) == true && pStatusEffectInfo.m_eBuffType == eBuffType.Buff)
        {
            return;
        }

        if (IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DeuffImmune) == true && pStatusEffectInfo.m_eBuffType == eBuffType.Debuff)
        {
            return;
        }

        ApplyStatusEffect pApplyStatusEffect = new ApplyStatusEffect();
        pApplyStatusEffect.m_pStatusEffectInfo = pStatusEffectInfo;
        pApplyStatusEffect.m_pActionInfo = pActionInfo;
        pApplyStatusEffect.m_nApplyValue = nApplyValue;
        pApplyStatusEffect.m_nRenameTurn = pApplyStatusEffect.m_pStatusEffectInfo.m_nLastingTurnCount;
        pApplyStatusEffect.m_eUnitDamage = eUnitDamage;
        pApplyStatusEffect.m_eOwner_Subject = eOwner_Subject;

        if (pStatusEffectInfo.m_strEffectLoop_Prefab != "0")
        {
            Vector3 vPos = m_pSlotFixObject_Unit.GetGameObject().transform.position;
            vPos.z = -(float)ePlaneOrder.Fx_TopLayer;
            pApplyStatusEffect.m_pParticleInfo = ParticleManager.Instance.LoadParticleSystem(pStatusEffectInfo.m_strEffectLoop_Prefab, vPos);
            pApplyStatusEffect.m_pParticleInfo.SetScale(InGameInfo.Instance.m_fInGameScale);
        }

        m_ApplyStatusEffectList.Add(pApplyStatusEffect);

        // 즉시 적용
        if (pApplyStatusEffect.m_pStatusEffectInfo.m_eApplyType == eApplyType.Instant)
        {
            if (pApplyStatusEffect.m_pStatusEffectInfo.m_eEffectType == eDamageEffectType.BuffDispel)           // 모든 버프 해제
            {
                // 자기 자신도 포함
                List<ApplyStatusEffect> removeList = new List<ApplyStatusEffect>();
                foreach (ApplyStatusEffect pApplyStatus in m_ApplyStatusEffectList)
                {
                    if (pApplyStatus.m_pStatusEffectInfo.m_eBuffType == eBuffType.Buff && pApplyStatus.m_pStatusEffectInfo.m_eUndispellable == eUndispellable.No)
                    {
                        removeList.Add(pApplyStatus);
                    }
                }

                foreach (ApplyStatusEffect pRemove in removeList)
                {
                    ReleaseApplyStatusEffect(pRemove);

                    if (pRemove.m_pParticleInfo != null)
                    {
                        ParticleManager.Instance.RemoveParticleInfo(pRemove.m_pParticleInfo);
                    }

                    m_ApplyStatusEffectList.Remove(pRemove);
                }

                Vector3 vPos_Target = m_pSlotFixObject_Unit.GetSlot().GetPosition();
                vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;
                ParticleManager.Instance.LoadParticleSystem(pStatusEffectInfo.m_strEffectHit_Prefab, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                Helper.OnSoundPlay(pStatusEffectInfo.m_strEffectHit_Sound, false);
            }
            else if (pApplyStatusEffect.m_pStatusEffectInfo.m_eEffectType == eDamageEffectType.DebuffDispel)    // 모든 디버프 해제
            {
                // 자기 자신도 포함
                List<ApplyStatusEffect> removeList = new List<ApplyStatusEffect>();
                foreach (ApplyStatusEffect pApplyStatus in m_ApplyStatusEffectList)
                {
                    if (pApplyStatus.m_pStatusEffectInfo.m_eBuffType == eBuffType.Debuff && pApplyStatus.m_pStatusEffectInfo.m_eUndispellable == eUndispellable.No)
                    {
                        removeList.Add(pApplyStatus);
                    }
                }

                foreach (ApplyStatusEffect pRemove in removeList)
                {
                    ReleaseApplyStatusEffect(pRemove);

                    if (pRemove.m_pParticleInfo != null)
                    {
                        ParticleManager.Instance.RemoveParticleInfo(pRemove.m_pParticleInfo);
                    }

                    m_ApplyStatusEffectList.Remove(pRemove);
                }

                Vector3 vPos_Target = m_pSlotFixObject_Unit.GetSlot().GetPosition();
                vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;
                ParticleManager.Instance.LoadParticleSystem(pStatusEffectInfo.m_strEffectHit_Prefab, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                Helper.OnSoundPlay(pStatusEffectInfo.m_strEffectHit_Sound, false);
            }
            else
            {
                ActiveStatusEffect(pApplyStatusEffect, pUnit_Actor);

                MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

                switch (EspressoInfo.Instance.m_eGameMode)
                {
                    case eGameMode.PvpStage:
                        {
                            if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
                            {
                                if (pDataStack.IsPlayerCharacterAllDead(eOwner.Other) == true)
                                {
                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                                }
                            }
                            else
                            {
                                if (pDataStack.IsPlayerCharacterAllDead(eOwner.My) == true)
                                {
                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Fail);
                                }
                            }
                        }
                        break;

                    case eGameMode.EventPvpStage:
                        {
                            if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
                            {
                                if (pDataStack.IsPlayerCharacterAllDead(eOwner.Other) == true)
                                {
                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                                }
                            }
                            else
                            {
                                if (pDataStack.IsPlayerCharacterAllDead(eOwner.My) == true)
                                {
                                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Fail);
                                }
                            }
                        }
                        break;

                    default:
                        {
                            if (pDataStack.m_nCurrShildPoint > 0 && pDataStack.m_EnemyMinionTable.Count == 0 && pDataStack.m_nCurrObjectiveCount <= 0)
                            {
                                EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                            }
                        }
                        break;
                }
            }
        }
    }

    private void ReleaseApplyStatusEffect(ApplyStatusEffect pApplyStatusEffect)
    {
        switch (pApplyStatusEffect.m_pStatusEffectInfo.m_eEffectType)
        {
            case eDamageEffectType.ATKChange:
                {
                    if (pApplyStatusEffect.m_pStatusEffectInfo.m_eLastingType == eLastingType.Temporary)
                    {
                        m_pSlotFixObject_Unit.PlusAdd_ATK(-pApplyStatusEffect.m_nChangeValue);

                        if (m_pSlotFixObject_Unit.GetObjectType() == eObjectType.Character)
                        {
                            MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
                            pDataStack.m_nPlayerCharacterAttackValue[(int)m_pSlotFixObject_Unit.GetUnitInfo().m_eElement] -= pApplyStatusEffect.m_nChangeValue;
                        }
                    }
                }
                break;

            case eDamageEffectType.MaxHPChange:
                {
                    if (pApplyStatusEffect.m_pStatusEffectInfo.m_eLastingType == eLastingType.Temporary)
                    {
                        m_pSlotFixObject_Unit.PlusAdd_MaxHP(-pApplyStatusEffect.m_nChangeValue);
                    }
                }
                break;

            case eDamageEffectType.SPChargeChange:
                {
                    if (pApplyStatusEffect.m_pStatusEffectInfo.m_eLastingType == eLastingType.Temporary)
                    {
                        m_pSlotFixObject_Unit.PlusAdd_ChargePerBlock_SP(-pApplyStatusEffect.m_nChangeValue);
                        m_pSlotFixObject_Unit.PlusAdd_ChargePerTurn_SP(-pApplyStatusEffect.m_nChangeValue);
                    }
                }
                break;

            case eDamageEffectType.ElementResist:
                {
                    if (pApplyStatusEffect.m_pStatusEffectInfo.m_eLastingType == eLastingType.Temporary)
                    {
                        m_pSlotFixObject_Unit.PlusAdd_ElementResist(pApplyStatusEffect.m_pStatusEffectInfo.m_eElement, -pApplyStatusEffect.m_nChangeValue);
                    }
                }
                break;
        }
    }

    public void StopApplyStatusEffect(int nStatusEffectGroup)
    {
        List<ApplyStatusEffect> releaseList = new List<ApplyStatusEffect>();

        foreach (ApplyStatusEffect pApplyStatusEffect in m_ApplyStatusEffectList)
        {
            // 같은 그룹, 해제 가능... eUndispellable.No : 해제 가능
            if (pApplyStatusEffect.m_pStatusEffectInfo.m_nEffectGroupID == nStatusEffectGroup && pApplyStatusEffect.m_pStatusEffectInfo.m_eUndispellable == eUndispellable.No)
            {
                ReleaseApplyStatusEffect(pApplyStatusEffect);
                releaseList.Add(pApplyStatusEffect);
            }
        }

        foreach (ApplyStatusEffect pApplyStatusEffect in releaseList)
        {
            if (pApplyStatusEffect.m_pParticleInfo != null)
            {
                ParticleManager.Instance.RemoveParticleInfo(pApplyStatusEffect.m_pParticleInfo);
            }

            m_ApplyStatusEffectList.Remove(pApplyStatusEffect);
        }
    }

    public  bool IsExistApplyStatusEffect_byEffectType(eDamageEffectType eEffectType)
    {
        foreach (ApplyStatusEffect pApplyStatusEffect in m_ApplyStatusEffectList)
        {
            if (pApplyStatusEffect.m_pStatusEffectInfo.m_eEffectType == eEffectType)
            {
                return true;
            }
        }

        return false;
    }

    public List<ApplyStatusEffect> GetApplyStatusEffect_byEffectType(eDamageEffectType eEffectType)
    {
        List <ApplyStatusEffect> list = new List<ApplyStatusEffect>();
        foreach (ApplyStatusEffect pApplyStatusEffect in m_ApplyStatusEffectList)
        {
            if (pApplyStatusEffect.m_pStatusEffectInfo.m_eEffectType == eEffectType)
            {
                list.Add(pApplyStatusEffect);
            }
        }

        return list;
    }

    public bool IsExistBuff()
    {
        foreach (ApplyStatusEffect pApplyStatusEffect in m_ApplyStatusEffectList)
        {
            if (pApplyStatusEffect.m_pStatusEffectInfo.m_eBuffType == eBuffType.Buff)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsExistDebuff()
    {
        foreach (ApplyStatusEffect pApplyStatusEffect in m_ApplyStatusEffectList)
        {
            if (pApplyStatusEffect.m_pStatusEffectInfo.m_eBuffType == eBuffType.Debuff)
            {
                return true;
            }
        }

        return false;
    }

    public int GetBuffCount()
    {
        int nCount = 0;

        foreach (ApplyStatusEffect pApplyStatusEffect in m_ApplyStatusEffectList)
        {
            if (pApplyStatusEffect.m_pStatusEffectInfo.m_eBuffType == eBuffType.Buff)
            {
                ++nCount;
            }
        }

        return nCount;
    }

    public ApplyStatusEffect GetApplyStatusBuffEffect_byIndex(int nIndex)
    {
        int i = 0;

        foreach (ApplyStatusEffect pApplyStatusEffect in m_ApplyStatusEffectList)
        {
            if (pApplyStatusEffect.m_pStatusEffectInfo.m_eBuffType == eBuffType.Buff)
            {
                if (nIndex == i)
                {
                    return pApplyStatusEffect;
                }

                ++i;
            }
        }

        return null;
    }

    public int GetDebuffCount()
    {
        int nCount = 0;

        foreach (ApplyStatusEffect pApplyStatusEffect in m_ApplyStatusEffectList)
        {
            if (pApplyStatusEffect.m_pStatusEffectInfo.m_eBuffType == eBuffType.Debuff)
            {
                ++nCount;
            }
        }

        return nCount;
    }

    public ApplyStatusEffect GetApplyStatusDebuffEffect_byIndex(int nIndex)
    {
        int i = 0;

        foreach (ApplyStatusEffect pApplyStatusEffect in m_ApplyStatusEffectList)
        {
            if (pApplyStatusEffect.m_pStatusEffectInfo.m_eBuffType == eBuffType.Debuff)
            {
                if (nIndex == i)
                {
                    return pApplyStatusEffect;
                }

                ++i;
            }
        }

        return null;
    }

    public bool VirtualDecreaseTurn(eBuffType eType)
    {
        bool IsRes = false;

        foreach (ApplyStatusEffect pApplyStatusEffect in m_ApplyStatusEffectList)
        {
            if (pApplyStatusEffect.m_pStatusEffectInfo.m_eBuffType == eType)
            {
                if (pApplyStatusEffect.m_nRenameTurn <= 1 || pApplyStatusEffect.m_pStatusEffectInfo.m_eApplyType == eApplyType.OverTurn)
                {
                    IsRes = true;
                }
            }
        }

        return IsRes;
    }

    public bool DecreaseTurn(eBuffType eType)
    {
        bool IsRes = false;

        foreach (ApplyStatusEffect pApplyStatusEffect in m_ApplyStatusEffectList)
        {
            if (pApplyStatusEffect.m_pStatusEffectInfo.m_eBuffType == eType)
            {
                IsRes = true;
            }
        }

        List<ApplyStatusEffect> removeList = new List<ApplyStatusEffect>();

        if (IsExistApplyStatusEffect_byEffectType(eDamageEffectType.BuffDispel) == true)            // 모든 버프 해제
        {
            // 자기 자신도 포함
            removeList.Clear();
            foreach (ApplyStatusEffect pApplyStatus in m_ApplyStatusEffectList)
            {
                if (pApplyStatus.m_pStatusEffectInfo.m_eBuffType == eBuffType.Buff && pApplyStatus.m_pStatusEffectInfo.m_eUndispellable == eUndispellable.No)
                {
                    removeList.Add(pApplyStatus);
                }
            }

            foreach (ApplyStatusEffect pRemove in removeList)
            {
                ReleaseApplyStatusEffect(pRemove);

                if (pRemove.m_pParticleInfo != null)
                {
                    ParticleManager.Instance.RemoveParticleInfo(pRemove.m_pParticleInfo);
                }

                m_ApplyStatusEffectList.Remove(pRemove);
            }
        }
        else if (IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DebuffDispel) == true)    // 모든 디버프 해제
        {
            // 자기 자신도 포함
            removeList.Clear();
            foreach (ApplyStatusEffect pApplyStatus in m_ApplyStatusEffectList)
            {
                if (pApplyStatus.m_pStatusEffectInfo.m_eBuffType == eBuffType.Debuff && pApplyStatus.m_pStatusEffectInfo.m_eUndispellable == eUndispellable.No)
                {
                    removeList.Add(pApplyStatus);
                }
            }

            foreach (ApplyStatusEffect pRemove in removeList)
            {
                ReleaseApplyStatusEffect(pRemove);

                if (pRemove.m_pParticleInfo != null)
                {
                    ParticleManager.Instance.RemoveParticleInfo(pRemove.m_pParticleInfo);
                }

                m_ApplyStatusEffectList.Remove(pRemove);
            }
        }

        removeList.Clear();

        foreach (ApplyStatusEffect pApplyStatusEffect in m_ApplyStatusEffectList)
        {
            if (pApplyStatusEffect.m_pStatusEffectInfo.m_eBuffType == eType)
            {
                if (pApplyStatusEffect.m_pStatusEffectInfo.m_eApplyType == eApplyType.OverTurn)
                {
                    bool IsDead = ActiveStatusEffect(pApplyStatusEffect, null);

                    if (IsDead == true)
                    {
                        break;
                    }
                }

                if (pApplyStatusEffect.m_pStatusEffectInfo.m_eLastingTurn == eMaxLastingTurn.TurnCount)
                {
                    if (--pApplyStatusEffect.m_nRenameTurn == 0)
                    {
                        removeList.Add(pApplyStatusEffect);
                    }
                }
            }
        }

        foreach (ApplyStatusEffect pRemove in removeList)
        {
            ReleaseApplyStatusEffect(pRemove);

            if (pRemove.m_pParticleInfo != null)
            {
                ParticleManager.Instance.RemoveParticleInfo(pRemove.m_pParticleInfo);
            }

            m_ApplyStatusEffectList.Remove(pRemove);
        }

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        switch (EspressoInfo.Instance.m_eGameMode)
        {
            case eGameMode.PvpStage:
                {
                    if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
                    {
                        if (pDataStack.IsPlayerCharacterAllDead(eOwner.Other) == true)
                        {
                            EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                        }
                    }
                    else
                    {
                        if (pDataStack.IsPlayerCharacterAllDead(eOwner.My) == true)
                        {
                            EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Fail);
                        }
                    }
                }
                break;

            case eGameMode.EventPvpStage:
                {
                    if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
                    {
                        if (pDataStack.IsPlayerCharacterAllDead(eOwner.Other) == true)
                        {
                            EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                        }
                    }
                    else
                    {
                        if (pDataStack.IsPlayerCharacterAllDead(eOwner.My) == true)
                        {
                            EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Fail);
                        }
                    }
                }
                break;

            default:
                {
                    if (pDataStack.m_nCurrShildPoint > 0 && pDataStack.m_EnemyMinionTable.Count == 0 && pDataStack.m_nCurrObjectiveCount <= 0)
                    {
                        EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                    }
                }
                break;
        }

        return IsRes;
    }

    public void OnTransformUnit(float fRate_MaxHP, float fRate_ATK, float fRate_ChargePreSP)
    {
        foreach (ApplyStatusEffect pApplyStatusEffect in m_ApplyStatusEffectList)
        {
            switch (pApplyStatusEffect.m_pStatusEffectInfo.m_eEffectType)
            {
                case eDamageEffectType.ATKChange:
                    {
                        if (m_pSlotFixObject_Unit.GetObjectType() == eObjectType.Character)
                        {
                            MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
                            pDataStack.m_nPlayerCharacterAttackValue[(int)m_pSlotFixObject_Unit.GetUnitInfo().m_eElement] -= pApplyStatusEffect.m_nChangeValue;
                        }

                        if (pApplyStatusEffect.m_pStatusEffectInfo.m_eEffectBaseUnitRole == eObjectRoleType.Target)
                        {
                            pApplyStatusEffect.m_nChangeValue = (int)(pApplyStatusEffect.m_nChangeValue * fRate_ATK);
                            pApplyStatusEffect.m_nApplyValue = (int)(pApplyStatusEffect.m_nApplyValue * fRate_ATK);
                        }

                        m_pSlotFixObject_Unit.PlusAdd_ATK(pApplyStatusEffect.m_nChangeValue);

                        if (m_pSlotFixObject_Unit.GetObjectType() == eObjectType.Character)
                        {
                            MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
                            pDataStack.m_nPlayerCharacterAttackValue[(int)m_pSlotFixObject_Unit.GetUnitInfo().m_eElement] += pApplyStatusEffect.m_nChangeValue;
                        }
                    }
                    break;

                case eDamageEffectType.MaxHPChange:
                    {
                        if (pApplyStatusEffect.m_pStatusEffectInfo.m_eEffectBaseUnitRole == eObjectRoleType.Target)
                        {
                            pApplyStatusEffect.m_nChangeValue = (int)(pApplyStatusEffect.m_nChangeValue * fRate_MaxHP);
                            pApplyStatusEffect.m_nApplyValue = (int)(pApplyStatusEffect.m_nApplyValue * fRate_MaxHP);
                        }

                        m_pSlotFixObject_Unit.PlusAdd_MaxHP(pApplyStatusEffect.m_nChangeValue);
                    }
                    break;

                case eDamageEffectType.SPChargeChange:
                    {
                        if (pApplyStatusEffect.m_pStatusEffectInfo.m_eEffectBaseUnitRole == eObjectRoleType.Target)
                        {
                            pApplyStatusEffect.m_nChangeValue = (int)(pApplyStatusEffect.m_nChangeValue * fRate_ChargePreSP);
                            pApplyStatusEffect.m_nApplyValue = (int)(pApplyStatusEffect.m_nApplyValue * fRate_ChargePreSP);
                        }

                        m_pSlotFixObject_Unit.PlusAdd_ChargePerBlock_SP(pApplyStatusEffect.m_nChangeValue);
                        m_pSlotFixObject_Unit.PlusAdd_ChargePerTurn_SP(pApplyStatusEffect.m_nChangeValue);
                    }
                    break;
            }
        }
    }
}
