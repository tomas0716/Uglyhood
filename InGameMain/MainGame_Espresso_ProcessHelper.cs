using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MainGame_Espresso_ProcessHelper
{
    public static bool OnMinionAttackActionDamage(SlotFixObject_Unit pUnit)
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        SelectTargetGenerator pSelectTargetGenerator = new SelectTargetGenerator();

        int nAttackActionID = pUnit.GetUnitInfo().m_nAttack_ActionTableID;
        ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(nAttackActionID);

        pSelectTargetGenerator.OnGenerator(nAttackActionID, pUnit.GetSlot(), pUnit.GetOwner(), pUnit.GetObjectType(), eAttackType.Attack);

        for (int i = 0; i < pActionInfo.m_nSelectTargetAmount; ++i)
        {
            SelectTargetInfo pSelectTargetInfo = pSelectTargetGenerator.GetSelectTargetInfo_byIndex(i);

            if (pSelectTargetInfo != null)
            {
                int nSelect_X = pSelectTargetInfo.m_nSelectTargetSlotX;
                int nSelect_Y = pSelectTargetInfo.m_nSelectTargetSlotY;

                if (pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nSelect_X, nSelect_Y)) == true)
                {
                    Slot pSelectSlot = pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nSelect_X, nSelect_Y)];

                    if (pSelectSlot != null && pSelectSlot.IsSlotFixObject_Obstacle() == false)
                    {
                        DamageTargetGenerator pDamageTargetGenerator = new DamageTargetGenerator();
                        pDamageTargetGenerator.OnGenerator(nAttackActionID, pSelectSlot, pUnit.GetOwner(), pUnit.GetObjectType(), eAttackType.Attack);

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

                                    if (pDamageSlot != null && pDamageSlot.IsSlotFixObject_Obstacle() == false)
                                    {
                                        SlotFixObject_Espresso pSlotFixObject_Espresso = pDamageSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                                        if (pDamageSlot.GetSlotBlock() != null && SlotManager.IsCombinationBlock(pDamageSlot) == true && pSlotFixObject_Espresso == null)
                                        {
                                            // Block or SpecialItemBlock

                                            if (pActionInfo.m_strEffectMissile != "0")
                                            {
                                                GameEvent_MinionAttackMissile pGameEvent = new GameEvent_MinionAttackMissile(pUnit.GetSlot(), pDamageSlot, pActionInfo, eDamageEffectType.ElementDamage, 1, pSelectSlot == pDamageSlot);
                                                GameEventManager.Instance.AddGameEvent(pGameEvent);
                                            }
                                            else
                                            {
                                                Helper.OnSoundPlay(pActionInfo.m_strEffectHit_Sound, false);

                                                if (pActionInfo.m_strEffectHit != "0")
                                                {
                                                    Vector3 vPos_Target = pDamageSlot.GetPosition();
                                                    vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                                                    ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                                                }

                                                if ((pActionInfo.m_strEffectHit_Target != "0" || pActionInfo.m_strEffectHit_Center != "0") && pSelectSlot == pDamageSlot)
                                                {
                                                    Vector3 vPos_Target = pDamageSlot.GetPosition();
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
                                                }

                                                switch (pActionInfo.m_eDamageType)
                                                {
                                                    case eDamageEffectType.BlockItemChange:
                                                        {
                                                            if (pActionInfo.m_eDamageChangeElement != eElement.Neutral)
                                                            {
                                                                pDamageSlot.ChangeSlotBlock((eBlockType)pActionInfo.m_eDamageChangeElement, eSpecialItem.None);
                                                            }

                                                            switch (pActionInfo.m_eDamageChangeType)
                                                            {
                                                                case eDamageChangeType.Bomb:
                                                                    {
                                                                        pDamageSlot.ChangeSlotBlock(eSpecialItem.Square3);
                                                                    }
                                                                    break;
                                                                case eDamageChangeType.Elixir:
                                                                    {
                                                                        pDamageSlot.ChangeSlotBlock(eSpecialItem.Match_Color);
                                                                    }
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                    case eDamageEffectType.AddObstacle:
                                                        {
                                                            switch (pActionInfo.m_eDamageAddObstacleType)
                                                            {
                                                                case eDamageAddObstacleType.Chain:
                                                                    {
                                                                        pDamageSlot.ChangeSlotBlock(eMapSlotItem.Chain, pActionInfo.m_nDamageAddObstacleLevel);
                                                                    }
                                                                    break;

                                                                case eDamageAddObstacleType.Ice:
                                                                    {
                                                                        pDamageSlot.ChangeSlotBlock(eMapSlotItem.Ice, pActionInfo.m_nDamageAddObstacleLevel);
                                                                    }
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                    default:
                                                        {
                                                            pDamageSlot.OnSlotBlockRemove();
                                                            pDamageSlot.SetParameta(pUnit);

                                                            if (pDataStack.m_EnemyColonyCreateTable.ContainsKey(pDamageSlot) == false)
                                                            {
                                                                pDataStack.m_EnemyColonyCreateTable.Add(pDamageSlot, pActionInfo.m_nEnemyColonyID);
                                                            }
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                        else if (pSlotFixObject_Espresso != null)
                                        {
                                            SlotFixObject_Espresso pDestFixObject = null;

                                            switch (pActionInfo.m_eDamageBaseUnitRole)
                                            {
                                                case eObjectRoleType.Actor:
                                                    {
                                                        pDestFixObject = pUnit;
                                                    }
                                                    break;
                                                case eObjectRoleType.Target:
                                                    {
                                                        pDestFixObject = pSlotFixObject_Espresso;
                                                    }
                                                    break;
                                            }

                                            int nDamage = pActionInfo.m_nDamageBaseRate;

                                            if (pActionInfo.m_eDamageBaseUnitRole != eObjectRoleType.Absolute)
                                            {
                                                switch (pActionInfo.m_eDamageBaseUnitProperty)
                                                {
                                                    case eUnitPropertyType.ATK:
                                                        {
                                                            SlotFixObject_Unit pDestUnit = pDestFixObject as SlotFixObject_Unit;

                                                            if (pDestUnit != null)
                                                            {
                                                                nDamage = pDestUnit.GetATK();
                                                            }
                                                        }
                                                        break;
                                                    case eUnitPropertyType.MaxHP:
                                                        {
                                                            nDamage = pDestFixObject.GetMaxHP();
                                                        }
                                                        break;
                                                    case eUnitPropertyType.HP:
                                                        {
                                                            nDamage = pDestFixObject.GetHP();
                                                        }
                                                        break;
                                                    case eUnitPropertyType.MaxSP:
                                                        {
                                                            SlotFixObject_Unit pDestUnit = pDestFixObject as SlotFixObject_Unit;

                                                            if (pDestUnit != null)
                                                            {
                                                                nDamage = pDestUnit.GetMaxSP();
                                                            }
                                                        }
                                                        break;
                                                    case eUnitPropertyType.SP:
                                                        {
                                                            SlotFixObject_Unit pDestUnit = pDestFixObject as SlotFixObject_Unit;

                                                            if (pDestUnit != null)
                                                            {
                                                                nDamage = pDestUnit.GetSP();
                                                            }
                                                        }
                                                        break;
                                                }
                                            }

                                            eElement eDamageElement = eElement.Neutral;
                                            SlotFixObject_Unit pTargetUnit = null;

                                            switch (pSlotFixObject_Espresso.GetObjectType())
                                            {
                                                case eObjectType.Minion:
                                                case eObjectType.MinionBoss:
                                                    {
                                                        pTargetUnit = pSlotFixObject_Espresso as SlotFixObject_Unit;
                                                        if (pTargetUnit != null)
                                                            eDamageElement = pTargetUnit.GetUnitInfo().m_eElement;
                                                    }
                                                    break;
                                                case eObjectType.EnemyBoss:
                                                    {
                                                    }
                                                    break;
                                                case eObjectType.Character:
                                                    {
                                                        pTargetUnit = pSlotFixObject_Espresso as SlotFixObject_Unit;
                                                        if (pTargetUnit != null)
                                                            eDamageElement = pTargetUnit.GetUnitInfo().m_eElement;
                                                    }
                                                    break;
                                            }

                                            float fCorrelationRate, fApplyCorrelationRate;
                                            fCorrelationRate = fApplyCorrelationRate = ExcelDataManager.Instance.m_pElement_Correlation.GetCorrelationRate(pUnit.GetUnitInfo().m_eElement, eDamageElement);

                                            if (pTargetUnit != null)
                                            {
                                                fApplyCorrelationRate -= pTargetUnit.GetAdd_ElementResist(pUnit.GetUnitInfo().m_eElement);
                                                if (fApplyCorrelationRate < 0) fApplyCorrelationRate = 0;
                                            }

                                            nDamage = (int)(nDamage * ((float)pActionInfo.m_nDamageBaseRate / 100.0f) * ((float)pDamageTargetInfo.m_pRangeInfo.m_nRelativeDamageRate / 100.0f) * (fApplyCorrelationRate / 100));
                                            nDamage = (int)(nDamage * (pDamageTargetInfo.m_pConditionInfo.m_fRelativeDamageRate / 100.0f));
                                            if (pActionInfo.m_strEffectMissile != "0")
                                            {
                                                GameEvent_MinionAttackMissile pGameEvent = new GameEvent_MinionAttackMissile(pUnit.GetSlot(), pDamageSlot, pActionInfo, eDamageEffectType.ElementDamage, nDamage, pSelectSlot == pDamageSlot);
                                                GameEventManager.Instance.AddGameEvent(pGameEvent);
                                            }
                                            else
                                            {
                                                Helper.OnSoundPlay(pActionInfo.m_strEffectHit_Sound, false);

                                                if (pActionInfo.m_strEffectHit != "0")
                                                {
                                                    Vector3 vPos_Target = pDamageSlot.GetPosition();
                                                    vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                                                    ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                                                }

                                                if ((pActionInfo.m_strEffectHit_Target != "0" || pActionInfo.m_strEffectHit_Center != "0") && pSelectSlot == pDamageSlot)
                                                {
                                                    Vector3 vPos_Target = pDamageSlot.GetPosition();
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
                                                }

                                                Color color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage];
                                                if (pActionInfo.m_eDamageType == eDamageEffectType.Damage || fCorrelationRate == 100)
                                                {
                                                    color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage];
                                                }
                                                else if (pActionInfo.m_eDamageType != eDamageEffectType.Damage && fCorrelationRate > 100)
                                                {
                                                    color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Strong];
                                                    pSlotFixObject_Espresso.OnDamageText(eUnitDamage.Strong);
                                                }
                                                else if (pActionInfo.m_eDamageType != eDamageEffectType.Damage && fCorrelationRate < 100)
                                                {
                                                    color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Weak];
                                                    pSlotFixObject_Espresso.OnDamageText(eUnitDamage.Weak);
                                                }

                                                int nHP = pSlotFixObject_Espresso.GetHP();

                                                switch (pSlotFixObject_Espresso.GetObjectType())
                                                {
                                                    case eObjectType.Minion:
                                                    case eObjectType.MinionBoss:
                                                    case eObjectType.Character:
                                                        {
                                                            SlotFixObject_Unit pDestUnit = pSlotFixObject_Espresso as SlotFixObject_Unit;

                                                            if (pDestUnit != null)
                                                            {
                                                                if (pDestUnit.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageChange) == true)
                                                                {
                                                                    nDamage = MainGame_Espresso_ProcessHelper.CalculationDamageChange(pDestUnit, nDamage);
                                                                }

                                                                if (pDestUnit.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageReflect) == true)
                                                                {
                                                                    MainGame_Espresso_ProcessHelper.OnDamageReflect(pUnit, pDestUnit, pActionInfo, nDamage);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                }

                                                
                                                pSlotFixObject_Espresso.ChangeHP(nHP - nDamage, color);

                                                if (pSlotFixObject_Espresso.GetHP() <= 0)
                                                {
                                                    switch (pSlotFixObject_Espresso.GetObjectType())
                                                    {
                                                        case eObjectType.Minion:
                                                        case eObjectType.MinionBoss:
                                                            {
                                                                SlotFixObject_Minion pMinion = pSlotFixObject_Espresso as SlotFixObject_Minion;

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
                                                            }
                                                            break;
                                                        case eObjectType.EnemyColony:
                                                            {
                                                                pDataStack.m_EnemyColonyTable.Remove(pSlotFixObject_Espresso.GetSlot().GetSlotIndex());
                                                            }
                                                            break;
                                                    }

                                                    pSlotFixObject_Espresso.OnDead();
                                                    pDamageSlot.SetParameta(pUnit);
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

        return false;
    }

    public static bool OnMinionSkillActionDamage(SlotFixObject_Unit pUnit)
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        SelectTargetGenerator pSelectTargetGenerator = new SelectTargetGenerator();

        int nAttackActionID = pUnit.GetUnitInfo().m_nActiveSkill_ActionTableID;
        ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(nAttackActionID);

        if(pActionInfo == null)
            return false;

        pSelectTargetGenerator.OnGenerator(nAttackActionID, pUnit.GetSlot(), pUnit.GetOwner(), pUnit.GetObjectType(), eAttackType.Skill);

        for (int i = 0; i < pActionInfo.m_nSelectTargetAmount; ++i)
        {
            SelectTargetInfo pSelectTargetInfo = pSelectTargetGenerator.GetSelectTargetInfo_byIndex(i);

            if (pSelectTargetInfo != null)
            {
                int nSelect_X = pSelectTargetInfo.m_nSelectTargetSlotX;
                int nSelect_Y = pSelectTargetInfo.m_nSelectTargetSlotY;

                if (pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nSelect_X, nSelect_Y)) == true)
                {
                    Slot pSelectSlot = pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nSelect_X, nSelect_Y)];

                    if (pSelectSlot != null && pSelectSlot.IsSlotFixObject_Obstacle() == false)
                    {
                        DamageTargetGenerator pDamageTargetGenerator = new DamageTargetGenerator();
                        pDamageTargetGenerator.OnGenerator(nAttackActionID, pSelectSlot, pUnit.GetOwner(), pUnit.GetObjectType(), eAttackType.Skill);

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

                                    if (pDamageSlot != null && pDamageSlot.IsSlotFixObject_Obstacle() == false)
                                    {
                                        SlotFixObject_Espresso pSlotFixObject_Espresso = pDamageSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                                        if (pDamageSlot.GetSlotBlock() != null && SlotManager.IsCombinationBlock(pDamageSlot) == true && pSlotFixObject_Espresso == null)
                                        {
                                            // Block or SpecialItemBlock

                                            if (pActionInfo.m_strEffectMissile != "0")
                                            {
                                                GameEvent_MinionAttackMissile pGameEvent = new GameEvent_MinionAttackMissile(pUnit.GetSlot(), pDamageSlot, pActionInfo, pActionInfo.m_eDamageType, 1, pSelectSlot == pDamageSlot);
                                                GameEventManager.Instance.AddGameEvent(pGameEvent);
                                            }
                                            else
                                            {
                                                Helper.OnSoundPlay(pActionInfo.m_strEffectHit_Sound, false);

                                                if (pActionInfo.m_strEffectHit != "0")
                                                {
                                                    Vector3 vPos_Target = pDamageSlot.GetPosition();
                                                    vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                                                    ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                                                }

                                                if ((pActionInfo.m_strEffectHit_Target != "0" || pActionInfo.m_strEffectHit_Center != "0") && pSelectSlot == pDamageSlot)
                                                {
                                                    Vector3 vPos_Target = pDamageSlot.GetPosition();
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
                                                }

                                                switch (pActionInfo.m_eDamageType)
                                                {
                                                    case eDamageEffectType.BlockItemChange:
                                                        {
                                                            if (pActionInfo.m_eDamageChangeElement != eElement.Neutral)
                                                            {
                                                                pDamageSlot.ChangeSlotBlock((eBlockType)pActionInfo.m_eDamageChangeElement, eSpecialItem.None);
                                                            }

                                                            switch (pActionInfo.m_eDamageChangeType)
                                                            {
                                                                case eDamageChangeType.Bomb:
                                                                    {
                                                                        pDamageSlot.ChangeSlotBlock(eSpecialItem.Square3);
                                                                    }
                                                                    break;
                                                                case eDamageChangeType.Elixir:
                                                                    {
                                                                        pDamageSlot.ChangeSlotBlock(eSpecialItem.Match_Color);
                                                                    }
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                    case eDamageEffectType.AddObstacle:
                                                        {
                                                            switch (pActionInfo.m_eDamageAddObstacleType)
                                                            {
                                                                case eDamageAddObstacleType.Chain:
                                                                    {
                                                                        pDamageSlot.ChangeSlotBlock(eMapSlotItem.Chain, pActionInfo.m_nDamageAddObstacleLevel);
                                                                    }
                                                                    break;

                                                                case eDamageAddObstacleType.Ice:
                                                                    {
                                                                        pDamageSlot.ChangeSlotBlock(eMapSlotItem.Ice, pActionInfo.m_nDamageAddObstacleLevel);
                                                                    }
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                    case eDamageEffectType.UnitSummon:
                                                        {
                                                            pDamageSlot.OnSlotBlockRemove();

                                                            ExcelData_EnemyColonyInfo pEnemyColonyInfo = ExcelDataManager.Instance.m_pEnemyColony.GetEnemyColonyInfo(pActionInfo.m_nEnemyColonyID);
                                                            if (pEnemyColonyInfo == null)
                                                            {
                                                                pEnemyColonyInfo = ExcelDataManager.Instance.m_pEnemyColony.GetEnemyColonyInfo(GameDefine.ms_nBasicEnemyColonyID);
                                                            }

                                                            if (pDataStack.m_EnemyColonyTable.ContainsKey(pDamageSlot.GetSlotIndex()) == true)
                                                            {
                                                                pDataStack.m_EnemyColonyTable.Remove(pDamageSlot.GetSlotIndex());
                                                                pDamageSlot.RemoveAllSlotFixObject();
                                                            }

                                                            SlotFixObject_EnemyColony pSlotFixObject_EnemyColony = new SlotFixObject_EnemyColony(pDamageSlot, pDamageSlot.GetSlotIndex(), pEnemyColonyInfo, 1);
                                                            pDamageSlot.AddSlotFixObject(pSlotFixObject_EnemyColony);
                                                            pDataStack.m_EnemyColonyTable.Add(pDamageSlot.GetSlotIndex(), pSlotFixObject_EnemyColony);

                                                            int nLevel = pActionInfo.m_nTransformUnitLevel;
                                                            if (nLevel == 0)
                                                            {
                                                                nLevel = pUnit.GetLevel();
                                                            }

                                                            ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(pActionInfo.m_nTransformUnitID);
                                                            eObjectType eObType = eObjectType.Minion;
                                                            if (pUnitInfo.m_eUnitType == eUnitType.BossMinion)
                                                                eObType = eObjectType.MinionBoss;

                                                            SlotFixObject_Minion pSlotFixObject_Minion = new SlotFixObject_Minion(pDamageSlot, pActionInfo.m_nTransformUnitID, nLevel, eMinionType.EnemySummonUnit, eObType, pUnit.GetOwner(), false);
                                                            pDamageSlot.AddSlotFixObject(pSlotFixObject_Minion);
                                                            pDataStack.m_EnemySummonUnitTable.Add(pDamageSlot.GetSlotIndex(), pSlotFixObject_Minion);

                                                            pSlotFixObject_Minion.SetDamageEffectType_TransformType(eDamageEffectType.UnitSummon, pActionInfo.m_nTransformUnitLifeTurn, pActionInfo.m_strEffectHit);
                                                        }
                                                        break;
                                                    case eDamageEffectType.Blind:
                                                        {
                                                            InGame_Cloud pApplyCloud = pDataStack.m_pCloudManager.GetCloud(pDamageSlot);

                                                            if (pApplyCloud == null)
                                                            {
                                                                GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_SlotFix/Cloud_" + pUnit.GetOwner());
                                                                ob = GameObject.Instantiate(ob);
                                                                InGame_Cloud pCloud = ob.AddComponent<InGame_Cloud>();
                                                                pCloud.Init(pDamageSlot, pUnit.GetOwner(), pActionInfo.m_nTransformUnitLifeTurn);
                                                                ob.transform.position = pDamageSlot.GetPosition();
                                                                ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                                                                Plane2D pPlane = ob.GetComponent<Plane2D>();
                                                                pPlane.SetDepthPlus(pDamageSlot.GetSlotIndex());

                                                                pDataStack.m_pCloudManager.AddCloud(pCloud);
                                                            }
                                                            else
                                                            {
                                                                if (pApplyCloud.GetOwner() == pUnit.GetOwner())     // 같은 편
                                                                {
                                                                    pApplyCloud.Remove();

                                                                    GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_SlotFix/Cloud_" + pUnit.GetOwner());
                                                                    ob = GameObject.Instantiate(ob);
                                                                    InGame_Cloud pCloud = ob.AddComponent<InGame_Cloud>();
                                                                    pCloud.Init(pDamageSlot, pUnit.GetOwner(), pActionInfo.m_nTransformUnitLifeTurn);
                                                                    ob.transform.position = pDamageSlot.GetPosition();
                                                                    ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                                                                    Plane2D pPlane = ob.GetComponent<Plane2D>();
                                                                    pPlane.SetDepthPlus(pDamageSlot.GetSlotIndex());

                                                                    pDataStack.m_pCloudManager.AddCloud(pCloud);
                                                                }
                                                                else                                                // 다른 편
                                                                {
                                                                    // 처리 안함
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    default:
                                                        {
                                                            pDamageSlot.OnSlotBlockRemove();
                                                            pDamageSlot.SetParameta(pUnit);

                                                            if (pDataStack.m_EnemyColonyCreateTable.ContainsKey(pDamageSlot) == false)
                                                            {
                                                                pDataStack.m_EnemyColonyCreateTable.Add(pDamageSlot, pActionInfo.m_nEnemyColonyID);
                                                            }
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                        else if (pSlotFixObject_Espresso != null)
                                        {
                                            SlotFixObject_Espresso pDestFixObject = null;

                                            switch (pActionInfo.m_eDamageBaseUnitRole)
                                            {
                                                case eObjectRoleType.Actor:
                                                    {
                                                        pDestFixObject = pUnit;
                                                    }
                                                    break;
                                                case eObjectRoleType.Target:
                                                    {
                                                        pDestFixObject = pSlotFixObject_Espresso;
                                                    }
                                                    break;
                                            }

                                            int nDamage = pActionInfo.m_nDamageBaseRate;

                                            if (pActionInfo.m_eDamageBaseUnitRole != eObjectRoleType.Absolute)
                                            {
                                                switch (pActionInfo.m_eDamageBaseUnitProperty)
                                                {
                                                    case eUnitPropertyType.ATK:
                                                        {
                                                            SlotFixObject_Unit pDestUnit = pDestFixObject as SlotFixObject_Unit;

                                                            if (pDestUnit != null)
                                                            {
                                                                nDamage = pDestUnit.GetATK();
                                                            }
                                                        }
                                                        break;
                                                    case eUnitPropertyType.MaxHP:
                                                        {
                                                            nDamage = pDestFixObject.GetMaxHP();
                                                        }
                                                        break;
                                                    case eUnitPropertyType.HP:
                                                        {
                                                            nDamage = pDestFixObject.GetHP();
                                                        }
                                                        break;
                                                    case eUnitPropertyType.MaxSP:
                                                        {
                                                            SlotFixObject_Unit pDestUnit = pDestFixObject as SlotFixObject_Unit;

                                                            if (pDestUnit != null)
                                                            {
                                                                nDamage = pDestUnit.GetMaxSP();
                                                            }
                                                        }
                                                        break;
                                                    case eUnitPropertyType.SP:
                                                        {
                                                            SlotFixObject_Unit pDestUnit = pDestFixObject as SlotFixObject_Unit;

                                                            if (pDestUnit != null)
                                                            {
                                                                nDamage = pDestUnit.GetSP();
                                                            }
                                                        }
                                                        break;
                                                }

                                                nDamage = (int)(nDamage * ((float)pActionInfo.m_nDamageBaseRate / 100.0f) * ((float)pDamageTargetInfo.m_pRangeInfo.m_nRelativeDamageRate / 100.0f));
                                            }
                                            else
                                            {
                                                nDamage = (int)(nDamage * ((float)pDamageTargetInfo.m_pRangeInfo.m_nRelativeDamageRate / 100.0f));
                                            }

                                            nDamage = (int)(nDamage * (pDamageTargetInfo.m_pConditionInfo.m_fRelativeDamageRate / 100.0f));

                                            if (pActionInfo.m_strEffectMissile != "0")
                                            {
                                                GameEvent_MinionAttackMissile pGameEvent = new GameEvent_MinionAttackMissile(pUnit.GetSlot(), pDamageSlot, pActionInfo, pActionInfo.m_eDamageType, nDamage, pSelectSlot == pDamageSlot, pDamageTargetInfo.m_pRangeInfo);
                                                GameEventManager.Instance.AddGameEvent(pGameEvent);
                                            }
                                            else
                                            {
                                                Helper.OnSoundPlay(pActionInfo.m_strEffectHit_Sound, false);

                                                if (pActionInfo.m_strEffectHit != "0")
                                                {
                                                    Vector3 vPos_Target = pDamageSlot.GetPosition();
                                                    vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                                                    ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                                                }

                                                if ((pActionInfo.m_strEffectHit_Target != "0" || pActionInfo.m_strEffectHit_Center != "0") && pSelectSlot == pDamageSlot)
                                                {
                                                    Vector3 vPos_Target = pDamageSlot.GetPosition();
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
                                                }

                                                int nHP = pSlotFixObject_Espresso.GetHP();

                                                switch (pActionInfo.m_eDamageType)
                                                {
                                                    case eDamageEffectType.Damage:
                                                        {
                                                            SlotFixObject_Unit pDestUnit = pSlotFixObject_Espresso as SlotFixObject_Unit;

                                                            if (pDestUnit != null)
                                                            {
                                                                if (pDestUnit.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageChange) == true)
                                                                {
                                                                    nDamage = MainGame_Espresso_ProcessHelper.CalculationDamageChange(pDestUnit, nDamage);
                                                                }

                                                                if (pDestUnit.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageReflect) == true)
                                                                {
                                                                    MainGame_Espresso_ProcessHelper.OnDamageReflect(pUnit, pDestUnit, pActionInfo, nDamage);
                                                                }
                                                            }

                                                            pSlotFixObject_Espresso.ChangeHP(nHP - nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage]);

                                                            if (pSlotFixObject_Espresso.GetHP() <= 0)
                                                            {
                                                                switch (pSlotFixObject_Espresso.GetObjectType())
                                                                {
                                                                    case eObjectType.Minion:
                                                                    case eObjectType.MinionBoss:
                                                                        {
                                                                            SlotFixObject_Minion pMinion = pSlotFixObject_Espresso as SlotFixObject_Minion;

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
                                                                        }
                                                                        break;

                                                                    case eObjectType.EnemyColony:
                                                                        {
                                                                            pDataStack.m_EnemyColonyTable.Remove(pSlotFixObject_Espresso.GetSlot().GetSlotIndex());
                                                                        }
                                                                        break;
                                                                }

                                                                pSlotFixObject_Espresso.OnDead();
                                                                pDamageSlot.SetParameta(pUnit);
                                                            }
                                                        }
                                                        break;

                                                    case eDamageEffectType.Heal:
                                                        {
                                                            pSlotFixObject_Espresso.ChangeHP(nHP + nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.HP_Recovery]);
                                                            pSlotFixObject_Espresso.OnDamageText(eUnitDamage.HP_Recovery);
                                                        }
                                                        break;

                                                    case eDamageEffectType.SPCharge:
                                                        {
                                                            SlotFixObject_Unit pUnit_Target = pSlotFixObject_Espresso as SlotFixObject_Unit;
                                                            if (pUnit_Target != null)
                                                            {
                                                                int nSP = pUnit_Target.GetSP();

                                                                if (nDamage >= 0)
                                                                {
                                                                    pUnit_Target.ChangeSP(nSP + nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.SP_Recharge]);
                                                                    pSlotFixObject_Espresso.OnDamageText(eUnitDamage.SP_Recharge);
                                                                }
                                                                else
                                                                {
                                                                    pUnit_Target.ChangeSP(nSP + nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.SP_Decrease]);
                                                                    pSlotFixObject_Espresso.OnDamageText(eUnitDamage.SP_Decrease);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    case eDamageEffectType.UnitSummon:
                                                        {
                                                            if (pSlotFixObject_Espresso.GetObjectType() == eObjectType.EnemyColony)
                                                            {
                                                                int nLevel = pActionInfo.m_nTransformUnitLevel;
                                                                if (nLevel == 0)
                                                                {
                                                                    nLevel = pUnit.GetLevel();
                                                                }

                                                                ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(pActionInfo.m_nTransformUnitID);
                                                                eObjectType eObType = eObjectType.Minion;
                                                                if (pUnitInfo.m_eUnitType == eUnitType.BossMinion)
                                                                    eObType = eObjectType.MinionBoss;

                                                                SlotFixObject_Minion pSlotFixObject_Minion = new SlotFixObject_Minion(pDamageSlot, pActionInfo.m_nTransformUnitID, nLevel, eMinionType.EnemySummonUnit, eObType, pUnit.GetOwner(), false);
                                                                pDamageSlot.AddSlotFixObject(pSlotFixObject_Minion);
                                                                pDataStack.m_EnemySummonUnitTable.Add(pDamageSlot.GetSlotIndex(), pSlotFixObject_Minion);

                                                                pSlotFixObject_Minion.SetDamageEffectType_TransformType(eDamageEffectType.UnitSummon, pActionInfo.m_nTransformUnitLifeTurn, pActionInfo.m_strEffectHit);
                                                            }
                                                        }
                                                        break;
                                                    case eDamageEffectType.UnitTransform:
                                                        {
                                                            switch (pSlotFixObject_Espresso.GetObjectType())
                                                            {
                                                                case eObjectType.Character:
                                                                    {
                                                                        SlotFixObject_PlayerCharacter pUnit_Target = pSlotFixObject_Espresso as SlotFixObject_PlayerCharacter;

                                                                        if (pUnit_Target != null)
                                                                        {
                                                                            ExcelData_Action_LevelUpInfo pActionLevelUpInfo = null;
                                                                            pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(pUnit_Target.GetUnitInfo().m_nActiveSkill_ActionTableID, pUnit_Target.GetCharacterInvenItemInfo().m_nAction_Level);

                                                                            pUnit_Target.OnTransformUnit(pActionInfo.m_nTransformUnitID, pActionLevelUpInfo.m_nChange_TransformUnitLevel, pActionLevelUpInfo.m_nChange_TransformUnitLevel);
                                                                            pUnit_Target.SetDamageEffectType_TransformType(eDamageEffectType.UnitTransform, pActionLevelUpInfo.m_nChange_TransformUnitLifeTurn, pActionInfo.m_strEffectHit);
                                                                        }
                                                                    }
                                                                    break;

                                                                case eObjectType.Minion:
                                                                case eObjectType.MinionBoss:
                                                                    {
                                                                        SlotFixObject_Unit pUnit_Target = pSlotFixObject_Espresso as SlotFixObject_Unit;

                                                                        if (pUnit_Target != null)
                                                                        {
                                                                            pUnit_Target.OnTransformUnit(pActionInfo.m_nTransformUnitID, pActionInfo.m_nTransformUnitLevel, pActionInfo.m_nTransformUnitLevel);
                                                                            pUnit_Target.SetDamageEffectType_TransformType(eDamageEffectType.UnitTransform, pActionInfo.m_nTransformUnitLifeTurn, pActionInfo.m_strEffectHit);
                                                                        }
                                                                    }
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                    case eDamageEffectType.UnitClone:
                                                        {
                                                            switch (pSlotFixObject_Espresso.GetObjectType())
                                                            {
                                                                case eObjectType.Character:
                                                                    {
                                                                        SlotFixObject_PlayerCharacter pUnit_Target = pSlotFixObject_Espresso as SlotFixObject_PlayerCharacter;

                                                                        if (pUnit_Target != null)
                                                                        {
                                                                            pUnit.OnTransformUnit(pUnit_Target.GetUnitInfo().m_nID, pUnit_Target.GetLevel(), pUnit_Target.GetCharacterInvenItemInfo().m_nAction_Level);
                                                                            pUnit.SetDamageEffectType_TransformType(eDamageEffectType.UnitClone, pActionInfo.m_nTransformUnitLifeTurn, pActionInfo.m_strEffectHit);
                                                                        }
                                                                    }
                                                                    break;

                                                                case eObjectType.Minion:
                                                                case eObjectType.MinionBoss:
                                                                    {
                                                                        SlotFixObject_Minion pUnit_Target = pSlotFixObject_Espresso as SlotFixObject_Minion;

                                                                        if (pUnit_Target != null)
                                                                        {
                                                                            pUnit.OnTransformUnit(pUnit_Target.GetUnitInfo().m_nID, pUnit_Target.GetLevel());
                                                                            pUnit.SetDamageEffectType_TransformType(eDamageEffectType.UnitClone, pActionInfo.m_nTransformUnitLifeTurn, pActionInfo.m_strEffectHit);
                                                                        }
                                                                    }
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                    case eDamageEffectType.Blind:
                                                        {
                                                            InGame_Cloud pApplyCloud = pDataStack.m_pCloudManager.GetCloud(pDamageSlot);

                                                            if (pApplyCloud == null)
                                                            {
                                                                GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_SlotFix/Cloud_" + pUnit.GetOwner());
                                                                ob = GameObject.Instantiate(ob);
                                                                InGame_Cloud pCloud = ob.AddComponent<InGame_Cloud>();
                                                                pCloud.Init(pDamageSlot, pUnit.GetOwner(), pActionInfo.m_nTransformUnitLifeTurn);
                                                                ob.transform.position = pDamageSlot.GetPosition();
                                                                ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                                                                Plane2D pPlane = ob.GetComponent<Plane2D>();
                                                                pPlane.SetDepthPlus(pDamageSlot.GetSlotIndex());

                                                                pDataStack.m_pCloudManager.AddCloud(pCloud);
                                                            }
                                                            else
                                                            {
                                                                if (pApplyCloud.GetOwner() == pUnit.GetOwner())     // 같은 편
                                                                {
                                                                    pApplyCloud.Remove();

                                                                    GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_SlotFix/Cloud_" + pUnit.GetOwner());
                                                                    ob = GameObject.Instantiate(ob);
                                                                    InGame_Cloud pCloud = ob.AddComponent<InGame_Cloud>();
                                                                    pCloud.Init(pDamageSlot, pUnit.GetOwner(), pActionInfo.m_nTransformUnitLifeTurn);
                                                                    ob.transform.position = pDamageSlot.GetPosition();
                                                                    ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                                                                    Plane2D pPlane = ob.GetComponent<Plane2D>();
                                                                    pPlane.SetDepthPlus(pDamageSlot.GetSlotIndex());

                                                                    pDataStack.m_pCloudManager.AddCloud(pCloud);
                                                                }
                                                                else                                                // 다른 편
                                                                {
                                                                    // 처리 안함
                                                                }
                                                            }
                                                        }
                                                        break;
                                                }

                                                MainGame_Espresso_ProcessHelper.ApplyStatusEffect(pUnit.GetSlot(), pDamageSlot, pActionInfo, pDamageTargetInfo.m_pRangeInfo);
                                            }
                                        }
                                        else if (pDamageSlot.GetSlotBlock() == null && pSlotFixObject_Espresso == null)
                                        {
                                            if (pActionInfo.m_eDamageType == eDamageEffectType.Blind)
                                            {
                                                InGame_Cloud pApplyCloud = pDataStack.m_pCloudManager.GetCloud(pDamageSlot);

                                                if (pApplyCloud == null)
                                                {
                                                    GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_SlotFix/Cloud_" + pUnit.GetOwner());
                                                    ob = GameObject.Instantiate(ob);
                                                    InGame_Cloud pCloud = ob.AddComponent<InGame_Cloud>();
                                                    pCloud.Init(pDamageSlot, pUnit.GetOwner(), pActionInfo.m_nTransformUnitLifeTurn);
                                                    ob.transform.position = pDamageSlot.GetPosition();
                                                    ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                                                    Plane2D pPlane = ob.GetComponent<Plane2D>();
                                                    pPlane.SetDepthPlus(pDamageSlot.GetSlotIndex());

                                                    pDataStack.m_pCloudManager.AddCloud(pCloud);
                                                }
                                                else
                                                {
                                                    if (pApplyCloud.GetOwner() == pUnit.GetOwner())     // 같은 편
                                                    {
                                                        pApplyCloud.Remove();

                                                        GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_SlotFix/Cloud_" + pUnit.GetOwner());
                                                        ob = GameObject.Instantiate(ob);
                                                        InGame_Cloud pCloud = ob.AddComponent<InGame_Cloud>();
                                                        pCloud.Init(pDamageSlot, pUnit.GetOwner(), pActionInfo.m_nTransformUnitLifeTurn);
                                                        ob.transform.position = pDamageSlot.GetPosition();
                                                        ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                                                        Plane2D pPlane = ob.GetComponent<Plane2D>();
                                                        pPlane.SetDepthPlus(pDamageSlot.GetSlotIndex());

                                                        pDataStack.m_pCloudManager.AddCloud(pCloud);
                                                    }
                                                    else                                                // 다른 편
                                                    {
                                                        // 처리 안함
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

        return false;
    }

    public static void OnSkillActionDamage(Slot pSrcSlot, Slot pTargetSlot, ExcelData_ActionInfo pActionInfo, eDamageEffectType eDamageType, eObjectRoleType eRoleType, eUnitPropertyType ePropertyType, int nDamageBaseRate, float fDamageRate, float fRelativeDamageRate)
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        if (pTargetSlot != null)
        {
            SlotFixObject_Unit pUnit = null;
            if (pSrcSlot != null)
            {
                pUnit = pSrcSlot.GetLastSlotFixObject() as SlotFixObject_Unit;
            }

            SlotFixObject_Espresso pSlotFixObject_Espresso = pTargetSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

            if (pTargetSlot.GetSlotBlock() != null && pSlotFixObject_Espresso == null)
            {
                // Block or SpecialItemBlock

                // HP 가 1이라는 조건이므로 그냥 없앤다.
                // 현재는 연출 없이 없앰
                switch (pActionInfo.m_eDamageType)
                {
                    case eDamageEffectType.BlockItemChange:
                        {
                            if (pActionInfo.m_eDamageChangeElement != eElement.Neutral)
                            {
                                pTargetSlot.ChangeSlotBlock((eBlockType)pActionInfo.m_eDamageChangeElement, eSpecialItem.None);
                            }

                            switch (pActionInfo.m_eDamageChangeType)
                            {
                                case eDamageChangeType.Bomb:
                                    {
                                        pTargetSlot.ChangeSlotBlock(eSpecialItem.Square3);
                                    }
                                    break;
                                case eDamageChangeType.Elixir:
                                    {
                                        pTargetSlot.ChangeSlotBlock(eSpecialItem.Match_Color);
                                    }
                                    break;
                            }
                        }
                        break;
                    case eDamageEffectType.AddObstacle:
                        {
                            switch (pActionInfo.m_eDamageAddObstacleType)
                            {
                                case eDamageAddObstacleType.Chain:
                                    {
                                        pTargetSlot.ChangeSlotBlock(eMapSlotItem.Chain, pActionInfo.m_nDamageAddObstacleLevel);
                                    }
                                    break;

                                case eDamageAddObstacleType.Ice:
                                    {
                                        pTargetSlot.ChangeSlotBlock(eMapSlotItem.Ice, pActionInfo.m_nDamageAddObstacleLevel);
                                    }
                                    break;
                            }
                        }
                        break;
                    case eDamageEffectType.UnitSummon:
                        {
                            pTargetSlot.RemoveSlotBlock();

                            SlotFixObject_PlayerCharacter pPlayerCharacter = pUnit as SlotFixObject_PlayerCharacter;
                            ExcelData_Action_LevelUpInfo pActionLevelUpInfo = null;
                            pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(pPlayerCharacter.GetUnitInfo().m_nActiveSkill_ActionTableID, pPlayerCharacter.GetCharacterInvenItemInfo().m_nAction_Level);

                            int nUnitID = pActionLevelUpInfo.m_nChange_TransformUnitID;
                            if (nUnitID == 0)
                            {
                                nUnitID = pActionInfo.m_nTransformUnitID;
                            }

                            int nLevel = pActionLevelUpInfo.m_nChange_TransformUnitLevel;
                            if (nLevel == 0)
                            {
                                nLevel = pUnit.GetLevel();
                            }

                            ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(pActionInfo.m_nTransformUnitID);
                            eObjectType eObType = eObjectType.Minion;
                            if (pUnitInfo.m_eUnitType == eUnitType.BossMinion)
                                eObType = eObjectType.MinionBoss;

                            SlotFixObject_Minion pSlotFixObject_Minion = new SlotFixObject_Minion(pTargetSlot, nUnitID, nLevel, eMinionType.PlayerSummonUnit, eObType, pUnit != null ? pUnit.GetOwner() : eOwner.My, false, true);
                            pTargetSlot.AddSlotFixObject(pSlotFixObject_Minion);
                            pDataStack.m_PlayerSummonUnitTable.Add(pTargetSlot.GetSlotIndex(), pSlotFixObject_Minion);
                            pTargetSlot.SetPossibleMove(true);

                            pSlotFixObject_Minion.SetDamageEffectType_TransformType(eDamageEffectType.UnitSummon, pActionLevelUpInfo.m_nChange_TransformUnitLifeTurn, pActionInfo.m_strEffectHit);
                        }
                        break;
                    case eDamageEffectType.BlockShuffle:
                        {
                            GameEvent_BlockShuffleSkill pGameEvent = new GameEvent_BlockShuffleSkill(pUnit, pActionInfo);
                            GameEventManager.Instance.AddGameEvent(pGameEvent);
                        }
                        break;
                    case eDamageEffectType.ShieldPointCharge:
                        {
                        }
                        break;
                    case eDamageEffectType.Blind:
                        {
                            ExcelData_Action_LevelUpInfo pActionLevelUpInfo = null;

                            if (pUnit != null)
                            {
                                SlotFixObject_PlayerCharacter pPlayerCharacter = pUnit as SlotFixObject_PlayerCharacter;
                                pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(pPlayerCharacter.GetUnitInfo().m_nActiveSkill_ActionTableID, pPlayerCharacter.GetCharacterInvenItemInfo().m_nAction_Level);
                            }

                            InGame_Cloud pApplyCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot);

                            if (pApplyCloud == null)
                            {
                                eOwner eOwn = eOwner.My;
                                if(pUnit != null)
                                    eOwn = pUnit.GetOwner();
                                GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_SlotFix/Cloud_" + eOwn);
                                ob = GameObject.Instantiate(ob);
                                InGame_Cloud pCloud = ob.AddComponent<InGame_Cloud>();

                                if (pActionLevelUpInfo != null)
                                {
                                    pCloud.Init(pTargetSlot, eOwn, pActionLevelUpInfo.m_nChange_TransformUnitLifeTurn);
                                }
                                else
                                {
                                    pCloud.Init(pTargetSlot, eOwn, pActionInfo.m_nTransformUnitLifeTurn);
                                }
                                
                                ob.transform.position = pTargetSlot.GetPosition();
                                ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                                Plane2D pPlane = ob.GetComponent<Plane2D>();
                                pPlane.SetDepthPlus(pTargetSlot.GetSlotIndex());

                                pDataStack.m_pCloudManager.AddCloud(pCloud);
                            }
                            else
                            {
                                if (pApplyCloud.GetOwner() == pUnit.GetOwner())     // 같은 편
                                {
                                    pApplyCloud.Remove();

                                    eOwner eOwn = eOwner.My;
                                    if (pUnit != null)
                                        eOwn = pUnit.GetOwner();
                                    GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_SlotFix/Cloud_" + eOwn);

                                    ob = GameObject.Instantiate(ob);
                                    InGame_Cloud pCloud = ob.AddComponent<InGame_Cloud>();

                                    if (pActionLevelUpInfo != null)
                                    {
                                        pCloud.Init(pTargetSlot, eOwn, pActionLevelUpInfo.m_nChange_TransformUnitLifeTurn);
                                    }
                                    else
                                    {
                                        pCloud.Init(pTargetSlot, eOwn, pActionInfo.m_nTransformUnitLifeTurn);
                                    }

                                    ob.transform.position = pTargetSlot.GetPosition();
                                    ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                                    Plane2D pPlane = ob.GetComponent<Plane2D>();
                                    pPlane.SetDepthPlus(pTargetSlot.GetSlotIndex());

                                    pDataStack.m_pCloudManager.AddCloud(pCloud);
                                }
                                else                                                // 다른 편
                                {
                                    // 처리 안함
                                }
                            }
                        }
                        break;
                    default:
                        {
                            if (pTargetSlot.IsSlotFixObject_Obstacle() == false)
                            {
                                switch (pTargetSlot.GetSlotBlock().GetSpecialItem())
                                {
                                    case eSpecialItem.None:
                                        {
                                            pTargetSlot.OnSlotBlockRemove();
                                            pTargetSlot.SetParameta(pUnit);
                                        }
                                        break;
                                    case eSpecialItem.Square3:
                                    case eSpecialItem.Match_Color:
                                        {
                                            SlotFixObject_Obstacle pObstacle_OnlyMineBreak = pTargetSlot.FindFixObject_SlotDyingAtOnlyMineBreak();

                                            if (pObstacle_OnlyMineBreak == null)
                                            {
                                                InGameInfo.Instance.m_IsPlayerCharacterSkillToSpeicalBlockAttack = true;
                                                pDataStack.m_pSlotManager.RemoveSpecialItemBlock(pTargetSlot);
                                            }
                                            else
                                            {
                                                pTargetSlot.SetSlotDying(true);
                                                pTargetSlot.OnSlotDying();
                                            }
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                pTargetSlot.SetSlotDying(true);
                                pTargetSlot.OnSlotDying();
                            }
                        }
                        break;
                }
            }
            else if (pSlotFixObject_Espresso != null)
            {
                SlotFixObject_Unit pDestFixObject = null;

                switch (eRoleType)
                {
                    case eObjectRoleType.Actor:
                        {
                            pDestFixObject = pUnit;
                        }
                        break;
                    case eObjectRoleType.Target:
                        {
                            pDestFixObject = pSlotFixObject_Espresso as SlotFixObject_Unit;
                        }
                        break;
                    case eObjectRoleType.Absolute:
                        {
                        }
                        break;
                }

                int nDamage = nDamageBaseRate;

                if (eRoleType != eObjectRoleType.Absolute)
                {
                    switch (ePropertyType)
                    {
                        case eUnitPropertyType.ATK:
                            {
                                nDamage = pDestFixObject.GetATK();
                            }
                            break;
                        case eUnitPropertyType.MaxHP:
                            {
                                nDamage = pDestFixObject.GetMaxHP();
                            }
                            break;
                        case eUnitPropertyType.HP:
                            {
                                nDamage = pDestFixObject.GetHP();
                            }
                            break;
                        case eUnitPropertyType.MaxSP:
                            {
                                nDamage = pDestFixObject.GetMaxSP();
                            }
                            break;
                        case eUnitPropertyType.SP:
                            {
                                nDamage = pDestFixObject.GetSP();
                            }
                            break;
                    }

                    nDamage = (int)(nDamage * ((float)nDamageBaseRate / 100.0f) * fDamageRate);
                }
                else
                {
                    nDamage = (int)(nDamage * fDamageRate);
                }

                nDamage = (int)(nDamage * (fRelativeDamageRate / 100.0f));

                int nHP = pSlotFixObject_Espresso.GetHP();

                switch (eDamageType)
                {
                    case eDamageEffectType.Damage:
                        {
                            switch (pSlotFixObject_Espresso.GetObjectType())
                            {
                                case eObjectType.EnemyColony:
                                    {
                                        pSlotFixObject_Espresso.ChangeHP(nHP - nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage]);

                                        if (pSlotFixObject_Espresso.GetHP() <= 0)
                                        {
                                            pDataStack.m_EnemyColonyTable.Remove(pSlotFixObject_Espresso.GetSlot().GetSlotIndex());

                                            pSlotFixObject_Espresso.OnDead();
                                            pTargetSlot.SetParameta(pUnit);
                                        }
                                    }
                                    break;
                                case eObjectType.Minion:
                                case eObjectType.MinionBoss:
                                    {
                                        SlotFixObject_Minion pMinion = pSlotFixObject_Espresso as SlotFixObject_Minion;
                                        if (pMinion != null)
                                        {
                                            if (pMinion.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageChange) == true)
                                            {
                                                nDamage = MainGame_Espresso_ProcessHelper.CalculationDamageChange(pMinion, nDamage);
                                            }

                                            if (pMinion.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageReflect) == true)
                                            {
                                                MainGame_Espresso_ProcessHelper.OnDamageReflect(pUnit, pMinion, pActionInfo, nDamage);
                                            }

                                            pSlotFixObject_Espresso.ChangeHP(nHP - nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage]);

                                            if (pSlotFixObject_Espresso.GetHP() <= 0)
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

                                                pMinion.OnDead();
                                                pTargetSlot.SetParameta(pUnit);
                                            }
                                        }
                                    }
                                    break;
                                case eObjectType.EnemyBoss:
                                    {
                                    }
                                    break;
                                case eObjectType.Character:
                                    {
                                        SlotFixObject_PlayerCharacter pPlayerCharacter = pSlotFixObject_Espresso as SlotFixObject_PlayerCharacter;

                                        if (pPlayerCharacter != null)
                                        {
                                            if (pPlayerCharacter.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageChange) == true)
                                            {
                                                nDamage = MainGame_Espresso_ProcessHelper.CalculationDamageChange(pPlayerCharacter, nDamage);
                                            }

                                            if (pPlayerCharacter.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageReflect) == true)
                                            {
                                                MainGame_Espresso_ProcessHelper.OnDamageReflect(pUnit, pPlayerCharacter, pActionInfo, nDamage);
                                            }

                                            pSlotFixObject_Espresso.ChangeHP(nHP - nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage]);

                                            if (pSlotFixObject_Espresso.GetHP() <= 0)
                                            {
                                                pSlotFixObject_Espresso.OnDead();
                                                pTargetSlot.SetParameta(pUnit);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        break;

                    case eDamageEffectType.Heal:
                        {
                            pSlotFixObject_Espresso.ChangeHP(nHP + nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.HP_Recovery]);
                            pSlotFixObject_Espresso.OnDamageText(eUnitDamage.HP_Recovery);
                        }
                        break;

                    case eDamageEffectType.SPCharge:
                        {
                            pUnit = pSlotFixObject_Espresso as SlotFixObject_Unit;

                            if (pUnit != null)
                            {
                                int nSP = pUnit.GetSP();

                                if (nDamage > 0)
                                {
                                    pUnit.ChangeSP(nSP + nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.SP_Recharge]);
                                    pSlotFixObject_Espresso.OnDamageText(eUnitDamage.SP_Recharge);
                                }
                                else if(nDamage < 0)
                                {
                                    pUnit.ChangeSP(nSP + nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.SP_Decrease]);
                                    pSlotFixObject_Espresso.OnDamageText(eUnitDamage.SP_Decrease);
                                }
                            }
                        }
                        break;

                    case eDamageEffectType.Revival:
                        {
                            SlotFixObject_PlayerCharacter pPlayerCharacter = pSlotFixObject_Espresso as SlotFixObject_PlayerCharacter;

                            if (pPlayerCharacter != null)
                            {
                                pPlayerCharacter.Revival(nDamage);
                            }
                        }
                        break;
                    case eDamageEffectType.UnitTransform:
                        {
                            switch (pSlotFixObject_Espresso.GetObjectType())
                            {
                                case eObjectType.Character:
                                    {
                                        SlotFixObject_PlayerCharacter pUnit_Target = pSlotFixObject_Espresso as SlotFixObject_PlayerCharacter;

                                        if (pUnit_Target != null)
                                        {
                                            ExcelData_Action_LevelUpInfo pActionLevelUpInfo = null;
                                            pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(pUnit_Target.GetUnitInfo().m_nActiveSkill_ActionTableID, pUnit_Target.GetCharacterInvenItemInfo().m_nAction_Level);

                                            pUnit_Target.OnTransformUnit(pActionInfo.m_nTransformUnitID, pActionLevelUpInfo.m_nChange_TransformUnitLevel, pActionLevelUpInfo.m_nChange_TransformUnitLevel);
                                            pUnit_Target.SetDamageEffectType_TransformType(eDamageEffectType.UnitTransform, pActionLevelUpInfo.m_nChange_TransformUnitLifeTurn, pActionInfo.m_strEffectHit);
                                        }
                                    }
                                    break;

                                case eObjectType.Minion:
                                case eObjectType.MinionBoss:
                                    {
                                        SlotFixObject_Unit pUnit_Target = pSlotFixObject_Espresso as SlotFixObject_Unit;

                                        if (pUnit_Target != null)
                                        {
                                            pUnit_Target.OnTransformUnit(pActionInfo.m_nTransformUnitID, pActionInfo.m_nTransformUnitLevel, pActionInfo.m_nTransformUnitLevel);
                                            pUnit_Target.SetDamageEffectType_TransformType(eDamageEffectType.UnitTransform, pActionInfo.m_nTransformUnitLifeTurn, pActionInfo.m_strEffectHit);
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case eDamageEffectType.UnitClone:
                        {
                            switch (pSlotFixObject_Espresso.GetObjectType())
                            {
                                case eObjectType.Character:
                                    {
                                        SlotFixObject_PlayerCharacter pUnit_Target = pSlotFixObject_Espresso as SlotFixObject_PlayerCharacter;

                                        if (pUnit_Target != null)
                                        {
                                            pUnit.OnTransformUnit(pUnit_Target.GetUnitInfo().m_nID, pUnit_Target.GetLevel(), pUnit_Target.GetCharacterInvenItemInfo().m_nAction_Level);
                                            pUnit.SetDamageEffectType_TransformType(eDamageEffectType.UnitClone, pActionInfo.m_nTransformUnitLifeTurn, pActionInfo.m_strEffectHit);
                                        }
                                    }
                                    break;

                                case eObjectType.Minion:
                                case eObjectType.MinionBoss:
                                    {
                                        SlotFixObject_Minion pUnit_Target = pSlotFixObject_Espresso as SlotFixObject_Minion;

                                        if (pUnit_Target != null)
                                        {
                                            pUnit.OnTransformUnit(pUnit_Target.GetUnitInfo().m_nID, pUnit_Target.GetLevel());
                                            pUnit.SetDamageEffectType_TransformType(eDamageEffectType.UnitClone, pActionInfo.m_nTransformUnitLifeTurn, pActionInfo.m_strEffectHit);
                                        }
                                    }
                                    break;
                            }
                        }
                        break;

                    case eDamageEffectType.BlockShuffle:
                        {
                            GameEvent_BlockShuffleSkill pGameEvent = new GameEvent_BlockShuffleSkill(pUnit, pActionInfo);
                            GameEventManager.Instance.AddGameEvent(pGameEvent);
                        }
                        break;

                    case eDamageEffectType.CharacterSwap:
                        {
                        }
                        break;

                    case eDamageEffectType.ShieldPointCharge:
                        {
                        }
                        break;

                    case eDamageEffectType.Blind:
                        {
                            ExcelData_Action_LevelUpInfo pActionLevelUpInfo = null;

                            if (pUnit != null)
                            {
                                SlotFixObject_PlayerCharacter pPlayerCharacter = pUnit as SlotFixObject_PlayerCharacter;
                                pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(pPlayerCharacter.GetUnitInfo().m_nActiveSkill_ActionTableID, pPlayerCharacter.GetCharacterInvenItemInfo().m_nAction_Level);
                            }

                            InGame_Cloud pApplyCloud = pDataStack.m_pCloudManager.GetCloud(pSlotFixObject_Espresso.GetSlot());

                            if (pApplyCloud == null)
                            {
                                eOwner eOwn = eOwner.My;
                                if (pUnit != null)
                                    eOwn = pUnit.GetOwner();
                                GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_SlotFix/Cloud_" + eOwn);

                                ob = GameObject.Instantiate(ob);
                                InGame_Cloud pCloud = ob.AddComponent<InGame_Cloud>();

                                if (pActionLevelUpInfo != null)
                                {
                                    pCloud.Init(pSlotFixObject_Espresso.GetSlot(), eOwn, pActionLevelUpInfo.m_nChange_TransformUnitLifeTurn);
                                }
                                else
                                {
                                    pCloud.Init(pSlotFixObject_Espresso.GetSlot(), eOwn, pActionInfo.m_nTransformUnitLifeTurn);
                                }

                                ob.transform.position = pSlotFixObject_Espresso.GetSlot().GetPosition();
                                ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                                Plane2D pPlane = ob.GetComponent<Plane2D>();
                                pPlane.SetDepthPlus(pSlotFixObject_Espresso.GetSlot().GetSlotIndex());

                                pDataStack.m_pCloudManager.AddCloud(pCloud);
                            }
                            else
                            {
                                if (pApplyCloud.GetOwner() == pUnit.GetOwner())     // 같은 편
                                {
                                    pApplyCloud.Remove();

                                    eOwner eOwn = eOwner.My;
                                    if (pUnit != null)
                                        eOwn = pUnit.GetOwner();
                                    GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_SlotFix/Cloud_" + eOwn);

                                    ob = GameObject.Instantiate(ob);
                                    InGame_Cloud pCloud = ob.AddComponent<InGame_Cloud>();

                                    if (pActionLevelUpInfo != null)
                                    {
                                        pCloud.Init(pSlotFixObject_Espresso.GetSlot(), eOwn, pActionLevelUpInfo.m_nChange_TransformUnitLifeTurn);
                                    }
                                    else
                                    {
                                        pCloud.Init(pSlotFixObject_Espresso.GetSlot(), eOwn, pActionInfo.m_nTransformUnitLifeTurn);
                                    }

                                    ob.transform.position = pSlotFixObject_Espresso.GetSlot().GetPosition();
                                    ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                                    Plane2D pPlane = ob.GetComponent<Plane2D>();
                                    pPlane.SetDepthPlus(pSlotFixObject_Espresso.GetSlot().GetSlotIndex());

                                    pDataStack.m_pCloudManager.AddCloud(pCloud);
                                }
                                else                                                // 다른 편
                                {
                                    // 처리 안함
                                }
                            }
                        }
                        break;
                }
            }
            else if (pTargetSlot.GetSlotBlock() == null && pSlotFixObject_Espresso == null)
            {
                if (pActionInfo.m_eDamageType == eDamageEffectType.Damage)
                {
                    pTargetSlot.SetSlotDying(true);
                    pTargetSlot.OnSlotDying();
                }
                else if (pActionInfo.m_eDamageType == eDamageEffectType.Blind)
                {
                    ExcelData_Action_LevelUpInfo pActionLevelUpInfo = null;

                    if (pUnit != null)
                    {
                        SlotFixObject_PlayerCharacter pPlayerCharacter = pUnit as SlotFixObject_PlayerCharacter;
                        pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(pPlayerCharacter.GetUnitInfo().m_nActiveSkill_ActionTableID, pPlayerCharacter.GetCharacterInvenItemInfo().m_nAction_Level);
                    }

                    InGame_Cloud pApplyCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot);

                    if (pApplyCloud == null)
                    {
                        eOwner eOwn = eOwner.My;
                        if (pUnit != null)
                            eOwn = pUnit.GetOwner();
                        GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_SlotFix/Cloud_" + eOwn);

                        ob = GameObject.Instantiate(ob);
                        InGame_Cloud pCloud = ob.AddComponent<InGame_Cloud>();

                        if (pActionLevelUpInfo != null)
                        {
                            pCloud.Init(pTargetSlot, eOwn, pActionLevelUpInfo.m_nChange_TransformUnitLifeTurn);
                        }
                        else
                        {
                            pCloud.Init(pTargetSlot, eOwn, pActionInfo.m_nTransformUnitLifeTurn);
                        }

                        ob.transform.position = pTargetSlot.GetPosition();
                        ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                        Plane2D pPlane = ob.GetComponent<Plane2D>();
                        pPlane.SetDepthPlus(pTargetSlot.GetSlotIndex());

                        pDataStack.m_pCloudManager.AddCloud(pCloud);
                    }
                    else
                    {
                        if (pApplyCloud.GetOwner() == pUnit.GetOwner())     // 같은 편
                        {
                            pApplyCloud.Remove();

                            eOwner eOwn = eOwner.My;
                            if (pUnit != null)
                                eOwn = pUnit.GetOwner();
                            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_SlotFix/Cloud_" + eOwn);

                            ob = GameObject.Instantiate(ob);
                            InGame_Cloud pCloud = ob.AddComponent<InGame_Cloud>();
                            
                            if (pActionLevelUpInfo != null)
                            {
                                pCloud.Init(pTargetSlot, eOwn, pActionLevelUpInfo.m_nChange_TransformUnitLifeTurn);
                            }
                            else
                            {
                                pCloud.Init(pTargetSlot, eOwn, pActionInfo.m_nTransformUnitLifeTurn);
                            }

                            ob.transform.position = pTargetSlot.GetPosition();
                            ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                            Plane2D pPlane = ob.GetComponent<Plane2D>();
                            pPlane.SetDepthPlus(pTargetSlot.GetSlotIndex());

                            pDataStack.m_pCloudManager.AddCloud(pCloud);
                        }
                        else                                                // 다른 편
                        {
                            // 처리 안함
                        }
                    }
                }
            }
        }
    }

    public static bool IsPlayerCharacterSkillUsePossible(SlotFixObject_PlayerCharacter pPlayerCharacter)
    {
        OutputLog.Log("MainGame_Espresso_ProcessHelper : IsPlayerCharacterSkillUsePossible Begin");

        if (pPlayerCharacter.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.AttackBlock) == true)
        {
            return false;
        }

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        ExcelData_UnitInfo pUnitInfo = pPlayerCharacter.GetUnitInfo();

        if (pUnitInfo != null)
        {
            ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(pUnitInfo.m_nActiveSkill_ActionTableID);

            if (pActionInfo != null)
            {
                CharacterInvenItemInfo pCharacterInfo = pPlayerCharacter.GetCharacterInvenItemInfo();
                ExcelData_Action_LevelUpInfo pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(pActionInfo.m_nID, pCharacterInfo.m_nAction_Level);

                SelectTargetGenerator pSelectTargetGenerator = new SelectTargetGenerator();

                pSelectTargetGenerator.OnGenerator(pActionInfo.m_nID, pPlayerCharacter.GetSlot(), pPlayerCharacter.GetOwner(), pPlayerCharacter.GetObjectType(), eAttackType.Skill, pActionLevelUpInfo.m_nChange_SelectTargetRangeLevel);;

                switch (pActionInfo.m_eTargetSelectType)
                {
                    case eTargetSelectType.Auto:
                        {
                            for (int i = 0; i < pActionInfo.m_nSelectTargetAmount; ++i)
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
                                        pDamageTargetGenerator.OnGenerator(pActionInfo.m_nID, pSelectSlot, pPlayerCharacter.GetOwner(), pPlayerCharacter.GetObjectType(), eAttackType.Skill, pActionLevelUpInfo);

                                        int nNumDamageTarget = pDamageTargetGenerator.GetNumDamageTargetInfo();

                                        if (nNumDamageTarget != 0)
                                        {
                                            OutputLog.Log("MainGame_Espresso_ProcessHelper : IsPlayerCharacterSkillUsePossible true : Auto");

                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case eTargetSelectType.ManualAndAuto:
                        {
                            bool IsReturnValue = false;
                            int nRealTarget = 0;

                            int nNumSelectTarget = pSelectTargetGenerator.GetNumSelectTargetInfo();
                            for (int i = 0; i < nNumSelectTarget; ++i)
                            {
                                SelectTargetInfo pSelectTargetInfo = pSelectTargetGenerator.GetSelectTargetInfo_byIndex(i);

                                if (pSelectTargetInfo != null)
                                {
                                    int nSelect_X = pSelectTargetInfo.m_nSelectTargetSlotX;
                                    int nSelect_Y = pSelectTargetInfo.m_nSelectTargetSlotY;

                                    if (pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nSelect_X, nSelect_Y)) == true)
                                    {
                                        OutputLog.Log("MainGame_Espresso_ProcessHelper : IsPlayerCharacterSkillUsePossible true : ManualAndAuto");
                                        Slot pSelectSlot = pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nSelect_X, nSelect_Y)];

                                        DamageTargetGenerator pDamageTargetGenerator = new DamageTargetGenerator();
                                        pDamageTargetGenerator.OnGenerator(pActionInfo.m_nID, pSelectSlot, pPlayerCharacter.GetOwner(), pPlayerCharacter.GetObjectType(), eAttackType.Skill, pActionLevelUpInfo);

                                        int nNumDamageTarget = pDamageTargetGenerator.GetNumDamageTargetInfo();

                                        if (nNumDamageTarget != 0)
                                        {
                                            OutputLog.Log("MainGame_Espresso_ProcessHelper : IsPlayerCharacterSkillUsePossible true : ManualAndAuto");
                                            ++nRealTarget;
                                            IsReturnValue = true;
                                        }
                                    }
                                }
                            }

                            if (pActionInfo.m_eDamageType == eDamageEffectType.CharacterSwap)
                            {
                                if (nRealTarget < 2)
                                {
                                    IsReturnValue = false;
                                }
                            }

                            return IsReturnValue;
                        }
                }
            }
        }

        OutputLog.Log("MainGame_Espresso_ProcessHelper : IsPlayerCharacterSkillUsePossible false End");

        return false;
    }

    public static bool IsMinionPossibleAction(int nActionID, Slot pSlot, eOwner eOwner, eAttackType eAtkType)
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        SelectTargetGenerator pSelectTargetGenerator = new SelectTargetGenerator();

        ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(nActionID);

        if (pActionInfo == null || pSlot == null)
            return false;

        SlotFixObject_Unit pUnit = pSlot.GetLastSlotFixObject() as SlotFixObject_Unit;
        pSelectTargetGenerator.OnGenerator(nActionID, pSlot, eOwner, pUnit.GetObjectType(), eAtkType);

        for (int i = 0; i < pActionInfo.m_nSelectTargetAmount; ++i)
        {
            SelectTargetInfo pSelectTargetInfo = pSelectTargetGenerator.GetSelectTargetInfo_byIndex(i);

            if (pSelectTargetInfo != null)
            {
                int nSelect_X = pSelectTargetInfo.m_nSelectTargetSlotX;
                int nSelect_Y = pSelectTargetInfo.m_nSelectTargetSlotY;

                if (pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nSelect_X, nSelect_Y)) == true)
                {
                    Slot pSelectSlot = pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nSelect_X, nSelect_Y)];

                    if (pSelectSlot != null && pSelectSlot.IsSlotFixObject_Obstacle() == false)
                    {
                        DamageTargetGenerator pDamageTargetGenerator = new DamageTargetGenerator();
                        pDamageTargetGenerator.OnGenerator(nActionID, pSelectSlot, eOwner, pUnit.GetObjectType(), eAtkType);

                        int nNumDamageTarget = pDamageTargetGenerator.GetNumDamageTargetInfo();

                        for (int j = 0; j < nNumDamageTarget; ++j)
                        {
                            DamageTargetInfo pDamageTargetInfo = pDamageTargetGenerator.GetDamageTargetInfo_byIndex(j);

                            if (pDamageTargetInfo != null)
                            {
                                int nDamage_X = nSelect_X + pDamageTargetInfo.m_pRangeInfo.m_nPosX;
                                int nDamage_Y = nSelect_Y + pDamageTargetInfo.m_pRangeInfo.m_nPosY;

								if (eOwner == eOwner.My || pDamageTargetInfo.m_pRangeInfo.m_eRangeType == eRangeType.All)
								{
									nDamage_Y = nSelect_Y + pDamageTargetInfo.m_pRangeInfo.m_nPosY * -1;
								}

								if (pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nDamage_X, nDamage_Y)) == true)
                                {
                                    OutputLog.Log("MainGame_Espresso_ProcessHelper : IsPlayerCharacterSkillUsePossible true : Auto");

                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    public static bool IsApplyStatusEffect(Slot pSlot_Subject, Slot pSlot_Target, ExcelData_Action_DamageTargetRangeInfo pDamageTargetRangeInfo)
    {
        ExcelData_StatusEffectInfo pStatusEffectInfo = ExcelDataManager.Instance.m_pStatusEffect.GetStatusEffectInfo(pDamageTargetRangeInfo.m_nStatusEffectTableID);

        eOwner eOwner_Subject = eOwner.My;

        if (pSlot_Target != null && pStatusEffectInfo != null)
        {
            SlotFixObject_Unit pUnit_Subject = null;

            if (pSlot_Subject != null)
            {
                List<SlotFixObject> list_Subject = pSlot_Subject.GetSlotFixObjectList();
                foreach (SlotFixObject pSlotFixObject in list_Subject)
                {
                    SlotFixObject_Unit pUnit = pSlotFixObject as SlotFixObject_Unit;
                    if (pUnit != null)
                    {
                        pUnit_Subject = pUnit;
                        eOwner_Subject = pUnit_Subject.GetOwner();
                    }
                }
            }

            SlotFixObject_Unit pUnit_Target = null;

            List<SlotFixObject> list_Target = pSlot_Target.GetSlotFixObjectList();
            foreach (SlotFixObject pSlotFixObject in list_Target)
            {
                SlotFixObject_Unit pUnit = pSlotFixObject as SlotFixObject_Unit;
                if (pUnit != null)
                {
                    pUnit_Target = pUnit;
                }
            }

            if (pUnit_Target != null)
            {
                if (pStatusEffectInfo != null)
                {
                    List<ExcelData_StatusEffect_TargetConditionInfo> targetConditionInfoList = ExcelDataManager.Instance.m_pStatusEffect_TargetCondition.GetStatusEffect_TargetConditionInfoList(pStatusEffectInfo.m_nID);

                    foreach (ExcelData_StatusEffect_TargetConditionInfo pTargetConditionInfo in targetConditionInfoList)
                    {
                        if (pUnit_Subject != null)
                        {
                            if (IsApplyStatusEffect(pUnit_Subject, pUnit_Target, pTargetConditionInfo) == true)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (IsApplyStatusEffect_ForNoSubject(pUnit_Target, pTargetConditionInfo) == true)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    private static bool IsApplyStatusEffect(SlotFixObject_Unit pUnit_Subject, SlotFixObject_Unit pUnit_Target, ExcelData_StatusEffect_TargetConditionInfo pTargetConditionInfo)
    {
        switch (pTargetConditionInfo.m_eTargetSide)
        {
            case eSideType.SameSide:
                {
                    if (pUnit_Subject.GetOwner() != pUnit_Target.GetOwner())
                    {
                        return false;
                    }
                }
                break;

            case eSideType.OhterSide:
                {
                    if (pUnit_Subject.GetOwner() == pUnit_Target.GetOwner())
                    {
                        return false;
                    }
                }
                break;
        }

        if (pTargetConditionInfo.m_eElement != eElement.All && pUnit_Target.GetUnitInfo().m_eElement != pTargetConditionInfo.m_eElement)
        {
            return false;
        }

        if (pTargetConditionInfo.m_nUnitTableID != 0 && pUnit_Target.GetUnitInfo().m_nID != pTargetConditionInfo.m_nUnitTableID)
        {
            return false;
        }

        return true;
    }

    private static bool IsApplyStatusEffect_ForNoSubject(SlotFixObject_Unit pUnit_Target, ExcelData_StatusEffect_TargetConditionInfo pTargetConditionInfo)
    {
        switch (pTargetConditionInfo.m_eTargetSide)
        {
            case eSideType.SameSide:
                {
                    if (eOwner.My != pUnit_Target.GetOwner())
                    {
                        return false;
                    }
                }
                break;

            case eSideType.OhterSide:
                {
                    if (eOwner.My == pUnit_Target.GetOwner())
                    {
                        return false;
                    }
                }
                break;
        }

        if (pTargetConditionInfo.m_eElement != eElement.All && pUnit_Target.GetUnitInfo().m_eElement != pTargetConditionInfo.m_eElement)
        {
            return false;
        }

        if (pTargetConditionInfo.m_nUnitTableID != 0 && pUnit_Target.GetUnitInfo().m_nID != pTargetConditionInfo.m_nUnitTableID)
        {
            return false;
        }

        return true;
    }

    public static void ApplyStatusEffect(Slot pSlot_Subject, Slot pSlot_Target, ExcelData_ActionInfo pActionInfo, ExcelData_Action_DamageTargetRangeInfo pDamageTargetRangeInfo, ExcelData_Action_LevelUpInfo pActionLevelUpInfo = null)
    {
        if (pDamageTargetRangeInfo != null)
        {
            ExcelData_StatusEffectInfo pStatusEffectInfo = ExcelDataManager.Instance.m_pStatusEffect.GetStatusEffectInfo(pDamageTargetRangeInfo.m_nStatusEffectTableID);
            int nApplyValue = ApplyStatusEffect(pSlot_Subject, pSlot_Target, pStatusEffectInfo, false, pActionInfo, pActionLevelUpInfo);

            if (pSlot_Subject != null && pStatusEffectInfo != null && pStatusEffectInfo.m_eApplyType == eApplyType.Instant && 
                (pStatusEffectInfo.m_eEffectType == eDamageEffectType.Damage || pStatusEffectInfo.m_eEffectType == eDamageEffectType.ElementDamage) &&
                nApplyValue > 0)
            {
                SlotFixObject_Unit pUnit_Subject = pSlot_Subject.GetLastSlotFixObject() as SlotFixObject_Unit;
                SlotFixObject_Unit pUnit_Target = pSlot_Target.GetLastSlotFixObject() as SlotFixObject_Unit;

                if (pUnit_Subject != null && pUnit_Target != null)
                {
                    OnDamageReflect(pUnit_Subject, pUnit_Target, pActionInfo, nApplyValue);
                }
            }
        }
    }

    public static int ApplyStatusEffect(Slot pSlot_Subject, Slot pSlot_Target, ExcelData_StatusEffectInfo pStatusEffectInfo, bool IsForceApply, ExcelData_ActionInfo pActionInfo, ExcelData_Action_LevelUpInfo pActionLevelUpInfo = null)
    {
        int nApplyValue = 0;

        eOwner eOwner_Subject = eOwner.My;

        if (pSlot_Target != null && pStatusEffectInfo != null)
        {
            SlotFixObject_Unit pUnit_Subject = null;

            if (pSlot_Subject != null)
            {
                List<SlotFixObject> list_Subject = pSlot_Subject.GetSlotFixObjectList();
                foreach (SlotFixObject pSlotFixObject in list_Subject)
                {
                    SlotFixObject_Unit pUnit = pSlotFixObject as SlotFixObject_Unit;
                    if (pUnit != null)
                    {
                        pUnit_Subject = pUnit;
                        eOwner_Subject = pUnit_Subject.GetOwner();
                    }
                }
            }

            SlotFixObject_Unit pUnit_Target = null;

            List<SlotFixObject> list_Target = pSlot_Target.GetSlotFixObjectList();
            foreach (SlotFixObject pSlotFixObject in list_Target)
            {
                SlotFixObject_Unit pUnit = pSlotFixObject as SlotFixObject_Unit;
                if (pUnit != null)
                {
                    pUnit_Target = pUnit;
                }
            }

            if (pUnit_Target != null)
            {
                if (pStatusEffectInfo != null)
                {
                    bool IsValid = false;

                    List<ExcelData_StatusEffect_TargetConditionInfo> targetConditionInfoList = ExcelDataManager.Instance.m_pStatusEffect_TargetCondition.GetStatusEffect_TargetConditionInfoList(pStatusEffectInfo.m_nID);

                    foreach (ExcelData_StatusEffect_TargetConditionInfo pTargetConditionInfo in targetConditionInfoList)
                    {
                        if (pUnit_Subject != null)
                        {
                            if (IsApplyStatusEffect(pUnit_Subject, pUnit_Target, pTargetConditionInfo) == true)
                            {
                                IsValid = true;
                            }
                        }
                        else
                        {
                            if (IsApplyStatusEffect_ForNoSubject(pUnit_Target, pTargetConditionInfo) == true)
                            {
                                IsValid = true;
                            }
                        }
                    }

                    if (IsValid == true || IsForceApply == true)
                    {
                        nApplyValue = pStatusEffectInfo.m_nEffectBaseRate;
                        SlotFixObject_Unit pDestFixObject = null;

                        switch (pStatusEffectInfo.m_eEffectBaseUnitRole)
                        {
                            case eObjectRoleType.Actor:
                                {
                                    pDestFixObject = pUnit_Subject;
                                }
                                break;

                            case eObjectRoleType.Target:
                                {
                                    pDestFixObject = pUnit_Target;
                                }
                                break;
                            case eObjectRoleType.Absolute:
                                {
                                }
                                break;
                        }

                        if (pDestFixObject != null)
                        {
                            switch (pStatusEffectInfo.m_eEffectBaseUnitProperty)
                            {
                                case eUnitPropertyType.ATK:
                                    {
                                        nApplyValue = pDestFixObject.GetATK();
                                    }
                                    break;
                                case eUnitPropertyType.MaxHP:
                                    {
                                        nApplyValue = pDestFixObject.GetMaxHP();
                                    }
                                    break;
                                case eUnitPropertyType.HP:
                                    {
                                        nApplyValue = pDestFixObject.GetHP();
                                    }
                                    break;
                                case eUnitPropertyType.MaxSP:
                                    {
                                        nApplyValue = pDestFixObject.GetMaxSP();
                                    }
                                    break;
                                case eUnitPropertyType.SP:
                                    {
                                        nApplyValue = pDestFixObject.GetSP();
                                    }
                                    break;
                                case eUnitPropertyType.SPCharge:
                                    {
                                        switch (pDestFixObject.GetObjectType())
                                        {
                                            case eObjectType.Character:
                                                {
                                                    nApplyValue = pDestFixObject.GetChargePerBlock_SP();
                                                }
                                                break;
                                            default:
                                                {
                                                    nApplyValue = pDestFixObject.GetChargePerTurn_SP();
                                                }
                                                break;
                                        }
                                    }
                                    break;
                            }

                            nApplyValue = (int)(nApplyValue * (((float)pStatusEffectInfo.m_nEffectBaseRate) / 100.0f));
                        }

                        if ((pStatusEffectInfo.m_eBuffType == eBuffType.Buff && pUnit_Target.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.BuffImmune)) ||
                            pStatusEffectInfo.m_eBuffType == eBuffType.Debuff && pUnit_Target.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DeuffImmune))
                        {
                            return 0;
                        }

                        eUnitDamage eUnitDemage = eUnitDamage.Damage;

                        if (pStatusEffectInfo.m_eEffectType == eDamageEffectType.ElementDamage)
                        {
                            float fCorrelationRate = ExcelDataManager.Instance.m_pElement_Correlation.GetCorrelationRate(pStatusEffectInfo.m_eElement, pUnit_Target.GetUnitInfo().m_eElement);

                            if (fCorrelationRate == 100)
                            {
                                eUnitDemage = eUnitDamage.Damage;
                            }
                            else if (fCorrelationRate > 100)
                            {
                                eUnitDemage = eUnitDamage.Strong;
                            }
                            else if (fCorrelationRate < 100)
                            {
                                eUnitDemage = eUnitDamage.Weak;
                            }

                            fCorrelationRate -= pUnit_Target.GetAdd_ElementResist(pStatusEffectInfo.m_eElement);
                            if (fCorrelationRate < 0) fCorrelationRate = 0;

                            nApplyValue = (int)(nApplyValue * (fCorrelationRate / 100));
                        }

                        pUnit_Target.StopApplyStatusEffect(pStatusEffectInfo.m_nEffectGroupID);
                        pUnit_Target.AddApplyStatusEffect(pStatusEffectInfo, pActionInfo, nApplyValue, eOwner_Subject, pUnit_Subject, eUnitDemage);
                        pUnit_Target.OnCheckBuffDebuffMark();
                    }
                }
            }
        }

        return nApplyValue;
    }

    public static int CalculationDamageChange(SlotFixObject_Unit pUnit, int nDamage)
    {
        List<ApplyStatusEffect> list = pUnit.GetApplyStatusEffect_byEffectType(eDamageEffectType.DamageChange);

        foreach (ApplyStatusEffect pApplyStatusEffect in list)
        {
            float fRate = pApplyStatusEffect.m_pStatusEffectInfo.m_nEffectBaseRate / 100.0f;
            nDamage = (int)(nDamage * fRate);

            pUnit.OnDamageText(Helper.GetDamageChangeText(pApplyStatusEffect.m_pStatusEffectInfo.m_nEffectBaseRate));

            Vector3 vPos_Target = pUnit.GetSlot().GetPosition();
            vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

            ParticleManager.Instance.LoadParticleSystem(pApplyStatusEffect.m_pStatusEffectInfo.m_strEffectHit_Prefab, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
            Helper.OnSoundPlay(pApplyStatusEffect.m_pStatusEffectInfo.m_strEffectHit_Sound, false);
        }

        return nDamage;
    }

    public static void OnDamageReflect(SlotFixObject_Unit pUnit_Actor, SlotFixObject_Unit pUnit_Target, ExcelData_ActionInfo pActionInfo, int nDamage, ApplyStatusEffect pApplyStatusEffect)
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        if (pUnit_Actor != null && pUnit_Target != null)
        {
            int nHP = pUnit_Actor.GetHP();

            switch (pUnit_Actor.GetObjectType())
            {
                case eObjectType.Minion:
                case eObjectType.MinionBoss:
                    {
                        SlotFixObject_Minion pMinion = pUnit_Actor as SlotFixObject_Minion;
                        if (pMinion != null && pMinion.GetGameObject() != null)
                        {
                            pUnit_Actor.ChangeHP(nHP - nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage]);

                            if (pUnit_Actor.GetHP() <= 0)
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

                                pMinion.OnDead();
                                pMinion.GetSlot().SetParameta(pUnit_Target);
                            }
                        }
                    }
                    break;
                case eObjectType.EnemyBoss:
                    {
                    }
                    break;
                case eObjectType.Character:
                    {
                        SlotFixObject_PlayerCharacter pPlayerCharacter = pUnit_Actor as SlotFixObject_PlayerCharacter;

                        if (pPlayerCharacter != null)
                        {
                            pPlayerCharacter.ChangeHP(nHP - nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage]);

                            if (pPlayerCharacter.GetHP() <= 0)
                            {
                                //pDataStack.m_PlayerCharacterTable.Remove(pPlayerCharacter.GetSlot().GetSlotIndex());
                                pPlayerCharacter.OnDead();
                                pPlayerCharacter.GetSlot().SetParameta(pUnit_Target);
                            }
                        }
                    }
                    break;
            }
        }
    }

    // pUnit_Target 이 반사를 한 상황이라, pUnit_Actor 이 반사 대미지를 입음
    public static void OnDamageReflect(SlotFixObject_Unit pUnit_Actor, SlotFixObject_Unit pUnit_Target, ExcelData_ActionInfo pActionInfo, int nDamage)
    {
        if (pUnit_Actor != null && pUnit_Target != null)
        {
            List<ApplyStatusEffect> list = pUnit_Target.GetApplyStatusEffect_byEffectType(eDamageEffectType.DamageReflect);

            foreach (ApplyStatusEffect pApplyStatusEffect in list)
            {
                float fRate = pApplyStatusEffect.m_pStatusEffectInfo.m_nEffectBaseRate / 100.0f;
                int nReflectDamage = (int)(nDamage * fRate);

                if (pActionInfo != null)
                {
                    if (pApplyStatusEffect.m_pStatusEffectInfo.m_strEffectHit_Prefab != "0")
                    {
                        Vector3 vPos_Target = pUnit_Target.GetGameObject().transform.position;
                        vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                        ParticleManager.Instance.LoadParticleSystem(pApplyStatusEffect.m_pStatusEffectInfo.m_strEffectHit_Prefab, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                    }

                    if (pActionInfo.m_strEffectMissile != "0")
                    {
                        GameEvent_ReflectDamageMissile pGameEvent = new GameEvent_ReflectDamageMissile(pUnit_Actor, pUnit_Target, pActionInfo, nReflectDamage, pApplyStatusEffect, pActionInfo.m_strEffectMissile);
                        GameEventManager.Instance.AddGameEvent(pGameEvent);
                    }
                    else
                    {
                        GameEvent_ReflectDamageMissile pGameEvent = new GameEvent_ReflectDamageMissile(pUnit_Actor, pUnit_Target, pActionInfo, nReflectDamage, pApplyStatusEffect, "ActionEffect_2100004_Missile");
                        GameEventManager.Instance.AddGameEvent(pGameEvent);
                    }
                }

                pUnit_Target.OnDamageText(ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_DAMAGE_REFLECT"));
            }
        }
    }
}