using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_BlockMatch_SP_ChargeMissile : GameEvent
{
    private Slot                            m_pSlot                     = null;
    private SlotFixObject_PlayerCharacter   m_pPlayerCharacter          = null;
    private eElement                        m_eBlockElement             = eElement.Neutral;

    private ParticleInfo                    m_pParticleInfo_Missile     = null;
    private Transformer_Bazier4             m_pBazier                   = null;

    public GameEvent_BlockMatch_SP_ChargeMissile(Slot pSlot, SlotFixObject_PlayerCharacter pPlayerCharacter, eElement eBlockElement)
    {
        m_pSlot = pSlot;
        m_pPlayerCharacter = pPlayerCharacter;
        m_eBlockElement = eBlockElement;

        CreateProjectile();

        Helper.OnSoundPlay("INGAME_BLOCK_MATCH_SP_GET", false);
    }

    private void CreateProjectile()
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        pDataStack.m_nSPChargeProjectileCount += 1;

        Vector3 vStartPos = m_pSlot.GetPosition();
        vStartPos.z = 0;
        Vector3 vEndPos = m_pPlayerCharacter.GetSlot().GetPosition();
        vEndPos.z = 0;

        m_pParticleInfo_Missile = ParticleManager.Instance.LoadParticleSystem("FX_SP_Charge_Missile_" + m_eBlockElement, new Vector3(vStartPos.x, vStartPos.y, -(float)ePlaneOrder.Fx_TopLayer));
        m_pParticleInfo_Missile.SetScale(InGameInfo.Instance.m_fInGameScale);

        Vector3 vCross = Vector3.Cross(vStartPos, vEndPos);
        vCross.Normalize();

        if(UnityEngine.Random.Range(0,2) == 0)
            vCross *= 64;
        else
            vCross *= -64;

        Vector3 vSecondPos = new Vector3(0, 120, 0);

        float fDistance = Vector3.Distance(vStartPos, vEndPos);
        float fTime = (fDistance / 128.0f) * 0.15f;

        TransformerEvent eventValue;

        m_pBazier = new Transformer_Bazier4(vStartPos);
        eventValue = new TransformerEvent_Vector3(0, vStartPos + vCross);
        m_pBazier.AddEvent(eventValue);
        eventValue = new TransformerEvent_Vector3(0, vEndPos + vCross);
        m_pBazier.AddEvent(eventValue);
        eventValue = new TransformerEvent_Vector3(fTime, vEndPos);
        m_pBazier.AddEvent(eventValue);
        m_pBazier.SetCallback(null, OnDone_Pos);
        m_pBazier.OnPlay();
    }

    public override void OnDestroy()
    {
        ParticleManager.Instance.RemoveParticleInfo(m_pParticleInfo_Missile);
    }

    public override void Update()
    {
        m_pBazier.Update(Time.deltaTime);
        Vector3 vPos = m_pBazier.GetCurVector3();
        vPos.z = -(float)ePlaneOrder.Fx_TopLayer;
        m_pParticleInfo_Missile.SetPosition(vPos);
    }

    private void OnDone_Pos(TransformerEvent eventVAlue)
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        int nPlusSP = m_pPlayerCharacter.GetChargePerBlock_SP();

        int nSP = m_pPlayerCharacter.GetSP();
        m_pPlayerCharacter.ChangeSP(nSP + nPlusSP);

        pDataStack.m_nSPChargeProjectileCount -= 1;

        if (pDataStack.m_nSPChargeProjectileCount == 0)
        {
            EventDelegateManager.Instance.OnInGame_Projectile_SP_Charge_Done();
        }

        OnDone();
    }
}
