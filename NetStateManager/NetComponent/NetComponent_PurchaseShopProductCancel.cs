using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;
using sg.protocol.common;

public class NetComponent_PurchaseShopProductCancel : TNetComponent_SuccessOrFail<EventArg_PurchaseShopProductCancel, EventArg_Null, Error>
{
    public NetComponent_PurchaseShopProductCancel()
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

    public override void OnEvent(EventArg_PurchaseShopProductCancel Arg)
    {
        NetLog.Log("NetComponent_PurchaseShopProductCancel : OnEvent");

        switch (Arg.m_eIAPContentType)
        {
            case eIAPContentType.Shop:
                {
                    MsgReqPurchaseShopProductCancel pReq = new MsgReqPurchaseShopProductCancel();
                    pReq.Uid = MyInfo.Instance.m_nUserIndex;
                    pReq.I_BuySeq = Arg.m_nBuySeq;

                    SendPacket<MsgReqPurchaseShopProductCancel, MsgAnsPurchaseShopProductCancel>(pReq, RecvPacket_PurchaseShopProductCancelSuccess, RecvPacket_PurchaseShopProductCancelFailure);
                }
                break;

            case eIAPContentType.Package:
                {
                }
                break;

            case eIAPContentType.BattlePassTicket:
                {
                    MsgReqPurchaseBattlePassCancel pReq = new MsgReqPurchaseBattlePassCancel();
                    pReq.Uid = MyInfo.Instance.m_nUserIndex;
                    pReq.I_BuySeq = Arg.m_nBuySeq;

                    SendPacket<MsgReqPurchaseBattlePassCancel, MsgAnsPurchaseBattlePassCancel>(pReq, RecvPacket_PurchaseBattlePassCancelSuccess, RecvPacket_PurchaseBattlePassCancelFailure);
                }
                break;		
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Shop

    public void RecvPacket_PurchaseShopProductCancelSuccess(MsgReqPurchaseShopProductCancel pReq, MsgAnsPurchaseShopProductCancel pAns)
    {
        NetLog.Log("NetComponent_PurchaseShopProductCancel : RecvPacket_PurchaseShopProductCancelSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_PurchaseShopProductCancelFailure(MsgReqPurchaseShopProductCancel pReq, Error error)
    {
        NetLog.Log("NetComponent_PurchaseShopProductCancel : RecvPacket_PurchaseShopProductCancelFailure");

        GetFailureEvent().OnEvent(error);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// BattlePass

    public void RecvPacket_PurchaseBattlePassCancelSuccess(MsgReqPurchaseBattlePassCancel pReq, MsgAnsPurchaseBattlePassCancel pAns)
    {
        NetLog.Log("NetComponent_PurchaseShopProductCancel : RecvPacket_PurchaseShopProductCancelSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_PurchaseBattlePassCancelFailure(MsgReqPurchaseBattlePassCancel pReq, Error error)
    {
        NetLog.Log("NetComponent_PurchaseShopProductCancel : RecvPacket_PurchaseShopProductCancelFailure");

        GetFailureEvent().OnEvent(error);
    }
}
