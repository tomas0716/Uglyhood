using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent_EnemyTurnLifeUnit : TComponent<EventArg_Null, EventArg_Null>
{
    private MainGame_DataStack m_pMainGame_DataStatck = null;
    private Transformer_Timer m_pTimer_NextEvent = new Transformer_Timer();

    public TurnComponent_EnemyTurnLifeUnit()
    {
        m_pMainGame_DataStatck = DataStackManager.Instance.Find<MainGame_DataStack>();

        TransformerEvent_Timer eventValue;
        eventValue = new TransformerEvent_Timer(1.5f);
        m_pTimer_NextEvent.AddEvent(eventValue);
        m_pTimer_NextEvent.SetCallback(null, OnDone_TimerNextEvent);
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
        m_pTimer_NextEvent.Update(Time.deltaTime);
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        OutputLog.Log("TurnComponent_EnemyTurnLifeUnit : OnEvent");
        InGameTurnLog.Log("TurnComponent_EnemyTurnLifeUnit : OnEvent");

        InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.EnemyTurnLifeUnit;
        EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.EnemyTurnLifeUnit);

        List<SlotFixObject_Unit> ProcessUnitList = new List<SlotFixObject_Unit>();

        foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pMainGame_DataStatck.m_EnemySummonUnitTable)
        {
            if (item.Value.DecreaseTransformTurnLife() == true)
            {
                ProcessUnitList.Add(item.Value);
            }
        }

        foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pMainGame_DataStatck.m_EnemyMinionTable)
        {
            if (item.Value.GetDamageEffectType_TransformType() != eDamageEffectType.None)
            {
                if (item.Value.DecreaseTransformTurnLife() == true)
                {
                    ProcessUnitList.Add(item.Value);
                }
            }
        }

        if (ProcessUnitList.Count == 0)
        {
            EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

            OutputLog.Log("TurnComponent_EnemyTurnLifeUnit : GetNextEvent().OnEvent(EventArg_Null.Object)");
            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
        else
        {
            foreach (SlotFixObject_Unit pUnit in ProcessUnitList)
            {
                switch (pUnit.GetDamageEffectType_TransformType())
                {
                    case eDamageEffectType.UnitSummon:
                        {
                            switch (pUnit.GetObjectType())
                            {
                                case eObjectType.Character:
                                    {
                                    }
                                    break;

                                case eObjectType.Minion:
                                case eObjectType.EnemyBoss:
                                    {
                                        ParticleInfo pParticleInfo = ParticleManager.Instance.LoadParticleSystem(pUnit.GetTransformRestoreEffect(), Vector3.zero);
                                        pParticleInfo.SetScale(InGameInfo.Instance.m_fInGameScale);
                                        pParticleInfo.GetGameObject().transform.localPosition = pUnit.GetGameObject().transform.position + new Vector3(0, 0, -(int)ePlaneOrder.Fx_TopLayer);

                                        m_pMainGame_DataStatck.m_EnemySummonUnitTable.Remove(pUnit.GetSlot().GetSlotIndex());
                                        pUnit.OnDead();
                                    }
                                    break;
                            }
                        }
                        break;

                    case eDamageEffectType.UnitTransform:
                    case eDamageEffectType.UnitClone:
                        {
                            pUnit.OnTransformUnitRestore();
                        }
                        break;
                }
            }

            m_pTimer_NextEvent.OnPlay();
        }
    }

    private void OnDone_TimerNextEvent(TransformerEvent eventValue)
    {
        EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

        OutputLog.Log("TurnComponent_EnemyTurnLifeUnit : GetNextEvent().OnEvent(EventArg_Null.Object)");
        GetNextEvent().OnEvent(EventArg_Null.Object);
    }

    public int IsProcess()
    {
        List<SlotFixObject_Unit> ProcessUnitList = new List<SlotFixObject_Unit>();

        foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pMainGame_DataStatck.m_EnemySummonUnitTable)
        {
            if (item.Value.VirtualDecreaseTransformTurnLife() == true)
            {
                ProcessUnitList.Add(item.Value);
            }
        }

        foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pMainGame_DataStatck.m_EnemyMinionTable)
        {
            if (item.Value.GetDamageEffectType_TransformType() != eDamageEffectType.None)
            {
                if (item.Value.VirtualDecreaseTransformTurnLife() == true)
                {
                    ProcessUnitList.Add(item.Value);
                }
            }
        }

        if (ProcessUnitList.Count != 0)
        {
            return 1;
        }

        return 0;
    }
}
