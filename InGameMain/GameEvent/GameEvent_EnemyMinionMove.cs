using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_EnemyMinionMove : GameEvent
{
    private SlotFixObject_Minion    m_pMinion           = null;
    private Transformer_Vector3     m_pPos              = null;
    private bool                    m_IsShieldAttack    = false;

    public GameEvent_EnemyMinionMove(SlotFixObject_Minion pMinion, List<PathFind.Point> pointList, bool IsShieldAttack)
    {
        m_pMinion = pMinion;
        m_IsShieldAttack = IsShieldAttack;

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        m_pPos = new Transformer_Vector3(pMinion.GetSlot().GetPosition());

        Slot pMoveSlot = null;
        TransformerEvent_Vector3 eventValue;
        float fTime = 0;
        for (int i = 0; i < pointList.Count; ++i)
        {
            int nSlotIndex = Helper.GetSlotIndex(pointList[i].x, pointList[i].y);
            if (pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(nSlotIndex) == true)
            {
                fTime += GameDefine.ms_fEnemyOneBlockMoveTime;
                pMoveSlot = pDataStack.m_pSlotManager.GetSlotTable()[nSlotIndex];
                eventValue = new TransformerEvent_Vector3(fTime, pMoveSlot.GetPosition(), pMoveSlot);
                m_pPos.AddEvent(eventValue);
            }
        }

        m_pPos.SetCallback(null, OnDone_Move);
        m_pPos.OnPlay();

        switch (pMinion.GetMinionType())
        {
            case eMinionType.EnemyMinion:
                {
                    pDataStack.m_EnemyMinionTable.Remove(m_pMinion.GetSlot().GetSlotIndex());

                    Slot pOriginSlot = m_pMinion.GetSlot();
                    pOriginSlot.PopSlotFixObject(m_pMinion);

                    if (pMoveSlot != null)
                    {
                        pMoveSlot.AddSlotFixObject(m_pMinion);
                        m_pMinion.SetSlot(pMoveSlot);
                    }

                    if (pDataStack.m_EnemyMinionTable.ContainsKey(pMoveSlot.GetSlotIndex()) == true)
                    {
                        pDataStack.m_EnemyMinionTable.Remove(pMoveSlot.GetSlotIndex());
                    }
                    pDataStack.m_EnemyMinionTable.Add(pMoveSlot.GetSlotIndex(), m_pMinion);
                }
                break;
            case eMinionType.EnemySummonUnit:
                {
                    pDataStack.m_EnemySummonUnitTable.Remove(m_pMinion.GetSlot().GetSlotIndex());

                    Slot pOriginSlot = m_pMinion.GetSlot();
                    pOriginSlot.PopSlotFixObject(m_pMinion);

                    if (pMoveSlot != null)
                    {
                        pMoveSlot.AddSlotFixObject(m_pMinion);
                        m_pMinion.SetSlot(pMoveSlot);
                    }

                    if (pDataStack.m_EnemySummonUnitTable.ContainsKey(pMoveSlot.GetSlotIndex()) == true)
                    {
                        pDataStack.m_EnemySummonUnitTable.Remove(pMoveSlot.GetSlotIndex());
                    }
                    pDataStack.m_EnemySummonUnitTable.Add(pMoveSlot.GetSlotIndex(), m_pMinion);
                }
                break;
        }

        Helper.OnSoundPlay("INGAME_MINION_MOVE_PATH", false);
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
        m_pPos.Update(Time.deltaTime);
        Vector3 vPos = m_pPos.GetCurVector3();
        m_pMinion.SetPosition(vPos);
    }

    public void OnDone_Move(TransformerEvent eventValue)
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        if (m_IsShieldAttack == true)
        {    
            pDataStack.m_nCurrShildPoint -= 1;
            if(pDataStack.m_nCurrShildPoint < 0) pDataStack.m_nCurrShildPoint = 0;
            EventDelegateManager.Instance.OnInGame_ChangeShieldPoint(pDataStack.m_nCurrShildPoint);

            switch (m_pMinion.GetMinionType())
            {
                case eMinionType.EnemyMinion:
                    {
                        pDataStack.m_EnemyMinionTable.Remove(m_pMinion.GetSlot().GetSlotIndex());
                    }
                    break;

                case eMinionType.EnemySummonUnit:
                    {
                        pDataStack.m_EnemySummonUnitTable.Remove(m_pMinion.GetSlot().GetSlotIndex());
                    }
                    break;
            }

            m_pMinion.OnDead();

            Helper.OnSoundPlay("INGAME_SHIELD_POINT_HIT", false);
        }

        if (pDataStack.m_nCurrShildPoint > 0 && pDataStack.m_nCurrObjectiveCount <= 0 && pDataStack.m_EnemyMinionTable.Count == 0)
        {
            EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
        }
        else
        {
            AppInstance.Instance.m_pEventDelegateManager.OnInGame_EnemyMinionMoveAndCreateDone();
        }

        OnDone();
    }
}
