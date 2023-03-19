using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_BuyShopBasicProduct : TNetComponent_SuccessOrFail<EventArg_BuyShopBasicProduct, EventArg_Null, Error>
{
    public NetComponent_BuyShopBasicProduct()
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

    public override void OnEvent(EventArg_BuyShopBasicProduct Arg)
    {
        NetLog.Log("NetComponent_BuyShopBasicProduct : OnEvent");

        MsgReqBuyShopBasicProduct pReq = new MsgReqBuyShopBasicProduct();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_Id = Arg.m_nID;

        SendPacket<MsgReqBuyShopBasicProduct, MsgAnsBuyShopBasicProduct>(pReq, RecvPacket_BuyShopBasicProductSuccess, RecvPacket_BuyShopBasicProductFailure);
    }

    public void RecvPacket_BuyShopBasicProductSuccess(MsgReqBuyShopBasicProduct pReq, MsgAnsBuyShopBasicProduct pAns)
    {
        NetLog.Log("NetComponent_BuyShopBasicProduct : RecvPacket_BuyShopBasicProductSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_BuyShopBasicProductFailure(MsgReqBuyShopBasicProduct pReq, Error error)
    {
        NetLog.Log("NetComponent_BuyShopBasicProduct : RecvPacket_BuyShopBasicProductFailure");

        GetFailureEvent().OnEvent(error);
    }
}
