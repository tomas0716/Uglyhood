using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;

public class NetComponent_UserAccountSet : TNetComponent_SuccessOrFail<EventArg_UserAccountSet, EventArg_UserAccountSet, Error>
{
    private EventArg_UserAccountSet m_pUserAccountSet = new EventArg_UserAccountSet();

    public NetComponent_UserAccountSet()
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

    public override void OnEvent(EventArg_UserAccountSet Arg)
    {
        NetLog.Log("NetComponent_UserAccountSet : OnEvent");

        MsgReqSetUserConnect pReq = new MsgReqSetUserConnect();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.Device_id = Arg.m_strDeviceID;
        pReq.I_BindType = (int)Arg.m_eLoginBind;

        m_pUserAccountSet = Arg;

        SendPacket<MsgReqSetUserConnect, MsgAnsSetUserConnect>(pReq, RecvPacket_UserAccountSetSuccess, RecvPacket_UserAccountSetFailure);
    }

    public void RecvPacket_UserAccountSetSuccess(MsgReqSetUserConnect pReq, MsgAnsSetUserConnect pAns)
    {
        NetLog.Log("NetComponent_UserAccountSet : RecvPacket_UserAccountSetSuccess");

        GetSuccessEvent().OnEvent(m_pUserAccountSet);
    }

    public void RecvPacket_UserAccountSetFailure(MsgReqSetUserConnect pReq, Error pError)
    {
        NetLog.Log("NetComponent_UserAccountDelete : RecvPacket_UserAccountSetFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
