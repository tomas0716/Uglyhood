using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_MinionAttackMissile : GameEvent
{
    private Slot                    m_pSrcSlot                  = null;
    private Slot                    m_pTargetSlot               = null;
    private ExcelData_ActionInfo    m_pActionInfo               = null;
    private int                     m_nDamage                   = 0;
    private eDamageEffectType       m_eDamageEffectType         = eDamageEffectType.ElementDamage;

    private ParticleInfo            m_pParticleInfo             = null;
    private Transformer_Vector3     m_pPos                      = null;

    private bool                    m_IsSelectTarget            = false;

    private Transformer_Timer       m_pTimer_Attack_Delay       = new Transformer_Timer();

    private ExcelData_Action_DamageTargetRangeInfo m_pDamageTargetRangeInfo = null;

    public GameEvent_MinionAttackMissile(Slot pSrcSlot, Slot pTargetSlot, ExcelData_ActionInfo pActionInfo, eDamageEffectType eDamageType, int nDamage, bool IsSelectTarget, ExcelData_Action_DamageTargetRangeInfo pDamageTargetRangeInfo = null)
    {
        m_pSrcSlot = pSrcSlot;
        m_pTargetSlot = pTargetSlot;
        m_pActionInfo = pActionInfo;
        m_eDamageEffectType = eDamageType;
        m_nDamage = nDamage;
        m_IsSelectTarget = IsSelectTarget;
        m_pDamageTargetRangeInfo = pDamageTargetRangeInfo;

        CreateProjectile();

        Helper.OnSoundPlay(pActionInfo.m_strEffectMissile_Sound, false);
    }

    private void CreateProjectile()
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        pDataStack.m_nEnemyMinionProjectileCount += 1;

        Vector3 vPos_Src = m_pSrcSlot.GetPosition();
        vPos_Src.z = -(float)ePlaneOrder.Fx_TopLayer;
        Vector3 vPos_Target = m_pTargetSlot.GetPosition();
        vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

        m_pParticleInfo = ParticleManager.Instance.LoadParticleSystem(m_pActionInfo.m_strEffectMissile, vPos_Src);
        m_pParticleInfo.SetScale(InGameInfo.Instance.m_fInGameScale);

        Quaternion Quat = m_pParticleInfo.GetGameObject().transform.rotation;
        Quaternion Rot = Quaternion.FromToRotation(vPos_Src, vPos_Target);
        m_pParticleInfo.GetGameObject().transform.rotation = Quat * Rot;

        float fLength = Vector2.Distance(vPos_Src, vPos_Target);
        float fTime = fLength / (InGameInfo.Instance.m_fSlotSize * m_pActionInfo.m_fEffectMissileVelocity);

        m_pPos = new Transformer_Vector3(vPos_Src);

        TransformerEvent eventValue;
        eventValue = new TransformerEvent_Vector3(fTime, vPos_Target);
        m_pPos.AddEvent(eventValue);
        m_pPos.SetCallback(null, OnDone_Pos);
        m_pPos.OnPlay();
    }

    public override void OnDestroy()
    {
        if (m_pParticleInfo != null)
        {
            ParticleManager.Instance.RemoveParticleInfo(m_pParticleInfo);
        }
    }

    public override void Update()
    {
        m_pPos.Update(Time.deltaTime);
        Vector3 vPos = m_pPos.GetCurVector3();

        if(m_pParticleInfo != null)
            m_pParticleInfo.SetPosition(vPos);

        m_pTimer_Attack_Delay.Update(Time.deltaTime);
    }

    private void OnDone_Pos(TransformerEvent eventVAlue)
    {
        Helper.OnSoundPlay(m_pActionInfo.m_strEffectHit_Sound, false);

        float fDelayTime = 0;
        if (m_pActionInfo.m_strEffectHit_Target != "0" || m_pActionInfo.m_strEffectHit_Center != "0")
        {
            Vector3 vPos_Target = m_pTargetSlot.GetPosition();
            vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

            if (m_pActionInfo.m_strEffectHit_Target != "0" &&
                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Contains(m_pActionInfo.m_strEffectHit_Target) == false)
            {
                ParticleManager.Instance.LoadParticleSystem(m_pActionInfo.m_strEffectHit_Target, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Add(m_pActionInfo.m_strEffectHit_Target);
            }

            if (m_pActionInfo.m_strEffectHit_Center != "0" &&
                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Contains(m_pActionInfo.m_strEffectHit_Center) == false)
            {
                ParticleManager.Instance.LoadParticleSystem(m_pActionInfo.m_strEffectHit_Center, InGameInfo.Instance.m_vSlotGridCenter).SetScale(InGameInfo.Instance.m_fInGameScale);
                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Add(m_pActionInfo.m_strEffectHit_Center);
            }

            fDelayTime = m_pActionInfo.m_fEffectHit_Target_Delay;
        }

        m_pTimer_Attack_Delay.OnReset();
        TransformerEvent eventValue;
        eventValue = new TransformerEvent_Timer(fDelayTime);
        m_pTimer_Attack_Delay.AddEvent(eventValue);
        m_pTimer_Attack_Delay.SetCallback(null, OnDone_Timer_Attack_Delay);
        m_pTimer_Attack_Delay.OnPlay();
    }

    public void OnDone_Timer_Attack_Delay(TransformerEvent eventValue)
    {
        if (m_pActionInfo.m_strEffectHit != "0")
        {
            Vector3 vPos_Target = m_pTargetSlot.GetPosition();
            vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

            ParticleManager.Instance.LoadParticleSystem(m_pActionInfo.m_strEffectHit, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
        }

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        SlotFixObject_Unit pUnit = m_pSrcSlot.GetLastSlotFixObject() as SlotFixObject_Unit;

        if (m_pTargetSlot.GetSlotBlock() != null)
        {
            switch (m_eDamageEffectType)
            {
                case eDamageEffectType.BlockItemChange:
                    {
                        if (m_pActionInfo.m_eDamageChangeElement != eElement.Neutral)
                        {
                            m_pTargetSlot.ChangeSlotBlock((eBlockType)m_pActionInfo.m_eDamageChangeElement, eSpecialItem.None);
                        }

                        switch (m_pActionInfo.m_eDamageChangeType)
                        {
                            case eDamageChangeType.Bomb:
                                {
                                    m_pTargetSlot.ChangeSlotBlock(eSpecialItem.Square3);
                                }
                                break;
                            case eDamageChangeType.Elixir:
                                {
                                    m_pTargetSlot.ChangeSlotBlock(eSpecialItem.Match_Color);
                                }
                                break;
                        }
                    }
                    break;

                case eDamageEffectType.AddObstacle:
                    {
                        switch (m_pActionInfo.m_eDamageAddObstacleType)
                        {
                            case eDamageAddObstacleType.Chain:
                                {
                                    m_pTargetSlot.ChangeSlotBlock(eMapSlotItem.Chain, m_pActionInfo.m_nDamageAddObstacleLevel);
                                }
                                break;

                            case eDamageAddObstacleType.Ice:
                                {
                                    m_pTargetSlot.ChangeSlotBlock(eMapSlotItem.Ice, m_pActionInfo.m_nDamageAddObstacleLevel);
                                }
                                break;
                        }
                    }
                    break;
                case eDamageEffectType.UnitSummon:
                    {
                        m_pTargetSlot.OnSlotBlockRemove();

                        ExcelData_EnemyColonyInfo pEnemyColonyInfo = ExcelDataManager.Instance.m_pEnemyColony.GetEnemyColonyInfo(m_pActionInfo.m_nEnemyColonyID);
                        if (pEnemyColonyInfo == null)
                        {
                            pEnemyColonyInfo = ExcelDataManager.Instance.m_pEnemyColony.GetEnemyColonyInfo(GameDefine.ms_nBasicEnemyColonyID);
                        }

                        SlotFixObject_EnemyColony pSlotFixObject_EnemyColony = new SlotFixObject_EnemyColony(m_pTargetSlot, m_pTargetSlot.GetSlotIndex(), pEnemyColonyInfo, 1);
                        m_pTargetSlot.AddSlotFixObject(pSlotFixObject_EnemyColony);
                        pDataStack.m_EnemyColonyTable.Add(m_pTargetSlot.GetSlotIndex(), pSlotFixObject_EnemyColony);

                        int nLevel = m_pActionInfo.m_nTransformUnitLevel;
                        if (nLevel == 0)
                        {
                            nLevel = pUnit.GetLevel();
                        }

                        ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(m_pActionInfo.m_nTransformUnitID);
                        eObjectType eObType = eObjectType.Minion;
                        if(pUnitInfo.m_eUnitType == eUnitType.BossMinion)
                            eObType = eObjectType.MinionBoss;
                        
                        SlotFixObject_Minion pSlotFixObject_Minion = new SlotFixObject_Minion(m_pTargetSlot, m_pActionInfo.m_nTransformUnitID, nLevel, eMinionType.EnemySummonUnit, eObType, pUnit.GetOwner(), false);
                        m_pTargetSlot.AddSlotFixObject(pSlotFixObject_Minion);
                        pDataStack.m_EnemySummonUnitTable.Add(m_pTargetSlot.GetSlotIndex(), pSlotFixObject_Minion);

                        pSlotFixObject_Minion.SetDamageEffectType_TransformType(eDamageEffectType.UnitSummon, m_pActionInfo.m_nTransformUnitLifeTurn, m_pActionInfo.m_strEffectHit);
                    }
                    break;
                case eDamageEffectType.Blind:
                    {
                        InGame_Cloud pApplyCloud = pDataStack.m_pCloudManager.GetCloud(m_pTargetSlot);

                        if (pApplyCloud == null)
                        {
                            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_SlotFix/Cloud_" + pUnit.GetOwner());
                            ob = GameObject.Instantiate(ob);
                            InGame_Cloud pCloud = ob.AddComponent<InGame_Cloud>();
                            pCloud.Init(m_pTargetSlot, pUnit.GetOwner(), m_pActionInfo.m_nTransformUnitLifeTurn);
                            ob.transform.position = m_pTargetSlot.GetPosition();
                            ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                            Plane2D pPlane = ob.GetComponent<Plane2D>();
                            pPlane.SetDepthPlus(m_pTargetSlot.GetSlotIndex());

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
                                pCloud.Init(m_pTargetSlot, pUnit.GetOwner(), m_pActionInfo.m_nTransformUnitLifeTurn);
                                ob.transform.position = m_pTargetSlot.GetPosition();
                                ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                                Plane2D pPlane = ob.GetComponent<Plane2D>();
                                pPlane.SetDepthPlus(m_pTargetSlot.GetSlotIndex());

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
                        m_pTargetSlot.OnSlotBlockRemove();
                        m_pTargetSlot.SetParameta(pUnit);

                        if (pDataStack.m_EnemyColonyCreateTable.ContainsKey(m_pTargetSlot) == false)
                        {
                            pDataStack.m_EnemyColonyCreateTable.Add(m_pTargetSlot, m_pActionInfo.m_nEnemyColonyID);
                        }
                    }
                    break;
            }
        }
        else
        {
            SlotFixObject_Espresso pSlotFixObject_Espresso = m_pTargetSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

            if (pSlotFixObject_Espresso != null)
            {
                switch (m_eDamageEffectType)
                {
                    case eDamageEffectType.ElementDamage:
                    case eDamageEffectType.Damage:
                        {
                            int nHP = pSlotFixObject_Espresso.GetHP();

                            switch (pSlotFixObject_Espresso.GetObjectType())
                            {
                                case eObjectType.Character:
                                case eObjectType.Minion:
                                case eObjectType.MinionBoss:
                                case eObjectType.EnemyBoss:
                                    {
                                        SlotFixObject_Unit pUnit_Target = pSlotFixObject_Espresso as SlotFixObject_Unit;

                                        if (pUnit_Target != null)
                                        {
                                            float fCorrelationRate = ExcelDataManager.Instance.m_pElement_Correlation.GetCorrelationRate(pUnit.GetUnitInfo().m_eElement, pUnit_Target.GetUnitInfo().m_eElement);

                                            Color color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage];
                                            if (m_pActionInfo.m_eDamageType == eDamageEffectType.Damage || fCorrelationRate == 100)
                                            {
                                                color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage];
                                            }
                                            else if (m_pActionInfo.m_eDamageType != eDamageEffectType.Damage && fCorrelationRate > 100)
                                            {
                                                color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Strong];
                                                pSlotFixObject_Espresso.OnDamageText(eUnitDamage.Strong);
                                            }
                                            else if (m_pActionInfo.m_eDamageType != eDamageEffectType.Damage && fCorrelationRate < 100)
                                            {
                                                color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Weak];
                                                pSlotFixObject_Espresso.OnDamageText(eUnitDamage.Weak);
                                            }

                                            if (pUnit_Target.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageChange) == true)
                                            {
                                                m_nDamage = MainGame_Espresso_ProcessHelper.CalculationDamageChange(pUnit_Target, m_nDamage);
                                            }

                                            if (pUnit_Target.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageReflect) == true)
                                            {
                                                MainGame_Espresso_ProcessHelper.OnDamageReflect(pUnit, pUnit_Target, m_pActionInfo, m_nDamage);
                                            }

                                            pSlotFixObject_Espresso.ChangeHP(nHP - m_nDamage, color);
                                        }
                                        else
                                        {
                                            pSlotFixObject_Espresso.ChangeHP(nHP - m_nDamage);
                                        }
                                    }
                                    break;

                                default:
                                    {
                                        pSlotFixObject_Espresso.ChangeHP(nHP - m_nDamage);
                                    }
                                    break;
                            }

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
                                }

                                pSlotFixObject_Espresso.OnDead();
                                m_pTargetSlot.SetParameta(pUnit);
                            }
                        }
                        break;
                    case eDamageEffectType.Heal:
                        {
                            int nHP = pSlotFixObject_Espresso.GetHP();
                            pSlotFixObject_Espresso.ChangeHP(nHP + m_nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.HP_Recovery]);
                            pSlotFixObject_Espresso.OnDamageText(eUnitDamage.HP_Recovery);
                        }
                        break;

                    case eDamageEffectType.SPCharge:
                        {
                            SlotFixObject_Unit pUnit_Target = pSlotFixObject_Espresso as SlotFixObject_Unit;
                            if (pUnit_Target != null)
                            {
                                int nSP = pUnit_Target.GetSP();
                                if (m_nDamage > 0)
                                {
                                    pUnit_Target.ChangeSP(nSP + m_nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.SP_Recharge]);
                                    pSlotFixObject_Espresso.OnDamageText(eUnitDamage.SP_Recharge);
                                }
                                else if(m_nDamage < 0)
                                {
                                    pUnit_Target.ChangeSP(nSP + m_nDamage, GameDefine.ms_UnitDamageColors[(int)eUnitDamage.SP_Decrease]);
                                    pSlotFixObject_Espresso.OnDamageText(eUnitDamage.SP_Decrease);
                                }
                            }
                        }
                        break;
                    case eDamageEffectType.UnitSummon:
                        {
                            if (pSlotFixObject_Espresso.GetObjectType() == eObjectType.EnemyColony)
                            {
                                if (pDataStack.m_EnemySummonUnitTable.ContainsKey(m_pTargetSlot.GetSlotIndex()) == false)
                                {
                                    int nLevel = m_pActionInfo.m_nTransformUnitLevel;
                                    if (nLevel == 0)
                                    {
                                        nLevel = pUnit.GetLevel();
                                    }

                                    ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(m_pActionInfo.m_nTransformUnitID);
                                    eObjectType eObType = eObjectType.Minion;
                                    if (pUnitInfo.m_eUnitType == eUnitType.BossMinion)
                                        eObType = eObjectType.MinionBoss;

                                    SlotFixObject_Minion pSlotFixObject_Minion = new SlotFixObject_Minion(m_pTargetSlot, m_pActionInfo.m_nTransformUnitID, nLevel, eMinionType.EnemySummonUnit, eObType, pUnit.GetOwner(), false);
                                    m_pTargetSlot.AddSlotFixObject(pSlotFixObject_Minion);
                                    pDataStack.m_EnemySummonUnitTable.Add(m_pTargetSlot.GetSlotIndex(), pSlotFixObject_Minion);

                                    pSlotFixObject_Minion.SetDamageEffectType_TransformType(eDamageEffectType.UnitSummon, m_pActionInfo.m_nTransformUnitLifeTurn, m_pActionInfo.m_strEffectHit);
                                }
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

                                            pUnit_Target.OnTransformUnit(m_pActionInfo.m_nTransformUnitID, pActionLevelUpInfo.m_nChange_TransformUnitLevel, pActionLevelUpInfo.m_nChange_TransformUnitLevel);
                                            //m_pActionInfo.m_nTransformUnitLevel, m_pActionInfo.m_nTransformUnitLevel);
                                            pUnit_Target.SetDamageEffectType_TransformType(eDamageEffectType.UnitTransform, pActionLevelUpInfo.m_nChange_TransformUnitLifeTurn, m_pActionInfo.m_strEffectHit);
                                        }
                                    }
                                    break;

                                case eObjectType.Minion:
                                case eObjectType.MinionBoss:
                                    {
                                        SlotFixObject_Unit pUnit_Target = pSlotFixObject_Espresso as SlotFixObject_Unit;

                                        if (pUnit_Target != null)
                                        {
                                            pUnit_Target.OnTransformUnit(m_pActionInfo.m_nTransformUnitID, m_pActionInfo.m_nTransformUnitLevel, m_pActionInfo.m_nTransformUnitLevel);
                                            pUnit_Target.SetDamageEffectType_TransformType(eDamageEffectType.UnitTransform, m_pActionInfo.m_nTransformUnitLifeTurn, m_pActionInfo.m_strEffectHit);
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
                                            pUnit.SetDamageEffectType_TransformType(eDamageEffectType.UnitClone, m_pActionInfo.m_nTransformUnitLifeTurn, m_pActionInfo.m_strEffectHit);
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
                                            pUnit.SetDamageEffectType_TransformType(eDamageEffectType.UnitClone, m_pActionInfo.m_nTransformUnitLifeTurn, m_pActionInfo.m_strEffectHit);
                                        }
                                    }
                                    break;
							}
                        }
                        break;
                    case eDamageEffectType.Blind:
                        {
                            InGame_Cloud pApplyCloud = pDataStack.m_pCloudManager.GetCloud(m_pTargetSlot);

                            if (pApplyCloud == null)
                            {
                                GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_SlotFix/Cloud_" + pUnit.GetOwner());
                                ob = GameObject.Instantiate(ob);
                                InGame_Cloud pCloud = ob.AddComponent<InGame_Cloud>();
                                pCloud.Init(m_pTargetSlot, pUnit.GetOwner(), m_pActionInfo.m_nTransformUnitLifeTurn);
                                ob.transform.position = m_pTargetSlot.GetPosition();
                                ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                                Plane2D pPlane = ob.GetComponent<Plane2D>();
                                pPlane.SetDepthPlus(m_pTargetSlot.GetSlotIndex());

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
                                    pCloud.Init(m_pTargetSlot, pUnit.GetOwner(), m_pActionInfo.m_nTransformUnitLifeTurn);
                                    ob.transform.position = m_pTargetSlot.GetPosition();
                                    ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

                                    Plane2D pPlane = ob.GetComponent<Plane2D>();
                                    pPlane.SetDepthPlus(m_pTargetSlot.GetSlotIndex());

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
        }

        pDataStack.m_nEnemyMinionProjectileCount -= 1;

        if (pDataStack.m_nEnemyMinionProjectileCount == 0 && pDataStack.m_nReflectDamageProjectileCount == 0)
        {
            EventDelegateManager.Instance.OnInGame_EnemyMinionProjectile_Done();
        }

        m_pParticleInfo.OnStop();
        ParticleManager.Instance.RemoveImmediateParticleInfo(m_pParticleInfo);
        m_pParticleInfo = null;

        if (m_pDamageTargetRangeInfo != null)
        {
            MainGame_Espresso_ProcessHelper.ApplyStatusEffect(m_pSrcSlot, m_pTargetSlot, m_pActionInfo, m_pDamageTargetRangeInfo);
        }

        OnDone();
    }
}
