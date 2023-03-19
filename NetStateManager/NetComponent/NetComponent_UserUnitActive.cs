using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_UserUnitActive : TNetComponent_SuccessOrFail<EventArg_UserUnitActive, EventArg_Null, Error>
{
    private int m_nUnitID = 0;

    public NetComponent_UserUnitActive()
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

    public override void OnEvent(EventArg_UserUnitActive Arg)
    {
        NetLog.Log("NetComponent_UserUnitActive : OnEvent");

        m_nUnitID = Arg.m_nUnitID;

        MsgReqUserUnitActive pReq = new MsgReqUserUnitActive();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_UnitId = m_nUnitID;

        SendPacket<MsgReqUserUnitActive, MsgAnsUserUnitActive>(pReq, RecvPacket_UserUnitActiveSuccess, RecvPacket_UserUnitActiveFailure);
    }

    public void RecvPacket_UserUnitActiveSuccess(MsgReqUserUnitActive pReq, MsgAnsUserUnitActive pAns)
    {
        NetLog.Log("NetComponent_UserUnitActive : RecvPacket_UserUnitActiveSuccess");

        CharacterInvenItemInfo pCharacterInvenItemInfo = new CharacterInvenItemInfo();
        pCharacterInvenItemInfo.m_nTableID = pAns.St_UserUnit.I_UnitId;
        pCharacterInvenItemInfo.m_nUniqueID = pAns.St_UserUnit.I_Seq;
        pCharacterInvenItemInfo.m_nLevel = pAns.St_UserUnit.I_UnitLevel;
        pCharacterInvenItemInfo.m_nMaxHP = pAns.St_UserUnit.I_UnitMaxHp;
        pCharacterInvenItemInfo.m_nMaxSP = pAns.St_UserUnit.I_UnitMaxSp;
        pCharacterInvenItemInfo.m_nSp_ChargePerBlock = pAns.St_UserUnit.I_UnitChargePerBlock;
        pCharacterInvenItemInfo.m_nATK = pAns.St_UserUnit.I_UnitAttack;
        pCharacterInvenItemInfo.m_nAction_Level = pAns.St_UserUnit.I_SkillLevel;
        pCharacterInvenItemInfo.m_nPower = pAns.St_UserUnit.I_UnitCombat;

        InventoryInfoManager.Instance.m_pCharacterInvenInfo.AddInvenItemInfo(pCharacterInvenItemInfo);
        
        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_UserUnitActiveFailure(MsgReqUserUnitActive pReq, Error pError)
    {
        NetLog.Log("NetComponent_UserUnitActive : RecvPacket_UserUnitActiveFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
