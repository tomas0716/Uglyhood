using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class MainGame_Espresso_PVE : MainGame
{
    private TurnStatePattern            m_pTurnStatePattern         = null;
    private MainGame_DataStack          m_pDataStack                = null;

    private int                         m_nMatchGroupIndex          = 0;
    private Dictionary<int,List<Slot>>  m_MatchSlotTable            = new Dictionary<int, List<Slot>>();        // Key : Line (x), Value : Line Slot
    private List<Dictionary<int, Slot>> m_MatchSlotTableList        = new List<Dictionary<int, Slot>>();

    private Transformer_Timer           m_pTimer_EnemyTurn          = new Transformer_Timer();

    private List<Plane2D>               m_pEnemySpawnPointList      = new List<Plane2D>();

    private InGame_Combo_UI             m_pInGame_Combo_UI          = null;

    private GameObject                  m_pIngameUI                 = null;

    public class SlotBlockParameta
    {
        public int  m_nMatchGroup;
        public int  m_nCombo;
    }

    private enum eMatchActionComplete
    {
        MatchTurnComplete,
        MatchProjectileComplete,

        Max,
    }

    private bool [] m_MatchActionComplete = new bool[(int)eMatchActionComplete.Max];

    protected override void Initialize()
	{
        m_pDataStack = new MainGame_DataStack();
        DataStackManager.Instance.Push(m_pDataStack);

        m_pDataStack.m_pMainGame = this;
        m_pDataStack.m_pInGameStageMapData = Resources.Load<InGameStageMapData>("StageMap/" + EspressoInfo.Instance.m_nStageID);
        m_pDataStack.m_pCloudManager = new InGame_CloudManager();

        for (int x = 0; x < GameDefine.ms_nInGameSlot_X; ++x)
        {
            m_MatchSlotTable.Add(x, new List<Slot>());
        }

        new SP_Full_Effect_Flesh();

        PreLoading_ParticleSystem();

        ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);

        GameObject ob = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGameUI_" + pStageInfo.m_strStageTheme);

        if (ob == null)
        {
            ob = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGameUI");
        }
        m_pIngameUI = GameObject.Instantiate(ob);

        ob = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGameCombo");
        ob = GameObject.Instantiate(ob);
        m_pInGame_Combo_UI = ob.GetComponent<InGame_Combo_UI>();

        EventDelegateManager.Instance.OnEventInGame_Start += OnInGame_Start;
        EventDelegateManager.Instance.OnEventInGame_StartBoosterTurnDone += OnInGame_StartBoosterTurnDone;
        EventDelegateManager.Instance.OnEventInGame_PlayerTurnDone += OnInGame_PlayerTurnDone;
        EventDelegateManager.Instance.OnEventInGame_EnemyTurnDone += OnInGame_EnemyTurnDone;
        EventDelegateManager.Instance.OnEventInGame_Projectile_Block_Done += OnInGame_Projectile_Block_Done;

        EventDelegateManager.Instance.OnEventInGame_GameOver += OnInGame_GameOver;

        EventDelegateManager.Instance.OnEventInGame_GetInGameUIGameObject += OnInGame_GetInGameUIGameObject;

        EventDelegateManager.Instance.OnEventInGame_ChangeTurn += OnInGame_ChangeTurn;

        EventDelegateManager.Instance.OnEventInGame_StartAnimationDone += OnInGame_StartAnimationDone;

        EventDelegateManager.Instance.OnEventInGame_StartActionTurnDone += OnInGame_StartActionTurnDone;
    }

    private void PreLoading_ParticleSystem()
    {
        int nTeamDeckID = InventoryInfoManager.Instance.m_pDeckInfo.GetGameModeDeckID(EspressoInfo.Instance.m_eGameMode);

        TeamInvenInfo pTeamInvenInfo = InventoryInfoManager.Instance.m_pTeamInvenInfoGroup.GetTeamInvenInfo(nTeamDeckID);

        for (int i = GameDefine.ms_nTeamCharacterCount - 1; i >= 0; --i)
        {
            CharacterInvenItemInfo pCharacterInvenItemInfo = pTeamInvenInfo.GetInvenItemInfo(i);

            if (pCharacterInvenItemInfo != null)
            {
                ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(pCharacterInvenItemInfo.m_nTableID);

                if (pUnitInfo != null)
                {
                    PreLoading_ParticleSystem(pUnitInfo);
                }
            }
        }

        List<ExcelData_Stage_EnemySpawnInfo> EnemySpawnInfoList = ExcelDataManager.Instance.m_pStage_EnemySpawn.GetEnemySpawnInfoList(EspressoInfo.Instance.m_nStageID);

        foreach (ExcelData_Stage_EnemySpawnInfo pEnemySpawnInfo in EnemySpawnInfoList)
        {
            ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(pEnemySpawnInfo.m_nUnitTableID);

            if (pUnitInfo != null)
            {
                PreLoading_ParticleSystem(pUnitInfo);
            }
        }
    }

    private void PreLoading_ParticleSystem(ExcelData_UnitInfo pUnitInfo)
    {
        ExcelData_ActionInfo pActionInfo;

        pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(pUnitInfo.m_nAttack_ActionTableID);
        if (pActionInfo != null)
        {
            if (pActionInfo.m_strEffectMissile != "0")
            {
                GameObject ob = Resources.Load<GameObject>("Effect/Prefabs/" + pActionInfo.m_strEffectMissile);
                ob = GameObject.Instantiate(ob);
                GameObject.Destroy(ob);
            }

            if (pActionInfo.m_strEffectHit != "0")
            {
                GameObject ob = Resources.Load<GameObject>("Effect/Prefabs/" + pActionInfo.m_strEffectHit);
                ob = GameObject.Instantiate(ob);
                GameObject.Destroy(ob);
            }
        }

        pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(pUnitInfo.m_nActiveSkill_ActionTableID);
        if (pActionInfo != null)
        {
            if (pActionInfo.m_strEffectMissile != "0")
            {
                GameObject ob = Resources.Load<GameObject>("Effect/Prefabs/" + pActionInfo.m_strEffectMissile);
                ob = GameObject.Instantiate(ob);
                GameObject.Destroy(ob);
            }

            if (pActionInfo.m_strEffectHit != "0")
            {
                GameObject ob = Resources.Load<GameObject>("Effect/Prefabs/" + pActionInfo.m_strEffectHit);
                ob = GameObject.Instantiate(ob);
                GameObject.Destroy(ob);
            }
        }
    }

    protected override void Destroy()
    {
        m_pTurnStatePattern.OnDestroy();

        EventDelegateManager.Instance.OnEventInGame_Start -= OnInGame_Start;
        EventDelegateManager.Instance.OnEventInGame_StartBoosterTurnDone -= OnInGame_StartBoosterTurnDone;
        EventDelegateManager.Instance.OnEventInGame_PlayerTurnDone -= OnInGame_PlayerTurnDone;
        EventDelegateManager.Instance.OnEventInGame_EnemyTurnDone -= OnInGame_EnemyTurnDone;
        EventDelegateManager.Instance.OnEventInGame_Projectile_Block_Done -= OnInGame_Projectile_Block_Done;

        EventDelegateManager.Instance.OnEventInGame_GameOver -= OnInGame_GameOver;

        EventDelegateManager.Instance.OnEventInGame_GetInGameUIGameObject -= OnInGame_GetInGameUIGameObject;

        EventDelegateManager.Instance.OnEventInGame_ChangeTurn -= OnInGame_ChangeTurn;

        EventDelegateManager.Instance.OnEventInGame_StartAnimationDone -= OnInGame_StartAnimationDone;

        EventDelegateManager.Instance.OnEventInGame_StartActionTurnDone -= OnInGame_StartActionTurnDone;
    }

    protected override void Inner_Update()
    {
        m_pTurnStatePattern.Update();
        m_pTimer_EnemyTurn.Update(Time.deltaTime);
        SP_Full_Effect_Flesh.Update();
        m_pDataStack.m_pCloudManager.Update();
    }

    protected override void Inner_LateUpdate()
    {
        m_pTurnStatePattern.LateUpdate();
    }

    protected override void SlotManagerCreateBefore()
    {
        m_pDataStack.m_pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);

        m_pDataStack.m_nCurrShildPoint = m_pDataStack.m_pStageInfo.m_nMaxShieldPoint;
        m_pDataStack.m_nPlayerTurnCount = 0;
        m_pDataStack.m_nEnemyTurnCount = 0;

        InGameInfo.Instance.InGameStart_Reset();

