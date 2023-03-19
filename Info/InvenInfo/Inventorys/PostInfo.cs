using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using sg.protocol.basic;
using sg.protocol.common;
using sg.protocol.user;

public class PostInvenInfo
{
    public int m_nPostSeq;
    public string m_strSendName;
    public int m_nPostKind;
    public string m_strTitle;
    public int m_nContentID;
    public string m_strContents;
    public PostReward m_pPostReward;
    public bool m_IsNew;            // 0 안읽음 , 1 읽음
    public bool m_IsReward;         // 0 안받음, 1 받음
    public DateTime m_dtDeleteLimit;
}

public class PostInfo
{
    private List<PostInvenInfo> m_pPostInvenList = new List<PostInvenInfo>();

    private NetComponent_GetPostList m_pNetComponent_GetPostList = new NetComponent_GetPostList();

    public void Init()
    {
        EventDelegateManager.Instance.OnEventTimer_Complete += OnTimer_Complete;
    }

    public void OnDestroy()
    {
        EventDelegateManager.Instance.OnEventTimer_Complete -= OnTimer_Complete;
    }

    public void ClearPostInvenList()
    {
        m_pPostInvenList.Clear();
    }

    public void AddPostInvenList(PostInvenInfo pInfo)
    {
        m_pPostInvenList.Add(pInfo);
    }

    public void SetPostTimer()
    {
        TimerManager.Instance.AddTimer(eTimerType.ePost, 360, 0);
    }

    private void OnTimer_Complete(Timer pTimer, eTimerType eType, object parameter)
    {
        if(eType == eTimerType.ePost && Time.timeScale != 0)
        {
            TEventDelegate<EventArg_Null> pSuccessEvent = new TEventDelegate<EventArg_Null>();
            TEventDelegate<Error> pFailureEvent = new TEventDelegate<Error>();

            pSuccessEvent.SetFunc(OnGetPostListSuccess);
            pFailureEvent.SetFunc(OnGetPostListFailure);

            m_pNetComponent_GetPostList.SetSuccessEvent(pSuccessEvent);
            m_pNetComponent_GetPostList.SetFailureEvent(pFailureEvent);

            EventArg_Null pArg = new EventArg_Null();

            m_pNetComponent_GetPostList.OnEvent(pArg);

            AppInstance.Instance.m_pEventDelegateManager.OnCreateLoadingResponse();
        }
        else
        {
            TimerManager.Instance.AddTimer(eTimerType.ePost, 360, 0);
        }
    }

    private void OnGetPostListSuccess(EventArg_Null Arg)
    {
        EventDelegateManager.Instance.OnPost_RefreshPostList();

        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    private void OnGetPostListFailure(Error error)
    {
        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    public List<PostInvenInfo> GetPostInvenList()
    {
        return m_pPostInvenList;
    }

    public List<PostInvenInfo> GetPostInvenList_byPostTabType(ePostTabType ePostTabType)
    {
        List<PostInvenInfo> pInfo = new List<PostInvenInfo>();

        if(ePostTabType == ePostTabType.All)
        {
            return m_pPostInvenList;
        }
        else if(ePostTabType == ePostTabType.Unread)
        {
            for(int i=0; i<m_pPostInvenList.Count; i++)
            {
                if (m_pPostInvenList[i].m_IsNew)
                {
                    pInfo.Add(m_pPostInvenList[i]);
                }
            }
        }
        else
        {
            for(int i=0; i<m_pPostInvenList.Count; i++)
            {
                if (m_pPostInvenList[i].m_pPostReward.I_FreeGem > 0 || m_pPostInvenList[i].m_pPostReward.I_Gold > 0 ||
                    m_pPostInvenList[i].m_pPostReward.I_PaymentGem > 0 || m_pPostInvenList[i].m_pPostReward.L_Items.Count > 0)
                {
                    if (!m_pPostInvenList[i].m_IsReward)
                    {
                        pInfo.Add(m_pPostInvenList[i]);
                    }
                }

                //if (!m_pPostInvenList[i].m_IsReward)
                //{
                //    pInfo.Add(m_pPostInvenList[i]);
                //}
            }
        }

        return pInfo;
    }

    public bool IsRemainClaimedPost()
    {
        for(int i=0; i<m_pPostInvenList.Count; i++)
        {
            if (m_pPostInvenList[i].m_pPostReward.I_FreeGem > 0 || m_pPostInvenList[i].m_pPostReward.I_Gold > 0 ||
               m_pPostInvenList[i].m_pPostReward.I_PaymentGem > 0 || m_pPostInvenList[i].m_pPostReward.L_Items.Count > 0)
            {
                if (!m_pPostInvenList[i].m_IsReward)
                    return true;
            }
        }

        return false;
    }

    public bool IsRedDotOn()
    {
        for(int i=0; i<m_pPostInvenList.Count; i++)
        {
            //if (m_pPostInvenList[i].m_IsNew)
            //    return true;

            if(m_pPostInvenList[i].m_pPostReward.I_FreeGem > 0 || m_pPostInvenList[i].m_pPostReward.I_Gold > 0 ||
               m_pPostInvenList[i].m_pPostReward.I_PaymentGem > 0 || m_pPostInvenList[i].m_pPostReward.L_Items.Count > 0)
            {
                if (!m_pPostInvenList[i].m_IsReward)
                    return true;
            }
        }

        return false;
    }
}
