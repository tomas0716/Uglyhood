using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent_PlayerStatusEffect_Buff : TComponent<EventArg_Null, EventArg_Null>
{
    private Transformer_Timer   m_pTimer_Delay_NextEvent    = new Transformer_Timer();
    private eOwner              m_eOwner                    = eOwner.My;

    public TurnComponent_PlayerStatusEffect_Buff(eOwner eOwn)
    {
        m_eOwner = eOwn;

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
        OutputLog.Log("TurnComponent_PlayerStatusEffect_Buff : OnEvent");
        InGameTurnLog.Log("TurnComponent_PlayerStatusEffect_Buff : OnEvent");

        InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.PlayerStatusEffect_Buff;
        EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.PlayerStatusEffect_Buff);

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        bool IsProcess = false;

        Dictionary<int, SlotFixObject_PlayerCharacter> characterTable = null;
        if (m_eOwner == eOwner.My)
        {
            characterTable = pDataStack.m_PlayerCharacterTable;
        }
        else
        {
            characterTable = pDataStack.m_OtherPlayerCharacterTable;
        }

        List<SlotFixObject_Unit> list = new List<SlotFixObject_Unit>();

        foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in characterTable)
        {
            list.Add(item.Value);
        }

        foreach (KeyValuePair<int, SlotFixObject_Minion> item in pDataStack.m_PlayerSummonUnitTable)
        {
            if(item.Value.GetOwner() == m_eOwner)
                list.Add(item.Value);
        }

        foreach (SlotFixObject_Unit pUnit in list)
        {
            switch (pUnit.GetObjectType())
            {
                case eObjectType.Character:
                    {
                        SlotFixObject_PlayerCharacter pPlayerCharacter = pUnit as SlotFixObject_PlayerCharacter;
                        if (pPlayerCharacter != null && pPlayerCharacter.IsDead() == false && pUnit.DecreaseStatusEffectTurn(eBuffType.Buff) == true)
                        {
                            IsProcess = true;
                        }
                    }
                    break;

                default:
                    {
                        if (pUnit.DecreaseStatusEffectTurn(eBuffType.Buff) == true)
                        {
                            IsProcess = true;
                        }
                    }
                    break;
            }
        }

        if (IsProcess == true)      // 만약 상태효과 처리할게 있다면 처리 후 다음 이벤트 넘김
        {
            m_pTimer_Delay_NextEvent.OnPlay();
        }
        else
        {
            EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

            OutputLog.Log("TurnComponent_PlayerStatusEffect_Buff : GetNextEvent().OnEvent(EventArg_Null.Object)");
            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
    }

    private void OnDone_Timer_Delay_NextEvent(TransformerEvent eventValue)
    {
        OutputLog.Log("TurnComponent_PlayerStatusEffect_Buff : OnDone_Timer_Delay_NextEvent");

        EventDelegateManager.Instance.OnInGame_Unit_CheckBuffDebuffMark();
        EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

        OutputLog.Log("TurnComponent_PlayerStatusEffect_Buff : GetNextEvent().OnEvent(EventArg_Null.Object)");
        GetNextEvent().OnEvent(EventArg_Null.Object);
    }
}
