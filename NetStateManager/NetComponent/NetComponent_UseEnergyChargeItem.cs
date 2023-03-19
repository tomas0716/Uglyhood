using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_UseEnergyChargeItem : TNetComponent_SuccessOrFail<EventArg_UseEnergyChargeItem, EventArg_Null, Error>
{
    public NetComponent_UseEnergyChargeItem()
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

    public override void OnEvent(EventArg_UseEnergyChargeItem Arg)
    {
        NetLog.Log("NetComponent_UseEnergyChargeItem : OnEvent");

        MsgReqUseApChargeItem pReq = new MsgReqUseApChargeItem();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_ItemId = Arg.m_nItemID;

        SendPacket<MsgReqUseApChargeItem, MsgAnsUseApChargeItem>(pReq, RecvPacket_UseEnergyChargeItemSuccess, RecvPacket_UseEnergyChargeItemFailure);
    }

    public void RecvPacket_UseEnergyChargeItemSuccess(MsgReqUseApChargeItem pReq, MsgAnsUseApChargeItem pAns)
    {
        NetLog.Log("NetComponent_UseEnergyChargeItem : RecvPacket_UseEnergyChargeItemSuccess");

        //EventDelegateManager.Instance.OnLobby_RefreshEnergy();

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_UseEnergyChargeItemFailure(MsgReqUseApChargeItem pReq, Error error)
    {
        NetLog.Log("NetComponent_UseEnergyChargeItem : RecvPacket_UseEnergyChargeItemFailure");

        GetFailureEvent().OnEvent(error);
    }
}
