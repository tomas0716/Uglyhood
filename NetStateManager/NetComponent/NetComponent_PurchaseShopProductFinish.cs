using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;
using sg.protocol.common;

public class NetComponent_PurchaseShopProductFinish : TNetComponent_SuccessOrFail<EventArg_PurchaseShopProductFinish, EventArg_Null, Error>
{
    public NetComponent_PurchaseShopProductFinish()
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

    public override void OnEvent(EventArg_PurchaseShopProductFinish Arg)
    {
        NetLog.Log("NetComponent_PurchaseShopProductFinish : OnEvent");

        switch (Arg.m_eIAPContentType)
        {
            case eIAPContentType.Shop:
                {
                    MsgReqPurchaseShopProductFinish pReq = new MsgReqPurchaseShopProductFinish();
                    pReq.Uid = MyInfo.Instance.m_nUserIndex;
                    pReq.I_BuySeq = Arg.m_nBuySeq;

                    SendPacket<MsgReqPurchaseShopProductFinish, MsgAnsPurchaseShopProductFinish>(pReq, RecvPacket_PurchaseShopProductFinishSuccess, RecvPacket_PurchaseShopProductFinishFailure);
                }
                break;

            case eIAPContentType.Package:
                {
                }
                break;

            case eIAPContentType.BattlePassTicket:
                {
                    MsgReqPurchaseBattlePassFinish pReq = new MsgReqPurchaseBattlePassFinish();
                    pReq.Uid = MyInfo.Instance.m_nUserIndex;
                    pReq.I_BuySeq = Arg.m_nBuySeq;

                    SendPacket<MsgReqPurchaseBattlePassFinish, MsgAnsPurchaseBattlePassFinish>(pReq, RecvPacket_PurchaseShopProductBattlePassFinishSuccess, RecvPacket_PurchaseShopProductBattlePassFinishFailure);
                }
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Shop

    public void RecvPacket_PurchaseShopProductFinishSuccess(MsgReqPurchaseShopProductFinish pReq, MsgAnsPurchaseShopProductFinish pAns)
    {
        NetLog.Log("NetComponent_PurchaseShopProductFinish : RecvPacket_PurchaseShopProductFinishSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_PurchaseShopProductFinishFailure(MsgReqPurchaseShopProductFinish pReq, Error error)
    {
        NetLog.Log("NetComponent_PurchaseShopProductFinish : RecvPacket_PurchaseShopProductFinishFailure");

        GetFailureEvent().OnEvent(error);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// BattlePass

    public void RecvPacket_PurchaseShopProductBattlePassFinishSuccess(MsgReqPurchaseBattlePassFinish pReq, MsgAnsPurchaseBattlePassFinish pAns)
    {
        NetLog.Log("NetComponent_PurchaseShopProductFinish : RecvPacket_PurchaseShopProductFinishSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_PurchaseShopProductBattlePassFinishFailure(MsgReqPurchaseBattlePassFinish pReq, Error error)
    {
        NetLog.Log("NetComponent_PurchaseShopProductFinish : RecvPacket_PurchaseShopProductFinishFailure");

        GetFailureEvent().OnEvent(error);
    }
}
