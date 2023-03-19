using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.basic;

public class BattlePassInfo : Singleton<BattlePassInfo>
{
	public int	m_nSeasonID			= 0;			// 0 이면 시즌이 없다.
	public int	m_nLevel			= 0;
	public int	m_nPoint			= 0;
	public bool m_IsPassTicketBuy	= false;
	public string m_strBattlePassProductID = "";

	public int m_nRecvRewardLevel_Free = 0;
	public int m_nRecvRewardLevel_Paid = 0;

	public enum eServerConnect
	{
		None,
		Start,
		End,
	}

	private eServerConnect		m_eServerConnect	= eServerConnect.None;
	private Transformer_Timer	m_pTimer			= new Transformer_Timer();

	private int								m_nPrevReqSeasonID					= 0;
	private NetComponent_GetUserBattlePass	m_pNetComponent_GetUserBattlePass	= new NetComponent_GetUserBattlePass();

	public bool IsPossibleGetReward()
	{
		ExcelData_BattlePass_RewardLevelInfo pBattlePass_RewardLevelInfo = ExcelDataManager.Instance.m_pBattlePass_RewardLevel.GetRewardLevelInfo(m_nSeasonID, m_nLevel);

		if (pBattlePass_RewardLevelInfo != null && ExcelDataManager.Instance.m_pBattlePass_RewardLevel.IsRewardLevelLast(m_nSeasonID, pBattlePass_RewardLevelInfo) == true)
		{
			if (BattlePassInfo.Instance.m_nPoint >= pBattlePass_RewardLevelInfo.m_nNextLevel_NeedPoint)
			{
				int nRecvCount = BattlePassInfo.Instance.m_nPoint / pBattlePass_RewardLevelInfo.m_nNextLevel_NeedPoint;

				if (nRecvCount > 0)
				{
					return true;
				}
			}

			if (m_IsPassTicketBuy == true && m_nLevel - 1 > m_nRecvRewardLevel_Paid)
				return true;
		}
		else
		{
			if (m_nLevel > m_nRecvRewardLevel_Free)
				return true;

			if (m_IsPassTicketBuy == true && m_nLevel > m_nRecvRewardLevel_Paid)
				return true;
		}

		return false;
	}

	public BattlePassInfo()
	{
		TEventDelegate<EventArg_Null> pSuccessEvent = new TEventDelegate<EventArg_Null>();
		TEventDelegate<Error> pFailureEvent = new TEventDelegate<Error>();
		pSuccessEvent.SetFunc(OnGetUserBattlePassSuccess);
		pFailureEvent.SetFunc(OnGetUserBattlePassFailure);

		m_pNetComponent_GetUserBattlePass.SetSuccessEvent(pSuccessEvent);
		m_pNetComponent_GetUserBattlePass.SetFailureEvent(pFailureEvent);

		TransformerEvent_Timer eventValue;
		eventValue = new TransformerEvent_Timer(60);
		m_pTimer.AddEvent(eventValue);
		m_pTimer.SetCallback(null, OnDone_Timer);
	}

	public void Update()
	{
		m_pTimer.Update(Time.deltaTime);
	}

	public void OnTimerAction(DateTime pTargetDateTime)
	{
		Timer pTimer = TimerManager.Instance.GetTimer(eTimerType.eLoginAfterTime);
		float fElapsedTime = pTimer.GetElapsedTime();
		TimeSpan pTimeSpan = TimeSpan.FromSeconds((double)fElapsedTime);

		DateTime pCurrDateTime = EspressoInfo.Instance.m_pConnectDateTime + pTimeSpan;
		TimeSpan pRemoveTimeSpan = pTargetDateTime - pCurrDateTime;

		EventDelegateManager.Instance.OnEventTimer_Complete += OnTimer_Complete;
		TimerManager.Instance.AddTimer(eTimerType.eBattlePass, (float)pRemoveTimeSpan.TotalSeconds, 3, true);
	}

