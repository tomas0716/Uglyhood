using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_Team : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_Team()
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
        NetLog.Log("NetComponent_Team : OnEvent");

        MsgReqTeam pReq = new MsgReqTeam();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqTeam, MsgAnsTeam>(pReq, RecvPacket_TeamSuccess, RecvPacket_TeamFailure);
    }

    public void RecvPacket_TeamSuccess(MsgReqTeam pReq, MsgAnsTeam pAns)
    {
        NetLog.Log("NetComponent_Team : RecvPacket_TeamSuccess");

        MyInfo.Instance.m_pUserProfile = pAns.St_UserProfile;

        if(pAns.I_NameChangeFree == 0)
        {
            MyInfo.Instance.m_IsNameChangeFree = true;
        }
        else
        {
            MyInfo.Instance.m_IsNameChangeFree = false;
            MyInfo.Instance.m_nNameChangePrice = pAns.I_NameChangeFree;
        }

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_TeamFailure(MsgReqTeam pReq, Error error)
    {
        NetLog.Log("NetComponent_Team : RecvPacket_TeamFailure");

        GetFailureEvent().OnEvent(error);
    }
}
