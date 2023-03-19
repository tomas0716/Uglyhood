using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotFixObject
{
    protected eSlotFixObjectType m_eSlotFixObjectType = eSlotFixObjectType.Obstacle;
    protected object    m_pParameta                 = null;
    protected bool      m_IsSlotBlockExist          = false;
    protected bool      m_IsMoveAndCreateInclude    = false;
    protected bool      m_IsMoving                  = false;

    public SlotFixObject(eSlotFixObjectType eType)      { m_eSlotFixObjectType = eType; }
    public virtual void OnDestroy()                     { }
    public virtual void Update(float fDeltaTime)        { }
    public virtual void SetPosition(Vector2 vPos)       { }
    public virtual void SetHighlight(int nDepth = 0)    { }
    public virtual void ClearHighlight()                { }

    public eSlotFixObjectType GetSlotFixObjectType()
    {
        return m_eSlotFixObjectType;
    }

    public void SetParameta(object pParameta)
    {
        m_pParameta = pParameta;
    }

    public object GetParameta()
    {
        return m_pParameta;
    }

    public bool IsSlotBlockExist()
    {
        return m_IsSlotBlockExist;
    }

    public bool IsMoveAndCreateInclude()
    {
        return m_IsMoveAndCreateInclude;
    }

    public void SetMoving(bool IsMoving)
    {
        m_IsMoving = IsMoving;
    }

    public bool IsMoving()
    {
        return m_IsMoving;
    }
}
