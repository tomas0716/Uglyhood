using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_SetUserAvatar : TNetComponent_SuccessOrFail<EventArg_SetUserAvatar, EventArg_Null, Error>
{
    public NetComponent_SetUserAvatar()
    {

    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
    }

    public override void OnEvent(EventArg_SetUserAvatar Arg)
    {
        NetLog.Log("NetComponent_SetUserAvatar : OnEvent");

        MsgReqSetUserAvatar pReq = new MsgReqSetUserAvatar();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_AvatarId = Arg.m_nAvatarUnitID;

        SendPacket<MsgReqSetUserAvatar, MsgAnsSetUserAvatar>(pReq, RecvPacket_SetUserAvatarSuccess, RecvPacket_SetUserAvatarFailure);
    }

    public void RecvPacket_SetUserAvatarSuccess(MsgReqSetUserAvatar pReq, MsgAnsSetUserAvatar pAns)
    {
        NetLog.Log("NetComponent_SetUserAvatar : RecvPacket_SetUserAvatarSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_SetUserAvatarFailure(MsgReqSetUserAvatar pReq, Error error)
    {
        NetLog.Log("NetComponent_SetUserAvatar : RecvPacket_SetUserAvatarFailure");

        GetFailureEvent().OnEvent(error);
    }
}
