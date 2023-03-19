using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame_Cloud : MonoBehaviour
{
    private Slot                    m_pSlot                 = null;
    private eOwner                  m_eOwner                = eOwner.My;
    private int                     m_nTurnCount            = 0;
    private SlotFixObject_Cloud     m_pSlotFixObject_pCloud = null;

    private ParticleInfo            m_pParticleInfo         = null;

    public void Init(Slot pSlot, eOwner eOwner, int nTurnCount)
    {
        m_pSlot = pSlot;
        m_eOwner = eOwner;
        m_nTurnCount = nTurnCount;

        switch (m_eOwner)
        {
            case eOwner.My:
                {
                    Vector3 vPos = m_pSlot.GetPosition();
                    vPos.z = -(float)ePlaneOrder.Fx_Cloud;
                    m_pParticleInfo = ParticleManager.Instance.LoadParticleSystem("FX_Obstacle_DarkCloud_Idle_2", vPos);
                    m_pParticleInfo.SetScale(InGameInfo.Instance.m_fInGameScale);
                }
                break;

            case eOwner.Other:
                {
                    Vector3 vPos = m_pSlot.GetPosition();
                    vPos.z = -(float)ePlaneOrder.Fx_Cloud;
                    m_pParticleInfo = ParticleManager.Instance.LoadParticleSystem("FX_Obstacle_DarkCloud_Idle", vPos);
                    m_pParticleInfo.SetScale(InGameInfo.Instance.m_fInGameScale);
                }
                break;
        }

        EventDelegateManager.Instance.OnEventInGame_SlotFixObject_Cloud_Reset += OnInGame_SlotFixObject_Cloud_Reset;
    }

	private void OnDestroy()
	{
        EventDelegateManager.Instance.OnEventInGame_SlotFixObject_Cloud_Reset -= OnInGame_SlotFixObject_Cloud_Reset;
    }

	public Slot GetSlot()
    {
        return m_pSlot;
    }

    public eOwner GetOwner()
    {
        return m_eOwner;
    }

    public int GetTurnCount()
    {
        return m_nTurnCount;
    }

	public void Remove()
	{
        ParticleManager.Instance.RemoveParticleInfo(m_pParticleInfo);

        switch (m_eOwner)
        {
            case eOwner.My:
                {
                    Vector3 vPos = m_pSlot.GetPosition();
                    vPos.z = -(float)ePlaneOrder.Fx_Cloud;
                    ParticleInfo pParticleInfo = ParticleManager.Instance.LoadParticleSystem("FX_Obstacle_DarkCloud_Destroy_2", vPos);
                    pParticleInfo.SetScale(InGameInfo.Instance.m_fInGameScale);
                }
                break;

            case eOwner.Other:
                {
                    Vector3 vPos = m_pSlot.GetPosition();
                    vPos.z = -(float)ePlaneOrder.Fx_Cloud;
                    ParticleInfo pParticleInfo = ParticleManager.Instance.LoadParticleSystem("FX_Obstacle_DarkCloud_Destroy", vPos);
                    pParticleInfo.SetScale(InGameInfo.Instance.m_fInGameScale);
                }
                break;
        }

        if (m_pSlotFixObject_pCloud != null)
        {
            m_pSlot.RemoveSlotFixObject(m_pSlotFixObject_pCloud);
            m_pSlotFixObject_pCloud = null;
        }

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        pDataStack.m_pCloudManager.RemoveCloud(this);

        EventDelegateManager.Instance.OnEventInGame_SlotFixObject_Cloud_Reset -= OnInGame_SlotFixObject_Cloud_Reset;
    }

	public void DecreaseTurn()
    {
        --m_nTurnCount;

        if (m_nTurnCount <= 0)
        {
            ParticleManager.Instance.RemoveParticleInfo(m_pParticleInfo);

            switch (m_eOwner)
            {
                case eOwner.My:
                    {
                        Vector3 vPos = m_pSlot.GetPosition();
                        vPos.z = -(float)ePlaneOrder.Fx_Cloud;
                        ParticleInfo pParticleInfo = ParticleManager.Instance.LoadParticleSystem("FX_Obstacle_DarkCloud_Destroy_2", vPos);
                        pParticleInfo.SetScale(InGameInfo.Instance.m_fInGameScale);
                    }
                    break;

                case eOwner.Other:
                    {
                        Vector3 vPos = m_pSlot.GetPosition();
                        vPos.z = -(float)ePlaneOrder.Fx_Cloud;
                        ParticleInfo pParticleInfo = ParticleManager.Instance.LoadParticleSystem("FX_Obstacle_DarkCloud_Destroy", vPos);
                        pParticleInfo.SetScale(InGameInfo.Instance.m_fInGameScale);
                    }
                    break;
            }

            if (m_pSlotFixObject_pCloud != null)
            {
                m_pSlot.RemoveSlotFixObject(m_pSlotFixObject_pCloud);
                m_pSlotFixObject_pCloud = null;
            }
            MainGame_DataStack pDataStack = DataStackManager.Instance.Find< MainGame_DataStack>();
            pDataStack.m_pCloudManager.RemoveCloud(this);

            EventDelegateManager.Instance.OnEventInGame_SlotFixObject_Cloud_Reset -= OnInGame_SlotFixObject_Cloud_Reset;
        }
    }

    public void OnInGame_SlotFixObject_Cloud_Reset()
    {
        if (m_pSlotFixObject_pCloud != null)
        {
            m_pSlot.RemoveSlotFixObject(m_pSlotFixObject_pCloud);
            m_pSlotFixObject_pCloud = null;
        }

        if (EspressoInfo.Instance.m_ePVP_CurrTurn_Owner != m_eOwner)
        {
            m_pSlotFixObject_pCloud = new SlotFixObject_Cloud(m_pSlot);
            m_pSlot.AddSlotFixObject_Cloud(m_pSlotFixObject_pCloud);
        }
    }
}
