using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class SlotManager : MonoBehaviour
{
    private MainGame                            m_pMainGame                             = null;
    public bool                                 m_IsInitialize                          = false;
    private Dictionary<int,Slot>                m_SlotTable                             = new Dictionary<int, Slot>();
    private List<Slot>                          m_ReverseSlotList                       = new List<Slot>();
    private List<Slot>                          m_SlotList                              = new List<Slot>();

    private Dictionary<int, Slot_Empty>         m_SlotEmptyTable                        = new Dictionary<int, Slot_Empty>();

    private List<SlotBlock>                     m_PossibleMoveSlotBlockList             = new List<SlotBlock>();

    private Transformer_Timer                   m_pTimer_Shuffle                        = new Transformer_Timer(0);

    private Dictionary<int,List<Slot>>          m_SlotMoveTable                         = new Dictionary<int, List<Slot>>();

    private static List<eBlockType>             ms_eAppearBlockTypeList                 = new List<eBlockType>();

    void Start()
    {
        m_pCombinationSlot = gameObject.AddComponent<Slot>();
        m_pCombinationSlot_Shape = gameObject.AddComponent<Slot>();

        m_pGameObject_ChainLightning = Resources.Load<GameObject>("ChainLightning/Prefabs/ChainLightning");

        for (int i = 0; i < m_nMaxSwwaPossibleCount; ++i)
        {
            m_SwapPossibleTable.Add(i, new List<KeyValuePair<Slot, Slot>>());
        }

        EventDelegateManager.Instance.OnEventInGame_SlotMoveAndCreate += OnInGame_SlotMoveAndCreate;

        m_IsInitialize = true;

        for (int i = 0; i < GameDefine.ms_nInGameSlot_X; ++i)
        {
            m_InitCreateBlockTypeQueue[i] = new Queue<sInitCreate>();
        }
    }

    private void OnDestroy()
    {
        //foreach (KeyValuePair<int, Slot> item in m_SlotTable)
        //{
        //    item.Value.OnDestroy();
        //}
        m_SlotTable.Clear();
        m_ReverseSlotList.Clear();
        m_SlotList.Clear();

        foreach (KeyValuePair<int, Slot_Empty> item in m_SlotEmptyTable)
        {
            item.Value.OnDestroy();
        }
        m_SlotEmptyTable.Clear();

        EventDelegateManager.Instance.OnEventInGame_SlotMoveAndCreate -= OnInGame_SlotMoveAndCreate;
    } 

    void Update()
    {
        m_pTimer_Shuffle.Update(Time.deltaTime);

        m_pTimer_SlotMoveAndCreate.Update(Time.deltaTime);
        m_pTimer_BlockSwap.Update(Time.deltaTime);
        m_pTimer_SpecialSlotComplete.Update(Time.deltaTime);
        m_pTimer_CheckNextSpecialSlotRemove_Delay.Update(Time.deltaTime);

        m_pTransformerCollector.Update(Time.deltaTime);

        m_pTimer_SlotMove.Update(Time.deltaTime);
        m_pTimer_RemoveEnd.Update(Time.deltaTime);

        m_pTimer_MatchColorTransSlot.Update(Time.deltaTime);

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnInGame_SlotMoveAndCreate(0.0f);
        }
#endif
    }

    public void SetMainGame(MainGame pMainGame)
    {
        m_pMainGame = pMainGame;
    }

    public MainGame GetMainGame()
    {
        return m_pMainGame;
    }

    public void ClearAppearBlockTypeList()
    {
        ms_eAppearBlockTypeList.Clear();
    }

    public void AddAppearBlockType(eBlockType eType)
    {
        ms_eAppearBlockTypeList.Add(eType);
    }

    public void OnAppaerBlockTypeEnd()
    {
        Helper.ShuffleList(ms_eAppearBlockTypeList);
    }

    public Dictionary<int, Slot> GetSlotTable()
    {
        return m_SlotTable;
    }

    public List<Slot> GetReverseSlotList()
    {
        return m_ReverseSlotList;
    }

    public void Initialize_PossibleMoveSlotCheck(bool IsDeleteInGameLoading = true)
    {
        if (Calculation_PossibleMoveSlot() == false)
        {
            StartCoroutine(PossibleMoveSlotCheck(IsDeleteInGameLoading));
        }
        else
        {
            m_pMainGame.OnSlotManagerInitializeDone();
        }
    }

    IEnumerator PossibleMoveSlotCheck(bool IsDeleteInGameLoading)
    {
        while (Calculation_PossibleMoveSlot() == false)
        {
            List<Slot> slotList = new List<Slot>();
            List<SlotBlock> slotBlockList = new List<SlotBlock>();

            foreach (KeyValuePair<int, Slot> item in m_SlotTable)
            {
                if (item.Value != null && item.Value.GetSlotBlock() != null)
                {
                    slotList.Add(item.Value);
                    slotBlockList.Add(item.Value.GetSlotBlock());
                }
            }

            Helper.ShuffleList<Slot>(slotList);
            Helper.ShuffleList<SlotBlock>(slotBlockList);

            for (int i = 0; i < slotList.Count; ++i)
            {
                Slot pSlot = slotList[i];
                SlotBlock pSlotBlock = slotBlockList[i];

                Slot.OnShuffleMove(pSlot, pSlotBlock, 0);
            }

            Recursive_Init();

            yield return new WaitForEndOfFrame();
        }

        m_pMainGame.OnSlotManagerInitializeDone();
    }

    private void Recursive_Init()
    {
        foreach (Slot pSlot in m_ReverseSlotList)
        {
            if (pSlot.GetSlotBlock() != null && pSlot.GetSlotFillWay() != eSlotFillWay.Close && IsNormalBlock(pSlot) == true)
            {
                Dictionary<int, Slot> MatchSlotTable = new Dictionary<int, Slot>();
                Slot pCombinationSlot = m_pCombinationSlot;
                int nIndex = GetShape(pSlot, ref MatchSlotTable, ref pCombinationSlot);
                if (nIndex != -1)
                {
                    pSlot.ChangeSlotBlock(GetRandomBlockType_ExceptBlockType(pSlot.GetSlotBlock().GetBlockType()));
                    Recursive_Init();
                    return;
                }
            }
        }
    }

    public static bool IsValidSlotIndex(int x, int y)
    {
        if (x >= InGameInfo.Instance.m_nSlotStartIndex_X && x <= InGameInfo.Instance.m_nSlotEndIndex_X &&
            y >= InGameInfo.Instance.m_nSlotStartIndex_Y && y <= InGameInfo.Instance.m_nSlotEndIndex_Y)
            return true;

        return false;
    }

    public static eBlockType GetRandomBlockType()
    {
        int nNumData = ms_eAppearBlockTypeList.Count;
        int nIndex = UnityEngine.Random.Range(0, nNumData);
        return ms_eAppearBlockTypeList[nIndex];
    }

    public static eBlockType GetRandomBlockType_ExceptBlockType(eBlockType eExceptType)
    {
        List<eBlockType> blockTypeList = new List<eBlockType>();

        foreach (eBlockType eType in ms_eAppearBlockTypeList)
        {
            if (eType != eExceptType)
            {
                blockTypeList.Add(eType);
            }
        }

        int nNumData = blockTypeList.Count;
        int nIndex = UnityEngine.Random.Range(0, nNumData);
        return blockTypeList[nIndex];
    }

    public static eBlockType GetRandomBlockType_ExceptBlockType(List<eBlockType> ExceptTypeList)
    {
        List<eBlockType> blockTypeList = new List<eBlockType>();

        foreach (eBlockType eType in ms_eAppearBlockTypeList)
        {
            if (ExceptTypeList.Contains(eType) == false)
            {
                blockTypeList.Add(eType);
            }
        }

        if (blockTypeList.Count != 0)
        {
            int nNumData = blockTypeList.Count;
            int nIndex = UnityEngine.Random.Range(0, nNumData);
            return blockTypeList[nIndex];
        }

        return eBlockType.Empty;
    }

    public eBlockType GetRandomBlockType_CurrExistSlotTable()
    {
        List<eBlockType> blockTypeList = new List<eBlockType>();

        foreach (KeyValuePair<int, Slot> item in m_SlotTable)
        {
            if (item.Value.GetSlotBlock() != null)
            {
                int nType = (int)item.Value.GetSlotBlock().GetBlockType();
                if (nType >= GameDefine.ms_nBlockStart && nType < GameDefine.ms_nBlockStart + InGameSetting.Instance.m_pInGameSettingData.m_nBlockCount)
                {
                    if (blockTypeList.Contains((eBlockType)nType) == false)
                    {
                        blockTypeList.Add((eBlockType)nType);
                    }
                }
            }
        }

        if (blockTypeList.Count != 0)
        {
            int nIndex = UnityEngine.Random.Range(0, blockTypeList.Count);
            return blockTypeList[nIndex];
        }

        return eBlockType.Block_Start;
    }

    public List<SlotFixObject> GetSlotFixObjects_byLine(int nX, int nY_ToTopping)
    {
        List<SlotFixObject> fixObjectList = new List<SlotFixObject>();

        for (int y = GameDefine.ms_nInGameSlot_Y - 1; y >= nY_ToTopping + 1; --y)
        {
            if (m_SlotTable.ContainsKey(Helper.GetSlotIndex(nX, y)) == true)
            {
                Slot pSlot = m_SlotTable[Helper.GetSlotIndex(nX, y)];

                List<SlotFixObject> slotFixObjectList = pSlot.GetSlotFixObjectList();

                foreach (SlotFixObject pSlotFixObject in slotFixObjectList)
                {
                    fixObjectList.Add(pSlotFixObject);
                }
            }
        }

        return fixObjectList;
    }

    private void Recursive_SlotMove_Freezing(Slot pSlot)
    {
        pSlot.SetSlotMove_Freezing(true);

        if (pSlot.GetSlot_SlotMove_Prev().IsSlotMove_Freezing() == true)
        {
            return;
        }

        Slot pSlot_Prev = pSlot.GetSlot_SlotMove_Prev();

        if (pSlot_Prev.GetSlotBlock() == null)
        {
            return;
        }

        Recursive_SlotMove_Freezing(pSlot_Prev);
    }

    public int GetLastSlotY(int nX, bool IsInverse = false)
    {
        int nLastSlotY = 0;

        if (IsInverse == false)
        {
            nLastSlotY = InGameInfo.Instance.m_nSlotStartIndex_Y;

            for (int y = nLastSlotY; y <= InGameInfo.Instance.m_nSlotEndIndex_Y; ++y)
            {
                if (m_SlotTable.ContainsKey(Helper.GetSlotIndex(nX, y)) == true)
                {
                    nLastSlotY = y;
                }
            }
        }
        else
        {
            nLastSlotY = InGameInfo.Instance.m_nSlotEndIndex_Y;

            for (int y = nLastSlotY; y >= InGameInfo.Instance.m_nSlotStartIndex_Y; --y)
            {
                if (m_SlotTable.ContainsKey(Helper.GetSlotIndex(nX, y)) == true)
                {
                    nLastSlotY = y;
                }
            }
        }

        return nLastSlotY;
    }

    private bool CheckSlotMove()
    {
        InGameInfo.Instance.m_IsCheckSlotMove = false;

        if (m_SlotMoveTable.Count == 0)
        {
            return false;
        }
        else
        {
            bool IsEmpty = true;
            foreach (KeyValuePair<int, List<Slot>> item in m_SlotMoveTable)
            {
                if (item.Value.Count != 0)
                {
                    IsEmpty = false;
                    break;
                }
            }

            if (IsEmpty == true)
            {
                return false;
            }

            foreach (KeyValuePair<int, List<Slot>> item in m_SlotMoveTable)
            {
                int nCount = item.Value.Count;

                foreach (Slot pSlot in item.Value)
                {
                    pSlot.SetSlotMove_Freezing(false);
                }

                Dictionary<Slot, SlotBlock> slotTable = new Dictionary<Slot, SlotBlock>();

                foreach (Slot pSlot in item.Value)
                {
                    if (pSlot.IsSlotMove_Freezing() == false)
                    {
                        slotTable.Add(pSlot, pSlot.GetSlot_SlotMove_Prev().GetSlotBlock());
                    }
                }

                foreach (KeyValuePair<Slot, SlotBlock> item_SlotMove in slotTable)
                {
                    if (item_SlotMove.Value == null)
                    {
                        item_SlotMove.Key.OnSlotMove(item_SlotMove.Value);
                    }
                    else
                    {
                        Slot pSlot_Prev = item_SlotMove.Value.GetSlot();

                        if (pSlot_Prev.IsSlotMove_Freezing() == false)
                        {
                            item_SlotMove.Key.OnSlotMove(item_SlotMove.Value);
                        }
                        else
                        {
                            item_SlotMove.Key.SetSlotBlock(null);
                        }
                    }
                }

                foreach (Slot pSlot in item.Value)
                {
                    if (pSlot.IsSlotMove_Freezing() == true)
                    {
                        if (pSlot.GetSlotMoveRoad() != eSlotMoveRoad.Ender)
                        {
                            pSlot.OnInPossibleSlotMoveAction();
                        }
                        else
                        {
                            pSlot.OnInPossibleSlotMoveAction_ForLast();
                        }
                    }
                }
            }
        }

        return true;
    }

	public static bool IsNormalBlock(Slot pSlot)
	{
		if (pSlot != null && pSlot.GetSlotBlock() != null)
		{
			if (pSlot.GetSlotBlock().GetSpecialItem() == eSpecialItem.None)
			{
                int nType = (int)pSlot.GetSlotBlock().GetBlockType();
                if (nType >= GameDefine.ms_nBlockStart && nType < GameDefine.ms_nBlockStart + InGameSetting.Instance.m_pInGameSettingData.m_nBlockCount)
                {
                    return true;
                }
			}
		}

		return false;
	}

    public static bool IsElementBlock(Slot pSlot)
    {
        if (pSlot != null && pSlot.GetSlotBlock() != null)
        {
            int nType = (int)pSlot.GetSlotBlock().GetBlockType();
            if (nType >= GameDefine.ms_nBlockStart && nType < GameDefine.ms_nBlockStart + InGameSetting.Instance.m_pInGameSettingData.m_nBlockCount)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsCombinationBlock(Slot pSlot)
    {
        if (pSlot != null && pSlot.GetSlotBlock() != null)
        {
            int nType = (int)pSlot.GetSlotBlock().GetBlockType();
            if (nType >= GameDefine.ms_nBlockStart && nType < GameDefine.ms_nBlockStart + InGameSetting.Instance.m_pInGameSettingData.m_nBlockCount)
            {
                return true;
            }
        }

        return false;
    }

    public static bool isCombinationAtPossibleChangeSpecialBlock(Slot pSlot)     // 결합시 특수 유닛을 변할수 있는 유닛인지?
    {
        if (pSlot != null && pSlot.GetSlotBlock() != null)
        {
            if (pSlot.GetSlotBlock().GetSpecialItem() == eSpecialItem.None)
            {
                int nType = (int)pSlot.GetSlotBlock().GetBlockType();
                if (nType >= GameDefine.ms_nBlockStart && nType < GameDefine.ms_nBlockStart + InGameSetting.Instance.m_pInGameSettingData.m_nBlockCount)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsSpecialBlock(Slot pSlot)
    {
        if (pSlot != null && pSlot.GetSlotBlock() != null)
        {
            if (pSlot.GetSlotBlock().GetSpecialItem() != eSpecialItem.None)
            {
                int nType = (int)pSlot.GetSlotBlock().GetBlockType();
                if ((nType >= GameDefine.ms_nBlockStart && nType < GameDefine.ms_nBlockStart + InGameSetting.Instance.m_pInGameSettingData.m_nBlockCount) ||
                    nType == (int)eBlockType.MatchColor)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsObstacle_OnlyMineBreak(Slot pSlot)
    {
        SlotFixObject_Obstacle pObstacle_OnlyMineBreak = pSlot.FindFixObject_SlotDyingAtOnlyMineBreak();
        if (pObstacle_OnlyMineBreak != null)
        {
            return true;
        }

        return false;
    }

    public static bool IsObstacle_MatchPossible(Slot pSlot)
    {
		//SlotFixObject_Obstacle pObstacle = pSlot.GetLastSlotFixObject() as SlotFixObject_Obstacle;

		//if (pObstacle != null && pObstacle.IsSlotBlockMatchPossible() == false)
		//{
		//	return false;
		//}

		//return true;

        return pSlot.IsSlotBlockMatchPossible();
    }

    public static bool IsPossibleMove(Slot pSlot)
    {
        if (pSlot.GetSlotBlock() != null)
        {
            return true;
        }

        SlotFixObject pSlotFixObject = pSlot.GetLastSlotFixObject();

        if (pSlotFixObject != null && pSlotFixObject.IsMoveAndCreateInclude() == true)
        {
            return true;
        }

        return false;
    }
}
