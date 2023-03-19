using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent_EnemyColonyEffect : TComponent<EventArg_Null, EventArg_Null>
{
    private MainGame_DataStack m_pDataStack = null;

    public TurnComponent_EnemyColonyEffect()
    {
        m_pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
    }

    public override void OnDestroy()
    {
        EventDelegateManager.Instance.OnEventInGame_EnemyColonyEffectDone -= OnInGame_EnemyColonyEffectDone;
    }

    public override void Update()
    {
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        OutputLog.Log("TurnComponent_EnemyColonyEffect : OnEvent");
        InGameTurnLog.Log("TurnComponent_EnemyColonyEffect : OnEvent");

        InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.EnemyColonyEffect;
        EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.EnemyColonyEffect);

        if (m_pDataStack.m_nEnemyColonyEffectCount == 0)
        {
            EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

            OutputLog.Log("TurnComponent_EnemyColonyEffect : GetNextEvent().OnEvent(EventArg_Null.Object)");
            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
        else
        {
            EventDelegateManager.Instance.OnEventInGame_EnemyColonyEffectDone += OnInGame_EnemyColonyEffectDone;
        }
    }

    public void OnInGame_EnemyColonyEffectDone()
    {
        if (m_pDataStack.m_nEnemyColonyEffectCount == 0)
        {
            EventDelegateManager.Instance.OnEventInGame_EnemyColonyEffectDone -= OnInGame_EnemyColonyEffectDone;

            EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

            m_pDataStack.m_EnemyColonyCreateTable.Clear();

            OutputLog.Log("TurnComponent_EnemyColonyEffect : GetNextEvent().OnEvent(EventArg_Null.Object)");
            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
    }

    public int IsProcess()
    {
        m_pDataStack.m_nPreCheckEnemyColonyEffectCount = 0;

        InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.EnemyColonyEffect;
        EventDelegateManager.Instance.OnInGame_PreChangeTurnComponent(eTurnComponentType.EnemyColonyEffect);

        if (m_pDataStack.m_nPreCheckEnemyColonyEffectCount != 0)
        {
            return 1;
        }

        return 0;
    }
}
