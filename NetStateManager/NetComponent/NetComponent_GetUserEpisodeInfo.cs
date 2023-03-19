using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetUserEpisodeInfo : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_GetUserEpisodeInfo()
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
        NetLog.Log("NetComponent_GetUserEpisodeInfo : OnEvent");

        MsgReqGetUserGameEpisode pReq = new MsgReqGetUserGameEpisode();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_EpisodeId = 1;

        SendPacket<MsgReqGetUserGameEpisode, MsgAnsGetUserGameEpisode>(pReq, RecvPacket_GetUserEpisodeInfoSuccess, RecvPacket_GetUserEpisodeInfoFailure);
    }

    public void RecvPacket_GetUserEpisodeInfoSuccess(MsgReqGetUserGameEpisode pReq, MsgAnsGetUserGameEpisode pAns)
    {
        NetLog.Log("NetComponent_GetUserEpisodeInfo : RecvPacket_GetUserEpisodeInfoSuccess");

        //InventoryInfoManager.Instance.m_pMainStageInvenInfo.Init();

        InventoryInfoManager.Instance.m_pMainStageInvenInfo.ClearChapterStarReward();
        InventoryInfoManager.Instance.m_pMainStageInvenInfo.ClearChapterStarCount();

        // 챕터 언락 리스트 추가
        foreach(KeyValuePair<int,UserEpisode> pInfo in pAns.M_UserEpisode)
        {
            // 에피소드 추가

            if (pInfo.Value.I_IsOpen == 1)
            {
                InventoryInfoManager.Instance.m_pMainStageInvenInfo.SetChapterUnlock_byChapterID(pInfo.Value.I_ChapterId, true);
            }
            else
            {
                InventoryInfoManager.Instance.m_pMainStageInvenInfo.SetChapterUnlock_byChapterID(pInfo.Value.I_ChapterId, false);
            }

            ChapterStarReward pChapterInfo = new ChapterStarReward();
            pChapterInfo.m_nChapterID = pInfo.Value.I_ChapterId;
            
            if(pInfo.Value.I_IsClear == 0)
            {
                pChapterInfo.m_IsClear = false;
            }
            else
            {
                pChapterInfo.m_IsClear = true;
            }

            if(pInfo.Value.I_IsOpen == 0)
            {
                pChapterInfo.m_IsOpen = false;
            }
            else
            {
                pChapterInfo.m_IsOpen = true;
            }

            pChapterInfo.m_nStarReward = pInfo.Value.I_StarReward;
            pChapterInfo.m_nMissionReward = pInfo.Value.I_MissionReward;

            InventoryInfoManager.Instance.m_pMainStageInvenInfo.SetChapterStarReward_byChapter(pChapterInfo);
        }

        foreach(KeyValuePair<int,UserChapterStar> pInfo in pAns.M_UserChapterStar)
        {
            InventoryInfoManager.Instance.m_pMainStageInvenInfo.SetStarCount_byChapter(pInfo.Value.I_ChapterId, pInfo.Value.I_NowStarCount,pInfo.Value.I_NowMissionCount);
        }

        if (!InventoryInfoManager.Instance.m_pMainStageInvenInfo.GetChapterUnlock_byChapter(EspressoInfo.Instance.m_nChapterID))
        {
            Dictionary<int, bool> pChapterUnlockTable = InventoryInfoManager.Instance.m_pMainStageInvenInfo.GetChapterUnlockList();

            foreach (KeyValuePair<int, bool> pChapterUnlockInfo in pChapterUnlockTable)
            {
                if (pChapterUnlockInfo.Value == true)
                {
                    EspressoInfo.Instance.m_nChapterID = pChapterUnlockInfo.Key;
                    SaveDataInfo.Instance.m_nRecentSelectChapter = EspressoInfo.Instance.m_nChapterID;
                    SaveDataInfo.Instance.RecentSelectChapterSave();
                    break;
                }
            }
        }

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetUserEpisodeInfoFailure(MsgReqGetUserGameEpisode pReq, Error pError)
    {
        NetLog.Log("NetComponent_GetUserEpisodeInfo : RecvPacket_GetUserEpisodeInfoFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
