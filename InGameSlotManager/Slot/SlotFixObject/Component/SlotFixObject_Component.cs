using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotFixObject_Component : MonoBehaviour
{
    public enum eComponentType
    {
        Break,
        NeighborSlotEffect,
        SlotDyingAtBreak,
        SlotDyingAtNoDie,
    }
}
