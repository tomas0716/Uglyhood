using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_NotPurchasePackageShopProduct : TNetComponent_SuccessOrFail<EventArg_NotPurchasePackageShopProduct, EventArg_Null, Error>
{
    public NetComponent_NotPurchasePackageShopProduct()
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

    public override void OnEvent(EventArg_NotPurchasePackageShopProduct Arg)
    {
        NetLog.Log("NetComponent_NotPurchasePackageShopProduct : OnEvent");

        MsgReqNotPurchasePackageShopProduct pReq = new MsgReqNotPurchasePackageShopProduct();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_BuySeq = Arg.m_nBuySeq;

        SendPacket<MsgReqNotPurchasePackageShopProduct, MsgAnsNotPurchasePackageShopProduct>(pReq, RecvPacket_NotPurchasePackageShopProductSuccess, RecvPacket_NotPurchasePackageShopProductFailure);
    }

    public void RecvPacket_NotPurchasePackageShopProductSuccess(MsgReqNotPurchasePackageShopProduct pReq, MsgAnsNotPurchasePackageShopProduct pAns)
    {
        NetLog.Log("NetComponent_NotPurchasePackageShopProduct : RecvPacket_NotPurchasePackageShopProductSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_NotPurchasePackageShopProductFailure(MsgReqNotPurchasePackageShopProduct pReq, Error error)
    {
        NetLog.Log("NetComponent_NotPurchasePackageShopProduct : RecvPacket_NotPurchasePackageShopProductFailure");

        GetFailureEvent().OnEvent(error);
    }
}
