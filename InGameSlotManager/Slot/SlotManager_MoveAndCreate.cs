using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class SlotManager : MonoBehaviour
{
    private int                     m_nMoveAndCreateCount               = 0;
    private float                   m_fBlockMoveTime                    = GameDefine.ms_fBlockMoveTime;
    private Transformer_Timer       m_pTimer_SlotMoveAndCreate          = new Transformer_Timer();
    private Dictionary<int, Slot>   m_MoveSlotTable                     = new Dictionary<int, Slot>();
    private Dictionary<int, Slot>   m_MoveSlotCheckTable                = new Dictionary<int, Slot>();

    private int                     m_nCreateSlotLastBlockTypePercent   = 0;
    private Dictionary<int, eBlockType> m_CreateSlotLastBlockTypeTable  = new Dictionary<int, eBlockType>();

    private List<Slot>              m_MoveAndCreateSlotList             = new List<Slot>();
    private bool                    m_IsMoveAndCreateInverse            = false;


    public struct sInitCreate
    {
        public eBlockType m_eBlockType;
        public eMapSlotItem m_eMapSlotItem;
    }

    private Queue<sInitCreate> []   m_InitCreateBlockTypeQueue        = new Queue<sInitCreate>[GameDefine.ms_nInGameSlot_X];

    public void EnqueueInitCreateBlockType(int nLine, eBlockType eType, eMapSlotItem eSlotItem)
    {
        sInitCreate pInitCreate = new sInitCreate();
        pInitCreate.m_eBlockType = eType;
        pInitCreate.m_eMapSlotItem = eSlotItem;
        m_InitCreateBlockTypeQueue[nLine].Enqueue(pInitCreate);
    }

    public void SetCreateSlotLastBlockTypePercent(int nPercent)
    {
        m_nCreateSlotLastBlockTypePercent = nPercent;
    }

    private void OnDoneTimer_SlotMoveAndCreate(TransformerEvent eventValue)
    {
        InGameLog.Log("SlotManager : OnDoneTimer_SlotMoveAndCreate Begin");

        OnSlotMoveAndCreate(false);

        InGameLog.Log("SlotManager : OnDoneTimer_SlotMoveAndCreate End");
    }

    private bool Recursive_UpLeftRightCreateSlot(Slot pSlot)
    {
        Slot pUpSlot = pSlot.GetNeighborSlot(eNeighbor.Neighbor_10);

        if (pUpSlot != null)
        {
            if (pUpSlot.GetSlotFillWay() == eSlotFillWay.Spawn)
            {
                return true;
            }
            else if (pUpSlot.GetSlotLink() != null)
            {
                Slot pSlotLink = pUpSlot.GetSlotLink();

                if (IsUpSlotExist(pSlotLink) == true)
                {
                    return true;
                }
            }

            if (Recursive_UpLeftRightCreateSlot(pUpSlot) == true)
                return true;
        }

        Slot pUpLeftSlot = pSlot.GetNeighborSlot(eNeighbor.Neighbor_00);

        if (pUpLeftSlot != null)
        {
            if (pUpLeftSlot.GetSlotFillWay() == eSlotFillWay.Spawn)
            {
                return true;
            }
            else if (pUpLeftSlot.GetSlotLink() != null)
            {
                Slot pSlotLink = pUpLeftSlot.GetSlotLink();

                if (IsUpSlotExist(pSlotLink) == true)
                {
                    return true;
                }
            }

            if (Recursive_UpLeftRightCreateSlot(pUpLeftSlot) == true)
                return true;
        }

        Slot pUpRightSlot = pSlot.GetNeighborSlot(eNeighbor.Neighbor_20);

        if (pUpRightSlot != null)
        {
            if (pUpRightSlot.GetSlotFillWay() == eSlotFillWay.Spawn)
            {
                return true;
            }
            else if (pUpRightSlot.GetSlotLink() != null)
            {
                Slot pSlotLink = pUpRightSlot.GetSlotLink();

                if (IsUpSlotExist(pSlotLink) == true)
                {
                    return true;
                }
            }

            if (Recursive_UpLeftRightCreateSlot(pUpRightSlot) == true)
                return true;
        }

        return false;
    }

    private bool IsLeftRightCreateSlot(Slot pSlot)
    {
        Slot pUpLeftSlot = pSlot.GetNeighborSlot(eNeighbor.Neighbor_00);

        if (pUpLeftSlot != null)
        {
            if (IsUpSlotExist(pUpLeftSlot) == true)
            {
                return true;
            }
            else if (pUpLeftSlot.GetSlotFillWay() == eSlotFillWay.Spawn)
            {
                return true;
            }
            else if (pUpLeftSlot.GetSlotLink() != null)
            {
                Slot pSlotLink = pUpLeftSlot.GetSlotLink();

                if (IsUpSlotExist(pSlotLink) == true)
                {
                    return true;
                }
            }

            if(Recursive_UpLeftRightCreateSlot(pUpLeftSlot) == true)
                return true;
        }

        Slot pUpRightSlot = pSlot.GetNeighborSlot(eNeighbor.Neighbor_20);

        if (pUpRightSlot != null)
        {
            if (IsUpSlotExist(pUpRightSlot) == true)
            {
                return true;
            }
            else if (pUpRightSlot.GetSlotFillWay() == eSlotFillWay.Spawn) 
            {
                return true;
            }
            else if (pUpRightSlot.GetSlotLink() != null)
            {
                Slot pSlotLink = pUpRightSlot.GetSlotLink();

                if (IsUpSlotExist(pSlotLink) == true)
                {
                    return true;
                }
            }

            if (Recursive_UpLeftRightCreateSlot(pUpRightSlot) == true)
                return true;
        }

        return false;
    }

    // Espresso 에서는 안쓰는 함수
    private bool IsUpSlotExist(Slot pSlot)
    {
        Debug.LogError("IsUpSlotExist !!");
        Debug.Break();

        if (pSlot.GetSlotFillWay() == eSlotFillWay.Spawn)
        {
            return true;
        }

        Slot pUpSlot;
        Slot pPrevSlot = pSlot;

        int nTop = 0;

        if (m_IsMoveAndCreateInverse == false)
        {
            for (int y = pSlot.GetY() - 1; y >= 0; --y)
            {
                if (m_SlotTable.ContainsKey(Helper.GetSlotIndex(pSlot.GetX(), y)) == true)
                {
                    Slot pTempSlot = m_SlotTable[Helper.GetSlotIndex(pSlot.GetX(), y)];
                    if (pTempSlot != null)
                    {
                        ++nTop;
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

            for (int y = nTop; y > 0; --y)
            {
                if (m_SlotTable.ContainsKey(Helper.GetSlotIndex(pSlot.GetX(), pSlot.GetY() - y)) == true)
                {
                    pUpSlot = m_SlotTable[Helper.GetSlotIndex(pSlot.GetX(), pSlot.GetY() - y)];

                    if (pUpSlot != null && pUpSlot.GetSlotBlock() != null)
                    {
                        return true;
                    }

                    if (pUpSlot.GetSlotFillWay() == eSlotFillWay.Spawn)
                    {
                        return true;
                    }
                    else if (pUpSlot.GetSlotLink() != null)
                    {
                        Slot pSlotLink = pUpSlot.GetSlotLink();

                        if (IsUpSlotExist(pSlotLink) == true)
                        {
                            return false;
                        }
                        else
                        {
                            return IsUpSlotExist(pSlotLink);
                        }
                    }

                    if (IsLeftRightCreateSlot(pUpSlot) == true)
                    {
                        return true;
                    }

                    pPrevSlot = pUpSlot;
                }
            }
        }
        else
        {
            for (int y = pSlot.GetY() + 1; y <= InGameInfo.Instance.m_nSlotCount_Y - 1; ++y)
            {
                if (m_SlotTable.ContainsKey(Helper.GetSlotIndex(pSlot.GetX(), y)) == true)
                {
                    Slot pTempSlot = m_SlotTable[Helper.GetSlotIndex(pSlot.GetX(), y)];
                    if (pTempSlot != null)
                    {
                        ++nTop;
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

            for (int y = nTop; y < InGameInfo.Instance.m_nSlotCount_Y - 1; ++y)
            {
                if (m_SlotTable.ContainsKey(Helper.GetSlotIndex(pSlot.GetX(), pSlot.GetY() - y)) == true)
                {
                    pUpSlot = m_SlotTable[Helper.GetSlotIndex(pSlot.GetX(), pSlot.GetY() - y)];

                    if (pUpSlot != null && pUpSlot.GetSlotBlock() != null)
                    {
                        return true;
                    }

                    if (pUpSlot.GetSlotFillWay() == eSlotFillWay.Spawn)
                    {
                        return true;
                    }
                    else if (pUpSlot.GetSlotLink() != null)
                    {
                        Slot pSlotLink = pUpSlot.GetSlotLink();

                        if (IsUpSlotExist(pSlotLink) == true)
                        {
                            return false;
                        }
                        else
                        {
                            return IsUpSlotExist(pSlotLink);
                        }
                    }

                    if (IsLeftRightCreateSlot(pUpSlot) == true)
                    {
                        return true;
                    }

                    pPrevSlot = pUpSlot;
                }
            }
        }

        return false;
    }

    private void OnInGame_SlotMoveAndCreate(float fDelay)
    {
        foreach (KeyValuePair<int, Slot> item in m_SlotTable)
        {
            Slot pSlot = item.Value;
            item.Value.SetMoveFlag(false);
        }

        m_pTimer_SlotMoveAndCreate.OnReset();
        TransformerEvent_Timer eventValue;
        eventValue = new TransformerEvent_Timer(fDelay);
        m_pTimer_SlotMoveAndCreate.AddEvent(eventValue);
        m_pTimer_SlotMoveAndCreate.SetCallback(null, OnDoneTimer_SlotMoveAndCreate);
        m_pTimer_SlotMoveAndCreate.OnPlay();
    }

    private void OnSlotMoveFillWayNormal(Slot pSlot, bool IsStrait)
    {
        Slot pAboveSlot = null;

        if (m_IsMoveAndCreateInverse == false)
        {
            pAboveSlot = pSlot.GetNeighborSlot(eNeighbor.Neighbor_10);
        }
        else
        {
            pAboveSlot = pSlot.GetNeighborSlot(eNeighbor.Neighbor_12);
        }

        if (pAboveSlot != null && IsPossibleMove(pAboveSlot) == true && pAboveSlot.IsPossibleMove() == true)
        {
            if (pAboveSlot.GetSlotBlock() != null)
            {
                if (pAboveSlot.GetSlotBlock().IsMoving() == false)
                {
                    ++m_nMoveAndCreateCount;
                    Slot.OnMove(pAboveSlot, pSlot, m_fBlockMoveTime);
                    m_MoveSlotCheckTable.Add(pSlot.GetSlotIndex(), pSlot);
                }
            }
            else
            {
                SlotFixObject pSlotFixObject = pAboveSlot.GetLastSlotFixObject();

                bool IsPossible = false;

                if (pSlotFixObject != null && pSlotFixObject.GetSlotFixObjectType() == eSlotFixObjectType.Obstacle)
                {
                    SlotFixObject_Obstacle pObstacle = pSlotFixObject as SlotFixObject_Obstacle;

                    if (pObstacle != null && pObstacle.GetObstacleType() == eSlotFixObjectObstacleType.Cloud)
                    {
                        IsPossible = true;
                    }
                }

                if ((pSlotFixObject != null || IsPossible == true) && pSlotFixObject.IsMoveAndCreateInclude() == true && pSlotFixObject.IsMoving() == false)
                {
                    ++m_nMoveAndCreateCount;
                    Slot.OnMove(pAboveSlot, pSlot, m_fBlockMoveTime);
                    m_MoveSlotCheckTable.Add(pSlot.GetSlotIndex(), pSlot);
                }
            }
        }
    }

    private Slot GetPossibleMoveUpSlot(Slot pSlot)
    {
        if (m_IsMoveAndCreateInverse == false)
        {
            for (int y = pSlot.GetY() - 1; y >= 0; --y)
            {
                if (m_SlotTable.ContainsKey(Helper.GetSlotIndex(pSlot.GetX(), y)) == true)
                {
                    Slot pUpSlot = m_SlotTable[Helper.GetSlotIndex(pSlot.GetX(), y)];
                    if (pUpSlot != null)
                    {
                        if ((pUpSlot.GetLastSlotFixObject() == null || IsPossibleMove(pUpSlot) == true) && pUpSlot.IsPossibleMove() == true)
                        {
                            return pUpSlot;
                        }
                    }
                }
            }
        }
        else
        {
            for (int y = pSlot.GetY() + 1; y <= InGameInfo.Instance.m_nSlotCount_Y - 1; ++y)
            {
                if (m_SlotTable.ContainsKey(Helper.GetSlotIndex(pSlot.GetX(), y)) == true)
                {
                    Slot pUpSlot = m_SlotTable[Helper.GetSlotIndex(pSlot.GetX(), y)];
                    if (pUpSlot != null)
                    {
                        if ((pUpSlot.GetLastSlotFixObject() == null || IsPossibleMove(pUpSlot) == true) && pUpSlot.IsPossibleMove() == true)
                        {
                            return pUpSlot;
                        }
                    }
                }
            }
        }

        return null;
    }

    private void ConnectLinkSlot()
    {
        foreach (Slot pSlot in m_MoveAndCreateSlotList)
        {
            pSlot.SetSlotLink_FromSpawn(false);
            pSlot.SetSlotLink(null);
            pSlot.SetSlotFillWay(eSlotFillWay.Normal);
        }

        //foreach (Slot pSlot in m_MoveAndCreateSlotList)
        //{
        //    if (pSlot.GetLastSlotFixObject() == null && pSlot.IsPossibleMove() == true)
        //    {
        //        if (pSlot.GetY() == 0)
        //        {
        //            SlotFixObject pSlotFixObject = new SlotFixObject(eSlotFixObjectType.None);
        //            pSlot.AddSlotFixObject(pSlotFixObject);
        //        }
        //    }
        //}

        foreach (Slot pSlot in m_MoveAndCreateSlotList)
        {
            if ((pSlot.GetLastSlotFixObject() == null || IsPossibleMove(pSlot) == true) && pSlot.IsPossibleMove() == true)
            {
                //if (pSlot.GetY() != 0)
                {
                    eNeighbor eUp = eNeighbor.Neighbor_10;

                    if (m_IsMoveAndCreateInverse == true)
                    {
                        eUp = eNeighbor.Neighbor_12;
                    }
                    
                    Slot pUpSlot = GetPossibleMoveUpSlot(pSlot);

                    if (pUpSlot == null)                                    // UpSlot 이 없다면
                    {
                        pSlot.SetSlotFillWay(eSlotFillWay.Spawn);
                    }
                    else if (pSlot.GetNeighborSlot(eUp) != pUpSlot)         // 바로 위 슬롯이 아니라면
                    {
                        pSlot.SetSlotFillWay(eSlotFillWay.Normal);
                        pSlot.SetSlotLink(pUpSlot);
                    }
                    else                                                    // 바로 위 슬롯이라면
                    {
                        pSlot.SetSlotFillWay(eSlotFillWay.Normal);
                        pSlot.SetSlotLink(null);
                    }
                }
            }
        }
    }

    public void SetBlockMoveTime(float fTime)
    {
        m_fBlockMoveTime = fTime;
    }

    public void SetMoveAndCreateInverse(bool IsInverse)
    {
        m_IsMoveAndCreateInverse = IsInverse;
    }

    public void OnSlotMoveAndCreate(bool IsSlotLinkAtRepeat, bool bNoMoveAndCreateCountZero = false)
    {
        ConnectLinkSlot();

        OutputLog.Log("SlotManager : OnSlotMoveAndCreate Begin");

        if (IsSlotLinkAtRepeat == false)
        {
            m_MoveSlotCheckTable.Clear();

            if (bNoMoveAndCreateCountZero == false)
            {
                m_nMoveAndCreateCount = 0;
            }
        }

        if (m_IsMoveAndCreateInverse == false)
        {
            m_MoveAndCreateSlotList = m_ReverseSlotList;
        }
        else
        {
            m_MoveAndCreateSlotList = m_SlotList;
        }

        foreach (Slot pSlot in m_MoveAndCreateSlotList)
        {
            if (pSlot.GetSlotBlock() == null && pSlot.IsFillBlock() == true)
            {
                switch (pSlot.GetSlotFillWay())
                {
                    case eSlotFillWay.Normal:
                        {
                            if (pSlot.GetSlotLink() == null)
                            {
                                OnSlotMoveFillWayNormal(pSlot, true);
                            }
                            else if (pSlot.GetSlotLink() != null)
                            {
                                Slot pSlotLink = pSlot.GetSlotLink();
                                if (pSlotLink != null && IsPossibleMove(pSlotLink) == true && pSlotLink.IsPossibleMove() == true)
                                {
                                    if (pSlotLink.GetSlotBlock() != null)
                                    {
                                        if (pSlotLink.GetSlotBlock().IsMoving() == false)
                                        {
                                            ++m_nMoveAndCreateCount;
                                            Slot.OnLinkMove(pSlotLink, pSlot, m_fBlockMoveTime, m_IsMoveAndCreateInverse);
                                            m_MoveSlotCheckTable.Add(pSlot.GetSlotIndex(), pSlot);
                                        }
                                    }
                                    else
                                    {
                                        SlotFixObject pSlotFixObject = pSlotLink.GetLastSlotFixObject();

                                        if (pSlotFixObject != null && pSlotFixObject.IsMoveAndCreateInclude() == true && pSlotFixObject.IsMoving() == false)
                                        {
                                            ++m_nMoveAndCreateCount;
                                            Slot.OnMove(pSlotLink, pSlot, m_fBlockMoveTime);
                                            m_MoveSlotCheckTable.Add(pSlot.GetSlotIndex(), pSlot);
                                        }
                                    }
                                }
                                else
                                {
                                    if (pSlotLink != null && pSlotLink.IsPossibleMove() == true)
                                    {
                                        OnSlotMoveFillWayNormal(pSlot, true);
                                    }
                                    else if(pSlotLink != null && pSlotLink.GetSlotBlock() == null && pSlotLink.IsPossibleMove() == true)
                                    {
                                        if (IsUpSlotExist(pSlotLink) == false)
                                        {
                                            OnSlotMoveFillWayNormal(pSlot, true);
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case eSlotFillWay.Spawn:
                        {
                            //Slot pSlotLink = pSlot.GetSlotLink();

                            //if (pSlotLink != null && pSlotLink.IsPossibleMove() == true && (pSlotLink.GetSlotBlock() != null || IsUpSlotExist(pSlotLink) == true))
                            //{
                            //    if (pSlotLink.GetSlotBlock() != null /*&& pSlotLink.GetSlotBlock().IsMoving() == false*/)
                            //    {
                            //        ++m_nMoveAndCreateCount;
                            //        Slot.OnLinkMove(pSlotLink, pSlot, m_fBlockMoveTime);
                            //        m_MoveSlotCheckTable.Add(pSlot.GetSlotIndex(), pSlot);
                            //    }
                            //}
                            //else
                            {
                                ++m_nMoveAndCreateCount;

                                eBlockType eCreateBlockType;
                                int nRandomValue = UnityEngine.Random.Range(0, 100);
                                if (nRandomValue < m_nCreateSlotLastBlockTypePercent && m_CreateSlotLastBlockTypeTable.ContainsKey(pSlot.GetSlotIndex()) == true)
                                    eCreateBlockType = GetRandomBlockType_ExceptBlockType(m_CreateSlotLastBlockTypeTable[pSlot.GetSlotIndex()]);
                                else
                                    eCreateBlockType = GetRandomBlockType();

                                if (m_InitCreateBlockTypeQueue[pSlot.GetX()].Count != 0)
                                {
                                    sInitCreate pInitCreate = m_InitCreateBlockTypeQueue[pSlot.GetX()].Dequeue();

                                    if (pInitCreate.m_eMapSlotItem != eMapSlotItem.None)
                                    {
                                        pSlot.OnCreateSlotBlock(pInitCreate.m_eMapSlotItem, m_fBlockMoveTime, m_IsMoveAndCreateInverse);
                                    }
                                    else
                                    {
                                        pSlot.OnCreateSlotBlock(pInitCreate.m_eBlockType, m_fBlockMoveTime, m_IsMoveAndCreateInverse);
                                    }
                                }
                                else
                                {
                                    pSlot.OnCreateSlotBlock(eCreateBlockType, m_fBlockMoveTime, m_IsMoveAndCreateInverse);
                                }
                                
                                m_CreateSlotLastBlockTypeTable[pSlot.GetSlotIndex()] = eCreateBlockType;

                                m_MoveSlotCheckTable.Add(pSlot.GetSlotIndex(), pSlot);
                            }
                        }
                        break;
                }
            }
        }

        foreach (KeyValuePair<int, Slot> item in m_MoveSlotTable)
        {
            bool IsFind = false;
            foreach (KeyValuePair<int, Slot> CurrMoveItem in m_MoveSlotCheckTable)
            {
                if (item.Key == CurrMoveItem.Key)
                {
                    IsFind = true;
                    break;
                }
            }

            if (IsFind == false)
            {
                item.Value.OnMoveComplete();
            }
        }

        m_MoveSlotTable.Clear();
        foreach (KeyValuePair<int, Slot> CurrMoveItem in m_MoveSlotCheckTable)
        {
            m_MoveSlotTable.Add(CurrMoveItem.Key, CurrMoveItem.Value);
        }

        if (m_nMoveAndCreateCount == 0 && IsAllSlotFull() == true)
        {
            OutputLog.Log("SlotManager : OnSlotMoveAndCreate m_nMoveAndCreateCount == 0 && IsAllSlotFull() == true");
            InGameLog.Log("SlotManager : OnSlotMoveAndCreate m_nMoveAndCreateCount == 0 && IsAllSlotFull() == true");

            // 모든 슬롯 재워짐
            // 제거할수 있는게 있는지 체크
            m_pMainGame.OnDoneSlotMoveAndCreate();
            OnCheckRemoveSlot();
            m_fBlockMoveTime = GameDefine.ms_fBlockMoveTime;
        }
        else if (m_nMoveAndCreateCount == 0 && IsAllSlotFull() == false)
        {
            OutputLog.Log("SlotManager : OnSlotMoveAndCreate m_nMoveAndCreateCount == 0 && IsAllSlotFull() == false");
            InGameLog.Log("SlotManager : OnSlotMoveAndCreate m_nMoveAndCreateCount == 0 && IsAllSlotFull() == false");

            m_pTimer_SlotMoveAndCreate.OnReset();
            TransformerEvent_Timer eventValue;
            eventValue = new TransformerEvent_Timer(0.03f);
            m_pTimer_SlotMoveAndCreate.AddEvent(eventValue);
            m_pTimer_SlotMoveAndCreate.SetCallback(null, OnDoneTimer_SlotMoveAndCreate);
            m_pTimer_SlotMoveAndCreate.OnPlay();
        }
        else
        {
            if (InGameInfo.Instance.m_IsSlotLink == false)
            {
                m_fBlockMoveTime *= GameDefine.ms_fBlockMoveMutipleTime;
            }
            else
            {
                if (IsSlotLinkAtRepeat == false)
                {
                    OnSlotMoveAndCreate(true);
                }
                else
                {
                    m_fBlockMoveTime *= GameDefine.ms_fBlockMoveMutipleTime;
                }
            }
        }

        OutputLog.Log("SlotManager : OnSlotMoveAndCreate End");
    }

    public bool IsAllSlotFull()
    {
        foreach (Slot pSlot in m_ReverseSlotList)
        {
            if (pSlot.GetSlotBlock() == null && pSlot.GetLastSlotFixObject() == null)
            {
                return false;
            }
        }

        return true;
    }

    public void OnDoneMoveAndCreate(Slot pSlot)
    {       
        if(m_MoveSlotCheckTable.ContainsKey(pSlot.GetSlotIndex()) == true)
            m_MoveSlotCheckTable.Remove(pSlot.GetSlotIndex());

        --m_nMoveAndCreateCount;

        if (m_nMoveAndCreateCount == 0)
        {
            OnSlotMoveAndCreate(false, true);
        }
    }

    public void OnDonePossibleBlockSwapAction(Slot pSlot)
    {
    }
}
