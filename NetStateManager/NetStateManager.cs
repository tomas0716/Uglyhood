using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eNetState
{
	None = -1,
	LogoToLobby,

	Max,
}

public enum eNetScenarioState
{
	Wait,
	Process,
	Pause,
	Done,
}

public class NetStateManager : Singleton<NetStateManager>
{
	private NetScenario<EventArg_Null, eNetState> []	m_pNetScenario		= new NetScenario<EventArg_Null, eNetState>[(int)eNetState.Max];
	private TEventDelegate<eNetState>					m_pScenarioDone		= new TEventDelegate<eNetState>();

	private eNetState				m_eCurrNetState			= eNetState.None;
	private eNetScenarioState []	m_eNetScenarioState		= new eNetScenarioState[(int)eNetState.Max];
	private bool					m_IsLock				= false;

	public NetStateManager()
	{
		for (int i = 0; i < (int)eNetState.Max; ++i)
		{
			m_eNetScenarioState[i] = eNetScenarioState.Wait;
		}

		m_pScenarioDone.SetFunc(OnScenarioDone);

		m_pNetScenario[(int)eNetState.LogoToLobby] = new NetScenario_LogoToLobby(eNetState.LogoToLobby);
		m_pNetScenario[(int)eNetState.LogoToLobby].SetNextEvent(m_pScenarioDone);
	}

	public void OnDestroy()
	{
		for (int i = 0; i < (int)eNetState.Max; ++i)
		{
			m_pNetScenario[i].OnDestroy();
		}
	}

	public void Update()
	{
		for (int i = 0; i < (int)eNetState.Max; ++i)
		{
			m_pNetScenario[i].Update();
		}
	}

	public void LateUpdate()
	{
		for (int i = 0; i < (int)eNetState.Max; ++i)
		{
			m_pNetScenario[i].LateUpdate();
		}
	}

	public void ResetState()
	{
		m_IsLock = false;
		m_eCurrNetState = eNetState.None;

		for (int i = 0; i < (int)eNetState.Max; ++i)
		{
			m_eNetScenarioState[i] = eNetScenarioState.Wait;
		}
	}

	public void OnActiveScenario(eNetState eState)
	{
		NetLog.Log("NetStateManager OnActiveScenario Start : " + eState.ToString());

		if (m_IsLock == false && m_eCurrNetState == eNetState.None && m_eNetScenarioState[(int)eState] == eNetScenarioState.Wait)
		{
			m_IsLock = true;
			m_eCurrNetState = eState;
			m_eNetScenarioState[(int)eState] = eNetScenarioState.Process;

			m_pNetScenario[(int)eState].OnEvent(EventArg_Null.Object);
		}

		NetLog.Log("NetStateManager OnActiveScenario End : " + eState.ToString());
	}

	public void OnScenarioDone(eNetState eState)
	{
		m_IsLock = false;
		m_eCurrNetState = eNetState.None;
		m_eNetScenarioState[(int)eState] = eNetScenarioState.Wait;
	}
}