	public void OnTimerAction_StartCheck(DateTime pTargetDateTime)
	{
		Timer pTimer = TimerManager.Instance.GetTimer(eTimerType.eLoginAfterTime);
		float fElapsedTime = pTimer.GetElapsedTime();
		TimeSpan pTimeSpan = TimeSpan.FromSeconds((double)fElapsedTime);

		DateTime pCurrDateTime = EspressoInfo.Instance.m_pConnectDateTime + pTimeSpan;
		TimeSpan pRemoveTimeSpan = pTargetDateTime - pCurrDateTime;

		EventDelegateManager.Instance.OnEventTimer_Complete += OnTimer_Complete;
		TimerManager.Instance.AddTimer(eTimerType.eBattlePassStartCheck, (float)pRemoveTimeSpan.TotalSeconds, 3, true);
	}

	public void OnTimer_Complete(Timer pTimer, eTimerType eType, object parameter)
	{
		OutputLog.Log("BattlePassInfo : OnTimer_Complete Begin");

		switch (eType)
		{
			case eTimerType.eBattlePass:
				{
					OutputLog.Log("BattlePassInfo : OnTimer_Complete Season End");

					EventDelegateManager.Instance.OnEventTimer_Complete -= OnTimer_Complete;

					// 배틀 패스 시즌 종료!!
					m_eServerConnect = eServerConnect.End;
					m_nPrevReqSeasonID = m_nSeasonID;
					m_pNetComponent_GetUserBattlePass.OnEvent(EventArg_Null.Object);
				}
				break;

			case eTimerType.eBattlePassStartCheck:
				{
					OutputLog.Log("BattlePassInfo : OnTimer_Complete Season Start");

					EventDelegateManager.Instance.OnEventTimer_Complete -= OnTimer_Complete;

					// 배틀 패스 시즌 시작!!
					m_eServerConnect = eServerConnect.Start;
					m_pNetComponent_GetUserBattlePass.OnEvent(EventArg_Null.Object);
				}
				break;
		}

		OutputLog.Log("BattlePassInfo : OnTimer_Complete End");
	}

	private void OnGetUserBattlePassSuccess(EventArg_Null Arg)
	{
		OutputLog.Log("BattlePassInfo : OnGetUserBattlePassSuccess Begin");

		switch (m_eServerConnect)
		{
			case eServerConnect.Start:
				{
					OutputLog.Log("BattlePassInfo : OnGetUserBattlePassSuccess Connect Start");

					if (BattlePassInfo.Instance.m_nSeasonID == 0)
					{
						OutputLog.Log("BattlePassInfo : OnGetUserBattlePassSuccess Connect Start - Timer Start");
						m_pTimer.OnPlay();
					}
					else
					{
						OutputLog.Log("BattlePassInfo : OnGetUserBattlePassSuccess Connect Start - Change State");

						// 배틀 패스 시작 - eventDelegate 날려야함
						EventDelegateManager.Instance.OnChangeBattlePassState();
					}
				}
				break;

			case eServerConnect.End:
				{
					OutputLog.Log("BattlePassInfo : OnGetUserBattlePassSuccess Connect End");

					if (BattlePassInfo.Instance.m_nSeasonID == 0 || BattlePassInfo.Instance.m_nSeasonID != m_nPrevReqSeasonID)
					{
						OutputLog.Log("BattlePassInfo : OnGetUserBattlePassSuccess Connect End - Change State");

						// 배틀 패스 종료 - eventDelegate 날려야함
						EventDelegateManager.Instance.OnChangeBattlePassState();
					}
					else
					{
						OutputLog.Log("BattlePassInfo : OnGetUserBattlePassSuccess Connect End - Timer Start");
						m_pTimer.OnPlay();
					}
				}
				break;
		}

		OutputLog.Log("BattlePassInfo : OnGetUserBattlePassSuccess End");
	}

	private void OnGetUserBattlePassFailure(Error error)
	{
		m_pTimer.OnPlay();
	}

	private void OnDone_Timer(TransformerEvent eventValue)
	{
		m_pNetComponent_GetUserBattlePass.OnEvent(EventArg_Null.Object);
	}
}
