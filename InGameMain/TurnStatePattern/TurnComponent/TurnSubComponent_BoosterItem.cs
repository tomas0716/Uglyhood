using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;

using sg.protocol.basic;

public class TurnSubComponent_BoosterItem
{
    private TurnComponent_BlockMatch    m_pTurnComponent_BlockMatch = null;

    private MainGame_DataStack  m_pDataStack                        = null;
    private bool                m_IsActive                          = false;

    private bool                m_IsBoosterItemTrigger              = false;

    private eTargetSelectType   m_eCurrTargetSelectType             = eTargetSelectType.Auto;

    private GameObject          m_pGameObject_BlackLayer            = null;

    private Dictionary<Slot, InGame_Highlight>  m_SelectRange_HighlightTable    = new Dictionary<Slot, InGame_Highlight>();
    private Dictionary<Slot, InGame_Highlight>  m_SelectTarget_HighlightTable   = new Dictionary<Slot, InGame_Highlight>();
    private List<Slot>                          m_SelectTargetSlotList          = new List<Slot>();

    private InGame_Highlight                    m_pInGame_Highlight_FirstSelect = null;
    private InGame_Highlight                    m_pInGame_Highlight_SecondSelect = null;

    private Dictionary<DamageTargetInfo, Slot>  m_pDamageSlotTable  = new Dictionary<DamageTargetInfo, Slot>();

    private GameObject          m_pGameObject_CancelArea            = null;

    private bool                m_IsAutoPlay                        = false;
    private int                 m_nCancelAreaFingerID               = 0;

    private Vector3             m_vInGame_Main_UI_CurrItem_ScreenPos = Vector3.zero;

    private NetComponent_InGameUseItem      m_pNetComponent_InGameUseItem       = new NetComponent_InGameUseItem();
    private NetComponent_GetUserInventory   m_pNetComponent_GetUserInventory    = new NetComponent_GetUserInventory();

    private eOwner              m_eOwner                            = eOwner.My;

    private Coroutine           m_pCoroutine_SkillTrigger           = null;

    public TurnSubComponent_BoosterItem(TurnComponent_BlockMatch pTurnComponent_BlockMatch, eOwner eOwn)
    {
        ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);

        m_pTurnComponent_BlockMatch = pTurnComponent_BlockMatch;
        m_eOwner = eOwn;

        m_pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/BlackLayer");
        m_pGameObject_BlackLayer = GameObject.Instantiate(ob);
        m_pGameObject_BlackLayer.SetActive(false);
        Vector3 vScale = m_pGameObject_BlackLayer.transform.localScale;
        vScale.x *= AppInstance.Instance.m_fHeightScale;
        vScale.y *= AppInstance.Instance.m_fHeightScale;
        m_pGameObject_BlackLayer.transform.localScale = vScale;
        //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
        Helper.SetMultipleScale_XY(m_pGameObject_BlackLayer, AppInstance.Instance.m_fHeightScale);

