using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_MainData : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_MainData()
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
        NetLog.Log("NetComponent_MainData : OnEvent");

        MsgReqMainData pReq = new MsgReqMainData();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqMainData, MsgAnsMainData>(pReq, RecvPacket_MainDataSuccess, RecvPacket_MainDataFailure);
    }

    public void RecvPacket_MainDataSuccess(MsgReqMainData pReq, MsgAnsMainData pAns)
    {
        Debug.Log("메인 데이터 성공");

        NetLog.Log("NetComponent_MainData : RecvPacket_MainDataSuccess");

        ItemInvenItemInfo pItemInvenItemInfo;
        pItemInvenItemInfo = new ItemInvenItemInfo((int)eItemType.Gem_Free, -1 * (int)eItemType.Gem_Free, 0, pAns.St_UserAssets.I_FreeGem, eItemType.Gem_Free);
        InventoryInfoManager.Instance.m_pItemInvenInfo.SetItemInvenItemInfo(pItemInvenItemInfo);

        pItemInvenItemInfo = new ItemInvenItemInfo((int)eItemType.Gem_Paid, -1 * (int)eItemType.Gem_Paid, 0, pAns.St_UserAssets.I_PurchaseGem, eItemType.Gem_Paid);
        InventoryInfoManager.Instance.m_pItemInvenInfo.SetItemInvenItemInfo(pItemInvenItemInfo);

        pItemInvenItemInfo = new ItemInvenItemInfo((int)eItemType.Gold, -1 * (int)eItemType.Gold, 0, pAns.St_UserAssets.I_Gold, eItemType.Gold);
        InventoryInfoManager.Instance.m_pItemInvenInfo.SetItemInvenItemInfo(pItemInvenItemInfo);

        pItemInvenItemInfo = new ItemInvenItemInfo((int)eItemType.Energy, -1 * (int)eItemType.Energy, 0, pAns.St_UserAssets.I_Ap, eItemType.Energy);
        InventoryInfoManager.Instance.m_pItemInvenInfo.SetItemInvenItemInfo(pItemInvenItemInfo);

        Debug.Log("현재 행동력 >> " + pAns.St_UserAssets.I_Ap);

        MyInfo.Instance.m_nMaxAP = pAns.St_UserAssets.I_ApMax;
        MyInfo.Instance.m_nAPChargeTerm = pAns.St_UserAssets.I_ApChargeTerm;

        MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic = pAns.St_UserInfoBasic;

        EventDelegateManager.Instance.OnLobby_RefreshEnergy();
        //EventDelegateManager.Instance.OnLobby_RefreshCurrencyData();

        if (pAns.St_UserAlarm.I_ConnectReward == 1)
        {
            MyInfo.Instance.m_IsConnectReward = true;
        }

        //if(pAns.St_UserAssets.I_ApChargeTime >= 0f)
        //{
        //    TimeInfo.Instance.SetTimer(pAns.St_UserAssets.I_ApChargeTime);
        //}

        if(pAns.St_UserAssets.I_ApChargeTime >= 0f)
        {
            InventoryInfoManager.Instance.m_pItemInvenInfo.SetTimerForEnergy(pAns.St_UserAssets.I_ApChargeTime);
        }

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_MainDataFailure(MsgReqMainData pReq, Error pError)
    {
        NetLog.Log("NetComponent_MainData : RecvPacket_MainDataFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
