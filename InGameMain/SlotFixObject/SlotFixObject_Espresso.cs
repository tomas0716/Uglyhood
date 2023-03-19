using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotFixObject_Espresso : SlotFixObject
{
    protected Slot          m_pSlot             = null;
    protected eObjectType   m_eObjectType       = eObjectType.Empty;
    protected eOwner        m_eOwner            = eOwner.My;

    protected int           m_nMaxHP            = 0;
    protected int           m_nHP               = 0;        // Minion : HP, EnemyColony : Level

    protected int           m_nVirtualHP        = 0;
    protected bool          m_IsVirtualDying    = false;

    public SlotFixObject_Espresso(Slot pSlot, eObjectType eObjectType, eOwner eOwner) : base(eSlotFixObjectType.Espresso)
    {
        m_pSlot = pSlot;
        m_eObjectType = eObjectType;
        m_eOwner = eOwner;
    }

    public virtual GameObject GetGameObject()
    {
        return null;
    }

    public void SetSlot(Slot pSlot)
    {
        m_pSlot = pSlot;
    }

    public Slot GetSlot()
    {
        return m_pSlot;
    }

    public eObjectType GetObjectType()
    {
        return m_eObjectType;
    }

    public eOwner GetOwner()
    {
        return m_eOwner;
    }

    public virtual void ChangeHP(int nHP)
    {
        m_nHP = nHP;
        if (m_nHP <= 0)
        {
            m_nHP = 0;
        }
        else if (m_nHP > GetMaxHP())
        {
            m_nHP = GetMaxHP();
        }

        ResetVirtualHP();
    }

    public virtual void ChangeHP(int nHP, Color color)
    {
        ChangeHP(nHP);
    }

    public virtual int GetHP()
    {
        return m_nHP;
    }

    public virtual int GetMaxHP()
    {
        return m_nMaxHP;
    }

    public virtual int ChangeVirtualHP(int nVirtualHP)
    {
        m_nVirtualHP = nVirtualHP;

        if (m_nVirtualHP <= 0)
        {
            m_nVirtualHP = 0;
            m_IsVirtualDying = true;
        }

        return m_nVirtualHP;
    }

    public virtual int GetVirtualHP()
    {
        return m_nVirtualHP;
    }

    public virtual void ResetVirtualHP()
    {
        m_nVirtualHP = m_nHP;
    }

    public virtual bool IsVirtualDying()
    {
        return m_IsVirtualDying;
    }

    public virtual void OnDamageText(eUnitDamage eString)
    {
    }

    public virtual void OnDamageText(string strText)
    {
    }

    public virtual void OnDead()
    {
    }
}
