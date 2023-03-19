using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class FastRewardItem
{
    public int m_nRewardItemID;
    public int m_nRewardItemCount;
}

public class NetComponent_UserInfoFastOfflineReward : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    private int m_nRewardGold, m_nFastGetGemPrice;

    private List<FastRewardItem> m_pRewardItemList = new List<FastRewardItem>();

    //private int m_nRewardItemID_1;
    //private int m_nRewardItemCount_1;
    //private int m_nRewardItemID_2;
    //private int m_nRewardItemCount_2;
    //private int m_nRewardItemID_3;
    //private int m_nRewardItemCount_3;

    public NetComponent_UserInfoFastOfflineReward()
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

    private void ClearInfo()
    {
        m_nRewardGold = 0;
        m_nFastGetGemPrice = 0;

        m_pRewardItemList.Clear();

        //m_nRewardItemID_1 = 0;
        //m_nRewardItemID_2 = 0;
        //m_nRewardItemID_3 = 0;
        //m_nRewardItemCount_1 = 0;
        //m_nRewardItemCount_2 = 0;
        //m_nRewardItemCount_3 = 0;
    }

    public int GetFastRewardGoldCount()
    {
        return m_nRewardGold;
    }

    public int GetFastGetGemPrice()
    {
        return m_nFastGetGemPrice;
    }

    public List<FastRewardItem> GetFastRewardItemInfo()
    {
        return m_pRewardItemList;
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        NetLog.Log("NetComponent_UserInfoFastOfflineReward : OnEvent");

        MsgReqUserInfoFastOfflineReward pReq = new MsgReqUserInfoFastOfflineReward();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqUserInfoFastOfflineReward, MsgAnsUserInfoFastOfflineReward>(pReq, RecvPacket_UserInfoFastOfflineRewardSuccess, RecvPacket_UserInfoFastOfflineRewardFailure);
    }

    public void RecvPacket_UserInfoFastOfflineRewardSuccess(MsgReqUserInfoFastOfflineReward pReq, MsgAnsUserInfoFastOfflineReward pAns)
    {
        NetLog.Log("NetComponent_UserInfoFastOfflineReward : RecvPacket_UserInfoFastOfflineRewardSuccess");

        ClearInfo();

        m_nRewardGold = pAns.I_RewardGold;
        m_nFastGetGemPrice = pAns.I_FastGetPrice;

        FastRewardItem Info = new FastRewardItem();
        Info.m_nRewardItemID = pAns.I_RewardId_1;
        Info.m_nRewardItemCount = pAns.I_RewardCount_1;

        m_pRewardItemList.Add(Info);

        Info = new FastRewardItem();
        Info.m_nRewardItemID = pAns.I_RewardId_2;
        Info.m_nRewardItemCount = pAns.I_RewardCount_2;

        m_pRewardItemList.Add(Info);

        Info = new FastRewardItem();
        Info.m_nRewardItemID = pAns.I_RewardId_3;
        Info.m_nRewardItemCount = pAns.I_RewardCount_3;

        m_pRewardItemList.Add(Info);

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_UserInfoFastOfflineRewardFailure(MsgReqUserInfoFastOfflineReward pReq, Error error)
    {
        NetLog.Log("NetComponent_UserInfoFastOfflineReward : RecvPacket_UserInfoFastOfflineRewardFailure");

        GetFailureEvent().OnEvent(error);
    }
}
