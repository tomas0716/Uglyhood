using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_UserBattleSkipLevel : TNetComponent_SuccessOrFail<EventArg_BattlePassSkip, EventArg_BattlePassSkip, Error>
{
    private EventArg_BattlePassSkip m_pEventArg;

    public NetComponent_UserBattleSkipLevel()
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

    public override void OnEvent(EventArg_BattlePassSkip Arg)
    {
        NetLog.Log("NetComponent_UserBattleSkipLevel : OnEvent");

        m_pEventArg = Arg;

        MsgReqUserBattleSkipLevel pReq = new MsgReqUserBattleSkipLevel();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_Level = Arg.m_nLevel;

        SendPacket<MsgReqUserBattleSkipLevel, MsgAnsUserBattleSkipLevel>(pReq, RecvPacket_UserBattleSkipLevelSuccess, RecvPacket_UserBattleSkipLevelFailure);
    }

    public void RecvPacket_UserBattleSkipLevelSuccess(MsgReqUserBattleSkipLevel pReq, MsgAnsUserBattleSkipLevel pAns)
    {
        NetLog.Log("NetComponent_UserBattleSkipLevel : RecvPacket_UserBattleSkipLevelSuccess");

        GetSuccessEvent().OnEvent(m_pEventArg);
    }

    public void RecvPacket_UserBattleSkipLevelFailure(MsgReqUserBattleSkipLevel pReq, Error error)
    {
        NetLog.Log("NetComponent_UserBattleSkipLevel : RecvPacket_UserBattleSkipLevelFailure");

        GetFailureEvent().OnEvent(error);
    }
}
