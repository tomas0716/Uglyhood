using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetLastPackagePurchaseBuySeq : TNetComponent_SuccessOrFail<EventArg_GetLastPackagePurchaseBuySeq, EventArg_Null, Error>
{
    private int m_nBuySeq = 0;

    public NetComponent_GetLastPackagePurchaseBuySeq()
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

    public override void OnEvent(EventArg_GetLastPackagePurchaseBuySeq Arg)
    {
        NetLog.Log("NetComponent_GetLastPackagePurchaseBuySeq : OnEvent");

        MsgReqGetLastPackagePurchaseBuySeq pReq = new MsgReqGetLastPackagePurchaseBuySeq();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.S_PaymentId = Arg.m_strPaymentID;

        SendPacket<MsgReqGetLastPackagePurchaseBuySeq, MsgAnsGetLastPackagePurchaseBuySeq>(pReq, RecvPacket_GetLastPackagePurchaseBuySeqSuccess, RecvPacket_GetLastPackagePurchaseBuySeqFailure);
    }

    public void RecvPacket_GetLastPackagePurchaseBuySeqSuccess(MsgReqGetLastPackagePurchaseBuySeq pReq, MsgAnsGetLastPackagePurchaseBuySeq pAns)
    {
        NetLog.Log("NetComponent_GetLastPackagePurchaseBuySeq : RecvPacket_GetLastPackagePurchaseBuySeqSuccess");

        ClearBuySeq();
        m_nBuySeq = pAns.I_BuySeq;

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetLastPackagePurchaseBuySeqFailure(MsgReqGetLastPackagePurchaseBuySeq pReq, Error error)
    {
        NetLog.Log("NetComponent_GetLastPackagePurchaseBuySeq : RecvPacket_GetLastPackagePurchaseBuySeqFailure");

        GetFailureEvent().OnEvent(error);
    }
}
