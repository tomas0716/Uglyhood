using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_ClearAtBreakColonyToGold : GameEvent
{
    private int m_nRemainColony = 0;
    public GameEvent_ClearAtBreakColonyToGold()
    {
        EspressoInfo.Instance.m_IsResultTextShowDone = true;

        Dictionary<int, SlotFixObject_EnemyColony> EnemyColony = new Dictionary<int, SlotFixObject_EnemyColony>();
        EnemyColony = DataStackManager.Instance.Find<MainGame_DataStack>().m_EnemyColonyTable;

        if (EnemyColony.Count != 0)
        {
            m_nRemainColony = EnemyColony.Count;

            foreach (KeyValuePair<int, SlotFixObject_EnemyColony> Colony in EnemyColony)
            {
                int nRandomNumber = Random.Range(3, 8);
                float fRandomNumber = (float)nRandomNumber * 0.1f;
                Colony.Value.ColonyToGold(fRandomNumber);
            }
        }
        else
        {
            EventDelegateManager.Instance.OnInGame_ShowStageClearReward();
            OnDone();
        }

        EventDelegateManager.Instance.OnEventInGame_BreakColonyToGold_MinusRenameColony += OnInGame_BreakColonyToGold_MinusRenameColony;
    }

    public override void OnDestroy()
    {
        EventDelegateManager.Instance.OnEventInGame_BreakColonyToGold_MinusRenameColony -= OnInGame_BreakColonyToGold_MinusRenameColony;
    }

    public override void Update()
    {

    }

    public void OnInGame_BreakColonyToGold_MinusRenameColony()
    {
        m_nRemainColony--;

        if (m_nRemainColony == 0)
        {
            EventDelegateManager.Instance.OnInGame_ShowStageClearReward();
            OnDone();
        }
    }
}
