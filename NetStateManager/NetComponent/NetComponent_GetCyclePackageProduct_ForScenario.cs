using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetCyclePackageProduct_ForScenario : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_GetCyclePackageProduct_ForScenario()
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
        NetLog.Log("NetComponent_GetCyclePackageProduct_ForScenario : OnEvent");

        MsgReqGetShopPackageProduct pReq = new MsgReqGetShopPackageProduct();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_PackageKind = (int)ePackageType.Cycle;

        InventoryInfoManager.Instance.m_pStoreInvenInfo.ClearCycleInfoTable();

        SendPacket<MsgReqGetShopPackageProduct, MsgAnsGetShopPackageProduct>(pReq, RecvPacket_GetShopPackageProductSuccess, RecvPacket_GetShopPackageProductFailure);
    }

    public void RecvPacket_GetShopPackageProductSuccess(MsgReqGetShopPackageProduct pReq, MsgAnsGetShopPackageProduct pAns)
    {
        NetLog.Log("NetComponent_GetCyclePackageProduct_ForScenario : RecvPacket_GetShopPackageProductSuccess");

        foreach (KeyValuePair<int, ShopPackage> pInfo in pAns.M_ShopPackage)
        {
            PackageStoreInfo pPackageInfo = new PackageStoreInfo();
            pPackageInfo.m_ePackageKind = (ePackageType)pInfo.Value.I_PackageKind;
            pPackageInfo.m_nID = pInfo.Value.I_Id;
            pPackageInfo.m_ePaymentType = (eShopPaymentType)pInfo.Value.I_PaymentType;
            pPackageInfo.m_strPaymentID = pInfo.Value.S_PaymentId;
            pPackageInfo.m_nPurchaseLimit = pInfo.Value.I_PurchaseLimit;
            pPackageInfo.m_nRequiredChapter = pInfo.Value.I_RequiredChapter;
            pPackageInfo.m_nCycleType = pInfo.Value.I_CycleType;
            pPackageInfo.m_nRankStart = pInfo.Value.I_RankStart;
            pPackageInfo.m_nRankEnd = pInfo.Value.I_RankEnd;
            pPackageInfo.m_nRequiredUnit = pInfo.Value.I_RequiredUnit;
            pPackageInfo.m_nExpiretimeHour = pInfo.Value.I_ExpiretimeHour;

            if (pInfo.Value.I_PackageKind == (int)ePackageType.Chapter)
            {
                InventoryInfoManager.Instance.m_pStoreInvenInfo.AddChapterInfoTable(pPackageInfo);
            }
            else if (pInfo.Value.I_PackageKind == (int)ePackageType.Cycle)
            {
                InventoryInfoManager.Instance.m_pStoreInvenInfo.AddCycleInfoTable(pPackageInfo);
            }
            else if (pInfo.Value.I_PackageKind == (int)ePackageType.Rank)
            {
                InventoryInfoManager.Instance.m_pStoreInvenInfo.AddRankInfoTable(pPackageInfo);
            }
            else
            {
                InventoryInfoManager.Instance.m_pStoreInvenInfo.AddGrowthInfoTable(pPackageInfo);
            }
        }

        InventoryInfoManager.Instance.m_pStoreInvenInfo.CharacterInfoTimeCheck();

        Dictionary<int, List<PackageGoodsInfo>> pPackageGoodsTable = new Dictionary<int, List<PackageGoodsInfo>>();

        foreach (KeyValuePair<int, ShopPackageGoods> pInfo in pAns.M_ShopPackageGoods)
        {
            PackageGoodsInfo pGoodsInfo = new PackageGoodsInfo();
            pGoodsInfo.m_nID = pInfo.Value.I_Id;
            pGoodsInfo.m_nProductType = pInfo.Value.I_ProductType;
            pGoodsInfo.m_nProductID = pInfo.Value.I_ProductId;
            pGoodsInfo.m_nProductCount = pInfo.Value.I_ProductCount;

            List<PackageGoodsInfo> pPackageGoodsList;
            if (pPackageGoodsTable.ContainsKey(pGoodsInfo.m_nID))
            {
                pPackageGoodsList = pPackageGoodsTable[pGoodsInfo.m_nID];
            }
            else
            {
                pPackageGoodsList = new List<PackageGoodsInfo>();
                pPackageGoodsTable.Add(pGoodsInfo.m_nID, pPackageGoodsList);
            }
            pPackageGoodsList.Add(pGoodsInfo);
        }

        InventoryInfoManager.Instance.m_pStoreInvenInfo.AddPackageGoodsInfoTable(pPackageGoodsTable);

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetShopPackageProductFailure(MsgReqGetShopPackageProduct pReq, Error error)
    {
        NetLog.Log("NetComponent_GetCyclePackageProduct_ForScenario : RecvPacket_GetShopPackageProductFailure");

        GetFailureEvent().OnEvent(error);
    }
}
