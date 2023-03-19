using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
	private TimerManager			m_pTimerManager				= null;
	private eTimerType				m_eTimerType				= eTimerType.eNone;
	private bool					m_IsCompleteToDestroy		= false;
	private bool					m_IsArriveTime				= false;
	private Transformer_Scalar		m_pTimer_ArriveRenameTime	= new Transformer_Scalar(0);

	private float					m_fArriveRenameTime			= 0;
	private float					m_dwLastRealTime			= 0;

	private object					m_Parameter					= null;

	private float					m_fStartTime				= 0;

	public Timer(TimerManager pTimerManager, eTimerType eType, float fTime, float fInterval, object parameter, bool IsCompleteToDestroy)
	{
		m_pTimerManager = pTimerManager;
		m_eTimerType = eType;
		m_IsCompleteToDestroy = IsCompleteToDestroy;
		m_Parameter = parameter;

		m_dwLastRealTime = Time.realtimeSinceStartup;

		TransformerEvent_Scalar eventValue = null;
		eventValue = new TransformerEvent_Scalar(0, (float)fTime);
		m_pTimer_ArriveRenameTime.AddEvent(eventValue);
		eventValue = new TransformerEvent_Scalar((float)fTime + fInterval, 0);
		m_pTimer_ArriveRenameTime.AddEvent(eventValue);

		m_pTimer_ArriveRenameTime.SetCallback(null, OnDone_Timer_ArriveRenameTime);
		m_pTimer_ArriveRenameTime.OnPlay();
	}

	public void Update()
	{
		float fDeltaTime = Time.realtimeSinceStartup - m_dwLastRealTime;
		m_dwLastRealTime = Time.realtimeSinceStartup;

		m_pTimer_ArriveRenameTime.Update(fDeltaTime);
		m_fArriveRenameTime = m_pTimer_ArriveRenameTime.GetCurScalar();
	}

	public bool IsArriveTime()
	{
		return m_IsArriveTime;
	}

	public float GetArriveRenameTime()
	{
		return m_fArriveRenameTime;
	}

	public float GetElapsedTime()
	{
		return m_pTimer_ArriveRenameTime.GetCurTime();
	}

	public eTimerType GetTimerType()
	{
		return m_eTimerType;
	}

	public object GetParameter()
    {
		return m_Parameter;
    }

	public void SetStartTime(float fStartTime)
    {
		m_fStartTime = fStartTime;
    }

	public float GetElapsedTime_byStartTime()
    {
		return m_pTimer_ArriveRenameTime.GetCurTime() + m_fStartTime;

	}

	private void OnDone_Timer_ArriveRenameTime(TransformerEvent eventValue)
	{
		m_IsArriveTime = true;

		AppInstance.Instance.StartCoroutine(WaitForArrive());
	}

	IEnumerator WaitForArrive()
	{
		yield return new WaitForEndOfFrame();

		if (m_IsCompleteToDestroy == true)
		{
			m_pTimerManager.OnDone_Timer(this);
		}

		AppInstance.Instance.m_pEventDelegateManager.OnTimer_Complete(this, m_eTimerType, m_Parameter);
	}
}
