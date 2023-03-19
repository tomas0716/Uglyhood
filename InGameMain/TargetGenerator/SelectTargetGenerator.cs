using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTargetInfo
{
    public ExcelData_Action_SelectTargetRangeInfo       m_pRangeInfo            = null;
    public ExcelData_Action_SelectTargetConditionInfo   m_pConditionInfo        = null;
    public int                                          m_nPriority             = 0;

    public int                                          m_nSelectTargetSlotX    = 0;
    public int                                          m_nSelectTargetSlotY    = 0;
}

public class SelectTargetGenerator
{
    private List<SelectTargetInfo> m_SelectTargetInfoList               = new List<SelectTargetInfo>();
    private List<SelectTargetInfo> m_FirstPrioritySelectTargetInfoList  = new List<SelectTargetInfo>();

    public SelectTargetGenerator()
	{
	}

    public void OnGenerator(int nActionID, Slot pSlot, eOwner eOwner, eObjectType eObjType, eAttackType eAtkType, int nSelectTargetRangeLevel = 1)
    {
        eOwner eOwner_Differ = eOwner == eOwner.My ? eOwner.Other : eOwner.My;

        m_SelectTargetInfoList.Clear();
        m_FirstPrioritySelectTargetInfoList.Clear();

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        int nSlotX = 0, nSlotY = 0;
        if (pSlot != null)
        {
            nSlotX = pSlot.GetX();
            nSlotY = pSlot.GetY();
        }

        ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(nActionID);

        if (pActionInfo != null)
        {
            List<ExcelData_Action_SelectTargetRangeInfo> SelectTargetRangeInfoList = ExcelDataManager.Instance.m_pAction_SelectTargetRange.GetAction_SelectTargetRangeInfoList(nActionID, nSelectTargetRangeLevel);
            if (SelectTargetRangeInfoList.Count != 0)
            {
                foreach (ExcelData_Action_SelectTargetRangeInfo pSelectTargetRangeInfo in SelectTargetRangeInfoList)
                {
                    switch (pSelectTargetRangeInfo.m_eRangeType)
                    {
                        case eRangeType.All:
                            {
                                Dictionary<int,Slot> slotTable = pDataStack.m_pSlotManager.GetSlotTable();

                                foreach (KeyValuePair<int, Slot> item in slotTable)
                                {
                                    Slot pTargetSlot = item.Value;

                                    List<ExcelData_Action_SelectTargetConditionInfo> SelectTargetConditionInfoList = ExcelDataManager.Instance.m_pAction_SelectTargetCondition.GetAction_SelectTargetConditionInfoList(nActionID);

                                    foreach (ExcelData_Action_SelectTargetConditionInfo pSelectTargetConditionInfo in SelectTargetConditionInfoList)
                                    {
                                        switch (pSelectTargetConditionInfo.m_eTargetSide)
                                        {
                                            case eSideType.NeutralSide:
                                                {
                                                    if (pTargetSlot.GetSlotBlock() != null)
                                                    {
                                                        switch (pSelectTargetConditionInfo.m_eTargetType)
                                                        {
                                                            case eObjectType.Block:
                                                                {
                                                                    if (pTargetSlot.GetSlotBlock().GetSpecialItem() == eSpecialItem.None &&
                                                                        (pSelectTargetConditionInfo.m_eElement == eElement.All ||
                                                                         pTargetSlot.GetSlotBlock().GetBlockType() == (eBlockType)pSelectTargetConditionInfo.m_eElement))
                                                                    {
                                                                        if (pActionInfo.m_eDamageType != eDamageEffectType.UnitSummon ||
                                                                            (pActionInfo.m_eDamageType == eDamageEffectType.UnitSummon && pTargetSlot.IsSlotFixObject_Obstacle() == false))
                                                                        {
                                                                            InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                            if (pCloud == null || (eObjType == eObjectType.Minion && eAtkType == eAttackType.Attack))
                                                                            {
                                                                                SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                            case eObjectType.SpecialItem:
                                                                {
                                                                    if (pTargetSlot.GetSlotBlock().GetSpecialItem() != eSpecialItem.None &&
                                                                        (pSelectTargetConditionInfo.m_eElement == eElement.All ||
                                                                         pTargetSlot.GetSlotBlock().GetBlockType() == (eBlockType)pSelectTargetConditionInfo.m_eElement))
                                                                    {
                                                                        if (pActionInfo.m_eDamageType != eDamageEffectType.UnitSummon ||
                                                                            (pActionInfo.m_eDamageType == eDamageEffectType.UnitSummon && pTargetSlot.IsSlotFixObject_Obstacle() == false))
                                                                        {
                                                                            InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                            if (pCloud == null || (eObjType == eObjectType.Minion && eAtkType == eAttackType.Attack))
                                                                            {
                                                                                SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                            case eObjectType.BlockObstacle:
                                                                {
                                                                    if (SlotManager.IsElementBlock(pTargetSlot) == false)
                                                                    {
                                                                        InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                        if (pCloud == null || (eObjType == eObjectType.Minion && eAtkType == eAttackType.Attack))
                                                                        {
                                                                            SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                            pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                            pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                            pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                            pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                            pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                            m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                            case eObjectType.SlotObstacle:
                                                                {
                                                                    if (pTargetSlot.IsSlotFixObject_Obstacle() == true)
                                                                    {
                                                                        if (pActionInfo.m_eDamageType != eDamageEffectType.UnitSummon ||
                                                                            (pActionInfo.m_eDamageType == eDamageEffectType.UnitSummon && pTargetSlot.IsSlotFixObject_Obstacle() == false))
                                                                        {
                                                                            InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                            if (pCloud == null || (eObjType == eObjectType.Minion && eAtkType == eAttackType.Attack))
                                                                            {
                                                                                SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        switch (pSelectTargetConditionInfo.m_eTargetType)
                                                        {
                                                            case eObjectType.SlotObstacle:
                                                                {
                                                                    if (pTargetSlot.IsSlotFixObject_Obstacle() == true)
                                                                    {
                                                                        InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                        if (pCloud == null || (eObjType == eObjectType.Minion && eAtkType == eAttackType.Attack))
                                                                        {
                                                                            SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                            pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                            pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                            pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                            pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                            pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                            m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                        }
                                                    }
                                                }
                                                break;
                                            case eSideType.SameSide:
                                                {
                                                    List<SlotFixObject> slotFixObjectList = pTargetSlot.GetSlotFixObjectList();

                                                    if (slotFixObjectList.Count != 0)
                                                    {
                                                        SlotFixObject_Espresso pSlotFixObject = slotFixObjectList[slotFixObjectList.Count - 1] as SlotFixObject_Espresso;

                                                        if (pSlotFixObject != null)
                                                        {
                                                            if (pSlotFixObject.GetOwner() == eOwner)
                                                            {
                                                                if (pSlotFixObject.GetObjectType() == pSelectTargetConditionInfo.m_eTargetType)
                                                                {
                                                                    switch (pSlotFixObject.GetObjectType())
                                                                    {
                                                                        case eObjectType.EnemyColony:
                                                                            {
                                                                                InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                if (pCloud == null)
                                                                                {
                                                                                    SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                    pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                    pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                    pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                    pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                    pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                    m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                }
                                                                            }
                                                                            break;

                                                                        case eObjectType.Minion:
                                                                        case eObjectType.MinionBoss:
                                                                        case eObjectType.EnemyBoss:
                                                                            {
                                                                                SlotFixObject_Unit pSlotFixObject_Unit = pSlotFixObject as SlotFixObject_Unit;

                                                                                if (pSlotFixObject_Unit != null && (pSelectTargetConditionInfo.m_eElement == eElement.All || pSlotFixObject_Unit.GetUnitInfo().m_eElement == pSelectTargetConditionInfo.m_eElement))
                                                                                {
                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                    if (pCloud == null)
                                                                                    {
                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                    }
                                                                                }
                                                                            }
                                                                            break;

                                                                        case eObjectType.Character:
                                                                            {
                                                                                SlotFixObject_PlayerCharacter pPlayerCharacter = pSlotFixObject as SlotFixObject_PlayerCharacter;

                                                                                if (pPlayerCharacter != null /*&& pPlayerCharacter.IsDead() == false*/ && (pSelectTargetConditionInfo.m_eElement == eElement.All || pPlayerCharacter.GetUnitInfo().m_eElement == pSelectTargetConditionInfo.m_eElement))
                                                                                {
                                                                                    switch (pActionInfo.m_eDamageType)
                                                                                    {
                                                                                        case eDamageEffectType.Revival:
                                                                                            {
                                                                                                if (pPlayerCharacter.IsDead() == true)
                                                                                                {
                                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                    if (pCloud == null)
                                                                                                    {
                                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            break;
                                                                                        case eDamageEffectType.CharacterSwap:
                                                                                        case eDamageEffectType.UnitClone:
                                                                                        case eDamageEffectType.Blind:
                                                                                            {
                                                                                                InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                if (pCloud == null)
                                                                                                {
                                                                                                    SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                    pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                    pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                    pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                    pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                                    pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                                    m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                }
                                                                                            }
                                                                                            break;
                                                                                        default:
                                                                                            {
                                                                                                if (pPlayerCharacter.IsDead() == false)
                                                                                                {
                                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                    if (pCloud == null)
                                                                                                    {
                                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                    }
                                                                                                }
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
                                                    }
                                                }
                                                break;
                                            case eSideType.OhterSide:
                                                {
                                                    List<SlotFixObject> slotFixObjectList = pTargetSlot.GetSlotFixObjectList();

                                                    if (slotFixObjectList.Count != 0)
                                                    {
                                                        SlotFixObject_Espresso pSlotFixObject = slotFixObjectList[slotFixObjectList.Count - 1] as SlotFixObject_Espresso;

                                                        if (pSlotFixObject != null)
                                                        {
                                                            if (pSlotFixObject.GetOwner() != eOwner)
                                                            {
                                                                if (pSlotFixObject.GetObjectType() == pSelectTargetConditionInfo.m_eTargetType)
                                                                {
                                                                    switch (pSlotFixObject.GetObjectType())
                                                                    {
                                                                        case eObjectType.EnemyColony:
                                                                            {
                                                                                InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                if (pCloud == null)
                                                                                {
                                                                                    SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                    pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                    pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                    pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                    pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                    pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                    m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                }
                                                                            }
                                                                            break;

                                                                        case eObjectType.Minion:
                                                                        case eObjectType.MinionBoss:
                                                                        case eObjectType.EnemyBoss:
                                                                            {
                                                                                SlotFixObject_Unit pSlotFixObject_Unit = pSlotFixObject as SlotFixObject_Unit;

                                                                                if (pSlotFixObject_Unit != null && (pSelectTargetConditionInfo.m_eElement == eElement.All || pSlotFixObject_Unit.GetUnitInfo().m_eElement == pSelectTargetConditionInfo.m_eElement))
                                                                                {
                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                    if (pCloud == null)
                                                                                    {
                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                    }
                                                                                }
                                                                            }
                                                                            break;
                                                                        case eObjectType.Character:
                                                                            {
                                                                                SlotFixObject_PlayerCharacter pPlayerCharacter = pSlotFixObject as SlotFixObject_PlayerCharacter;

                                                                                if (pPlayerCharacter != null /*&& pPlayerCharacter.IsDead() == false*/ && (pSelectTargetConditionInfo.m_eElement == eElement.All || pPlayerCharacter.GetUnitInfo().m_eElement == pSelectTargetConditionInfo.m_eElement))
                                                                                {
                                                                                    switch (pActionInfo.m_eDamageType)
                                                                                    {
                                                                                        case eDamageEffectType.Revival:
                                                                                            {
                                                                                                if (pPlayerCharacter.IsDead() == true)
                                                                                                {
                                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                    if (pCloud == null)
                                                                                                    {
                                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            break;
                                                                                        case eDamageEffectType.CharacterSwap:
                                                                                        case eDamageEffectType.UnitClone:
                                                                                        case eDamageEffectType.Blind:
                                                                                            {
                                                                                                InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                if (pCloud == null)
                                                                                                {
                                                                                                    SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                    pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                    pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                    pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                    pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                                    pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                                    m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                }
                                                                                            }
                                                                                            break;
                                                                                        default:
                                                                                            {
                                                                                                if (pPlayerCharacter.IsDead() == false)
                                                                                                {
                                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                    if (pCloud == null)
                                                                                                    {
                                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                    }
                                                                                                }
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
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                            break;
                        case eRangeType.RelativeCoordinate:
                            {
                                int nTargetSlotX = nSlotX + pSelectTargetRangeInfo.m_nPosX;
                                int nTargetSlotY = nSlotY + pSelectTargetRangeInfo.m_nPosY;

								if (eOwner == eOwner.My)
								{
                                    nTargetSlotY = nSlotY + pSelectTargetRangeInfo.m_nPosY * -1;
								}

								if (pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nTargetSlotX, nTargetSlotY)) == true)
                                {
                                    Slot pTargetSlot = pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nTargetSlotX, nTargetSlotY)];

                                    List<ExcelData_Action_SelectTargetConditionInfo> SelectTargetConditionInfoList = ExcelDataManager.Instance.m_pAction_SelectTargetCondition.GetAction_SelectTargetConditionInfoList(nActionID);

                                    foreach (ExcelData_Action_SelectTargetConditionInfo pSelectTargetConditionInfo in SelectTargetConditionInfoList)
                                    {
                                        switch (pSelectTargetConditionInfo.m_eTargetSide)
                                        {
                                            case eSideType.NeutralSide:
                                                {
                                                    if (pTargetSlot.GetSlotBlock() != null)
                                                    {
                                                        switch (pSelectTargetConditionInfo.m_eTargetType)
                                                        {
                                                            case eObjectType.Block:
                                                                {
                                                                    if (pTargetSlot.GetSlotBlock().GetSpecialItem() == eSpecialItem.None &&
                                                                        (pSelectTargetConditionInfo.m_eElement == eElement.All ||
                                                                         pTargetSlot.GetSlotBlock().GetBlockType() == (eBlockType)pSelectTargetConditionInfo.m_eElement))
                                                                    {
                                                                        if (pActionInfo.m_eDamageType != eDamageEffectType.UnitSummon ||
                                                                            (pActionInfo.m_eDamageType == eDamageEffectType.UnitSummon && pTargetSlot.IsSlotFixObject_Obstacle() == false))
                                                                        {
                                                                            InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                            if (pCloud == null || (eObjType == eObjectType.Minion && eAtkType == eAttackType.Attack))
                                                                            {
                                                                                SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                            case eObjectType.SpecialItem:
                                                                {
                                                                    if (pTargetSlot.GetSlotBlock().GetSpecialItem() != eSpecialItem.None &&
                                                                        (pSelectTargetConditionInfo.m_eElement == eElement.All ||
                                                                         pTargetSlot.GetSlotBlock().GetBlockType() == (eBlockType)pSelectTargetConditionInfo.m_eElement))
                                                                    {
                                                                        if (pActionInfo.m_eDamageType != eDamageEffectType.UnitSummon ||
                                                                            (pActionInfo.m_eDamageType == eDamageEffectType.UnitSummon && pTargetSlot.IsSlotFixObject_Obstacle() == false))
                                                                        {
                                                                            InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                            if (pCloud == null || (eObjType == eObjectType.Minion && eAtkType == eAttackType.Attack))
                                                                            {
                                                                                SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                            case eObjectType.BlockObstacle:
                                                                {
                                                                    if (SlotManager.IsElementBlock(pTargetSlot) == false)
                                                                    {
                                                                        InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                        if (pCloud == null || (eObjType == eObjectType.Minion && eAtkType == eAttackType.Attack))
                                                                        {
                                                                            SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                            pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                            pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                            pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                            pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                            pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                            m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                            case eObjectType.SlotObstacle:
                                                                {
                                                                    if (pTargetSlot.IsSlotFixObject_Obstacle() == true)
                                                                    {
                                                                        if (pActionInfo.m_eDamageType != eDamageEffectType.UnitSummon ||
                                                                            (pActionInfo.m_eDamageType == eDamageEffectType.UnitSummon && pTargetSlot.IsSlotFixObject_Obstacle() == false))
                                                                        {
                                                                            InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                            if (pCloud == null || (eObjType == eObjectType.Minion && eAtkType == eAttackType.Attack))
                                                                            {
                                                                                SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                        }
                                                    }
                                                }
                                                break;
                                            case eSideType.SameSide:
                                                {
                                                    List<SlotFixObject> slotFixObjectList = pTargetSlot.GetSlotFixObjectList();

                                                    if (slotFixObjectList.Count != 0)
                                                    {
                                                        SlotFixObject_Espresso pSlotFixObject = slotFixObjectList[slotFixObjectList.Count - 1] as SlotFixObject_Espresso;

                                                        if (pSlotFixObject != null)
                                                        {
                                                            if (pSlotFixObject.GetOwner() == eOwner)
                                                            {
                                                                if (pSlotFixObject.GetObjectType() == pSelectTargetConditionInfo.m_eTargetType)
                                                                {
                                                                    switch (pSlotFixObject.GetObjectType())
                                                                    {
                                                                        case eObjectType.Block:
                                                                        case eObjectType.SpecialItem:
                                                                            {
                                                                                InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                if (pCloud == null || (eObjType == eObjectType.Minion && eAtkType == eAttackType.Attack))
                                                                                {
                                                                                    SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                    pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                    pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                    pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                    pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                    pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                    m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                }
                                                                            }
                                                                            break;

                                                                        case eObjectType.EnemyColony:
                                                                            {
                                                                                InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                if (pCloud == null)
                                                                                {
                                                                                    SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                    pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                    pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                    pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                    pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                    pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                    m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                }
                                                                            }
                                                                            break;

                                                                        case eObjectType.Minion:
                                                                        case eObjectType.MinionBoss:
                                                                        case eObjectType.EnemyBoss:
                                                                            {
                                                                                SlotFixObject_Unit pSlotFixObject_Unit = pSlotFixObject as SlotFixObject_Unit;

                                                                                if (pSlotFixObject_Unit != null && (pSelectTargetConditionInfo.m_eElement == eElement.All || pSlotFixObject_Unit.GetUnitInfo().m_eElement == pSelectTargetConditionInfo.m_eElement))
                                                                                {
                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                    if (pCloud == null)
                                                                                    {
                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                    }
                                                                                }
                                                                            }
                                                                            break;

                                                                        case eObjectType.Character:
                                                                            {
                                                                                SlotFixObject_PlayerCharacter pPlayerCharacter = pSlotFixObject as SlotFixObject_PlayerCharacter;

                                                                                if (pPlayerCharacter != null /*&& pPlayerCharacter.IsDead() == false*/ && (pSelectTargetConditionInfo.m_eElement == eElement.All || pPlayerCharacter.GetUnitInfo().m_eElement == pSelectTargetConditionInfo.m_eElement))
                                                                                {
                                                                                    switch (pActionInfo.m_eDamageType)
                                                                                    {
                                                                                        case eDamageEffectType.Revival:
                                                                                            {
                                                                                                if (pPlayerCharacter.IsDead() == true)
                                                                                                {
                                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                    if (pCloud == null)
                                                                                                    {
                                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            break;
                                                                                        case eDamageEffectType.CharacterSwap:
                                                                                        case eDamageEffectType.UnitClone:
                                                                                        case eDamageEffectType.Blind:
                                                                                            {
                                                                                                InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                if (pCloud == null)
                                                                                                {
                                                                                                    SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                    pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                    pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                    pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                    pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                                    pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                                    m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                }
                                                                                            }
                                                                                            break;
                                                                                        default:
                                                                                            {
                                                                                                if (pPlayerCharacter.IsDead() == false)
                                                                                                {
                                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                    if (pCloud == null)
                                                                                                    {
                                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                    }
                                                                                                }
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
                                                    }
                                                }
                                                break;
                                            case eSideType.OhterSide:
                                                {
                                                    List<SlotFixObject> slotFixObjectList = pTargetSlot.GetSlotFixObjectList();

                                                    if (slotFixObjectList.Count != 0)
                                                    {
                                                        SlotFixObject_Espresso pSlotFixObject = slotFixObjectList[slotFixObjectList.Count - 1] as SlotFixObject_Espresso;

                                                        if (pSlotFixObject != null)
                                                        {
                                                            if (pSlotFixObject.GetOwner() != eOwner)
                                                            {
                                                                if (pSlotFixObject.GetObjectType() == pSelectTargetConditionInfo.m_eTargetType)
                                                                {
                                                                    switch (pSlotFixObject.GetObjectType())
                                                                    {
                                                                        case eObjectType.Block:
                                                                        case eObjectType.SpecialItem:
                                                                            {
                                                                                InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                if (pCloud == null || (eObjType == eObjectType.Minion && eAtkType == eAttackType.Attack))
                                                                                {
                                                                                    SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                    pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                    pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                    pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                    pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                    pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                    m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                }
                                                                            }
                                                                            break;

                                                                        case eObjectType.EnemyColony:
                                                                            {
                                                                                InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                if (pCloud == null)
                                                                                {
                                                                                    SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                    pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                    pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                    pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                    pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                    pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                    m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                }
                                                                            }
                                                                            break;

                                                                        case eObjectType.Minion:
                                                                        case eObjectType.MinionBoss:
                                                                        case eObjectType.EnemyBoss:
                                                                            {
                                                                                SlotFixObject_Unit pSlotFixObject_Unit = pSlotFixObject as SlotFixObject_Unit;

                                                                                if (pSlotFixObject_Unit != null && (pSelectTargetConditionInfo.m_eElement == eElement.All || pSlotFixObject_Unit.GetUnitInfo().m_eElement == pSelectTargetConditionInfo.m_eElement) &&
                                                                                    (pSelectTargetConditionInfo.m_eTargetType == eObjectType.Minion || pSelectTargetConditionInfo.m_eTargetType == eObjectType.MinionBoss ||
                                                                                    pSelectTargetConditionInfo.m_eTargetType == eObjectType.EnemyBoss || pSelectTargetConditionInfo.m_eTargetType == eObjectType.Character))
                                                                                {
                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                    if (pCloud == null)
                                                                                    {
                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                    }
                                                                                }
                                                                            }
                                                                            break;

                                                                        case eObjectType.Character:
                                                                            {
                                                                                SlotFixObject_PlayerCharacter pPlayerCharacter = pSlotFixObject as SlotFixObject_PlayerCharacter;

                                                                                if (pPlayerCharacter != null && (pSelectTargetConditionInfo.m_eElement == eElement.All || pPlayerCharacter.GetUnitInfo().m_eElement == pSelectTargetConditionInfo.m_eElement) &&
                                                                                    pSelectTargetConditionInfo.m_eTargetType == eObjectType.Character)
                                                                                {
                                                                                    switch (pActionInfo.m_eDamageType)
                                                                                    {
                                                                                        case eDamageEffectType.Revival:
                                                                                            {
                                                                                                if (pPlayerCharacter.IsDead() == true)
                                                                                                {
                                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                    if (pCloud == null)
                                                                                                    {
                                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            break;
                                                                                        case eDamageEffectType.CharacterSwap:
                                                                                        case eDamageEffectType.UnitClone:
                                                                                        case eDamageEffectType.Blind:
                                                                                            {
                                                                                                InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                if (pCloud == null)
                                                                                                {
                                                                                                    SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                    pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                    pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                    pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                    pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                                    pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                                    m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                }
                                                                                            }
                                                                                            break;
                                                                                        default:
                                                                                            {
                                                                                                if (pPlayerCharacter.IsDead() == false)
                                                                                                {
                                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                    if (pCloud == null)
                                                                                                    {
                                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                    }
                                                                                                }
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
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                            break;
                        case eRangeType.EnemyBossArea:
                            {
                            }
                            break;
                        case eRangeType.PlayerCharacterArea:
                            {
                                int nTargetSlotX = nSlotX + pSelectTargetRangeInfo.m_nCharacterSlotPos;
                                int nTargetSlotY = 0;

                                if(eOwner == eOwner.Other)
                                    nTargetSlotY = 9;

                                if (pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nTargetSlotX, nTargetSlotY)) == true)
                                {
                                    Slot pTargetSlot = pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nTargetSlotX, nTargetSlotY)];

                                    List<ExcelData_Action_SelectTargetConditionInfo> SelectTargetConditionInfoList = ExcelDataManager.Instance.m_pAction_SelectTargetCondition.GetAction_SelectTargetConditionInfoList(nActionID);

                                    foreach (ExcelData_Action_SelectTargetConditionInfo pSelectTargetConditionInfo in SelectTargetConditionInfoList)
                                    {
                                        switch (pSelectTargetConditionInfo.m_eTargetSide)
                                        {
                                            case eSideType.SameSide:
                                                {
                                                    List<SlotFixObject> slotFixObjectList = pTargetSlot.GetSlotFixObjectList();

                                                    if (slotFixObjectList.Count != 0)
                                                    {
                                                        SlotFixObject_Espresso pSlotFixObject = slotFixObjectList[slotFixObjectList.Count - 1] as SlotFixObject_Espresso;

                                                        if (pSlotFixObject != null)
                                                        {
                                                            if (pSlotFixObject.GetOwner() == eOwner)
                                                            {
                                                                if (pSlotFixObject.GetObjectType() == pSelectTargetConditionInfo.m_eTargetType)
                                                                {
                                                                    SlotFixObject_Unit pSlotFixObject_Unit = pSlotFixObject as SlotFixObject_Unit;

                                                                    switch (pSlotFixObject_Unit.GetObjectType())
                                                                    {
                                                                        case eObjectType.Character:
                                                                            {
                                                                                SlotFixObject_PlayerCharacter pPlayerCharacter = pSlotFixObject_Unit as SlotFixObject_PlayerCharacter;
                                                                                if (pPlayerCharacter != null /*&& pPlayerCharacter.IsDead() == false*/ && (pSelectTargetConditionInfo.m_eElement == eElement.All || pPlayerCharacter.GetUnitInfo().m_eElement == pSelectTargetConditionInfo.m_eElement))
                                                                                {
                                                                                    switch (pActionInfo.m_eDamageType)
                                                                                    {
                                                                                        case eDamageEffectType.Revival:
                                                                                            {
                                                                                                if (pPlayerCharacter.IsDead() == true)
                                                                                                {
                                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                    if (pCloud == null)
                                                                                                    {
                                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            break;
                                                                                        case eDamageEffectType.CharacterSwap:
                                                                                        case eDamageEffectType.UnitClone:
                                                                                        case eDamageEffectType.Blind:
                                                                                            {
                                                                                                InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                if (pCloud == null)
                                                                                                {
                                                                                                    SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                    pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                    pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                    pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                    pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                                    pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                                    m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                }
                                                                                            }
                                                                                            break;
                                                                                        default:
                                                                                            {
                                                                                                if (pPlayerCharacter.IsDead() == false)
                                                                                                {
                                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                    if (pCloud == null)
                                                                                                    {
                                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            break;
                                                                                    }
                                                                                }
                                                                            }
                                                                            break;
                                                                        default:
                                                                            {
                                                                                if (pSlotFixObject_Unit != null && (pSelectTargetConditionInfo.m_eElement == eElement.All || pSlotFixObject_Unit.GetUnitInfo().m_eElement == pSelectTargetConditionInfo.m_eElement))
                                                                                {
                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                    if (pCloud == null)
                                                                                    {
                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                    }
                                                                                }
                                                                            }
                                                                            break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            case eSideType.OhterSide:
                                                {
                                                    List<SlotFixObject> slotFixObjectList = pTargetSlot.GetSlotFixObjectList();

                                                    if (slotFixObjectList.Count != 0)
                                                    {
                                                        SlotFixObject_Espresso pSlotFixObject = slotFixObjectList[slotFixObjectList.Count - 1] as SlotFixObject_Espresso;

                                                        if (pSlotFixObject != null)
                                                        {
                                                            if (pSlotFixObject.GetOwner() == eOwner)
                                                            {
                                                                if (pSlotFixObject.GetObjectType() == pSelectTargetConditionInfo.m_eTargetType)
                                                                {
                                                                    SlotFixObject_Unit pSlotFixObject_Unit = pSlotFixObject as SlotFixObject_Unit;

                                                                    switch (pSlotFixObject_Unit.GetObjectType())
                                                                    {
                                                                        case eObjectType.Character:
                                                                            {
                                                                                SlotFixObject_PlayerCharacter pPlayerCharacter = pSlotFixObject_Unit as SlotFixObject_PlayerCharacter;
                                                                                if (pPlayerCharacter != null /*&& pPlayerCharacter.IsDead() == false*/ && (pSelectTargetConditionInfo.m_eElement == eElement.All || pPlayerCharacter.GetUnitInfo().m_eElement == pSelectTargetConditionInfo.m_eElement))
                                                                                {
                                                                                    switch (pActionInfo.m_eDamageType)
                                                                                    {
                                                                                        case eDamageEffectType.Revival:
                                                                                            {
                                                                                                if (pPlayerCharacter.IsDead() == true)
                                                                                                {
                                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                    if (pCloud == null)
                                                                                                    {
                                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            break;
                                                                                        case eDamageEffectType.CharacterSwap:
                                                                                        case eDamageEffectType.UnitClone:
                                                                                        case eDamageEffectType.Blind:
                                                                                            {
                                                                                                InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                if (pCloud == null)
                                                                                                {
                                                                                                    SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                    pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                    pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                    pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                    pSelectTargetInfo.m_nSelectTargetSlotX = pTargetSlot.GetX();
                                                                                                    pSelectTargetInfo.m_nSelectTargetSlotY = pTargetSlot.GetY();
                                                                                                    m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                }
                                                                                            }
                                                                                            break;
                                                                                        default:
                                                                                            {
                                                                                                if (pPlayerCharacter.IsDead() == false)
                                                                                                {
                                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                                    if (pCloud == null)
                                                                                                    {
                                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            break;
                                                                                    }
                                                                                }
                                                                            }
                                                                            break;

                                                                        default:
                                                                            {
                                                                                if (pSlotFixObject_Unit != null && (pSelectTargetConditionInfo.m_eElement == eElement.All || pSlotFixObject_Unit.GetUnitInfo().m_eElement == pSelectTargetConditionInfo.m_eElement))
                                                                                {
                                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                                    if (pCloud == null)
                                                                                    {
                                                                                        SelectTargetInfo pSelectTargetInfo = new SelectTargetInfo();
                                                                                        pSelectTargetInfo.m_pRangeInfo = pSelectTargetRangeInfo;
                                                                                        pSelectTargetInfo.m_pConditionInfo = pSelectTargetConditionInfo;
                                                                                        pSelectTargetInfo.m_nPriority = (pSelectTargetRangeInfo.m_nPriority + 1) * 1000 + pSelectTargetConditionInfo.m_nPriority;
                                                                                        pSelectTargetInfo.m_nSelectTargetSlotX = nTargetSlotX;
                                                                                        pSelectTargetInfo.m_nSelectTargetSlotY = nTargetSlotY;
                                                                                        m_SelectTargetInfoList.Add(pSelectTargetInfo);
                                                                                    }
                                                                                }
                                                                            }
                                                                            break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        if (eOwner == eOwner.Other)
        {
            Helper.ShuffleList_UnityEngineRandom<SelectTargetInfo>(m_SelectTargetInfoList);
            Helper.ShuffleList_UnityEngineRandom<SelectTargetInfo>(m_SelectTargetInfoList);
        }
        else
        {
            Helper.ShuffleList(m_SelectTargetInfoList, EspressoInfo.Instance.m_nDamageTargetRandomSeed);
            Helper.ShuffleList(m_SelectTargetInfoList, EspressoInfo.Instance.m_nDamageTargetRandomSeed);
        }

        m_SelectTargetInfoList.Sort(CompareSelectTarget);

        if (m_SelectTargetInfoList.Count != 0)
        {
            m_FirstPrioritySelectTargetInfoList.Add(m_SelectTargetInfoList[0]);
            int nPriority = m_SelectTargetInfoList[0].m_nPriority;

            for (int i = 1; i < m_SelectTargetInfoList.Count; ++i)
            {
                if (m_SelectTargetInfoList[i].m_nPriority == nPriority)
                {
                    m_FirstPrioritySelectTargetInfoList.Add(m_SelectTargetInfoList[i]);
                }
            }
        }
    }

    private int CompareSelectTarget(SelectTargetInfo A, SelectTargetInfo B)
    {
        return B.m_nPriority.CompareTo(A.m_nPriority);
    }

    public int GetNumSelectTargetInfo()
    {
        return m_SelectTargetInfoList.Count;
    }

    public SelectTargetInfo GetSelectTargetInfo_byIndex(int nIndex)
    {
        if (nIndex < 0 || m_SelectTargetInfoList.Count <= nIndex)
        {
            return null;
        }
        return m_SelectTargetInfoList[nIndex];
    }

    public List<SelectTargetInfo> GetFirstPrioritySelectTargetInfoList()
    {
        return m_FirstPrioritySelectTargetInfoList;
    }
}
