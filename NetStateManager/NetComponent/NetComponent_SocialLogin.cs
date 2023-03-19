using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;

public class NetComponent_SocialLogin : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_SocialLogin()
    {
    }

    public override void OnDestroy()
    {
        AppInstance.Instance.m_pEventDelegateManager.OnEventFlatform_LoginResult -= OnFlatform_LoginResult;
    }

    public override void Update()
    {
    }

    public override void LateUpdate()
    {
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        NetLog.Log("NetComponent_SocialLogin : OnEvent");

#if !UNITY_EDITOR
        switch (SaveDataInfo.Instance.m_eNetwork_LoginBind)
        {
            case eNetwork_LoginBind.None:
                {
                    AppInstance.Instance.m_pEventDelegateManager.OnEventFlatform_LoginResult += OnFlatform_LoginResult;
                    EventDelegateManager.Instance.OnShow_LoginMenu();
                }
                break;
            default:
                {
                    GetSuccessEvent().OnEvent(EventArg_Null.Object);
                }
                break;
        }
#else
        SaveDataInfo.Instance.m_strNetwork_LoginAccountID = OptionInfo.Instance.m_strDeviceUID;
        GetSuccessEvent().OnEvent(EventArg_Null.Object);
#endif
    }

    public void OnFlatform_LoginResult(bool IsSuccess, eNetwork_LoginBind eLoginBind, string strAccountID)
    {
        AppInstance.Instance.m_pEventDelegateManager.OnEventFlatform_LoginResult -= OnFlatform_LoginResult;

        AppInstance.Instance.StartCoroutine(Co_OnFlatform_LoginResult(IsSuccess, eLoginBind, strAccountID));
    }

    IEnumerator Co_OnFlatform_LoginResult(bool IsSuccess, eNetwork_LoginBind eLoginBind, string strAccountID)
    {
        yield return new WaitForEndOfFrame();

        if (IsSuccess == true)
        {
            SaveDataInfo.Instance.m_eNetwork_LoginBind = eLoginBind;
            SaveDataInfo.Instance.m_strNetwork_LoginAccountID = strAccountID;
            SaveDataInfo.Instance.Save();

            GetSuccessEvent().OnEvent(EventArg_Null.Object);
        }
    }
}