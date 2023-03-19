using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent_EnemyStatusEffect_Debuff : TComponent<EventArg_Null, EventArg_Null>
{
    private Transformer_Timer m_pTimer_Delay_NextEvent = new Transformer_Timer();

    public TurnComponent_EnemyStatusEffect_Debuff()
    {
        m_pTimer_Delay_NextEvent.OnReset();

        TransformerEvent_Timer eventValue;
        eventValue = new TransformerEvent_Timer(0.5f);
        m_pTimer_Delay_NextEvent.AddEvent(eventValue);
        m_pTimer_Delay_NextEvent.SetCallback(null, OnDone_Timer_Delay_NextEvent);
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
        m_pTimer_Delay_NextEvent.Update(Time.deltaTime);
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        OutputLog.Log("TurnComponent_EnemyStatusEffect_Debuff : OnEvent");
        InGameTurnLog.Log("TurnComponent_EnemyStatusEffect_Debuff : OnEvent");

        InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.EnemyStatusEffect_Debuff;
        EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.EnemyStatusEffect_Debuff);

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        bool IsProcess = false;

        List<SlotFixObject_Minion> list = new List<SlotFixObject_Minion>();

        foreach (KeyValuePair<int, SlotFixObject_Minion> item in pDataStack.m_EnemyMinionTable)
        {
            list.Add(item.Value);
        }

        foreach (KeyValuePair<int, SlotFixObject_Minion> item in pDataStack.m_EnemySummonUnitTable)
        {
            list.Add(item.Value);
        }

        foreach (SlotFixObject_Minion pMinion in list)
        {
            if (pMinion.DecreaseStatusEffectTurn(eBuffType.Debuff) == true)
            {
                IsProcess = true;
            }
        }

        pDataStack.m_pCloudManager.DecreaseTurn(eOwner.My);

        if (IsProcess == true)      // 만약 상태효과 처리할게 있다면 처리 후 다음 이벤트 넘김
        {
            m_pTimer_Delay_NextEvent.OnPlay();
        }
        else
        {
            EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

            OutputLog.Log("TurnComponent_EnemyStatusEffect_Debuff : GetNextEvent().OnEvent(EventArg_Null.Object)");
            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
    }

    private void OnDone_Timer_Delay_NextEvent(TransformerEvent eventValue)
    {
        OutputLog.Log("TurnComponent_EnemyStatusEffect_Debuff : OnDone_Timer_Delay_NextEvent");

        EventDelegateManager.Instance.OnInGame_Unit_CheckBuffDebuffMark();
        EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

        OutputLog.Log("TurnComponent_EnemyStatusEffect_Debuff : GetNextEvent().OnEvent(EventArg_Null.Object)");
        GetNextEvent().OnEvent(EventArg_Null.Object);
    }

    public int IsProcess()
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        List<SlotFixObject_Minion> list = new List<SlotFixObject_Minion>();

        foreach (KeyValuePair<int, SlotFixObject_Minion> item in pDataStack.m_EnemyMinionTable)
        {
            list.Add(item.Value);
        }

        foreach (KeyValuePair<int, SlotFixObject_Minion> item in pDataStack.m_EnemySummonUnitTable)
        {
            list.Add(item.Value);
        }

        foreach (SlotFixObject_Minion pMinion in list)
        {
            if (pMinion.VirtualDecreaseStatusEffectTurn(eBuffType.Debuff) == true)
            {
                return 1;
            }
        }

        return 0;
    }
}
