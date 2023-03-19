using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_TeamDeck : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_TeamDeck()
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
        NetLog.Log("NetComponent_TeamDeck : OnEvent");

        MsgReqTeamDeck pReq = new MsgReqTeamDeck();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqTeamDeck, MsgAnsTeamDeck>(pReq, RecvPacket_TeamDeckSuccess, RecvPacket_TeamDeckFailure);
    }

    public void RecvPacket_TeamDeckSuccess(MsgReqTeamDeck pReq, MsgAnsTeamDeck pAns)
    {
        NetLog.Log("NetComponent_TeamDeck : RecvPacket_TeamDeckSuccess");

        InventoryInfoManager.Instance.m_pTeamInvenInfoGroup.ClearAll();

        // Character Deck
        foreach (KeyValuePair<int, UserTeamDeck> item in pAns.M_UserTeamDeck)
        {
            TeamInvenInfo pTeamInvenInfo = new TeamInvenInfo(item.Value.I_DeckId, item.Key);
            pTeamInvenInfo.SetTeamPower(item.Value.I_TeamPoint);

            List<int> checkTeam = new List<int>();

            int [] slotUnitSeqs = { item.Value.I_Slot0UnitSeq, item.Value.I_Slot1UnitSeq, item.Value.I_Slot2UnitSeq, item.Value.I_Slot3UnitSeq, item.Value.I_Slot4UnitSeq, item.Value.I_Slot5UnitSeq };

            for (int i = 0; i < GameDefine.ms_nTeamCharacterCount; ++i)
            {
                if (slotUnitSeqs[i] != 0 && checkTeam.Contains(slotUnitSeqs[i]) == false)
                {
                    checkTeam.Add(slotUnitSeqs[i]);

                    CharacterInvenItemInfo pCharacterInvenItemInfo = InventoryInfoManager.Instance.m_pCharacterInvenInfo.GetInvenItem_byUniqueID(slotUnitSeqs[i]);

                    if (pCharacterInvenItemInfo != null)
                    {
                        pTeamInvenInfo.AddInvenItemInfo(i, pCharacterInvenItemInfo);
                    }
                }
            }

            InventoryInfoManager.Instance.m_pTeamInvenInfoGroup.AddTeamInvenInfo(pTeamInvenInfo);

            InventoryInfoManager.Instance.m_pDeckInfo.SetDeck(item.Value.I_DeckId, item.Value.I_RuneDeckId, item.Value.I_BoosterItemDeckId);
        }

        // Rune Deck
        foreach (KeyValuePair<int, UserRuneDeck> item in pAns.M_UserRuneDeck)
        {
        }

        // BoosterItem Deck
        foreach (KeyValuePair<int, UserBoosterDeck> item in pAns.M_UserBoosterDeck)
        {
            InventoryInfoManager.Instance.m_pBoosterItemInvenInfoGroup.SetBoosterItemDeckUniqueID(item.Value.I_DeckId, item.Value.I_DeckSeq);
            InventoryInfoManager.Instance.m_pBoosterItemInvenInfoGroup.SetBoosterItemSetName(item.Value.I_DeckId, item.Value.S_DeckName);
            InventoryInfoManager.Instance.m_pBoosterItemInvenInfoGroup.SetBoosterItemDB_Seq(item.Value.I_DeckId, item.Key);

            int[] tableIDs = { item.Value.I_Slot0BoosterId, item.Value.I_Slot1BoosterId, item.Value.I_Slot2BoosterId };

            for (int i = 0; i < GameDefine.ms_nMaxBoosterItemCount; ++i)
            {
                BoosterItemInvenItemInfo pItemInvenInfo = new BoosterItemInvenItemInfo();
                pItemInvenInfo.m_nTableID = tableIDs[i];

                if (pItemInvenInfo.m_nTableID != 0)
                {
                    InventoryInfoManager.Instance.m_pBoosterItemInvenInfoGroup.AddBoosterItemInvenInfo(item.Value.I_DeckId, i, pItemInvenInfo);
                }
            }
        }

        InventoryInfoManager.Instance.m_pBoosterItemInvenInfoGroup.OnResetEquipCount();

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_TeamDeckFailure(MsgReqTeamDeck pReq, Error pError)
    {
        NetLog.Log("NetComponent_TeamDeck : RecvPacket_TeamDeckFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
