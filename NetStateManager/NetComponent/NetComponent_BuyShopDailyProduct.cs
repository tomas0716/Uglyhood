using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_BuyShopDailyProduct : TNetComponent_SuccessOrFail<EventArg_BuyShopDailyProduct, EventArg_Null, Error>
{
    public NetComponent_BuyShopDailyProduct()
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

    public override void OnEvent(EventArg_BuyShopDailyProduct Arg)
    {
        NetLog.Log("NetComponent_BuyShopDailyProduct : OnEvent");

        MsgReqBuyShopDailyProduct pReq = new MsgReqBuyShopDailyProduct();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_Id = Arg.m_nID;

        SendPacket<MsgReqBuyShopDailyProduct, MsgAnsBuyShopDailyProduct>(pReq, RecvPacket_BuyShopDailyProductSuccess, RecvPacket_BuyShopDailyProductFailure);
    }

    public void RecvPacket_BuyShopDailyProductSuccess(MsgReqBuyShopDailyProduct pReq, MsgAnsBuyShopDailyProduct pAns)
    {
        Debug.Log("바이 샵 데일리 프로덕트 성공");

        NetLog.Log("NetComponent_BuyShopDailyProduct : RecvPacket_BuyShopDailyProductSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_BuyShopDailyProductFailure(MsgReqBuyShopDailyProduct pReq, Error error)
    {
        NetLog.Log("NetComponent_BuyShopDailyProduct : RecvPacket_BuyShopDailyProductFailure");

        GetFailureEvent().OnEvent(error);
    }
}
