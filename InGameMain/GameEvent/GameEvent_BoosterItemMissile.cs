using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_BoosterItemAttackMissile : GameEvent
{
    private Vector3                 m_vStartPos                 = Vector3.zero;
    private Slot                    m_pTargetSlot               = null;
    private ExcelData_ActionInfo    m_pActionInfo               = null;

    private ParticleInfo            m_pParticleInfo             = null;
    private Transformer_Vector3     m_pPos                      = null;

    private float                   m_fDamageRate               = 1;
    private bool                    m_IsSelectTarget            = false;

    private DamageTargetInfo        m_pDamageTargetInfo         = null;

    private Transformer_Timer       m_pTimer_Attack_Delay       = new Transformer_Timer();

    public GameEvent_BoosterItemAttackMissile(Vector3 vStartPos, Slot pTargetSlot, ExcelData_ActionInfo pActionInfo, float fDamageRate, bool IsSelectTarget, DamageTargetInfo pDamageTargetInfo)
    {
        m_vStartPos = vStartPos;
        m_pTargetSlot = pTargetSlot;
        m_pActionInfo = pActionInfo;
        m_fDamageRate = fDamageRate;
        m_IsSelectTarget = IsSelectTarget;
        m_pDamageTargetInfo = pDamageTargetInfo;

        CreateProjectile();

        Helper.OnSoundPlay(pActionInfo.m_strEffectMissile_Sound, false);
    }

    private void CreateProjectile()
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        pDataStack.m_nBoosterItemProjectileCount += 1;

        m_vStartPos.z = -(float)ePlaneOrder.Fx_TopLayer;
        Vector3 vPos_Target = m_pTargetSlot.GetPosition();
        vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

        m_pParticleInfo = ParticleManager.Instance.LoadParticleSystem(m_pActionInfo.m_strEffectMissile, m_vStartPos);
        m_pParticleInfo.SetScale(InGameInfo.Instance.m_fInGameScale);

        Quaternion Quat = m_pParticleInfo.GetGameObject().transform.rotation;
        Quaternion Rot = Quaternion.FromToRotation(m_vStartPos, vPos_Target);
        m_pParticleInfo.GetGameObject().transform.rotation = Quat * Rot;

        float fLength = Vector2.Distance(m_vStartPos, vPos_Target);
        float fTime = fLength / (InGameInfo.Instance.m_fSlotSize * m_pActionInfo.m_fEffectMissileVelocity);

        m_pPos = new Transformer_Vector3(m_vStartPos);

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
        if ((m_pActionInfo.m_strEffectHit_Target != "0" || m_pActionInfo.m_strEffectHit_Center != "0") && m_IsSelectTarget == true)
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

        MainGame_Espresso_ProcessHelper.OnSkillActionDamage(null, m_pTargetSlot, m_pActionInfo, m_pActionInfo.m_eDamageType, m_pActionInfo.m_eDamageBaseUnitRole, m_pActionInfo.m_eDamageBaseUnitProperty,
                                                                   m_pActionInfo.m_nDamageBaseRate, m_fDamageRate, m_pDamageTargetInfo.m_pConditionInfo.m_fRelativeDamageRate);

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        pDataStack.m_nBoosterItemProjectileCount -= 1;

        if (pDataStack.m_nBoosterItemProjectileCount == 0)
        {
            EventDelegateManager.Instance.OnInGame_BoosterItemProjectile_Done(m_pActionInfo);
        }

        m_pParticleInfo.OnStop();
        ParticleManager.Instance.RemoveImmediateParticleInfo(m_pParticleInfo);
        m_pParticleInfo = null;

        if (m_pDamageTargetInfo.m_pRangeInfo != null)
        {
            MainGame_Espresso_ProcessHelper.ApplyStatusEffect(null, m_pTargetSlot, m_pActionInfo, m_pDamageTargetInfo.m_pRangeInfo);
        }

        OnDone();
    }
}
