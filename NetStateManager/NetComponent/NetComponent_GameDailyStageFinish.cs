using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GameDailyStageFinish : TNetComponent_SuccessOrFail<EventArg_DailyStageEnd, EventArg_Null, Error>
{
    private int m_nDestroyColonyReward = 0;
    private Dictionary<int, int> m_pRewardCount_byTableID = new Dictionary<int, int>();
    private EventArg_DailyStageEnd m_pEventArg_DailyStageEnd;

    public NetComponent_GameDailyStageFinish()
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

    public Dictionary<int, int> GetRewardCountTable()
    {
        return m_pRewardCount_byTableID;
    }

    public int GetDestroyColonyReward()
    {
        return m_nDestroyColonyReward;
    }

    public override void OnEvent(EventArg_DailyStageEnd Arg)
    {
        NetLog.Log("NetComponent_GameDailyStageFinish : OnEvent");
        MsgReqGameDailyStageFinish pReq = new MsgReqGameDailyStageFinish();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_ModeId = Arg.m_nModeID;
        pReq.I_Difficulty = Arg.m_nDifficulty;
        pReq.I_StageId = Arg.m_nStageID;
        pReq.I_PassKind = Arg.m_nPassKind;
        pReq.I_IsClear = Arg.m_nIsClear;
        pReq.I_StarCount = Arg.m_nStarCount;
        pReq.I_DestroyColonyCount = Arg.m_nDestroyColonyCount;

        SendPacket<MsgReqGameDailyStageFinish, MsgAnsGameDailyStageFinish>(pReq, RecvPacket_GameDailyStageFinishSuccess, RecvPacket_GameDailyStqageFinishFailure);
    }

    public void RecvPacket_GameDailyStageFinishSuccess(MsgReqGameDailyStageFinish pReq, MsgAnsGameDailyStageFinish pAns)
    {
        NetLog.Log("NetComponent_GameDailyStageFinish : RecvPacket_GameDailyStageFinishSuccess");

        ClearInfo();

        ItemInvenItemInfo pItemInfo = null;

        m_nDestroyColonyReward = pAns.I_DestroyColonyReward;

        // 게임 보상 아이템 리스트
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

                if (m_pRewardCount_byTableID.ContainsKey(pInfo.Value.I_RewardId))
                {
                    m_pRewardCount_byTableID[pInfo.Value.I_RewardId] += pInfo.Value.I_RewardCount;
                }
                else
                {
                    m_pRewardCount_byTableID.Add(pInfo.Value.I_RewardId, pInfo.Value.I_RewardCount);
                }
            }
            else if (pInfo.Value.I_RewardKind == (int)eStageClearRewardType.Gem_Free)
            {
                pItemInfo = InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemInfo_byTableID((int)eStageClearRewardType.Gem_Free);

                pItemInfo.m_nItemCount += pInfo.Value.I_RewardCount;

                if (m_pRewardCount_byTableID.ContainsKey((int)eStageClearRewardType.Gem_Free))
                {
                    m_pRewardCount_byTableID[(int)eStageClearRewardType.Gem_Free] += pInfo.Value.I_RewardCount;
                }
                else
                {
                    m_pRewardCount_byTableID.Add((int)eStageClearRewardType.Gem_Free, pInfo.Value.I_RewardCount);
                }
            }
            else
            {
                pItemInfo = InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemInfo_byTableID((int)eStageClearRewardType.Gold + 1);

                pItemInfo.m_nItemCount += pInfo.Value.I_RewardCount;
                pItemInfo.m_nItemCount += m_nDestroyColonyReward;

                if (m_pRewardCount_byTableID.ContainsKey((int)eStageClearRewardType.Gold + 1))
                {
                    m_pRewardCount_byTableID[(int)eStageClearRewardType.Gold + 1] += pInfo.Value.I_RewardCount;
                }
                else
                {
                    m_pRewardCount_byTableID.Add((int)eStageClearRewardType.Gold + 1, pInfo.Value.I_RewardCount);
                }
            }
        }

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GameDailyStqageFinishFailure(MsgReqGameDailyStageFinish pReq, Error pError)
    {
        NetLog.Log("NetComponent_GameDailyStageFinish : RecvPacket_GameDailyStageFinishFailure");

        Debug.Log(pError.Err_code);

        GetFailureEvent().OnEvent(pError);
    }
}
