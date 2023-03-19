using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_UserUnitActionLevelUp : TNetComponent_SuccessOrFail<EventArg_UserUnitActionLevelUp, EventArg_Null, Error>
{
    private int m_nUnitID   = 0;
    private int m_nActionID = 0;

    public NetComponent_UserUnitActionLevelUp()
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

    public override void OnEvent(EventArg_UserUnitActionLevelUp Arg)
    {
        NetLog.Log("NetComponent_UserUnitActionLevelUp : OnEvent");

        m_nUnitID = Arg.m_nUnitID;
        m_nActionID = Arg.m_nActionID;

        MsgReqUserUnitActionLevelUp pReq = new MsgReqUserUnitActionLevelUp();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_UnitId = m_nUnitID;
        pReq.I_ActionId = m_nActionID;

        SendPacket<MsgReqUserUnitActionLevelUp, MsgAnsUserUnitActionLevelUp>(pReq, RecvPacket_UserUnitActionLevelUpSuccess, RecvPacket_UserUnitActionLevelUpFailure);
    }

    public void RecvPacket_UserUnitActionLevelUpSuccess(MsgReqUserUnitActionLevelUp pReq, MsgAnsUserUnitActionLevelUp pAns)
    {
        NetLog.Log("NetComponent_UserUnitActionLevelUp : RecvPacket_UserUnitActionLevelUpSuccess");

        if (pAns.I_UnitId != m_nUnitID)
        {
            ErrorLog.Log("UserUnitActionLevelUp UnitID Miss Mtach");
        }

        if (m_nActionID != pAns.I_ActionId)
        {
            ErrorLog.Log("UserUnitActionLevelUp ActionID Miss Mtach");
        }

        CharacterInvenItemInfo pCharacterInvenItemInfo = InventoryInfoManager.Instance.m_pCharacterInvenInfo.GetInvenItem_byTableID(m_nUnitID);

        if (pCharacterInvenItemInfo != null)
        {
            pCharacterInvenItemInfo.m_nAction_Level = pAns.I_AfterLevel;
        }
        else
        {
            ErrorLog.Log("UserUnitAcionLevelUp : Not Find GetUniqueID");
        }

        InventoryInfoManager.Instance.Reset_Editing_CharacterInvenInfo();

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_UserUnitActionLevelUpFailure(MsgReqUserUnitActionLevelUp pReq, Error pError)
    {
        NetLog.Log("NetComponent_UserUnitActionLevelUp : RecvPacket_UserUnitActionLevelUpFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
