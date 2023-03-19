using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_PlayerPassiveMissile : GameEvent
{
    private Slot                    m_pSrcSlot                  = null;
    private Slot                    m_pTargetSlot               = null;
    private ExcelData_ActionInfo    m_pActionInfo               = null;

    private ParticleInfo            m_pParticleInfo             = null;
    private Transformer_Vector3     m_pPos                      = null;

    private float                   m_fDamageRate               = 1;

    private ExcelData_Action_DamageTargetRangeInfo m_pDamageTargetRangeInfo   = null;
    private ExcelData_Action_LevelUpInfo m_pActionLevelUpInfo = null;

    public GameEvent_PlayerPassiveMissile(Slot pSrcSlot, Slot pTargetSlot, ExcelData_ActionInfo pActionInfo, ExcelData_Action_DamageTargetRangeInfo pDamageTargetRangeInfo, ExcelData_Action_LevelUpInfo pActionLevelUpInfo = null)
    {
        m_pSrcSlot = pSrcSlot;
        m_pTargetSlot = pTargetSlot;
        m_pActionInfo = pActionInfo;
        m_pDamageTargetRangeInfo = pDamageTargetRangeInfo;
        m_pActionLevelUpInfo = pActionLevelUpInfo;

        CreateProjectile();

        Helper.OnSoundPlay(pActionInfo.m_strEffectMissile_Sound, false);
    }

    private void CreateProjectile()
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        pDataStack.m_nPlayerCharacterPassiveProjectileCount += 1;

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
    }

    private void OnDone_Pos(TransformerEvent eventVAlue)
    {
        SlotFixObject_Unit pUnit = m_pSrcSlot.GetLastSlotFixObject() as SlotFixObject_Unit;

        switch (pUnit.GetObjectType())
        {
            case eObjectType.Character:
                {
                    SlotFixObject_PlayerCharacter pPlayerCharacter = pUnit as SlotFixObject_PlayerCharacter;

                    if (pPlayerCharacter != null)
                    {
                        if (m_pActionInfo.m_strEffectHit != "0")
                        {
                            Vector3 vPos_Target = m_pTargetSlot.GetPosition();
                            vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                            ParticleManager.Instance.LoadParticleSystem(m_pActionInfo.m_strEffectHit, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                        }

                        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

                        pDataStack.m_nPlayerCharacterPassiveProjectileCount -= 1;

                        if (pDataStack.m_nPlayerCharacterPassiveProjectileCount == 0)
                        {
                            EventDelegateManager.Instance.OnInGame_PlayerCharacterPassiveProjectile_Done();
                        }

                        m_pParticleInfo.OnStop();
                        ParticleManager.Instance.RemoveImmediateParticleInfo(m_pParticleInfo);
                        m_pParticleInfo = null;

                        if (m_pDamageTargetRangeInfo != null)
                        {
                            MainGame_Espresso_ProcessHelper.ApplyStatusEffect(m_pSrcSlot, m_pTargetSlot, m_pActionInfo, m_pDamageTargetRangeInfo, m_pActionLevelUpInfo);
                        }
                    }
                }
                break;

            case eObjectType.Minion:
                {
                    SlotFixObject_Minion pMinion = pUnit as SlotFixObject_Minion;

                    if (pMinion != null)
                    {
                        if (m_pActionInfo.m_strEffectHit != "0")
                        {
                            Vector3 vPos_Target = m_pTargetSlot.GetPosition();
                            vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                            ParticleManager.Instance.LoadParticleSystem(m_pActionInfo.m_strEffectHit, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                        }

                        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

                        pDataStack.m_nPlayerCharacterPassiveProjectileCount -= 1;

                        if (pDataStack.m_nPlayerCharacterPassiveProjectileCount == 0)
                        {
                            EventDelegateManager.Instance.OnInGame_PlayerCharacterPassiveProjectile_Done();
                        }

                        m_pParticleInfo.OnStop();
                        ParticleManager.Instance.RemoveImmediateParticleInfo(m_pParticleInfo);
                        m_pParticleInfo = null;

                        if (m_pDamageTargetRangeInfo != null)
                        {
                            MainGame_Espresso_ProcessHelper.ApplyStatusEffect(m_pSrcSlot, m_pTargetSlot, m_pActionInfo, m_pDamageTargetRangeInfo, m_pActionLevelUpInfo);
                        }
                    }
                }
                break;
        }

        OnDone();
    }
}
