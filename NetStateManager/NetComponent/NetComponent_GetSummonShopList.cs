using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetSummonShopList : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_GetSummonShopList()
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
        NetLog.Log("NetComponent_GetSummonShopList : OnEvent");

        MsgReqGetSummonShopList pReq = new MsgReqGetSummonShopList();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqGetSummonShopList, MsgAnsGetSummonShopList>(pReq, RecvPacket_GetSummonShopListSuccess, RecvPacket_GetSummonShopListFailure);
    }

    public void RecvPacket_GetSummonShopListSuccess(MsgReqGetSummonShopList pReq, MsgAnsGetSummonShopList pAns)
    {
        NetLog.Log("NetComponent_GetSummonShopList : RecvPacket_GetSummonShopListSuccess");

        //TimeInfo.Instance.ClearTimer_ForSummonList();

        InventoryInfoManager.Instance.m_pSummonShopInfo.SummonShopInfoTableClear();
        InventoryInfoManager.Instance.m_pSummonShopInfo.InitSummonInfo(pAns.M_SummonShopList);

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetSummonShopListFailure(MsgReqGetSummonShopList pReq, Error pError)
    {
        NetLog.Log("NetComponent_GetSummonShopList : RecvPacket_GetSummonShopListFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
