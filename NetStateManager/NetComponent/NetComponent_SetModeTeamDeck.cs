using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_SetModeTeamDeck : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    private eGameMode   m_eGameMode = eGameMode.MainStage;
    private int         m_nDeckID   = 0;

    public NetComponent_SetModeTeamDeck()
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

    public void SetModeTeamDeck(eGameMode eMode, int nDeckID)
    {
        m_eGameMode = eMode;
        m_nDeckID = nDeckID;
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        NetLog.Log("NetComponent_SetModeTeamDeck : OnEvent");

        MsgReqSetModeTeamDeck pReq = new MsgReqSetModeTeamDeck();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_ModeId = (int)m_eGameMode;
        pReq.I_DeckId = m_nDeckID;

        SendPacket<MsgReqSetModeTeamDeck, MsgAnsSetModeTeamDeck>(pReq, RecvPacket_SetModeTeamDeckSuccess, RecvPacket_SetModeTeamDeckFailure);
    }

    public void RecvPacket_SetModeTeamDeckSuccess(MsgReqSetModeTeamDeck pReq, MsgAnsSetModeTeamDeck pAns)
    {
        NetLog.Log("NetComponent_SetModeTeamDeck : RecvPacket_SetModeTeamDeckSuccess");

        InventoryInfoManager.Instance.m_pDeckInfo.SetGameModeDeckID(m_eGameMode, m_nDeckID);
        
        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_SetModeTeamDeckFailure(MsgReqSetModeTeamDeck pReq, Error pError)
    {
        NetLog.Log("NetComponent_SetModeTeamDeck : RecvPacket_SetModeTeamDeckFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
