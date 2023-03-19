using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_UserGetOfflineReward : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_UserGetOfflineReward()
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
        NetLog.Log("NetComponent_UserGetOfflineReward : OnEvent");

        MsgReqUserGetOfflineReward pReq = new MsgReqUserGetOfflineReward();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqUserGetOfflineReward, MsgAnsUserGetOfflineReward>(pReq, RecvPacket_UserGetOfflineRewardSuccess, RecvPacket_UserGetOfflineRewardFailure);
    }

    public void RecvPacket_UserGetOfflineRewardSuccess(MsgReqUserGetOfflineReward pReq, MsgAnsUserGetOfflineReward pAns)
    {
        NetLog.Log("NetComponent_UserGetOfflineReward : RecvPacket_UserGetOfflineRewardSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_UserGetOfflineRewardFailure(MsgReqUserGetOfflineReward pReq, Error error)
    {
        NetLog.Log("NetComponent_UserGetOfflineReward : RecvPacket_UserGetOfflineRewardFailure");

        GetFailureEvent().OnEvent(error);
    }
}
