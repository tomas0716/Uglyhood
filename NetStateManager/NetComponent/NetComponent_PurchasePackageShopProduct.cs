using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_PurchasePackageShopProduct : TNetComponent_SuccessOrFail<EventArg_PurchasePackageShopProduct, EventArg_Null, Error>
{
    public NetComponent_PurchasePackageShopProduct()
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

    public override void OnEvent(EventArg_PurchasePackageShopProduct Arg)
    {
        NetLog.Log("NetComponent_PurchasePackageShopProduct : OnEvent");

        MsgReqPurchasePackageShopProduct pReq = new MsgReqPurchasePackageShopProduct();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_BuySeq = Arg.m_nBuySeq;
        pReq.I_Store = Arg.m_nStore;
        pReq.S_PackageName = Arg.m_strPackageName;
        pReq.S_ProductId = Arg.m_strProductID;
        pReq.S_Token = Arg.m_strToken;

        SendPacket<MsgReqPurchasePackageShopProduct, MsgAnsPurchasePackageShopProduct>(pReq, RecvPacket_PurchasePackageShopProductSuccess, RecvPacket_PurchasePackageShopProductFailure);
    }

    public void RecvPacket_PurchasePackageShopProductSuccess(MsgReqPurchasePackageShopProduct pReq, MsgAnsPurchasePackageShopProduct pAns)
    {
        NetLog.Log("NetComponent_PurchasePackageShopProduct : RecvPacket_PurchasePackageShopProductSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_PurchasePackageShopProductFailure(MsgReqPurchasePackageShopProduct pReq, Error error)
    {
        NetLog.Log("NetComponent_PurchasePackageShopProduct : RecvPacket_PurchasePackageShopProductFailure");

        GetFailureEvent().OnEvent(error);
    }
}
