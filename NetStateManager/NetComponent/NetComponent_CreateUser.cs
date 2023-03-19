using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;

public class NetComponent_CreateUser : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_CreateUser()
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
        NetLog.Log("NetComponent_CreateUser : OnEvent");

        MsgReqCreateUser pReq = new MsgReqCreateUser();
        pReq.Membership_info_did = new MembershipInfoDID();

        if (SaveDataInfo.Instance.m_eNetwork_LoginBind == eNetwork_LoginBind.Guest)
        {
            Debug.Log("Create Guest");

            pReq.Membership_info_did.Device_id = OptionInfo.Instance.m_strDeviceUID;
#if UNITY_EDITOR
            pReq.Membership_info_did.I_BindType = 0;
#else
            pReq.Membership_info_did.I_BindType = (int)SaveDataInfo.Instance.m_eNetwork_LoginBind;
#endif
        }
        else
        {
            pReq.Membership_info_did.Device_id = SaveDataInfo.Instance.m_strNetwork_LoginAccountID;

#if UNITY_EDITOR
            pReq.Membership_info_did.I_BindType = 0;
#else
            pReq.Membership_info_did.I_BindType = (int)SaveDataInfo.Instance.m_eNetwork_LoginBind;
#endif

        }

#if UNITY_ANDROID
        pReq.I_Platform = (int)eNetwork_LoginFlatform.AOS;
#elif UNITY_IOS
        pReq.I_Platform = (int)eNetwork_LoginFlatform.IOS;
#else
        pReq.I_Platform = (int)eNetwork_LoginFlatform.PC;
#endif

        OutputLog.Log("NetComponent_CreateUser : OnEvent > " + pReq.Membership_info_did.Device_id + " > " + pReq.Membership_info_did.I_BindType.ToString());

        SendPacket<MsgReqCreateUser, MsgAnsCreateUser>(pReq, RecvPacket_LoginSuccess, RecvPacket_LoginFailure);
    }

    public void RecvPacket_LoginSuccess(MsgReqCreateUser pReq, MsgAnsCreateUser pAns)
    {
        NetLog.Log("NetComponent_CreateUser : RecvPacket_LoginSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_LoginFailure(MsgReqCreateUser pReq, Error pError)
    {
        NetLog.Log("NetComponent_CreateUser : RecvPacket_LoginFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
