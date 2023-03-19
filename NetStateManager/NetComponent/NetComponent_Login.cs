using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;

public class NetComponent_Login : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_Login()
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
        NetLog.Log("NetComponent_Login : OnEvent");

#if UNITY_EDITOR
        {
            SaveDataInfo.Instance.m_eNetwork_LoginBind = eNetwork_LoginBind.Guest;

            MsgReqLogin pReq = new MsgReqLogin();
            pReq.Membership_info_did = new MembershipInfoDID();
            pReq.Membership_info_did.Device_id = OptionInfo.Instance.m_strDeviceUID;
#if UNITY_EDITOR
            pReq.Membership_info_did.I_BindType = 0;
#else
            pReq.Membership_info_did.I_BindType = (int)SaveDataInfo.Instance.m_eNetwork_LoginBind;
#endif
            pReq.S_AppVersion = Application.version;

            SendPacket<MsgReqLogin, MsgAnsLogin>(pReq, RecvPacket_LoginSuccess, RecvPacket_LoginFailure);
            return;
        }
#endif

        if (SaveDataInfo.Instance.m_eNetwork_LoginBind != eNetwork_LoginBind.None)
        {
            MsgReqLogin pReq = new MsgReqLogin();
            pReq.Membership_info_did = new MembershipInfoDID();
            pReq.Membership_info_did.Device_id = SaveDataInfo.Instance.m_strNetwork_LoginAccountID;
#if UNITY_EDITOR
            pReq.Membership_info_did.I_BindType = 0;
#else
            pReq.Membership_info_did.I_BindType = (int)SaveDataInfo.Instance.m_eNetwork_LoginBind;
#endif
            pReq.S_AppVersion = Application.version;

            OutputLog.Log("NetComponent_Login : OnEvent > " + pReq.Membership_info_did.Device_id + " > " + pReq.Membership_info_did.I_BindType.ToString());

            SendPacket<MsgReqLogin, MsgAnsLogin>(pReq, RecvPacket_LoginSuccess, RecvPacket_LoginFailure);
        }
    }

    public void RecvPacket_LoginSuccess(MsgReqLogin pReq, MsgAnsLogin pAns)
    {
        NetLog.Log("NetComponent_Login : RecvPacket_LoginSuccess");

        NetworkManager.Instance.m_strAccessToken = pAns.S_AccessToken;
        MyInfo.Instance.m_strAccessToken = pAns.S_AccessToken;
        MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic = pAns.User;
        MyInfo.Instance.m_nUserIndex = pAns.User.Uid;
        MyInfo.Instance.m_strUserName = pAns.User.S_PlayerName;

        if (pAns.User.I_UserAvatar == 0)
        {
            MyInfo.Instance.m_nProfileAvatar = 100001;
        }
        else
        {
            MyInfo.Instance.m_nProfileAvatar = pAns.User.I_UserAvatar;
        }

        int nTimeStamp = pAns.I_ServerTime;
        DateTime dateTime = new DateTime(1970, 1, 1, 9, 0, 0, 0);
        EspressoInfo.Instance.m_pConnectDateTime = dateTime.AddSeconds(nTimeStamp);
        TimerManager.Instance.AddTimer(eTimerType.eLoginAfterTime, float.MaxValue, 0, false);

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_LoginFailure(MsgReqLogin pReq, Error pError)
    {
        NetLog.Log("NetComponent_Login : RecvPacket_LoginFailure");

        GetFailureEvent().OnEvent(pError);
    }
}