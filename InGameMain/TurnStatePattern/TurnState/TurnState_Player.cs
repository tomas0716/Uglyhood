using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnState_Player : TComponent<EventArg_Null, EventArg_Null>
{   
    private TurnComponent_PlayerTurnLifeUnit        m_pTurnComponent_PlayerTurnLifeUnit         = null;
    private TurnComponent_BoosterEffect_Passive     m_pTurnComponent_BoosterEffect_Passive      = null;
    private TurnComponent_PlayerStatusEffect_Buff   m_pTurnComponent_PlayerStatusEffect_Buff    = null;
    private TurnComponent_PlayerMinionAttack        m_pTurnComponent_PlayerMinionAttack         = null;
    private TurnComponent_BlockMatch                m_pTurnComponent_BlockMatch                 = null;
    private TurnComponent_PlayerStatusEffect_Debuff m_pTurnComponent_PlayerStatusEffect_Debuff  = null;

    private TEventDelegate<EventArg_Null>           m_pDone                                     = new TEventDelegate<EventArg_Null>();

    private bool                                    m_IsProcess                                 = false;
    private eOwner                                  m_eOwner                                    = eOwner.My;


    public TurnState_Player(eOwner eOwner = eOwner.My)
    {
        m_eOwner = eOwner;

        m_pTurnComponent_PlayerTurnLifeUnit = new TurnComponent_PlayerTurnLifeUnit(m_eOwner);
        m_pTurnComponent_BoosterEffect_Passive = new TurnComponent_BoosterEffect_Passive(m_eOwner);
        m_pTurnComponent_PlayerStatusEffect_Buff = new TurnComponent_PlayerStatusEffect_Buff(m_eOwner);
        m_pTurnComponent_PlayerMinionAttack = new TurnComponent_PlayerMinionAttack(m_eOwner);
        m_pTurnComponent_BlockMatch = new TurnComponent_BlockMatch(m_eOwner);
        m_pTurnComponent_PlayerStatusEffect_Debuff = new TurnComponent_PlayerStatusEffect_Debuff(m_eOwner);

        m_pDone.SetFunc(OnDone);

        m_pTurnComponent_PlayerTurnLifeUnit.SetNextEvent(m_pTurnComponent_PlayerStatusEffect_Buff);
        m_pTurnComponent_PlayerStatusEffect_Buff.SetNextEvent(m_pTurnComponent_BoosterEffect_Passive);
        m_pTurnComponent_BoosterEffect_Passive.SetNextEvent(m_pTurnComponent_PlayerMinionAttack);
        m_pTurnComponent_PlayerMinionAttack.SetNextEvent(m_pTurnComponent_BlockMatch);
        m_pTurnComponent_BlockMatch.SetNextEvent(m_pTurnComponent_PlayerStatusEffect_Debuff);
        m_pTurnComponent_PlayerStatusEffect_Debuff.SetNextEvent(m_pDone);
    }

    public override void OnDestroy()
    {
        m_pTurnComponent_PlayerTurnLifeUnit.OnDestroy();
        m_pTurnComponent_BoosterEffect_Passive.OnDestroy();
        m_pTurnComponent_PlayerStatusEffect_Buff.OnDestroy();
        m_pTurnComponent_PlayerMinionAttack.OnDestroy();
        m_pTurnComponent_BlockMatch.OnDestroy();
        m_pTurnComponent_PlayerStatusEffect_Debuff.OnDestroy();
    }

    public override void Update()
    {
        m_pTurnComponent_PlayerTurnLifeUnit.Update();
        m_pTurnComponent_BoosterEffect_Passive.Update();
        m_pTurnComponent_PlayerStatusEffect_Buff.Update();
        m_pTurnComponent_PlayerMinionAttack.Update();
        m_pTurnComponent_BlockMatch.Update();
        m_pTurnComponent_PlayerStatusEffect_Debuff.Update();
    }

    public override void LateUpdate()
    {
        m_pTurnComponent_PlayerTurnLifeUnit.LateUpdate();
        m_pTurnComponent_BoosterEffect_Passive.LateUpdate();
        m_pTurnComponent_PlayerStatusEffect_Buff.LateUpdate();
        m_pTurnComponent_PlayerMinionAttack.LateUpdate();
        m_pTurnComponent_BlockMatch.LateUpdate();
        m_pTurnComponent_PlayerStatusEffect_Debuff.LateUpdate();
    }

    private void OnDone(EventArg_Null Arg)
    {
        OutputLog.Log("TurnState_Player : OnDone");

        // 플레이어 턴 카운트 증가

        if(m_eOwner == eOwner.My && InGameInfo.Instance.m_eCurrGameResult == eGameResult.None)
        {
            DataStackManager.Instance.Find<MainGame_DataStack>().m_nPlayerTurnCount++;
            EventDelegateManager.Instance.OnInGame_AddTurnCount();
            EventDelegateManager.Instance.OnInGame_ChangeClearStarsInGame();
        }
        EventDelegateManager.Instance.OnInGame_SetADBoosterItemDrawIcon();

        m_IsProcess = false;
        GetNextEvent().OnEvent(EventArg_Null.Object);
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        if (m_IsProcess == false)
        {
            m_IsProcess = true;
            OutputLog.Log("TurnState_Player : OnEvent");
            m_pTurnComponent_PlayerTurnLifeUnit.OnEvent(EventArg_Null.Object);
        }
    }
}
