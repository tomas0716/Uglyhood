using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetShopBasicProduct : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_GetShopBasicProduct()
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
        NetLog.Log("NetComponent_GetShopBasicProduct : OnEvent");

        MsgReqGetShopBasicProduct pReq = new MsgReqGetShopBasicProduct();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqGetShopBasicProduct, MsgAnsGetShopBasicProduct>(pReq, RecvPacket_GetShopBasicProductSuccess, RecvPacket_GetShopBasicProductFailure);
    }

    public void RecvPacket_GetShopBasicProductSuccess(MsgReqGetShopBasicProduct pReq, MsgAnsGetShopBasicProduct pAns)
    {
        NetLog.Log("NetComponent_GetShopBasicProduct : RecvPacket_GetShopBasicProductSuccess");

        InventoryInfoManager.Instance.m_pStoreInvenInfo.ClearBasicStoreInfoTable();

        foreach(KeyValuePair<int,ShopBasic> pBasicInfo in pAns.M_ShopBasic)
        {
            BasicStoreInfo pInfo = new BasicStoreInfo();

            pInfo.m_nID = pBasicInfo.Value.I_Id;
            pInfo.m_ePaymentType = (eShopPaymentType)pBasicInfo.Value.I_PaymentType;
            pInfo.m_strPaymentID = pBasicInfo.Value.S_PaymentId;
            pInfo.m_nPaymentQty = pBasicInfo.Value.I_PaymentQty;
            pInfo.m_eProductType = (eShopProductType)pBasicInfo.Value.I_ProductType;
            pInfo.m_nProductID = pBasicInfo.Value.I_ProductId;
            pInfo.m_nProductQty = pBasicInfo.Value.I_ProductQty;
            pInfo.m_eBonusProductType = (eItemType)pBasicInfo.Value.I_BonusProductType;
            pInfo.m_nBonusProductID = pBasicInfo.Value.I_BonusProductId;
            pInfo.m_nBonusProductQty = pBasicInfo.Value.I_BonusProductQty;
            pInfo.m_nPurchaseLimit = pBasicInfo.Value.I_PurchaseLimit;

            InventoryInfoManager.Instance.m_pStoreInvenInfo.AddBasicStoreInfo(pInfo);
        }

#if UNITY_EDITOR

        EventDelegateManager.Instance.OnSetIAPManagerInitialize();

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
#else
        if (EspressoInfo.Instance.m_IsOneStore == false)
        {
            if (IAPManager.m_pInstance != null && IAPManager.m_pInstance.IsProductInfoExist())
            {
                GetSuccessEvent().OnEvent(EventArg_Null.Object);
            }
            else
            {
                EventDelegateManager.Instance.OnEventBasicProduct_GetSuccessNextEvent += OnBasicProductGetSuccessNextEvent;

                EventDelegateManager.Instance.OnSetIAPManagerInitialize();
            }
        }
        else
        {
            if (IAPManager_Onestore.m_pInstance != null && IAPManager_Onestore.m_pInstance.IsProductInfoExist())
            {
                GetSuccessEvent().OnEvent(EventArg_Null.Object);
            }
            else
            {
                EventDelegateManager.Instance.OnEventBasicProduct_GetSuccessNextEvent += OnBasicProductGetSuccessNextEvent;

                EventDelegateManager.Instance.OnSetIAPManagerInitialize();
            }
        }
#endif
    }

    private void OnBasicProductGetSuccessNextEvent()
    {
        Debug.Log("베이직 샵 추가 다음 이벤트 넘어가기~");

        EventDelegateManager.Instance.OnEventBasicProduct_GetSuccessNextEvent -= OnBasicProductGetSuccessNextEvent;

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetShopBasicProductFailure(MsgReqGetShopBasicProduct pReq, Error error)
    {
        NetLog.Log("NetComponent_GetShopBasicProduct : RecvPacket_GetShopBasicProductFailure");

        GetFailureEvent().OnEvent(error);
    }
}
