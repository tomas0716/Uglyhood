using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColony_Base : MonoBehaviour
{
    protected Slot                      m_pSlot             = null;
    protected ExcelData_EnemyColonyInfo m_pEnemyColonyInfo  = null;

    public virtual void Init(Slot pSlot, ExcelData_EnemyColonyInfo pEnemyColonyInfo)
    {
        m_pSlot = pSlot;
        m_pEnemyColonyInfo = pEnemyColonyInfo;

        EventDelegateManager.Instance.OnEventInGame_PreChangeTurnComponent += OnInGame_PreChangeTurnComponent;
        EventDelegateManager.Instance.OnEventInGame_ChangeTurnComponent += OnInGame_ChangeTurnComponent;
        EventDelegateManager.Instance.OnEventInGame_TurnComponentDone += OnInGame_TurnComponentDone;
    }

	private void OnDestroy()
	{
        EventDelegateManager.Instance.OnEventInGame_PreChangeTurnComponent -= OnInGame_PreChangeTurnComponent;
        EventDelegateManager.Instance.OnEventInGame_ChangeTurnComponent -= OnInGame_ChangeTurnComponent;
        EventDelegateManager.Instance.OnEventInGame_TurnComponentDone -= OnInGame_TurnComponentDone;
    }

    public void OnInGame_PreChangeTurnComponent(eTurnComponentType eType)
    {
        Inner_PreChangeTurnComponent(eType);
    }

    public void OnInGame_ChangeTurnComponent(eTurnComponentType eType)
    {
        Inner_ChangeTurnComponent(eType);
    }

    public void OnInGame_TurnComponentDone(eTurnComponentType eType)
    {
        Inner_TurnComponentDone(eType);
    }

    public virtual void Inner_PreChangeTurnComponent(eTurnComponentType eType)
    {
    }

    public virtual void Inner_ChangeTurnComponent(eTurnComponentType eType)
    {
    }

    public virtual void Inner_TurnComponentDone(eTurnComponentType eType)
    {
    }
}
