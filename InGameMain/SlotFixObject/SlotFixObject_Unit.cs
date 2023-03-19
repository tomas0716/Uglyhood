using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotFixObject_Unit : SlotFixObject_Espresso
{
	protected int					m_nUnitID					= 0;
	protected ExcelData_UnitInfo	m_pOriginalUnitInfo			= null;
	protected ExcelData_UnitInfo	m_pUnitInfo					= null;

	protected int					m_nLevel					= 0;

	protected int					m_nSP						= 0;
	protected int					m_nMaxSP					= 0;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 상태 효과, 패시브 효과로 변화는 스탯의 총 합
	// protected int				m_nMaxHP					= 0;		// SlotFixObject_Espresso 가 가지고 있음
	protected int					m_nATK						= 0;
	protected int					m_nChargePerTurn_SP			= 0;
	protected int					m_nChargePerBlock_SP		= 0;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 상태 효과, 패시브 효과로 더해지는 값
	protected int					m_nAdd_MaxHP				= 0;
	protected int					m_nAdd_ATK					= 0;
	protected int					m_nAdd_ChargePerTurn_SP		= 0;
	protected int					m_nAdd_ChargePerBlock_SP	= 0;
	protected int []				m_nAdd_ElementResist		= new int[(int)eElement.Max];

	////////////////////////////////////////////////////////////////////////////////////////////////////////////

	protected int					m_nDelayTurn				= 0;

	protected Unit_StatusEffect		m_pUnit_StatusEffect		= null;

	protected GameObject			m_pGameObject_Buff			= null;
	protected GameObject			m_pGameObject_Debuff		= null;

	protected eDamageEffectType		m_eEffecType_TransformType	= eDamageEffectType.None;
	protected bool					m_IsTransformUnitLifeTurn_Infinity = false;
	protected int					m_nTransformTurnLifeCount	= 0;
	protected string				m_strTransformRestoreEffect = "";

	public SlotFixObject_Unit(Slot pSlot, int nUnitID, eObjectType eObjectType, eOwner eOwner) : base(pSlot, eObjectType, eOwner)
	{
		m_nUnitID = nUnitID;
		m_pOriginalUnitInfo = m_pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(m_nUnitID);

		m_nLevel = m_pUnitInfo.m_nLevel;
		m_nMaxSP = m_pUnitInfo.m_nMaxSP;
		m_nATK = m_pUnitInfo.m_nATK;
		m_nChargePerTurn_SP = m_pUnitInfo.m_nSP_ChargePerTurn;
		m_nChargePerBlock_SP = m_pUnitInfo.m_nSP_ChargePerBlock;

		m_pUnit_StatusEffect = new Unit_StatusEffect(this);

		for (int i = 0; i < (int)eElement.Max; ++i)
		{
			m_nAdd_ElementResist[i] = 0;
		}

		EventDelegateManager.Instance.OnEventInGame_Unit_CheckBuffDebuffMark += OnInGame_Unit_CheckBuffDebuffMark;
	}

	public override void OnDestroy()
	{
		base.OnDestroy();

		m_pUnit_StatusEffect.OnDestroy();

		EventDelegateManager.Instance.OnEventInGame_Unit_CheckBuffDebuffMark -= OnInGame_Unit_CheckBuffDebuffMark;
	}

	public virtual ExcelData_UnitInfo GetOriginalUnitInfo()
	{
		return m_pOriginalUnitInfo;
	}

	public virtual ExcelData_UnitInfo GetUnitInfo()
	{
		return m_pUnitInfo;
	}

	public virtual int GetLevel()
	{
		return m_nLevel;
	}

	public virtual int GetATK()
	{
		return m_nATK;
	}

	public virtual void ChangeSP(int nSP)
	{
		if (m_nMaxSP == 0 || m_pUnitInfo.m_nActiveSkill_ActionTableID == 0)
			return;

		m_nSP = nSP;
		if (m_nSP <= 0)
		{
			m_nSP = 0;
		}
		else if (m_nSP > GetMaxSP())
		{
			m_nSP = GetMaxSP();
		}
	}

	public virtual void ChangeSP(int nSP, Color color)
	{
		ChangeSP(nSP);
	}

	public virtual int GetMaxSP()
	{
		return m_nMaxSP;
	}

	public virtual int GetSP()
	{
		return m_nSP;
	}

	public virtual bool IsFull_SP()
	{
		if (GetMaxSP() == 0 || m_pUnitInfo.m_nActiveSkill_ActionTableID == 0)
			return false;

		return GetMaxSP() <= m_nSP;
	}

	public virtual int GetChargePerTurn_SP()
	{
		return m_nChargePerTurn_SP;
	}

	public virtual int GetChargePerBlock_SP()
	{
		return m_nChargePerBlock_SP;
	}

	public virtual void AddApplyStatusEffect(ExcelData_StatusEffectInfo pStatusEffectInfo, ExcelData_ActionInfo pActionInfo, int nApplyValue, eOwner eOwner_Subject, SlotFixObject_Unit pUnit_Actor, eUnitDamage eUnitDamage = eUnitDamage.Damage)
	{
		m_pUnit_StatusEffect.AddApplyStatusEffect(pStatusEffectInfo, pActionInfo, nApplyValue, eOwner_Subject, pUnit_Actor, eUnitDamage);
	}

	public virtual void StopApplyStatusEffect(int nStatusEffectGroup)
	{
		m_pUnit_StatusEffect.StopApplyStatusEffect(nStatusEffectGroup);
	}

	public virtual bool IsExistApplyStatusEffect_byEffectType(eDamageEffectType eEffectType)
	{
		return m_pUnit_StatusEffect.IsExistApplyStatusEffect_byEffectType(eEffectType);
	}

	public List<ApplyStatusEffect> GetApplyStatusEffect_byEffectType(eDamageEffectType eEffectType)
	{
		return m_pUnit_StatusEffect.GetApplyStatusEffect_byEffectType(eEffectType);
	}

	public Unit_StatusEffect GetUnit_StatusEffect()
	{
		return m_pUnit_StatusEffect;
	}

	public virtual bool VirtualDecreaseTurn()
	{
		int nDelayTurn = m_nDelayTurn;
		--nDelayTurn;

		if (nDelayTurn == 0)
		{
			return true;
		}

		return false;
	}

	public virtual bool DecreaseTurn()
	{
		--m_nDelayTurn;

		if (m_nDelayTurn == 0)
		{
			m_nDelayTurn = m_pUnitInfo.m_nMaxAttackDelayTurn;
			return true;
		}

		return false;
	}

	public virtual int GetDelayTurn()
	{
		return m_nDelayTurn;
	}

	public virtual bool VirtualDecreaseStatusEffectTurn(eBuffType eType)
	{
		return m_pUnit_StatusEffect.VirtualDecreaseTurn(eType);
	}

	public virtual bool DecreaseStatusEffectTurn(eBuffType eType)
	{
		return m_pUnit_StatusEffect.DecreaseTurn(eType);
	}

	public virtual void OnNumberDamageActionDone(NumberDamageAction pNumberDamageAction)
	{
	}

	public override void ChangeHP(int nHP)
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

	public override void ChangeHP(int nHP, Color color)
	{
		ChangeHP(nHP);
	}

	public override int GetHP()
	{
		return m_nHP;
	}

	public override int GetMaxHP()
	{
		return (int)m_nMaxHP;
	}

	public void OnInGame_Unit_CheckBuffDebuffMark()
	{
		OnCheckBuffDebuffMark();
	}

	public virtual void OnCheckBuffDebuffMark()
	{
		if (m_pGameObject_Buff != null)
		{
			m_pGameObject_Buff.SetActive(m_pUnit_StatusEffect.IsExistBuff());
		}

		if (m_pGameObject_Debuff != null)
		{
			m_pGameObject_Debuff.SetActive(m_pUnit_StatusEffect.IsExistDebuff());
		}
	}

	public int GetBuffCount()
	{
		return m_pUnit_StatusEffect.GetBuffCount();
	}

	public ApplyStatusEffect GetApplyStatusBuffEffect_byIndex(int nIndex)
	{
		return m_pUnit_StatusEffect.GetApplyStatusBuffEffect_byIndex(nIndex);
	}

	public int GetDebuffCount()
	{
		return m_pUnit_StatusEffect.GetDebuffCount();
	}

	public ApplyStatusEffect GetApplyStatusDebuffEffect_byIndex(int nIndex)
	{
		return m_pUnit_StatusEffect.GetApplyStatusDebuffEffect_byIndex(nIndex);
	}

	public virtual void SetDamageEffectType_TransformType(eDamageEffectType eType, int nTurnLifeCount, string strTransformRestoreEffect)
	{
		if (eType == eDamageEffectType.UnitSummon || eType == eDamageEffectType.UnitTransform || eType == eDamageEffectType.UnitClone)
		{
			m_eEffecType_TransformType = eType;
			m_nTransformTurnLifeCount = nTurnLifeCount;
			m_strTransformRestoreEffect = strTransformRestoreEffect;
		}
	}

	public virtual eDamageEffectType GetDamageEffectType_TransformType()
	{
		return m_eEffecType_TransformType;
	}

	public virtual int GetTransformTurnLifeCount()
	{
		return m_nTransformTurnLifeCount;
	}

	public virtual bool IsTransformUnitLifeTurn_Infinity()
	{
		return m_IsTransformUnitLifeTurn_Infinity;
	}

	public virtual bool DecreaseTransformTurnLife()
	{
		--m_nTransformTurnLifeCount;

		if (m_nTransformTurnLifeCount == 0)
		{
			return true;
		}

		return false;
	}

	public virtual string GetTransformRestoreEffect()
	{
		return m_strTransformRestoreEffect;
	}

	public virtual void OnTransformUnit(int nUnitID, int nUnitLevel)
	{
	}

	public virtual void OnTransformUnit(int nUnitID, int nUnitLevel, int nActionLevel)
	{
	}

	public virtual void OnTransformUnitRestore()
	{
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 상태 효과, 패시브 효과로 더해지는 값
	/// Get
	public int GetAdd_MaxHP()
	{
		return m_nAdd_MaxHP;
	}

	public int GetAdd_ATK()
	{
		return m_nAdd_ATK;
	}

	public int GetAdd_ChargePerTurn_SP()
	{
		return m_nAdd_ChargePerTurn_SP;
	}

	public int GetAdd_ChargePerBlock_SP()
	{
		return m_nAdd_ChargePerBlock_SP;
	}

	public int GetAdd_ElementResist(eElement eElem)
	{
		return m_nAdd_ElementResist[(int)eElem];
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 상태 효과, 패시브 효과로 더해지는 값
	/// Add
	public void PlusAdd_MaxHP(int nValue)
	{
		m_nAdd_MaxHP += nValue;
		m_nMaxHP += nValue;

		ChangeHP(GetHP(), new Color(1,1,1,0));
	}

	public void PlusAdd_ATK(int nValue)
	{
		m_nAdd_ATK += nValue;
		m_nATK += nValue;
	}

	public void PlusAdd_ChargePerTurn_SP(int nValue)
	{
		m_nAdd_ChargePerTurn_SP += nValue;
		m_nChargePerTurn_SP += nValue;
	}

	public void PlusAdd_ChargePerBlock_SP(int nValue)
	{
		m_nAdd_ChargePerBlock_SP += nValue;
		m_nChargePerBlock_SP += nValue;
	}

	public void PlusAdd_ElementResist(eElement eElem, int nValue)
	{
		m_nAdd_ElementResist[(int)eElem] += nValue;
	}
}
