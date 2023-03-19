using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_ClearAtSpecialBlockExplosion : GameEvent
{
    public GameEvent_ClearAtSpecialBlockExplosion()
    {
        OnRemoveSpecialBlock();

        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone += OnInGame_CheckRemoveSlotDone;
    }

    public override void OnDestroy()
    {
        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
    }

    public override void Update()
    {

    }

    private void OnRemoveSpecialBlock()
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        bool IsExistSpecialBlock = false;

        Dictionary<int, Slot> slotTable = pDataStack.m_pSlotManager.GetSlotTable();

        foreach (KeyValuePair<int, Slot> item in slotTable)
        {
            if (item.Value.GetSlotBlock() != null && item.Value.GetSlotBlock().GetSpecialItem() != eSpecialItem.None)
            {
                SlotFixObject_Obstacle pObstacle_OnlyMineBreak = item.Value.FindFixObject_SlotDyingAtOnlyMineBreak();

                if (pObstacle_OnlyMineBreak == null)
                {
                    IsExistSpecialBlock = true;
                    pDataStack.m_pSlotManager.RemoveSpecialItemBlock(item.Value);
                }
            }
        }

        if (IsExistSpecialBlock == false)
        {
            //EventDelegateManager.Instance.OnInGame_ShowStageClearReward();
            GameEvent_ClearAtBreakColonyToGold pGameEvent = new GameEvent_ClearAtBreakColonyToGold();
            GameEventManager.Instance.AddGameEvent(pGameEvent);

            OnDone();
        }
    }

    public void OnInGame_CheckRemoveSlotDone()
    {
        OnRemoveSpecialBlock();
    }
}
