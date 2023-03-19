using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SlotManager : MonoBehaviour
{
    private int m_nCreateEdgeID = 1000;

    public void CreateMap()
    {
        m_nCreateEdgeID = 1000;

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        Vector2 vLeftTopPos = InGameInfo.Instance.m_vSlot_LeftTopPos;

        switch (InGameSetting.Instance.m_pInGameSettingData.m_eBlockFillDirection)
        {
            case eBlockFillDirection.BottomToUp:    vLeftTopPos *= -1;      break;
            case eBlockFillDirection.LeftToRight:   vLeftTopPos.y *= -1;    break;
            case eBlockFillDirection.RightToLeft:   vLeftTopPos.x *= -1;    break;
        }

        int nDelta = InGameSetting.Instance.m_pInGameSettingData.m_IsSlotFrame == true ? 1 : 0;

        List<Vector2Int> CheckedEdgeList = new List<Vector2Int>();

        for (int y = InGameInfo.Instance.m_nSlotStartIndex_Y - nDelta; y <= InGameInfo.Instance.m_nSlotEndIndex_Y + nDelta; ++y)
        {
            Vector2 vPos = new Vector2(0, (y - InGameInfo.Instance.m_nSlotStartIndex_Y) * -InGameInfo.Instance.m_fSlotSize);
            for (int x = InGameInfo.Instance.m_nSlotStartIndex_X - nDelta; x <= InGameInfo.Instance.m_nSlotEndIndex_X + nDelta; ++x)
            {
                vPos.x = (x - InGameInfo.Instance.m_nSlotStartIndex_X) * InGameInfo.Instance.m_fSlotSize;

                Vector2 vRealPos = vPos;

                switch (InGameSetting.Instance.m_pInGameSettingData.m_eBlockFillDirection)
                {
                    case eBlockFillDirection.BottomToUp:    vRealPos = vPos * -1;                           break;
                    case eBlockFillDirection.LeftToRight:   vRealPos.x = -vPos.y; vRealPos.y = vPos.x;      break;
                    case eBlockFillDirection.RightToLeft:   vRealPos.x = vPos.y; vRealPos.y = -vPos.x;      break;
                }

                // 일단 하드코딩
                if (InGameSetting.Instance.m_pInGameSettingData.m_IsSlotFrame == true &&
                    (y == InGameInfo.Instance.m_nSlotStartIndex_Y - 1 || y == InGameInfo.Instance.m_nSlotEndIndex_Y + 1 ||
                     x == InGameInfo.Instance.m_nSlotStartIndex_X - 1 || x == InGameInfo.Instance.m_nSlotEndIndex_X + 1) )
                {
                    GameObject ob_slot = new GameObject("Slot_" + Helper.GetSlotIndex(x, y).ToString() + "_Empty");
                    Slot_Empty pSlot_Empty = ob_slot.AddComponent<Slot_Empty>();
                    ob_slot.transform.SetParent(gameObject.transform);
                    ob_slot.transform.localPosition = (vRealPos + vLeftTopPos);

                    m_SlotEmptyTable.Add(Helper.GetSlotIndex(x, y), pSlot_Empty);
                }
                else
                {
                    // 일단 하드코딩
                    eSlotFillWay eFillWay = eSlotFillWay.Normal;

                    if (InGameSetting.Instance.m_pInGameSettingData.m_eBlockFillDirection == eBlockFillDirection.BottomToUp)
                    {
                        Vector2Int vIndex = ExcelDataHelper.GetSlotConverting(x, y);
                        if (pDataStack.m_pInGameStageMapData.IsInGameStageMapSlotData(vIndex.x, vIndex.y, eMapSlotItem.Open) == false)
                        {
                            eFillWay = eSlotFillWay.Close;
                        }
                    }

                    if (y == InGameInfo.Instance.m_nSlotStartIndex_Y)
                    {
                        eFillWay = eSlotFillWay.PlayerCharacter;
                    }
                    else if (y == InGameInfo.Instance.m_nSlotStartIndex_Y + 1)
                    {
                        eFillWay = eSlotFillWay.Spawn;
                    }

                    if (eFillWay != eSlotFillWay.Close)
                    {
                        GameObject ob_slot = new GameObject("Slot_" + Helper.GetSlotIndex(x, y).ToString());
                        Slot pSlot = ob_slot.AddComponent<Slot>();
                        pSlot.Init(this, m_pMainGame, Helper.GetSlotIndex(x, y), x, y, vRealPos + vLeftTopPos, eFillWay);
                        ob_slot.transform.SetParent(gameObject.transform);
                        ob_slot.transform.localPosition = (vRealPos + vLeftTopPos);

                        m_SlotTable.Add(Helper.GetSlotIndex(x, y), pSlot);
                        m_ReverseSlotList.Insert(0, pSlot);
                        m_SlotList.Add(pSlot);
                    }
					else
					{
						GameObject ob_slot = new GameObject("Slot_" + Helper.GetSlotIndex(x, y).ToString() + "_Empty");
						Slot_Empty pSlot_Empty = ob_slot.AddComponent<Slot_Empty>();
						ob_slot.transform.SetParent(gameObject.transform);
						ob_slot.transform.localPosition = (vRealPos + vLeftTopPos);

						m_SlotEmptyTable.Add(Helper.GetSlotIndex(x, y), pSlot_Empty);

                        CheckedEdgeList.Add(new Vector2Int(x, y));

                    }
				}
            }
        }

        for (int y = InGameInfo.Instance.m_nSlotStartIndex_Y - nDelta; y <= InGameInfo.Instance.m_nSlotEndIndex_Y + nDelta; ++y)
        {
            for (int x = InGameInfo.Instance.m_nSlotStartIndex_X - nDelta; x <= InGameInfo.Instance.m_nSlotEndIndex_X + nDelta; ++x)
            {
                //if (x == 0 || y == 0 || x == InGameInfo.Instance.m_nSlotEndIndex_X + nDelta && y == InGameInfo.Instance.m_nSlotEndIndex_Y + nDelta)
                if (y != 0 && y != InGameInfo.Instance.m_nSlotEndIndex_Y + nDelta)
                {
                    if (m_SlotTable.ContainsKey(Helper.GetSlotIndex(x, y)) == false)
                        continue;

                    Slot pSlot = m_SlotTable[Helper.GetSlotIndex(x, y)];

                    int nX, nY;

                    nX = x - 1;  nY = y - 1;
                    if (CheckedEdgeList.Contains(new Vector2Int(nX, nY)) == false)
                    {
                        CheckedEdgeList.Add(new Vector2Int(nX, nY));
                        // eBlockFillDirection.BottomToUp 으로 계산..
                        Vector2 vPos = pSlot.GetPosition() + new Vector2(InGameInfo.Instance.m_fSlotSize, -InGameInfo.Instance.m_fSlotSize);
                        CreateEdge(nX, nY, vPos);
                    }

                    nX = x; nY = y - 1;
                    if (CheckedEdgeList.Contains(new Vector2Int(nX, nY)) == false)
                    {
                        CheckedEdgeList.Add(new Vector2Int(nX, nY));
                        // eBlockFillDirection.BottomToUp 으로 계산..
                        Vector2 vPos = pSlot.GetPosition() + new Vector2(0, -InGameInfo.Instance.m_fSlotSize);
                        CreateEdge(nX, nY, vPos);
                    }

                    nX = x + 1; nY = y - 1;
                    if (CheckedEdgeList.Contains(new Vector2Int(nX, nY)) == false)
                    {
                        CheckedEdgeList.Add(new Vector2Int(nX, nY));
                        // eBlockFillDirection.BottomToUp 으로 계산..
                        Vector2 vPos = pSlot.GetPosition() + new Vector2(-InGameInfo.Instance.m_fSlotSize, -InGameInfo.Instance.m_fSlotSize);
                        CreateEdge(nX, nY, vPos);
                    }

                    nX = x - 1; nY = y;
                    if (CheckedEdgeList.Contains(new Vector2Int(nX, nY)) == false)
                    {
                        CheckedEdgeList.Add(new Vector2Int(nX, nY));
                        // eBlockFillDirection.BottomToUp 으로 계산..
                        Vector2 vPos = pSlot.GetPosition() + new Vector2(InGameInfo.Instance.m_fSlotSize, 0);
                        CreateEdge(nX, nY, vPos);
                    }

                    nX = x + 1; nY = y;
                    if (CheckedEdgeList.Contains(new Vector2Int(nX, nY)) == false)
                    {
                        CheckedEdgeList.Add(new Vector2Int(nX, nY));
                        // eBlockFillDirection.BottomToUp 으로 계산..
                        Vector2 vPos = pSlot.GetPosition() + new Vector2(-InGameInfo.Instance.m_fSlotSize, 0);
                        CreateEdge(nX, nY, vPos);
                    }

                    nX = x - 1; nY = y + 1;
                    if (CheckedEdgeList.Contains(new Vector2Int(nX, nY)) == false)
                    {
                        CheckedEdgeList.Add(new Vector2Int(nX, nY));
                        // eBlockFillDirection.BottomToUp 으로 계산..
                        Vector2 vPos = pSlot.GetPosition() + new Vector2(InGameInfo.Instance.m_fSlotSize, InGameInfo.Instance.m_fSlotSize);
                        CreateEdge(nX, nY, vPos);
                    }

                    nX = x; nY = y + 1;
                    if (CheckedEdgeList.Contains(new Vector2Int(nX, nY)) == false)
                    {
                        CheckedEdgeList.Add(new Vector2Int(nX, nY));
                        // eBlockFillDirection.BottomToUp 으로 계산..
                        Vector2 vPos = pSlot.GetPosition() + new Vector2(0, InGameInfo.Instance.m_fSlotSize);
                        CreateEdge(nX, nY, vPos);
                    }

                    nX = x + 1; nY = y + 1;
                    if (CheckedEdgeList.Contains(new Vector2Int(nX, nY)) == false)
                    {
                        CheckedEdgeList.Add(new Vector2Int(nX, nY));
                        // eBlockFillDirection.BottomToUp 으로 계산..
                        Vector2 vPos = pSlot.GetPosition() + new Vector2(-InGameInfo.Instance.m_fSlotSize, InGameInfo.Instance.m_fSlotSize);
                        CreateEdge(nX, nY, vPos);
                    }
                }
            }
        }

        for (int y = InGameInfo.Instance.m_nSlotStartIndex_Y - nDelta; y <= InGameInfo.Instance.m_nSlotEndIndex_Y + nDelta; ++y)
        {
            for (int x = InGameInfo.Instance.m_nSlotStartIndex_X - nDelta; x <= InGameInfo.Instance.m_nSlotEndIndex_X + nDelta; ++x)
            {
                Slot pSlot = null;
                Slot_Empty pSlot_Empty = null;

                if (m_SlotTable.ContainsKey(Helper.GetSlotIndex(x, y)) == true)
                    pSlot = m_SlotTable[Helper.GetSlotIndex(x, y)];

                if (m_SlotEmptyTable.ContainsKey(Helper.GetSlotIndex(x, y)) == true)
                    pSlot_Empty = m_SlotEmptyTable[Helper.GetSlotIndex(x, y)];

                // Neighbor_00
                if (x > InGameInfo.Instance.m_nSlotStartIndex_X && m_SlotTable.ContainsKey(Helper.GetSlotIndex(x - 1, y - 1)) == true)
                {
                    if (pSlot != null)
                    {
                        pSlot.SetNeighbor(eNeighbor.Neighbor_00, m_SlotTable[Helper.GetSlotIndex(x - 1, y - 1)]);
                    }

                    if (pSlot_Empty != null)
                    {
                        pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_00, m_SlotTable[Helper.GetSlotIndex(x - 1, y - 1)]);
                    }
                }

                // Neighbor_10
                if (m_SlotTable.ContainsKey(Helper.GetSlotIndex(x, y - 1)) == true)
                {
                    if (pSlot != null)
                    {
                        pSlot.SetNeighbor(eNeighbor.Neighbor_10, m_SlotTable[Helper.GetSlotIndex(x, y - 1)]);
                    }

                    if (pSlot_Empty != null)
                    {
                        pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_10, m_SlotTable[Helper.GetSlotIndex(x, y - 1)]);
                    }
                }

                // Neighbor_20
                if (x < InGameInfo.Instance.m_nSlotEndIndex_X && m_SlotTable.ContainsKey(Helper.GetSlotIndex(x + 1, y - 1)) == true)
                {
                    if (pSlot != null)
                    {
                        pSlot.SetNeighbor(eNeighbor.Neighbor_20, m_SlotTable[Helper.GetSlotIndex(x + 1, y - 1)]);
                    }

                    if (pSlot_Empty != null)
                    {
                        pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_20, m_SlotTable[Helper.GetSlotIndex(x + 1, y - 1)]);
                    }
                }

                // Neighbor_01
                if (x > InGameInfo.Instance.m_nSlotStartIndex_X && m_SlotTable.ContainsKey(Helper.GetSlotIndex(x - 1, y)) == true)
                {
                    if (pSlot != null)
                    {
                        pSlot.SetNeighbor(eNeighbor.Neighbor_01, m_SlotTable[Helper.GetSlotIndex(x - 1, y)]);
                    }

                    if (pSlot_Empty != null)
                    {
                        pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_01, m_SlotTable[Helper.GetSlotIndex(x - 1, y)]);
                    }
                }

                // Neighbor_21
                if (x < InGameInfo.Instance.m_nSlotEndIndex_X && m_SlotTable.ContainsKey(Helper.GetSlotIndex(x + 1, y)) == true)
                {
                    if (pSlot != null)
                    {
                        pSlot.SetNeighbor(eNeighbor.Neighbor_21, m_SlotTable[Helper.GetSlotIndex(x + 1, y)]);
                    }

                    if (pSlot_Empty != null)
                    {
                        pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_21, m_SlotTable[Helper.GetSlotIndex(x + 1, y)]);
                    }
                }

                // Neighbor_02
                if (x > InGameInfo.Instance.m_nSlotStartIndex_X && m_SlotTable.ContainsKey(Helper.GetSlotIndex(x - 1, y + 1)) == true)
                {
                    if (pSlot != null)
                    {
                        pSlot.SetNeighbor(eNeighbor.Neighbor_02, m_SlotTable[Helper.GetSlotIndex(x - 1, y + 1)]);
                    }

                    if (pSlot_Empty != null)
                    {
                        pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_02, m_SlotTable[Helper.GetSlotIndex(x - 1, y + 1)]);
                    }
                }

                // Neighbor_12
                if (m_SlotTable.ContainsKey(Helper.GetSlotIndex(x, y + 1)) == true)
                {
                    if (pSlot != null)
                    {
                        pSlot.SetNeighbor(eNeighbor.Neighbor_12, m_SlotTable[Helper.GetSlotIndex(x, y + 1)]);
                    }

                    if (pSlot_Empty != null)
                    {
                        pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_12, m_SlotTable[Helper.GetSlotIndex(x, y + 1)]);
                    }
                }

                // Neighbor_22
                if (x < InGameInfo.Instance.m_nSlotEndIndex_X && m_SlotTable.ContainsKey(Helper.GetSlotIndex(x + 1, y + 1)) == true)
                {
                    if (pSlot != null)
                    {
                        pSlot.SetNeighbor(eNeighbor.Neighbor_22, m_SlotTable[Helper.GetSlotIndex(x + 1, y + 1)]);
                    }

                    if (pSlot_Empty != null)
                    {
                        pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_22, m_SlotTable[Helper.GetSlotIndex(x + 1, y + 1)]);
                    }
                }
            }
        }

        foreach (KeyValuePair<int, Slot_Empty> item in m_SlotEmptyTable)
        {
            item.Value.CreateEdge();
        }

		foreach (KeyValuePair<int, Slot> item in m_SlotTable)
		{
			if (item.Value != null && item.Value.GetSlotFillWay() != eSlotFillWay.Close && item.Value.GetY() > 0)
			{
				item.Value.InitSlotBlock(GetRandomBlockType(), eSpecialItem.None);
			}
		}

        Recursive_Init();

        for (int i = (int)eMapSlotItem.Box; i < (int)eMapSlotItem.Max; ++i)
        {
            List<InGameStageMapSlotData> MapSlotDataList = pDataStack.m_pInGameStageMapData.GetInGameStageMapSlotData((eMapSlotItem)i);
            foreach (InGameStageMapSlotData pInGameStageMapSlotData in MapSlotDataList)
            {
                Vector2Int vIndex = new Vector2Int(pInGameStageMapSlotData.m_nX, pInGameStageMapSlotData.m_nY);
                if (InGameSetting.Instance.m_pInGameSettingData.m_eBlockFillDirection == eBlockFillDirection.BottomToUp)
                {
                    vIndex = ExcelDataHelper.GetSlotConverting(pInGameStageMapSlotData.m_nX, pInGameStageMapSlotData.m_nY);
                }

                int nSlotIndex = Helper.GetSlotIndex(vIndex.x, vIndex.y);
                if (m_SlotTable.ContainsKey(nSlotIndex) == true)
                {
                    m_SlotTable[nSlotIndex].ChangeSlotBlock(pInGameStageMapSlotData);
                }
            }
        }

        Recursive_Init();
    }

    private void CreateEdge(int nX, int nY, Vector2 vPos)
    {
        if (IsValidSlotIndex(nX, nY) == true && m_SlotTable.ContainsKey(Helper.GetSlotIndex(nX, nY)) == true)
        {
            return;
        }

        GameObject ob_slot = new GameObject("Slot_" + m_nCreateEdgeID + "_Empty");
        Slot_Empty pSlot_Empty = ob_slot.AddComponent<Slot_Empty>();
        ob_slot.transform.SetParent(gameObject.transform);
        ob_slot.transform.localPosition = vPos;

        m_SlotEmptyTable.Add(m_nCreateEdgeID++, pSlot_Empty);

        // Neighbor_00
        if (IsValidSlotIndex(nX - 1, nY - 1) == true && m_SlotTable.ContainsKey(Helper.GetSlotIndex(nX - 1, nY - 1)) == true)
        {
            pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_00, m_SlotTable[Helper.GetSlotIndex(nX - 1, nY - 1)]);
        }

        // Neighbor_10
        if (IsValidSlotIndex(nX, nY - 1) == true && m_SlotTable.ContainsKey(Helper.GetSlotIndex(nX, nY - 1)) == true)
        {
            pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_10, m_SlotTable[Helper.GetSlotIndex(nX, nY - 1)]);
        }

        // Neighbor_20
        if (IsValidSlotIndex(nX + 1, nY - 1) == true && m_SlotTable.ContainsKey(Helper.GetSlotIndex(nX + 1, nY - 1)) == true)
        {
            pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_20, m_SlotTable[Helper.GetSlotIndex(nX + 1, nY - 1)]);
        }

        // Neighbor_01
        if (IsValidSlotIndex(nX - 1, nY) == true && m_SlotTable.ContainsKey(Helper.GetSlotIndex(nX - 1, nY)) == true)
        {
            pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_01, m_SlotTable[Helper.GetSlotIndex(nX - 1, nY)]);
        }

        // Neighbor_21
        if (IsValidSlotIndex(nX + 1, nY) == true && m_SlotTable.ContainsKey(Helper.GetSlotIndex(nX + 1, nY)) == true)
        {
            pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_21, m_SlotTable[Helper.GetSlotIndex(nX + 1, nY)]);
        }

        // Neighbor_02
        if (IsValidSlotIndex(nX - 1, nY + 1) == true && m_SlotTable.ContainsKey(Helper.GetSlotIndex(nX - 1, nY + 1)) == true)
        {
            pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_02, m_SlotTable[Helper.GetSlotIndex(nX - 1, nY + 1)]);
        }

        // Neighbor_12
        if (IsValidSlotIndex(nX, nY + 1) == true && m_SlotTable.ContainsKey(Helper.GetSlotIndex(nX, nY + 1)) == true)
        {
            pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_12, m_SlotTable[Helper.GetSlotIndex(nX, nY + 1)]);
        }

        // Neighbor_22
        if (IsValidSlotIndex(nX + 1, nY + 1) == true && m_SlotTable.ContainsKey(Helper.GetSlotIndex(nX + 1, nY + 1)) == true)
        {
            pSlot_Empty.SetNeighbor(eNeighbor.Neighbor_22, m_SlotTable[Helper.GetSlotIndex(nX + 1, nY + 1)]);
        }
    }
}
