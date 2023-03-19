using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnStatePattern
{
	private TurnState_StartAction			m_pTurnState_StartAction	= new TurnState_StartAction();
	private TEventDelegate<EventArg_Null>	m_pDone_StartAction			= new TEventDelegate<EventArg_Null>();

	private TurnState_StartBooster			m_pTurnState_StartBooster	= new TurnState_StartBooster();
	private TEventDelegate<EventArg_Null>	m_pDone_StartBooster		= new TEventDelegate<EventArg_Null>();

	private TurnState_Player				m_pTurnState_Player			= new TurnState_Player();
	private TEventDelegate<EventArg_Null>	m_pDone_Player				= new TEventDelegate<EventArg_Null>();

	private TurnState_Enemy					m_pTurnState_Enemy			= new TurnState_Enemy();
	private TEventDelegate<EventArg_Null>	m_pDone_Enemy				= new TEventDelegate<EventArg_Null>();

	private TurnState_Player				m_pTurnState_OtherPlayer	= new TurnState_Player(eOwner.Other);
	private TEventDelegate<EventArg_Null>	m_pDone_OtherPlayer			= new TEventDelegate<EventArg_Null>();

	public enum eTurnState
	{
		None,
		StartAction,
		StartBooster,
		Player,
		Enemy,
		OtherPlayer,
	}

	private eTurnState				m_eTurnState			= eTurnState.None;

	public TurnStatePattern()
	{
		m_pDone_StartAction.SetFunc(OnDone_StartAction_Turn);
		m_pTurnState_StartAction.SetNextEvent(m_pDone_StartAction);

		m_pDone_StartBooster.SetFunc(OnDone_StartBooster_Turn);
		m_pTurnState_StartBooster.SetNextEvent(m_pDone_StartBooster);

		m_pDone_Player.SetFunc(OnDone_Player_Turn);
		m_pTurnState_Player.SetNextEvent(m_pDone_Player);

		m_pDone_Enemy.SetFunc(OnDone_Enemy_Turn);
		m_pTurnState_Enemy.SetNextEvent(m_pDone_Enemy);

		m_pDone_OtherPlayer.SetFunc(OnDone_OtherPlayer_Turn);
		m_pTurnState_OtherPlayer.SetNextEvent(m_pDone_OtherPlayer);
	}

	public void OnDestroy()
	{
		m_pTurnState_StartAction.OnDestroy();
		m_pTurnState_StartBooster.OnDestroy();
		m_pTurnState_Player.OnDestroy();
		m_pTurnState_Enemy.OnDestroy();
		m_pTurnState_OtherPlayer.OnDestroy();
	}

	public void Update()
	{
		m_pTurnState_StartAction.Update();
		m_pTurnState_StartBooster.Update();
		m_pTurnState_Player.Update();
		m_pTurnState_Enemy.Update();
		m_pTurnState_OtherPlayer.Update();
	}

	public void LateUpdate()
	{
		m_pTurnState_StartAction.LateUpdate();
		m_pTurnState_StartBooster.LateUpdate();
		m_pTurnState_Player.LateUpdate();
		m_pTurnState_Enemy.LateUpdate();
		m_pTurnState_OtherPlayer.LateUpdate();
	}

	public eTurnState GetTurnState()
	{
		return m_eTurnState;
	}

	public void OnStartActionTurn()
	{
		if (m_eTurnState == eTurnState.None)
		{
			OutputLog.Log("TurnStatePattern : OnStartActionTurn");

			m_eTurnState = eTurnState.StartAction;
			m_pTurnState_StartAction.OnEvent(EventArg_Null.Object);
		}
	}

	public void OnDone_StartAction_Turn(EventArg_Null Arg)
	{
		m_eTurnState = eTurnState.None;

		EventDelegateManager.Instance.OnInGame_StartActionTurnDone();
	}


	public void OnStartBoosterTurn()
	{
		if (m_eTurnState == eTurnState.None)
		{
			OutputLog.Log("TurnStatePattern : OnStartBoosterTurn");

			m_eTurnState = eTurnState.StartBooster;
			m_pTurnState_StartBooster.OnEvent(EventArg_Null.Object);
		}
	}

	public void OnDone_StartBooster_Turn(EventArg_Null Arg)
	{
		m_eTurnState = eTurnState.None;

		EventDelegateManager.Instance.OnInGame_StartBoosterTurnDone();
	}

	public void OnPlayerTurn()
	{
		if (m_eTurnState == eTurnState.None && InGameInfo.Instance.m_eCurrGameResult == eGameResult.None)
		{
			OutputLog.Log("TurnStatePattern : OnPlayerTurn");

			EspressoInfo.Instance.m_nDamageTargetRandomSeed = UnityEngine.Random.Range(0, int.MaxValue);

			if (!EspressoInfo.Instance.m_IsSuddenDeathOn_PvP && DataStackManager.Instance.Find<MainGame_DataStack>().m_nPlayerTurnCount >= (int)eTempSuddenDeathCount.Count - 1)
			{
				EspressoInfo.Instance.m_IsSuddenDeathOn_PvP = true;
			}

			Debug.Log(DataStackManager.Instance.Find<MainGame_DataStack>().m_nPlayerTurnCount);
			EventDelegateManager.Instance.OnInGame_AddTurnCount_PvP();

			m_eTurnState = eTurnState.Player;
			m_pTurnState_Player.OnEvent(EventArg_Null.Object);
		}
	}

	public void OnDone_Player_Turn(EventArg_Null Arg)
	{
		m_eTurnState = eTurnState.None;

        if (EspressoInfo.Instance.m_IsChallengeMode)
        {
			EventDelegateManager.Instance.OnInGameUI_ChallengeModeCheck();
		}

		EventDelegateManager.Instance.OnInGame_PlayerTurnDone();
	}

	public void OnEnemyTurn()
	{
		if (m_eTurnState == eTurnState.None && InGameInfo.Instance.m_eCurrGameResult == eGameResult.None)
		{
			OutputLog.Log("TurnStatePattern : OnEnemyTurn");

			m_eTurnState = eTurnState.Enemy;
			m_pTurnState_Enemy.OnEvent(EventArg_Null.Object);
		}
	}

	public void OnDone_Enemy_Turn(EventArg_Null Arg)
	{
		m_eTurnState = eTurnState.None;

		EventDelegateManager.Instance.OnInGame_EnemyTurnDone();
	}

	public void OnOtherPlayerTurn()
	{
		if (m_eTurnState == eTurnState.None && InGameInfo.Instance.m_eCurrGameResult == eGameResult.None)
		{
			OutputLog.Log("TurnStatePattern : OnOtherPlayerTurn");

			m_eTurnState = eTurnState.OtherPlayer;
			m_pTurnState_OtherPlayer.OnEvent(EventArg_Null.Object);
		}
	}

	public void OnDone_OtherPlayer_Turn(EventArg_Null Arg)
	{
		m_eTurnState = eTurnState.None;

		EventDelegateManager.Instance.OnInGame_OtherPlayerTurnDone();
	}
}
