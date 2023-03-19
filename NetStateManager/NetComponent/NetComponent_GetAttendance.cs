using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetAttendance : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_GetAttendance()
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
        NetLog.Log("NetComponent_GetAttendance : OnEvent");

        MsgReqGetAttendance pReq = new MsgReqGetAttendance();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqGetAttendance, MsgAnsGetAttendance>(pReq, RecvPacket_GetAttendanceSuccess, RecvPacket_GetAttendanceFailure);
    }

    public void RecvPacket_GetAttendanceSuccess(MsgReqGetAttendance pReq, MsgAnsGetAttendance pAns)
    {
        Debug.Log("°Ù Ãâ¼® ¼º°ø");

        NetLog.Log("NetComponent_GetAttendance : RecvPacket_GetAttendanceSuccess");

        InventoryInfoManager.Instance.m_pAttendanceInfo.ClearAttendanceInfo();

        int nCount = 0;

        nCount = pAns.I_AttendanceCount % 7;
        if(pAns.I_IsTodayAttendance == 1 && pAns.I_AttendanceCount % 7 == 0)
        {
            nCount = 7;
        }

        Debug.Log(pAns.I_AttendanceCount);

        InventoryInfoManager.Instance.m_pAttendanceInfo.SetAttendanceBasicInfo(nCount, pAns.I_IsTodayAttendance);

        foreach(KeyValuePair<int, sg.protocol.common.Attendance> pInfo in pAns.M_Attendance)
        {
            AttendanceInvenInfo pAttendanceInfo = new AttendanceInvenInfo();
            pAttendanceInfo.m_nDay = pInfo.Value.I_Day;
            pAttendanceInfo.m_nRewardType = pInfo.Value.I_RewardType;
            pAttendanceInfo.m_nRewardID = pInfo.Value.I_RewardId;
            pAttendanceInfo.m_nRewardCount = pInfo.Value.I_RewardCount;

            InventoryInfoManager.Instance.m_pAttendanceInfo.AddAttendanceInfoList(pAttendanceInfo);
        }

        EventDelegateManager.Instance.OnLobby_RefreshAttendanceRedDot();

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetAttendanceFailure(MsgReqGetAttendance pReq, Error error)
    {
        NetLog.Log("NetComponent_GetAttendance : RecvPacket_GetAttendanceFailure");

        GetFailureEvent().OnEvent(error);
    }
}
