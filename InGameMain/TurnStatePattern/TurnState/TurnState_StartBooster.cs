using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnState_StartBooster : TComponent<EventArg_Null, EventArg_Null>
{   
    private TurnComponent_BoosterEffect_Account     m_pTurnComponent_BoosterEffect_Account  = new TurnComponent_BoosterEffect_Account();
    private TurnComponent_BoosterEffect_Group       m_pTurnComponent_BoosterEffect_Group    = new TurnComponent_BoosterEffect_Group();
    private TurnComponent_BoosterEffect_Start       m_pTurnComponent_BoosterEffect_Start    = new TurnComponent_BoosterEffect_Start();
    private TurnComponent_EnemyMinionBossIntro      m_pTurnComponent_EnemyMinionBossIntro   = new TurnComponent_EnemyMinionBossIntro();

    private TEventDelegate<EventArg_Null>           m_pDone                                 = new TEventDelegate<EventArg_Null>();

    private bool                                    m_IsProcess                             = false;


    public TurnState_StartBooster()
    {
        m_pDone.SetFunc(OnDone);

        m_pTurnComponent_BoosterEffect_Account.SetNextEvent(m_pTurnComponent_BoosterEffect_Group);
        m_pTurnComponent_BoosterEffect_Group.SetNextEvent(m_pTurnComponent_BoosterEffect_Start);
        m_pTurnComponent_BoosterEffect_Start.SetNextEvent(m_pTurnComponent_EnemyMinionBossIntro);
        m_pTurnComponent_EnemyMinionBossIntro.SetNextEvent(m_pDone);
    }

    public override void OnDestroy()
    {
        m_pTurnComponent_BoosterEffect_Account.OnDestroy();
        m_pTurnComponent_BoosterEffect_Group.OnDestroy();
        m_pTurnComponent_BoosterEffect_Start.OnDestroy();
        m_pTurnComponent_EnemyMinionBossIntro.OnDestroy();
    }

    public override void Update()
    {
        m_pTurnComponent_BoosterEffect_Account.Update();
        m_pTurnComponent_BoosterEffect_Group.Update();
        m_pTurnComponent_BoosterEffect_Start.Update();
        m_pTurnComponent_EnemyMinionBossIntro.Update();
    }

    public override void LateUpdate()
    {
        m_pTurnComponent_BoosterEffect_Account.LateUpdate();
        m_pTurnComponent_BoosterEffect_Group.LateUpdate();
        m_pTurnComponent_BoosterEffect_Start.LateUpdate();
        m_pTurnComponent_EnemyMinionBossIntro.LateUpdate();
    }

    private void OnDone(EventArg_Null Arg)
    {
        OutputLog.Log("TurnState_StartBooster : OnDone");

        m_IsProcess = false;
        GetNextEvent().OnEvent(EventArg_Null.Object);
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        if (m_IsProcess == false)
        {
            OutputLog.Log("TurnState_StartBooster : OnEvent");

            m_pTurnComponent_BoosterEffect_Account.OnEvent(EventArg_Null.Object);
            m_IsProcess = true;
        }
    }
}
