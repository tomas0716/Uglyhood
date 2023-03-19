using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetUserInventory : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_GetUserInventory()
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
        NetLog.Log("NetComponent_GetUserInventory : OnEvent");

        MsgReqGetUserInventory pReq = new MsgReqGetUserInventory();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqGetUserInventory, MsgAnsGetUserInventory>(pReq, RecvPacket_GetUserInventorySuccess, RecvPacket_GetUserInventoryFailure);
    }

    public void RecvPacket_GetUserInventorySuccess(MsgReqGetUserInventory pReq, MsgAnsGetUserInventory pAns)
    {
        Debug.Log("겟 유저 인벤토리 성공");

        NetLog.Log("NetComponent_GetUserInventory : RecvPacket_GetUserInventorySuccess");

        InventoryInfoManager.Instance.m_pItemInvenInfo.ClearAll();

        foreach (KeyValuePair<int, UserItem> item in pAns.M_UserItem)
        {
            ExcelData_ItemInfo pExcelData_ItemInfo = ExcelDataManager.Instance.m_pItem.GetItemInfo_byID(item.Value.I_ItemId);

            if (pExcelData_ItemInfo != null)
            {
                ItemInvenItemInfo pItemInvenItemInfo = new ItemInvenItemInfo(item.Value.I_ItemId, item.Value.I_ItemSeq, item.Key, item.Value.I_ItemQuantity, pExcelData_ItemInfo.m_eItemType);
                InventoryInfoManager.Instance.m_pItemInvenInfo.SetItemInvenItemInfo(pItemInvenItemInfo);
            }
        }

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetUserInventoryFailure(MsgReqGetUserInventory pReq, Error pError)
    {
        NetLog.Log("NetComponent_GetUserInventory : RecvPacket_GetUserInventoryFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
