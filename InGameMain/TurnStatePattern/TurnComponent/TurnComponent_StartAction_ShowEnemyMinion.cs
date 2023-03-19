using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent_StartAction_ShowEnemyMinion : TComponent<EventArg_Null, EventArg_Null>
{
    Transformer_Timer m_pTimer = new Transformer_Timer();

    public TurnComponent_StartAction_ShowEnemyMinion()
    {
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
        m_pTimer.Update(Time.deltaTime);
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        OutputLog.Log("TurnComponent_StartAction_ShowEnemyMinion : OnEvent");
        InGameTurnLog.Log("TurnComponent_StartAction_ShowEnemyMinion : OnEvent");

        InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.StartAction_ShowEnemyMinion;
        EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.StartAction_ShowEnemyMinion);

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        if (pDataStack.m_EnemyMinionTable.Count == 0)
        {
            TransformerEvent_Timer eventValue;
            eventValue = new TransformerEvent_Timer(0.6f);
            m_pTimer.AddEvent(eventValue);
            m_pTimer.SetCallback(null, OnDone_Timer_Next);
            m_pTimer.OnPlay();
        }
        else
        {
            TransformerEvent_Timer eventValue;
            eventValue = new TransformerEvent_Timer(0.6f);
            m_pTimer.AddEvent(eventValue);
            m_pTimer.SetCallback(null, OnDone_Timer_ShowEnemy);
            m_pTimer.OnPlay();
        }
    }

    private void OnDone_Timer_Next(TransformerEvent eventvalue)
    {
        EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

        OutputLog.Log("TurnComponent_StartAction_ShowEnemyMinion : GetNextEvent().OnEvent(EventArg_Null.Object)");
        GetNextEvent().OnEvent(EventArg_Null.Object);
    }

    private void OnDone_Timer_ShowEnemy(TransformerEvent eventvalue)
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        foreach (KeyValuePair<int, SlotFixObject_Minion> item in pDataStack.m_EnemyMinionTable)
        {
            item.Value.SetVisible(true);
            ParticleManager.Instance.LoadParticleSystem("FX_UnitWarp", item.Value.GetSlot().GetPosition()).SetScale(InGameInfo.Instance.m_fInGameScale);
        }

        TransformerEvent_Timer eventValue;
        eventValue = new TransformerEvent_Timer(1);
        m_pTimer.AddEvent(eventValue);
        m_pTimer.SetCallback(null, OnDone_Timer_Next);
        m_pTimer.OnPlay();

        Helper.OnSoundPlay("INGAME_MINION_MOVE_WARP", false);
    }
}
