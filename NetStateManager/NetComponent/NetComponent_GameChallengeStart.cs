using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GameChallengeStart : TNetComponent_SuccessOrFail<EventArg_GameChallengeStart,EventArg_Null,Error>
{
    public NetComponent_GameChallengeStart()
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

    public override void OnEvent(EventArg_GameChallengeStart Arg)
    {
        NetLog.Log("NetComponent_GameChallengeStart : OnEvent");

        MsgReqGameChallengeStart pReq = new MsgReqGameChallengeStart();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_EpisodeId = Arg.m_nEpisodeID;
        pReq.I_ChapterId = Arg.m_nChapterID;
        pReq.I_StageId = Arg.m_nStageID;

        SendPacket<MsgReqGameChallengeStart, MsgAnsGameChallengeStart>(pReq, RecvPacket_GameChallengeStartSuccess, RecvPacket_GameChallengeStartFailure);
    }

    public void RecvPacket_GameChallengeStartSuccess(MsgReqGameChallengeStart pReq, MsgAnsGameChallengeStart pAns)
    {
        NetLog.Log("NetComponent_GameChallengeStart : RecvPacket_GameChallengeStartSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GameChallengeStartFailure(MsgReqGameChallengeStart pReq, Error error)
    {
        NetLog.Log("NetComponent_GameChallengeStart : RecvPacket_GameChallengeStartFailure");

        GetFailureEvent().OnEvent(error);
    }
}
