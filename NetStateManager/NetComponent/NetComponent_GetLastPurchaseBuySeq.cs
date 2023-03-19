using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;
using sg.protocol.common;

public class NetComponent_GetLastPurchaseBuySeq : TNetComponent_SuccessOrFail<EventArg_GetLastPurchaseBuySeq, EventArg_Null, Error>
{
    private int m_nBuySeq = 0;

    public NetComponent_GetLastPurchaseBuySeq()
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

    private void ClearBuySeq()
    {
        m_nBuySeq = 0;
    }

    public int GetBuySeq()
    {
        return m_nBuySeq;
    }

    public override void OnEvent(EventArg_GetLastPurchaseBuySeq Arg)
    {
        NetLog.Log("NetComponent_GetLastPurchaseBuySeq : OnEvent");

        switch (Arg.m_eIAPContentType)
        {
            case eIAPContentType.Shop:
                {
                    MsgReqGetLastPurchaseBuySeq pReq = new MsgReqGetLastPurchaseBuySeq();
                    pReq.Uid = MyInfo.Instance.m_nUserIndex;
                    pReq.S_PaymentId = Arg.m_strPaymentID;

                    SendPacket<MsgReqGetLastPurchaseBuySeq, MsgAnsGetLastPurchaseBuySeq>(pReq, RecvPacket_GetLastPurchaseBuySeqSuccess, RecvPacket_GetLastPurchaseBuySeqFailure);
                }
                break;

            case eIAPContentType.Package:
                {
                }
                break;

            case eIAPContentType.BattlePassTicket:
                {
                    MsgReqGetLastPurchaseBattlePassBuySeq pReq = new MsgReqGetLastPurchaseBattlePassBuySeq();
                    pReq.Uid = MyInfo.Instance.m_nUserIndex;
                    pReq.S_PaymentId = Arg.m_strPaymentID;

                    SendPacket<MsgReqGetLastPurchaseBattlePassBuySeq, MsgAnsGetLastPurchaseBattlePassBuySeq>(pReq, RecvPacket_GetLastPurchaseBattlePassBuySeqSuccess, RecvPacket_GetLastPurchaseBattlePassBuySeqFailure);
                }
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Shop

    public void RecvPacket_GetLastPurchaseBuySeqSuccess(MsgReqGetLastPurchaseBuySeq pReq, MsgAnsGetLastPurchaseBuySeq pAns)
    {
        NetLog.Log("NetComponent_GetLastPurchaseBuySeq : GetLastPurchaseBuySeqSuccess");

        ClearBuySeq();

        m_nBuySeq = pAns.I_BuySeq;

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetLastPurchaseBuySeqFailure(MsgReqGetLastPurchaseBuySeq pReq, Error pError)
    {
        NetLog.Log("NetComponent_GetLastPurchaseBuySeq : GetLastPurchaseBuySeqFailure");

        GetFailureEvent().OnEvent(pError);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// BattlePass

    public void RecvPacket_GetLastPurchaseBattlePassBuySeqSuccess(MsgReqGetLastPurchaseBattlePassBuySeq pReq, MsgAnsGetLastPurchaseBattlePassBuySeq pAns)
    {
        NetLog.Log("NetComponent_GetLastPurchaseBuySeq : GetLastPurchaseBuySeqSuccess");

        ClearBuySeq();

        m_nBuySeq = pAns.I_BuySeq;

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetLastPurchaseBattlePassBuySeqFailure(MsgReqGetLastPurchaseBattlePassBuySeq pReq, Error pError)
    {
        NetLog.Log("NetComponent_GetLastPurchaseBuySeq : GetLastPurchaseBuySeqFailure");

        GetFailureEvent().OnEvent(pError);
    }

}
