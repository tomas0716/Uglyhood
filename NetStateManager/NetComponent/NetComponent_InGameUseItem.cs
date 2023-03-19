using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_InGameUseItem : TNetComponent_SuccessOrFail<EventArg_InGameUseItem, EventArg_Null, Error>
{
    public NetComponent_InGameUseItem()
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

    public override void OnEvent(EventArg_InGameUseItem Arg)
    {
        NetLog.Log("NetComponent_InGameUseItem : OnEvent");

        MsgReqInGameUseItem pReq = new MsgReqInGameUseItem();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_EpisodeId = Arg.m_nEpisodeID;
        pReq.I_ChapterId = Arg.m_nChapterID;
        pReq.I_StageId = Arg.m_nStageID;
        pReq.I_Id = Arg.m_nItemID;
        pReq.I_Count = Arg.m_nItemCount;

        SendPacket<MsgReqInGameUseItem, MsgAnsInGameUseItem>(pReq, RecvPacket_InGameUseItemSuccess, RecvPacket_InGameUseItemFailure);

        Parameter[] parameters = new Parameter[4];

        parameters[0] = new Parameter("item_id", Arg.m_nItemID);
        parameters[1] = new Parameter("count", Arg.m_nItemCount);
        parameters[2] = new Parameter("spend_to", "main_stage");
        parameters[3] = new Parameter("detail_id", EspressoInfo.Instance.m_nStageID);

        Helper.FirebaseLogEvent("booster_item_spend", parameters);
    }

    public void RecvPacket_InGameUseItemSuccess(MsgReqInGameUseItem pReq, MsgAnsInGameUseItem pAns)
    {
        NetLog.Log("NetComponent_InGameUseItem : RecvPacket_InGameUseItemSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }
    
    public void RecvPacket_InGameUseItemFailure(MsgReqInGameUseItem pReq, Error pError)
    {
        NetLog.Log("NetComponent_InGameUseItem : RecvPacket_InGameUseItemFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
