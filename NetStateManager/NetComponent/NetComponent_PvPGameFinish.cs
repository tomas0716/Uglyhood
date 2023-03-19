using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_PvPGameFinish : TNetComponent_SuccessOrFail<EventArg_PvPGameFinish, EventArg_Null, Error>
{
    private Dictionary<int, int> m_pRewardCount_byTableID = new Dictionary<int, int>();

    public NetComponent_PvPGameFinish()
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
        m_pRewardCount_byTableID.Clear();
    }

    public Dictionary<int,int> GetRewardCountTable()
    {
        return m_pRewardCount_byTableID;
    }

    public override void OnEvent(EventArg_PvPGameFinish Arg)
    {
        NetLog.Log("NetComponent_PvPGameFinish : OnEvent");

        MsgReqGamePvpFinish pReq = new MsgReqGamePvpFinish();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_GameSeq = InventoryInfoManager.Instance.m_pSpecialStageInvenInfo.GetPvPSeqNumber();
        pReq.I_IsResult = Arg.m_nResult;

        SendPacket<MsgReqGamePvpFinish, MsgAnsGamePvpFinish>(pReq, RecvPacket_PvPGameFinishSuccess, RecvPacket_PvPGameFinishFailure);
    }

    public void RecvPacket_PvPGameFinishSuccess(MsgReqGamePvpFinish pReq, MsgAnsGamePvpFinish pAns)
    {
        NetLog.Log("NetComponent_PvPGameFinish : RecvPacket_PvPGameFinishSuccess");

        ClearInfo();

        ItemInvenItemInfo pItemInfo = null;

        foreach(KeyValuePair<int, GameReward> pInfo in pAns.M_GameReward)
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

    public void RecvPacket_PvPGameFinishFailure(MsgReqGamePvpFinish pReq, Error error)
    {
        NetLog.Log("NetComponent_PvPGameFinish : RecvPacket_PvPGameFinishFailure");

        GetFailureEvent().OnEvent(error);
    }
}
