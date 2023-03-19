using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_PlayerCharacterSubSkillMissile : GameEvent
{
    private Slot                    m_pSrcSlot                  = null;
    private Slot                    m_pTargetSlot               = null;
    private ExcelData_ActionInfo    m_pActionInfo               = null;

    private ParticleInfo            m_pParticleInfo             = null;
    private Transformer_Vector3     m_pPos                      = null;

    private float                   m_fDamageRate               = 1;
    private bool                    m_IsSelectTarget            = false;

    private Transformer_Timer       m_pTimer_Attack_Delay       = new Transformer_Timer();

    private DamageTargetInfo        m_pDamageTargetInfo         = null;

    public GameEvent_PlayerCharacterSubSkillMissile(Slot pSrcSlot, Slot pTargetSlot, ExcelData_ActionInfo pActionInfo, float fDamageRate, bool IsSelectTarget, DamageTargetInfo pDamageTargetInfo = null)
    {
        m_pSrcSlot = pSrcSlot;
        m_pTargetSlot = pTargetSlot;
        m_pActionInfo = pActionInfo;
        m_fDamageRate = fDamageRate;
        m_pDamageTargetInfo = pDamageTargetInfo;
        m_IsSelectTarget = IsSelectTarget;

        CreateProjectile();

        Helper.OnSoundPlay(pActionInfo.m_strEffectMissile_Sound, false);
    }

    private void CreateProjectile()
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        pDataStack.m_nPlayerCharacterProjectileCount += 1;

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

        string strEffectHit_Target = m_pActionInfo.m_strEffectHit_Target;

        SlotFixObject_PlayerCharacter pPlayerCharacter = m_pSrcSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;
        if (pPlayerCharacter != null)
        {
            CharacterInvenItemInfo pCharacterInfo = pPlayerCharacter.GetCharacterInvenItemInfo();

            ExcelData_Action_LevelUpInfo pActionLevelUpInfo;
            pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(m_pActionInfo.m_nID, pCharacterInfo.m_nAction_Level);

            if (pActionLevelUpInfo != null && pActionLevelUpInfo.m_strChange_EffectHit_Target_Prefab != "0")
            {
                strEffectHit_Target = pActionLevelUpInfo.m_strChange_EffectHit_Target_Prefab;
            }
        }


        float fDelayTime = 0;
        if ((strEffectHit_Target != "0" || m_pActionInfo.m_strEffectHit_Center != "0") && m_IsSelectTarget == true)
        {
            Vector3 vPos_Target = m_pTargetSlot.GetPosition();
            vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

            if (strEffectHit_Target != "0" &&
                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Contains(strEffectHit_Target) == false)
            {
                ParticleManager.Instance.LoadParticleSystem(strEffectHit_Target, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Add(strEffectHit_Target);
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

        SlotFixObject_PlayerCharacter pPlayerCharacter = m_pSrcSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;
        if (pPlayerCharacter != null)
        {
            CharacterInvenItemInfo pCharacterInfo = pPlayerCharacter.GetCharacterInvenItemInfo();
            ExcelData_Action_LevelUpInfo pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(m_pActionInfo.m_nID, pCharacterInfo.m_nAction_Level);

            MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

            pDataStack.m_nPlayerCharacterProjectileCount -= 1;

            if (pDataStack.m_nPlayerCharacterProjectileCount == 0 && pDataStack.m_nReflectDamageProjectileCount == 0)
            {
                EventDelegateManager.Instance.OnInGame_PlayerCharacterProjectile_Done(m_pActionInfo);
            }

            m_pParticleInfo.OnStop();
            ParticleManager.Instance.RemoveImmediateParticleInfo(m_pParticleInfo);
            m_pParticleInfo = null;

            if (m_pDamageTargetInfo.m_pRangeInfo != null)
            {
                MainGame_Espresso_ProcessHelper.ApplyStatusEffect(m_pSrcSlot, m_pTargetSlot, m_pActionInfo, m_pDamageTargetInfo.m_pRangeInfo);
            }
        }

        OnDone();
    }
}
