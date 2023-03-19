using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GameChallengeFinish : TNetComponent_SuccessOrFail<EventArg_GameChallengeFinish, EventArg_Null, Error>
{
    private int m_nDestroyColonyReward = 0;
    private Dictionary<int, int> m_pRewardCount_byTableID = new Dictionary<int, int>();
    private EventArg_GameChallengeFinish m_pEventArg_GameChallengeFinish;

    public NetComponent_GameChallengeFinish()
    {

    }

    public override void OnDestroy()
    {
    }

    public override void LateUpdate()
    {
    }

    public override void Update()
    {
    }

    private void ClearInfo()
    {
        m_nDestroyColonyReward = 0;
        m_pRewardCount_byTableID.Clear();
    }

    public Dictionary<int,int> GetRewardCountTable()
    {
        return m_pRewardCount_byTableID;
    }

    public int GetDestroyColonyReward()
    {
        return m_nDestroyColonyReward;
    }

    public override void OnEvent(EventArg_GameChallengeFinish Arg)
    {
        NetLog.Log("NetComponent_GameChallengeFinish : OnEvent");

        MsgReqGameChallengeFinish pReq = new MsgReqGameChallengeFinish();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_EpisodeId = Arg.m_nEpisodeID;
        pReq.I_ChapterId = Arg.m_nChapterID;
        pReq.I_StageId = Arg.m_nStageID;
        pReq.I_IsClear = Arg.m_nIsClear;
        pReq.I_DestroyColonyCount = Arg.m_nDestroyColonyCount;
        pReq.I_MaxComboCount = Arg.m_nMaxComboCount;
        pReq.I_IsMissionClear_1 = Arg.m_nIsMissionClear_1;
        pReq.I_IsMissionClear_2 = Arg.m_nIsMissionClear_2;
        pReq.I_IsMissionClear_3 = Arg.m_nIsMissionClear_3;

        SendPacket<MsgReqGameChallengeFinish, MsgAnsGameChallengeFinish>(pReq, RecvPacket_GameChallengeFinishSuccess, RecvPacket_GameChallengeFinishFailure);
    }

    public void RecvPacket_GameChallengeFinishSuccess(MsgReqGameChallengeFinish pReq, MsgAnsGameChallengeFinish pAns)
    {
        NetLog.Log("NetComponent_GameChallengeFinish : RecvPacket_GameChallengeFinishSuccess");

        ClearInfo();

        ItemInvenItemInfo pItemInfo = InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemInfo_byTableID((int)eItemType.Energy);
        pItemInfo.m_nItemCount = pAns.I_Ap;
        InventoryInfoManager.Instance.m_pItemInvenInfo.SetItemInvenItemInfo(pItemInfo);

        m_nDestroyColonyReward = pAns.I_DestroyColonyReward;

        foreach (KeyValuePair<int, GameReward> pInfo in pAns.M_GameReward)
        {
            if (pInfo.Value.I_RewardKind == (int)eStageClearRewardType.Item)
            {
                pItemInfo = InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemInfo_byTableID(pInfo.Value.I_RewardId);

                if (pItemInfo != null)
                {
                    pItemInfo.m_nItemCount += pInfo.Value.I_RewardCount;
                }
                else
                {
                    ExcelData_ItemInfo pItemExcelInfo = ExcelDataManager.Instance.m_pItem.GetItemInfo_byID(pInfo.Value.I_RewardId);

                    if (pItemExcelInfo != null)
                    {
                        ItemInvenItemInfo pTempItemInfo = new ItemInvenItemInfo(pInfo.Value.I_RewardId, pInfo.Value.I_RewardSeq, 0, pInfo.Value.I_RewardCount, pItemExcelInfo.m_eItemType);
                        InventoryInfoManager.Instance.m_pItemInvenInfo.SetItemInvenItemInfo(pTempItemInfo);
                    }
                }

                m_pRewardCount_byTableID.Add(pInfo.Value.I_RewardId, pInfo.Value.I_RewardCount);

                //ExcelData_ItemInfo pExcelData_ItemInfo = ExcelDataManager.Instance.m_pItem.GetItemInfo_byID(pInfo.Value.I_RewardId);
                //if (pExcelData_ItemInfo != null)
                //{
                //    switch (pExcelData_ItemInfo.m_eItemType)
                //    {
                //        case eItemType.Stuff:
                //            {
                //                Parameter[] parameters = new Parameter[4];

                //                parameters[0] = new Parameter("item_id", pInfo.Value.I_RewardId);
                //                parameters[1] = new Parameter("count", pInfo.Value.I_RewardCount);
                //                parameters[2] = new Parameter("earned_by", "main_stage_reward");
                //                parameters[3] = new Parameter("detail_id", m_pEventArg_InGameFinish.m_nStageID);

                //                Helper.FirebaseLogEvent("stuff_item_earn", parameters);
                //            }
                //            break;

                //        case eItemType.IngameBooster:
                //            {
                //                Parameter[] parameters = new Parameter[4];

                //                parameters[0] = new Parameter("item_id", pInfo.Value.I_RewardId);
                //                parameters[1] = new Parameter("count", pInfo.Value.I_RewardCount);
                //                parameters[2] = new Parameter("earned_by", "main_stage_reward");
                //                parameters[3] = new Parameter("detail_id", m_pEventArg_InGameFinish.m_nStageID);

                //                Helper.FirebaseLogEvent("booster_item_earn", parameters);
                //            }
                //            break;
                //    }
                //}
            }
            else if (pInfo.Value.I_RewardKind == (int)eStageClearRewardType.Gem_Free)
            {
                pItemInfo = InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemInfo_byTableID((int)eStageClearRewardType.Gem_Free);

                pItemInfo.m_nItemCount += pInfo.Value.I_RewardCount;

                m_pRewardCount_byTableID.Add((int)eStageClearRewardType.Gem_Free, pInfo.Value.I_RewardCount);
            }
            else
            {
                pItemInfo = InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemInfo_byTableID((int)eStageClearRewardType.Gold + 1);

                pItemInfo.m_nItemCount += pInfo.Value.I_RewardCount;
                pItemInfo.m_nItemCount += m_nDestroyColonyReward;

                m_pRewardCount_byTableID.Add((int)eStageClearRewardType.Gold + 1, pInfo.Value.I_RewardCount);

                //Parameter[] parameters = new Parameter[3];
                //parameters[0] = new Parameter("count", pInfo.Value.I_RewardCount - m_nDestroyColonyReward);
                //parameters[1] = new Parameter("earned_by", "main_stage_reward");
                //parameters[2] = new Parameter("detail_id", m_pEventArg_InGameFinish.m_nStageID);
                //Helper.FirebaseLogEvent("gold_earn", parameters);
            }
        }

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GameChallengeFinishFailure(MsgReqGameChallengeFinish pReq, Error error)
    {
        NetLog.Log("NetComponent_GameChallengeFinish : RecvPacket_GameChallengeFinishFailure");

        GetFailureEvent().OnEvent(error);
    }
}
