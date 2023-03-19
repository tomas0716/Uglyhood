using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetModeTeamDeck : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    private eGameMode m_eGameMode = eGameMode.MainStage;

    public NetComponent_GetModeTeamDeck(eGameMode eMode)
    {
        m_eGameMode = eMode;
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
        NetLog.Log("NetComponent_GetModeTeamDeck : OnEvent");

        MsgReqGetModeTeamDeck pReq = new MsgReqGetModeTeamDeck();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_ModeId = (int)m_eGameMode;

        SendPacket<MsgReqGetModeTeamDeck, MsgAnsGetModeTeamDeck>(pReq, RecvPacket_GetModeTeamDeckSuccess, RecvPacket_GetModeTeamDeckFailure);
    }

    public void RecvPacket_GetModeTeamDeckSuccess(MsgReqGetModeTeamDeck pReq, MsgAnsGetModeTeamDeck pAns)
    {
        NetLog.Log("NetComponent_GetModeTeamDeck : RecvPacket_GetModeTeamDeckSuccess");

        InventoryInfoManager.Instance.m_pDeckInfo.SetGameModeDeckID(m_eGameMode, pAns.I_DeckId);

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetModeTeamDeckFailure(MsgReqGetModeTeamDeck pReq, Error pError)
    {
        if (pError.Err_code == errorConstants.kEmptyData)
        {
            NetLog.Log("NetComponent_GetModeTeamDeck : RecvPacket_GetModeTeamDeckSuccess");

            InventoryInfoManager.Instance.m_pDeckInfo.SetGameModeDeckID(m_eGameMode, 0);
            GetSuccessEvent().OnEvent(EventArg_Null.Object);
            return;
        }

        NetLog.Log("NetComponent_GetModeTeamDeck : RecvPacket_GetModeTeamDeckFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
