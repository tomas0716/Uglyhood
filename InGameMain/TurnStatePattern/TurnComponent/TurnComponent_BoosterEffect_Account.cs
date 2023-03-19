using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent_BoosterEffect_Account : TComponent<EventArg_Null, EventArg_Null>
{
    public TurnComponent_BoosterEffect_Account()
    {
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        OutputLog.Log("TurnComponent_BoosterEffect_Account : OnEvent");
        InGameTurnLog.Log("TurnComponent_BoosterEffect_Account : OnEvent");

        InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.BoosterEffect_Account;
        EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.BoosterEffect_Account);

        EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

        OutputLog.Log("TurnComponent_BoosterEffect_Account : GetNextEvent().OnEvent(EventArg_Null.Object)");
        GetNextEvent().OnEvent(EventArg_Null.Object);
    }
}
