using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent_BoosterEffect_Group : TComponent<EventArg_Null, EventArg_Null>
{
    public TurnComponent_BoosterEffect_Group()
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
        OutputLog.Log("TurnComponent_BoosterEffect_Group : OnEvent");
        InGameTurnLog.Log("TurnComponent_BoosterEffect_Group : OnEvent");

        InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.BoosterEffect_Group;
        EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.BoosterEffect_Group);

        EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

        OutputLog.Log("TurnComponent_BoosterEffect_Group : GetNextEvent().OnEvent(EventArg_Null.Object)");
        GetNextEvent().OnEvent(EventArg_Null.Object);
    }
}
