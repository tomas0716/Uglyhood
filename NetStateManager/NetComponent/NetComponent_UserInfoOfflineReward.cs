using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_UserInfoOfflineReward : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_UserInfoOfflineReward()
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
        NetLog.Log("NetComponent_UserInfoOfflineReward : OnEvent");

        MsgReqUserInfoOfflineReward pReq = new MsgReqUserInfoOfflineReward();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqUserInfoOfflineReward, MsgAnsUserInfoOfflineReward>(pReq, RecvPacket_UserInfoOfflineRewardSuccess, RecvPacket_UserInfoOfflineRewardFailure);
    }

    public void RecvPacket_UserInfoOfflineRewardSuccess(MsgReqUserInfoOfflineReward pReq, MsgAnsUserInfoOfflineReward pAns)
    {
        NetLog.Log("NetComponent_UserInfoOfflineReward : RecvPacket_UserInfoOfflineRewardSuccess");

        InventoryInfoManager.Instance.m_pOfflineRewardInfo.ClearInfo();
        InventoryInfoManager.Instance.m_pOfflineRewardInfo.SetTimeInfo(pAns.I_AccrueTime, pAns.I_FastAdLimit, pAns.I_FastGemLimit, pAns.I_NextStartTime);
        InventoryInfoManager.Instance.m_pOfflineRewardInfo.SetRewardGold(pAns.I_RewardGold);

        Debug.Log("Gold: " + pAns.I_RewardGold);

        InventoryInfoManager.Instance.m_pOfflineRewardInfo.SetRewardInfo(pAns.I_RewardId_1, pAns.I_RewardCount_1, 2);
        InventoryInfoManager.Instance.m_pOfflineRewardInfo.SetRewardInfo(pAns.I_RewardId_2, pAns.I_RewardCount_2, 3);
        InventoryInfoManager.Instance.m_pOfflineRewardInfo.SetRewardInfo(pAns.I_RewardId_3, pAns.I_RewardCount_3, 4);
        InventoryInfoManager.Instance.m_pOfflineRewardInfo.SetRewardInfo(pAns.I_RewardId_4, pAns.I_RewardCount_4, 5);
        InventoryInfoManager.Instance.m_pOfflineRewardInfo.SetRewardInfo(pAns.I_RewardId_5, pAns.I_RewardCount_5, 6);
        InventoryInfoManager.Instance.m_pOfflineRewardInfo.SetRewardInfo(pAns.I_RewardId_6, pAns.I_RewardCount_6, 7);
        InventoryInfoManager.Instance.m_pOfflineRewardInfo.SetRewardInfo(pAns.I_RewardId_7, pAns.I_RewardCount_7, 8);
        InventoryInfoManager.Instance.m_pOfflineRewardInfo.SetRewardInfo(pAns.I_RewardId_8, pAns.I_RewardCount_8, 9);
        InventoryInfoManager.Instance.m_pOfflineRewardInfo.SetRewardInfo(pAns.I_RewardId_9, pAns.I_RewardCount_9, 10);

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_UserInfoOfflineRewardFailure(MsgReqUserInfoOfflineReward pReq, Error error)
    {
        NetLog.Log("NetComponent_UserInfoOfflineReward : RecvPacket_UserInfoOfflineRewardFailure");

        GetFailureEvent().OnEvent(error);
    }
}
