using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;

public class GameEvent_BlockShuffleSkill : GameEvent
{
    private MainGame_DataStack          m_pDataStack        = null;
    private SlotFixObject_Unit          m_pUnit             = null;
    private ExcelData_ActionInfo        m_pActionInfo       = null;

    private Transformer_Timer           m_pTimer_Shuffle    = new Transformer_Timer();

    private object                      m_pLockObject       = new object();

    public GameEvent_BlockShuffleSkill(SlotFixObject_Unit pUnit, ExcelData_ActionInfo pActionInfo)
    {
        m_pUnit = pUnit;
        m_pActionInfo = pActionInfo;

        m_pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        int [] nBlockRate = pActionInfo.m_nBlockRate;

        if (pUnit != null && pUnit.GetObjectType() == eObjectType.Character)
        {
            SlotFixObject_PlayerCharacter pPlayerCharacter = pUnit as SlotFixObject_PlayerCharacter;

            if (pPlayerCharacter != null)
            {
                int nLevel = pPlayerCharacter.GetLevel();
                ExcelData_Action_LevelUpInfo pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(pUnit.GetUnitInfo().m_nActiveSkill_ActionTableID, nLevel);

                if (pActionLevelUpInfo != null)
                {
                    nBlockRate = pActionLevelUpInfo.m_nChange_BlockRate;
                }
            }
        }

        m_pDataStack.m_pSlotManager.ClearAppearBlockTypeList();

        for (int i = 0; i < GameDefine.ms_nBlockCount; ++i)
        {
            for (int j = 0; j < nBlockRate[i]; ++j)
            {
                m_pDataStack.m_pSlotManager.AddAppearBlockType(eBlockType.Block_Start + i);
            }
        }

        m_pDataStack.m_pSlotManager.OnAppaerBlockTypeEnd();

        InGameInfo.Instance.m_IsInGameClick = false;
        m_pDataStack.m_pSlotManager.OnShuffle();

        TransformerEvent eventValue;

        m_pTimer_Shuffle.OnReset();
        eventValue = new TransformerEvent_Timer(0.85f);
        m_pTimer_Shuffle.AddEvent(eventValue);
        m_pTimer_Shuffle.SetCallback(null, OnDone_Timer_Shuffle);
        m_pTimer_Shuffle.OnPlay();
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
        m_pTimer_Shuffle.Update(Time.deltaTime);
    }

    private void OnDone_Timer_Shuffle(TransformerEvent eventValue)
    {
        EventDelegateManager.Instance.OnInGame_BlockShuffleSkill(m_pUnit, m_pActionInfo);
        OnDone();
    }
}