        if (m_eOwner == eOwner.My)
        {
            EventDelegateManager.Instance.OnEventInGame_UseBoosterItem += OnInGame_UseBoosterItem;
            EventDelegateManager.Instance.OnEventInGame_Request_ActionTrigger_On += OnInGame_Request_ActionTrigger_On;
            EventDelegateManager.Instance.OnEventInGame_Request_ActionTrigger_Cancel += OnInGame_Request_ActionTrigger_Cancel;
            EventDelegateManager.Instance.OnEventInGame_Slot_Click += OnInGame_Slot_Click;
            EventDelegateManager.Instance.OnEventInGame_Projectile_Block_Done += OnInGame_Projectile_Block_Done;
            EventDelegateManager.Instance.OnEventInGame_BoosterItemProjectile_Done += OnInGame_BoosterItemProjectile_Done;
            EventDelegateManager.Instance.OnEventInGame_Projectile_SP_Charge_Done += OnInGame_Projectile_SP_Charge_Done;
            EventDelegateManager.Instance.OnEventInGame_ChangeAutoPlay += OnInGame_ChangeAutoPlay;
            EventDelegateManager.Instance.OnEventInGame_Tooltip_DestroyCancelArea += OnInGame_Tooltip_DestroyCancelArea;
            EventDelegateManager.Instance.OnEventInGame_UI_ShieldClick += OnInGame_UI_ShieldClick;
        }
    }

    public void OnDestroy()
    {
        if (m_pGameObject_BlackLayer != null)
        {
            GameObject.Destroy(m_pGameObject_BlackLayer);
            m_pGameObject_BlackLayer = null;
        }

        if(m_pGameObject_CancelArea != null)
        {
            GameObject.Destroy(m_pGameObject_CancelArea);
            m_pGameObject_CancelArea = null;
        }
        if (m_eOwner == eOwner.My)
        {
            EventDelegateManager.Instance.OnEventInGame_UseBoosterItem -= OnInGame_UseBoosterItem;
            EventDelegateManager.Instance.OnEventInGame_Request_ActionTrigger_On -= OnInGame_Request_ActionTrigger_On;
            EventDelegateManager.Instance.OnEventInGame_Request_ActionTrigger_Cancel -= OnInGame_Request_ActionTrigger_Cancel;
            EventDelegateManager.Instance.OnEventInGame_Slot_Click -= OnInGame_Slot_Click;
            EventDelegateManager.Instance.OnEventInGame_Projectile_Block_Done -= OnInGame_Projectile_Block_Done;
            EventDelegateManager.Instance.OnEventInGame_BoosterItemProjectile_Done -= OnInGame_BoosterItemProjectile_Done;
            EventDelegateManager.Instance.OnEventInGame_Projectile_SP_Charge_Done -= OnInGame_Projectile_SP_Charge_Done;
            EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
            EventDelegateManager.Instance.OnEventInGame_ChangeAutoPlay -= OnInGame_ChangeAutoPlay;
            EventDelegateManager.Instance.OnEventInGame_Tooltip_DestroyCancelArea -= OnInGame_Tooltip_DestroyCancelArea;
            EventDelegateManager.Instance.OnEventInGame_BlockShuffleSkill -= OnInGame_BlockShuffleSkill;
            EventDelegateManager.Instance.OnEventInGame_UI_ShieldClick -= OnInGame_UI_ShieldClick;
        }
    }

    public void Update()
    {
    }

    public void LateUpdate()
    {
    }

    public void SetActive(bool IsActive)
    {
        InGameTurnLog.Log("TurnSubComponent_BoosterItem : SetActive : " + IsActive);

        m_IsActive = IsActive;
    }

    public void OnInGame_ChangeAutoPlay(bool IsAutoPlay)
    {
        m_IsAutoPlay = IsAutoPlay;
	}

    private void ClearHighlight()
    {
        foreach (KeyValuePair<Slot, InGame_Highlight> item in m_SelectRange_HighlightTable)
        {
            item.Key.ClearHighlight();
            GameObject.Destroy(item.Value.gameObject);
        }
        m_SelectRange_HighlightTable.Clear();

        foreach (KeyValuePair<Slot, InGame_Highlight> item in m_SelectTarget_HighlightTable)
        {
            item.Key.ClearHighlight();
            GameObject.Destroy(item.Value.gameObject);
        }
        m_SelectTarget_HighlightTable.Clear();

        foreach (KeyValuePair<InGame_Highlight, Slot> item in m_pDataStack.m_DamageTarget_HighlightTable)
        {
            item.Value.ClearHighlight();
            GameObject.Destroy(item.Key.gameObject);
        }
        m_pDataStack.m_DamageTarget_HighlightTable.Clear();
    }

    public void OnInGame_UseBoosterItem(int nSlot, Vector3 vScreenPos)
    {
        if (m_IsActive == true && m_IsAutoPlay == false && InGameInfo.Instance.m_IsInGameClick == true && m_pDataStack.m_nBlockProjectileCount == 0 && m_IsBoosterItemTrigger == false)
        {
            int nTeamDeckID = InventoryInfoManager.Instance.m_pDeckInfo.GetGameModeDeckID(EspressoInfo.Instance.m_eGameMode);
            int nBoosterItemDeckID = InventoryInfoManager.Instance.m_pDeckInfo.GetBoosterItemDeckID(nTeamDeckID);

            BoosterItemInvenItemInfo pInvenItemInfo = InventoryInfoManager.Instance.m_pBoosterItemInvenInfoGroup.GetBoosterItemInfoInfo_bySlot(nBoosterItemDeckID, nSlot);

            if (pInvenItemInfo != null)
            {
                m_vInGame_Main_UI_CurrItem_ScreenPos = vScreenPos;

                EspressoInfo.Instance.m_eActionTriggerSubject = eActionTriggerSubject.BoosterItem;
                EspressoInfo.Instance.m_nUseBoosterItemSlot = nSlot;
                ExcelData_ItemInfo pItemInfo = ExcelDataManager.Instance.m_pItem.GetItemInfo_byID(pInvenItemInfo.m_nTableID);

                GameObject ob;

                if (InGameInfo.Instance.m_pSelectModeSlot != null)
                {
                    InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
                    InGameInfo.Instance.m_pSelectModeSlot = null;
                }

                ClearHighlight();
                m_SelectTargetSlotList.Clear();
                m_pInGame_Highlight_FirstSelect = null;
                m_pInGame_Highlight_SecondSelect = null;

                bool IsAbleSkill = false;

                ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(pItemInfo.m_nItem_ActionID);

                if (pActionInfo != null)
                {
                    switch (pActionInfo.m_eDamageType)
                    {
                        case eDamageEffectType.ShieldPointCharge:
                            {
                                if (EspressoInfo.Instance.m_eGameMode != eGameMode.PvpStage && EspressoInfo.Instance.m_eGameMode != eGameMode.EventPvpStage)
                                {
                                    EventDelegateManager.Instance.OnInGame_ShieldSelecTarget(true);

                                    m_eCurrTargetSelectType = eTargetSelectType.Auto;
                                    IsAbleSkill = true;
                                }
                            }
                            break;

                        default:
                            {
                                SelectTargetGenerator pSelectTargetGenerator = new SelectTargetGenerator();
                                pSelectTargetGenerator.OnGenerator(pActionInfo.m_nID, null, eOwner.My, eObjectType.Character, eAttackType.Skill);

                                m_eCurrTargetSelectType = pActionInfo.m_eTargetSelectType;

                                switch (pActionInfo.m_eTargetSelectType)
                                {
                                    case eTargetSelectType.Auto:
                                        {
                                            for (int i = 0; i < pActionInfo.m_nSelectTargetAmount; ++i)
                                            {
                                                SelectTargetInfo pSelectTargetInfo = pSelectTargetGenerator.GetSelectTargetInfo_byIndex(i);

                                                if (pSelectTargetInfo != null)
                                                {
                                                    int nSelect_X = pSelectTargetInfo.m_nSelectTargetSlotX;
                                                    int nSelect_Y = pSelectTargetInfo.m_nSelectTargetSlotY;

                                                    if (m_pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nSelect_X, nSelect_Y)) == true)
                                                    {
                                                        Slot pSelectSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nSelect_X, nSelect_Y)];

                                                        if (m_SelectTarget_HighlightTable.ContainsKey(pSelectSlot) == false)
                                                        {
                                                            GameObject ob_Highlight = null;
                                                            SlotFixObject_PlayerCharacter pPlayerCharacter = pSelectSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                                            if (pPlayerCharacter != null)
                                                            {
                                                                string strPos = "_Center";
                                                                if (nSelect_X == 0) strPos = "_Right";
                                                                else if (nSelect_X == InGameInfo.Instance.m_nSlotCount_X - 1) strPos = "_Left";

                                                                if (pPlayerCharacter.GetOwner() == eOwner.My)
                                                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Green" + strPos);
                                                                else
                                                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Red" + strPos);

                                                                ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                            }
                                                            else
                                                            {
                                                                if (pSelectSlot.GetSlotBlock() != null)
                                                                {
                                                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                                    ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                    //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                    Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                }
                                                                else
                                                                {
                                                                    SlotFixObject_Espresso pSlotFixObject = pSelectSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                                                                    if (pSlotFixObject != null && pSlotFixObject.GetOwner() == eOwner.My)
                                                                    {
                                                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Green");
                                                                        ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                        //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                        Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                    }
                                                                    else
                                                                    {
                                                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                                        ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                        //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                        Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                    }
                                                                }
                                                            }

                                                            InGame_Highlight pHighlight = ob_Highlight.GetComponent<InGame_Highlight>();
                                                            pHighlight.Initialize(ePlayerCharacterHighlight.SelectTarget, pSelectSlot, null);

                                                            m_SelectTarget_HighlightTable.Add(pSelectSlot, pHighlight);
                                                            pSelectSlot.SetHighlight();
                                                            m_SelectTargetSlotList.Add(pSelectSlot);
                                                        }

                                                        DamageTargetGenerator pDamageTargetGenerator = new DamageTargetGenerator();
                                                        pDamageTargetGenerator.OnGenerator(pActionInfo.m_nID, pSelectSlot, eOwner.My, eObjectType.Character, eAttackType.Skill);

                                                        int nNumDamageTarget = pDamageTargetGenerator.GetNumDamageTargetInfo();

                                                        for (int j = 0; j < nNumDamageTarget; ++j)
                                                        {
                                                            DamageTargetInfo pDamageTargetInfo = pDamageTargetGenerator.GetDamageTargetInfo_byIndex(j);

                                                            if (pDamageTargetInfo != null)
                                                            {
                                                                int nDamage_X = nSelect_X + pDamageTargetInfo.m_pRangeInfo.m_nPosX;
                                                                int nDamage_Y = nSelect_Y + pDamageTargetInfo.m_pRangeInfo.m_nPosY * -1;

                                                                if (m_pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nDamage_X, nDamage_Y)) == true)
                                                                {
                                                                    Slot pDamageSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nDamage_X, nDamage_Y)];

                                                                    //if (m_pDataStack.m_DamageTarget_HighlightTable.ContainsKey(pDamageSlot) == false)
                                                                    {
                                                                        GameObject ob_Highlight = null;
                                                                        SlotFixObject_PlayerCharacter pPlayerCharacter = pDamageSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                                                        if (pPlayerCharacter != null)
                                                                        {
                                                                            string strPos = "_Center";
                                                                            if (nDamage_X == 0) strPos = "_Right";
                                                                            else if (nDamage_X == InGameInfo.Instance.m_nSlotCount_X - 1) strPos = "_Left";

                                                                            if (pPlayerCharacter.GetOwner() == eOwner.My)
                                                                                ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Green" + strPos);
                                                                            else
                                                                                ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Red" + strPos);

                                                                            ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                            //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                            Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                        }
                                                                        else
                                                                        {
                                                                            if (pDamageSlot.GetSlotBlock() != null)
                                                                            {
                                                                                ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                                                ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                                //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                                Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                            }
                                                                            else
                                                                            {
                                                                                SlotFixObject_Espresso pSlotFixObject = pDamageSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                                                                                if (pSlotFixObject != null && pSlotFixObject.GetOwner() == eOwner.My)
                                                                                {
                                                                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Green");
                                                                                    ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                                    //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                                    Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                                }
                                                                                else
                                                                                {
                                                                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                                                    ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                                    //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                                    Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                                }
                                                                            }
                                                                        }

                                                                        InGame_Highlight pHighlight = ob_Highlight.GetComponent<InGame_Highlight>();
                                                                        pHighlight.Initialize(ePlayerCharacterHighlight.DamageTarget, pDamageSlot, pDamageTargetInfo);

                                                                        m_pDataStack.m_DamageTarget_HighlightTable.Add(pHighlight, pDamageSlot);
                                                                        pDamageSlot.SetHighlight();

                                                                        IsAbleSkill = true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;

                                    case eTargetSelectType.ManualAndAuto:
                                        {
                                            int nNumSelectTarget = pSelectTargetGenerator.GetNumSelectTargetInfo();
                                            for (int i = 0; i < nNumSelectTarget; ++i)
                                            {
                                                SelectTargetInfo pSelectTargetInfo = pSelectTargetGenerator.GetSelectTargetInfo_byIndex(i);

                                                if (pSelectTargetInfo != null)
                                                {
                                                    int nSelect_X = pSelectTargetInfo.m_nSelectTargetSlotX;
                                                    int nSelect_Y = pSelectTargetInfo.m_nSelectTargetSlotY;

                                                    if (m_pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nSelect_X, nSelect_Y)) == true)
                                                    {
                                                        Slot pSelectSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nSelect_X, nSelect_Y)];

                                                        if (m_SelectRange_HighlightTable.ContainsKey(pSelectSlot) == false)
                                                        {
                                                            GameObject ob_Highlight = null;
                                                            SlotFixObject_PlayerCharacter pPlayerCharacter = pSelectSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                                            if (pPlayerCharacter != null)
                                                            {
                                                                string strPos = "_Center";
                                                                if (nSelect_X == 0) strPos = "_Right";
                                                                else if (nSelect_X == InGameInfo.Instance.m_nSlotCount_X - 1) strPos = "_Left";

                                                                if (pPlayerCharacter.GetOwner() == eOwner.My)
                                                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Green" + strPos);
                                                                else
                                                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Red" + strPos);

                                                                ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                            }
                                                            else
                                                            {
                                                                if (pSelectSlot.GetSlotBlock() != null)
                                                                {
                                                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                                    ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                    //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                    Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                }
                                                                else
                                                                {
                                                                    SlotFixObject_Espresso pSlotFixObject = pSelectSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                                                                    if (pSlotFixObject != null && pSlotFixObject.GetOwner() == eOwner.My)
                                                                    {
                                                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Green");
                                                                        ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                        //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                        Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                    }
                                                                    else
                                                                    {
                                                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                                        ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                                        //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                                        Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                                    }
                                                                }
                                                            }

                                                            InGame_Highlight pHighlight = ob_Highlight.GetComponent<InGame_Highlight>();
                                                            pHighlight.Initialize(ePlayerCharacterHighlight.SelectRange, pSelectSlot, null);

                                                            m_SelectRange_HighlightTable.Add(pSelectSlot, pHighlight);
                                                            pSelectSlot.SetHighlight();

                                                            IsAbleSkill = true;
                                                        }
                                                    }
                                                }
                                            }

                                            List<SelectTargetInfo> firstPrioritySelectTargetInfoList = pSelectTargetGenerator.GetFirstPrioritySelectTargetInfoList();

                                            if (firstPrioritySelectTargetInfoList.Count != 0)
                                            {
                                                int nCount = firstPrioritySelectTargetInfoList.Count;
                                                int nRandom = UnityEngine.Random.Range(0, nCount);

                                                SelectTargetInfo pFirstSelectTargetInfo = firstPrioritySelectTargetInfoList[nRandom];

                                                if (pFirstSelectTargetInfo != null)
                                                {
                                                    int nSlotIndex = Helper.GetSlotIndex(pFirstSelectTargetInfo.m_nSelectTargetSlotX, pFirstSelectTargetInfo.m_nSelectTargetSlotY);

                                                    if (m_pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(nSlotIndex) == true)
                                                    {
                                                        m_IsBoosterItemTrigger = true;
                                                        OnInGame_Slot_Click(m_pDataStack.m_pSlotManager.GetSlotTable()[nSlotIndex]);
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                    
                }

                if (IsAbleSkill == false)
                {
                    GameObject ob_Ms = Resources.Load<GameObject>("GUI/Prefabs/Common/CommonMessageBox");
                    ob_Ms = GameObject.Instantiate(ob_Ms);
                    CommonMessageBox_UI pScript = ob_Ms.GetComponent<CommonMessageBox_UI>();
                    pScript.Init(ExcelDataHelper.GetString("ACTION_MESSAGE_NO_VAILD_TARGET"), 0, 0.7f, 0.2f);
                }

                EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Close();
                ob = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGameActionDetail");
                GameObject.Instantiate(ob);

                EventDelegateManager.Instance.OnInGame_RefillGuide_UI_Destroy();

                m_pGameObject_BlackLayer.SetActive(true);

                EspressoInfo.Instance.m_IsBoosterItemlTrigger = true;
                m_IsBoosterItemTrigger = true;

                if (m_pGameObject_CancelArea != null) GameObject.Destroy(m_pGameObject_CancelArea);
                ob = Resources.Load<GameObject>("2D/Prefabs/Character/PlayerCharacterAttackCancelArea");
                m_pGameObject_CancelArea = GameObject.Instantiate(ob);
                Plane2D pPlane_CancelArea = m_pGameObject_CancelArea.GetComponent<Plane2D>();
                pPlane_CancelArea.AddCallback_LButtonDown(OnCallback_LButtonDown_CancelArea);
                pPlane_CancelArea.AddCallback_LButtonUp(OnCallback_LButtonUp_CancelArea);
            }
        }
        else if (m_IsActive == true && m_IsBoosterItemTrigger == false)
        {
            OnInGame_Request_ActionTrigger_Cancel();
        }
    }

    public void OnInGame_Slot_Click(Slot pSlot)
    {
        OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_Slot_Click");

        if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.BoosterItem)
        {
            GameObject ob;

			if (m_IsBoosterItemTrigger == true && m_IsActive == true)
			{
				int nTeamDeckID = InventoryInfoManager.Instance.m_pDeckInfo.GetGameModeDeckID(EspressoInfo.Instance.m_eGameMode);
				int nBoosterItemDeckID = InventoryInfoManager.Instance.m_pDeckInfo.GetBoosterItemDeckID(nTeamDeckID);

				BoosterItemInvenItemInfo pInvenItemInfo = InventoryInfoManager.Instance.m_pBoosterItemInvenInfoGroup.GetBoosterItemInfoInfo_bySlot(nBoosterItemDeckID, EspressoInfo.Instance.m_nUseBoosterItemSlot);

				ExcelData_ItemInfo pItemInfo = ExcelDataManager.Instance.m_pItem.GetItemInfo_byID(pInvenItemInfo.m_nTableID);

				ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(pItemInfo.m_nItem_ActionID);

				if (pActionInfo != null)
				{
                    if (m_SelectTarget_HighlightTable.ContainsKey(pSlot) == true || m_pInGame_Highlight_FirstSelect != null)
                    {
                        switch (pActionInfo.m_eDamageType)
                        {
                            case eDamageEffectType.CharacterSwap:
                                {
                                    if (m_pInGame_Highlight_FirstSelect == null)
                                    {
                                        Helper.OnSoundPlay("UI_CHARACTER_TOUCH", false);
                                        m_pInGame_Highlight_FirstSelect = m_SelectTarget_HighlightTable[pSlot];

                                        // 첫 선택 타겟 처리
                                        SlotFixObject_PlayerCharacter pPlayerCharacter = pSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                        if (pPlayerCharacter != null)
                                        {
                                            pPlayerCharacter.SetSkillFirstSelect(true);
                                        }

                                        m_pInGame_Highlight_FirstSelect.gameObject.SetActive(false);
                                        m_SelectTarget_HighlightTable.Remove(pSlot);

                                        List<Slot> highlightList = new List<Slot>();
                                        foreach (KeyValuePair<Slot, InGame_Highlight> item in m_SelectRange_HighlightTable)
                                        {
                                            if (m_pInGame_Highlight_FirstSelect != null && m_pInGame_Highlight_FirstSelect.GetSlot() != item.Value.GetSlot())
                                            {
                                                highlightList.Add(item.Key);
                                            }
                                        }

                                        if (highlightList.Count != 0)
                                        {
                                            Helper.ShuffleList_UnityEngineRandom(highlightList);
                                            Helper.ShuffleList_UnityEngineRandom(highlightList);
                                            int nRandom = UnityEngine.Random.Range(0, highlightList.Count);
                                            Slot pNextTarget = highlightList[nRandom];
                                            OnInGame_Slot_Click(pNextTarget);
                                        }

                                        return;
                                    }
                                    else if (m_pInGame_Highlight_FirstSelect.GetSlot() == pSlot)
                                    {
                                        Helper.OnSoundPlay("UI_CHARACTER_TOUCH", false);

                                        // 첫 선택 타겟 해제 처리
                                        Slot pSlot_FirstSelect = m_pInGame_Highlight_FirstSelect.GetSlot();
                                        SlotFixObject_PlayerCharacter pPlayerCharacter = pSlot_FirstSelect.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                        if (pPlayerCharacter != null)
                                        {
                                            pPlayerCharacter.SetSkillFirstSelect(false);
                                        }

                                        if (m_SelectTarget_HighlightTable.ContainsKey(pSlot) == true)
                                        {
                                            GameObject.Destroy(m_pInGame_Highlight_FirstSelect.gameObject);
                                            m_SelectTarget_HighlightTable.Remove(pSlot);
                                        }

                                        m_pInGame_Highlight_FirstSelect = null;
                                    }
                                    else if (m_SelectTarget_HighlightTable.ContainsKey(pSlot) == true)
                                    {
                                        Helper.OnSoundPlay("UI_CHARACTER_TOUCH", false);

                                        m_pInGame_Highlight_SecondSelect = m_SelectTarget_HighlightTable[pSlot];

                                        Slot pSlot_FirstSelect = m_pInGame_Highlight_FirstSelect.GetSlot();
                                        SlotFixObject_PlayerCharacter pPlayerCharacter = pSlot_FirstSelect.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                        if (pPlayerCharacter != null)
                                        {
                                            pPlayerCharacter.SetSkillFirstSelect(false);
                                        }

                                        EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Close();
                                        EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_On();
                                        return;
                                    }
                                }
                                break;

                            default:
                                {
                                    EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Close();
                                    EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_On();
                                    return;
                                }
                        }
                    }
                     
                    if (m_SelectTarget_HighlightTable.ContainsKey(pSlot) == true)
					{
						EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Close();
						EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_On();
						return;
					}

					if (m_SelectRange_HighlightTable.ContainsKey(pSlot) == true)
					{
                        OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_Slot_Click In");

                        if (m_eCurrTargetSelectType == eTargetSelectType.ManualAndAuto)
                        {
                            foreach (KeyValuePair<Slot, InGame_Highlight> item in m_SelectTarget_HighlightTable)
                            {
                                GameObject.Destroy(item.Value.gameObject);
                            }
                            m_SelectTarget_HighlightTable.Clear();
                            m_SelectTargetSlotList.Clear();

                            foreach (KeyValuePair<InGame_Highlight, Slot> item in m_pDataStack.m_DamageTarget_HighlightTable)
                            {
                                GameObject.Destroy(item.Key.gameObject);
                            }
                            m_pDataStack.m_DamageTarget_HighlightTable.Clear();

                            GameObject ob_Highlight = null;
                            SlotFixObject_PlayerCharacter pPlayerCharacter = pSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                            if (pPlayerCharacter != null)
                            {
                                string strPos = "_Center";
                                if (pSlot.GetX() == 0) strPos = "_Right";
                                else if (pSlot.GetX() == InGameInfo.Instance.m_nSlotCount_X - 1) strPos = "_Left";

                                if (pPlayerCharacter.GetOwner() == eOwner.My)
                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Green" + strPos);
                                else
                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Red" + strPos);

                                ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                            }
                            else
                            {
                                if (pSlot.GetSlotBlock() != null)
                                {
                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                    ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                    //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                    Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                }
                                else
                                {
                                    SlotFixObject_Espresso pSlotFixObject = pSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                                    if (pSlotFixObject != null && pSlotFixObject.GetOwner() == eOwner.My)
                                    {
                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Green");
                                        ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                        //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                        Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                    }
                                    else
                                    {
                                        ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                        ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                        //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                        Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                    }
                                }
                            }

                            InGame_Highlight pHighlight = ob_Highlight.GetComponent<InGame_Highlight>();
                            pHighlight.Initialize(ePlayerCharacterHighlight.SelectTarget, pSlot, null);

                            m_SelectTarget_HighlightTable.Add(pSlot, pHighlight);
                            pSlot.SetHighlight();
                            m_SelectTargetSlotList.Add(pSlot);

                            foreach (KeyValuePair<Slot, InGame_Highlight> item in m_SelectRange_HighlightTable)
                            {
                                item.Value.gameObject.SetActive(true);
                            }

                            Helper.OnSoundPlay("INGAME_SKILL_TARGET_SELECT", false);

                            DamageTargetGenerator pDamageTargetGenerator = new DamageTargetGenerator();
                            pDamageTargetGenerator.OnGenerator(pActionInfo.m_nID, pSlot, eOwner.My, eObjectType.Character, eAttackType.Skill);

                            int nNumDamageTarget = pDamageTargetGenerator.GetNumDamageTargetInfo();

                            for (int j = 0; j < nNumDamageTarget; ++j)
                            {
                                DamageTargetInfo pDamageTargetInfo = pDamageTargetGenerator.GetDamageTargetInfo_byIndex(j);

                                if (pDamageTargetInfo != null)
                                {
                                    int nDamage_X = pSlot.GetX() + pDamageTargetInfo.m_pRangeInfo.m_nPosX;
                                    int nDamage_Y = pSlot.GetY() + pDamageTargetInfo.m_pRangeInfo.m_nPosY * -1;

                                    if (m_pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nDamage_X, nDamage_Y)) == true)
                                    {
                                        Slot pDamageSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nDamage_X, nDamage_Y)];

                                        ob_Highlight = null;
                                        pPlayerCharacter = pDamageSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                        if (pPlayerCharacter != null)
                                        {
                                            string strPos = "_Center";
                                            if (nDamage_X == 0) strPos = "_Right";
                                            else if (nDamage_X == InGameInfo.Instance.m_nSlotCount_X - 1) strPos = "_Left";

                                            if (pPlayerCharacter.GetOwner() == eOwner.My)
                                                ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Green" + strPos);
                                            else
                                                ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Player_Highlight_Red" + strPos);

                                            ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                            //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                            Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                        }
                                        else
                                        {
                                            if (pDamageSlot.GetSlotBlock() != null)
                                            {
                                                ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                            }
                                            else
                                            {
                                                SlotFixObject_Espresso pSlotFixObject = pDamageSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                                                if (pSlotFixObject != null && pSlotFixObject.GetOwner() == eOwner.My)
                                                {
                                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Green");
                                                    ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                    //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                    Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                }
                                                else
                                                {
                                                    ob_Highlight = Resources.Load<GameObject>("2D/Prefabs/Highlight/Highlight_Red");
                                                    ob_Highlight = GameObject.Instantiate(ob_Highlight);
                                                    //ob_Highlight.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
                                                    Helper.SetMultipleScale_XY(ob_Highlight, InGameInfo.Instance.m_fInGameScale);
                                                }
                                            }
                                        }

                                        pHighlight = ob_Highlight.GetComponent<InGame_Highlight>();
                                        pHighlight.Initialize(ePlayerCharacterHighlight.DamageTarget, pDamageSlot, pDamageTargetInfo);

                                        m_pDataStack.m_DamageTarget_HighlightTable.Add(pHighlight, pDamageSlot);
                                        pDamageSlot.SetHighlight();

                                        if (m_SelectRange_HighlightTable.ContainsKey(pDamageSlot) == true)
                                        {
                                            m_SelectRange_HighlightTable[pDamageSlot].gameObject.SetActive(false);
                                        }
                                    }
                                }
                            }
                        }
                    }
				}
			}
		}
	}

    public void OnInGame_UI_ShieldClick()
    {
        OnInGame_Request_ActionTrigger_On();
    }

    private void OnInGame_Request_ActionTrigger_On()
    {
        OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_Request_ActionTrigger_On");

        if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.BoosterItem)
        {
            if (m_pGameObject_CancelArea != null)
            {
                GameObject.Destroy(m_pGameObject_CancelArea);
                m_pGameObject_CancelArea = null;
            }

            int nTeamDeckID = InventoryInfoManager.Instance.m_pDeckInfo.GetGameModeDeckID(EspressoInfo.Instance.m_eGameMode);
            int nBoosterItemDeckID = InventoryInfoManager.Instance.m_pDeckInfo.GetBoosterItemDeckID(nTeamDeckID);
            BoosterItemInvenItemInfo pInvenItemInfo = InventoryInfoManager.Instance.m_pBoosterItemInvenInfoGroup.GetBoosterItemInfoInfo_bySlot(nBoosterItemDeckID, EspressoInfo.Instance.m_nUseBoosterItemSlot);
            ExcelData_ItemInfo pItemInfo = ExcelDataManager.Instance.m_pItem.GetItemInfo_byID(pInvenItemInfo.m_nTableID);
            ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(pItemInfo.m_nItem_ActionID);

            m_pDamageSlotTable.Clear();

            if (pActionInfo != null)
            {
                switch (pActionInfo.m_eDamageType)
                {
                    case eDamageEffectType.CharacterSwap:
                        {
                            CharacterInvenItemInfo pCharacterInfo = m_pDataStack.m_pPlayerCharacter_SkillTrigger.GetCharacterInvenItemInfo();
                            ExcelData_Action_LevelUpInfo pActionLevelUpInfo = ExcelDataManager.Instance.m_pAction_LevelUp.GetActionLevelUpInfo_byKey(pActionInfo.m_nID, pCharacterInfo.m_nAction_Level);

                            DamageTargetGenerator pDamageTargetGenerator;

                            pDamageTargetGenerator = new DamageTargetGenerator();
                            pDamageTargetGenerator.OnGenerator(pActionInfo.m_nID, m_pInGame_Highlight_FirstSelect.GetSlot(), eOwner.My, eObjectType.Character, eAttackType.Skill);

                            int nNumDamageTarget = pDamageTargetGenerator.GetNumDamageTargetInfo();
                            if (nNumDamageTarget != 0)
                            {
                                DamageTargetInfo pDamageTargetInfo = pDamageTargetGenerator.GetDamageTargetInfo_byIndex(0);
                                m_pDamageSlotTable.Add(pDamageTargetInfo, m_pInGame_Highlight_FirstSelect.GetSlot());
                            }

                            pDamageTargetGenerator = new DamageTargetGenerator();
                            pDamageTargetGenerator.OnGenerator(pActionInfo.m_nID, m_pInGame_Highlight_SecondSelect.GetSlot(), eOwner.My, eObjectType.Character, eAttackType.Skill);

                            nNumDamageTarget = pDamageTargetGenerator.GetNumDamageTargetInfo();
                            if (nNumDamageTarget != 0)
                            {
                                DamageTargetInfo pDamageTargetInfo = pDamageTargetGenerator.GetDamageTargetInfo_byIndex(0);
                                m_pDamageSlotTable.Add(pDamageTargetInfo, m_pInGame_Highlight_SecondSelect.GetSlot());
                            }
                        }
                        break;

                    default:
                        {
                            foreach (KeyValuePair<InGame_Highlight, Slot> item in m_pDataStack.m_DamageTarget_HighlightTable)
                            {
                                m_pDamageSlotTable.Add(item.Key.GetAction_DamageTargetInfo(), item.Value);
                            }
                        }
                        break;
                }
            }

            ClearHighlight();

            m_pGameObject_BlackLayer.SetActive(false);

            InGameInfo.Instance.m_nClickSlotFingerID = -1;
            InGameInfo.Instance.m_pClickSlot = null;

            OnUseItemEffect();

            if(EspressoInfo.Instance.m_nUseBoosterItemSlot != (int)eBoosterItemType_bySlot.AD)
            {
                TEventDelegate<EventArg_Null> pSuccessEvent = new TEventDelegate<EventArg_Null>();
                TEventDelegate<Error> pFailureEvent = new TEventDelegate<Error>();
                pSuccessEvent.SetFunc(OnUseItemSuccess);
                pFailureEvent.SetFunc(OnUseItemFailure);

                EventArg_InGameUseItem pArg = new EventArg_InGameUseItem();
                pArg.m_nEpisodeID = EspressoInfo.Instance.m_nEpisodeID;
                pArg.m_nChapterID = EspressoInfo.Instance.m_nChapterID;
                pArg.m_nStageID = EspressoInfo.Instance.m_nStageID;
                pArg.m_nItemID = pInvenItemInfo.m_nTableID;
                pArg.m_nItemCount = 1;

                m_pNetComponent_InGameUseItem.SetSuccessEvent(m_pNetComponent_GetUserInventory);
                m_pNetComponent_InGameUseItem.SetFailureEvent(pFailureEvent);
                m_pNetComponent_GetUserInventory.SetSuccessEvent(pSuccessEvent);
                m_pNetComponent_GetUserInventory.SetFailureEvent(pFailureEvent);

                m_pNetComponent_InGameUseItem.OnEvent(pArg);

                AppInstance.Instance.m_pEventDelegateManager.OnCreateLoadingResponse();
            }
            else
            {
                pInvenItemInfo.m_nItemCount -= 1;
                EventDelegateManager.Instance.OnInGame_UsedBoosterItem(EspressoInfo.Instance.m_nUseBoosterItemSlot);
            }

            Helper.OnSoundPlay("INGAME_SKILL_CUTSCENE_START", false);
        }
    }

    private void OnUseItemSuccess(EventArg_Null Arg)
    {
        int nTeamDeckID = InventoryInfoManager.Instance.m_pDeckInfo.GetGameModeDeckID(EspressoInfo.Instance.m_eGameMode);
        int nBoosterItemDeckID = InventoryInfoManager.Instance.m_pDeckInfo.GetBoosterItemDeckID(nTeamDeckID);
        BoosterItemInvenItemInfo pInvenItemInfo = InventoryInfoManager.Instance.m_pBoosterItemInvenInfoGroup.GetBoosterItemInfoInfo_bySlot(nBoosterItemDeckID, EspressoInfo.Instance.m_nUseBoosterItemSlot);
        pInvenItemInfo.m_nItemCount -= 1;
        EventDelegateManager.Instance.OnInGame_UsedBoosterItem(EspressoInfo.Instance.m_nUseBoosterItemSlot);

        m_pDataStack.m_nBoosterItemUseCount++;
        EventDelegateManager.Instance.OnInGameUI_ChallengeModeCheck();

        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    private void OnUseItemFailure(Error error)
    {
        // 서버 통신 에러
        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    private void OnInGame_Request_ActionTrigger_Cancel()
    {
        OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_Request_ActionTrigger_Cancel");

        if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.BoosterItem)
        {
            EspressoInfo.Instance.m_eActionTriggerSubject = eActionTriggerSubject.None;

            if (m_pInGame_Highlight_FirstSelect != null)
            {
                Slot pSlot_FirstSelect = m_pInGame_Highlight_FirstSelect.GetSlot();
                SlotFixObject_PlayerCharacter pPlayerCharacter = pSlot_FirstSelect.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                if (pPlayerCharacter != null)
                {
                    pPlayerCharacter.SetSkillFirstSelect(false);
                }

                m_pInGame_Highlight_FirstSelect = null;
            }

            ClearHighlight();

            m_IsBoosterItemTrigger = false;
            InGameInfo.Instance.m_IsInGameClick = true;
            EspressoInfo.Instance.m_IsBoosterItemlTrigger = false;

            m_pGameObject_BlackLayer.SetActive(false);
        }
    }
    private void OnUseItemEffect()
    {
        if (m_pCoroutine_SkillTrigger != null)
        {
            AppInstance.Instance.StopCoroutine(m_pCoroutine_SkillTrigger);
            m_pCoroutine_SkillTrigger = null;
        }

        m_pCoroutine_SkillTrigger = AppInstance.Instance.StartCoroutine(Co_OnUseItemEffect());
    }

    public IEnumerator Co_OnUseItemEffect()
    {
        OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_PlayerCharacterSkill_CutScene_UIDone");

        int nTeamDeckID = InventoryInfoManager.Instance.m_pDeckInfo.GetGameModeDeckID(EspressoInfo.Instance.m_eGameMode);
        int nBoosterItemDeckID = InventoryInfoManager.Instance.m_pDeckInfo.GetBoosterItemDeckID(nTeamDeckID);

        BoosterItemInvenItemInfo pInvenItemInfo = InventoryInfoManager.Instance.m_pBoosterItemInvenInfoGroup.GetBoosterItemInfoInfo_bySlot(nBoosterItemDeckID, EspressoInfo.Instance.m_nUseBoosterItemSlot);
        ExcelData_ItemInfo pItemInfo = ExcelDataManager.Instance.m_pItem.GetItemInfo_byID(pInvenItemInfo.m_nTableID);

        ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(pItemInfo.m_nItem_ActionID);

        if (pActionInfo != null)
        {
            InGameInfo.Instance.m_IsPlayerCharacterSkillToSpeicalBlockAttack = false;
            m_pDataStack.m_nBoosterItemProjectileCount = 0;
            m_pDataStack.m_fKnockBackDelayTime = 0;

            switch (pActionInfo.m_eDamageType)
            {
                case eDamageEffectType.CharacterSwap:
                    {
                        Helper.OnSoundPlay(pActionInfo.m_strEffectHit_Sound, false);

                        float fDelayTime = 0;
                        foreach (KeyValuePair<DamageTargetInfo, Slot> item in m_pDamageSlotTable)
                        {
                            if ((pActionInfo.m_strEffectHit_Target != "0" || pActionInfo.m_strEffectHit_Center != "0") && m_SelectTargetSlotList.Contains(item.Value) == true)
                            {
                                Vector3 vPos_Target = item.Value.GetPosition();
                                vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                                if (pActionInfo.m_strEffectHit_Target != "0" &&
                                    InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Contains(pActionInfo.m_strEffectHit_Target) == false)
                                {
                                    ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit_Target, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                                    InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Add(pActionInfo.m_strEffectHit_Target);
                                }

                                if (pActionInfo.m_strEffectHit_Center != "0" &&
                                    InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Contains(pActionInfo.m_strEffectHit_Center) == false)
                                {
                                    ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit_Center, InGameInfo.Instance.m_vSlotGridCenter).SetScale(InGameInfo.Instance.m_fInGameScale);
                                    InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Add(pActionInfo.m_strEffectHit_Center);
                                }

                                float fDelay = pActionInfo.m_fEffectHit_Target_Delay;

                                if (fDelay > fDelayTime)
                                {
                                    fDelayTime = fDelay;
                                }
                            }
                        }

                        yield return new WaitForSeconds(fDelayTime);

                        foreach (KeyValuePair<DamageTargetInfo, Slot> item in m_pDamageSlotTable)
                        {
                            if (pActionInfo.m_strEffectHit != "0")
                            {
                                Vector3 vPos_Target = item.Value.GetPosition();
                                vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                                ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                            }

                            MainGame_Espresso_ProcessHelper.ApplyStatusEffect(null, item.Value, pActionInfo, item.Key.m_pRangeInfo);
                        }

                        Slot pSlot_01 = m_pInGame_Highlight_FirstSelect.GetSlot();
                        Slot pSlot_02 = m_pInGame_Highlight_SecondSelect.GetSlot();
                        SlotFixObject_PlayerCharacter pPlayerCharacter_01 = pSlot_01.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;
                        SlotFixObject_PlayerCharacter pPlayerCharacter_02 = pSlot_02.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;
                        m_pDataStack.m_PlayerCharacterTable.Remove(pSlot_01.GetSlotIndex());
                        m_pDataStack.m_PlayerCharacterTable.Remove(pSlot_02.GetSlotIndex());

                        pSlot_01.AddSlotFixObject(pPlayerCharacter_02);
                        pSlot_02.AddSlotFixObject(pPlayerCharacter_01);
                        pPlayerCharacter_01.SetSlot(pSlot_02);
                        pPlayerCharacter_02.SetSlot(pSlot_01);
                        m_pDataStack.m_PlayerCharacterTable.Add(pSlot_01.GetSlotIndex(), pPlayerCharacter_02);
                        m_pDataStack.m_PlayerCharacterTable.Add(pSlot_02.GetSlotIndex(), pPlayerCharacter_01);

                        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
                        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone += OnInGame_CheckRemoveSlotDone;

                        m_IsBoosterItemTrigger = false;

                        EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate = false;

                        EventDelegateManager.Instance.OnInGame_Projectile_Block_Done();
                        EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0.4f);
                        InGameInfo.Instance.m_IsInGameClick = false;
                        m_pDataStack.m_pPlayerCharacter_SkillTrigger = null;
                        m_pGameObject_BlackLayer.SetActive(false);
                    }
                    break;

                case eDamageEffectType.ShieldPointCharge:
                    {
                        EventDelegateManager.Instance.OnInGame_ShieldSelecTarget(false);
                        m_pDataStack.m_nCurrShildPoint += pActionInfo.m_nDamageBaseRate;
                        EventDelegateManager.Instance.OnInGame_Action_ShieldCharge(pActionInfo, m_pDataStack.m_nCurrShildPoint, 1);

                        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
                        EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone += OnInGame_CheckRemoveSlotDone;

                        m_IsBoosterItemTrigger = false;

                        EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate = false;

                        EventDelegateManager.Instance.OnInGame_Projectile_Block_Done();
                        EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0.4f);
                        InGameInfo.Instance.m_IsInGameClick = false;
                        m_pDataStack.m_pPlayerCharacter_SkillTrigger = null;
                        m_pGameObject_BlackLayer.SetActive(false);
                    }
                    break;

                default:
                    {
                        bool IsProjectile = false;
                        float fDelayTime = 0;

                        foreach (KeyValuePair<DamageTargetInfo, Slot> item in m_pDamageSlotTable)
                        {
                            float fDamageRate = ((float)item.Key.m_pRangeInfo.m_nRelativeDamageRate) / 100.0f;
                            if (pActionInfo.m_strEffectMissile == "0")
                            {
                                Helper.OnSoundPlay(pActionInfo.m_strEffectHit_Sound, false);

                                if ((pActionInfo.m_strEffectHit_Target != "0" || pActionInfo.m_strEffectHit_Center != "0") && m_SelectTargetSlotList.Contains(item.Value) == true)
                                {
                                    Vector3 vPos_Target = item.Value.GetPosition();
                                    vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                                    if (pActionInfo.m_strEffectHit_Target != "0" &&
                                        InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Contains(pActionInfo.m_strEffectHit_Target) == false)
                                    {
                                        ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit_Target, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                                        InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Add(pActionInfo.m_strEffectHit_Target);
                                    }

                                    if (pActionInfo.m_strEffectHit_Center != "0" &&
                                        InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Contains(pActionInfo.m_strEffectHit_Center) == false)
                                    {
                                        ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit_Center, InGameInfo.Instance.m_vSlotGridCenter).SetScale(InGameInfo.Instance.m_fInGameScale);
                                        InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Add(pActionInfo.m_strEffectHit_Center);
                                    }

                                    float fDelay = pActionInfo.m_fEffectHit_Target_Delay;

                                    if (fDelay > fDelayTime)
                                    {
                                        fDelayTime = fDelay;
                                    }
                                }
                            }
                            else
                            {
                                OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_PlayerCharacterSkill_CutScene_UIDone Missile Attack");

                                IsProjectile = true;

                                if (pActionInfo.m_eDamageType == eDamageEffectType.BlockShuffle)
                                {
                                    EventDelegateManager.Instance.OnEventInGame_BlockShuffleSkill -= OnInGame_BlockShuffleSkill;
                                    EventDelegateManager.Instance.OnEventInGame_BlockShuffleSkill += OnInGame_BlockShuffleSkill;
                                }

                                GameEvent_BoosterItemAttackMissile pGameEvent = new GameEvent_BoosterItemAttackMissile(m_vInGame_Main_UI_CurrItem_ScreenPos, item.Value, pActionInfo, fDamageRate, m_SelectTargetSlotList.Contains(item.Value), item.Key);
                                GameEventManager.Instance.AddGameEvent(pGameEvent);
                            }
                        }

                        yield return new WaitForSeconds(fDelayTime);

                        foreach (KeyValuePair<DamageTargetInfo, Slot> item in m_pDamageSlotTable)
                        {
                            float fDamageRate = ((float)item.Key.m_pRangeInfo.m_nRelativeDamageRate) / 100.0f;
                            if (pActionInfo.m_strEffectMissile == "0")
                            {
                                Helper.OnSoundPlay(pActionInfo.m_strEffectHit_Sound, false);

                                if (pActionInfo.m_strEffectHit != "0")
                                {
                                    Vector3 vPos_Target = item.Value.GetPosition();
                                    vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;

                                    ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit, vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                                }

                                OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_PlayerCharacterSkill_CutScene_UIDone Immediate Attack");

                                if (pActionInfo.m_eDamageType == eDamageEffectType.BlockShuffle)
                                {
                                    EventDelegateManager.Instance.OnEventInGame_BlockShuffleSkill -= OnInGame_BlockShuffleSkill;
                                    EventDelegateManager.Instance.OnEventInGame_BlockShuffleSkill += OnInGame_BlockShuffleSkill;
                                }

                                MainGame_Espresso_ProcessHelper.OnSkillActionDamage(null, item.Value, pActionInfo, pActionInfo.m_eDamageType, pActionInfo.m_eDamageBaseUnitRole, pActionInfo.m_eDamageBaseUnitProperty,
                                                                                    pActionInfo.m_nDamageBaseRate, fDamageRate, item.Key.m_pConditionInfo.m_fRelativeDamageRate);

                                MainGame_Espresso_ProcessHelper.ApplyStatusEffect(null, item.Value, pActionInfo, item.Key.m_pRangeInfo);
                            }
                            else
                            {
                                OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_PlayerCharacterSkill_CutScene_UIDone Missile Attack");

                                IsProjectile = true;

                                if (pActionInfo.m_eDamageType == eDamageEffectType.BlockShuffle)
                                {
                                    EventDelegateManager.Instance.OnEventInGame_BlockShuffleSkill -= OnInGame_BlockShuffleSkill;
                                    EventDelegateManager.Instance.OnEventInGame_BlockShuffleSkill += OnInGame_BlockShuffleSkill;
                                }

                                GameEvent_BoosterItemAttackMissile pGameEvent = new GameEvent_BoosterItemAttackMissile(m_vInGame_Main_UI_CurrItem_ScreenPos, item.Value, pActionInfo, fDamageRate, m_SelectTargetSlotList.Contains(item.Value), item.Key);
                                GameEventManager.Instance.AddGameEvent(pGameEvent);
                            }
                        }

                        if (IsProjectile == false)
                        {
                            OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_PlayerCharacterSkill_CutScene_UIDone Immediate Attack To SlotMoveAndCreate");

                            EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
                            EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone += OnInGame_CheckRemoveSlotDone;

                            m_IsBoosterItemTrigger = false;

                            EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate = true;

                            if (InGameInfo.Instance.m_IsPlayerCharacterSkillToSpeicalBlockAttack == false && pActionInfo.m_eDamageType != eDamageEffectType.BlockShuffle)
                            {
                                EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0.4f + m_pDataStack.m_fKnockBackDelayTime);
                                m_pDataStack.m_fKnockBackDelayTime = 0;
                            }
                        }

                        InGameInfo.Instance.m_IsInGameClick = false;
                        m_pGameObject_BlackLayer.SetActive(false);
                    }
                    break;
            }
        }
    }

    public void OnInGame_BoosterItemProjectile_Done(ExcelData_ActionInfo pActionInfo)
    {
        OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_BoosterItemProjectile_Done");

        if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.BoosterItem)
        {
            EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;
            EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone += OnInGame_CheckRemoveSlotDone;

            m_IsBoosterItemTrigger = false;

            EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate = true;

            if (InGameInfo.Instance.m_IsPlayerCharacterSkillToSpeicalBlockAttack == false && pActionInfo.m_eDamageType != eDamageEffectType.BlockShuffle)
            {
                EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0.4f + m_pDataStack.m_fKnockBackDelayTime);
                m_pDataStack.m_fKnockBackDelayTime = 0;
            }
        }
    }

    public void OnInGame_Projectile_Block_Done()
    {
        OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_Projectile_Block_Done");

        if (m_IsActive == true)
        {
        }
    }

    public void OnInGame_Projectile_SP_Charge_Done()
    {
        OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_Projectile_SP_Charge_Done");

        if (m_IsActive == true)
        {
        }
    }

    public void OnInGame_BlockShuffleSkill(SlotFixObject_Unit pUnit, ExcelData_ActionInfo pActionInfo)
    {
        OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_BlockShuffleSkill");

        if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.BoosterItem)
        {
            EspressoInfo.Instance.m_eActionTriggerSubject = eActionTriggerSubject.None;

            EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;

            EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate = false;
            InGameInfo.Instance.m_IsInGameClick = true;

            InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();

            if (m_IsActive == true)
            {
                OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_BlockShuffleSkill active");

                if (m_pDataStack.m_nCurrShildPoint > 0 && m_pDataStack.m_EnemyMinionTable.Count == 0 && m_pDataStack.m_nCurrObjectiveCount <= 0)
                {
                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                }
                else
                {
                    EspressoInfo.Instance.m_IsBoosterItemlTrigger = false;
                    InGameInfo.Instance.m_IsInGameClick = false;
                    m_pTurnComponent_BlockMatch.OnPlayerCharacterAttackDone();
                }
            }
        }
    }


    public void OnInGame_CheckRemoveSlotDone()
    {
        OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_CheckRemoveSlotDone");

        if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.BoosterItem)
        {
            EspressoInfo.Instance.m_eActionTriggerSubject = eActionTriggerSubject.None;

            EventDelegateManager.Instance.OnEventInGame_CheckRemoveSlotDone -= OnInGame_CheckRemoveSlotDone;

            InGameInfo.Instance.m_IsInGameClick = true;

            InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();

            if (m_IsActive == true)
            {
                OutputLog.Log("TurnSubComponent_BoosterItem : OnInGame_CheckRemoveSlotDone active");

                if (m_pDataStack.m_nCurrShildPoint > 0 && m_pDataStack.m_EnemyMinionTable.Count == 0 && m_pDataStack.m_nCurrObjectiveCount <= 0)
                {
                    EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
                }
                else
                {
                    EspressoInfo.Instance.m_IsBoosterItemlTrigger = false;
                    InGameInfo.Instance.m_IsInGameClick = false;
                    m_pTurnComponent_BlockMatch.OnPlayerCharacterAttackDone();
                }
            }
        }
    }

    protected void OnCallback_LButtonDown_CancelArea(GameObject gameObject, Vector3 vPos, object ob, int nFingerID)
    {
        if (m_IsAutoPlay == false)
        {
            m_nCancelAreaFingerID = nFingerID;
        }
    }

    protected void OnCallback_LButtonUp_CancelArea(GameObject gameObject, Vector3 vPos, object ob, int nFingerID, bool IsDown)
    {
        if (IsDown == true && m_nCancelAreaFingerID == nFingerID && EspressoInfo.Instance.m_IsBoosterItemlTrigger == true && 
            EspressoInfo.Instance.m_IsInGame_SkillTriggerDrag == false && m_IsAutoPlay == false)
        {
            EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Cancel();
            EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Close();

            InGameInfo.Instance.m_ActionEffectHit_TargetAndCenterList.Clear();

            if (m_pGameObject_CancelArea != null)
            {
                GameObject.Destroy(m_pGameObject_CancelArea);
                m_pGameObject_CancelArea = null;
            }

            if (m_pDataStack.m_IsCurrRefillGuideState == true)
            {
                GameObject ob_RefillGuide = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGameRefillGuide");
                GameObject.Instantiate(ob_RefillGuide);
            }

            EventDelegateManager.Instance.OnInGame_ShieldSelecTarget(false);
        }
    }

    public void OnInGame_Tooltip_DestroyCancelArea()
    {
        if (m_pGameObject_CancelArea != null)
        {
            GameObject.Destroy(m_pGameObject_CancelArea);
            m_pGameObject_CancelArea = null;
        }
    }
}
