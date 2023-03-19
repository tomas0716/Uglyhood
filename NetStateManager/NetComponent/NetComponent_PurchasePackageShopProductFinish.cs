using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_PurchasePackageShopProductFinish : TNetComponent_SuccessOrFail<EventArg_PurchasePackageShopProductFinish, EventArg_Null, Error>
{
    public NetComponent_PurchasePackageShopProductFinish()
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

    public override void OnEvent(EventArg_PurchasePackageShopProductFinish Arg)
    {
        NetLog.Log("NetComponent_PurchasePackageShopProductFinish : OnEvent");

        MsgReqPurchasePackageShopProductFinish pReq = new MsgReqPurchasePackageShopProductFinish();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_BuySeq = Arg.m_nBuySeq;

        SendPacket<MsgReqPurchasePackageShopProductFinish, MsgAnsPurchasePackageShopProductFinish>(pReq, RecvPacket_PurchasePackageShopProductFinishSuceess, RecvPacket_PurchasePackageShopProductFinishFailure);
    }

    public void RecvPacket_PurchasePackageShopProductFinishSuceess(MsgReqPurchasePackageShopProductFinish pReq, MsgAnsPurchasePackageShopProductFinish pAns)
    {
        NetLog.Log("NetComponent_PurchasePackageShopProductFinish : RecvPacket_PurchasePackageShopProductFinishSuceess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_PurchasePackageShopProductFinishFailure(MsgReqPurchasePackageShopProductFinish pReq, Error error)
    {
        NetLog.Log("NetComponent_PurchasePackageShopProductFinish : RecvPacket_PurchasePackageShopProductFinishFailure");

        GetFailureEvent().OnEvent(error);
    }
}
