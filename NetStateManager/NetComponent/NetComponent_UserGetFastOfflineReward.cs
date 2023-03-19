using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_UserGetFastOfflineReward : TNetComponent_SuccessOrFail<EventArg_UserGetFastOfflineReward, EventArg_Null, Error>
{
    public NetComponent_UserGetFastOfflineReward()
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

    public override void OnEvent(EventArg_UserGetFastOfflineReward Arg)
    {
        NetLog.Log("NetComponent_UserGetFastOfflineReward : OnEvent");

        MsgReqUserGetFastOfflineReward pReq = new MsgReqUserGetFastOfflineReward();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_Kind = Arg.m_nKind;

        SendPacket<MsgReqUserGetFastOfflineReward, MsgAnsUserGetFastOfflineReward>(pReq, RecvPacket_UserGetFastOfflineRewardSuccess, RecvPacket_UserGetFastOfflineRewardFailure);
    }

    public void RecvPacket_UserGetFastOfflineRewardSuccess(MsgReqUserGetFastOfflineReward pReq, MsgAnsUserGetFastOfflineReward pAns)
    {
        NetLog.Log("NetComponent_UserGetFastOfflineReward : RecvPacket_UserGetFastOfflineRewardSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_UserGetFastOfflineRewardFailure(MsgReqUserGetFastOfflineReward pReq, Error error)
    {
        NetLog.Log("NetComponent_UserGetFastOfflineReward : RecvPacket_UserGetFastOfflineRewardFailure");

        GetFailureEvent().OnEvent(error);
    }
}
