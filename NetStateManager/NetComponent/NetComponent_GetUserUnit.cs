using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetUserUnit : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_GetUserUnit()
    {
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
    }

    public override void LateUpdate()
    {
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        NetLog.Log("NetComponent_GetUserUnit : OnEvent");

        MsgReqGetUserUnit pReq = new MsgReqGetUserUnit();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqGetUserUnit, MsgAnsGetUserUnit>(pReq, RecvPacket_GetUserUnitSuccess, RecvPacket_GetUserUnitFailure);
    }

    public void RecvPacket_GetUserUnitSuccess(MsgReqGetUserUnit pReq, MsgAnsGetUserUnit pAns)
    {
        NetLog.Log("NetComponent_GetUserUnit : RecvPacket_GetUserUnitSuccess");

        InventoryInfoManager.Instance.m_pCharacterInvenInfo.ClearAll();

        foreach (KeyValuePair<int, UserUnit> item in pAns.M_UserUnit)
        {
            //int nMaxHP, nMaxSP, nSP_ChargePerBlock, nATK, nCombat;
            //ExcelDataHelper.GetUnitStat(item.Value.I_UnitId, item.Value.I_UnitLevel, out nMaxHP, out nMaxSP, out nSP_ChargePerBlock, out nATK, out nCombat);

            //if (item.Value.I_UnitMaxHp != nMaxHP || item.Value.I_UnitMaxSp != nMaxSP || item.Value.I_UnitChargePerBlock != nSP_ChargePerBlock ||
            //    item.Value.I_UnitAttack != nATK /*|| item.Value.I_UnitCombat != nCombat*/)
            //{
            //    ErrorLog.Log("GetUserUnit Miss Mtach Stats");
            //    ErrorLog.Log("Table ID : " + item.Value.I_UnitId.ToString() + ", Level : " + item.Value.I_UnitLevel.ToString());
            //    ErrorLog.Log("ServerData : MaxHP > " + item.Value.I_UnitMaxHp.ToString() + ", MaxSP > " + item.Value.I_UnitMaxSp.ToString() + ", CPB > " + item.Value.I_UnitChargePerBlock.ToString() + ", ATK > " + item.Value.I_UnitAttack.ToString() + ", Combat > " + item.Value.I_UnitCombat.ToString());
            //    ErrorLog.Log("ClientData : MaxHP > " + nMaxHP.ToString() + ", MaxSP > " + nMaxSP.ToString() + ", CPB > " + nSP_ChargePerBlock.ToString() + ", ATK > " + nATK.ToString() + ", Combat > " + nCombat.ToString());
            //}

            CharacterInvenItemInfo pCharacterInvenItemInfo = new CharacterInvenItemInfo();
            pCharacterInvenItemInfo.m_nTableID = item.Value.I_UnitId;
            pCharacterInvenItemInfo.m_nUniqueID = item.Value.I_Seq;
            pCharacterInvenItemInfo.m_nDB_Seq = item.Key;
            pCharacterInvenItemInfo.m_nLevel = item.Value.I_UnitLevel;
            pCharacterInvenItemInfo.m_nMaxHP = item.Value.I_UnitMaxHp;
            pCharacterInvenItemInfo.m_nMaxSP = item.Value.I_UnitMaxSp;
            pCharacterInvenItemInfo.m_nSp_ChargePerBlock = item.Value.I_UnitChargePerBlock;
            pCharacterInvenItemInfo.m_nATK = item.Value.I_UnitAttack;
            pCharacterInvenItemInfo.m_nAction_Level = item.Value.I_SkillLevel;
            if(pCharacterInvenItemInfo.m_nAction_Level > 10)
                pCharacterInvenItemInfo.m_nAction_Level = 10;
            pCharacterInvenItemInfo.m_nPassive_Level = item.Value.I_UnitPassiveSkillLevel;

            if (pCharacterInvenItemInfo.m_nPassive_Level > 10)
            {
                pCharacterInvenItemInfo.m_nPassive_Level = 10;
            }
            
            //if(pCharacterInvenItemInfo.m_nPassive_Level < 5)
            //{
            //    pCharacterInvenItemInfo.m_nPassive_Level = 5;
            //}

            pCharacterInvenItemInfo.m_nPower = item.Value.I_UnitCombat;
            pCharacterInvenItemInfo.m_nUnit_AddTime = item.Value.I_UnitAddTime;

#if ExcelData && UNITY_EDITOR
            ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(item.Value.I_UnitId);
            ExcelData_Unit_LevelUpInfo pUnitLevelUpInfo = ExcelDataManager.Instance.m_pUnit_LevelUp.GetUnitLevelUpInfo_byKeys(eUnitType.Character, pUnitInfo.m_nRank, pUnitInfo.m_nElementTableID, pUnitInfo.m_nClassTableID, item.Value.I_UnitLevel);

            pCharacterInvenItemInfo.m_nMaxHP = (int)System.Math.Round(((float)pUnitInfo.m_nMaxHP * (float)pUnitLevelUpInfo.m_nChange_MaxHP_Rate) / 100.0f, System.MidpointRounding.AwayFromZero);
            pCharacterInvenItemInfo.m_nMaxSP = (int)System.Math.Round(((float)pUnitInfo.m_nMaxSP * (float)pUnitLevelUpInfo.m_nChange_MaxSP_Rate) / 100.0f, System.MidpointRounding.AwayFromZero);
            pCharacterInvenItemInfo.m_nSp_ChargePerBlock = (int)System.Math.Round(((float)pUnitInfo.m_nSP_ChargePerBlock * (float)pUnitLevelUpInfo.m_nChange_SPCharge_Rate) / 100.0f, System.MidpointRounding.AwayFromZero);
            pCharacterInvenItemInfo.m_nATK = (int)System.Math.Round(((float)pUnitInfo.m_nATK * (float)pUnitLevelUpInfo.m_nChange_ATK_Rate) / 100.0f, System.MidpointRounding.AwayFromZero);

            double nPower = 0;

            if (pUnitInfo.m_nMaxSP != 0)
            {
                nPower = (pCharacterInvenItemInfo.m_nATK * 2.5) + ((float)pCharacterInvenItemInfo.m_nMaxHP * 0.28) + ((float)pCharacterInvenItemInfo.m_nMaxSP * 0.35) + (10 / (float)pCharacterInvenItemInfo.m_nMaxSP / (float)pCharacterInvenItemInfo.m_nSp_ChargePerBlock * 3) + (18 * ((float)pUnitInfo.m_nRank - 1));
            }
            else
            {
                nPower = (pCharacterInvenItemInfo.m_nATK * 2.5) + ((float)pCharacterInvenItemInfo.m_nMaxHP * 0.28) + ((float)pCharacterInvenItemInfo.m_nMaxSP * 0.35) + (18 * ((float)pUnitInfo.m_nRank - 1));
            }

            pCharacterInvenItemInfo.m_nPower = (int)System.Math.Round(nPower, System.MidpointRounding.AwayFromZero);
#endif

            InventoryInfoManager.Instance.m_pCharacterInvenInfo.AddInvenItemInfo(pCharacterInvenItemInfo);
        }

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetUserUnitFailure(MsgReqGetUserUnit pReq, Error pError)
    {
        NetLog.Log("NetComponent_GetUserUnit : RecvPacket_GetUserUnitFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
