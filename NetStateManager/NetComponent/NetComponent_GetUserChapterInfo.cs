using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetUserChapterInfo : TNetComponent_SuccessOrFail<EventArg_GetUserChapterInfo, EventArg_Null, Error>
{
    public NetComponent_GetUserChapterInfo()
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

    public override void OnEvent(EventArg_GetUserChapterInfo Arg)
    {
        NetLog.Log("NetComponent_GetUserChapterInfo : OnEvent");

        MsgReqGetUserGameChapter pReq = new MsgReqGetUserGameChapter();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_EpisodeId = Arg.m_nEpisodeID;
        pReq.I_ChapterId = Arg.m_nChapterID;

        SendPacket<MsgReqGetUserGameChapter, MsgAnsGetUserGameChapter>(pReq, RecvPacket_GetUserChapterInfoSuccess, RecvPacket_GetUserChapteInfoFailure);
    }

    public void RecvPacket_GetUserChapterInfoSuccess(MsgReqGetUserGameChapter pReq, MsgAnsGetUserGameChapter pAns)
    {
        NetLog.Log("NetComponent_GetUserChapterInfo : RecvPacket_GetUserEpisodeInfoSuccess");

        int nCount = 0;

        foreach(KeyValuePair<int,UserChapter> pInfo in pAns.M_UserChapter)
        {
            if(pInfo.Value.I_IsClear == 0)
            {
                if (nCount == 0 ||
                    InventoryInfoManager.Instance.m_pMainStageInvenInfo.GetStageClearRank_byStage(pInfo.Value.I_StageId - 1) >= 1)
                {
                    InventoryInfoManager.Instance.m_pMainStageInvenInfo.SetStageUnlock_byStageID(pInfo.Value.I_StageId, true);
                    InventoryInfoManager.Instance.m_pMainStageInvenInfo.SetStageClearRank_byStage(pInfo.Value.I_StageId, pInfo.Value.I_StarCount);
                    InventoryInfoManager.Instance.m_pMainStageInvenInfo.SetStageMissionClearRank_byStage(pInfo.Value.I_StageId, pInfo.Value.I_MissionCount);
                }
                else
                {
                    InventoryInfoManager.Instance.m_pMainStageInvenInfo.SetStageUnlock_byStageID(pInfo.Value.I_StageId, false);
                    InventoryInfoManager.Instance.m_pMainStageInvenInfo.SetStageClearRank_byStage(pInfo.Value.I_StageId, pInfo.Value.I_StarCount);
                    InventoryInfoManager.Instance.m_pMainStageInvenInfo.SetStageMissionClearRank_byStage(pInfo.Value.I_StageId, pInfo.Value.I_MissionCount);
                }
            }
            else
            {
                InventoryInfoManager.Instance.m_pMainStageInvenInfo.SetStageUnlock_byStageID(pInfo.Value.I_StageId, true);
                InventoryInfoManager.Instance.m_pMainStageInvenInfo.SetStageClearRank_byStage(pInfo.Value.I_StageId, pInfo.Value.I_StarCount);
                InventoryInfoManager.Instance.m_pMainStageInvenInfo.SetStageMissionClearRank_byStage(pInfo.Value.I_StageId, pInfo.Value.I_MissionCount);
            }

            nCount++;
        }

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetUserChapteInfoFailure(MsgReqGetUserGameChapter pReq, Error pError)
    {
        NetLog.Log("NetComponent_GetUserChapterInfo : RecvPacket_GetUserEpisodeInfoFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
