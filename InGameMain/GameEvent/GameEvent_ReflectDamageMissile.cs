using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_ReflectDamageMissile : GameEvent
{
    private SlotFixObject_Unit      m_pUnit_Actor               = null;
    private SlotFixObject_Unit      m_pUnit_Target              = null;     // 반사 시키는 주체
    private ExcelData_ActionInfo    m_pActionInfo               = null;

    private ParticleInfo            m_pParticleInfo             = null;
    private Transformer_Vector3     m_pPos                      = null;

    private int                     m_nDamage                   = 0;
    private ApplyStatusEffect       m_pApplyStatusEffect        = null;

    public GameEvent_ReflectDamageMissile(SlotFixObject_Unit pUnit_Actor, SlotFixObject_Unit pUnit_Target, ExcelData_ActionInfo pActionInfo, int nDamage, 
                                          ApplyStatusEffect pApplyStatusEffect, string strEffectMissile)
    {
        m_pUnit_Actor = pUnit_Actor;
        m_pUnit_Target = pUnit_Target;
        m_pActionInfo = pActionInfo;
        m_nDamage = nDamage;
        m_pApplyStatusEffect = pApplyStatusEffect;

        CreateProjectile(strEffectMissile);

        Helper.OnSoundPlay(pActionInfo.m_strEffectMissile_Sound, false);
    }

    private void CreateProjectile(string strEffectMissile)
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        pDataStack.m_nReflectDamageProjectileCount += 1;

        Vector3 vPos_Src = m_pUnit_Target.GetSlot().GetPosition();
        vPos_Src.z = -(float)ePlaneOrder.Fx_TopLayer;
        Vector3 vPos_Target = m_pUnit_Actor.GetSlot().GetPosition();
        vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

        m_pParticleInfo = ParticleManager.Instance.LoadParticleSystem(strEffectMissile, vPos_Src);
        m_pParticleInfo.SetScale(InGameInfo.Instance.m_fInGameScale);

        Quaternion Quat = m_pParticleInfo.GetGameObject().transform.rotation;
        Quaternion Rot = Quaternion.FromToRotation(vPos_Src, vPos_Target);
        m_pParticleInfo.GetGameObject().transform.rotation = Quat * Rot;

        float fVelocity = m_pActionInfo.m_fEffectMissileVelocity;
        if(fVelocity == 0) fVelocity = 12;
        float fLength = Vector2.Distance(vPos_Src, vPos_Target);
        float fTime = fLength / (InGameInfo.Instance.m_fSlotSize * fVelocity);

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
        Helper.OnSoundPlay(m_pActionInfo.m_strEffectHit_Sound, false);

        Vector3 vPos_Target = m_pUnit_Actor.GetSlot().GetPosition();
        vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

        if (m_pActionInfo.m_strEffectHit != "0")
        {
            ParticleManager.Instance.LoadParticleSystem(m_pActionInfo.m_strEffectHit, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
        }
        else
        {
            ParticleManager.Instance.LoadParticleSystem("ActionEffect_2100004_Hit", vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
        }

        MainGame_Espresso_ProcessHelper.OnDamageReflect(m_pUnit_Actor, m_pUnit_Target, m_pActionInfo, m_nDamage, m_pApplyStatusEffect);

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        pDataStack.m_nReflectDamageProjectileCount -= 1;

        if (pDataStack.m_nPlayerCharacterProjectileCount == 0 && pDataStack.m_nEnemyMinionProjectileCount == 0 && pDataStack.m_nReflectDamageProjectileCount == 0)
        {
            switch (m_pUnit_Actor.GetObjectType())
            {
                case eObjectType.Minion:
                case eObjectType.MinionBoss:
                    {
                        switch (m_pUnit_Actor.GetOwner())
                        {
                            case eOwner.My:
                                {
                                }
                                break;

                            case eOwner.Other:
                                {
                                    EventDelegateManager.Instance.OnInGame_EnemyMinionProjectile_Done();
                                }
                                break;
                        }
                    }
                    break;

                case eObjectType.EnemyBoss:
                    {
                    }
                    break;

                case eObjectType.Character:
                    {
                        EventDelegateManager.Instance.OnInGame_PlayerCharacterProjectile_Done(m_pActionInfo);
                    }
                    break;
            }
            //EventDelegateManager.Instance.OnInGame_PlayerCharacterProjectile_Done();
        }

        m_pParticleInfo.OnStop();
        ParticleManager.Instance.RemoveImmediateParticleInfo(m_pParticleInfo);
        m_pParticleInfo = null;

        if (pDataStack.m_nReflectDamageProjectileCount == 0)
        {
            EventDelegateManager.Instance.OnInGame_ReflectDamageProjectile_Done();
        }

        OnDone();
    }
}
