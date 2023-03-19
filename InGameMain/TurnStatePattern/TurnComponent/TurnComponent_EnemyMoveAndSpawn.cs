using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PathFind;

public class TurnComponent_EnemyMoveAndSpawn : TComponent<EventArg_Null, EventArg_Null>
{
    private MainGame_DataStack m_pDataStack = null;
    private Dictionary<SlotFixObject_Minion, List<Slot>> m_MinionPathTable = new Dictionary<SlotFixObject_Minion, List<Slot>>();
    private List<Slot> m_CurrTurnMoveAndSpawnTargetList = new List<Slot>();

    private int m_nMoveAndCreateCount = 0;

    private Dictionary<Slot, ExcelData_Stage_EnemySpawnInfo> m_pSpawnPointAtEnemySpawnTable = new Dictionary<Slot, ExcelData_Stage_EnemySpawnInfo>();
    private Transformer_Timer m_pTimer_SpawnPointAtEnemy_CreateEnemyColony = new Transformer_Timer();
    private Transformer_Timer m_pTimer_SpawnPointAtEnemy_CreateEnemyDelay = new Transformer_Timer();

    private Dictionary<SlotFixObject_Minion, SlotFixObject_PlayerCharacter> m_ShieldDecreaseMinionTable = new Dictionary<SlotFixObject_Minion, SlotFixObject_PlayerCharacter>();

    private bool m_IsSpawnPointAtEnemy_ExistColonySpawn = false;
    private bool m_IsSpawnPointAtEnemy_CreateEnemyDelay = false;

    public TurnComponent_EnemyMoveAndSpawn()
    {
        m_pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        TransformerEvent eventValue;
        eventValue = new TransformerEvent_Timer(0.3f * GameDefine.ms_fEnemySlotMoveMultiple);
        m_pTimer_SpawnPointAtEnemy_CreateEnemyColony.AddEvent(eventValue);
        m_pTimer_SpawnPointAtEnemy_CreateEnemyColony.SetCallback(null, OnDone_Timer_SpawnPointAtEnemy_CreateEnemyColony);

        eventValue = new TransformerEvent_Timer(GameDefine.ms_fEnemyOneBlockMoveTime + 1.5f * GameDefine.ms_fEnemySlotMoveMultiple);
        m_pTimer_SpawnPointAtEnemy_CreateEnemyDelay.AddEvent(eventValue);
        m_pTimer_SpawnPointAtEnemy_CreateEnemyDelay.SetCallback(null, OnDone_Timer_SpawnPointAtEnemy_CreateEnemyDelay);

        AppInstance.Instance.m_pEventDelegateManager.OnEventInGame_EnemyMinionMoveAndCreateDone += OnInGame_EnemyMinionMoveAndCreateDone;
    }

    public override void OnDestroy()
    {
        AppInstance.Instance.m_pEventDelegateManager.OnEventInGame_EnemyMinionMoveAndCreateDone -= OnInGame_EnemyMinionMoveAndCreateDone;
    }

    public override void Update()
    {
        m_pTimer_SpawnPointAtEnemy_CreateEnemyColony.Update(Time.deltaTime);
        m_pTimer_SpawnPointAtEnemy_CreateEnemyDelay.Update(Time.deltaTime);
    }

