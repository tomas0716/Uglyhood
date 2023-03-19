using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Slot : MonoBehaviour
{
    private SlotManager         m_pSlotManager                      = null;
    private MainGame            m_pMainGame                         = null;

    private Plane2D             m_Plane_Slot                        = null;

    private int                 m_nSlotIndex                        = 0;
    private int                 m_nX                                = 0;
    private int                 m_nY                                = 0;

    private float               m_fIntervalHeightPos                = 0;
    private Vector2             m_vPos                              = Vector2.zero;

    private eSlotFillWay        m_eSlotFillWay                      = eSlotFillWay.Normal;

    private bool                m_IsFillBlock                       = true;
    private bool                m_IsPossibleMove                    = true;

    private eSlotLinkType       m_eSlotLinkType_In                  = eSlotLinkType.None;
    private eSlotLinkType       m_eSlotLinkType_Out                 = eSlotLinkType.None;
    private Slot                m_pSlotLink                         = null;
    private bool                m_IsSlotLink_FromSpawn              = false;

    private SlotBlock           m_pSlotBlock                        = null;
    private SlotBlock           m_pSlotBlock_Die                    = null;
    private SlotBlock           m_pSlotBlock_ShuffleBackup          = null;

    private Transformer_Timer   m_pTimer_Shuffle_Remove             = new Transformer_Timer();
    private Transformer_Timer   m_pTimer_SlotBlock_Die              = new Transformer_Timer();

    private bool                m_IsSlotDying                       = false;
    private bool                m_IsRemoveSchedule                  = false;

    private Slot[]              m_pNeighborSlots                    = new Slot[(int)eNeighbor.eMax];

    private eSlotMoveRoad       m_eSlotMoveRoad                     = eSlotMoveRoad.None;
    private bool                m_IsSlotMoveHeadTailEqual           = false;
    private int                 m_nSlotMoveIndex                    = -1;
    private Slot                m_pSlot_SlotMove_Prev               = null;
    private Slot                m_pSlot_SlotMove_Next               = null;
    private eNeighbor           m_eNeighbor_SlotMove_Prev           = eNeighbor.Neighbor_00;
    private eNeighbor           m_eNeighbor_SlotMove_Next           = eNeighbor.Neighbor_00;

    private bool                m_IsSlotMove_Freezing               = false;

    private float               m_fClickTime                        = 0;

    private List<SlotFixObject> m_SlotFixObjectList                 = new List<SlotFixObject>();

    private object              m_pParameta                         = null;

    public GameObject           m_pGameObject_SelectMode            = null;

    private Color               m_CreateColor_RndSlotBlock          = Color.white;

    void Awake()
    {
    }

    public void Init(SlotManager pSlotManager, MainGame pMainGame, int nSlotInex, int nX, int nY, Vector2 vPos, eSlotFillWay eFillWay)
    {
        ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);

        m_pSlotManager = pSlotManager;
        m_pMainGame = pMainGame;
        m_nSlotIndex = nSlotInex;
        m_nX = nX;
        m_nY = nY;
        m_vPos = vPos;
        m_eSlotFillWay = eFillWay;

        GameObject ob;

        //if (nY != InGameInfo.Instance.m_nSlotStartIndex_Y)
        {
            if ((nX + nY) % 2 == 0)
            {
                ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Plane2D_Slot_01");
                ob = GameObject.Instantiate(ob);
                ob.transform.SetParent(gameObject.transform);
                ob.transform.localPosition = Vector3.zero;
                ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

                m_Plane_Slot = ob.GetComponent<Plane2D>();
                Helper.Change_Plane2D_AtlasInfo(m_Plane_Slot, "Slot_" + pStageInfo.m_strStageTheme, "Block_BG_0");
            }
            else
            {
                ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Plane2D_Slot_02");
                ob = GameObject.Instantiate(ob);
                ob.transform.SetParent(gameObject.transform);
                ob.transform.localPosition = Vector3.zero;
                ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

                m_Plane_Slot = ob.GetComponent<Plane2D>();
                Helper.Change_Plane2D_AtlasInfo(m_Plane_Slot, "Slot_" + pStageInfo.m_strStageTheme, "Block_BG_1");
            }

            if (nY != InGameInfo.Instance.m_nSlotStartIndex_Y)
            {
                m_Plane_Slot.AddCallback_LButtonDown(OnCallback_LButtonDown);
                m_Plane_Slot.AddCallback_LButtonUp(OnCallback_LButtonUp);
                m_Plane_Slot.AddCallback_Move(OnCallback_Move);
            }
        }

        for (int i = 0; i < (int)eNeighbor.eMax; ++i)
        {
            m_pNeighborSlots[i] = null;
        }


        ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/SelectMode");
        m_pGameObject_SelectMode = GameObject.Instantiate(ob);
        m_pGameObject_SelectMode.transform.SetParent(gameObject.transform);
        m_pGameObject_SelectMode.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
        m_pGameObject_SelectMode.SetActive(false);

        EventDelegateManager.Instance.OnEventInGame_ChangeAutoPlay += OnInGame_ChangeAutoPlay;
    }

    public void OnDestroy()
    {
        EventDelegateManager.Instance.OnEventInGame_ChangeAutoPlay -= OnInGame_ChangeAutoPlay;

        if (m_pSlotBlock != null)
        {
            m_pSlotBlock.OnDestroy();
            m_pSlotBlock = null;
        }

        foreach (SlotFixObject pSlotFixObject in m_SlotFixObjectList)
        {
            pSlotFixObject.OnDestroy();
        }

        m_SlotFixObjectList.Clear();
    }

    public void Update()
    {
        if (m_pSlotBlock != null)
        {
            m_pSlotBlock.Update(Time.deltaTime);
        }

        if (m_pSlotBlock_Die != null)
        {
            m_pSlotBlock_Die.Update(Time.deltaTime);
        }

        if (m_pSlotBlock_ShuffleBackup != null)
        {
            m_pSlotBlock_ShuffleBackup.Update(Time.deltaTime);
        }

        m_pTimer_Shuffle_Remove.Update(Time.deltaTime);
        m_pTimer_SlotBlock_Die.Update(Time.deltaTime);

        foreach (SlotFixObject pObject in m_SlotFixObjectList)
        {
            pObject.Update(Time.deltaTime);
        }
    }

    public Plane2D GetPlane_Slot()
    {
        return m_Plane_Slot;
    }

    public void InitSlotBlock(eBlockType eType, eSpecialItem eSpecialItem)
    {
        m_pSlotBlock = new SlotBlock(m_pSlotManager, this, eType, eSpecialItem);
        m_pSlotBlock.SetPosition(m_vPos);

        m_pSlotBlock.SetColor(m_CreateColor_RndSlotBlock, 0, 0);
    }

    public void SetSlotLink(Slot pSlot)
    {
        m_pSlotLink = pSlot;
    }

    public Slot GetSlotLink()
    {
        return m_pSlotLink;
    }

    public void SetSlotLink_FromSpawn(bool IsSlotLink_FromSpawn)
    {
        m_IsSlotLink_FromSpawn = IsSlotLink_FromSpawn;
    }

    public bool IsSlotLink_FromSpawn()
    {
        return m_IsSlotLink_FromSpawn;
    }

    public bool IsSlotLink()
    {
        return m_eSlotLinkType_In != eSlotLinkType.None || m_eSlotLinkType_Out != eSlotLinkType.None;
    }

    public int GetSlotIndex()
    {
        return m_nSlotIndex;
    }

    public int GetX()
    {
        return m_nX;
    }

    public int GetY()
    {
        return m_nY;
    }

    public  void SetSlotFillWay(eSlotFillWay eFillWay)
    {
        m_eSlotFillWay = eFillWay;
    }

    public eSlotFillWay GetSlotFillWay()
    {
        return m_eSlotFillWay;
    }

    public bool IsLeftSide()
    {
        return m_nX == InGameInfo.Instance.m_nSlotStartIndex_X ? true : false;
    }

    public bool IsRightSide()
    {
        return m_nX == InGameInfo.Instance.m_nSlotEndIndex_X ? true : false;
    }

    public bool IsTopSide()
    {
        return m_nY == InGameInfo.Instance.m_nSlotStartIndex_Y ? true : false;
    }

    public bool IsBottomSide()
    {
        return m_nY== InGameInfo.Instance.m_nSlotEndIndex_Y ? true : false;
    }

    public bool IsLeftSecondSide()
    {
        return m_nX - 1 == InGameInfo.Instance.m_nSlotStartIndex_X ? true : false;
    }

    public bool IsRightSecondSide()
    {
        return m_nX + 1== InGameInfo.Instance.m_nSlotEndIndex_X ? true : false;
    }

    public bool IsTopSecondSide()
    {
        return m_nY - 1== InGameInfo.Instance.m_nSlotStartIndex_Y ? true : false;
    }

    public bool IsBottomSecondSide()
    {
        return m_nY + 1== InGameInfo.Instance.m_nSlotEndIndex_Y ? true : false;
    }

    public void SetCreateColor_RndSlotBlock(Color color)
    {
        m_CreateColor_RndSlotBlock = color;
    }

    public void ChangeSlotBlock(eBlockType eType)
    {
        eSpecialItem eSpecialItem = eSpecialItem.None;
        if (m_pSlotBlock != null)
        {
            eSpecialItem = m_pSlotBlock.GetSpecialItem();
            m_pSlotBlock.OnDestroy();
        }

        m_pSlotBlock = new SlotBlock(m_pSlotManager, this, eType, eSpecialItem);
        m_pSlotBlock.SetPosition(m_vPos);

        m_pSlotBlock.SetColor(m_CreateColor_RndSlotBlock, 0, 0);
    }

    public void ChangeSlotBlock(eBlockType eType, eSpecialItem eSpecialItem)
    {
        if (m_pSlotBlock != null)
        {
            m_pSlotBlock.OnDestroy();
        }

        m_pSlotBlock = new SlotBlock(m_pSlotManager, this, eType, eSpecialItem);
        m_pSlotBlock.SetPosition(m_vPos);

        m_pSlotBlock.SetColor(m_CreateColor_RndSlotBlock, 0, 0);
    }

    public void ChangeSlotBlock(eSpecialItem eSpecialItem)
    {
        eBlockType eType = eBlockType.Block_Start;
        if (m_pSlotBlock != null)
        {
            eType = m_pSlotBlock.GetBlockType();
            m_pSlotBlock.OnDestroy();
        }

        m_pSlotBlock = new SlotBlock(m_pSlotManager, this, eType, eSpecialItem);
        m_pSlotBlock.SetPosition(m_vPos);

        m_pSlotBlock.SetColor(m_CreateColor_RndSlotBlock, 0, 0);
    }

    public void ChangeSlotBlock(eMapSlotItem eSlotItem, int nLayer)
    {
        switch (eSlotItem)
        {
            case eMapSlotItem.Box:
                {
                    RemoveSlotBlock();

                    SlotFixObject_Box pSlotFixObject = new SlotFixObject_Box(this, nLayer);
                    AddSlotFixObject(pSlotFixObject);
                }
                break;

            case eMapSlotItem.Chain:
                {
                    SlotFixObject_Chain pSlotFixObject = new SlotFixObject_Chain(this, nLayer);
                    AddSlotFixObject(pSlotFixObject);
                }
                break;

            case eMapSlotItem.Tire:
                {
                    RemoveSlotBlock();

                    eSpecialItem eSpecialItem = eSpecialItem.None;

                    m_pSlotBlock = new SlotBlock(m_pSlotManager, this, eBlockType.Custom, eSpecialItem, eSlotItem);
                    m_pSlotBlock.SetPosition(m_vPos);

                    m_pSlotBlock.SetColor(m_CreateColor_RndSlotBlock, 0, 0);
                }
                break;

            case eMapSlotItem.Ice:
                {
                    SlotFixObject_Ice pSlotFixObject = new SlotFixObject_Ice(this, nLayer);
                    AddSlotFixObject(pSlotFixObject);
                }
                break;
        }
    }

    public void ChangeSlotBlock(InGameStageMapSlotData pInGameStageMapSlotData, bool IsCreate = false)
    {
        switch (pInGameStageMapSlotData.m_eMapSlotItem)
        {
            case eMapSlotItem.Box:
                {
                    RemoveSlotBlock();

                    SlotFixObject_Box pSlotFixObject = new SlotFixObject_Box(this, pInGameStageMapSlotData.m_pInGameStageMapMetaData.m_nMetaData);
                    AddSlotFixObject(pSlotFixObject);
                }
                break;

            case eMapSlotItem.Chain:
                {
                    SlotFixObject_Chain pSlotFixObject = new SlotFixObject_Chain(this, pInGameStageMapSlotData.m_pInGameStageMapMetaData.m_nMetaData);
                    AddSlotFixObject(pSlotFixObject);
                }
                break;

            case eMapSlotItem.Tire:
                {
                    RemoveSlotBlock();

                    eSpecialItem eSpecialItem = eSpecialItem.None;

                    m_pSlotBlock = new SlotBlock(m_pSlotManager, this, eBlockType.Custom, eSpecialItem, pInGameStageMapSlotData.m_eMapSlotItem);
                    m_pSlotBlock.SetPosition(m_vPos);

                    m_pSlotBlock.SetColor(m_CreateColor_RndSlotBlock, 0, 0);
                }
                break;

            case eMapSlotItem.Ice:
                {
                    SlotFixObject_Ice pSlotFixObject = new SlotFixObject_Ice(this, pInGameStageMapSlotData.m_pInGameStageMapMetaData.m_nMetaData);
                    AddSlotFixObject(pSlotFixObject);
                }
                break;
        }
    }

    public void OnCreateSlotBlock(eMapSlotItem eSlotItem, float fMoveTime, bool IsInverse, int nInterval = 1)
    {
        switch (eSlotItem)
        {
            case eMapSlotItem.Tire:
                {
                    RemoveSlotBlock();

                    eSpecialItem eSpecialItem = eSpecialItem.None;

                    m_pSlotBlock = new SlotBlock(m_pSlotManager, this, eBlockType.Custom, eSpecialItem, eSlotItem);
                    m_pSlotBlock.OnCreate(m_vPos, fMoveTime, nInterval, IsInverse);

                    m_pSlotBlock.SetColor(m_CreateColor_RndSlotBlock, 0, 0);
                }
                break;
        }
    }

    public void OnCreateSlotBlock(eBlockType eType, float fMoveTime, bool IsInverse, int nInterval = 1)
    {
        RemoveSlotBlock();

        m_pSlotBlock = new SlotBlock(m_pSlotManager, this, eType, eSpecialItem.None);
        m_pSlotBlock.OnCreate(m_vPos, fMoveTime, nInterval, IsInverse);

        m_pSlotBlock.SetColor(m_CreateColor_RndSlotBlock, 0, 0);
    }

    public void OnCreateSlotBlock(eBlockType eType, eSpecialItem eSpecialItem, float fMoveTime, bool IsInverse, int nInterval = 1)
    {
        RemoveSlotBlock();

        m_pSlotBlock = new SlotBlock(m_pSlotManager, this, eType, eSpecialItem);
        m_pSlotBlock.OnCreate(m_vPos, fMoveTime, nInterval, IsInverse);

        m_pSlotBlock.SetColor(m_CreateColor_RndSlotBlock, 0, 0);
    }

    public void RemoveSlotBlock()
    {
        if (m_pSlotBlock != null)
        {
            m_pSlotBlock.OnDestroy();
            m_pSlotBlock = null;
        }
    }

    public void OnShuffleBackup()
    {
        m_pSlotBlock_ShuffleBackup = new SlotBlock(m_pSlotManager, this, m_pSlotBlock.GetBlockType(), m_pSlotBlock.GetSpecialItem());
        m_pSlotBlock_ShuffleBackup.OnCreate(m_vPos, 0, 0, false);

        m_pSlotBlock_ShuffleBackup.SetColor(m_CreateColor_RndSlotBlock, 0, 0);
    }

    private eBlockType m_eBlockType_Shuffle = eBlockType.Block_Start;

    public void SetShuffleBlockType(eBlockType eType)
    {
        m_eBlockType_Shuffle = eType;
    }

    public eBlockType GetShuffleBlockType()
    {
        return m_eBlockType_Shuffle;
    }

    public void ChangeSlotBlock_Shuffle(eBlockType eType)
    {
        eSpecialItem eSpecialItem = eSpecialItem.None;
        if (m_pSlotBlock != null)
        {
            eSpecialItem = m_pSlotBlock.GetSpecialItem();
            m_pSlotBlock.OnDestroy();
        }

        m_pSlotBlock = new SlotBlock(m_pSlotManager, this, eType, eSpecialItem);
        m_pSlotBlock.SetPosition(m_vPos);
        m_pSlotBlock.SetChangeShuffle();

        m_pSlotBlock.SetColor(m_CreateColor_RndSlotBlock, 0, 0);
    }

    public void OnShuffle()
    {
        if (m_pSlotBlock_ShuffleBackup != null)
        {
            m_pSlotBlock_ShuffleBackup.OnChangeShuffleRemove();
        }

        if (m_pSlotBlock != null)
        {
            m_pSlotBlock.OnCheckShffleCreate();
        }

        m_pTimer_Shuffle_Remove.OnReset();
        TransformerEvent eventValue;
        eventValue = new TransformerEvent_Timer(0.3f);
        m_pTimer_Shuffle_Remove.AddEvent(eventValue);
        m_pTimer_Shuffle_Remove.SetCallback(null, OnDone_Timer_Shuffle_Remove);
        m_pTimer_Shuffle_Remove.OnPlay();
    }

    private void OnDone_Timer_Shuffle_Remove(TransformerEvent eventValue)
    {
        if (m_pSlotBlock_ShuffleBackup != null)
        {
            m_pSlotBlock_ShuffleBackup.OnDestroy();
            m_pSlotBlock_ShuffleBackup = null;
        }
    }

    public void SetNeighbor(eNeighbor eNe, Slot pSlot)
    {
        m_pNeighborSlots[(int)eNe] = pSlot;
    }

    public Slot GetNeighborSlot(eNeighbor eNe)
    {
        return m_pNeighborSlots[(int)eNe];
    }

    public bool IsNeighborSlot_Cross(Slot pSlot)
    {
        if (m_pNeighborSlots[(int)eNeighbor.Neighbor_10] == pSlot)
            return true;

        if (m_pNeighborSlots[(int)eNeighbor.Neighbor_01] == pSlot)
            return true;

        if (m_pNeighborSlots[(int)eNeighbor.Neighbor_21] == pSlot)
            return true;

        if (m_pNeighborSlots[(int)eNeighbor.Neighbor_12] == pSlot)
            return true;

        return false;
    }

    public void ChangeBlockShape_byMatch(eSpecialItem specialItem)
    {
        if (m_pSlotBlock != null)
        {
            eBlockType eType = m_pSlotBlock.GetBlockType();

            if (m_pSlotBlock_Die != null)
            {
                m_pSlotBlock_Die.OnDestroy();
            }

            m_pSlotBlock_Die = m_pSlotBlock;
            m_pSlotBlock_Die.ChangeSlotBlockGameObjectName("SlotBlock_Die");

            m_pTimer_SlotBlock_Die.OnReset();
            TransformerEvent_Timer eventValue;
            eventValue = new TransformerEvent_Timer(0.5f);
            m_pTimer_SlotBlock_Die.AddEvent(eventValue);
            m_pTimer_SlotBlock_Die.SetCallback(null, OnDone_Timer_SlotBlock_Die);
            m_pTimer_SlotBlock_Die.OnPlay();

            m_pSlotBlock = new SlotBlock(m_pSlotManager, this, eType, specialItem);
            m_pSlotBlock.OnCreate(m_vPos, 0, 0, false);

            m_pSlotBlock.SetColor(m_CreateColor_RndSlotBlock, 0, 0);
        }
    }

    public void SetSlotBlock(SlotBlock pSlotBlock)
    {
        m_pSlotBlock = pSlotBlock;
    }

    public void SetTempSlotBlock(SlotBlock pSlotBlock)
    {
        m_pSlotBlock = pSlotBlock;

        if (pSlotBlock != null)
        {
            pSlotBlock.SetTempSlot(this);
        }
    }

    public SlotBlock GetSlotBlock()
    {
        return m_pSlotBlock;
    }

    public void AddSlotFixObject_Cloud(SlotFixObject pSlotFixObject)
    {
        if (pSlotFixObject.IsSlotBlockExist() == false)
        {
            RemoveSlotBlock();
        }

        if (m_SlotFixObjectList.Contains(pSlotFixObject) == false)
        {
            m_SlotFixObjectList.Insert(0, pSlotFixObject);
        }

        pSlotFixObject.SetPosition(m_vPos);
    }

    public void AddSlotFixObject(SlotFixObject pSlotFixObject)
    {
        if (pSlotFixObject.IsSlotBlockExist() == false)
        {
            RemoveSlotBlock();
        }

        if (m_SlotFixObjectList.Contains(pSlotFixObject) == false)
        {
            m_SlotFixObjectList.Add(pSlotFixObject);
        }

        pSlotFixObject.SetPosition(m_vPos);

        m_IsFillBlock = false;
        m_IsPossibleMove = false;
    }

    public void RemoveSlotFixObject(SlotFixObject pSlotFixObject)
    {
        if (m_SlotFixObjectList.Contains(pSlotFixObject) == true)
        {
            pSlotFixObject.OnDestroy();
            m_SlotFixObjectList.Remove(pSlotFixObject);
        }

        if (m_SlotFixObjectList.Count == 0)
        {
            m_IsFillBlock = true;
            m_IsPossibleMove = true;
        }
        else if(m_SlotFixObjectList.Count == 1)
        {
            SlotFixObject_Obstacle pObstacle = m_SlotFixObjectList[0] as SlotFixObject_Obstacle;

            if (pObstacle != null && pObstacle.GetObstacleType() == eSlotFixObjectObstacleType.Cloud)
            {
                m_IsFillBlock = true;
                m_IsPossibleMove = true;
            }
        }
    }

    public void RemoveAllSlotFixObject()
    {
        foreach (SlotFixObject pSlotFixObject in m_SlotFixObjectList)
        {
            pSlotFixObject.OnDestroy();
        }

        m_SlotFixObjectList.Clear();

        m_IsFillBlock = true;
        m_IsPossibleMove = true;
    }

    public void PopSlotFixObject(SlotFixObject pSlotFixObject)
    {
        if (m_SlotFixObjectList.Contains(pSlotFixObject) == true)
        {
            m_SlotFixObjectList.Remove(pSlotFixObject);
        }

        if (m_SlotFixObjectList.Count == 0)
        {
            m_IsFillBlock = true;
            m_IsPossibleMove = true;
        }
        else if (m_SlotFixObjectList.Count == 1)
        {
            SlotFixObject_Obstacle pObstacle = m_SlotFixObjectList[0] as SlotFixObject_Obstacle;

            if (pObstacle != null && pObstacle.GetObstacleType() == eSlotFixObjectObstacleType.Cloud)
            {
                m_IsFillBlock = true;
                m_IsPossibleMove = true;
            }
        }
    }

    public List<SlotFixObject> GetSlotFixObjectList()
    {
        return m_SlotFixObjectList;
    }

    public SlotFixObject GetLastSlotFixObject()
    {
        if (m_SlotFixObjectList.Count == 0)
        {
            return null;
        }

        return m_SlotFixObjectList[m_SlotFixObjectList.Count - 1];
    }

    public void SetRemoveSchedule(bool isRemvoeSchedule)
    {
        m_IsRemoveSchedule = isRemvoeSchedule;
    }

    public bool IsRemoveSchedule()
    {
        return m_IsRemoveSchedule;
    }

    public void SetSlotDying(bool IsSlotDying)
    {
        m_IsSlotDying = IsSlotDying;
    }

    public bool IsSlotDying()
    {
        return m_IsSlotDying;
    }

    public SlotFixObject_Obstacle FindFixObject_SlotDyingAtOnlyMineBreak()
    {
        int nNumSlotFixObject = m_SlotFixObjectList.Count;
        for(int i = nNumSlotFixObject - 1; i >= 0; --i)
        {
            SlotFixObject_Obstacle pObstacle = m_SlotFixObjectList[i] as SlotFixObject_Obstacle;

            if (pObstacle != null && pObstacle.IsSlotDyingAtOnlyMineBreak() == true)
            {
                return pObstacle;
            }
        }

        return null;
    }

    public bool IsSlotBlockMatchPossible()
    {
        int nNumSlotFixObject = m_SlotFixObjectList.Count;
        for (int i = nNumSlotFixObject - 1; i >= 0; --i)
        {
            SlotFixObject_Obstacle pObstacle = m_SlotFixObjectList[i] as SlotFixObject_Obstacle;

            if (pObstacle != null && pObstacle.IsSlotBlockMatchPossible() == false)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsSlotBlockMatchInPossibleAtCheckRemovePossible()
    {
        int nNumSlotFixObject = m_SlotFixObjectList.Count;
        for (int i = nNumSlotFixObject - 1; i >= 0; --i)
        {
            SlotFixObject_Obstacle pObstacle = m_SlotFixObjectList[i] as SlotFixObject_Obstacle;

            if (pObstacle != null && pObstacle.IsSlotBlockMatchInPossibleAtCheckRemovePossible() == false)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsSlotBlockNoSwap()
    {
        int nNumSlotFixObject = m_SlotFixObjectList.Count;
        for (int i = nNumSlotFixObject - 1; i >= 0; --i)
        {
            SlotFixObject_Obstacle pObstacle = m_SlotFixObjectList[i] as SlotFixObject_Obstacle;

            if (pObstacle != null && pObstacle.IsSlotBlockNoSwap() == true)
            {
                return true;
            }
        }

        return false;
    }

    public SlotFixObject_Obstacle FindFixObject_NeighborSlotBlockDyingAtBreak()
    {
        int nNumSlotFixObject = m_SlotFixObjectList.Count;
        for (int i = nNumSlotFixObject - 1; i >= 0; --i)
        {
            SlotFixObject_Obstacle pObstacle = m_SlotFixObjectList[i] as SlotFixObject_Obstacle;

            if (pObstacle != null && pObstacle.IsNeighborSlotBlockDyingAtBreak() == true)
            {
                return pObstacle;
            }
        }

        return null;
    }

    public bool IsSlotFixObject_Obstacle()
    {
        foreach (SlotFixObject pSlotFixObject in m_SlotFixObjectList)
        {
            SlotFixObject_Obstacle pObstacle = pSlotFixObject as SlotFixObject_Obstacle;

            if(pObstacle != null && pObstacle.GetObstacleType() != eSlotFixObjectObstacleType.Cloud)
                return true;
        }

        return false;
    }


    public void OnSlotDying()
    {
        if (m_IsSlotDying == true)
        {
            SlotFixObject_Obstacle pObstacle_OnlyMineBreak = FindFixObject_SlotDyingAtOnlyMineBreak();
            SlotFixObject_Obstacle pObstacle_NeighborSlotBlockDyingAtBreak = FindFixObject_NeighborSlotBlockDyingAtBreak();

            if (m_pSlotBlock != null && pObstacle_OnlyMineBreak == null)
            {
                if (m_pSlotBlock.GetBlockType() == eBlockType.Custom)
                {
                    switch (m_pSlotBlock.GetMapSlotItem())
                    {
                        case eMapSlotItem.Tire:
                            {
                                ParticleManager.Instance.LoadParticleSystem("FX_Obstacle_Tire_Destroy", m_vPos).SetScale(InGameInfo.Instance.m_fInGameScale);
                            }
                            break;
                    }
                }

                m_pMainGame.OnBeforeBlockDying(this);

                if (m_pSlotBlock_Die != null)
                {
                    m_pSlotBlock_Die.OnDestroy();
                }

                m_pSlotBlock_Die = m_pSlotBlock;
                m_pSlotBlock_Die.ChangeSlotBlockGameObjectName("SlotBlock_Die");
                m_pSlotBlock = null;

                m_pMainGame.OnAfterBlockDying(this);

                m_pTimer_SlotBlock_Die.OnReset();
                TransformerEvent_Timer eventValue;
                eventValue = new TransformerEvent_Timer(0.3f);
                m_pTimer_SlotBlock_Die.AddEvent(eventValue);
                m_pTimer_SlotBlock_Die.SetCallback(null, OnDone_Timer_SlotBlock_Die);
                m_pTimer_SlotBlock_Die.OnPlay();
            }
            else
            {
                if(pObstacle_OnlyMineBreak != null)
                    pObstacle_OnlyMineBreak.OnBreak();
                else if(pObstacle_NeighborSlotBlockDyingAtBreak != null)
                    pObstacle_NeighborSlotBlockDyingAtBreak.OnBreak();
            }
        }
    }

	public void OnBreak()
	{
	}

	private void OnDone_Timer_SlotBlock_Die(TransformerEvent eventValue)
    {
        m_pSlotBlock_Die.OnDestroy();
        m_pSlotBlock_Die = null;
    }

    public void OnSlotBlockRemove()
    {
        if (m_pSlotBlock != null)
        {
            m_pSlotBlock.OnDestroy();
            m_pSlotBlock = null;
        }
    }

    public static void OnMove(Slot pFromSlot, Slot pToSlot, float fMoveTime)
    {
        if (pFromSlot.GetSlotBlock() != null)
        {
            pToSlot.SetSlotBlock(pFromSlot.GetSlotBlock());
            pToSlot.GetSlotBlock().OnMove(pFromSlot, pToSlot, fMoveTime);
            pFromSlot.SetSlotBlock(null);
        }
        else
        {
            SlotFixObject pSlotFixObject = pFromSlot.GetLastSlotFixObject();

            if (pSlotFixObject != null && pSlotFixObject.IsMoveAndCreateInclude() == true)
            {
                GameEvent_InGame_SlotFixObjectSlotMove pGameEvent = new GameEvent_InGame_SlotFixObjectSlotMove(pToSlot.m_pSlotManager, pSlotFixObject, pFromSlot, pToSlot, fMoveTime);
                AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
            }
        }
    }

    public static void OnLinkMove(Slot pFromSlot, Slot pToSlot, float fMoveTime, bool IsReverse)
    {
        if (pFromSlot.GetSlotBlock() != null)
        {
            pToSlot.SetSlotBlock(pFromSlot.GetSlotBlock());
            pToSlot.GetSlotBlock().OnLinkMove(pFromSlot, pToSlot, fMoveTime, IsReverse);
            pFromSlot.SetSlotBlock(null);
        }
        else
        {
            SlotFixObject pSlotFixObject = pFromSlot.GetLastSlotFixObject();

            if (pSlotFixObject != null && pSlotFixObject.IsMoveAndCreateInclude() == true)
            {
                GameEvent_InGame_SlotFixObjectSlotLinkMove pGameEvent = new GameEvent_InGame_SlotFixObjectSlotLinkMove(pToSlot.m_pSlotManager, pSlotFixObject, pFromSlot, pToSlot, fMoveTime);
                AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
            }
        }
    }

    public static void OnShuffleMove(Slot pSlot, SlotBlock pSlotBlock, float fMoveTime)
    {
        Slot pScrSlot = pSlotBlock.GetSlot();
        pSlot.SetSlotBlock(pSlotBlock);
        pSlotBlock.OnMove(pScrSlot, pSlot, fMoveTime);
    }

    public void OnDone_Move()
    {
        m_pSlotManager.OnDoneMoveAndCreate(this);
    }

    public void OnDone_PossibleBlockSwapAction()
    {
        m_pSlotManager.OnDonePossibleBlockSwapAction(this);
    }

    public void OnMoveComplete()
    {
        if (m_pSlotBlock != null)
        {
            Helper.OnSoundPlay("INGAME_BLOCK_MOVE_END", false);

            m_pSlotBlock.OnMoveComplete();
        }
    }

    public bool IsFillBlock()        // 유닛이 비워 있는데, 채워질수 있는 상황
    {
        return m_IsFillBlock;
    }

    public bool IsPossibleMove()
    {
        return m_IsPossibleMove;
    }

    public void SetPossibleMove(bool IsPossibleMove)
    {
        m_IsPossibleMove = IsPossibleMove;
    }


    public void SetMoveFlag(bool IsMoveFlag)
    {
        if (m_pSlotBlock != null)
        {
            m_pSlotBlock.SetMoveFlag(IsMoveFlag);
        }
    }

    public bool IsMoveFlag()
    {
        if (m_pSlotBlock != null)
        {
            return m_pSlotBlock.IsMoveFlag();
        }

        return false;
    }

    public void SetIntervalHeightPos(float fHeight)
    {
        m_fIntervalHeightPos = fHeight;
    }


    public float GetIntervalHeightPos()
    {
        return m_fIntervalHeightPos;
}

    public Vector2 GetPosition()
    {
        Vector3 vRes = m_vPos;
        vRes.y += m_fIntervalHeightPos;
        return vRes;
    }

    public void PossibleBlockSwapAction(Slot pSlot)
    {
        if (m_pSlotBlock != null)
        {
            m_pSlotBlock.PossibleBlockSwapAction(pSlot);
        }
    }

    public void InPossibleBlockSwapAction(Slot pSlot)
    {
        if (m_pSlotBlock != null)
        {
            m_pSlotBlock.InPossibleBlockSwapAction(pSlot);
        }
    }

    public void SetSlotMoveRoad(eSlotMoveRoad eType)
    {
        m_eSlotMoveRoad = eType;
    }

    public eSlotMoveRoad GetSlotMoveRoad()
    {
        return m_eSlotMoveRoad;
    }

    public void SetSlotMoveHeadTailEqual(bool IsEqual)
    {
        m_IsSlotMoveHeadTailEqual = IsEqual;
    }

    public bool IsSlotMoveHeadTailEqual()
    {
        return m_IsSlotMoveHeadTailEqual;
    }

    public void SetSlotMoveIndex(int nIndex)
    {
        m_nSlotMoveIndex = nIndex;
    }

    public int GetSlotMoveIndex()
    {
        return m_nSlotMoveIndex;
    }

    public void SetSlot_SlotMove_Prev(Slot pSlot)
    {
        m_pSlot_SlotMove_Prev = pSlot;

        for (int i = 0; i < (int)eNeighbor.eMax; ++i)
        {
            if (m_pNeighborSlots[i] == m_pSlot_SlotMove_Prev)
            {
                m_eNeighbor_SlotMove_Prev = (eNeighbor)i;
                break;
            }
        }
    }

    public Slot GetSlot_SlotMove_Prev()
    {
        return m_pSlot_SlotMove_Prev;
    }

    public void SetSlot_SlotMove_Next(Slot pSlot)
    {
        m_pSlot_SlotMove_Next = pSlot;

        for (int i = 0; i < (int)eNeighbor.eMax; ++i)
        {
            if (m_pNeighborSlots[i] == m_pSlot_SlotMove_Next)
            {
                m_eNeighbor_SlotMove_Next = (eNeighbor)i;
                break;
            }
        }
    }

    public Slot GetSlot_SlotMove_Next()
    {
        return m_pSlot_SlotMove_Next;
    }

    public eNeighbor GetNeighbor_SlotMove_Prev()
    {
        return m_eNeighbor_SlotMove_Prev;
    }

    public eNeighbor GetNeighbor_SlotMove_Next()
    {
        return m_eNeighbor_SlotMove_Next;
    }

    public eNeighbor GetNeighborType(Slot pSlot)
    {
        for (int i = (int)eNeighbor.Neighbor_00; i < (int)eNeighbor.eMax; ++i)
        {
            if (pSlot == GetNeighborSlot((eNeighbor)i))
            {
                return (eNeighbor)i;
            }
        }

        return eNeighbor.Neighbor_None;
    }

    public void SetSlotMove_Freezing(bool IsFreezing)
    {
        m_IsSlotMove_Freezing = IsFreezing;
    }

    public bool IsSlotMove_Freezing()
    {
        return m_IsSlotMove_Freezing;
    }

    public void OnSlotMove(SlotBlock pSlotBlock)
    {
        m_pSlotBlock = pSlotBlock;

        if (m_pSlotBlock != null)
        {
            if (m_pSlotBlock.GetSlot().GetSlotMoveRoad() != eSlotMoveRoad.Ender)
            {
                if (m_pSlotBlock != null)
                {
                    m_pSlotBlock.OnMove(m_pSlot_SlotMove_Prev, this, GameDefine.ms_fBlockSwapMoveTime);
                }
            }
            else
            {
                if (m_pSlotBlock != null)
                {
                    m_pSlotBlock.OnWarp(m_pSlot_SlotMove_Prev, this, GameDefine.ms_fBlockSwapMoveTime);
                }
            }
        }
    }

    public void OnInPossibleSlotMoveAction()
    {
        if (IsPossibleSlotMove() == true && m_pSlotBlock != null)
        {
            m_pSlotBlock.OnInPossibleSlotMoveAction(this, m_pSlot_SlotMove_Next, GameDefine.ms_fBlockSwapMoveTime);
        }
    }

    public void OnInPossibleSlotMoveAction_ForLast()
    {
        if (IsPossibleSlotMove() == true && m_pSlotBlock != null)
        {
            m_pSlotBlock.OnInPossibleSlotMoveAction_ForLast(GameDefine.ms_fBlockSwapMoveTime);
        }
    }

    public void OnSwapPossibleDirection()
    {
        if (m_pSlotBlock != null)
        {
            m_pSlotBlock.OnSwapPossibleDirection();
        }
    }

    public void OnCancelSwapPossibleDirection()
    {
        if (m_pSlotBlock != null)
        {
            m_pSlotBlock.OnCancelSwapPossibleDirection();
        }
    }

    public bool IsCombinationActive()
    {
        if (m_pSlotBlock == null)
            return false;

        return true;
    }

    public bool IsPossibleSlotMove()
    {
        return true;
    }

    public void SetParameta(object ob)
    {
        m_pParameta = ob;
    }

    public object GetParameta()
    {
        return m_pParameta;
    }

    public void SetHighlight()
    {
        if (m_pSlotBlock != null)
        {
            m_pSlotBlock.SetHighlight(1);
        }

        Vector3 vPos = m_Plane_Slot.gameObject.transform.position;
        vPos.z = -(float)(ePlaneOrder.PlayerCharacterAttackHighlightSlot);
        m_Plane_Slot.gameObject.transform.position = vPos;

        for (int i = 0; i < m_SlotFixObjectList.Count; ++i)
        {
            SlotFixObject pSlotFixObject = m_SlotFixObjectList[i];
            pSlotFixObject.SetHighlight(i + 1);
        }
    }

    public void ClearHighlight()
    {
        if (m_pSlotBlock != null)
        {
            m_pSlotBlock.ClearHighlight();
        }

        Vector3 vPos = m_Plane_Slot.gameObject.transform.position;
        vPos.z = 0;
        m_Plane_Slot.gameObject.transform.position = vPos;

        for (int i = 0; i < m_SlotFixObjectList.Count; ++i)
        {
            SlotFixObject pSlotFixObject = m_SlotFixObjectList[i];
            pSlotFixObject.ClearHighlight();
        }
    }

    public void OnInGame_ChangeAutoPlay(bool IsAutoPlay)
    {
        InGameInfo.Instance.m_nClickSlotFingerID = -1;
        InGameInfo.Instance.m_pClickSlot = null;
    }

    protected void OnCallback_LButtonDown(GameObject gameObject, Vector3 vPos, object ob, int nFingerID)
    {
        if (InGameInfo.Instance.m_IsInGameClick == true && InGameInfo.Instance.m_IsAutoPlay == false && EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
        {
            InGameInfo.Instance.m_nClickSlotFingerID = nFingerID;
            InGameInfo.Instance.m_pClickSlot = this;
        }
    }

    protected void OnCallback_LButtonUp(GameObject gameObject, Vector3 vPos, object ob, int nFingerID, bool IsDown)
    {
        if (InGameInfo.Instance.m_IsAutoPlay == false && EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
        {
            MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

            if (IsDown == true && InGameInfo.Instance.m_nClickSlotFingerID == nFingerID)
            {
                EventDelegateManager.Instance.OnInGame_Slot_Click(this);
            }

            if ((InGameSetting.Instance.m_pInGameSettingData.m_IsAnytimeSwap == true ||
                (InGameSetting.Instance.m_pInGameSettingData.m_IsAnytimeSwap == false && InGameInfo.Instance.m_IsInGameClick == true)))
            {
                if (pDataStack.m_pCloudManager.GetCloud(this, eOwner.Other) == null)
                {
                    if (IsDown == true && InGameInfo.Instance.m_pClickSlot == this &&
                        InGameInfo.Instance.m_nClickSlotFingerID == nFingerID)
                    {
                        if (EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger == false && EspressoInfo.Instance.m_IsBoosterItemlTrigger == false)
                        {
                            bool IsSpecialItemBlock = false;

                            if (InGameSetting.Instance.m_pInGameSettingData.m_eSpecialItemUseWay == eSpecialItemUseWay.Click ||
                                InGameSetting.Instance.m_pInGameSettingData.m_eSpecialItemUseWay == eSpecialItemUseWay.MoveOrClick)
                            {
                                if (m_pSlotBlock != null && m_pSlotBlock.GetSpecialItem() != eSpecialItem.None)
                                {
                                    IsSpecialItemBlock = true;

                                    if (InGameInfo.Instance.m_pSelectModeSlot == null)
                                    {
                                        SlotFixObject_Obstacle pObstacle_OnlyMineBreak = FindFixObject_SlotDyingAtOnlyMineBreak();

                                        if (pObstacle_OnlyMineBreak == null)
                                        {
                                            m_pSlotManager.RemoveSpecialItemBlock(this);
                                            EventDelegateManager.Instance.OnInGame_BeginBlockSwap();
                                        }
                                    }
                                    else
                                    {
                                        if (InGameInfo.Instance.m_pSelectModeSlot.IsNeighborSlot_Cross(this) == true)
                                        {
                                            int nClickSlot_X = InGameInfo.Instance.m_pSelectModeSlot.GetX();
                                            int nClickSlot_Y = InGameInfo.Instance.m_pSelectModeSlot.GetY();

                                            int nRes = (m_nX - nClickSlot_X) + (m_nY - nClickSlot_Y);
                                            nRes = Mathf.Abs(nRes);

                                            if (nRes == 1)
                                            {
                                                m_pSlotManager.OnBlockSwap(InGameInfo.Instance.m_pSelectModeSlot, this);

                                                InGameInfo.Instance.m_nClickSlotFingerID = -1;
                                                InGameInfo.Instance.m_pClickSlot = null;

                                                if (InGameInfo.Instance.m_pSelectModeSlot != null)
                                                {
                                                    InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
                                                    InGameInfo.Instance.m_pSelectModeSlot = null;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            SlotFixObject_Obstacle pObstacle_OnlyMineBreak = FindFixObject_SlotDyingAtOnlyMineBreak();

                                            if (pObstacle_OnlyMineBreak == null)
                                            {
                                                if (InGameInfo.Instance.m_pSelectModeSlot != null)
                                                {
                                                    InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
                                                    InGameInfo.Instance.m_pSelectModeSlot = null;
                                                }
                                                m_pSlotManager.RemoveSpecialItemBlock(this);
                                                EventDelegateManager.Instance.OnInGame_BeginBlockSwap();
                                            }
                                        }
                                    }
                                }
                            }

                            if (GameDefine.ms_fDoubleClickTime >= Time.time - m_fClickTime)
                            {
                                if (InGameSetting.Instance.m_pInGameSettingData.m_eSpecialItemUseWay == eSpecialItemUseWay.DoubleClick ||
                                    InGameSetting.Instance.m_pInGameSettingData.m_eSpecialItemUseWay == eSpecialItemUseWay.MoveOrDoubleClick)
                                {
                                    if (m_pSlotBlock != null && m_pSlotBlock.GetSpecialItem() != eSpecialItem.None)
                                    {
                                        IsSpecialItemBlock = true;

                                        if (InGameInfo.Instance.m_pSelectModeSlot == null)
                                        {
                                            SlotFixObject_Obstacle pObstacle_OnlyMineBreak = FindFixObject_SlotDyingAtOnlyMineBreak();

                                            if (pObstacle_OnlyMineBreak == null)
                                            {
                                                m_pSlotManager.RemoveSpecialItemBlock(this);
                                                EventDelegateManager.Instance.OnInGame_BeginBlockSwap();
                                            }
                                        }
                                        else
                                        {
                                            if (InGameInfo.Instance.m_pSelectModeSlot.IsNeighborSlot_Cross(this) == true)
                                            {
                                                int nClickSlot_X = InGameInfo.Instance.m_pSelectModeSlot.GetX();
                                                int nClickSlot_Y = InGameInfo.Instance.m_pSelectModeSlot.GetY();

                                                int nRes = (m_nX - nClickSlot_X) + (m_nY - nClickSlot_Y);
                                                nRes = Mathf.Abs(nRes);

                                                if (nRes == 1)
                                                {
                                                    m_pSlotManager.OnBlockSwap(InGameInfo.Instance.m_pSelectModeSlot, this);

                                                    InGameInfo.Instance.m_nClickSlotFingerID = -1;
                                                    InGameInfo.Instance.m_pClickSlot = null;

                                                    if (InGameInfo.Instance.m_pSelectModeSlot != null)
                                                    {
                                                        InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
                                                        InGameInfo.Instance.m_pSelectModeSlot = null;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SlotFixObject_Obstacle pObstacle_OnlyMineBreak = FindFixObject_SlotDyingAtOnlyMineBreak();

                                                if (pObstacle_OnlyMineBreak == null)
                                                {
                                                    if (InGameInfo.Instance.m_pSelectModeSlot != null)
                                                    {
                                                        InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
                                                        InGameInfo.Instance.m_pSelectModeSlot = null;
                                                    }
                                                    m_pSlotManager.RemoveSpecialItemBlock(this);
                                                    EventDelegateManager.Instance.OnInGame_BeginBlockSwap();
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (IsSpecialItemBlock == false && m_pSlotBlock != null)
                            {
                                if (InGameInfo.Instance.m_pSelectModeSlot == null)
                                {
                                    InGameInfo.Instance.m_pSelectModeSlot = this;
                                    InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(true);
                                }
                                else
                                {
                                    if (InGameInfo.Instance.m_pSelectModeSlot.IsNeighborSlot_Cross(this) == true)
                                    {
                                        int nClickSlot_X = InGameInfo.Instance.m_pSelectModeSlot.GetX();
                                        int nClickSlot_Y = InGameInfo.Instance.m_pSelectModeSlot.GetY();

                                        int nRes = (m_nX - nClickSlot_X) + (m_nY - nClickSlot_Y);
                                        nRes = Mathf.Abs(nRes);

                                        if (nRes == 1)
                                        {
                                            m_pSlotManager.OnBlockSwap(InGameInfo.Instance.m_pSelectModeSlot, this);

                                            InGameInfo.Instance.m_nClickSlotFingerID = -1;
                                            InGameInfo.Instance.m_pClickSlot = null;

                                            if (InGameInfo.Instance.m_pSelectModeSlot != null)
                                            {
                                                InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
                                                InGameInfo.Instance.m_pSelectModeSlot = null;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
                                        InGameInfo.Instance.m_pSelectModeSlot = this;
                                        InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(true);
                                    }
                                }
                            }
                        }

                        m_fClickTime = Time.time;
                    }
                }

                InGameInfo.Instance.m_nClickSlotFingerID = -1;
                InGameInfo.Instance.m_pClickSlot = null;
            }
        }
    }

    protected void OnCallback_Move(GameObject gameObject, Vector3 vPos, object ob, int nFingerID)
    {
		if (InGameInfo.Instance.m_nClickSlotFingerID == nFingerID &&
			InGameInfo.Instance.m_pClickSlot != null &&
			InGameInfo.Instance.m_pClickSlot != this &&
            (InGameSetting.Instance.m_pInGameSettingData.m_IsAnytimeSwap == true || 
            (InGameSetting.Instance.m_pInGameSettingData.m_IsAnytimeSwap == false && InGameInfo.Instance.m_IsInGameClick == true)) &&
            InGameInfo.Instance.m_IsAutoPlay == false)
		{
            if (EspressoInfo.Instance.m_IsPlayerCharacterSkillTrigger == false && EspressoInfo.Instance.m_IsBoosterItemlTrigger == false)
            {
                int nClickSlot_X = InGameInfo.Instance.m_pClickSlot.GetX();
                int nClickSlot_Y = InGameInfo.Instance.m_pClickSlot.GetY();

                int nRes = (m_nX - nClickSlot_X) + (m_nY - nClickSlot_Y);
                nRes = Mathf.Abs(nRes);

                if (nRes == 1)
                {
                    m_pSlotManager.OnBlockSwap(InGameInfo.Instance.m_pClickSlot, this);

                    InGameInfo.Instance.m_nClickSlotFingerID = -1;
                    InGameInfo.Instance.m_pClickSlot = null;

                    if (InGameInfo.Instance.m_pSelectModeSlot != null)
                    {
                        InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
                        InGameInfo.Instance.m_pSelectModeSlot = null;
                    }
                }
            }
		}
	}
}
