using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnState_StartAction : TComponent<EventArg_Null, EventArg_Null>
{   
    private TurnComponent_StartAction_ShowEnemyMinion   m_pTurnComponent_StartAction_ShowEnemyMinion    = new TurnComponent_StartAction_ShowEnemyMinion();
    private TurnComponent_StartAction_FillBlock         m_pTurnComponent_StartAction_FillBlock          = new TurnComponent_StartAction_FillBlock();

    private TEventDelegate<EventArg_Null>               m_pDone                                         = new TEventDelegate<EventArg_Null>();

    private bool                                        m_IsProcess                                     = false;


    public TurnState_StartAction()
    {
        m_pDone.SetFunc(OnDone);

        m_pTurnComponent_StartAction_ShowEnemyMinion.SetNextEvent(m_pTurnComponent_StartAction_FillBlock);
        m_pTurnComponent_StartAction_FillBlock.SetNextEvent(m_pDone);
    }

    public override void OnDestroy()
    {
        m_pTurnComponent_StartAction_ShowEnemyMinion.OnDestroy();
        m_pTurnComponent_StartAction_FillBlock.OnDestroy();
    }

    public override void Update()
    {
        m_pTurnComponent_StartAction_ShowEnemyMinion.Update();
        m_pTurnComponent_StartAction_FillBlock.Update();
    }

    public override void LateUpdate()
    {
        m_pTurnComponent_StartAction_ShowEnemyMinion.LateUpdate();
        m_pTurnComponent_StartAction_FillBlock.LateUpdate();
    }

    private void OnDone(EventArg_Null Arg)
    {
        OutputLog.Log("TurnState_StartAction : OnDone");

        m_IsProcess = false;
        GetNextEvent().OnEvent(EventArg_Null.Object);
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        if (m_IsProcess == false)
        {
            OutputLog.Log("TurnState_StartAction : OnEvent");

            m_pTurnComponent_StartAction_ShowEnemyMinion.OnEvent(EventArg_Null.Object);
            m_IsProcess = true;
        }
    }
}
