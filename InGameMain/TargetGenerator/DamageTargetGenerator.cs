using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTargetInfo
{
    public int m_nActionID = 0;
    public ExcelData_Action_DamageTargetRangeInfo       m_pRangeInfo        = null;
    public ExcelData_Action_DamageTargetConditionInfo   m_pConditionInfo    = null;
    public int                                          m_nPriority         = 0;

    public bool m_IsSubSkillStatusEffect    = false;
}

public class DamageTargetGenerator
{
    private List<DamageTargetInfo> m_DamageTargetInfoList = new List<DamageTargetInfo>();
    private CustomRandomI m_pCustomRandomI = new CustomRandomI();

    public DamageTargetGenerator()
    {
    }

    public void OnGenerator(int nActionID, Slot pSlot, eOwner eOwner, eObjectType eObjType, eAttackType eAtkType, ExcelData_Action_LevelUpInfo pActionLevelUpInfo = null)
    {
        eOwner eOwner_Differ = eOwner == eOwner.My ? eOwner.Other : eOwner.My;
        eRangeType eRangeType = eRangeType.RelativeCoordinate;

        m_DamageTargetInfoList.Clear();
        ExcelData_Action_DamageTargetRangeInfo pDamageTargetRangeInfo_ForAll = null;

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        int nSlotX = pSlot.GetX();
        int nSlotY = pSlot.GetY();

        ExcelData_ActionInfo pActionInfo = ExcelDataManager.Instance.m_pAction.GetActionInfo(nActionID);

        if (pActionInfo != null)
        {
            List<ExcelData_Action_DamageTargetRangeInfo> DamageTargetRangeInfoList;
            DamageTargetRangeInfoList = ExcelDataManager.Instance.m_pAction_DamageTargetRange.GetAction_DamageTargetRangeInfoList(nActionID, pActionLevelUpInfo == null ? 1 : pActionLevelUpInfo.m_nChange_DamageTargetRangeLevel);

            if (DamageTargetRangeInfoList.Count != 0)
            {
                if (DamageTargetRangeInfoList.Count == 1)
                {
                    pDamageTargetRangeInfo_ForAll = DamageTargetRangeInfoList[0];

                    if (pDamageTargetRangeInfo_ForAll.m_eRangeType == eRangeType.All)
                    {
                        DamageTargetRangeInfoList = new List<ExcelData_Action_DamageTargetRangeInfo>();

                        eRangeType = eRangeType.All;

                        Dictionary<int,Slot> slotTable = pDataStack.m_pSlotManager.GetSlotTable();

                        foreach(KeyValuePair<int, Slot> item in slotTable)
                        {
                            int nX = item.Value.GetX();
                            int nY = item.Value.GetY();

                            ExcelData_Action_DamageTargetRangeInfo pCopy = pDamageTargetRangeInfo_ForAll.Copy();
                            pCopy.m_nPosX = nX - nSlotX;
                            pCopy.m_nPosY = nSlotY - nY;
                            DamageTargetRangeInfoList.Add(pCopy);
                        }
                    }
                }

                foreach (ExcelData_Action_DamageTargetRangeInfo pDamageTargetRangeInfo in DamageTargetRangeInfoList)
                {
                    int nTargetSlotX = nSlotX + pDamageTargetRangeInfo.m_nPosX;
                    int nTargetSlotY = nSlotY + pDamageTargetRangeInfo.m_nPosY;

                    if (eOwner == eOwner.My || eRangeType == eRangeType.All)
                    {
                        nTargetSlotY = nSlotY + pDamageTargetRangeInfo.m_nPosY * -1;
                    }

                    if (pDataStack.m_pSlotManager.GetSlotTable().ContainsKey(Helper.GetSlotIndex(nTargetSlotX, nTargetSlotY)) == true)
                    {
                        Slot pTargetSlot = pDataStack.m_pSlotManager.GetSlotTable()[Helper.GetSlotIndex(nTargetSlotX, nTargetSlotY)];

                        List<ExcelData_Action_DamageTargetConditionInfo> DamageTargetConditionInfoList = ExcelDataManager.Instance.m_pAction_DamageTargetCondition.GetAction_DamageTargetConditionInfoList(nActionID);

                        foreach (ExcelData_Action_DamageTargetConditionInfo pDamageTargetConditionInfo in DamageTargetConditionInfoList)
                        {
                            switch (pDamageTargetConditionInfo.m_eTargetSide)
                            {
                                case eSideType.NeutralSide:
                                    {
                                        if (pTargetSlot.GetSlotBlock() != null)
                                        {
                                            switch (pDamageTargetConditionInfo.m_eTargetType)
                                            {
                                                case eObjectType.Block:
                                                    {
                                                        if (pTargetSlot.GetSlotBlock().GetSpecialItem() == eSpecialItem.None &&
                                                            (pDamageTargetConditionInfo.m_eElement == eElement.All ||
                                                             pTargetSlot.GetSlotBlock().GetBlockType() == (eBlockType)pDamageTargetConditionInfo.m_eElement))
                                                        {
                                                            InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                            if (pCloud == null || (eObjType == eObjectType.Minion && eAtkType == eAttackType.Attack))
                                                            {
                                                                DamageTargetInfo pDamageTargetInfo = new DamageTargetInfo();
                                                                pDamageTargetInfo.m_pRangeInfo = pDamageTargetRangeInfo;
                                                                pDamageTargetInfo.m_pConditionInfo = pDamageTargetConditionInfo;
                                                                pDamageTargetInfo.m_nPriority = pDamageTargetConditionInfo.m_nPriority;

                                                                pDamageTargetInfo.m_nActionID = nActionID;
                                                                m_DamageTargetInfoList.Add(pDamageTargetInfo);
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case eObjectType.SpecialItem:
                                                    {
                                                        if (pTargetSlot.GetSlotBlock().GetSpecialItem() != eSpecialItem.None &&
                                                            (pDamageTargetConditionInfo.m_eElement == eElement.All ||
                                                             pTargetSlot.GetSlotBlock().GetBlockType() == (eBlockType)pDamageTargetConditionInfo.m_eElement))
                                                        {
                                                            InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                            if (pCloud == null || (eObjType == eObjectType.Minion && eAtkType == eAttackType.Attack))
                                                            {
                                                                DamageTargetInfo pDamageTargetInfo = new DamageTargetInfo();
                                                                pDamageTargetInfo.m_pRangeInfo = pDamageTargetRangeInfo;
                                                                pDamageTargetInfo.m_pConditionInfo = pDamageTargetConditionInfo;
                                                                pDamageTargetInfo.m_nPriority = pDamageTargetConditionInfo.m_nPriority;

                                                                pDamageTargetInfo.m_nActionID = nActionID;
                                                                m_DamageTargetInfoList.Add(pDamageTargetInfo);
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
                                                                DamageTargetInfo pDamageTargetInfo = new DamageTargetInfo();
                                                                pDamageTargetInfo.m_pRangeInfo = pDamageTargetRangeInfo;
                                                                pDamageTargetInfo.m_pConditionInfo = pDamageTargetConditionInfo;
                                                                pDamageTargetInfo.m_nPriority = pDamageTargetConditionInfo.m_nPriority;

                                                                pDamageTargetInfo.m_nActionID = nActionID;
                                                                m_DamageTargetInfoList.Add(pDamageTargetInfo);
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case eObjectType.SlotObstacle:
                                                    {
                                                        if (pTargetSlot.IsSlotFixObject_Obstacle() == true)
                                                        {
                                                            InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                            if (pCloud == null || (eObjType == eObjectType.Minion && eAtkType == eAttackType.Attack))
                                                            {
                                                                DamageTargetInfo pDamageTargetInfo = new DamageTargetInfo();
                                                                pDamageTargetInfo.m_pRangeInfo = pDamageTargetRangeInfo;
                                                                pDamageTargetInfo.m_pConditionInfo = pDamageTargetConditionInfo;
                                                                pDamageTargetInfo.m_nPriority = pDamageTargetConditionInfo.m_nPriority;

                                                                pDamageTargetInfo.m_nActionID = nActionID;
                                                                m_DamageTargetInfoList.Add(pDamageTargetInfo);
                                                            }
                                                        }
                                                    }
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            switch (pDamageTargetConditionInfo.m_eTargetType)
                                            {
                                                case eObjectType.SlotObstacle:
                                                    {
                                                        if (pTargetSlot.IsSlotFixObject_Obstacle() == true)
                                                        {
                                                            InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                            if (pCloud == null || (eObjType == eObjectType.Minion && eAtkType == eAttackType.Attack))
                                                            {
                                                                DamageTargetInfo pDamageTargetInfo = new DamageTargetInfo();
                                                                pDamageTargetInfo.m_pRangeInfo = pDamageTargetRangeInfo;
                                                                pDamageTargetInfo.m_pConditionInfo = pDamageTargetConditionInfo;
                                                                pDamageTargetInfo.m_nPriority = pDamageTargetConditionInfo.m_nPriority;

                                                                pDamageTargetInfo.m_nActionID = nActionID;
                                                                m_DamageTargetInfoList.Add(pDamageTargetInfo);
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
                                                    if (pSlotFixObject.GetObjectType() == pDamageTargetConditionInfo.m_eTargetType)
                                                    {
                                                        switch (pSlotFixObject.GetObjectType())
                                                        {
                                                            case eObjectType.EnemyColony:
                                                                {
                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                    if (pCloud == null)
                                                                    {
                                                                        DamageTargetInfo pDamageTargetInfo = new DamageTargetInfo();
                                                                        pDamageTargetInfo.m_pRangeInfo = pDamageTargetRangeInfo;
                                                                        pDamageTargetInfo.m_pConditionInfo = pDamageTargetConditionInfo;
                                                                        pDamageTargetInfo.m_nPriority = pDamageTargetConditionInfo.m_nPriority;

                                                                        pDamageTargetInfo.m_nActionID = nActionID;
                                                                        m_DamageTargetInfoList.Add(pDamageTargetInfo);
                                                                    }
                                                                }
                                                                break;

                                                            case eObjectType.Minion:
                                                            case eObjectType.MinionBoss:
                                                            case eObjectType.EnemyBoss:
                                                                {
                                                                    SlotFixObject_Unit pSlotFixObject_Unit = pSlotFixObject as SlotFixObject_Unit;

                                                                    if (pSlotFixObject_Unit != null && (pDamageTargetConditionInfo.m_eElement == eElement.All || pSlotFixObject_Unit.GetUnitInfo().m_eElement == pDamageTargetConditionInfo.m_eElement))
                                                                    {
                                                                        InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                        if (pCloud == null)
                                                                        {
                                                                            if (pDamageTargetConditionInfo.m_nUnitTableID == 0 || pDamageTargetConditionInfo.m_nUnitTableID == pSlotFixObject_Unit.GetUnitInfo().m_nID)
                                                                            {
                                                                                DamageTargetInfo pDamageTargetInfo = new DamageTargetInfo();
                                                                                pDamageTargetInfo.m_pRangeInfo = pDamageTargetRangeInfo;
                                                                                pDamageTargetInfo.m_pConditionInfo = pDamageTargetConditionInfo;
                                                                                pDamageTargetInfo.m_nPriority = pDamageTargetConditionInfo.m_nPriority;

                                                                                pDamageTargetInfo.m_nActionID = nActionID;
                                                                                m_DamageTargetInfoList.Add(pDamageTargetInfo);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                            case eObjectType.Character:
                                                                {
                                                                    SlotFixObject_PlayerCharacter pPlayerCharacter = pSlotFixObject as SlotFixObject_PlayerCharacter;

                                                                    if (pPlayerCharacter != null /*&& pPlayerCharacter.IsDead() == false*/ && (pDamageTargetConditionInfo.m_eElement == eElement.All || pPlayerCharacter.GetUnitInfo().m_eElement == pDamageTargetConditionInfo.m_eElement))
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
                                                                                            if (pDamageTargetConditionInfo.m_nUnitTableID == 0 || pDamageTargetConditionInfo.m_nUnitTableID == pPlayerCharacter.GetUnitInfo().m_nID)
                                                                                            {
                                                                                                DamageTargetInfo pDamageTargetInfo = new DamageTargetInfo();
                                                                                                pDamageTargetInfo.m_pRangeInfo = pDamageTargetRangeInfo;
                                                                                                pDamageTargetInfo.m_pConditionInfo = pDamageTargetConditionInfo;
                                                                                                pDamageTargetInfo.m_nPriority = pDamageTargetConditionInfo.m_nPriority;

                                                                                                pDamageTargetInfo.m_nActionID = nActionID;
                                                                                                m_DamageTargetInfoList.Add(pDamageTargetInfo);
                                                                                            }
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
                                                                                        if (pDamageTargetConditionInfo.m_nUnitTableID == 0 || pDamageTargetConditionInfo.m_nUnitTableID == pPlayerCharacter.GetUnitInfo().m_nID)
                                                                                        {
                                                                                            DamageTargetInfo pDamageTargetInfo = new DamageTargetInfo();
                                                                                            pDamageTargetInfo.m_pRangeInfo = pDamageTargetRangeInfo;
                                                                                            pDamageTargetInfo.m_pConditionInfo = pDamageTargetConditionInfo;
                                                                                            pDamageTargetInfo.m_nPriority = pDamageTargetConditionInfo.m_nPriority;

                                                                                            pDamageTargetInfo.m_nActionID = nActionID;
                                                                                            m_DamageTargetInfoList.Add(pDamageTargetInfo);
                                                                                        }
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
                                                                                            if (pDamageTargetConditionInfo.m_nUnitTableID == 0 || pDamageTargetConditionInfo.m_nUnitTableID == pPlayerCharacter.GetUnitInfo().m_nID)
                                                                                            {
                                                                                                DamageTargetInfo pDamageTargetInfo = new DamageTargetInfo();
                                                                                                pDamageTargetInfo.m_pRangeInfo = pDamageTargetRangeInfo;
                                                                                                pDamageTargetInfo.m_pConditionInfo = pDamageTargetConditionInfo;
                                                                                                pDamageTargetInfo.m_nPriority = pDamageTargetConditionInfo.m_nPriority;

                                                                                                pDamageTargetInfo.m_nActionID = nActionID;
                                                                                                m_DamageTargetInfoList.Add(pDamageTargetInfo);
                                                                                            }
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
                                                    if (pSlotFixObject.GetObjectType() == pDamageTargetConditionInfo.m_eTargetType)
                                                    {
                                                        switch (pSlotFixObject.GetObjectType())
                                                        {
                                                            case eObjectType.EnemyColony:
                                                                {
                                                                    InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                    if (pCloud == null)
                                                                    {
                                                                        DamageTargetInfo pDamageTargetInfo = new DamageTargetInfo();
                                                                        pDamageTargetInfo.m_pRangeInfo = pDamageTargetRangeInfo;
                                                                        pDamageTargetInfo.m_pConditionInfo = pDamageTargetConditionInfo;
                                                                        pDamageTargetInfo.m_nPriority = pDamageTargetConditionInfo.m_nPriority;

                                                                        pDamageTargetInfo.m_nActionID = nActionID;
                                                                        m_DamageTargetInfoList.Add(pDamageTargetInfo);
                                                                    }
                                                                }
                                                                break;

                                                            case eObjectType.Minion:
                                                            case eObjectType.MinionBoss:
                                                            case eObjectType.EnemyBoss:
                                                                {
                                                                    SlotFixObject_Unit pSlotFixObject_Unit = pSlotFixObject as SlotFixObject_Unit;

                                                                    if (pSlotFixObject_Unit != null && (pDamageTargetConditionInfo.m_eElement == eElement.All || pSlotFixObject_Unit.GetUnitInfo().m_eElement == pDamageTargetConditionInfo.m_eElement))
                                                                    {
                                                                        InGame_Cloud pCloud = pDataStack.m_pCloudManager.GetCloud(pTargetSlot, eOwner_Differ);
                                                                        if (pCloud == null)
                                                                        {
                                                                            if (pDamageTargetConditionInfo.m_nUnitTableID == 0 || pDamageTargetConditionInfo.m_nUnitTableID == pSlotFixObject_Unit.GetUnitInfo().m_nID)
                                                                            {
                                                                                DamageTargetInfo pDamageTargetInfo = new DamageTargetInfo();
                                                                                pDamageTargetInfo.m_pRangeInfo = pDamageTargetRangeInfo;
                                                                                pDamageTargetInfo.m_pConditionInfo = pDamageTargetConditionInfo;
                                                                                pDamageTargetInfo.m_nPriority = pDamageTargetConditionInfo.m_nPriority;

                                                                                pDamageTargetInfo.m_nActionID = nActionID;
                                                                                m_DamageTargetInfoList.Add(pDamageTargetInfo);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                break;

                                                            case eObjectType.Character:
                                                                {
                                                                    SlotFixObject_PlayerCharacter pPlayerCharacter = pSlotFixObject as SlotFixObject_PlayerCharacter;

                                                                    if (pPlayerCharacter != null /*&& pPlayerCharacter.IsDead() == false*/ && (pDamageTargetConditionInfo.m_eElement == eElement.All || pPlayerCharacter.GetUnitInfo().m_eElement == pDamageTargetConditionInfo.m_eElement))
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
                                                                                            if (pDamageTargetConditionInfo.m_nUnitTableID == 0 || pDamageTargetConditionInfo.m_nUnitTableID == pPlayerCharacter.GetUnitInfo().m_nID)
                                                                                            {
                                                                                                DamageTargetInfo pDamageTargetInfo = new DamageTargetInfo();
                                                                                                pDamageTargetInfo.m_pRangeInfo = pDamageTargetRangeInfo;
                                                                                                pDamageTargetInfo.m_pConditionInfo = pDamageTargetConditionInfo;
                                                                                                pDamageTargetInfo.m_nPriority = pDamageTargetConditionInfo.m_nPriority;

                                                                                                pDamageTargetInfo.m_nActionID = nActionID;
                                                                                                m_DamageTargetInfoList.Add(pDamageTargetInfo);
                                                                                            }
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
                                                                                        if (pDamageTargetConditionInfo.m_nUnitTableID == 0 || pDamageTargetConditionInfo.m_nUnitTableID == pPlayerCharacter.GetUnitInfo().m_nID)
                                                                                        {
                                                                                            DamageTargetInfo pDamageTargetInfo = new DamageTargetInfo();
                                                                                            pDamageTargetInfo.m_pRangeInfo = pDamageTargetRangeInfo;
                                                                                            pDamageTargetInfo.m_pConditionInfo = pDamageTargetConditionInfo;
                                                                                            pDamageTargetInfo.m_nPriority = pDamageTargetConditionInfo.m_nPriority;

                                                                                            pDamageTargetInfo.m_nActionID = nActionID;
                                                                                            m_DamageTargetInfoList.Add(pDamageTargetInfo);
                                                                                        }
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
                                                                                            if (pDamageTargetConditionInfo.m_nUnitTableID == 0 || pDamageTargetConditionInfo.m_nUnitTableID == pPlayerCharacter.GetUnitInfo().m_nID)
                                                                                            {
                                                                                                DamageTargetInfo pDamageTargetInfo = new DamageTargetInfo();
                                                                                                pDamageTargetInfo.m_pRangeInfo = pDamageTargetRangeInfo;
                                                                                                pDamageTargetInfo.m_pConditionInfo = pDamageTargetConditionInfo;
                                                                                                pDamageTargetInfo.m_nPriority = pDamageTargetConditionInfo.m_nPriority;

                                                                                                pDamageTargetInfo.m_nActionID = nActionID;
                                                                                                m_DamageTargetInfoList.Add(pDamageTargetInfo);
                                                                                            }
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
            }
        }

        if (eRangeType == eRangeType.All)
        {
            int nRandomDamageTargetRangeAmount = pDamageTargetRangeInfo_ForAll.m_nRandomDamageTargetAmount;

            if (pActionLevelUpInfo != null && pActionLevelUpInfo.m_nActionLevel != 1)
            {
                nRandomDamageTargetRangeAmount = pActionLevelUpInfo.m_nChange_RandomDamageTargetAmount;
            }

            if (m_DamageTargetInfoList.Count > nRandomDamageTargetRangeAmount)
            {
                Helper.ShuffleList(m_DamageTargetInfoList, EspressoInfo.Instance.m_nDamageTargetRandomSeed);
                Helper.ShuffleList(m_DamageTargetInfoList, EspressoInfo.Instance.m_nDamageTargetRandomSeed);
                m_DamageTargetInfoList.Sort(CompareDamageTarget);

                List <DamageTargetInfo> damageTargetInfoList = new List<DamageTargetInfo>();

                for (int i = 0; i < nRandomDamageTargetRangeAmount; ++i)
                {
                    damageTargetInfoList.Add(m_DamageTargetInfoList[i]);
                }

                m_DamageTargetInfoList.Clear();

                for (int i = 0; i < nRandomDamageTargetRangeAmount; ++i)
                {
                    m_DamageTargetInfoList.Add(damageTargetInfoList[i]);
                }
            }
        }
    }

    private int CompareDamageTarget(DamageTargetInfo A, DamageTargetInfo B)
    {
        return B.m_nPriority.CompareTo(A.m_nPriority);
    }

    public int GetNumDamageTargetInfo()
    {
        return m_DamageTargetInfoList.Count;
    }

    public DamageTargetInfo GetDamageTargetInfo_byIndex(int nIndex)
    {
        if (nIndex < 0 || m_DamageTargetInfoList.Count <= nIndex)
        {
            return null;
        }
        return m_DamageTargetInfoList[nIndex];
    }
}