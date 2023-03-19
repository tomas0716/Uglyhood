using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GameEventPvpFinish : TNetComponent_SuccessOrFail<EventArg_GameEventPvpFinish, EventArg_Null, Error>
{
    private Dictionary<int, int> m_pRewardCount_byTableID = new Dictionary<int, int>();

    public NetComponent_GameEventPvpFinish()
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

    public Dictionary<int, int> GetRewardCountTable()
    {
        return m_pRewardCount_byTableID;
    }

    public override void OnEvent(EventArg_GameEventPvpFinish Arg)
    {
        NetLog.Log("NetComponent_GameEventPvpFinish : OnEvent");

        MsgReqGameEventPvpFinish pReq = new MsgReqGameEventPvpFinish();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_GameSeq = Arg.m_nGameSeq;
        pReq.I_IsResult = Arg.m_nResult;

        SendPacket<MsgReqGameEventPvpFinish, MsgAnsGameEventPvpFinish>(pReq, RecvPacket_GameEventPvpFinishSuccess, RecvPacket_GameEventPvpFinishFailure);
    }

    public void RecvPacket_GameEventPvpFinishSuccess(MsgReqGameEventPvpFinish pReq, MsgAnsGameEventPvpFinish pAns)
    {
        NetLog.Log("NetComponent_GameEventPvpFinish : RecvPacket_GameEventPvpFinishSuccess");

        ClearInfo();

        ItemInvenItemInfo pItemInfo = null;

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
                    m_pRewardCount_byTableID[(int)eItemType.Gem_Free] += pInfo.Value.I_RewardCount;
                }
                else
                {
                    m_pRewardCount_byTableID.Add((int)eItemType.Gem_Free, pInfo.Value.I_RewardCount);
                }
            }
            else if(pInfo.Value.I_RewardKind == (int)eStageClearRewardType.Gem_Paid)
            {
                if (m_pRewardCount_byTableID.ContainsKey((int)eItemType.Gem_Paid))
                {
                    m_pRewardCount_byTableID[(int)eItemType.Gem_Paid] += pInfo.Value.I_RewardCount;
                }
                else
                {
                    m_pRewardCount_byTableID.Add((int)eItemType.Gem_Paid, pInfo.Value.I_RewardCount);
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

    public void RecvPacket_GameEventPvpFinishFailure(MsgReqGameEventPvpFinish pReq, Error error)
    {
        NetLog.Log("NetComponent_GameEventPvpFinish : RecvPacket_GameEventPvpFinishFilure");

        GetFailureEvent().OnEvent(error);
    }
}
