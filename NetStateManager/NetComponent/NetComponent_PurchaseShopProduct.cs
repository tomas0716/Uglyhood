using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;
using sg.protocol.common;

public class NetComponent_PurchaseShopProduct : TNetComponent_SuccessOrFail<EventArg_PurchaseShopProduct, EventArg_Null, Error>
{
    public NetComponent_PurchaseShopProduct()
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

    public override void OnEvent(EventArg_PurchaseShopProduct Arg)
    {
        NetLog.Log("NetComponent_PurchaseShopProduct : OnEvent");

        switch (Arg.m_eIAPContentType)
        {
            case eIAPContentType.Shop:
                {
                    MsgReqPurchaseShopProduct pReq = new MsgReqPurchaseShopProduct();
                    pReq.Uid = MyInfo.Instance.m_nUserIndex;
                    pReq.I_BuySeq = Arg.m_nBuySeq;
                    pReq.I_Store = Arg.m_nStore;
                    pReq.S_PackageName = Arg.m_strPackageName;
                    pReq.S_ProductId = Arg.m_strProductID;
                    pReq.S_Token = Arg.m_strToken;

                    SendPacket<MsgReqPurchaseShopProduct, MsgAnsPurchaseShopProduct>(pReq, RecvPacket_PurchaseShopProductSuccess, RecvPacket_PurchaseShopProductFailure);
                }
                break;

            case eIAPContentType.Package:
                {
                }
                break;

            case eIAPContentType.BattlePassTicket:
                {
                    MsgReqPurchaseBattlePass pReq = new MsgReqPurchaseBattlePass();
                    pReq.Uid = MyInfo.Instance.m_nUserIndex;
                    pReq.I_BuySeq = Arg.m_nBuySeq;
                    pReq.I_Store = Arg.m_nStore;
                    pReq.S_PackageName = Arg.m_strPackageName;
                    pReq.S_ProductId = Arg.m_strProductID;
                    pReq.S_Token = Arg.m_strToken;

                    SendPacket<MsgReqPurchaseBattlePass, MsgAnsPurchaseBattlePass>(pReq, RecvPacket_PurchaseBattlePassProductSuccess, RecvPacket_PurchaseBattlePassProductFailure);
                }
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Shop

    public void RecvPacket_PurchaseShopProductSuccess(MsgReqPurchaseShopProduct pReq, MsgAnsPurchaseShopProduct pAns)
    {
        NetLog.Log("NetComponent_PurchaseShopProduct : RecvPacket_PurchaseShopProductSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_PurchaseShopProductFailure(MsgReqPurchaseShopProduct pReq, Error error)
    {
        NetLog.Log("NetComponent_PurchaseShopProduct : RecvPacket_PurchaseShopProductFailure");

        GetFailureEvent().OnEvent(error);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// BattlePass

    public void RecvPacket_PurchaseBattlePassProductSuccess(MsgReqPurchaseBattlePass pReq, MsgAnsPurchaseBattlePass pAns)
    {
        NetLog.Log("NetComponent_PurchaseShopProduct : RecvPacket_PurchaseShopProductSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_PurchaseBattlePassProductFailure(MsgReqPurchaseBattlePass pReq, Error error)
    {
        NetLog.Log("NetComponent_PurchaseShopProduct : RecvPacket_PurchaseShopProductFailure");

        GetFailureEvent().OnEvent(error);
    }
}
