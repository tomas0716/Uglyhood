using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotFixObject_Obstacle : SlotFixObject
{
    protected GameObject    m_pGameObject                       = null;
    protected Slot          m_pSlot                             = null;
    protected Vector3       m_vPos                              = Vector3.zero;

    protected int           m_nLayer                            = 1;

    protected eSlotFixObjectObstacleType m_eSlotFixObjectObstacleType = eSlotFixObjectObstacleType.Box;

    protected List<Slot>    m_DyingNeighborSlotList             = new List<Slot>();

    protected bool          m_IsSlotDyingAtOnlyMineBreak        = false;                    // 블록이 있고, 블록엔 영향이 없고 나만 영향을 받음
    protected bool          m_IsSlotBlockNoSwap                 = false;                    // 블록이 있고, 스왑 불가능
    protected bool          m_IsSlotBlockMatchPossible          = true;                     // 블록이 있고, 매치 가능
    protected bool          m_IsSlotBlockMatchInPossibleAtCheckRemovePossible = false;

    protected bool          m_IsNeighborSlotBlockDyingAtBreak   = false;                    // 블록이 없고, 주변 블록 죽을때 영향을 받음

    public SlotFixObject_Obstacle(Slot pSlot, eSlotFixObjectObstacleType eObstacleType) : base(eSlotFixObjectType.Obstacle)
    {
        m_pSlot = pSlot;
        m_eSlotFixObjectObstacleType = eObstacleType;
    }

    public override void OnDestroy()
    {
        if (m_pGameObject != null)
        {
            GameObject.Destroy(m_pGameObject);
            m_pGameObject = null;
        }
    }

    public Slot GetSlot()
    {
        return m_pSlot;
    }

    public eSlotFixObjectObstacleType GetObstacleType()
    {
        return m_eSlotFixObjectObstacleType;
    }

    public GameObject GetGameObject()
    {
        return m_pGameObject;
    }

    public int GetLayer()
    {
        return m_nLayer;
    }

    public override void SetPosition(Vector2 vPos)
    {
        m_vPos = vPos;
        m_pGameObject.transform.position = new Vector3(m_vPos.x, m_vPos.y, 0);
    }

    public virtual void OnBreak()
    {
    }

    public virtual void AddDyingNeighborSlot(Slot pSlot)
    {
        m_DyingNeighborSlotList.Add(pSlot);
    }

    public virtual void ClearDyingNeighborSlot()
    {
        m_DyingNeighborSlotList.Clear();
    }

    public bool IsSlotDyingAtOnlyMineBreak()
    {
        return m_IsSlotDyingAtOnlyMineBreak;
    }

    public bool IsSlotBlockNoSwap()
    {
        return m_IsSlotBlockNoSwap;
    }

    public bool IsSlotBlockMatchPossible()
    {
        return m_IsSlotBlockMatchPossible;
    }

    public bool IsSlotBlockMatchInPossibleAtCheckRemovePossible()
    {
        return m_IsSlotBlockMatchInPossibleAtCheckRemovePossible;
    }

    public bool IsNeighborSlotBlockDyingAtBreak()
    {
        return m_IsNeighborSlotBlockDyingAtBreak;
    }

    public virtual void OnDead()
    {
        // 일단 바로 죽임
        m_pSlot.RemoveSlotFixObject(this);
    }

    public override void SetHighlight(int nDepth = 0)
    {
        Vector3 vPos = m_pGameObject.transform.position;
        vPos.z = -(float)(ePlaneOrder.Highlight_Content + nDepth);
        m_pGameObject.transform.position = vPos;
    }

    public override void ClearHighlight()
    {
        if (m_pGameObject != null)
        {
            Vector3 vPos = m_pGameObject.transform.position;
            vPos.z = 0;
            m_pGameObject.transform.position = vPos;
        }
    }
}