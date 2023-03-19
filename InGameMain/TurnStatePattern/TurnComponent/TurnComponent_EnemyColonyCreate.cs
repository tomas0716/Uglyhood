using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent_EnemyColonyCreate : TComponent<EventArg_Null, EventArg_Null>
{
    private Transformer_Timer m_pTimer_NextEvent = new Transformer_Timer();

    public TurnComponent_EnemyColonyCreate()
    {
        TransformerEvent_Timer eventValue;
        eventValue = new TransformerEvent_Timer(0.15f);
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
        OutputLog.Log("TurnComponent_EnemyColonyCreate : OnEvent");
        InGameTurnLog.Log("TurnComponent_EnemyColonyCreate : OnEvent");

        InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.EnemyColonyCreate;
        EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.EnemyColonyCreate);

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        bool IsCreateEnemyColony = false;

        foreach (KeyValuePair<int, Slot> item in pDataStack.m_pSlotManager.GetSlotTable())
        {
            if (item.Value != null)
            {
                if (item.Value.GetSlotBlock() == null && item.Value.GetSlotFixObjectList().Count == 0)
                {
                    SlotFixObject_Unit pUnit = item.Value.GetParameta() as SlotFixObject_Unit;

                    if (pUnit != null)
                    {
                        int nEnemyColonyTableID = GameDefine.ms_nBasicEnemyColonyID;
                        if (pDataStack.m_EnemyColonyCreateTable.ContainsKey(item.Value) == true)
                        {
                            nEnemyColonyTableID = pDataStack.m_EnemyColonyCreateTable[item.Value];
                        }
                        ExcelData_EnemyColonyInfo pEnemyColonyInfo = ExcelDataManager.Instance.m_pEnemyColony.GetEnemyColonyInfo(nEnemyColonyTableID);
                        if (pEnemyColonyInfo == null)
                        {
                            pEnemyColonyInfo = ExcelDataManager.Instance.m_pEnemyColony.GetEnemyColonyInfo(GameDefine.ms_nBasicEnemyColonyID);
                        }

                        if (pDataStack.m_EnemyColonyTable.ContainsKey(item.Value.GetSlotIndex()) == false)
                        {
                            pDataStack.m_EnemyColonyTable.Remove(item.Value.GetSlotIndex());
                            item.Value.RemoveAllSlotFixObject();
                        }

                        if (pDataStack.m_EnemyColonyTable.ContainsKey(item.Value.GetSlotIndex()) == false)
                        {
                            IsCreateEnemyColony = true;

                            SlotFixObject_EnemyColony pSlotFixObject_EnemyColony = new SlotFixObject_EnemyColony(item.Value, item.Value.GetSlotIndex(), pEnemyColonyInfo, 1/*pUnit.GetUnitInfo().m_nColonyGenLevel*/);
                            item.Value.AddSlotFixObject(pSlotFixObject_EnemyColony);
                            pDataStack.m_EnemyColonyTable.Add(item.Value.GetSlotIndex(), pSlotFixObject_EnemyColony);

                            Helper.OnSoundPlay("INGAME_ENEMY_COLONY_GENERATE", false);
                        }
                    }
                }
            }
        }

        if (IsCreateEnemyColony == false)
        {
            EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

            OutputLog.Log("TurnComponent_EnemyColonyCreate : GetNextEvent().OnEvent(EventArg_Null.Object)");
            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
        else
        {
            m_pTimer_NextEvent.OnPlay();
        }
    }

    private void OnDone_TimerNextEvent(TransformerEvent eventValue)
    {
        EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

        OutputLog.Log("TurnComponent_EnemyColonyCreate : GetNextEvent().OnEvent(EventArg_Null.Object)");
        GetNextEvent().OnEvent(EventArg_Null.Object);
    }

    public int IsProcess()
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        foreach (KeyValuePair<int, Slot> item in pDataStack.m_pSlotManager.GetSlotTable())
        {
            if (item.Value != null)
            {
                if (item.Value.GetSlotBlock() == null && item.Value.GetSlotFixObjectList().Count == 0)
                {
                    SlotFixObject_Unit pUnit = item.Value.GetParameta() as SlotFixObject_Unit;

                    if (pUnit != null)
                    {
                        return 1;
                    }
                }
            }
        }

        return 0;
    }
}
