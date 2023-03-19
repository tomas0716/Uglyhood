using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.common;

public class NetComponent_ConnectReward : TNetComponent_Next<EventArg_Null, EventArg_Null>
{
    private Queue<ConnectReward> m_ConnectRewardQueue = new Queue<ConnectReward>();

    public NetComponent_ConnectReward()
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
        NetLog.Log("NetComponent_ConnectReward : OnEvent");

        if (MyInfo.Instance.m_IsConnectReward == false)
        {
            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
        else
        {
            NetLog.Log("NetComponent_ConnectReward : OnEvent, Req");

            MsgReqGetConnectReward pReq = new MsgReqGetConnectReward();
            pReq.Uid = MyInfo.Instance.m_nUserIndex;

            SendPacket<MsgReqGetConnectReward, MsgAnsGetConnectReward>(pReq, RecvPacket_ConnectRewardSuccess, RecvPacket_ConnectRewardFailure);
        }
    }

    public void RecvPacket_ConnectRewardSuccess(MsgReqGetConnectReward pReq, MsgAnsGetConnectReward pAns)
    {
        NetLog.Log("NetComponent_ConnectReward : RecvPacket_ConnectRewardSuccess");

        if (pAns.M_ConnectReward.Count != 0)
        {
            foreach (KeyValuePair<int, ConnectReward> item in pAns.M_ConnectReward)
            {
                m_ConnectRewardQueue.Enqueue(item.Value);

                switch (item.Value.I_RewardKind)
                {
                    case 1:     // 무료 젬
                        {
                            // 무료 젬 아이디로 변경
                            item.Value.I_RewardId = 1;
                        }
                        break;

                    case 4:     // 골드
                        {
                            // 골드 아이디로 변경
                            item.Value.I_RewardId = 3;

                            Parameter[] parameters = new Parameter[3];
                            parameters[0] = new Parameter("count", item.Value.I_RewardCount);
                            parameters[1] = new Parameter("earned_by", "login_bonus");
                            parameters[2] = new Parameter("detail_id", 0);
                            Helper.FirebaseLogEvent("gold_earn", parameters);
                        }
                        break;

                    case 5:     // 아이템
                        {
                            ExcelData_ItemInfo pItemInfo = ExcelDataManager.Instance.m_pItem.GetItemInfo_byID(item.Value.I_RewardId);

                            switch (pItemInfo.m_eItemType)
                            {
                                case eItemType.Stuff:
                                    {
                                        Parameter[] parameters = new Parameter[4];

                                        parameters[0] = new Parameter("item_id", item.Value.I_RewardId);
                                        parameters[1] = new Parameter("count", item.Value.I_RewardCount);
                                        parameters[2] = new Parameter("earned_by", "etc");
                                        parameters[3] = new Parameter("detail_id", 0);

                                        Helper.FirebaseLogEvent("stuff_item_earn", parameters);
                                    }
                                    break;

                                case eItemType.IngameBooster:
                                    {
                                        Parameter[] parameters = new Parameter[4];

                                        parameters[0] = new Parameter("item_id", item.Value.I_RewardId);
                                        parameters[1] = new Parameter("count", item.Value.I_RewardCount);
                                        parameters[2] = new Parameter("earned_by", "etc");
                                        parameters[3] = new Parameter("detail_id", 0);

                                        Helper.FirebaseLogEvent("booster_item_earn", parameters);
                                    }
                                    break;
                            }
                        }
                        break;
                }

                ItemInvenItemInfo pItemInvenItemInfo = InventoryInfoManager.Instance.m_pItemInvenInfo.GetInvenItemInfo_byTableID(item.Value.I_RewardId);

                if (pItemInvenItemInfo != null)
                {
                    pItemInvenItemInfo.m_nItemCount += item.Value.I_RewardCount;
                }
                else
                {
                    ExcelData_ItemInfo pItemInfo = ExcelDataManager.Instance.m_pItem.GetItemInfo_byID(item.Value.I_RewardId);

                    if (pItemInfo != null)
                    {
                        pItemInvenItemInfo = new ItemInvenItemInfo(item.Value.I_RewardId, item.Value.I_RewardSeq, item.Key, item.Value.I_RewardCount, pItemInfo.m_eItemType);
                        InventoryInfoManager.Instance.m_pItemInvenInfo.SetItemInvenItemInfo(pItemInvenItemInfo);
                    }
                }
            }

            EventDelegateManager.Instance.OnLobby_RefreshCurrencyData();

            NextRewardPopup();
        }
        else
        {
            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
    }

    public void RecvPacket_ConnectRewardFailure(MsgReqGetConnectReward pReq, Error pError)
    {
        NetLog.Log("NetComponent_ConnectReward : RecvPacket_ConnectRewardFailure");

        GetNextEvent().OnEvent(EventArg_Null.Object);
    }

    public void NextRewardPopup()
    {
        if (m_ConnectRewardQueue.Count != 0)
        {
            ConnectReward pConnectReward = m_ConnectRewardQueue.Dequeue();
            UIHelper.OnCommonItemMessagePopupOpen(ExcelDataHelper.GetString("LOGIN_BONUS_POPUP_TITLE"),
                                                  eRewardType.Item,
                                                  pConnectReward.I_RewardId,
                                                  pConnectReward.I_RewardCount,
                                                  ExcelDataHelper.GetString("LOGIN_BONUS_POPUP_DESC"),
                                                  ExcelDataHelper.GetString("LOGIN_BONUS_POPUP_BUTTON_OK"),
                                                  OnButtonClick_Confirm);
        }
        else
        {
            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
    }

    public void OnButtonClick_Confirm()
    {
        NextRewardPopup();
    }
}
