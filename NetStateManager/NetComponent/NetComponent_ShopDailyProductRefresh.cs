using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_ShopDailyProductRefresh : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_ShopDailyProductRefresh()
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

    public override void OnEvent(EventArg_Null Arg)
    {
        NetLog.Log("NetComponent_ShopDailyProductRefresh : OnEvent");

        MsgReqRenewShopDailyProduct pReq = new MsgReqRenewShopDailyProduct();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqRenewShopDailyProduct, MsgAnsRenewShopDailyProduct>(pReq, RecvPacket_ShopDailyProductRefreshSuccess, RecvPacket_ShopDailyProductRefreshFailure);
    }

    public void RecvPacket_ShopDailyProductRefreshSuccess(MsgReqRenewShopDailyProduct pReq, MsgAnsRenewShopDailyProduct pAns)
    {
        NetLog.Log("NetComponent_ShopDailyProductRefresh : RecvPacket_ShopDailyProductRefreshSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_ShopDailyProductRefreshFailure(MsgReqRenewShopDailyProduct pReq, Error error)
    {
        NetLog.Log("NetComponent_ShopDailyProductRefresh : RecvPacket_ShopDailyProductRefreshFailure");

        GetFailureEvent().OnEvent(error);
    }
}
