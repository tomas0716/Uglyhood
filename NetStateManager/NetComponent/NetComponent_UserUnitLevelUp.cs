using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_UserUnitLevelUp : TNetComponent_SuccessOrFail<EventArg_UserUnitLevelUp, EventArg_Null, Error>
{
    private int m_nUnitID = 0;

    public NetComponent_UserUnitLevelUp()
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

    public override void OnEvent(EventArg_UserUnitLevelUp Arg)
    {
        NetLog.Log("NetComponent_UserUnitLevelUp : OnEvent");

        m_nUnitID = Arg.m_nUnitID;

        MsgReqUserUnitLevelUp pReq = new MsgReqUserUnitLevelUp();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_UnitId = m_nUnitID;

        SendPacket<MsgReqUserUnitLevelUp, MsgAnsUserUnitLevelUp>(pReq, RecvPacket_UserUnitLevelUpSuccess, RecvPacket_UserUnitLevelUpFailure);
    }

    public void RecvPacket_UserUnitLevelUpSuccess(MsgReqUserUnitLevelUp pReq, MsgAnsUserUnitLevelUp pAns)
    {
        NetLog.Log("NetComponent_UserUnitLevelUp : RecvPacket_SetTeamDeckSuccess");

        if (pAns.St_UserUnit.I_UnitLevel != pAns.I_AfterLevel)
        {
            ErrorLog.Log("UserUnitLevelUp Level Miss Mtach");
        }

        if (pAns.St_UserUnit.I_UnitId != m_nUnitID)
        {
            ErrorLog.Log("UserUnitLevelUp UnitID Miss Mtach");
        }

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

            if (pCharacterInvenItemInfo.m_nUniqueID != pAns.St_UserUnit.I_Seq)
            {
                ErrorLog.Log("UserUnitLevelUp UnitSeq Miss Mtach");
            }
        }
        else
        {
            ErrorLog.Log("UserUnitLevelUp : Not Find GetUniqueID");
        }

        InventoryInfoManager.Instance.Reset_Editing_CharacterInvenInfo();

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_UserUnitLevelUpFailure(MsgReqUserUnitLevelUp pReq, Error pError)
    {
        NetLog.Log("NetComponent_UserUnitLevelUp : RecvPacket_SetTeamDeckFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
