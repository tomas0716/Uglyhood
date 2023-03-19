using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_UserUnitPassiveSkillUp : TNetComponent_SuccessOrFail<EventArg_UserUnitPassiveSkillUp, EventArg_Null, Error>
{
    public NetComponent_UserUnitPassiveSkillUp()
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

    public override void OnEvent(EventArg_UserUnitPassiveSkillUp Arg)
    {
        NetLog.Log("NetComponent_UserUnitPassiveSkillUp : OnEvent");

        MsgReqUserUnitPassiveSkillUp pReq = new MsgReqUserUnitPassiveSkillUp();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_UnitId = Arg.m_nUnitID;

        SendPacket<MsgReqUserUnitPassiveSkillUp, MsgAnsUserUnitPassiveSkillUp>(pReq, RecvPacket_UserUnitPassiveSkillUpSuccess, RecvPacket_UserUnitPassiveSkillUpFailure);
    }

    public void RecvPacket_UserUnitPassiveSkillUpSuccess(MsgReqUserUnitPassiveSkillUp pReq, MsgAnsUserUnitPassiveSkillUp pAns)
    {
        NetLog.Log("NetComponent_UserUnitPassiveSkillUp : RecvPacket_UserUnitPassiveSkillUpSuccess");

        CharacterInvenItemInfo pCharacterInvenItemInfo = InventoryInfoManager.Instance.m_pCharacterInvenInfo.GetInvenItem_byTableID(pAns.St_UserUnit.I_UnitId);

        if (pCharacterInvenItemInfo != null)
        {
            pCharacterInvenItemInfo.m_nTableID = pAns.St_UserUnit.I_UnitId;
            pCharacterInvenItemInfo.m_nLevel = pAns.St_UserUnit.I_UnitLevel;
            pCharacterInvenItemInfo.m_nMaxHP = pAns.St_UserUnit.I_UnitMaxHp;
            pCharacterInvenItemInfo.m_nMaxSP = pAns.St_UserUnit.I_UnitMaxSp;
            pCharacterInvenItemInfo.m_nSp_ChargePerBlock = pAns.St_UserUnit.I_UnitChargePerBlock;
            pCharacterInvenItemInfo.m_nATK = pAns.St_UserUnit.I_UnitAttack;
            pCharacterInvenItemInfo.m_nAction_Level = pAns.St_UserUnit.I_SkillLevel;
            pCharacterInvenItemInfo.m_nPower = pAns.St_UserUnit.I_UnitCombat;
            pCharacterInvenItemInfo.m_nPassive_Level = pAns.St_UserUnit.I_UnitPassiveSkillLevel;

            if (pCharacterInvenItemInfo.m_nUniqueID != pAns.St_UserUnit.I_Seq)
            {
                ErrorLog.Log("UserUnitLevelUp UnitSeq Miss Mtach");
            }

            pCharacterInvenItemInfo.m_nUnit_AddTime = pAns.St_UserUnit.I_UnitAddTime;
        }
        else
        {
            ErrorLog.Log("UserUnitLevelUp : Not Find GetUniqueID");
        }

        InventoryInfoManager.Instance.Reset_Editing_CharacterInvenInfo();

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_UserUnitPassiveSkillUpFailure(MsgReqUserUnitPassiveSkillUp pReq, Error error)
    {
        NetLog.Log("NetComponent_UserUnitPassiveSkillUp : RecvPacket_UserUnitPassiveSkillUpFailur");

        GetFailureEvent().OnEvent(error);
    }
}
