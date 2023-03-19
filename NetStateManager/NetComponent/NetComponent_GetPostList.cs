using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

using sg.protocol.common;
using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.user;

public class NetComponent_GetPostList : TNetComponent_SuccessOrFail<EventArg_Null, EventArg_Null, Error>
{
    public NetComponent_GetPostList()
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
        NetLog.Log("NetComponent_GetPostList : OnEvent");

        Debug.Log(DateTime.Now);

        MsgReqPostList pReq = new MsgReqPostList();
        pReq.Uid = MyInfo.Instance.m_nUserIndex;

        SendPacket<MsgReqPostList, MsgAnsPostList>(pReq, RecvPacket_GetPostListSuccess, RecvPacket_GetPostListFailure);
    }

    public void RecvPacket_GetPostListSuccess(MsgReqPostList pReq, MsgAnsPostList pAns)
    {
        NetLog.Log("NetComponent_GetPostList : RecvPacket_GetPostListSuccess");

        List<int> pSeqList = new List<int>();
        Dictionary<int, PostInvenInfo> pPostTable_bySeq = new Dictionary<int, PostInvenInfo>();

        InventoryInfoManager.Instance.m_pPostInfo.ClearPostInvenList();

        foreach(Post pInfo in pAns.L_PostList)
        {
            bool IsItemIdNull = false;

            PostInvenInfo pPostInvenInfo = new PostInvenInfo();

            pPostInvenInfo.m_nPostSeq = pInfo.I_PostSeq;
            pPostInvenInfo.m_strSendName = pInfo.S_SendName;
            pPostInvenInfo.m_nPostKind = pInfo.I_PostKind;
            pPostInvenInfo.m_strTitle = pInfo.S_Title;
            pPostInvenInfo.m_nContentID = pInfo.I_ContentsId;
            pPostInvenInfo.m_strContents = pInfo.S_Contents;
            pPostInvenInfo.m_pPostReward = pInfo.St_PostReward;
            pPostInvenInfo.m_IsNew = pInfo.B_IsNew;

            if(pInfo.I_IsReward == 0)
            {
                pPostInvenInfo.m_IsReward = false;
            }
            else
            {
                pPostInvenInfo.m_IsReward = true;
            }

            DateTime dtDelLimit = new DateTime();
            dtDelLimit = DateTime.Now.AddSeconds(pInfo.I_DelLimit);
            pPostInvenInfo.m_dtDeleteLimit = dtDelLimit;

            for(int i=0; i<pPostInvenInfo.m_pPostReward.L_Items.Count; i++)
            {
                ExcelData_ItemInfo pItemInfo = ExcelDataManager.Instance.m_pItem.GetItemInfo_byID(pPostInvenInfo.m_pPostReward.L_Items[i].I_ItemId);
                if(pItemInfo == null)
                {
                    IsItemIdNull = true;
                }
            }

            if(IsItemIdNull == false)
            {
                pSeqList.Add(pPostInvenInfo.m_nPostSeq);
                pPostTable_bySeq.Add(pPostInvenInfo.m_nPostSeq, pPostInvenInfo);

                //InventoryInfoManager.Instance.m_pPostInfo.AddPostInvenList(pPostInvenInfo);
            }
        }

        //List<int> pSeqList_bySort = new List<int>();
        //pSeqList_bySort = from n in pSeqList orderby n descending select n;

        pSeqList.Sort();
        pSeqList.Reverse();

        for(int i=0; i<pSeqList.Count; i++)
        {
            InventoryInfoManager.Instance.m_pPostInfo.AddPostInvenList(pPostTable_bySeq[pSeqList[i]]);
        }

        InventoryInfoManager.Instance.m_pPostInfo.SetPostTimer();

        GetSuccessEvent().OnEvent(EventArg_Null.Object);
    }

    public void RecvPacket_GetPostListFailure(MsgReqPostList pReq, Error error)
    {
        NetLog.Log("NetComponent_GetPostList : RecvPacket_GetPostListFailure");

        GetFailureEvent().OnEvent(error);
    }
}
