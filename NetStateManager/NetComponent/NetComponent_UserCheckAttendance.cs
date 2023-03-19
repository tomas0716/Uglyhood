using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_UserCheckAttendance : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_UserCheckAttendance()
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
        NetLog.Log("NetComponent_UserCheckAttendance : OnEvent");

        MsgReqUserCheckAttendance pReq = new MsgReqUserCheckAttendance();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqUserCheckAttendance, MsgAnsUserCheckAttendance>(pReq, RecvPacket_UserCheckAttendanceSuccess, RecvPacket_UserCheckAttendanceFailure);
    }

    public void RecvPacket_UserCheckAttendanceSuccess(MsgReqUserCheckAttendance pReq, MsgAnsUserCheckAttendance pAns)
    {
        NetLog.Log("NetComponent_UserCheckAttendance : RecvPacket_UserCheckAttendanceSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_UserCheckAttendanceFailure(MsgReqUserCheckAttendance pReq, Error error)
    {
        NetLog.Log("NetComponent_UserCheckAttendance : RecvPacket_UserCheckAttendanceFailure");

        GetFailureEvent().OnEvent(error);
    }
}
