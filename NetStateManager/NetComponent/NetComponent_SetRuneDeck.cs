using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_SetRuneDeck : TNetComponent_SuccessOrFail<EventArg_SetRuneDeck, EventArg_Null, Error>
{
    private EventArg_SetRuneDeck m_pEventArg_SetRuneDeck;

    public NetComponent_SetRuneDeck()
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

    public override void OnEvent(EventArg_SetRuneDeck Arg)
    {
        NetLog.Log("NetComponent_SetRuneDeck : OnEvent");

        m_pEventArg_SetRuneDeck = Arg;

        MsgReqSetRuneDeck pReq = new MsgReqSetRuneDeck();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqSetRuneDeck, MsgAnsSetRuneDeck>(pReq, RecvPacket_SetRuneDeckSuccess, RecvPacket_SetRuneDeckFailure);
    }

    public void RecvPacket_SetRuneDeckSuccess(MsgReqSetRuneDeck pReq, MsgAnsSetRuneDeck pAns)
    {
        NetLog.Log("NetComponent_SetRuneDeck : RecvPacket_SetRuneDeckSuccess");

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_SetRuneDeckFailure(MsgReqSetRuneDeck pReq, Error pError)
    {
        NetLog.Log("NetComponent_SetRuneDeck : RecvPacket_SetRuneDeckFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
