using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotFixObject_Chain : SlotFixObject_Obstacle
{
    public SlotFixObject_Chain(Slot pSlot, int nLayer) : base(pSlot, eSlotFixObjectObstacleType.Chain)
    {
        m_nLayer = nLayer;

        ChangeLayer(m_nLayer);

        m_IsSlotDyingAtOnlyMineBreak = true;
        m_IsSlotBlockNoSwap = true;
        m_IsSlotBlockExist = true;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void Update(float fDeltaTime)
    {
    }

    public void ChangeLayer(int nLayer)
    {
        m_nLayer = nLayer;

        if (m_pGameObject != null)
        {
            GameObject.Destroy(m_pGameObject);
        }

        GameObject ob;

        ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_SlotFix/Chain_" + m_nLayer);
        m_pGameObject = GameObject.Instantiate(ob);
        m_pGameObject.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

        SetPosition(m_vPos);
    }

    public override void OnBreak()
    {
        ParticleManager.Instance.LoadParticleSystem("FX_Obstacle_Chain_Destroy_" + m_nLayer, m_vPos).SetScale(InGameInfo.Instance.m_fInGameScale);

        --m_nLayer;

        if (m_nLayer == 0)
        {
            OnDead();
        }
        else
        {
            ChangeLayer(m_nLayer);
        }
    }
}