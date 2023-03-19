using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_PurchasePackageShopProductStart : TNetComponent_SuccessOrFail<EventArg_PurchasePackageShopProductStart, EventArg_Null, Error>
{
    private int m_nBuySeq_ForNOT = 0;

    public NetComponent_PurchasePackageShopProductStart()
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

    private void ClearBuySeq()
    {
        m_nBuySeq_ForNOT = 0;
    }

    public int GetBuySeq()
    {
        return m_nBuySeq_ForNOT;
    }

    public override void OnEvent(EventArg_PurchasePackageShopProductStart Arg)
    {
        NetLog.Log("NetComponent_PurchasePackageShopProductStart : OnEvent");

        MsgReqPurchasePackageShopProductStart pReq = new MsgReqPurchasePackageShopProductStart();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_Id = Arg.m_nID;

        SendPacket<MsgReqPurchasePackageShopProductStart, MsgAnsPurchasePackageShopProductStart>(pReq, RecvPacket_PurchasePackageShopProductStartSuccess, RecvPacket_PurchasePackageShopProductStartFailure);
    }

    public void RecvPacket_PurchasePackageShopProductStartSuccess(MsgReqPurchasePackageShopProductStart pReq, MsgAnsPurchasePackageShopProductStart pAns)
    {
        NetLog.Log("NetComponent_PurchasePackageShopProductStart : RecvPacket_PurchasePackageShopProductStartSuccess");

        ClearBuySeq();
        m_nBuySeq_ForNOT = pAns.I_BuySeq;

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_PurchasePackageShopProductStartFailure(MsgReqPurchasePackageShopProductStart pReq, Error error)
    {
        NetLog.Log("NetComponent_PurchasePackageShopProductStart : RecvPacket_PurchasePackageShopProductStartFailure");

        GetFailureEvent().OnEvent(error);
    }
}
