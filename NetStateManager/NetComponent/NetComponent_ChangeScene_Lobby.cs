using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetComponent_ChangeScene_Lobby : TNetComponent_Next<EventArg_Null, EventArg_Null>
{
    public NetComponent_ChangeScene_Lobby()
    {
    }

    public override void OnDestroy()
    {
        EventDelegateManager.Instance.OnEventLobby_EnterLobbyDone -= OnLobby_EnterLobbyDone;
    }

    public override void Update()
    {
    }

    public override void LateUpdate()
    {
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        NetLog.Log("NetComponent_ChangeScene_Lobby : OnEvent");

        EventDelegateManager.Instance.OnEventLobby_EnterLobbyDone += OnLobby_EnterLobbyDone;

        AppInstance.Instance.m_pSceneManager.ChangeScene(eSceneType.Scene_Lobby, true, 0, 0.3f);
        //AppInstance.Instance.m_pSceneManager.OnCreateLobbyLoading();
    }

    public void OnLobby_EnterLobbyDone()
    {
        EventDelegateManager.Instance.OnEventLobby_EnterLobbyDone -= OnLobby_EnterLobbyDone;
        GetNextEvent().OnEvent(EventArg_Null.Object);
    }
}
