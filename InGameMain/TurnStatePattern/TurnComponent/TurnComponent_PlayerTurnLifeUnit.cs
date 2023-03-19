using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent_PlayerTurnLifeUnit : TComponent<EventArg_Null, EventArg_Null>
{
    private MainGame_DataStack  m_pMainGame_DataStatck  = null;
    private Transformer_Timer   m_pTimer_NextEvent      = new Transformer_Timer();

    private bool                m_IsActive              = false;
    private eOwner              m_eOwner                = eOwner.My;

    public TurnComponent_PlayerTurnLifeUnit(eOwner eOwn)
    {
        m_eOwner = eOwn;

        m_pMainGame_DataStatck = DataStackManager.Instance.Find<MainGame_DataStack>();

        TransformerEvent_Timer eventValue;
        eventValue = new TransformerEvent_Timer(1.5f);
        m_pTimer_NextEvent.AddEvent(eventValue);
        m_pTimer_NextEvent.SetCallback(null, OnDone_TimerNextEvent);
    }

    public override void OnDestroy()
    {
        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
    }

    public override void Update()
    {
        m_pTimer_NextEvent.Update(Time.deltaTime);
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        OutputLog.Log("TurnComponent_PlayerTurnLifeUnit : OnEvent");
        InGameTurnLog.Log("TurnComponent_PlayerTurnLifeUnit : OnEvent");

        m_IsActive = true;

        InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.PlayerTurnLifeUnit;
        EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.PlayerTurnLifeUnit);

        List<SlotFixObject_Unit> ProcessUnitList = new List<SlotFixObject_Unit>();

        foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pMainGame_DataStatck.m_PlayerSummonUnitTable)
        {
            if (item.Value.GetOwner() == m_eOwner && item.Value.DecreaseTransformTurnLife() == true)
            {
                ProcessUnitList.Add(item.Value);
            }
        }

        Dictionary<int, SlotFixObject_PlayerCharacter> characterTable = null;

        if (m_eOwner == eOwner.My)
        {
            characterTable = m_pMainGame_DataStatck.m_PlayerCharacterTable;
        }
        else
        {
            characterTable = m_pMainGame_DataStatck.m_OtherPlayerCharacterTable;
        }

        foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in characterTable)
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
            m_IsActive = false;
            EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

            OutputLog.Log("TurnComponent_PlayerTurnLifeUnit : GetNextEvent().OnEvent(EventArg_Null.Object)");
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

                                        m_pMainGame_DataStatck.m_PlayerSummonUnitTable.Remove(pUnit.GetSlot().GetSlotIndex());
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

        OutputLog.Log("TurnComponent_PlayerTurnLifeUnit : GetNextEvent().OnEvent(EventArg_Null.Object)");

        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone += OnInGame_CheckRemoveSlotDone;

        EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0.2f);
    }

    public void OnInGame_CheckRemoveSlotDone()
    {
        OutputLog.Log("TurnComponent_PlayerTurnLifeUnit : OnInGame_CheckRemoveSlotDone");

        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;

        if (m_IsActive == true)
        {
            m_IsActive = false;
            OutputLog.Log("TurnComponent_PlayerCharacterAttack : OnInGame_CheckRemoveSlotDone active");

            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
    }
}
