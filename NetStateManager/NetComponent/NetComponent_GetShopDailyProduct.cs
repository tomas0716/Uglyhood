using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetShopDailyProduct : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_GetShopDailyProduct()
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
        NetLog.Log("NetComponent_GetShopDailyProduct : OnEvent");

        MsgReqGetShopDailyProduct pReq = new MsgReqGetShopDailyProduct();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqGetShopDailyProduct, MsgAnsGetShopDailyProduct>(pReq, RecvPacket_GetShopDailyProductSuccess, RecvPacket_GetShopDailyProductFailure);
    }

    public void RecvPacket_GetShopDailyProductSuccess(MsgReqGetShopDailyProduct pReq, MsgAnsGetShopDailyProduct pAns)
     {
        Debug.Log("百 讥 单老府 橇肺傣飘 己傍");

        NetLog.Log("NetComponent_GetShopDailyProduct : RecvPacket_GetShopDailyProductSuccess");

        InventoryInfoManager.Instance.m_pStoreInvenInfo.ClearDailyStoreInfoTable();

        InventoryInfoManager.Instance.m_pStoreInvenInfo.SetDailyStore_RefreshCount(pAns.I_RenewCount);

        if (pAns.I_RenewCount >= 6)
        {
            InventoryInfoManager.Instance.m_pStoreInvenInfo.SetDailyStoreRefreshRemainTime_byTimeInfo(DateTime.Now);
        }

        foreach (KeyValuePair<int, ShopDaily> pDailyInfo in pAns.M_ShopDaily)
        {
            DailyStoreInfo pInfo = new DailyStoreInfo();

            pInfo.m_nID = pDailyInfo.Value.I_Id;
            pInfo.m_ePaymentType = (eShopPaymentType)pDailyInfo.Value.I_PaymentType;
            pInfo.m_strPaymentID = pDailyInfo.Value.S_PaymentId;
            pInfo.m_nPaymentQty = pDailyInfo.Value.I_PaymentQty;
            pInfo.m_eProductType = (eShopProductType)pDailyInfo.Value.I_ProductType;
            pInfo.m_nProductID = pDailyInfo.Value.I_ProductId;
            pInfo.m_nProductQty = pDailyInfo.Value.I_ProductQty;

            if (pDailyInfo.Value.I_IsBuy == 0)
            {
                pInfo.m_IsBuy = false;
            }
            else
            {
                pInfo.m_IsBuy = true;
            }

            InventoryInfoManager.Instance.m_pStoreInvenInfo.AddDailyStoreInfo(pInfo);
        }

        InventoryInfoManager.Instance.m_pStoreInvenInfo.SetTimer_ForDailyStore();

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetShopDailyProductFailure(MsgReqGetShopDailyProduct pReq, Error error)
    {
        NetLog.Log("NetComponent_GetShopDailyProduct : RecvPacket_GetShopDailyProductFailure");

        GetFailureEvent().OnEvent(error);
    }
}
