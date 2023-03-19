using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class SummonResult
{
    public eRewardType eRewardType = eRewardType.Item;
    public int nID = 0;
    public int nCount = 0;
    public int nBeforeRewardID = 0;
}

public class NetComponent_UnitSummon : TNetComponent_SuccessOrFail<EventArg_UnitSummon, EventArg_Null, Error>
{
    private List<SummonResult> m_pSummonResult = new List<SummonResult>();

    public NetComponent_UnitSummon()
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
        m_pSummonResult.Clear();
    }

    public List<SummonResult> GetSummonResults()
    {
        return m_pSummonResult;
    }

    public override void OnEvent(EventArg_UnitSummon Arg)
    {
        NetLog.Log("NetComponent_UnitSummon : OnEvent");

        MsgReqUnitSummon pReq = new MsgReqUnitSummon();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;
        pReq.I_ProductId = Arg.m_nProductID;
        pReq.I_PriceId = Arg.m_nPriceID;

        Debug.Log("Net SummonResult ProductID " + pReq.I_ProductId);
        Debug.Log("Net SummonResult PriceID " + pReq.I_PriceId);

        SendPacket<MsgReqUnitSummon, MsgAnsUnitSummon>(pReq, RecvPacket_UnitSummonSuccess, RecvPacket_UnitSummonFailure);
    }

    public void RecvPacket_UnitSummonSuccess(MsgReqUnitSummon pReq, MsgAnsUnitSummon pAns)
    {
        NetLog.Log("NetComponent_UnitSummon : RecvPacket_UnitSummonSuccess");

        ClearInfo();

        Debug.Log("Net SummonResultCount " + pAns.M_Summon.Count);

        for(int i=0; i<pAns.M_Summon.Count; i++)
        {
            SummonResult pSummonResult = new SummonResult();
            pSummonResult.eRewardType = (eRewardType)pAns.M_Summon[i].I_Type;
            pSummonResult.nID = pAns.M_Summon[i].I_Id;
            pSummonResult.nCount = pAns.M_Summon[i].I_Count;
            pSummonResult.nBeforeRewardID = pAns.M_Summon[i].I_BeforeRewardId;

            //if(pAns.M_Summon[i].I_BeforeRewardId != 0)
            //{
            //    Debug.Log(pAns.M_Summon[i].I_BeforeRewardId);
            //}

            //if(i == 0)
            //{
            //    pSummonResult = new SummonResult();
            //    pSummonResult.eRewardType = (eRewardType)0;
            //    pSummonResult.nID = 100013;
            //    pSummonResult.nCount = 1;
            //    pSummonResult.nBeforeRewardID = 0;
            //}

            m_pSummonResult.Add(pSummonResult);

            if (pSummonResult.eRewardType == eRewardType.Character)
            {
                ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(pSummonResult.nID);

                if (pUnitInfo != null)
                {
                    Parameter[] parameters = new Parameter[5];
                    parameters[0] = new Parameter("character_id", pUnitInfo.m_nID);
                    parameters[1] = new Parameter("character_rank", (int)pUnitInfo.m_eUnitRank);
                    parameters[2] = new Parameter("character_element", (int)pUnitInfo.m_eElement);
                    parameters[3] = new Parameter("character_class", (int)pUnitInfo.m_eClass);
                    parameters[4] = new Parameter("get_by", "summon");

                    Helper.FirebaseLogEvent("character_get", parameters);
                }
            }
        }

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_UnitSummonFailure(MsgReqUnitSummon pReq, Error pError)
    {
        NetLog.Log("NetComponent_UnitSummon : RecvPacket_UnitSummonFailure");

        GetFailureEvent().OnEvent(pError);
    }
}