    private int CompareMoveRangeInfoList(ExcelData_Unit_MoveRangeInfo A, ExcelData_Unit_MoveRangeInfo B)
    {
        return A.m_nPosY.CompareTo(B.m_nPosY);
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        OutputLog.Log("TurnComponent_EnemyMoveAndSpawn : OnEvent");
        InGameTurnLog.Log("TurnComponent_EnemyMoveAndSpawn : OnEvent");

        m_IsSpawnPointAtEnemy_ExistColonySpawn = false;
        m_IsSpawnPointAtEnemy_CreateEnemyDelay = false;

        InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.EnemyMoveAndSpawn;
        EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.EnemyMoveAndSpawn);
        ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);

        m_CurrTurnMoveAndSpawnTargetList.Clear();
        m_MinionPathTable.Clear();
        m_nMoveAndCreateCount = 0;
        m_ShieldDecreaseMinionTable.Clear();
        m_pSpawnPointAtEnemySpawnTable.Clear();

        bool[,] Map = new bool[GameDefine.ms_nInGameSlot_X, GameDefine.ms_nInGameSlot_Y];

        for (int y = 0; y < GameDefine.ms_nInGameSlot_Y; ++y)
        {
            for (int x = 0; x < GameDefine.ms_nInGameSlot_X; ++x)
            {
                int nSlotIndex = Helper.GetSlotIndex(x, y);

                if (m_pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(nSlotIndex) == true)
                {
                    Slot pSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[nSlotIndex];

                    if (pSlot.GetSlotFixObjectList().Count == 1)
                    {
                        SlotFixObject_Espresso pSlotFixObject_Espresso = pSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                        if (pSlotFixObject_Espresso != null)
                        {
                            switch (pSlotFixObject_Espresso.GetObjectType())
                            {
                                case eObjectType.EnemyColony:
                                    {
                                        Map[x, y] = true;
                                    }
                                    break;
                                case eObjectType.Minion:
                                case eObjectType.MinionBoss:
                                    {
                                        Map[x, y] = false;
                                    }
                                    break;
                                case eObjectType.Character:
                                    {
                                        SlotFixObject_PlayerCharacter pPlayerCharactger = pSlotFixObject_Espresso as SlotFixObject_PlayerCharacter;
                                        if (pPlayerCharactger != null)
                                        {
                                            if (pPlayerCharactger.GetHP() > 0)
                                            {
                                                Map[x, y] = false;
                                            }
                                            else
                                            {
                                                Map[x, y] = true;
                                            }
                                        }
                                        else
                                        {
                                            Map[x, y] = true;
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        Map[x, y] = true;
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            SlotFixObject_Obstacle pSlotFixObject_Obstacle = pSlot.GetLastSlotFixObject() as SlotFixObject_Obstacle;

                            if (pSlotFixObject_Obstacle == null)
                            {
                                Map[x, y] = true;
                            }
                            else
                            {
                                Map[x, y] = false;
                            }
                        }
                    }
                    else
                    {
                        Map[x, y] = false;
                    }
                }
                else
                {
                    Map[x, y] = false;
                }
            }
        }

        if (m_pDataStack.m_EnemyMinionTable.Count != 0 || m_pDataStack.m_EnemySummonUnitTable.Count != 0)
        {
            Dictionary<int, SlotFixObject_Minion> dataTable = new Dictionary<int, SlotFixObject_Minion>();

            foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pDataStack.m_EnemyMinionTable)
            {
                dataTable.Add(item.Key, item.Value);
            }

            foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pDataStack.m_EnemySummonUnitTable)
            {
                dataTable.Add(item.Key, item.Value);
            }

            dataTable = dataTable.OrderBy(item => item.Key).ToDictionary(x => x.Key, x => x.Value);

            foreach (KeyValuePair<int, SlotFixObject_Minion> item in dataTable)
            {
                ExcelData_UnitInfo pUnitInfo = item.Value.GetUnitInfo();

                if (pUnitInfo != null && pUnitInfo.m_eMoveType != eMoveType.Fixed && item.Value.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.MoveHold) == false)
                {
                    if (pUnitInfo.m_eMoveType == eMoveType.Path)
                    {
                        int nStartX = item.Value.GetSlot().GetX();
                        int nStartY = item.Value.GetSlot().GetY();

                        List<ExcelData_Unit_MoveRangeInfo> MoveRangeInfoList = ExcelDataManager.Instance.m_pUnit_MoveRange.GetMoveRangeInfoList(pUnitInfo.m_nMoveRangeID);

                        if (MoveRangeInfoList.Count != 0)
                        {
                            Helper.ShuffleList<ExcelData_Unit_MoveRangeInfo>(MoveRangeInfoList);
                            MoveRangeInfoList.Sort(CompareMoveRangeInfoList);

                            foreach (ExcelData_Unit_MoveRangeInfo pMoveRangeInfo in MoveRangeInfoList)
                            {
                                int nX = nStartX + pMoveRangeInfo.m_nPosX;
                                int nY = nStartY + pMoveRangeInfo.m_nPosY;

                                if (SlotManager.IsValidSlotIndex(nX, nY) == true)
                                {
                                    PathFind.Grid pGrid = new PathFind.Grid(GameDefine.ms_nInGameSlot_X, GameDefine.ms_nInGameSlot_Y, Map);
                                    List<Point> pointList = Pathfinding.FindPath(pGrid, new Point(nStartX, nStartY), new Point(nX, nY));

                                    if (pointList.Count != 0)
                                    {
                                        Map[nStartX, nStartY] = true;
                                        Map[nX, nY] = false;

                                        Slot pSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nX, nY)];
                                        SlotFixObject_PlayerCharacter pPlayerCharacter = pSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                        if (pPlayerCharacter != null)
                                        {
                                            if (pPlayerCharacter.GetHP() > 0)
                                            {
                                                ++m_nMoveAndCreateCount;
                                                GameEvent_EnemyMinionMove pGameEvent = new GameEvent_EnemyMinionMove(item.Value, pointList, false);
                                                AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
                                            }
                                            else
                                            {
                                                ++m_nMoveAndCreateCount;
                                                GameEvent_EnemyMinionMove pGameEvent = new GameEvent_EnemyMinionMove(item.Value, pointList, true);
                                                AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
                                            }
                                        }
                                        else
                                        {
                                            Point pLastPoint = pointList[pointList.Count - 1];

                                            ++m_nMoveAndCreateCount;
                                            GameEvent_EnemyMinionMove pGameEvent = new GameEvent_EnemyMinionMove(item.Value, pointList, pLastPoint.y == 0 ? true : false);
                                            AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
                                        }

                                        Helper.OnSoundPlay("INGAME_MINION_MOVE", false);

                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (pUnitInfo.m_eMoveType == eMoveType.Warp)
                    {
                        int nStartX = item.Value.GetSlot().GetX();
                        int nStartY = item.Value.GetSlot().GetY();

                        List<ExcelData_Unit_MoveRangeInfo> MoveRangeInfoList = ExcelDataManager.Instance.m_pUnit_MoveRange.GetMoveRangeInfoList(pUnitInfo.m_nMoveRangeID);

                        if (MoveRangeInfoList.Count != 0)
                        {
                            Helper.ShuffleList<ExcelData_Unit_MoveRangeInfo>(MoveRangeInfoList);
                            MoveRangeInfoList.Sort(CompareMoveRangeInfoList);

                            foreach (ExcelData_Unit_MoveRangeInfo pMoveRangeInfo in MoveRangeInfoList)
                            {
                                int nX = nStartX + pMoveRangeInfo.m_nPosX;
                                int nY = nStartY + pMoveRangeInfo.m_nPosY;

                                if (SlotManager.IsValidSlotIndex(nX, nY) == true && Map[nX, nY] == true)
                                {
                                    Map[nStartX, nStartY] = true;
                                    Map[nX, nY] = false;

                                    Slot pSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nX, nY)];
                                    SlotFixObject_PlayerCharacter pPlayerCharacter = pSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                    if (pPlayerCharacter != null)
                                    {
                                        if (pPlayerCharacter.GetHP() > 0)
                                        {
                                            ++m_nMoveAndCreateCount;
                                            GameEvent_EnemyMinionWarp pGameEvent = new GameEvent_EnemyMinionWarp(item.Value, new PathFind.Point(nX, nY), false);
                                            AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
                                        }
                                        else
                                        {
                                            ++m_nMoveAndCreateCount;
                                            GameEvent_EnemyMinionWarp pGameEvent = new GameEvent_EnemyMinionWarp(item.Value, new PathFind.Point(nX, nY), true);
                                            AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
                                        }
                                    }
                                    else
                                    {
                                        ++m_nMoveAndCreateCount;
                                        GameEvent_EnemyMinionWarp pGameEvent = new GameEvent_EnemyMinionWarp(item.Value, new PathFind.Point(nX, nY), nY == 0 ? true : false);
                                        AppInstance.Instance.m_pGameEventManager.AddGameEvent(pGameEvent);
                                    }

                                    Helper.OnSoundPlay("INGAME_MINION_MOVE", false);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        bool IsEmptySpawnPoint = true;
        List<Slot> spawnPointSlotList = new List<Slot>();

        List<InGameStageMapSlotData> spawnStageMapSlotDataList = m_pDataStack.m_pInGameStageMapData.GetInGameStageMapSlotData(eMapSlotItem.EnemySpawn);

        foreach (InGameStageMapSlotData pInGameStageMapSlotData in spawnStageMapSlotDataList)
        {
            Vector2Int vIndex = ExcelDataHelper.GetSlotConverting(pInGameStageMapSlotData.m_nX, pInGameStageMapSlotData.m_nY);
            int nSlotIndex = Helper.GetSlotIndex(vIndex.x, vIndex.y);
            if (m_pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(nSlotIndex) == true)
            {
                Slot pSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[nSlotIndex];
                spawnPointSlotList.Add(pSlot);

                SlotFixObject_Espresso pSlotFixObject_Espresso = pSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                if (pSlot.GetSlotBlock() == null && pSlotFixObject_Espresso != null)
                {
                    IsEmptySpawnPoint = false;
                }
            }
        }

        Helper.ShuffleList<Slot>(spawnPointSlotList);

        List<ExcelData_Stage_EnemySpawnInfo> ResetIntervalTurnList = new List<ExcelData_Stage_EnemySpawnInfo>();
        List<ExcelData_Stage_EnemySpawnInfo> EnemySpawnInfoList = ExcelDataManager.Instance.m_pStage_EnemySpawn.GetEnemySpawnInfoList(EspressoInfo.Instance.m_nStageID);
        int nPossibleCreateCount = m_pDataStack.m_pStageInfo.m_nEnemySpawnMax - m_pDataStack.m_EnemyMinionTable.Count;

        if (IsEmptySpawnPoint == false)
        {
            foreach (Slot pSlot in spawnPointSlotList)
            {
                if (nPossibleCreateCount > 0)
                {
                    if (pSlot.GetSlotBlock() == null && pSlot.GetSlotFixObjectList().Count == 1)
                    {
                        SlotFixObject_EnemyColony pEnemyColony = pSlot.GetSlotFixObjectList()[0] as SlotFixObject_EnemyColony;

                        if (pEnemyColony != null)
                        {
                            foreach (ExcelData_Stage_EnemySpawnInfo pEnemySpawnInfo in EnemySpawnInfoList)
                            {
                                if (m_pDataStack.m_EnemySpawnInfoCountTable.ContainsKey(pEnemySpawnInfo) == true)
                                {
                                    int nCount = m_pDataStack.m_EnemySpawnInfoCountTable[pEnemySpawnInfo];

                                    if (nCount < pEnemySpawnInfo.m_nAmount && m_pDataStack.m_nEnemyTurnCount >= pEnemySpawnInfo.m_nStartTurn)
                                    {
                                        int nIntervalTurn = m_pDataStack.m_EnemySpawnInfoIntervalTurnTable[pEnemySpawnInfo];

                                        if (--nIntervalTurn <= 0)
                                        {
                                            m_IsSpawnPointAtEnemy_ExistColonySpawn = true;

                                            ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(pEnemySpawnInfo.m_nUnitTableID);
                                            eObjectType eObType = eObjectType.Minion;
                                            if (pUnitInfo.m_eUnitType == eUnitType.BossMinion)
                                                eObType = eObjectType.MinionBoss;

                                            int nLevel = pStageInfo.m_nEnemyLevel;

                                            if (pEnemySpawnInfo.m_nUnitLevel != 0)
                                            {
                                                nLevel = pEnemySpawnInfo.m_nUnitLevel;
                                            }

                                            // 실제 스폰
                                            SlotFixObject_Minion pSlotFixObject_Minion = new SlotFixObject_Minion(pSlot, pEnemySpawnInfo.m_nUnitTableID, nLevel, eMinionType.EnemyMinion, eObType, eOwner.Other, true);
                                            pSlot.AddSlotFixObject(pSlotFixObject_Minion);
                                            m_pDataStack.m_EnemyMinionTable.Add(pSlot.GetSlotIndex(), pSlotFixObject_Minion);

                                            EventDelegateManager.Instance.OnInGame_EnemyMinionSpawn(pSlot, pSlotFixObject_Minion);

                                            //m_pDataStack.m_EnemySpawnInfoIntervalTurnTable[pEnemySpawnInfo] = pEnemySpawnInfo.m_nIntervalTurn + 1;
                                            ResetIntervalTurnList.Add(pEnemySpawnInfo);
                                            m_pDataStack.m_EnemySpawnInfoCountTable[pEnemySpawnInfo] = nCount + 1;
                                            --nPossibleCreateCount;

                                            if (pUnitInfo != null && pUnitInfo.m_eUnitType == eUnitType.BossMinion)
                                            {
                                                m_pDataStack.m_EnemyMinionBossAppearList.Add(pSlotFixObject_Minion);
                                            }

                                            Helper.OnSoundPlay("INGAME_MINION_SPAWN", false);

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        bool IsCreateEnemyColony = false;

        foreach (Slot pSlot in spawnPointSlotList)
        {
            if (nPossibleCreateCount > 0)
            {
                if (pSlot.GetSlotBlock() != null && pSlot.GetSlotFixObjectList().Count == 0)
                {
                    foreach (ExcelData_Stage_EnemySpawnInfo pEnemySpawnInfo in EnemySpawnInfoList)
                    {
                        if (m_pDataStack.m_EnemySpawnInfoCountTable.ContainsKey(pEnemySpawnInfo) == true)
                        {
                            int nCount = m_pDataStack.m_EnemySpawnInfoCountTable[pEnemySpawnInfo];

                            if (nCount < pEnemySpawnInfo.m_nAmount && m_pDataStack.m_nEnemyTurnCount >= pEnemySpawnInfo.m_nStartTurn)
                            {
                                int nIntervalTurn = m_pDataStack.m_EnemySpawnInfoIntervalTurnTable[pEnemySpawnInfo];

                                if (--nIntervalTurn <= 0)
                                {
                                    m_pSpawnPointAtEnemySpawnTable.Add(pSlot, pEnemySpawnInfo);

                                    ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(pEnemySpawnInfo.m_nUnitTableID);
                                    ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(pUnitInfo.m_nAttack_ActionTableID);

                                    IsCreateEnemyColony = true;

                                    if (m_pDataStack.m_EnemyColonyTable.ContainsKey(pSlot.GetSlotIndex()) == true)
                                    {
                                        m_pDataStack.m_EnemyColonyTable.Remove(pSlot.GetSlotIndex());
                                        pSlot.RemoveAllSlotFixObject();
                                    }

                                    ExcelData_EnemyColonyInfo pEnemyColonyInfo = ExcelDataManager.Instance.m_pEnemyColony.GetEnemyColonyInfo(GameDefine.ms_nBasicEnemyColonyID);
                                    SlotFixObject_EnemyColony pSlotFixObject_EnemyColony = new SlotFixObject_EnemyColony(pSlot, pSlot.GetSlotIndex(), pEnemyColonyInfo, 1);
                                    pSlot.AddSlotFixObject(pSlotFixObject_EnemyColony);
                                    m_pDataStack.m_EnemyColonyTable.Add(pSlot.GetSlotIndex(), pSlotFixObject_EnemyColony);

                                    if (pActionInfo != null)
                                    {
                                        ParticleManager.Instance.LoadParticleSystem(pActionInfo.m_strEffectHit, pSlot.GetPosition());
                                    }

                                    //m_pDataStack.m_EnemySpawnInfoIntervalTurnTable[pEnemySpawnInfo] = pEnemySpawnInfo.m_nIntervalTurn + 1;
                                    ResetIntervalTurnList.Add(pEnemySpawnInfo);
                                    m_pDataStack.m_EnemySpawnInfoCountTable[pEnemySpawnInfo] = nCount + 1;
                                    --nPossibleCreateCount;

                                    Helper.OnSoundPlay("INGAME_MINION_SPAWN", false);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        foreach (ExcelData_Stage_EnemySpawnInfo pEnemySpawnInfo in EnemySpawnInfoList)
        {
            if (m_pDataStack.m_EnemySpawnInfoCountTable.ContainsKey(pEnemySpawnInfo) == true)
            {
                if (m_pDataStack.m_nEnemyTurnCount >= pEnemySpawnInfo.m_nStartTurn)
                {
                    if (ResetIntervalTurnList.Contains(pEnemySpawnInfo) == false)
                    {
                        --m_pDataStack.m_EnemySpawnInfoIntervalTurnTable[pEnemySpawnInfo];
                    }
                    else
                    {
                        m_pDataStack.m_EnemySpawnInfoIntervalTurnTable[pEnemySpawnInfo] = pEnemySpawnInfo.m_nIntervalTurn;
                    }
                }
            }
        }

        if (IsCreateEnemyColony == true)
        {
            m_IsSpawnPointAtEnemy_CreateEnemyDelay = true;
            m_pTimer_SpawnPointAtEnemy_CreateEnemyColony.OnPlay();
        }
        else if (m_nMoveAndCreateCount == 0)
        {
            EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);
            EventDelegateManager.Instance.OnInGame_TurnComponentDone(eTurnComponentType.EnemyColonyChangeState);

            OutputLog.Log("TurnComponent_EnemyMoveAndSpawn : GetNextEvent().OnEvent(EventArg_Null.Object)");
            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
    }

    private void OnInGame_EnemyMinionMoveAndCreateDone()
    {
        OutputLog.Log("TurnComponent_EnemyMoveAndSpawn : OnInGame_EnemyMinionMoveAndCreateDone");

        --m_nMoveAndCreateCount;

        if (m_nMoveAndCreateCount == 0 && m_IsSpawnPointAtEnemy_CreateEnemyDelay == false)
        {
            if (m_IsSpawnPointAtEnemy_ExistColonySpawn == false)
            {
                EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);
                EventDelegateManager.Instance.OnInGame_TurnComponentDone(eTurnComponentType.EnemyColonyChangeState);

                OutputLog.Log("TurnComponent_EnemyMoveAndSpawn : GetNextEvent().OnEvent(EventArg_Null.Object)");
                GetNextEvent().OnEvent(EventArg_Null.Object);
            }
            else
            {
                m_pTimer_SpawnPointAtEnemy_CreateEnemyDelay.OnPlay();
            }
        }
    }

    private void OnDone_Timer_SpawnPointAtEnemy_CreateEnemyColony(TransformerEvent eventValue)
    {
        OutputLog.Log("TurnComponent_EnemyMoveAndSpawn : OnDone_Timer_SpawnPointAtEnemy_CreateEnemyColony");

        ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);

        foreach (KeyValuePair<Slot, ExcelData_Stage_EnemySpawnInfo> item in m_pSpawnPointAtEnemySpawnTable)
        {
            ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(item.Value.m_nUnitTableID);
            eObjectType eObType = eObjectType.Minion;
            if (pUnitInfo.m_eUnitType == eUnitType.BossMinion)
                eObType = eObjectType.MinionBoss;

            int nLevel = pStageInfo.m_nEnemyLevel;

            if (item.Value.m_nUnitLevel != 0)
            {
                nLevel = item.Value.m_nUnitLevel;
            }

            SlotFixObject_Minion pSlotFixObject_Minion = new SlotFixObject_Minion(item.Key, item.Value.m_nUnitTableID, nLevel, eMinionType.EnemyMinion, eObType, eOwner.Other, true);
            item.Key.AddSlotFixObject(pSlotFixObject_Minion);
            m_pDataStack.m_EnemyMinionTable.Add(item.Key.GetSlotIndex(), pSlotFixObject_Minion);

            EventDelegateManager.Instance.OnInGame_EnemyMinionSpawn(item.Key, pSlotFixObject_Minion);
        }

        m_pTimer_SpawnPointAtEnemy_CreateEnemyDelay.OnPlay();
    }

    private void OnDone_Timer_SpawnPointAtEnemy_CreateEnemyDelay(TransformerEvent eventValue)
    {
        OutputLog.Log("TurnComponent_EnemyMoveAndSpawn : OnDone_Timer_SpawnPointAtEnemy_CreateEnemyDelay");

        EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);
        EventDelegateManager.Instance.OnInGame_TurnComponentDone(eTurnComponentType.EnemyColonyChangeState);

        OutputLog.Log("TurnComponent_EnemyMoveAndSpawn : GetNextEvent().OnEvent(EventArg_Null.Object)");
        GetNextEvent().OnEvent(EventArg_Null.Object);
    }
    public int IsProcess()
    {
        ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);

        m_CurrTurnMoveAndSpawnTargetList.Clear();
        m_MinionPathTable.Clear();
        m_nMoveAndCreateCount = 0;
        m_ShieldDecreaseMinionTable.Clear();
        m_pSpawnPointAtEnemySpawnTable.Clear();

        bool[,] Map = new bool[GameDefine.ms_nInGameSlot_X, GameDefine.ms_nInGameSlot_Y];

        for (int y = 0; y < GameDefine.ms_nInGameSlot_Y; ++y)
        {
            for (int x = 0; x < GameDefine.ms_nInGameSlot_X; ++x)
            {
                int nSlotIndex = Helper.GetSlotIndex(x, y);

                if (m_pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(nSlotIndex) == true)
                {
                    Slot pSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[nSlotIndex];

                    if (pSlot.GetSlotFixObjectList().Count == 1)
                    {
                        SlotFixObject_Espresso pSlotFixObject_Espresso = pSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                        if (pSlotFixObject_Espresso != null)
                        {
                            switch (pSlotFixObject_Espresso.GetObjectType())
                            {
                                case eObjectType.EnemyColony:
                                    {
                                        Map[x, y] = true;
                                    }
                                    break;
                                case eObjectType.Minion:
                                case eObjectType.MinionBoss:
                                    {
                                        Map[x, y] = false;
                                    }
                                    break;
                                case eObjectType.Character:
                                    {
                                        SlotFixObject_PlayerCharacter pPlayerCharactger = pSlotFixObject_Espresso as SlotFixObject_PlayerCharacter;
                                        if (pPlayerCharactger != null)
                                        {
                                            if (pPlayerCharactger.GetHP() > 0)
                                            {
                                                Map[x, y] = false;
                                            }
                                            else
                                            {
                                                Map[x, y] = true;
                                            }
                                        }
                                        else
                                        {
                                            Map[x, y] = true;
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        Map[x, y] = true;
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            SlotFixObject_Obstacle pSlotFixObject_Obstacle = pSlot.GetLastSlotFixObject() as SlotFixObject_Obstacle;

                            if (pSlotFixObject_Obstacle == null)
                            {
                                Map[x, y] = true;
                            }
                            else
                            {
                                Map[x, y] = false;
                            }
                        }
                    }
                    else
                    {
                        Map[x, y] = false;
                    }
                }
                else
                {
                    Map[x, y] = false;
                }
            }
        }

        if (m_pDataStack.m_EnemyMinionTable.Count != 0 || m_pDataStack.m_EnemySummonUnitTable.Count != 0)
        {
            Dictionary<int, SlotFixObject_Minion> dataTable = new Dictionary<int, SlotFixObject_Minion>();

            foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pDataStack.m_EnemyMinionTable)
            {
                dataTable.Add(item.Key, item.Value);
            }

            foreach (KeyValuePair<int, SlotFixObject_Minion> item in m_pDataStack.m_EnemySummonUnitTable)
            {
                dataTable.Add(item.Key, item.Value);
            }

            dataTable = dataTable.OrderBy(item => item.Key).ToDictionary(x => x.Key, x => x.Value);

            foreach (KeyValuePair<int, SlotFixObject_Minion> item in dataTable)
            {
                ExcelData_UnitInfo pUnitInfo = item.Value.GetUnitInfo();

                if (pUnitInfo != null && pUnitInfo.m_eMoveType != eMoveType.Fixed && item.Value.IsExistApplyStatusEffect_byEffectType(eDamageEffectType.MoveHold) == false)
                {
                    if (pUnitInfo.m_eMoveType == eMoveType.Path)
                    {
                        int nStartX = item.Value.GetSlot().GetX();
                        int nStartY = item.Value.GetSlot().GetY();

                        List<ExcelData_Unit_MoveRangeInfo> MoveRangeInfoList = ExcelDataManager.Instance.m_pUnit_MoveRange.GetMoveRangeInfoList(pUnitInfo.m_nMoveRangeID);

                        if (MoveRangeInfoList.Count != 0)
                        {
                            Helper.ShuffleList<ExcelData_Unit_MoveRangeInfo>(MoveRangeInfoList);
                            MoveRangeInfoList.Sort(CompareMoveRangeInfoList);

                            foreach (ExcelData_Unit_MoveRangeInfo pMoveRangeInfo in MoveRangeInfoList)
                            {
                                int nX = nStartX + pMoveRangeInfo.m_nPosX;
                                int nY = nStartY + pMoveRangeInfo.m_nPosY;

                                if (SlotManager.IsValidSlotIndex(nX, nY) == true)
                                {
                                    PathFind.Grid pGrid = new PathFind.Grid(GameDefine.ms_nInGameSlot_X, GameDefine.ms_nInGameSlot_Y, Map);
                                    List<Point> pointList = Pathfinding.FindPath(pGrid, new Point(nStartX, nStartY), new Point(nX, nY));

                                    if (pointList.Count != 0)
                                    {
                                        Map[nStartX, nStartY] = true;
                                        Map[nX, nY] = false;

                                        Slot pSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nX, nY)];
                                        SlotFixObject_PlayerCharacter pPlayerCharacter = pSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                        if (pPlayerCharacter != null)
                                        {
                                            if (pPlayerCharacter.GetHP() > 0)
                                            {
                                                return 1;
                                            }
                                            else
                                            {
                                                return 1;
                                            }
                                        }
                                        else
                                        {
                                            Point pLastPoint = pointList[pointList.Count - 1];

                                            return 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (pUnitInfo.m_eMoveType == eMoveType.Warp)
                    {
                        int nStartX = item.Value.GetSlot().GetX();
                        int nStartY = item.Value.GetSlot().GetY();

                        List<ExcelData_Unit_MoveRangeInfo> MoveRangeInfoList = ExcelDataManager.Instance.m_pUnit_MoveRange.GetMoveRangeInfoList(pUnitInfo.m_nMoveRangeID);

                        if (MoveRangeInfoList.Count != 0)
                        {
                            Helper.ShuffleList<ExcelData_Unit_MoveRangeInfo>(MoveRangeInfoList);
                            MoveRangeInfoList.Sort(CompareMoveRangeInfoList);

                            foreach (ExcelData_Unit_MoveRangeInfo pMoveRangeInfo in MoveRangeInfoList)
                            {
                                int nX = nStartX + pMoveRangeInfo.m_nPosX;
                                int nY = nStartY + pMoveRangeInfo.m_nPosY;

                                if (SlotManager.IsValidSlotIndex(nX, nY) == true && Map[nX, nY] == true)
                                {
                                    Map[nStartX, nStartY] = true;
                                    Map[nX, nY] = false;

                                    Slot pSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nX, nY)];
                                    SlotFixObject_PlayerCharacter pPlayerCharacter = pSlot.GetLastSlotFixObject() as SlotFixObject_PlayerCharacter;

                                    if (pPlayerCharacter != null)
                                    {
                                        if (pPlayerCharacter.GetHP() > 0)
                                        {
                                            return 1;
                                        }
                                        else
                                        {
                                            return 1;
                                        }
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        bool IsEmptySpawnPoint = true;
        List<Slot> spawnPointSlotList = new List<Slot>();

        List<InGameStageMapSlotData> spawnStageMapSlotDataList = m_pDataStack.m_pInGameStageMapData.GetInGameStageMapSlotData(eMapSlotItem.EnemySpawn);

        foreach (InGameStageMapSlotData pInGameStageMapSlotData in spawnStageMapSlotDataList)
        {
            Vector2Int vIndex = ExcelDataHelper.GetSlotConverting(pInGameStageMapSlotData.m_nX, pInGameStageMapSlotData.m_nY);
            int nSlotIndex = Helper.GetSlotIndex(vIndex.x, vIndex.y);
            if (m_pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(nSlotIndex) == true)
            {
                Slot pSlot = m_pDataStack.m_pSlotManager.GetSlotTable()[nSlotIndex];
                spawnPointSlotList.Add(pSlot);

                SlotFixObject_Espresso pSlotFixObject_Espresso = pSlot.GetLastSlotFixObject() as SlotFixObject_Espresso;

                if (pSlot.GetSlotBlock() == null && pSlotFixObject_Espresso != null)
                {
                    IsEmptySpawnPoint = false;
                }
            }
        }

        Helper.ShuffleList<Slot>(spawnPointSlotList);

        List<ExcelData_Stage_EnemySpawnInfo> ResetIntervalTurnList = new List<ExcelData_Stage_EnemySpawnInfo>();
        List<ExcelData_Stage_EnemySpawnInfo> EnemySpawnInfoList = ExcelDataManager.Instance.m_pStage_EnemySpawn.GetEnemySpawnInfoList(EspressoInfo.Instance.m_nStageID);
        int nPossibleCreateCount = m_pDataStack.m_pStageInfo.m_nEnemySpawnMax - m_pDataStack.m_EnemyMinionTable.Count;

        if (IsEmptySpawnPoint == false)
        {
            foreach (Slot pSlot in spawnPointSlotList)
            {
                if (nPossibleCreateCount > 0)
                {
                    if (pSlot.GetSlotBlock() == null && pSlot.GetSlotFixObjectList().Count == 1)
                    {
                        SlotFixObject_EnemyColony pEnemyColony = pSlot.GetSlotFixObjectList()[0] as SlotFixObject_EnemyColony;

                        if (pEnemyColony != null)
                        {
                            foreach (ExcelData_Stage_EnemySpawnInfo pEnemySpawnInfo in EnemySpawnInfoList)
                            {
                                if (m_pDataStack.m_EnemySpawnInfoCountTable.ContainsKey(pEnemySpawnInfo) == true)
                                {
                                    int nCount = m_pDataStack.m_EnemySpawnInfoCountTable[pEnemySpawnInfo];

                                    if (nCount < pEnemySpawnInfo.m_nAmount && m_pDataStack.m_nEnemyTurnCount >= pEnemySpawnInfo.m_nStartTurn)
                                    {
                                        int nIntervalTurn = m_pDataStack.m_EnemySpawnInfoIntervalTurnTable[pEnemySpawnInfo];

                                        if (--nIntervalTurn <= 0)
                                        {
                                            return 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        foreach (Slot pSlot in spawnPointSlotList)
        {
            if (nPossibleCreateCount > 0)
            {
                if (pSlot.GetSlotBlock() != null && pSlot.GetSlotFixObjectList().Count == 0)
                {
                    foreach (ExcelData_Stage_EnemySpawnInfo pEnemySpawnInfo in EnemySpawnInfoList)
                    {
                        if (m_pDataStack.m_EnemySpawnInfoCountTable.ContainsKey(pEnemySpawnInfo) == true)
                        {
                            int nCount = m_pDataStack.m_EnemySpawnInfoCountTable[pEnemySpawnInfo];

                            if (nCount < pEnemySpawnInfo.m_nAmount && m_pDataStack.m_nEnemyTurnCount >= pEnemySpawnInfo.m_nStartTurn)
                            {
                                int nIntervalTurn = m_pDataStack.m_EnemySpawnInfoIntervalTurnTable[pEnemySpawnInfo];

                                if (--nIntervalTurn <= 0)
                                {
                                    return 1;
                                }
                            }
                        }
                    }
                }
            }
        }

        return 0;
    }
}
