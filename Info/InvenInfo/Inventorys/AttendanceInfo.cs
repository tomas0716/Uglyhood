using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using sg.protocol.basic;
using sg.protocol.common;
using sg.protocol.user;

public class AttendanceInvenInfo
{
    public int m_nDay;
    public int m_nRewardType;
    public int m_nRewardID;
    public int m_nRewardCount;
}

public class AttendanceInfo : MonoBehaviour
{
    private int m_nAttendanceCount = 0;
    private bool m_IsTodayAttendance = false;
    private Dictionary<int, AttendanceInvenInfo> m_pAttendanceInfoTable = new Dictionary<int, AttendanceInvenInfo>();

    private NetComponent_GetAttendance m_pNetComponent_GetAttendance = new NetComponent_GetAttendance();

    private void Start()
    {
        EventDelegateManager.Instance.OnEventTimer_Complete += OnTimer_Complete;
    }

    private void OnDestroy()
    {
        EventDelegateManager.Instance.OnEventTimer_Complete -= OnTimer_Complete;
    }

    public void ClearAttendanceInfo()
    {
        m_nAttendanceCount = 0;
        m_IsTodayAttendance = false;
        m_pAttendanceInfoTable.Clear();
    }

    public void SetAttendanceBasicInfo(int nAttendanceCount, int nIsTodayAttendance)
    {
        m_nAttendanceCount = nAttendanceCount;

        if (nIsTodayAttendance == 0)
        {
            m_IsTodayAttendance = false;
        }
        else
        {
            m_IsTodayAttendance = true;
        }

        DateTime dtNow = new DateTime();
        dtNow = DateTime.Now;

        int nRemainTime = (int)DateTime.Today.AddDays(1).Subtract(DateTime.Now).TotalSeconds;

        //TimeInfo.Instance.SetTimer_ForAttendanceRefresh(nRemainTime);

        TimerManager.Instance.AddTimer(eTimerType.eAttendance, nRemainTime, 0);
    }

    private void OnTimer_Complete(Timer pTimer, eTimerType eType, object parameter)
    {
        if(eType == eTimerType.eAttendance)
        {
            TEventDelegate<EventArg_Null> pSuccessEvent = new TEventDelegate<EventArg_Null>();
            TEventDelegate<Error> pFailureEvent = new TEventDelegate<Error>();

            pSuccessEvent.SetFunc(OnGetAttendanceSuccess);
            pFailureEvent.SetFunc(OnGetAttendanceFailure);

            m_pNetComponent_GetAttendance.SetSuccessEvent(pSuccessEvent);
            m_pNetComponent_GetAttendance.SetFailureEvent(pFailureEvent);

            EventArg_Null pArg = new EventArg_Null();

            m_pNetComponent_GetAttendance.OnEvent(pArg);

            AppInstance.Instance.m_pEventDelegateManager.OnCreateLoadingResponse();
        }
    }

    private void OnGetAttendanceSuccess(EventArg_Null Arg)
    {
        EspressoInfo.Instance.m_IsShowAttendance = false;

        if(AppInstance.Instance.m_pSceneManager.GetCurrSceneType() == eSceneType.Scene_Lobby)
        {
            EventDelegateManager.Instance.OnLobby_ShowAttendancePopup();
        }

        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    private void OnGetAttendanceFailure(Error error)
    {
        AppInstance.Instance.m_pEventDelegateManager.OnDeleteLoadingResponse();
    }

    public void AddAttendanceInfoList(AttendanceInvenInfo pInfo)
    {
        m_pAttendanceInfoTable.Add(pInfo.m_nDay, pInfo);
    }

    public AttendanceInvenInfo GetAttendanceInfo_byDay(int nDay)
    {
        if (m_pAttendanceInfoTable.ContainsKey(nDay))
            return m_pAttendanceInfoTable[nDay];

        return null;
    }

    public bool IsTodayAttendanceDone()
    {
        return m_IsTodayAttendance;
    }

    public int GetAttendanceCount()
    {
        return m_nAttendanceCount;
    }
}
