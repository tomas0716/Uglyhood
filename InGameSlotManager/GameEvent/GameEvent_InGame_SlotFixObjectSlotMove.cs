using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_InGame_SlotFixObjectSlotMove : GameEvent
{
    private SlotManager             m_pSlotManager          = null;
    private SlotFixObject           m_pSlotFixObject        = null;
    private Transformer_Vector3     m_pPos                  = null;
    private Slot                    m_pSlot_To              = null;

    public GameEvent_InGame_SlotFixObjectSlotMove(SlotManager pSlotManager, SlotFixObject pSlotFixObject, Slot pFrom, Slot pTo, float fMoveTime)
    {
        m_pSlotManager = pSlotManager;
        m_pSlotFixObject = pSlotFixObject;
        m_pSlotFixObject.SetMoving(true);
        m_pSlot_To = pTo;

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        pSlotManager.GetMainGame().OnSlotMvoe_SlotFixObject(pFrom, pTo, pSlotFixObject, fMoveTime);

        m_pPos = new Transformer_Vector3(pFrom.GetPosition());

        TransformerEvent_Vector3 eventValue;
        eventValue = new TransformerEvent_Vector3(fMoveTime, pTo.GetPosition(), pTo);
        m_pPos.AddEvent(eventValue);
        m_pPos.SetCallback(null, OnDone_Move);
        m_pPos.OnPlay();

        Helper.OnSoundPlay("INGAME_MINION_MOVE_PATH", false);
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
        m_pPos.Update(Time.deltaTime);
        Vector3 vPos = m_pPos.GetCurVector3();
        m_pSlotFixObject.SetPosition(vPos);
    }

    public void OnDone_Move(TransformerEvent eventValue)
    {
        m_pSlotFixObject.SetMoving(false);
        m_pSlotManager.OnDoneMoveAndCreate(m_pSlot_To);
        OnDone();
    }
}
