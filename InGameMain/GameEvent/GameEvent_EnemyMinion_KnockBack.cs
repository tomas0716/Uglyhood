using System.Collections.Generic;
using UnityEngine;

public class GameEvent_EnemyMinion_KnockBack : GameEvent
{
    private SlotFixObject_Minion            m_pMinion               = null;
    private eKnockBackStatusEffect_Direct   m_eDirect               = eKnockBackStatusEffect_Direct.Up;
    private int                             m_nKnockBackValue       = 0;

    private MainGame_DataStack              m_pDataStack            = null;

    private Transformer_Vector3             m_pPos_KnockBackMove    = null;

    public GameEvent_EnemyMinion_KnockBack(SlotFixObject_Minion pMinion, eKnockBackStatusEffect_Direct eDirect, int nKnockBackValue)
    {
        m_pMinion = pMinion;
        m_eDirect = eDirect;
        m_nKnockBackValue = nKnockBackValue;

        int nX = pMinion.GetSlot().GetX();
        int nY = pMinion.GetSlot().GetY();
        Vector3 vCurrPos = pMinion.GetSlot().GetPosition();
        m_pPos_KnockBackMove = new Transformer_Vector3(vCurrPos);

        m_pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
        Dictionary<int,Slot> slotTable = m_pDataStack.m_pSlotManager.GetSlotTable();

        float fReactionInterval = 0;

        List<Slot> KnockDonwSlotList = new List<Slot>();

        switch (m_eDirect)
        {
            case eKnockBackStatusEffect_Direct.Up:
                {
                    fReactionInterval = 128 * 0.6f * InGameInfo.Instance.m_fInGameScale;
                    int nMax = InGameSetting.Instance.m_pInGameSettingData.m_vGrid.y - (nY + 1);
                    nMax = nMax >= nKnockBackValue ? nKnockBackValue : nMax;

                    for (int i = nY + 1; i <= nY + nMax; ++i)
                    {
                        if (slotTable.ContainsKey(Helper.GetSlotIndex(nX, i)) == true)
                        {
                            Slot pSlot = slotTable[Helper.GetSlotIndex(nX, i)];

                            if (pSlot.GetSlotBlock() != null)
                            {
                                KnockDonwSlotList.Add(pSlot);
                            }
                            else if (pSlot.GetLastSlotFixObject() != null)
                            {
                                eSlotFixObjectType eFixObjectType = pSlot.GetLastSlotFixObject().GetSlotFixObjectType();
                                if (eFixObjectType == eSlotFixObjectType.Espresso)
                                {
                                    SlotFixObject_Espresso pEspresso = pSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                                    if (pEspresso.GetObjectType() == eObjectType.EnemyColony)
                                    {
                                        KnockDonwSlotList.Add(pSlot);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                break;

            case eKnockBackStatusEffect_Direct.Down:
                {
                    fReactionInterval = -128 * 0.6f * InGameInfo.Instance.m_fInGameScale;
                    int nMax = nY - 1;
                    nMax = nMax >= nKnockBackValue ? nKnockBackValue : nMax;

                    for (int i = nY - 1; i >= nY - nMax; --i)
                    {
                        if (slotTable.ContainsKey(Helper.GetSlotIndex(nX, i)) == true)
                        {
                            Slot pSlot = slotTable[Helper.GetSlotIndex(nX, i)];

                            if (pSlot.GetSlotBlock() != null)
                            {
                                KnockDonwSlotList.Add(pSlot);
                            }
                            else if (pSlot.GetLastSlotFixObject() != null)
                            {
                                eSlotFixObjectType eFixObjectType = pSlot.GetLastSlotFixObject().GetSlotFixObjectType();
                                if (eFixObjectType == eSlotFixObjectType.Espresso)
                                {
                                    SlotFixObject_Espresso pEspresso = pSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                                    if (pEspresso.GetObjectType() == eObjectType.EnemyColony)
                                    {
                                        KnockDonwSlotList.Add(pSlot);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                break;
        }

        if (KnockDonwSlotList.Count != 0)
        {
            List<SlotFixObject> slotFixObjectList = m_pMinion.GetSlot().GetSlotFixObjectList();

            foreach (SlotFixObject pSlotFixObject in slotFixObjectList)
            {
				eSlotFixObjectType eFixObjectType = pSlotFixObject.GetSlotFixObjectType();
				if (eFixObjectType == eSlotFixObjectType.Espresso)
				{
					SlotFixObject_Espresso pEspresso = pSlotFixObject as SlotFixObject_Espresso;

					if (pEspresso.GetObjectType() == eObjectType.EnemyColony)
					{
						m_pDataStack.m_EnemyColonyTable.Remove(pEspresso.GetSlot().GetSlotIndex());
                        pEspresso.OnDead();
                        break;
					}
				}
			}

            TransformerEvent_Vector3 eventValue;
            float fMoveTime = GameDefine.ms_fBlockMoveTime;
            float fEntireTime = 0;
            for (int i = 0; i < KnockDonwSlotList.Count; ++i)
            {
                Slot pSlot = KnockDonwSlotList[i];

                if (i == KnockDonwSlotList.Count - 1)
                {
                    Vector3 vPos = pSlot.GetPosition();
                    vPos.y += fReactionInterval;
                    eventValue = new TransformerEvent_Vector3(fEntireTime + fMoveTime * 1.6f, vPos);
                    m_pPos_KnockBackMove.AddEvent(eventValue);
                    eventValue = new TransformerEvent_Vector3(fEntireTime + fMoveTime * 1.6f, pSlot.GetPosition(), pSlot);
                    m_pPos_KnockBackMove.AddEvent(eventValue);
                }
                else
                {
                    eventValue = new TransformerEvent_Vector3(fEntireTime + fMoveTime, pSlot.GetPosition(), pSlot);
                    m_pPos_KnockBackMove.AddEvent(eventValue);
                }

                fEntireTime += fMoveTime;
                fMoveTime *= 0.95f;
            }

            if (fEntireTime > m_pDataStack.m_fKnockBackDelayTime)
            {
                m_pDataStack.m_fKnockBackDelayTime = fEntireTime;
            }

            m_pPos_KnockBackMove.SetCallback(OnOneEventDone_Pos, OnDone_Pos);
            m_pPos_KnockBackMove.OnPlay();
        }
        else
        {
            OnDone();
        }
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
        m_pPos_KnockBackMove.Update(Time.deltaTime);
        Vector3 vPos = m_pPos_KnockBackMove.GetCurVector3();
        m_pMinion.SetForcePosition(vPos);
    }

    private void OnOneEventDone_Pos(int nIndex, TransformerEvent eventValue)
    {
        if (eventValue.m_pParameta != null)
        {
            Slot pSlot = eventValue.m_pParameta as Slot;

            if (pSlot != null)
            {
                if (pSlot.GetSlotBlock() != null)
                {
                    pSlot.OnSlotBlockRemove();
                }
                else if (pSlot.GetLastSlotFixObject() != null)
                {
                    eSlotFixObjectType eFixObjectType = pSlot.GetLastSlotFixObject().GetSlotFixObjectType();
                    if (eFixObjectType == eSlotFixObjectType.Espresso)
                    {
                        SlotFixObject_Espresso pEspresso = pSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                        if (pEspresso.GetObjectType() == eObjectType.EnemyColony)
                        {
                            m_pDataStack.m_EnemyColonyTable.Remove(pEspresso.GetSlot().GetSlotIndex());
                            pEspresso.OnDead();
                        }
                    }
                }

                switch (m_pMinion.GetMinionType())
                {
                    case eMinionType.EnemyMinion:
                        {
                            m_pDataStack.m_EnemyMinionTable.Remove(m_pMinion.GetSlot().GetSlotIndex());

                            Slot pOriginSlot = m_pMinion.GetSlot();
                            pOriginSlot.PopSlotFixObject(m_pMinion);

                            if (pSlot != null)
                            {
                                pSlot.AddSlotFixObject(m_pMinion);
                                m_pMinion.SetSlot(pSlot);
                            }

                            if (m_pDataStack.m_EnemyMinionTable.ContainsKey(pSlot.GetSlotIndex()) == true)
                            {
                                m_pDataStack.m_EnemyMinionTable.Remove(pSlot.GetSlotIndex());
                            }
                            m_pDataStack.m_EnemyMinionTable.Add(pSlot.GetSlotIndex(), m_pMinion);
                        }
                        break;

                    case eMinionType.PlayerSummonUnit:
                        {
                            m_pDataStack.m_PlayerSummonUnitTable.Remove(m_pMinion.GetSlot().GetSlotIndex());

                            Slot pOriginSlot = m_pMinion.GetSlot();
                            pOriginSlot.PopSlotFixObject(m_pMinion);

                            if (pSlot != null)
                            {
                                pSlot.AddSlotFixObject(m_pMinion);
                                m_pMinion.SetSlot(pSlot);
                            }

                            if (m_pDataStack.m_PlayerSummonUnitTable.ContainsKey(pSlot.GetSlotIndex()) == true)
                            {
                                m_pDataStack.m_PlayerSummonUnitTable.Remove(pSlot.GetSlotIndex());
                            }
                            m_pDataStack.m_PlayerSummonUnitTable.Add(pSlot.GetSlotIndex(), m_pMinion);
                        }
                        break;

                    case eMinionType.EnemySummonUnit:
                        {
                            m_pDataStack.m_EnemySummonUnitTable.Remove(m_pMinion.GetSlot().GetSlotIndex());

                            Slot pOriginSlot = m_pMinion.GetSlot();
                            pOriginSlot.PopSlotFixObject(m_pMinion);

                            if (pSlot != null)
                            {
                                pSlot.AddSlotFixObject(m_pMinion);
                                m_pMinion.SetSlot(pSlot);
                            }

                            if (m_pDataStack.m_EnemySummonUnitTable.ContainsKey(pSlot.GetSlotIndex()) == true)
                            {
                                m_pDataStack.m_EnemySummonUnitTable.Remove(pSlot.GetSlotIndex());
                            }
                            m_pDataStack.m_EnemySummonUnitTable.Add(pSlot.GetSlotIndex(), m_pMinion);
                        }
                        break;
                }
            }
        }
    }

    private void OnDone_Pos(TransformerEvent eventValue)
    {
        if (eventValue.m_pParameta != null)
        {
            Slot pSlot = eventValue.m_pParameta as Slot;

            if (pSlot != null)
            {
                if (pSlot.GetSlotBlock() != null)
                {
                    pSlot.OnSlotBlockRemove();
                }
                else if (pSlot.GetLastSlotFixObject() != null)
                {
                    eSlotFixObjectType eFixObjectType = pSlot.GetLastSlotFixObject().GetSlotFixObjectType();
                    if (eFixObjectType == eSlotFixObjectType.Espresso)
                    {
                        SlotFixObject_Espresso pEspresso = pSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                        if (pEspresso.GetObjectType() == eObjectType.EnemyColony)
                        {
                            m_pDataStack.m_EnemyColonyTable.Remove(pEspresso.GetSlot().GetSlotIndex());
                            pEspresso.OnDead();
                        }
                    }
                }

                switch (m_pMinion.GetMinionType())
                {
                    case eMinionType.EnemyMinion:
                        {
                            m_pDataStack.m_EnemyMinionTable.Remove(m_pMinion.GetSlot().GetSlotIndex());

                            Slot pOriginSlot = m_pMinion.GetSlot();
                            pOriginSlot.PopSlotFixObject(m_pMinion);

                            if (pSlot != null)
                            {
                                pSlot.AddSlotFixObject(m_pMinion);
                                m_pMinion.SetSlot(pSlot);
                            }

                            if (m_pDataStack.m_EnemyMinionTable.ContainsKey(pSlot.GetSlotIndex()) == true)
                            {
                                m_pDataStack.m_EnemyMinionTable.Remove(pSlot.GetSlotIndex());
                            }
                            m_pDataStack.m_EnemyMinionTable.Add(pSlot.GetSlotIndex(), m_pMinion);
                        }
                        break;

                    case eMinionType.PlayerSummonUnit:
                        {
                            m_pDataStack.m_PlayerSummonUnitTable.Remove(m_pMinion.GetSlot().GetSlotIndex());

                            Slot pOriginSlot = m_pMinion.GetSlot();
                            pOriginSlot.PopSlotFixObject(m_pMinion);

                            if (pSlot != null)
                            {
                                pSlot.AddSlotFixObject(m_pMinion);
                                m_pMinion.SetSlot(pSlot);
                            }

                            if (m_pDataStack.m_PlayerSummonUnitTable.ContainsKey(pSlot.GetSlotIndex()) == true)
                            {
                                m_pDataStack.m_PlayerSummonUnitTable.Remove(pSlot.GetSlotIndex());
                            }
                            m_pDataStack.m_PlayerSummonUnitTable.Add(pSlot.GetSlotIndex(), m_pMinion);
                        }
                        break;

                    case eMinionType.EnemySummonUnit:
                        {
                            m_pDataStack.m_EnemySummonUnitTable.Remove(m_pMinion.GetSlot().GetSlotIndex());

                            Slot pOriginSlot = m_pMinion.GetSlot();
                            pOriginSlot.PopSlotFixObject(m_pMinion);

                            if (pSlot != null)
                            {
                                pSlot.AddSlotFixObject(m_pMinion);
                                m_pMinion.SetSlot(pSlot);
                            }

                            if (m_pDataStack.m_EnemySummonUnitTable.ContainsKey(pSlot.GetSlotIndex()) == true)
                            {
                                m_pDataStack.m_EnemySummonUnitTable.Remove(pSlot.GetSlotIndex());
                            }
                            m_pDataStack.m_EnemySummonUnitTable.Add(pSlot.GetSlotIndex(), m_pMinion);
                        }
                        break;
                }
            }
        }

        OnDone();
    }
}
