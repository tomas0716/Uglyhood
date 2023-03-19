using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotFixObject_Cloud : SlotFixObject_Obstacle
{
    public SlotFixObject_Cloud(Slot pSlot) : base(pSlot, eSlotFixObjectObstacleType.Cloud)
    {
        m_nLayer = 100;

        m_IsSlotDyingAtOnlyMineBreak = false;
        m_IsSlotBlockNoSwap = true;
        m_IsSlotBlockExist = true;
        m_IsSlotBlockMatchPossible = false;
        m_IsSlotBlockMatchInPossibleAtCheckRemovePossible = true;
        m_IsNeighborSlotBlockDyingAtBreak = false;
        m_IsMoveAndCreateInclude = true;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void Update(float fDeltaTime)
    {
    }

    public override void SetPosition(Vector2 vPos)
    {
    }

    public override void OnBreak()
    {
    }

    public override void SetHighlight(int nDepth = 0)
    {
    }

    public override void ClearHighlight()
    {
    }
}
