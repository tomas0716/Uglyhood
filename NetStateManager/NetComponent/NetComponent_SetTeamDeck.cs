using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_SetTeamDeck : TNetComponent_SuccessOrFail<EventArg_SetTeamDeck, EventArg_Null, Error>
{
    private EventArg_SetTeamDeck m_pEventArg_SetTeamDeck;

    public NetComponent_SetTeamDeck()
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

    public override void OnEvent(EventArg_SetTeamDeck Arg)
    {
        NetLog.Log("NetComponent_SetTeamDeck : OnEvent");

        m_pEventArg_SetTeamDeck = Arg;

        MsgReqSetTeamDeck pReq = new MsgReqSetTeamDeck();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_DeckId = Arg.m_nTeamDeckID;
        pReq.I_Slot0UnitSeq = Arg.m_nUnitSeq[0];
        pReq.I_Slot1UnitSeq = Arg.m_nUnitSeq[1];
        pReq.I_Slot2UnitSeq = Arg.m_nUnitSeq[2];
        pReq.I_Slot3UnitSeq = Arg.m_nUnitSeq[3];
        pReq.I_Slot4UnitSeq = Arg.m_nUnitSeq[4];
        pReq.I_Slot5UnitSeq = Arg.m_nUnitSeq[5];
        pReq.I_RuneDeckId = Arg.m_nRuneDeckID;
        pReq.I_BoosterItemDeckId = Arg.m_nBoosterItemDeckID;

        SendPacket<MsgReqSetTeamDeck, MsgAnsSetTeamDeck>(pReq, RecvPacket_SetTeamDeckSuccess, RecvPacket_SetTeamDeckFailure);
    }

    public void RecvPacket_SetTeamDeckSuccess(MsgReqSetTeamDeck pReq, MsgAnsSetTeamDeck pAns)
    {
        NetLog.Log("NetComponent_SetTeamDeck : RecvPacket_SetTeamDeckSuccess");

        TeamInvenInfo pTeamInvenInfo = InventoryInfoManager.Instance.m_pTeamInvenInfoGroup.GetTeamInvenInfo(m_pEventArg_SetTeamDeck.m_nTeamDeckID);

        if (pTeamInvenInfo == null)
        {
            pTeamInvenInfo = new TeamInvenInfo(m_pEventArg_SetTeamDeck.m_nTeamDeckID, m_pEventArg_SetTeamDeck.m_nTeamDeckID);
            InventoryInfoManager.Instance.m_pTeamInvenInfoGroup.AddTeamInvenInfo(pTeamInvenInfo);
        }

        //이상하게 초기화 후에 넣어주는건 m_TeamSlotTable에 값이 정상적으로 들어감
        pTeamInvenInfo.ClearInvenItemInfo();

        for (int i = 0; i < GameDefine.ms_nTeamCharacterCount; ++i)
        {
            if (m_pEventArg_SetTeamDeck.m_nUnitSeq[i] != 0)
            {
                CharacterInvenItemInfo pCharacterInvenItemInfo = InventoryInfoManager.Instance.m_pCharacterInvenInfo.GetInvenItem_byUniqueID(m_pEventArg_SetTeamDeck.m_nUnitSeq[i]);

                if (pCharacterInvenItemInfo != null)
                {
                    pTeamInvenInfo.AddInvenItemInfo(i, pCharacterInvenItemInfo);
                }
                else
                {
                    pTeamInvenInfo.DeleteInvenItemInfo(i);
                }
            }
            else
            {
                pTeamInvenInfo.DeleteInvenItemInfo(i);
            }
        }

        //InventoryInfoManager.Instance.m_pCharacterInvenInfo.GetInvenItem_byTableID(100001);

        //InventoryInfoManager.Instance.m_pCharacterInvenInfo 최신화?

        Debug.Log(InventoryInfoManager.Instance.m_pCharacterInvenInfo.GetInvenItem_byTableID(100001));
        Debug.Log(InventoryInfoManager.Instance.m_pEdit_CharacterInvenInfo.GetInvenItem_byTableID(100001));

        List<CharacterInvenItemInfo> pList = InventoryInfoManager.Instance.m_pEdit_CharacterInvenInfo.GetInvenItem_All();

        InventoryInfoManager.Instance.m_pCharacterInvenInfo.ClearAll();

        for (int i = 0; i < pList.Count; i++)
        {
            InventoryInfoManager.Instance.m_pCharacterInvenInfo.AddInvenItemInfo(pList[i]);
        }

        InventoryInfoManager.Instance.m_pDeckInfo.SetDeck(m_pEventArg_SetTeamDeck.m_nTeamDeckID, m_pEventArg_SetTeamDeck.m_nRuneDeckID, m_pEventArg_SetTeamDeck.m_nBoosterItemDeckID);

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_SetTeamDeckFailure(MsgReqSetTeamDeck pReq, Error pError)
    {
        NetLog.Log("NetComponent_SetTeamDeck : RecvPacket_SetTeamDeckFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
