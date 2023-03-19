using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_PurchasePackageShopProductCancel : TNetComponent_SuccessOrFail<EventArg_PurchasePackageShopProductCancel, EventArg_Null, Error>
{
    public NetComponent_PurchasePackageShopProductCancel()
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

    public override void OnEvent(EventArg_PurchasePackageShopProductCancel Arg)
    {
        NetLog.Log("NetComponent_PurchasePackageShopProductCancel : OnEvent");

        MsgReqPurchasePackageShopProductCancel pReq = new MsgReqPurchasePackageShopProductCancel();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_BuySeq = Arg.m_nBuySeq;

        SendPacket<MsgReqPurchasePackageShopProductCancel, MsgAnsPurchasePackageShopProductCancel>(pReq, RecvPacket_PurchasePackageShopProductCancelSuccess, RecvPacket_PurchasePackageShopProductCancelFailure);
    }

    public void RecvPacket_PurchasePackageShopProductCancelSuccess(MsgReqPurchasePackageShopProductCancel pReq, MsgAnsPurchasePackageShopProductCancel pAns)
    {
        NetLog.Log("NetComponent_PurchasePackageShopProductCancel : RecvPacket_PurchasePackageShopProductCancelSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_PurchasePackageShopProductCancelFailure(MsgReqPurchasePackageShopProductCancel pReq, Error error)
    {
        NetLog.Log("NetComponent_PurchasePackageShopProductCancel : RecvPacket_PurchasePackageShopProductCancelFailure");

        GetFailureEvent().OnEvent(error);
    }
}
