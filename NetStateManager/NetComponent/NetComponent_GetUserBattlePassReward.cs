using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetUserBattlePassReward : TNetComponent_SuccessOrFail<EventArg_BattlePassReward, EventArg_BattlePassReward, Error>
{
    private EventArg_BattlePassReward   m_pEventArg;


    private int m_nRecvItemID;
    private int m_nRecvItemCount;

    public NetComponent_GetUserBattlePassReward()
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

    public int GetRecvItemID()
    {
        return m_nRecvItemID;
    }

    public int GetRecvItemCount()
    {
        return m_nRecvItemCount;
    }

    public override void OnEvent(EventArg_BattlePassReward Arg)
    {
        NetLog.Log("NetComponent_GetUserBattlePassReward : OnEvent");

        m_pEventArg = Arg;

        MsgReqGetUserBattlePassReward pReq = new MsgReqGetUserBattlePassReward();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_Kind = Arg.m_IsFree == true ? 0 : 1;
        pReq.I_Level = Arg.m_nLevel;

        SendPacket<MsgReqGetUserBattlePassReward, MsgAnsGetUserBattlePassReward>(pReq, RecvPacket_GetUserBattlePassRewardSuccess, RecvPacket_GetUserBattlePassRewardFailure);
    }

    public void RecvPacket_GetUserBattlePassRewardSuccess(MsgReqGetUserBattlePassReward pReq, MsgAnsGetUserBattlePassReward pAns)
    {
        NetLog.Log("NetComponent_GetUserBattlePassReward : RecvPacket_GetUserBattlePassRewardSuccess");

        m_nRecvItemID = pAns.I_Id;
        m_nRecvItemCount = pAns.I_Count;

        GetSuccessEvent().OnEvent(m_pEventArg);
    }

    public void RecvPacket_GetUserBattlePassRewardFailure(MsgReqGetUserBattlePassReward pReq, Error error)
    {
        NetLog.Log("NetComponent_GetUserBattlePassReward : RecvPacket_GetUserBattlePassRewardFailure");

        GetFailureEvent().OnEvent(error);
    }
}
