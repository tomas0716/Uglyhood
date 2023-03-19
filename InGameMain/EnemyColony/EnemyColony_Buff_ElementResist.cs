using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColony_Buff_ElementResist : EnemyColony_Base
{
    private Transformer_Timer   m_pTimer_Effect                     = new Transformer_Timer();

    public override void Init(Slot pSlot, ExcelData_EnemyColonyInfo pEnemyColonyInfo)
    {
        base.Init(pSlot, pEnemyColonyInfo);
    }

	private void Update()
	{
        m_pTimer_Effect.Update(Time.deltaTime);
    }

    public override void Inner_PreChangeTurnComponent(eTurnComponentType eType)
    {
        if (eType == eTurnComponentType.EnemyColonyEffect)
        {
            SlotFixObject_Minion pMinion = m_pSlot.GetLastSlotFixObject() as SlotFixObject_Minion;

            if (pMinion != null)
            {
                if (m_pEnemyColonyInfo != null)
                {
                    ExcelData_StatusEffectInfo pStatusEffectInfo = ExcelDataManager.Instance.m_pStatusEffect.GetStatusEffectInfo(m_pEnemyColonyInfo.m_nStatuseEffectID);

                    if (pStatusEffectInfo != null)
                    {
                        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
                        ++pDataStack.m_nPreCheckEnemyColonyEffectCount;
                    }
                }
            }
        }
    }

    public override void Inner_ChangeTurnComponent(eTurnComponentType eType)
    {
        if (eType == eTurnComponentType.EnemyColonyEffect)
        {
            SlotFixObject_Minion pMinion = m_pSlot.GetLastSlotFixObject() as SlotFixObject_Minion;

            if (pMinion != null)
            {
                if (m_pEnemyColonyInfo != null)
                {
                    ExcelData_StatusEffectInfo pStatusEffectInfo = ExcelDataManager.Instance.m_pStatusEffect.GetStatusEffectInfo(m_pEnemyColonyInfo.m_nStatuseEffectID);

                    if (pStatusEffectInfo != null)
                    {
                        MainGame_Espresso_ProcessHelper.ApplyStatusEffect(null, m_pSlot, pStatusEffectInfo, true, null);

                        m_pTimer_Effect.OnReset();
                        TransformerEvent_Timer eventValue;
                        eventValue = new TransformerEvent_Timer(1.0f);
                        m_pTimer_Effect.AddEvent(eventValue);
                        m_pTimer_Effect.SetCallback(null, OnDone_Timer_Effect);
                        m_pTimer_Effect.OnPlay();

                        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
                        ++pDataStack.m_nEnemyColonyEffectCount;

                        OutputLog.Log("EnemyColony_Buff_Heal : Inner_ChangeTurnComponent > ++pDataStack.m_nEnemyColonyEffectCount");
                    }
                }
            }
        }
    }

    public override void Inner_TurnComponentDone(eTurnComponentType eType)
    {
        if (eType == eTurnComponentType.EnemyColonyChangeState)
        {
        }
    }

    private void OnDone_Timer_Effect(TransformerEvent eventValue)
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        --pDataStack.m_nEnemyColonyEffectCount;

        OutputLog.Log("EnemyColony_Buff_Heal : OnDone_Timer_Effect > --pDataStack.m_nEnemyColonyEffectCount");

        EventDelegateManager.Instance.OnInGame_EnemyColonyEffectDone();
    }
}
