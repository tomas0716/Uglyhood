using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent_StartAction_FillBlock : TComponent<EventArg_Null, EventArg_Null>
{
    private bool    m_IsActiveComponent     = false;
    private float   m_fBackup_BlockMoveTime = 0.0f;

    public TurnComponent_StartAction_FillBlock()
    {
    }

    public override void OnDestroy()
    {
        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
    }

    public override void Update()
    {
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        OutputLog.Log("TurnComponent_StartAction_FillBlock : OnEvent");
        InGameTurnLog.Log("TurnComponent_StartAction_FillBlock : OnEvent");

        InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.StartAction_FillBlock;
        EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.StartAction_FillBlock);

        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone += OnInGame_CheckRemoveSlotDone;

        m_fBackup_BlockMoveTime = GameDefine.ms_fBlockMoveTime;
        GameDefine.ms_fBlockMoveTime *= 0.78f;

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        pDataStack.m_pSlotManager.SetBlockMoveTime(GameDefine.ms_fBlockMoveTime);
        pDataStack.m_pSlotManager.OnSlotMoveAndCreate(false);
        m_IsActiveComponent = true;
    }

    private void OnInGame_CheckRemoveSlotDone()
    {
        OutputLog.Log("TurnComponent_StartAction_FillBlock : OnInGame_MatchTurnComplete");

        if (m_IsActiveComponent == true)
        {
            EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;

            InGameInfo.Instance.m_IsInGameStart = true;
            m_IsActiveComponent = false;
            GameDefine.ms_fBlockMoveTime = m_fBackup_BlockMoveTime;

            EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

            OutputLog.Log("TurnComponent_StartAction_FillBlock : GetNextEvent().OnEvent(EventArg_Null.Object)");
            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
    }
}
