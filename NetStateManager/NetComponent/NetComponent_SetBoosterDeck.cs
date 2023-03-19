using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_SetBoosterDeck : TNetComponent_SuccessOrFail<EventArg_SetBoosterDeck, EventArg_Null, Error>
{
    private EventArg_SetBoosterDeck m_pEventArg_SetBoosterDeck;

    public NetComponent_SetBoosterDeck()
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

    public override void OnEvent(EventArg_SetBoosterDeck Arg)
    {
        NetLog.Log("NetComponent_SetBoosterDeck : OnEvent");

        m_pEventArg_SetBoosterDeck = Arg;

        MsgReqSetBoosterDeck pReq = new MsgReqSetBoosterDeck();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_DeckId = Arg.m_nBoosterDeckID;
        pReq.I_Slot0BoosterId = Arg.m_nBoosterIDs[0];
        pReq.I_Slot1BoosterId = Arg.m_nBoosterIDs[1];
        pReq.I_Slot2BoosterId = Arg.m_nBoosterIDs[2];
        pReq.S_DeckName = Arg.m_strName;

        SendPacket<MsgReqSetBoosterDeck, MsgAnsSetBoosterDeck>(pReq, RecvPacket_SetBoosterDeckSuccess, RecvPacket_SetBoosterDeckFailure);
    }

    public void RecvPacket_SetBoosterDeckSuccess(MsgReqSetBoosterDeck pReq, MsgAnsSetBoosterDeck pAns)
    {
        NetLog.Log("NetComponent_SetBoosterDeck : RecvPacket_SetBoosterDeckSuccess");

        for (int i = 0; i < GameDefine.ms_nMaxBoosterItemCount; ++i)
        {
            if (m_pEventArg_SetBoosterDeck.m_nBoosterIDs[i] != 0)
            {
                BoosterItemInvenItemInfo pBoosterItemInvenItemInfo = new BoosterItemInvenItemInfo();
                pBoosterItemInvenItemInfo.m_nTableID = m_pEventArg_SetBoosterDeck.m_nBoosterIDs[i];
                InventoryInfoManager.Instance.m_pBoosterItemInvenInfoGroup.AddBoosterItemInvenInfo(m_pEventArg_SetBoosterDeck.m_nBoosterDeckID, i, pBoosterItemInvenItemInfo);
            }
            else
            {
                InventoryInfoManager.Instance.m_pBoosterItemInvenInfoGroup.RemoveBoosterItemInvenInfo(m_pEventArg_SetBoosterDeck.m_nBoosterDeckID, i);
            }
        }

        InventoryInfoManager.Instance.m_pBoosterItemInvenInfoGroup.SetBoosterItemSetName(m_pEventArg_SetBoosterDeck.m_nBoosterDeckID, m_pEventArg_SetBoosterDeck.m_strName);

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_SetBoosterDeckFailure(MsgReqSetBoosterDeck pReq, Error pError)
    {
        NetLog.Log("NetComponent_SetBoosterDeck : RecvPacket_SetBoosterDeckFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
