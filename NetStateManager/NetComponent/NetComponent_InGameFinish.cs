using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase.Analytics;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;
using sg.protocol.common;

public class NetComponent_InGameFinish : TNetComponent_SuccessOrFail<EventArg_InGameFinish, EventArg_Null, Error>
{
    private int m_nDestroyColonyReward = 0;
    private Dictionary<int, int> m_pRewardCount_byTableID = new Dictionary<int, int>();
    private EventArg_InGameFinish m_pEventArg_InGameFinish;

    public NetComponent_InGameFinish()
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

    public override void OnEvent(EventArg_InGameFinish Arg)
    {
        NetLog.Log("NetComponent_InGameFinish : OnEvent");

#if UNITY_EDITOR
        if (GameConfig.Instance.m_IsInGameSuerverCommunication == false)
        {
            GetSuccessEvent().OnEvent(EventArg_Null.Object);
            return;
        }
#endif

        m_pEventArg_InGameFinish = Arg;

        MsgReqGameFinish pReq = new MsgReqGameFinish();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_EpisodeId = Arg.m_nEpisodeID;
        pReq.I_ChapterId = Arg.m_nChapterID;
        pReq.I_StageId = Arg.m_nStageID;
        pReq.I_IsClear = Arg.m_nIsClear;
        pReq.I_StarCount = Arg.m_nStarCount;
        pReq.I_DestroyColonyCount = Arg.m_nDestroyColonyCount;
        pReq.I_MaxComboCount = Arg.m_nMaxCombo;

        SendPacket<MsgReqGameFinish, MsgAnsGameFinish>(pReq, RecvPacket_InGameFinishSuccess, RecvPacket_InGameFinishFailure);

        Parameter[] parameters = new Parameter[3];
        parameters[0] = new Parameter("count", Arg.m_nDestroyColonyCount * 10);
        parameters[1] = new Parameter("earned_by", "main_stage_colony_destroy");
        parameters[2] = new Parameter("detail_id", Arg.m_nStageID);
        Helper.FirebaseLogEvent("gold_earn", parameters);
    }

    public void RecvPacket_InGameFinishSuccess(MsgReqGameFinish pReq, MsgAnsGameFinish pAns)
    {
        NetLog.Log("NetComponent_InGameFinish : RecvPacket_InGameFinishSuccess");

        ClearInfo();

        // 행동력
        ItemInvenItemInfo pItemInfo = InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemInfo_byTableID((int)eItemType.Energy);
        pItemInfo.m_nItemCount = pAns.I_Ap;
        InventoryInfoManager.Instance.m_pItemInvenInfo.SetItemInvenItemInfo(pItemInfo);

        m_nDestroyColonyReward = pAns.I_DestroyColonyReward;

        // 게임 보상 아이템 리스트
        foreach (KeyValuePair<int,GameReward> pInfo in pAns.M_GameReward)
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

                    if(pItemExcelInfo != null)
                    {
                        ItemInvenItemInfo pTempItemInfo = new ItemInvenItemInfo(pInfo.Value.I_RewardId, pInfo.Value.I_RewardSeq, 0, pInfo.Value.I_RewardCount, pItemExcelInfo.m_eItemType);
                        InventoryInfoManager.Instance.m_pItemInvenInfo.SetItemInvenItemInfo(pTempItemInfo);
                    }
                }

                m_pRewardCount_byTableID.Add(pInfo.Value.I_RewardId, pInfo.Value.I_RewardCount);

                ExcelData_ItemInfo pExcelData_ItemInfo = ExcelDataManager.Instance.m_pItem.GetItemInfo_byID(pInfo.Value.I_RewardId);
                if (pExcelData_ItemInfo != null)
                {
                    switch (pExcelData_ItemInfo.m_eItemType)
                    {
                        case eItemType.Stuff:
                            {
                                Parameter[] parameters = new Parameter[4];

                                parameters[0] = new Parameter("item_id", pInfo.Value.I_RewardId);
                                parameters[1] = new Parameter("count", pInfo.Value.I_RewardCount);
                                parameters[2] = new Parameter("earned_by", "main_stage_reward");
                                parameters[3] = new Parameter("detail_id", m_pEventArg_InGameFinish.m_nStageID);

                                Helper.FirebaseLogEvent("stuff_item_earn", parameters);
                            }
                            break;

                        case eItemType.IngameBooster:
                            {
                                Parameter[] parameters = new Parameter[4];

                                parameters[0] = new Parameter("item_id", pInfo.Value.I_RewardId);
                                parameters[1] = new Parameter("count", pInfo.Value.I_RewardCount);
                                parameters[2] = new Parameter("earned_by", "main_stage_reward");
                                parameters[3] = new Parameter("detail_id", m_pEventArg_InGameFinish.m_nStageID);

                                Helper.FirebaseLogEvent("booster_item_earn", parameters);
                            }
                            break;
                    }
                }
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

                Parameter[] parameters = new Parameter[3];
                parameters[0] = new Parameter("count", pInfo.Value.I_RewardCount - m_nDestroyColonyReward);
                parameters[1] = new Parameter("earned_by", "main_stage_reward");
                parameters[2] = new Parameter("detail_id", m_pEventArg_InGameFinish.m_nStageID);
                Helper.FirebaseLogEvent("gold_earn", parameters);
            }
        }

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_InGameFinishFailure(MsgReqGameFinish pReq, Error pError)
    {
        NetLog.Log("NetComponent_InGameFinish : RecvPacket_InGameFinishFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