#if UNITY_EDITOR
        if (GameConfig.Instance.m_IsInGameAuto == true)
        {
            InGameInfo.Instance.m_IsAutoPlay = true;
        }
#endif

        InGameInfo.Instance.m_nSlotStartIndex_X = 0;
        InGameInfo.Instance.m_nSlotStartIndex_Y = 0;
        InGameInfo.Instance.m_nSlotEndIndex_X = InGameSetting.Instance.m_pInGameSettingData.m_vGrid.x - 1;
        InGameInfo.Instance.m_nSlotEndIndex_Y = InGameSetting.Instance.m_pInGameSettingData.m_vGrid.y - 1;
        InGameInfo.Instance.m_nSlotCount_X = InGameSetting.Instance.m_pInGameSettingData.m_vGrid.x;
        InGameInfo.Instance.m_nSlotCount_Y = InGameSetting.Instance.m_pInGameSettingData.m_vGrid.y;


        float fRatio_X = InGameInfo.Instance.m_fGameArea_Width / InGameInfo.Instance.m_nSlotCount_X;
        float fRatio_Y = (InGameInfo.Instance.m_fGameArea_Height) / InGameInfo.Instance.m_nSlotCount_Y;

        float fDifferHeight = 0;

        if (fRatio_X > fRatio_Y)        // 세로로 피트
        {
            InGameInfo.Instance.m_fSlotSize = InGameInfo.Instance.m_fGameArea_Height / InGameInfo.Instance.m_nSlotCount_Y;
            InGameInfo.Instance.m_fInGameScale = InGameInfo.Instance.m_fSlotSize / GameDefine.ms_fBaseSlotSize;
		}
        else                            // 가로로 피트
        {
            InGameInfo.Instance.m_fSlotSize = InGameInfo.Instance.m_fGameArea_Width / InGameInfo.Instance.m_nSlotCount_X;
            InGameInfo.Instance.m_fInGameScale = InGameInfo.Instance.m_fSlotSize / GameDefine.ms_fBaseSlotSize;

            fDifferHeight = (InGameInfo.Instance.m_fGameArea_Height - (InGameInfo.Instance.m_nSlotCount_Y * InGameInfo.Instance.m_fSlotSize));
            fDifferHeight *= 0.5f;
        }
        
        InGameInfo.Instance.m_vSlot_LeftTopPos = InGameInfo.Instance.m_vGameAreaCenter;

        InGameInfo.Instance.m_vSlot_LeftTopPos.x = -((InGameInfo.Instance.m_nSlotCount_X * 0.5f) * InGameInfo.Instance.m_fSlotSize - InGameInfo.Instance.m_fSlotSize * 0.5f);
        InGameInfo.Instance.m_vSlot_LeftTopPos.y += (InGameInfo.Instance.m_nSlotCount_Y * 0.5f) * InGameInfo.Instance.m_fSlotSize - InGameInfo.Instance.m_fSlotSize * 0.5f + fDifferHeight;
        
        InGameInfo.Instance.m_vSlotGridCenter = InGameInfo.Instance.m_vSlot_LeftTopPos;
        InGameInfo.Instance.m_vSlotGridCenter.x = 0;
        InGameInfo.Instance.m_vSlotGridCenter.y -= (InGameInfo.Instance.m_nSlotCount_Y * 0.5f) * InGameInfo.Instance.m_fSlotSize;
        InGameInfo.Instance.m_vSlotGridCenter.z = -(float)ePlaneOrder.Fx_TopLayer;

        m_pTurnStatePattern = new TurnStatePattern();
        m_pDataStack.m_pTurnStatePattern = m_pTurnStatePattern;

        //InGameTurnLog.Log("New Game Start");
        OutputLog.Log("New Game Start");
    }

    protected override void SlotManagerCreateAfter()
    {
        m_pDataStack.m_pSlotManager = m_pSlotManager;

        for (int i = 0; i < GameDefine.ms_nBlockCount; ++i)
		{
			for (int j = 0; j < m_pDataStack.m_pStageInfo.m_nBlockRate[i]; ++j)
			{
				m_pSlotManager.AddAppearBlockType(eBlockType.Block_Start + i);
			}
		}

		m_pSlotManager.OnAppaerBlockTypeEnd();

		m_pSlotManager.SetCreateSlotLastBlockTypePercent(m_pDataStack.m_pStageInfo.m_nSameBlockAvoidRate);
	}

    public override void OnSlotManagerInitializeDone() 
    {
        ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);

        Helper.OnSoundPlay("BGM_STAGE", true);
        Dictionary<int, Slot> slotTable = m_pSlotManager.GetSlotTable();

        List<InGameStageMapSlotData> enemySpawnList = m_pDataStack.m_pInGameStageMapData.GetInGameStageMapSlotData(eMapSlotItem.EnemySpawn);
        foreach (InGameStageMapSlotData pInGameStageMapSlotData in enemySpawnList)
        {
            Vector2Int vIndex = ExcelDataHelper.GetSlotConverting(pInGameStageMapSlotData.m_nX, pInGameStageMapSlotData.m_nY);
            int nSlotIndex = Helper.GetSlotIndex(vIndex.x, vIndex.y);

            bool IsLeft = false;
            bool IsRight = false;

            if (m_pDataStack.m_pInGameStageMapData.IsInGameStageMapSlotData(pInGameStageMapSlotData.m_nX - 1, pInGameStageMapSlotData.m_nY, eMapSlotItem.EnemySpawn) == true)
            {
                IsLeft = true;
            }

            if (m_pDataStack.m_pInGameStageMapData.IsInGameStageMapSlotData(pInGameStageMapSlotData.m_nX + 1, pInGameStageMapSlotData.m_nY, eMapSlotItem.EnemySpawn) == true)
            {
                IsRight = true;
            }

            string strPrefabName = "EnemySpawnPoint_Center";

            if ((IsLeft == true && IsRight == true) || (IsLeft == false && IsRight == false))
            {
                strPrefabName = "EnemySpawnPoint_Center";
            }
            else if (IsLeft == false)
            {
                strPrefabName = "EnemySpawnPoint_Left";
            }
            else if (IsRight == false)
            {
                strPrefabName = "EnemySpawnPoint_Right";
            }

			Slot pSlot = m_pSlotManager.GetSlotTable()[nSlotIndex];
			Vector3 vPos = pSlot.GetPosition();
			vPos.y += InGameInfo.Instance.m_fSlotSize * 1.11f;
			GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/" + strPrefabName);
			ob = GameObject.Instantiate(ob);
			ob.transform.localPosition = vPos;
			ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
            Plane2D pPlane2D = ob.GetComponent<Plane2D>();
            m_pEnemySpawnPointList.Add(pPlane2D);
            SpawnPointObject pSpawnPointObject = ob.GetComponent<SpawnPointObject>();
            pSpawnPointObject.Init(pSlot);

            string strAtlasInfoName = "Block_BG_Enemy_Center";

            if ((IsLeft == true && IsRight == true) || (IsLeft == false && IsRight == false))
            {
                strAtlasInfoName = "Block_BG_Enemy_Center";
            }
            else if (IsLeft == false)
            {
                strAtlasInfoName = "Block_BG_Enemy_Left";
            }
            else if (IsRight == false)
            {
                strAtlasInfoName = "Block_BG_Enemy_Right";
            }

            Helper.Change_Plane2D_AtlasInfo(pPlane2D, "Slot_" + pStageInfo.m_strStageTheme, strAtlasInfoName);

            ob = Helper.FindChildGameObject(ob, "Plane2D");
            pPlane2D = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane2D, "Slot_" + pStageInfo.m_strStageTheme, "EnemySpawnPoint");
        }

        for (int i = GameDefine.ms_nInGameSlot_X - 1; i >= 0; --i)
        {
			string strPrefabName = "BG_Character_Center";

            if (i == GameDefine.ms_nInGameSlot_X - 1)
            {
                strPrefabName = "BG_Character_Left";
            }
            else if (i == 0)
            {
                strPrefabName = "BG_Character_Right";
            }

            int nSlotIndex = Helper.GetSlotIndex(i, 0);
            Vector3 vPos = slotTable[nSlotIndex].GetPosition();
            vPos.y -= 9.0f * InGameInfo.Instance.m_fInGameScale;
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/" + strPrefabName);
            ob = GameObject.Instantiate(ob);
            ob.transform.localPosition = vPos;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

            Plane2D pPlane2D = ob.GetComponent<Plane2D>();
            string strAtlasInfoName = "Block_BG_Character_Center";
            if (i == GameDefine.ms_nInGameSlot_X - 1)
            {
                strAtlasInfoName = "Block_BG_Character_Left";
            }
            else if (i == 0)
            {
                strAtlasInfoName = "Block_BG_Character_Right";
            }
            Helper.Change_Plane2D_AtlasInfo(pPlane2D, "Slot_" + pStageInfo.m_strStageTheme, strAtlasInfoName);
        }

        Slot pSlot_Top = null;
        for (int y = GameDefine.ms_nInGameSlot_Y - 1; y >= 0; --y)
        {
            for (int x = 0; x < GameDefine.ms_nInGameSlot_X; ++x)
            {
                if (m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(x, y)) == true)
                {
                    pSlot_Top = m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(x, y)];

                    break;
                }
            }

            if (pSlot_Top != null)
                break;
        }

        if (pSlot_Top != null)
        {
            float fHeight = Screen.height * 0.5f;
            float fSlotTop = pSlot_Top.GetPosition().y + InGameInfo.Instance.m_fSlotSize * 0.5f;
            float fSize = fHeight - fSlotTop;

            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Up");
            ob = GameObject.Instantiate(ob);
            Plane2D pPlane2D = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane2D, "Slot_" + pStageInfo.m_strStageTheme, "Block_BG_3");
            pPlane2D.SetSize(new Vector2(788 * InGameInfo.Instance.m_fInGameScale, fSize));
            ob.transform.position = new Vector3(0, fSlotTop + fSize * 0.5f, 0);
        }

        Slot pSlot_Bottom = null;
        for (int y = 0; y < GameDefine.ms_nInGameSlot_Y; ++y)
        {
            for (int x = 0; x < GameDefine.ms_nInGameSlot_X; ++x)
            {
                if (m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(x, y)) == true)
                {
                    pSlot_Bottom = m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(x, y)];

                    break;
                }
            }

            if (pSlot_Bottom != null)
                break;
        }

        if (pSlot_Bottom != null)
        {
            float fHeight = Screen.height * -0.5f;
            float fSlotBottom = pSlot_Bottom.GetPosition().y - InGameInfo.Instance.m_fSlotSize * 0.5f;
            float fSize = fHeight - fSlotBottom;

            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Bottom");
            ob = GameObject.Instantiate(ob);
            Plane2D pPlane2D = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane2D, "Slot_" + pStageInfo.m_strStageTheme, "Block_BG_2");
            pPlane2D.SetSize(new Vector2(788 * InGameInfo.Instance.m_fInGameScale, -fSize));
            ob.transform.position = new Vector3(0, fSlotBottom + fSize * 0.5f, 0);
        }

        List <InGameStageMapSlotData> enemyColonyList = m_pDataStack.m_pInGameStageMapData.GetInGameStageMapSlotData(eMapSlotItem.EnemyColony);

		foreach (InGameStageMapSlotData pInGameStageMapSlotData in enemyColonyList)
		{
			Vector2Int vIndex = ExcelDataHelper.GetSlotConverting(pInGameStageMapSlotData.m_nX, pInGameStageMapSlotData.m_nY);
			int nSlotIndex = Helper.GetSlotIndex(vIndex.x, vIndex.y);

            ExcelData_EnemyColonyInfo pEnemyColonyInfo = ExcelDataManager.Instance.m_pEnemyColony.GetEnemyColonyInfo(GameDefine.ms_nBasicEnemyColonyID);
            SlotFixObject_EnemyColony pSlotFixObject_EnemyColony = new SlotFixObject_EnemyColony(slotTable[nSlotIndex], nSlotIndex, pEnemyColonyInfo, 1);
			slotTable[nSlotIndex].AddSlotFixObject(pSlotFixObject_EnemyColony);
			m_pDataStack.m_EnemyColonyTable.Add(nSlotIndex, pSlotFixObject_EnemyColony);
		}

        List<InGameStageMapSlotData> unitStageMapDataList = m_pDataStack.m_pInGameStageMapData.GetInGameStageMapSlotData(eMapSlotItem.Unit);

        foreach (InGameStageMapSlotData pInGameStageMapSlotData in unitStageMapDataList)
        {
            ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(pInGameStageMapSlotData.m_pInGameStageMapMetaData.m_nMetaData);

            Vector2Int vIndex = ExcelDataHelper.GetSlotConverting(pInGameStageMapSlotData.m_nX, pInGameStageMapSlotData.m_nY);
            int nSlotIndex = Helper.GetSlotIndex(vIndex.x, vIndex.y);

            // Create EnemyColony
            if (m_pDataStack.m_EnemyColonyTable.ContainsKey(nSlotIndex) == false)
            {
                ExcelData_EnemyColonyInfo pEnemyColonyInfo = ExcelDataManager.Instance.m_pEnemyColony.GetEnemyColonyInfo(GameDefine.ms_nBasicEnemyColonyID);
                SlotFixObject_EnemyColony pSlotFixObject_EnemyColony = new SlotFixObject_EnemyColony(slotTable[nSlotIndex], nSlotIndex, pEnemyColonyInfo, 1/*pUnitInfo.m_nColonyGenLevel*/);
                slotTable[nSlotIndex].AddSlotFixObject(pSlotFixObject_EnemyColony);
                m_pDataStack.m_EnemyColonyTable.Add(nSlotIndex, pSlotFixObject_EnemyColony);
            }
            else
            {
                SlotFixObject_EnemyColony pSlotFixObject_EnemyColony = m_pDataStack.m_EnemyColonyTable[nSlotIndex];
                if (1/*pUnitInfo.m_nColonyGenLevel*/ > pSlotFixObject_EnemyColony.GetHP())
                {
                    pSlotFixObject_EnemyColony.ChangeHP(1/*pUnitInfo.m_nColonyGenLevel*/);
                }
            }

            eObjectType eObType = eObjectType.Minion;
            if (pUnitInfo.m_eUnitType == eUnitType.BossMinion)
                eObType = eObjectType.MinionBoss;

            // Create EnemyMinion
            SlotFixObject_Minion pSlotFixObject_Minion = null;

            if (!EspressoInfo.Instance.m_IsChallengeMode)
            {
                pSlotFixObject_Minion = new SlotFixObject_Minion(slotTable[nSlotIndex], pInGameStageMapSlotData.m_pInGameStageMapMetaData.m_nMetaData, pStageInfo.m_nEnemyLevel, eMinionType.EnemyMinion, eObType, eOwner.Other, false);
            }
            else
            {
                pSlotFixObject_Minion = new SlotFixObject_Minion(slotTable[nSlotIndex], pInGameStageMapSlotData.m_pInGameStageMapMetaData.m_nMetaData, ExcelDataManager.Instance.m_pStage_ChallengeMode.GetStageMissionInfo_byStageID(EspressoInfo.Instance.m_nStageID).m_nEnemyLevel, eMinionType.EnemyMinion, eObType, eOwner.Other, false);
            }

            slotTable[nSlotIndex].AddSlotFixObject(pSlotFixObject_Minion);
            m_pDataStack.m_EnemyMinionTable.Add(nSlotIndex, pSlotFixObject_Minion);

            if (pUnitInfo.m_eUnitType == eUnitType.BossMinion)
            {
                m_pDataStack.m_EnemyMinionBossAppearList.Add(pSlotFixObject_Minion);
            }
        }

        // Player Character 
        int nTeamDeckID = InventoryInfoManager.Instance.m_pDeckInfo.GetGameModeDeckID(EspressoInfo.Instance.m_eGameMode);
        TeamInvenInfo pTeamInvenInfo = InventoryInfoManager.Instance.m_pTeamInvenInfoGroup.GetTeamInvenInfo(nTeamDeckID);

		for (int i = GameDefine.ms_nTeamCharacterCount - 1; i >= 0; --i)
		{
			CharacterInvenItemInfo pCharacterInvenItemInfo = pTeamInvenInfo.GetInvenItemInfo(i);

			if (pCharacterInvenItemInfo != null)
			{
				ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(pCharacterInvenItemInfo.m_nTableID);

				if (pUnitInfo != null)
				{
                    m_pDataStack.m_nPlayerCharacterAttackValue[(int)pUnitInfo.m_eElement] += pCharacterInvenItemInfo.m_nATK;

                    int nSlotIndex = Helper.GetSlotIndex(GameDefine.ms_nTeamCharacterCount - i - 1, 0);
					SlotFixObject_PlayerCharacter pSlotFixObject_PlayerCharacter = new SlotFixObject_PlayerCharacter(slotTable[nSlotIndex], pUnitInfo.m_nID, pCharacterInvenItemInfo.m_nUniqueID, eObjectType.Character, eOwner.My);
					slotTable[nSlotIndex].AddSlotFixObject(pSlotFixObject_PlayerCharacter);
					m_pDataStack.m_PlayerCharacterTable.Add(nSlotIndex, pSlotFixObject_PlayerCharacter);
				}
			}
		}

		List<ExcelData_Stage_EnemySpawnInfo> EnemySpawnInfoList = ExcelDataManager.Instance.m_pStage_EnemySpawn.GetEnemySpawnInfoList(EspressoInfo.Instance.m_nStageID);
        foreach (ExcelData_Stage_EnemySpawnInfo pEnemySpawnInfo in EnemySpawnInfoList)
        {
            m_pDataStack.m_EnemySpawnInfoCountTable.Add(pEnemySpawnInfo, 0);
            m_pDataStack.m_EnemySpawnInfoIntervalTurnTable.Add(pEnemySpawnInfo, 1);
        }

        EventDelegateManager.Instance.OnInGame_InitEnemyCount();

        foreach (KeyValuePair<int,Slot> item in slotTable)
        {
            if (item.Value.GetLastSlotFixObject() == null && item.Value.IsPossibleMove() == true)
            {
                if (item.Value.GetY() == 0)
                {
                    SlotFixObject pSlotFixObject = new SlotFixObject(eSlotFixObjectType.None);
                    item.Value.AddSlotFixObject(pSlotFixObject);
                }
            }
        }

        EnqueueInitCreateBlock();
        HideEnemyMinion();

        AppInstance.Instance.m_pEventDelegateManager.OnDeleteInGameLoading();

