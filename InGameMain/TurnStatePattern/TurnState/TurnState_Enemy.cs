using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnState_Enemy : TComponent<EventArg_Null, EventArg_Null>
{
    private TurnComponent_EnemyTurnLifeUnit         m_pTurnComponent_EnemyTurnLifeUnit          = new TurnComponent_EnemyTurnLifeUnit();
    private TurnComponent_EnemyMoveAndSpawn         m_pTurnComponent_EnemyMoveAndSpawn          = new TurnComponent_EnemyMoveAndSpawn();
    private TurnComponent_EnemyMinionBossIntro      m_pTurnComponent_EnemyMinionBossIntro       = new TurnComponent_EnemyMinionBossIntro();
    private TurnComponent_EnemyStatusEffect_Buff    m_pTurnComponent_EnemyStatusEffect_Buff     = new TurnComponent_EnemyStatusEffect_Buff();
    private TurnComponent_EnemyColonyEffect         m_pTurnComponent_EnemyColonyEffect          = new TurnComponent_EnemyColonyEffect();
    private TurnComponent_EnemyMinionAttack         m_pTurnComponent_EnemyMinionAttack          = new TurnComponent_EnemyMinionAttack();
    private TurnComponent_EnemyColonyCreate         m_pTurnComponent_EnemyColonyCreate          = new TurnComponent_EnemyColonyCreate();
    private TurnComponent_EnemyStatusEffect_Debuff  m_pTurnComponent_EnemyStatusEffect_Debuff   = new TurnComponent_EnemyStatusEffect_Debuff();

    private TEventDelegate<EventArg_Null>           m_pDone                                     = new TEventDelegate<EventArg_Null>();

    private bool                                    m_IsProcess                                 = false;

    public TurnState_Enemy()
    {
        m_pDone.SetFunc(OnDone);

        m_pTurnComponent_EnemyTurnLifeUnit.SetNextEvent(m_pTurnComponent_EnemyMoveAndSpawn);
        m_pTurnComponent_EnemyMoveAndSpawn.SetNextEvent(m_pTurnComponent_EnemyMinionBossIntro);
        m_pTurnComponent_EnemyMinionBossIntro.SetNextEvent(m_pTurnComponent_EnemyStatusEffect_Buff);
        m_pTurnComponent_EnemyStatusEffect_Buff.SetNextEvent(m_pTurnComponent_EnemyColonyEffect);
        m_pTurnComponent_EnemyColonyEffect.SetNextEvent(m_pTurnComponent_EnemyMinionAttack);
        m_pTurnComponent_EnemyMinionAttack.SetNextEvent(m_pTurnComponent_EnemyColonyCreate);
        m_pTurnComponent_EnemyColonyCreate.SetNextEvent(m_pTurnComponent_EnemyStatusEffect_Debuff);
        m_pTurnComponent_EnemyStatusEffect_Debuff.SetNextEvent(m_pDone);
    }

    public override void OnDestroy()
    {
        m_pTurnComponent_EnemyTurnLifeUnit.OnDestroy();
        m_pTurnComponent_EnemyMoveAndSpawn.OnDestroy();
        m_pTurnComponent_EnemyMinionBossIntro.OnDestroy();
        m_pTurnComponent_EnemyStatusEffect_Buff.OnDestroy();
        m_pTurnComponent_EnemyColonyEffect.OnDestroy();
        m_pTurnComponent_EnemyMinionAttack.OnDestroy();
        m_pTurnComponent_EnemyColonyCreate.OnDestroy();
        m_pTurnComponent_EnemyStatusEffect_Debuff.OnDestroy();
    }

    public override void Update()
    {
        m_pTurnComponent_EnemyTurnLifeUnit.Update();
        m_pTurnComponent_EnemyMoveAndSpawn.Update();
        m_pTurnComponent_EnemyMinionBossIntro.Update();
        m_pTurnComponent_EnemyStatusEffect_Buff.Update();
        m_pTurnComponent_EnemyColonyEffect.Update();
        m_pTurnComponent_EnemyMinionAttack.Update();
        m_pTurnComponent_EnemyColonyCreate.Update();
        m_pTurnComponent_EnemyStatusEffect_Debuff.Update();
    }

    public override void LateUpdate()
    {
        m_pTurnComponent_EnemyTurnLifeUnit.LateUpdate();
        m_pTurnComponent_EnemyMoveAndSpawn.LateUpdate();
        m_pTurnComponent_EnemyMinionBossIntro.LateUpdate();
        m_pTurnComponent_EnemyStatusEffect_Buff.LateUpdate();
        m_pTurnComponent_EnemyColonyEffect.LateUpdate();
        m_pTurnComponent_EnemyMinionAttack.LateUpdate();
        m_pTurnComponent_EnemyColonyCreate.LateUpdate();
        m_pTurnComponent_EnemyStatusEffect_Debuff.LateUpdate();
    }

    private void OnDone(EventArg_Null Arg)
    {
        OutputLog.Log("TurnState_Enemy : OnDone");

        m_IsProcess = false;
        GetNextEvent().OnEvent(EventArg_Null.Object);
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        OutputLog.Log("TurnState_Enemy : OnEvent");

        if (m_IsProcess == false)
        {
            m_IsProcess = true;
            DataStackManager.Instance.Find<MainGame_DataStack>().m_nEnemyTurnCount++;

            int nActiveCount = 0;
            nActiveCount += m_pTurnComponent_EnemyTurnLifeUnit.IsProcess();
            nActiveCount += m_pTurnComponent_EnemyMoveAndSpawn.IsProcess();
            nActiveCount += m_pTurnComponent_EnemyMinionBossIntro.IsProcess();
            nActiveCount += m_pTurnComponent_EnemyStatusEffect_Buff.IsProcess();
            nActiveCount += m_pTurnComponent_EnemyColonyEffect.IsProcess();
            nActiveCount += m_pTurnComponent_EnemyMinionAttack.IsProcess();
            nActiveCount += m_pTurnComponent_EnemyColonyCreate.IsProcess();
            nActiveCount += m_pTurnComponent_EnemyStatusEffect_Debuff.IsProcess();

            if (nActiveCount == 0)
            {
                EspressoInfo.Instance.m_IsPVE_EnemyTurnNoticePass = true;
                m_pTurnComponent_EnemyTurnLifeUnit.OnEvent(EventArg_Null.Object);
            }
            else
            {
                EspressoInfo.Instance.m_IsPVE_EnemyTurnNoticePass = false;
                EventDelegateManager.Instance.OnInGame_PVE_EnemyTurnNotice();
                AppInstance.Instance.StartCoroutine(Co_OnUseItemEffect());
            }
        }

        IEnumerator Co_OnUseItemEffect()
        {
            yield return new WaitForSeconds(0.75f);

            m_pTurnComponent_EnemyTurnLifeUnit.OnEvent(EventArg_Null.Object);
        }
    }
}
