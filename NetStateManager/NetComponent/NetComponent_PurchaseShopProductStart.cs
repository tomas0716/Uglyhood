using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;
using sg.protocol.common;

public class NetComponent_PurchaseShopProductStart : TNetComponent_SuccessOrFail<EventArg_PurchaseShopProductStart, EventArg_Null, Error>
{
    public NetComponent_PurchaseShopProductStart()
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

    public override void OnEvent(EventArg_PurchaseShopProductStart Arg)
    {
        Debug.Log("Purchase Start OnEvent");

        NetLog.Log("NetComponent_PurchaseShopProductStart : OnEvent");

        switch (Arg.m_eIAPContentType)
        {
            case eIAPContentType.Shop:
                {
                    MsgReqPurchaseShopProductStart pReq = new MsgReqPurchaseShopProductStart();
                    pReq.Uid = MyInfo.Instance.m_nUserIndex;
                    pReq.I_Id = Arg.m_nID;

                    SendPacket<MsgReqPurchaseShopProductStart, MsgAnsPurchaseShopProductStart>(pReq, RecvPacket_PurchaseShopProductStartSuccess, RecvPacket_PurchaseShopProductStartFailure);
                }
                break;

            case eIAPContentType.Package:
                {
                }
                break;

            case eIAPContentType.BattlePassTicket:
                {
                    MsgReqPurchaseBattlePassStart pReq = new MsgReqPurchaseBattlePassStart();
                    pReq.Uid = MyInfo.Instance.m_nUserIndex;
                    pReq.S_ProductId = Arg.m_strProductID;

                    SendPacket<MsgReqPurchaseBattlePassStart, MsgAnsPurchaseBattlePassStart>(pReq, RecvPacket_PurchaseBattlePassProductStartSuccess, RecvPacket_PurchaseBattlePassProductStartFailure);
                }
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Shop
    
    public void RecvPacket_PurchaseShopProductStartSuccess(MsgReqPurchaseShopProductStart pReq, MsgAnsPurchaseShopProductStart pAns)
    {
        Debug.Log("Purchase Start Success");

        NetLog.Log("NetComponent_PurchaseShopProductStart : RecvPacket_PurchaseShopProductStartSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_PurchaseShopProductStartFailure(MsgReqPurchaseShopProductStart pReq, Error error)
    {
        Debug.Log("Purchase Start Failure");

        NetLog.Log("NetComponent_PurchaseShopProductStart : RecvPacket_PurchaseShopProductStartFailure");

        GetFailureEvent().OnEvent(error);
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// BattlePass

    public void RecvPacket_PurchaseBattlePassProductStartSuccess(MsgReqPurchaseBattlePassStart pReq, MsgAnsPurchaseBattlePassStart pAns)
    {
        Debug.Log("Purchase Start Success");

        NetLog.Log("NetComponent_PurchaseShopProductStart : RecvPacket_PurchaseShopProductStartSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_PurchaseBattlePassProductStartFailure(MsgReqPurchaseBattlePassStart pReq, Error error)
    {
        Debug.Log("Purchase Start Failure");

        NetLog.Log("NetComponent_PurchaseShopProductStart : RecvPacket_PurchaseShopProductStartFailure");

        GetFailureEvent().OnEvent(error);
    }
}
