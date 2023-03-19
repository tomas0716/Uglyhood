using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;

public class NetComponent_UserAccountGet : TNetComponent_SuccessOrFail<EventArg_Send_UserAccountGet, EventArg_Recv_UserAccountGet, Error>
{
    private EventArg_Recv_UserAccountGet    m_pRecv_UserAccountGet = new EventArg_Recv_UserAccountGet();

    public NetComponent_UserAccountGet()
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

    public override void OnEvent(EventArg_Send_UserAccountGet Arg)
    {
        NetLog.Log("NetComponent_UserAccountGet : OnEvent");

        MsgReqGetUserConnect pReq = new MsgReqGetUserConnect();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.Device_id = Arg.m_strDeviceID;
        pReq.I_BindType = (int)Arg.m_eLoginBind;

        m_pRecv_UserAccountGet.m_pUserAccountGet = Arg;

        SendPacket<MsgReqGetUserConnect, MsgAnsGetUserConnect>(pReq, RecvPacket_UserAccountGetSuccess, RecvPacket_UserAccountGetFailure);
    }

    public void RecvPacket_UserAccountGetSuccess(MsgReqGetUserConnect pReq, MsgAnsGetUserConnect pAns)
    {
        NetLog.Log("NetComponent_UserAccountGet : RecvPacket_UserAccountGetSuccess");

        m_pRecv_UserAccountGet.m_pProtocol_GetUserConnect = pAns;

        GetSuccessEvent().OnEvent(m_pRecv_UserAccountGet);
    }

    public void RecvPacket_UserAccountGetFailure(MsgReqGetUserConnect pReq, Error pError)
    {
        NetLog.Log("NetComponent_UserAccountGet : RecvPacket_UserAccountGetFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
