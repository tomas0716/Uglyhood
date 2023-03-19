using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_BlockMatchMissile : GameEvent
{
    private Slot                    m_pSrcSlot                  = null;
    private Slot                    m_pTargetSlot               = null;
    private bool                    m_IsTargeting               = false;
    private eObjectType             m_eObjectType_Target        = eObjectType.Block;
    private eElement                m_eBlockElement             = eElement.Neutral;
    private int                     m_nCombo                    = 0;
    private bool                    m_IsDead                    = false;

    private Plane2D                 m_pPalne_Projectile         = null;
    private ParticleInfo            m_pParticleInfo_Projectile  = null;
    private Transformer_Vector2     m_pPos                      = null;

    public GameEvent_BlockMatchMissile(Slot pSrcSlot, Slot pTargetSlot, eObjectType eObjectType_Target, eElement eBlockElement, int nCombo, bool IsDead)
    {
        m_pSrcSlot = pSrcSlot;
        m_pTargetSlot = pTargetSlot;
        m_IsTargeting = true;
        m_eObjectType_Target = eObjectType_Target;
        m_eBlockElement = eBlockElement;
        m_nCombo = nCombo;
        m_IsDead = IsDead;

        CreateProjectile();

        Helper.OnSoundPlay("INGAME_BLOCK_MATCH_MISSILE_MOVE", false);
    }

    public GameEvent_BlockMatchMissile(Slot pSrcSlot, Slot pTargetSlot, eElement eBlockElement)
    {
        m_pSrcSlot = pSrcSlot;
        m_pTargetSlot = pTargetSlot;
        m_eBlockElement = eBlockElement;

        CreateProjectile();
    }

    private void CreateProjectile()
    {
        ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        pDataStack.m_nBlockProjectileCount += 1;

        Vector3 vPos_Src = m_pSrcSlot.GetPosition();
        vPos_Src.z = -(int)(ePlaneOrder.Fx_TopLayer);
        string strFileName = "FX_Match_Missile_Trail_" + m_eBlockElement.ToString() + "_" + pStageInfo.m_strStageTheme;
        m_pParticleInfo_Projectile = ParticleManager.Instance.LoadParticleSystem(strFileName, vPos_Src);
        //GameObject ob;
        //ob = Resources.Load<GameObject>("2D/Prefabs/Projectile/Projectile_Block_" + m_eBlockElement);
        //ob = GameObject.Instantiate(ob);
        //ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
        //m_pPalne_Projectile = ob.GetComponent<Plane2D>();
        Vector2 vPos_Target = m_pTargetSlot.GetPosition();
        if (m_IsTargeting == false)
        {
            if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
            {
                vPos_Target.y += InGameInfo.Instance.m_fSlotSize * 3;
            }
            else
            {
                vPos_Target.y -= InGameInfo.Instance.m_fSlotSize * 3;
            }
        }

        if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.Other)
        {
            m_pParticleInfo_Projectile.GetGameObject().transform.eulerAngles = new Vector3(0,0,180);
        }

        float fLength = Vector2.Distance(vPos_Src, vPos_Target);
        float fTime = fLength / (InGameInfo.Instance.m_fSlotSize * GameDefine.ms_fProjectile_Block_Speed);
        //m_pPalne_Projectile.SetPosition(vPos_Src);

        m_pPos = new Transformer_Vector2(vPos_Src);

        //ob = Helper.FindChildGameObject(ob, "FX_Match_Missile_Trail");
        //TrailRenderer pTrailRenderer = ob.GetComponent<TrailRenderer>();
        ////pTrailRenderer.widthCurve.

        TransformerEvent eventValue;
        eventValue = new TransformerEvent_Vector2(fTime, vPos_Target);
        m_pPos.AddEvent(eventValue);
        m_pPos.SetCallback(null, OnDone_Pos);
        m_pPos.OnPlay();
    }

    public override void OnDestroy()
    {
        //if (m_pPalne_Projectile != null && m_pPalne_Projectile.gameObject != null)
        //{
        //    GameObject.Destroy(m_pPalne_Projectile.gameObject);
        //}

        if (m_pParticleInfo_Projectile != null && m_pParticleInfo_Projectile.GetGameObject() != null)
        {
            ParticleManager.Instance.RemoveParticleInfo(m_pParticleInfo_Projectile);
        }
    }

    public override void Update()
    {
        m_pPos.Update(Time.deltaTime);
        Vector3 vPos = m_pPos.GetCurVector2();
        vPos.z = -(int)(ePlaneOrder.Fx_TopLayer);
        //m_pPalne_Projectile.SetPosition(vPos);
        m_pParticleInfo_Projectile.SetPosition(vPos);
    }

    private void OnDone_Pos(TransformerEvent eventVAlue)
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        if (m_IsTargeting == true)
        {
            switch (m_eObjectType_Target)
            {
                case eObjectType.BlockObstacle:
                    {
                        if (m_pTargetSlot.GetSlotBlock() != null)
                        {
                            Vector3 vPos = m_pTargetSlot.GetPosition();
                            vPos.z = -(float)ePlaneOrder.Character_MatchDamage_Effect;
                            ParticleManager.Instance.LoadParticleSystem("FX_BlockMatchDamage", vPos).SetScale(InGameInfo.Instance.m_fInGameScale);

                            Helper.OnSoundPlay("INGAME_BLOCK_MATCH_MISSILE_HIT", false);

                            if (m_IsDead == true)
                            {
                                m_pTargetSlot.SetSlotDying(true);
                                m_pTargetSlot.OnSlotDying();
                            }
                        }
                        
                        if (m_IsDead == true)
                        {
                            --pDataStack.m_pSlotManager.m_nTurnCompleteCheck;
                            OutputLog.Log("--pDataStack.m_pSlotManager.m_nTurnCompleteCheck > GameEvent_BlockMatchMissile : OnDone_Pos : BlockObstacle");

                            if (pDataStack.m_pSlotManager.m_nTurnCompleteCheck == 0)
                            {
                                EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0);
                            }
                        }
                    }
                    break;

                case eObjectType.EnemyColony:
                    {
                        SlotFixObject_EnemyColony pEnemyColony = m_pTargetSlot.GetLastSlotFixObject() as SlotFixObject_EnemyColony;
                        if (pEnemyColony != null)
                        {
                            int nHP = pEnemyColony.GetHP();

                            Vector3 vPos = pEnemyColony.GetSlot().GetPosition();
                            vPos.z = -(float)ePlaneOrder.Character_MatchDamage_Effect;
                            ParticleManager.Instance.LoadParticleSystem("FX_BlockMatchDamage", vPos).SetScale(InGameInfo.Instance.m_fInGameScale);

                            pEnemyColony.ChangeHP(nHP - 1);

                            Helper.OnSoundPlay("INGAME_BLOCK_MATCH_MISSILE_HIT", false);

                            if (pEnemyColony.GetHP() <= 0 && m_IsDead == true)
                            {
                                pDataStack.m_EnemyColonyTable.Remove(pEnemyColony.GetSlot().GetSlotIndex());

                                // Á×ÀÌÀÚ
                                pEnemyColony.OnDead();
                            }
                        }
                        
                        if (m_IsDead == true)
                        {
                            --pDataStack.m_pSlotManager.m_nTurnCompleteCheck;
                            OutputLog.Log("--pDataStack.m_pSlotManager.m_nTurnCompleteCheck > GameEvent_BlockMatchMissile : OnDone_Pos : EnemyColony");

                            if (pDataStack.m_pSlotManager.m_nTurnCompleteCheck == 0)
                            {
                                EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0);
                            }
                        }
                    }
                    break;

                case eObjectType.Minion:
                case eObjectType.MinionBoss:
                    {
                        SlotFixObject_Minion pMinion = m_pTargetSlot.GetLastSlotFixObject() as SlotFixObject_Minion;

                        if (pMinion != null)
                        {
                            int nHP = pMinion.GetHP();

                            Vector3 vPos = pMinion.GetSlot().GetPosition();
                            vPos.z = -(float)ePlaneOrder.Character_MatchDamage_Effect;
                            ParticleManager.Instance.LoadParticleSystem("FX_BlockMatchDamage", vPos).SetScale(InGameInfo.Instance.m_fInGameScale);

                            float fCorrelationRate = ExcelDataManager.Instance.m_pElement_Correlation.GetCorrelationRate(m_eBlockElement, pMinion.GetUnitInfo().m_eElement);

                            Color color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage];
                            if (fCorrelationRate == 100 && pDataStack.m_nPlayerCharacterAttackValue[(int)m_eBlockElement] > 0)
                            {
                                color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage];
                            }
                            else if (fCorrelationRate > 100 && pDataStack.m_nPlayerCharacterAttackValue[(int)m_eBlockElement] > 0)
                            {
                                color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Strong];
                                pMinion.OnDamageText(eUnitDamage.Strong);
                            }
                            else if (fCorrelationRate < 100 || pDataStack.m_nPlayerCharacterAttackValue[(int)m_eBlockElement] == 0)
                            {
                                color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Weak];

                                if (pDataStack.m_nPlayerCharacterAttackValue[(int)m_eBlockElement] == 0)
                                {
                                    color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Hero_Missing];
                                }

                                if (fCorrelationRate < 100 && pDataStack.m_nPlayerCharacterAttackValue[(int)m_eBlockElement] > 0)
                                {
                                    color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Weak];
                                    pMinion.OnDamageText(eUnitDamage.Weak);
                                }
                            }

                            fCorrelationRate -= pMinion.GetAdd_ElementResist(m_eBlockElement);
                            if(fCorrelationRate < 0) fCorrelationRate = 0;
                            float fMatchDamage = (pDataStack.m_nPlayerCharacterAttackValue[(int)m_eBlockElement] / 3) * (fCorrelationRate / 100);
                            int nMatchDamage = (int)Math.Round(fMatchDamage, MidpointRounding.AwayFromZero);
                            nMatchDamage = nMatchDamage > 1 ? nMatchDamage : 1;
                            nMatchDamage *= (int)(ExcelDataManager.Instance.m_pMatchDamage_ComboBonus.GetBonusRate(m_nCombo) / 100);

                            if (pMinion.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageChange) == true)
                            {
                                nMatchDamage = MainGame_Espresso_ProcessHelper.CalculationDamageChange(pMinion, nMatchDamage);
                            }

                            if (EspressoInfo.Instance.m_IsSuddenDeathOn_PvP == true)
                            {
                                nMatchDamage *= 3;
                            }

                            pMinion.ChangeHP(nHP - nMatchDamage, color);

                            Helper.OnSoundPlay("INGAME_BLOCK_MATCH_MISSILE_HIT", false);

                            if (pMinion.GetHP() <= 0 && m_IsDead == true)
                            {
                                switch (pMinion.GetMinionType())
                                {
                                    case eMinionType.EnemyMinion:
                                        {
                                            pDataStack.m_EnemyMinionTable.Remove(pMinion.GetSlot().GetSlotIndex());
                                        }
                                        break;

                                    case eMinionType.EnemySummonUnit:
                                        {
                                            pDataStack.m_EnemySummonUnitTable.Remove(pMinion.GetSlot().GetSlotIndex());
                                        }
                                        break;

                                    case eMinionType.PlayerSummonUnit:
                                        {
                                            pDataStack.m_PlayerSummonUnitTable.Remove(pMinion.GetSlot().GetSlotIndex());
                                        }
                                        break;
                                }

                                // Á×ÀÌÀÚ
                                pMinion.OnDead();
                            }
                        }

                        if (m_IsDead == true)
                        {
                            --pDataStack.m_pSlotManager.m_nTurnCompleteCheck;
                            OutputLog.Log("--pDataStack.m_pSlotManager.m_nTurnCompleteCheck > GameEvent_BlockMatchMissile : OnDone_Pos : Minion");

                            if (pDataStack.m_pSlotManager.m_nTurnCompleteCheck == 0)
                            {
                                EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0);
                            }
                        }
                    }
                    break;

                case eObjectType.Character:
                    {
                        SlotFixObject_PlayerCharacter pPlayerCharacter = m_pTargetSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                        if (pPlayerCharacter != null)
                        {
                            int nHP = pPlayerCharacter.GetHP();

                            Vector3 vPos = pPlayerCharacter.GetSlot().GetPosition();
                            vPos.z = -(float)ePlaneOrder.Character_MatchDamage_Effect;
                            ParticleManager.Instance.LoadParticleSystem("FX_BlockMatchDamage", vPos).SetScale(InGameInfo.Instance.m_fInGameScale);

                            float fCorrelationRate = ExcelDataManager.Instance.m_pElement_Correlation.GetCorrelationRate(m_eBlockElement, pPlayerCharacter.GetUnitInfo().m_eElement);

                            Color color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage];
                            if (fCorrelationRate == 100 && pDataStack.m_nPlayerCharacterAttackValue[(int)m_eBlockElement] > 0)
                            {
                                color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage];
                            }
                            else if (fCorrelationRate > 100 && pDataStack.m_nPlayerCharacterAttackValue[(int)m_eBlockElement] > 0)
                            {
                                color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Strong];
                                pPlayerCharacter.OnDamageText(eUnitDamage.Strong);
                            }
                            else if (fCorrelationRate < 100 || pDataStack.m_nPlayerCharacterAttackValue[(int)m_eBlockElement] == 0)
                            {
                                color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Weak];

                                if (pDataStack.m_nPlayerCharacterAttackValue[(int)m_eBlockElement] == 0)
                                {
                                    color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Hero_Missing];
                                }

                                if (fCorrelationRate < 100 && pDataStack.m_nPlayerCharacterAttackValue[(int)m_eBlockElement] > 0)
                                {
                                    color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Weak];
                                    pPlayerCharacter.OnDamageText(eUnitDamage.Weak);
                                }
                            }

                            fCorrelationRate -= pPlayerCharacter.GetAdd_ElementResist(m_eBlockElement);
                            if (fCorrelationRate < 0) fCorrelationRate = 0;
                            float fMatchDamage = (pDataStack.m_nPlayerCharacterAttackValue[(int)m_eBlockElement] / 3) * (fCorrelationRate / 100);
                            int nMatchDamage = (int)Math.Round(fMatchDamage, MidpointRounding.AwayFromZero);
                            nMatchDamage = nMatchDamage > 1 ? nMatchDamage : 1;
                            nMatchDamage *= (int)(ExcelDataManager.Instance.m_pMatchDamage_ComboBonus.GetBonusRate(m_nCombo) / 100);

                            if (pPlayerCharacter.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.DamageChange) == true)
                            {
                                nMatchDamage = MainGame_Espresso_ProcessHelper.CalculationDamageChange(pPlayerCharacter, nMatchDamage);
                            }

                            if (EspressoInfo.Instance.m_IsSuddenDeathOn_PvP == true)
                            {
                                nMatchDamage *= 3;
                            }

                            pPlayerCharacter.ChangeHP(nHP - nMatchDamage, color);

                            Helper.OnSoundPlay("INGAME_BLOCK_MATCH_MISSILE_HIT", false);

                            if (pPlayerCharacter.GetHP() <= 0 && m_IsDead == true)
                            {
                                // Á×ÀÌÀÚ
                                pPlayerCharacter.OnDead();
                            }
                        }

                        if (m_IsDead == true)
                        {
                            --pDataStack.m_pSlotManager.m_nTurnCompleteCheck;
                            OutputLog.Log("--pDataStack.m_pSlotManager.m_nTurnCompleteCheck > GameEvent_BlockMatchMissile : OnDone_Pos : Minion");

                            if (pDataStack.m_pSlotManager.m_nTurnCompleteCheck == 0)
                            {
                                EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0);
                            }
                        }
                    }
                    break;

                case eObjectType.SlotObstacle:
                    {
                        SlotFixObject_Obstacle pObstacle = m_pTargetSlot.GetLastSlotFixObject() as SlotFixObject_Obstacle;
                        if (pObstacle != null)
                        {
                            Vector3 vPos = pObstacle.GetSlot().GetPosition();
                            vPos.z = -(float)ePlaneOrder.Character_MatchDamage_Effect;
                            ParticleManager.Instance.LoadParticleSystem("FX_BlockMatchDamage", vPos).SetScale(InGameInfo.Instance.m_fInGameScale);

                            Helper.OnSoundPlay("INGAME_BLOCK_MATCH_MISSILE_HIT", false);

                            if (m_IsDead == true)
                            {
                                pObstacle.GetSlot().SetSlotDying(true);
                                pObstacle.GetSlot().OnSlotDying();
                            }
                        }

                        if (m_IsDead == true)
                        {
                            --pDataStack.m_pSlotManager.m_nTurnCompleteCheck;
                            OutputLog.Log("--pDataStack.m_pSlotManager.m_nTurnCompleteCheck > GameEvent_BlockMatchMissile : OnDone_Pos : SlotObstacle");

                            if (pDataStack.m_pSlotManager.m_nTurnCompleteCheck == 0)
                            {
                                EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0);
                            }
                        }
                    }
                    break;
			}
            
        }

        pDataStack.m_nBlockProjectileCount -= 1;

        if (pDataStack.m_nBlockProjectileCount == 0)
        {
            EventDelegateManager.Instance.OnInGame_Projectile_Block_Done();
        }

        OnDone();
    }
}