#if UNITY_EDITOR
        if (GameConfig.Instance.m_IsInGameAuto == true)
        {
            EventDelegateManager.Instance.OnInGame_ChangeAutoPlay(InGameInfo.Instance.m_IsAutoPlay);
        }
#endif
    }

    public void EnqueueInitCreateBlock()
    {
        foreach (Slot pSlot in m_pDataStack.m_pSlotManager.GetReverseSlotList())
        {
            if (pSlot.GetSlotBlock() != null)
            {
                SlotFixObject_Obstacle pObstacle_OnlyMineBreak = pSlot.FindFixObject_SlotDyingAtOnlyMineBreak();

                if (pObstacle_OnlyMineBreak == null)
                {
                    m_pDataStack.m_pSlotManager.EnqueueInitCreateBlockType(pSlot.GetX(), pSlot.GetSlotBlock().GetBlockType(), pSlot.GetSlotBlock().GetMapSlotItem());
                    pSlot.RemoveSlotBlock();
                }
            }
        }
    }

    public void HideEnemyMinion()
    {
        foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pDataStack.m_EnemyMinionTable)
        {
            item.Value.SetVisible(false);
        }
    }

    public void OnInGame_Start()
    {
        m_pTurnStatePattern.OnStartActionTurn();
    }

    public override void OnPossibleBlockSwap(Slot pSlot_01, Slot pSlot_02) 
    {
    }

    public override void OnMatchSlot(Dictionary<int, Slot> slotTable, eBlockType eBlockType = eBlockType.Empty)
    {
        ReCalculationMatchDamage();

        List<int> RemoveSlotIndexList = new List<int>();

        foreach (KeyValuePair<int, Slot> item in slotTable)
        {
            if (item.Value.IsSlotFixObject_Obstacle() == true)
            {
                RemoveSlotIndexList.Add(item.Key);
            }
        }

        foreach (int nSlotIndex in RemoveSlotIndexList)
        {
            slotTable.Remove(nSlotIndex);
        }

        m_MatchSlotTableList.Add(slotTable);

        OutputLog.Log("MainGame_Espresso_PVE : OnMatchSlot Begin");

        for (int i = 0; i < (int)eMatchActionComplete.Max; ++i)
        {
            m_MatchActionComplete[i] = false;
        }

        m_pDataStack.m_nCombo += 1;

        Helper.OnSoundPlay("INGAME_BLOCK_MATCH_COMBO_" + m_pDataStack.m_nCombo, false);

        if (m_pDataStack.m_nCombo > 1)
        {
            // Combo Display
            float fBonusRate = ExcelDataManager.Instance.m_pMatchDamage_ComboBonus.GetBonusRate(m_pDataStack.m_nCombo);
            float fScale = ExcelDataManager.Instance.m_pMatchDamage_ComboBonus.GetScale(m_pDataStack.m_nCombo);

            m_pInGame_Combo_UI.SetCombo(m_pDataStack.m_nCombo, fBonusRate / 100.0f, fScale);
        }

        foreach (KeyValuePair<int, Slot> item in slotTable)
        {
            if (m_MatchSlotTable[item.Value.GetX()].Contains(item.Value) == false)
            {
                if (item.Value.GetSlotBlock() != null)
                {
                    SlotBlockParameta pParameta = new SlotBlockParameta();
                    pParameta.m_nMatchGroup = m_nMatchGroupIndex;
                    pParameta.m_nCombo = m_pDataStack.m_nCombo;
                    item.Value.GetSlotBlock().SetParameta(pParameta);
                    m_MatchSlotTable[item.Value.GetX()].Add(item.Value);

                    if (item.Value.GetSlotBlock().GetSpecialItem() == eSpecialItem.Square3 && m_pDataStack.m_pSlotManager.GetCombinationTransSpecialSlotTable().ContainsKey(item.Key) == false)
                    {
                        eElement eElem = (eElement)item.Value.GetSlotBlock().GetBlockType();
                        Vector3 vPos = item.Value.GetPosition();
                        vPos.z = -(float)ePlaneOrder.Fx_TopLayer;
                        ParticleManager.Instance.LoadParticleSystem("FX_ElementalBomb_" + eElem.ToString(), vPos).SetScale(InGameInfo.Instance.m_fInGameScale);

                        Helper.OnSoundPlay("INGAME_4MATCH_BOMB_ACTIVATE", false);
                    }
                }
            }
        }

        ++m_nMatchGroupIndex;

        OutputLog.Log("MainGame_Espresso_PVE : OnMatchSlot End");
    }

    public override void OnDoneSlotMoveAndCreate() 
    {
    }

    public override void OnBeforeBlockDying(Slot pSlot)
    {
        if (SlotManager.IsCombinationBlock(pSlot) == true)
        {
            eElement eElement = (eElement)pSlot.GetSlotBlock().GetBlockType();
            Vector3 vPos = pSlot.GetPosition();
            vPos.z = -(float)ePlaneOrder.Fx_TopLayer;
            ParticleManager.Instance.LoadParticleSystem("FX_BlockMatch_" + eElement, vPos).SetScale(InGameInfo.Instance.m_fInGameScale);

            OnPlayerCharacter_SP_Charge(pSlot);
        }
    }

    public override void OnAfterBlockDying(Slot pSlot)
    {
    }

    public override void OnBeforeChangeBlockShape(Slot pSlot, eSpecialItem specialItem)
    {
    }

    public override void OnAfterChangeBlockShape(Slot pSlot, eSpecialItem specialItem)
    {
        switch (specialItem)
        {
            case eSpecialItem.Plus_B5:
            case eSpecialItem.Square3:
                {
                    Helper.OnSoundPlay("INGAME_4MATCH_BOMB_GENERATE", false);
                }
                break;
            case eSpecialItem.Match_Color:
				{
                    Helper.OnSoundPlay("INGAME_5MATCH_ELIXIR_GENERATE", false);
                }
                break;
        }
    }

    public override void OnDoneAllBlockDying()
    {
        Dictionary<int, Slot> TransSpecialSlotTable = m_pSlotManager.GetCombinationTransSpecialSlotTable();
        foreach (KeyValuePair<int, Slot> item in TransSpecialSlotTable)
        {
            if (item.Value.GetSlotBlock() != null)
            {
                eElement eElement = (eElement)item.Value.GetSlotBlock().GetBlockType();
                Vector3 vPos = item.Value.GetPosition();
                vPos.z = -(float)ePlaneOrder.Fx_TopLayer;
                ParticleManager.Instance.LoadParticleSystem("FX_BlockMatch_" + eElement, vPos).SetScale(InGameInfo.Instance.m_fInGameScale);

                OnPlayerCharacter_SP_Charge(item.Value);
            }
        }
    }

    private void OnPlayerCharacter_SP_Charge(Slot pSlot)
    {
        OutputLog.Log("MainGame_Espresso_PVE : OnPlayerCharacter_SP_Charge Begin");

        if (pSlot != null && pSlot.GetSlotBlock() != null)
        {
            eElement eElement = (eElement)pSlot.GetSlotBlock().GetBlockType();

            if (eElement >= eElement.Water && eElement <= eElement.Dark)
            {
                foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_pDataStack.m_PlayerCharacterTable)
                {
                    SlotFixObject_PlayerCharacter pPlayerCharacter = item.Value;
                    if (pPlayerCharacter.GetHP() > 0 && pPlayerCharacter.GetUnitInfo().m_eElement == eElement && pPlayerCharacter.GetUnitInfo().m_nMaxSP != 0 && pPlayerCharacter.GetUnitInfo().m_nActiveSkill_ActionTableID != 0)
                    {
                        GameEvent_BlockMatch_SP_ChargeMissile pGameEvent = new GameEvent_BlockMatch_SP_ChargeMissile(pSlot, pPlayerCharacter, eElement);
                        GameEventManager.Instance.AddGameEvent(pGameEvent);
                    }
                }
            }
        }

        OutputLog.Log("MainGame_Espresso_PVE : OnPlayerCharacter_SP_Charge End");
    }

    private int CompareMatchSlotList(Slot A, Slot B)
    {
        return B.GetY().CompareTo(A.GetY());
    }

    private void OnCheckNeighborEnemyColony(Slot pSlot)
    {
        SlotFixObject pSlotFixObject = pSlot.GetLastSlotFixObject();
        if (pSlotFixObject != null)
        {
            SlotFixObject_Espresso pSlotFixObject_Espresso = pSlotFixObject as SlotFixObject_Espresso;

            if (pSlotFixObject_Espresso != null && pSlotFixObject_Espresso.GetObjectType() == eObjectType.EnemyColony)
            {
                Vector3 vPos_Target = pSlot.GetPosition();
                vPos_Target.z = -(float)ePlaneOrder.Fx_TopLayer;
                ParticleManager.Instance.LoadParticleSystem("FX_EnemyColony_Destroy", vPos_Target).SetScale(InGameInfo.Instance.m_fInGameScale);
                pSlotFixObject_Espresso.OnDead();
                m_pDataStack.m_EnemyColonyTable.Remove(pSlot.GetSlotIndex());
            }
        }
    }

    private void OnSpecialBlockDyingToFixObject(SlotFixObject_Espresso pSlotFixObject, eElement eBlockElement)
    {
        ReCalculationMatchDamage();

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        if (pSlotFixObject != null)
        {
            int nHP = pSlotFixObject.GetHP();

            switch (pSlotFixObject.GetObjectType())
            {
                case eObjectType.EnemyColony:
                    {
                        Vector3 vPos = pSlotFixObject.GetSlot().GetPosition();
                        vPos.z = -(float)ePlaneOrder.Character_MatchDamage_Effect;
                        ParticleManager.Instance.LoadParticleSystem("FX_BlockMatchDamage", vPos).SetScale(InGameInfo.Instance.m_fInGameScale);

                        pSlotFixObject.ChangeHP(nHP - 1);

                        Helper.OnSoundPlay("INGAME_BLOCK_MATCH_MISSILE_HIT", false);
                    }
                    break;

                case eObjectType.Minion:
                case eObjectType.MinionBoss:
                    {
                        Vector3 vPos = pSlotFixObject.GetSlot().GetPosition();
                        vPos.z = -(float)ePlaneOrder.Character_MatchDamage_Effect;
                        ParticleManager.Instance.LoadParticleSystem("FX_BlockMatchDamage", vPos).SetScale(InGameInfo.Instance.m_fInGameScale);

                        SlotFixObject_Minion pMinion = pSlotFixObject as SlotFixObject_Minion;

                        float fCorrelationRate = ExcelDataManager.Instance.m_pElement_Correlation.GetCorrelationRate(eBlockElement, pMinion.GetUnitInfo().m_eElement);

                        Color color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage];
                        if (fCorrelationRate == 100 && pDataStack.m_nPlayerCharacterAttackValue[(int)eBlockElement] > 0)
                        {
                            color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Damage];
                        }
                        else if (fCorrelationRate > 100 && pDataStack.m_nPlayerCharacterAttackValue[(int)eBlockElement] > 0)
                        {
                            color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Strong];
                            pSlotFixObject.OnDamageText(eUnitDamage.Strong);
                        }
                        else if (fCorrelationRate < 100 || pDataStack.m_nPlayerCharacterAttackValue[(int)eBlockElement] == 0)
                        {
                            color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Weak];

                            if (pDataStack.m_nPlayerCharacterAttackValue[(int)eBlockElement] == 0)
                            {
                                color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Hero_Missing];
                                pSlotFixObject.OnDamageText(eUnitDamage.Hero_Missing);
                            }

                            if (fCorrelationRate < 100 && pDataStack.m_nPlayerCharacterAttackValue[(int)eBlockElement] > 0)
                            {
                                color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Weak];
                                pSlotFixObject.OnDamageText(eUnitDamage.Weak);
                            }
                        }

                        fCorrelationRate -= pMinion.GetAdd_ElementResist(eBlockElement);
                        if (fCorrelationRate < 0) fCorrelationRate = 0;

                        float fMatchDamage = (pDataStack.m_nPlayerCharacterAttackValue[(int)eBlockElement] / 3) * (fCorrelationRate / 100);
                        int nMatchDamage = (int)Math.Round(fMatchDamage, MidpointRounding.AwayFromZero);
                        nMatchDamage = nMatchDamage > 1 ? nMatchDamage : 1;
                        nMatchDamage *= (int)(ExcelDataManager.Instance.m_pMatchDamage_ComboBonus.GetBonusRate(1) / 100);

                        pSlotFixObject.ChangeHP(nHP - nMatchDamage, color);

                        Helper.OnSoundPlay("INGAME_BLOCK_MATCH_MISSILE_HIT", false);
                    }
                    break;
                case eObjectType.EnemyBoss:
                    {
                    }
                    break;
            }

            if (pSlotFixObject.GetHP() <= 0)
            {
                switch (pSlotFixObject.GetObjectType())
                {
                    case eObjectType.EnemyColony:
                        {
                            pDataStack.m_EnemyColonyTable.Remove(pSlotFixObject.GetSlot().GetSlotIndex());
                            --pDataStack.m_pSlotManager.m_nTurnCompleteCheck;
                            OutputLog.Log("--pDataStack.m_pSlotManager.m_nTurnCompleteCheck > MainGame_Espresso_PVE : OnSpecialBlockDyingToFixObject : EnemyColony");

                            if (pDataStack.m_pSlotManager.m_nTurnCompleteCheck == 0)
                            {
                                EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(0);
                            }
                        }
                        break;
                    case eObjectType.Minion:
                    case eObjectType.MinionBoss:
                        {
                            SlotFixObject_Minion pMinion = pSlotFixObject as SlotFixObject_Minion;

                            if (pMinion != null)
                            {
                                switch (pMinion.GetMinionType())
                                {
                                    case eMinionType.EnemyMinion:
                                        {
                                            m_pDataStack.m_EnemyMinionTable.Remove(pMinion.GetSlot().GetSlotIndex());
                                        }
                                        break;

                                    case eMinionType.EnemySummonUnit:
                                        {
                                            m_pDataStack.m_EnemySummonUnitTable.Remove(pMinion.GetSlot().GetSlotIndex());
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    case eObjectType.EnemyBoss:
                        {
                        }
                        break;
                    default:
                        {
                        }
                        break;
                }

                // 죽이자
                pSlotFixObject.OnDead();
            }
			else
			{
				pSlotFixObject.GetSlot().SetSlotDying(false);
			}
		}
    }

    public class DamageSlotInfo
    {
        public Slot m_pSlot = null;
        public eObjectType m_eObjectType = eObjectType.Block;
        public SlotFixObject m_SlotFixObject = null;
        public int m_nLayer = 0;
    }

    public List<DamageSlotInfo> GetDamageSlotList_byLine(int nX, int nY_ToTopping)
    {
        List<DamageSlotInfo> damageSlotList = new List<DamageSlotInfo>();

        for (int y = GameDefine.ms_nInGameSlot_Y - 1; y >= nY_ToTopping + 1; --y)
        {
            if (m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nX, y)) == true)
            {
                bool IsTarget = false;
                Slot pSlot = m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nX, y)];
                SlotFixObject_Obstacle pSlotFixObject_Obstacle = pSlot.GetLastSlotFixObject() as SlotFixObject_Obstacle;

                if (pSlot.IsSlotDying() == false && pSlotFixObject_Obstacle == null)
                {
                    IsTarget = true;
                }
                else if (pSlot.IsSlotDying() == true && pSlotFixObject_Obstacle != null)
                {
                    int nLayer = pSlotFixObject_Obstacle.GetLayer();

                    if (nLayer > 1)
                    {
                        pSlot.OnSlotDying();
                        pSlot.SetSlotDying(false);
                        IsTarget = true;
                    }
                }
                else if (pSlot.IsSlotDying() == false)
                {
                    IsTarget = true;
                }

                if (IsTarget == true)
                {
                    if (pSlot.GetSlotBlock() != null)
                    {
                        if (pSlot.GetSlotBlock().GetBlockType() == eBlockType.Custom && pSlot.GetSlotBlock().GetMapSlotItem() == eMapSlotItem.Tire)
                        {
                            DamageSlotInfo pDamageSlotInfo = new DamageSlotInfo();
                            pDamageSlotInfo.m_pSlot = pSlot;
                            pDamageSlotInfo.m_eObjectType = eObjectType.BlockObstacle;
                            damageSlotList.Add(pDamageSlotInfo);
                        }
                        else
                        {
                            SlotFixObject pSlotFixObject = pSlot.GetLastSlotFixObject();

                            if (pSlotFixObject != null)
                            {
                                switch (pSlotFixObject.GetSlotFixObjectType())
                                {
                                    case eSlotFixObjectType.Obstacle:
                                        {
                                            SlotFixObject_Obstacle pObstacle = pSlotFixObject as SlotFixObject_Obstacle;

                                            if (pObstacle != null)
                                            {
                                                switch (pObstacle.GetObstacleType())
                                                {
                                                    case eSlotFixObjectObstacleType.Ice:
                                                        {
                                                            SlotFixObject_Ice pIce = pObstacle as SlotFixObject_Ice;

                                                            DamageSlotInfo pDamageSlotInfo = new DamageSlotInfo();
                                                            pDamageSlotInfo.m_pSlot = pSlot;
                                                            pDamageSlotInfo.m_eObjectType = eObjectType.SlotObstacle;
                                                            pDamageSlotInfo.m_SlotFixObject = pSlotFixObject;
                                                            pDamageSlotInfo.m_nLayer = pIce.GetLayer();
                                                            damageSlotList.Add(pDamageSlotInfo);
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        List<SlotFixObject> slotFixObjectList = pSlot.GetSlotFixObjectList();
                        foreach (SlotFixObject pSlotFixObject in slotFixObjectList)
                        {
                            switch (pSlotFixObject.GetSlotFixObjectType())
                            {
                                case eSlotFixObjectType.Obstacle:
                                    {
                                        SlotFixObject_Obstacle pObstacle = pSlotFixObject as SlotFixObject_Obstacle;

                                        if (pObstacle != null)
                                        {
                                            switch (pObstacle.GetObstacleType())
                                            {
                                                case eSlotFixObjectObstacleType.Box:
                                                    {
                                                        SlotFixObject_Box pBox = pObstacle as SlotFixObject_Box;

                                                        DamageSlotInfo pDamageSlotInfo = new DamageSlotInfo();
                                                        pDamageSlotInfo.m_pSlot = pSlot;
                                                        pDamageSlotInfo.m_eObjectType = eObjectType.SlotObstacle;
                                                        pDamageSlotInfo.m_SlotFixObject = pSlotFixObject;
                                                        pDamageSlotInfo.m_nLayer = pBox.GetLayer();
                                                        damageSlotList.Add(pDamageSlotInfo);
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    break;

                                case eSlotFixObjectType.Espresso:
                                    {
                                        SlotFixObject_Espresso pEspresso = pSlotFixObject as SlotFixObject_Espresso;

                                        if (pEspresso != null && pEspresso.GetOwner() == eOwner.Other &&
                                            (pEspresso.GetObjectType() == eObjectType.EnemyColony || pEspresso.GetObjectType() == eObjectType.Minion ||
                                            pEspresso.GetObjectType() == eObjectType.MinionBoss || pEspresso.GetObjectType() == eObjectType.EnemyBoss))
                                        {
                                            DamageSlotInfo pDamageSlotInfo = new DamageSlotInfo();
                                            pDamageSlotInfo.m_pSlot = pSlot;
                                            pDamageSlotInfo.m_eObjectType = pEspresso.GetObjectType();
                                            pDamageSlotInfo.m_SlotFixObject = pSlotFixObject;
                                            damageSlotList.Add(pDamageSlotInfo);
                                        }
                                    }
                                    break;
                            }

                        }
                    }
                }
            }
        }

        return damageSlotList;
    }

    private void ReCalculationMatchDamage()
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        for (int i = 0; i < (int)eElement.Max; ++i)
        {
            pDataStack.m_nPlayerCharacterAttackValue[i] = 0;
        }

        foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in pDataStack.m_PlayerCharacterTable)
        {
            pDataStack.m_nPlayerCharacterAttackValue[(int)item.Value.GetUnitInfo().m_eElement] += item.Value.GetATK();
        }
    }

    public override void OnDoneCheckRemoveSlot() 
    {
        ReCalculationMatchDamage();

        OutputLog.Log("MainGame_Espresso_PVE : OnDoneCheckRemoveSlot Begin");

        // EnemyColony Destroy
        foreach (Dictionary<int, Slot> matchSlotTable in m_MatchSlotTableList)
        {
            List<Slot> matchNotSlotBlockList = new List<Slot>();
            Slot pSlot_Special = null;

            foreach (KeyValuePair<int, Slot> item in matchSlotTable)
            {
                if (item.Value.GetSlotBlock() != null && item.Value.IsSlotFixObject_Obstacle() == false)
                {
                    Slot pNeighbor;

                    pNeighbor = item.Value.GetNeighborSlot(eNeighbor.Neighbor_10);
                    if (pNeighbor != null && pNeighbor.GetSlotBlock() == null)
                    {
                        OnCheckNeighborEnemyColony(pNeighbor);
                    }
                    pNeighbor = item.Value.GetNeighborSlot(eNeighbor.Neighbor_01);
                    if (pNeighbor != null && pNeighbor.GetSlotBlock() == null)
                    {
                        OnCheckNeighborEnemyColony(pNeighbor);
                    }
                    pNeighbor = item.Value.GetNeighborSlot(eNeighbor.Neighbor_21);
                    if (pNeighbor != null && pNeighbor.GetSlotBlock() == null)
                    {
                        OnCheckNeighborEnemyColony(pNeighbor);
                    }
                    pNeighbor = item.Value.GetNeighborSlot(eNeighbor.Neighbor_12);
                    if (pNeighbor != null && pNeighbor.GetSlotBlock() == null)
                    {
                        OnCheckNeighborEnemyColony(pNeighbor);
                    }
                }
                else if(item.Value.GetSlotBlock() == null && item.Value.IsSlotFixObject_Obstacle() == false)
                {
                    matchNotSlotBlockList.Add(item.Value);
                }

                if (item.Value.GetSlotBlock() != null && item.Value.GetSlotBlock().GetSpecialItem() != eSpecialItem.None)
                {
                    pSlot_Special = item.Value;
                }
            }

            if (pSlot_Special != null && matchNotSlotBlockList.Count != 0)
            {
                foreach (Slot pSlot in matchNotSlotBlockList)
                {
                    SlotFixObject_Espresso pSlotFixObject = pSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;
                    if (pSlotFixObject != null && pSlotFixObject.GetOwner() != eOwner.My)
                    {
                        OnSpecialBlockDyingToFixObject(pSlotFixObject, (eElement)pSlot_Special.GetSlotBlock().GetBlockType());
                    }
                }
            }
        }

        m_MatchSlotTableList.Clear();

        // 오버킬 계산 및 공격
        for (int x = 0; x < GameDefine.ms_nInGameSlot_X; ++x)
        {
            List<Slot> slotList = m_MatchSlotTable[x];
            slotList.Sort(CompareMatchSlotList);

            int nLastGroup = -1;

            DamageSlotInfo pDamageSlotInfo = null;
            Slot pDamageSlot = null;

            for(int i = 0; i < slotList.Count; ++i)
            {
                Slot pSlot = slotList[i];

                if (pSlot.GetSlotBlock() != null && SlotManager.IsCombinationBlock(pSlot) == true && pSlot.IsSlotFixObject_Obstacle() == false)
                {
                    Slot pNextSlot = null;
                    if (slotList.Count > i + 1)
                        pNextSlot = slotList[i + 1];

                    eElement eBlockElement = (eElement)pSlot.GetSlotBlock().GetBlockType();

                    List<DamageSlotInfo> damageSlotInfoList = GetDamageSlotList_byLine(x, pSlot.GetY());
                    damageSlotInfoList.Reverse();

                    SlotBlockParameta pParameta = pSlot.GetSlotBlock().GetParameta() as SlotBlockParameta;

                    if (pParameta != null)
                    {
                        int nGruopIndex = pParameta.m_nMatchGroup;

                        if (nGruopIndex != nLastGroup)      // 전 슬롯 공격과 같은 그룹이 아니라면 ( 새로운 그룹 )
                        {
                            if (pDamageSlotInfo != null)
                            {
                                if (pDamageSlotInfo.m_eObjectType == eObjectType.BlockObstacle)
                                {
                                    damageSlotInfoList.Remove(pDamageSlotInfo);
                                }
                                else
                                {
                                    switch (pDamageSlotInfo.m_SlotFixObject.GetSlotFixObjectType())
                                    {
                                        case eSlotFixObjectType.Obstacle:
                                            {
                                                SlotFixObject_Obstacle pObstacle = pDamageSlotInfo.m_SlotFixObject as SlotFixObject_Obstacle;

                                                if (pObstacle != null)
                                                {
                                                    switch (pObstacle.GetObstacleType())
                                                    {
                                                        case eSlotFixObjectObstacleType.Box:
                                                            {
                                                                if (--pDamageSlotInfo.m_nLayer == 0)
                                                                {
                                                                    damageSlotInfoList.Remove(pDamageSlotInfo);
                                                                }
                                                            }
                                                            break;

                                                        case eSlotFixObjectObstacleType.Ice:
                                                            {
                                                                if (--pDamageSlotInfo.m_nLayer == 0)
                                                                {
                                                                    damageSlotInfoList.Remove(pDamageSlotInfo);
                                                                }
                                                            }
                                                            break;
                                                    }
                                                }
                                            }
                                            break;

                                        case eSlotFixObjectType.Espresso:
                                            {
                                                SlotFixObject_Espresso pEspresso = pDamageSlotInfo.m_SlotFixObject as SlotFixObject_Espresso;

                                                if (pEspresso != null)
                                                {
                                                    switch (pEspresso.GetObjectType())
                                                    {
                                                        case eObjectType.EnemyColony:
                                                            {
                                                                damageSlotInfoList.Remove(pDamageSlotInfo);
                                                            }
                                                            break;

                                                        case eObjectType.Minion:
                                                        case eObjectType.MinionBoss:
                                                            {
                                                                SlotFixObject_Minion pMinion = pEspresso as SlotFixObject_Minion;
                                                                if (pMinion.IsVirtualDying() == true)
                                                                {
                                                                    damageSlotInfoList.Remove(pDamageSlotInfo);
                                                                }
                                                            }
                                                            break;

                                                        case eObjectType.EnemyBoss:
                                                            {
                                                            }
                                                            break;
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                            }

                            if (damageSlotInfoList.Count != 0)
                                pDamageSlotInfo = damageSlotInfoList[0];
                        }
                        else
                        {
                            if (damageSlotInfoList.Count != 0)
                            {
                                if (pDamageSlotInfo != damageSlotInfoList[0])
                                {
                                    pDamageSlotInfo = damageSlotInfoList[0];
                                }
                            }
                        }

                        if (pDamageSlotInfo != null)
                        {
                            pDamageSlot = pDamageSlotInfo.m_pSlot;

                            bool IsDeadProjectile = false;

                            if (pDamageSlotInfo.m_eObjectType == eObjectType.BlockObstacle)        // 블록이라면
                            {
                                if (pNextSlot == null)
                                {
                                    IsDeadProjectile = true;
                                    ++m_pSlotManager.m_nTurnCompleteCheck;
                                    OutputLog.Log("++pDataStack.m_pSlotManager.m_nTurnCompleteCheck > MainGame_Espresso_PVE : OnDoneCheckRemoveSlot : BlockObstacle");
                                }
                            }
                            else                                                        // 블록이 아니라면
                            {
                                switch (pDamageSlotInfo.m_SlotFixObject.GetSlotFixObjectType())
                                {
                                    case eSlotFixObjectType.Obstacle:
                                        {
                                            SlotFixObject_Obstacle pObstacle = pDamageSlotInfo.m_SlotFixObject as SlotFixObject_Obstacle;

                                            if (pObstacle != null)
                                            {
                                                switch (pObstacle.GetObstacleType())
                                                {
                                                    case eSlotFixObjectObstacleType.Box:
                                                        {
                                                            if (pNextSlot == null)
                                                            {
                                                                IsDeadProjectile = true;
                                                                ++m_pSlotManager.m_nTurnCompleteCheck;
                                                                OutputLog.Log("++pDataStack.m_pSlotManager.m_nTurnCompleteCheck > MainGame_Espresso_PVE : OnDoneCheckRemoveSlot : Box");
                                                            }
                                                        }
                                                        break;

                                                    case eSlotFixObjectObstacleType.Ice:
                                                        {
                                                            if (pNextSlot == null)
                                                            {
                                                                IsDeadProjectile = true;
                                                                ++m_pSlotManager.m_nTurnCompleteCheck;
                                                                OutputLog.Log("++pDataStack.m_pSlotManager.m_nTurnCompleteCheck > MainGame_Espresso_PVE : OnDoneCheckRemoveSlot : Box");
                                                            }
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                        break;

                                    case eSlotFixObjectType.Espresso:
                                        {
                                            SlotFixObject_Espresso pEspresso = pDamageSlotInfo.m_SlotFixObject as SlotFixObject_Espresso;

                                            if (pEspresso != null)
                                            {
                                                switch (pEspresso.GetObjectType())
                                                {
                                                    case eObjectType.EnemyColony:
                                                        {
                                                            int nHP = pEspresso.GetVirtualHP();
                                                            pEspresso.ChangeVirtualHP(nHP - 1);

                                                            if (pNextSlot == null && pEspresso.IsVirtualDying() == true)
                                                            {
                                                                IsDeadProjectile = true;
                                                                ++m_pSlotManager.m_nTurnCompleteCheck;
                                                                OutputLog.Log("++pDataStack.m_pSlotManager.m_nTurnCompleteCheck > MainGame_Espresso_PVE : OnDoneCheckRemoveSlot : EnemyColony");
                                                            }
                                                        }
                                                        break;

                                                    case eObjectType.Minion:
                                                    case eObjectType.MinionBoss:
                                                        {
                                                            SlotFixObject_Minion pMinion = pEspresso as SlotFixObject_Minion;
                                                            int nHP = pMinion.GetVirtualHP();
                                                            float fCorrelationRate = ExcelDataManager.Instance.m_pElement_Correlation.GetCorrelationRate(eBlockElement, pMinion.GetUnitInfo().m_eElement);

                                                            fCorrelationRate -= pMinion.GetAdd_ElementResist(eBlockElement);
                                                            if (fCorrelationRate < 0) fCorrelationRate = 0;

                                                            float fMatchDamage = (m_pDataStack.m_nPlayerCharacterAttackValue[(int)eBlockElement] / 3) * (fCorrelationRate / 100);
                                                            int nMatchDamage = (int)Math.Round(fMatchDamage, MidpointRounding.AwayFromZero);
                                                            nMatchDamage = nMatchDamage > 1 ? nMatchDamage : 1;
                                                            nMatchDamage *= (int)(ExcelDataManager.Instance.m_pMatchDamage_ComboBonus.GetBonusRate(pParameta.m_nCombo) / 100);
                                                            pMinion.ChangeVirtualHP(nHP - nMatchDamage);

                                                            if (pMinion.IsVirtualDying() == true)
                                                            {
                                                                if (pNextSlot == null)
                                                                {
                                                                    IsDeadProjectile = true;
                                                                    ++m_pSlotManager.m_nTurnCompleteCheck;
                                                                    OutputLog.Log("++pDataStack.m_pSlotManager.m_nTurnCompleteCheck > MainGame_Espresso_PVE : OnDoneCheckRemoveSlot : Minion");
                                                                }
                                                            }
                                                        }
                                                        break;

                                                    case eObjectType.EnemyBoss:
                                                        {
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                        break;
                                }
                            }

                            GameEvent_BlockMatchMissile pGameEvent = new GameEvent_BlockMatchMissile(pSlot, pDamageSlotInfo.m_pSlot, pDamageSlotInfo.m_eObjectType, eBlockElement, pParameta.m_nCombo, IsDeadProjectile);
                            GameEventManager.Instance.AddGameEvent(pGameEvent);

                            EventDelegateManager.Instance.OnInGame_CreateBlockMatchMissile(eBlockElement, eOwner.My);
                        }
                        else
                        {
                            int nY = m_pSlotManager.GetLastSlotY(pSlot.GetX());
                            Slot pSlot_Target = m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(pSlot.GetX(), nY)];
                            GameEvent_BlockMatchMissile pGameEvent = new GameEvent_BlockMatchMissile(pSlot, pSlot_Target, eBlockElement);
                            GameEventManager.Instance.AddGameEvent(pGameEvent);

                            EventDelegateManager.Instance.OnInGame_CreateBlockMatchMissile(eBlockElement, eOwner.My);
                        }

                        nLastGroup = nGruopIndex;
                    }
                }
            }
        }

        m_nMatchGroupIndex = 0;
        for (int x = 0; x < GameDefine.ms_nInGameSlot_X; ++x)
        {
            m_MatchSlotTable[x].Clear();
        }

        Helper.OnCameraBigShaking();

        Helper.SetVibrate_byHapticType(HapticTypes.LightImpact);

        EventDelegateManager.Instance.OnInGame_SlotMoveAndCreate(GameDefine.ms_fSlotMoveAndCreate_Delay);

        OutputLog.Log("MainGame_Espresso_PVE : OnDoneCheckRemoveSlot End");
    }

    private void OnInGame_Projectile_Block_Done()
    {
        OutputLog.Log("MainGame_Espresso_PVE : OnInGame_Projectile_Block_Done Begin");

        m_MatchActionComplete[(int)eMatchActionComplete.MatchProjectileComplete] = true;
        CheckMatchComplete();

        OutputLog.Log("MainGame_Espresso_PVE : OnInGame_Projectile_Block_Done End");
    }

    public override void OnMatchTurnComplete() 
    {
        OutputLog.Log("MainGame_Espresso_PVE : OnMatchTurnComplete Begin");

        if (InGameInfo.Instance.m_IsInGameStart == true)
        {
            m_MatchActionComplete[(int)eMatchActionComplete.MatchTurnComplete] = true;
            CheckMatchComplete();

            if (EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate == true &&
                (m_MatchActionComplete[(int)eMatchActionComplete.MatchProjectileComplete] = true || m_pDataStack.m_nBlockProjectileCount == 0))
            {
                OutputLog.Log("MainGame_Espresso_PVE : OnMatchTurnComplete EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate = false");

                EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate = false;
            }

            if(m_pDataStack.m_nMaxCombo < m_pDataStack.m_nCombo)
            {
                m_pDataStack.m_nMaxCombo = m_pDataStack.m_nCombo;
            }

            if (InGameInfo.Instance.m_eCurrGameResult == eGameResult.None)
            {
                m_pDataStack.m_nCombo = 0;
            }
        }

        OutputLog.Log("MainGame_Espresso_PVE : OnMatchTurnComplete End");
    }

    private void CheckMatchComplete()
    {
        OutputLog.Log("MainGame_Espresso_PVE : CheckMatchComplete Begin");

        if (InGameInfo.Instance.m_IsInGameStart == true)
        {
            bool IsComplete = true;
            for (int i = 0; i < (int)eMatchActionComplete.Max; ++i)
            {
                if (m_MatchActionComplete[i] == false)
                {
                    IsComplete = false;
                }
            }

            if (IsComplete == true)
            {
                OutputLog.Log("MainGame_Espresso_PVE : CheckMatchComplete IsComplete == true");

                for (int i = 0; i < (int)eMatchActionComplete.Max; ++i)
                {
                    m_MatchActionComplete[i] = false;
                }

                if (EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate == false)
                {
                    OutputLog.Log("MainGame_Espresso_PVE : CheckMatchComplete EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate == false");

                    m_pTimer_EnemyTurn.OnReset();
                    TransformerEvent_Timer eventValue;
                    eventValue = new TransformerEvent_Timer(0.5f);
                    m_pTimer_EnemyTurn.AddEvent(eventValue);
                    m_pTimer_EnemyTurn.SetCallback(null, OnDone_Timer_MatchTurnComplete);
                    m_pTimer_EnemyTurn.OnPlay();
                }
                else
                {
                    OutputLog.Log("MainGame_Espresso_PVE : CheckMatchComplete EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate = false;");

                    EspressoInfo.Instance.m_IsSkillAttack_ForMoveAndCreate = false;
                }
            }
        }

        OutputLog.Log("MainGame_Espresso_PVE : CheckMatchComplete End");
    }

    private void OnDone_Timer_MatchTurnComplete(TransformerEvent eventValue)
    {
        EventDelegateManager.Instance.OnInGame_MatchTurnComplete();
    }

    public void OnInGame_StartActionTurnDone()
    {
        m_pTurnStatePattern.OnStartBoosterTurn();
    }

    public void OnInGame_StartBoosterTurnDone()
    {
        EventDelegateManager.Instance.OnInGame_ShowPlayerTurnInfo_FirstTime();

        m_pTurnStatePattern.OnPlayerTurn();
    }

    public void OnInGame_StartAnimationDone()
    {
        m_pTurnStatePattern.OnPlayerTurn();
    }

    public void OnInGame_PlayerTurnDone()
    {
        if (m_pDataStack.m_nCurrShildPoint > 0 && m_pDataStack.m_nCurrObjectiveCount <= 0 && m_pDataStack.m_EnemyMinionTable.Count == 0)
        {
            EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
        }
        else
        {
            SetColor_RndSlotBlock(new Color(210.0f / 255.0f, 210.0f / 255.0f, 210.0f / 255.0f, 1.0f), 0.0f, 0.2f);

            EventDelegateManager.Instance.OnInGame_ShowTurnInfo(false);
        }
    }

    public void OnInGame_EnemyTurnDone()
    {
        if (m_pDataStack.m_nCurrShildPoint == 0)
        {
            EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Fail);
        }
        else
        {
            EventDelegateManager.Instance.OnInGame_ShowTurnInfo(true);

            //m_pTurnStatePattern.OnPlayerTurn();
        }
    }

    public void OnInGame_ChangeTurn(bool IsMyTurn)
    {
        if (!IsMyTurn)
        {
            m_pTurnStatePattern.OnEnemyTurn();
        }
        else
        {
            m_pTurnStatePattern.OnPlayerTurn();
        }
    }

    public void OnInGame_GameOver(eGameResult eResult)
    {
        if (InGameInfo.Instance.m_eCurrGameResult == eGameResult.None)
        {
            InGameInfo.Instance.m_eCurrGameResult = eResult;

            GameObject ob = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGameResult");
            ob = GameObject.Instantiate(ob);
            InGame_Result_UI pScript = ob.GetComponent<InGame_Result_UI>();
            pScript.Init(eResult);

            switch (eResult)
            {
                case eGameResult.Success:
                    Helper.OnSoundPlay("INGAME_JINGLE_VICTORY", false);
                    break;
                case eGameResult.Fail: Helper.OnSoundPlay("INGAME_JINGLE_DEFEAT", false); break;
            }

            SoundPlayer.Instance.StopBGM();

            EventDelegateManager.Instance.OnInGame_UnitDetail_Close();
            EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Close();
        }
    }

    private GameObject OnInGame_GetInGameUIGameObject()
    {
        return m_pIngameUI;
    }
}
