using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EspressoInfo : Singleton<EspressoInfo>
{
	public int m_nEpisodeID = 1;
	public int m_nChapterID = 101;
	public int m_nStageID = 10101;

	public bool m_IsChallengeMode = false;

	public bool m_IsSkillAttack_ForMoveAndCreate = false;
	public bool m_IsPlayerCharacterSkillTrigger = false;
	public bool m_IsBoosterItemlTrigger = false;
	public bool m_IsInGame_SkillTriggerDrag = false;

	public bool m_IsResultTextShowDone = false;

	public bool m_IsInGameToLobby = false;
	public bool m_IsInGameToLobby_forDailyStage = false;
	public bool m_IsInGameToLobby_forPvP = false;

	public bool m_IsChapterUnlock = false;
	public int m_nUnlockChapterIndex = 0;

	public eActionTriggerSubject m_eActionTriggerSubject = eActionTriggerSubject.None;
	public int m_nUseBoosterItemSlot = -1;

	public bool m_IsActionAndUnitDetailCloseBlock = false;

	public eGameMode m_eGameMode = eGameMode.MainStage;
	public int m_nGameMode = 0;

	public ePassKind m_ePassKind_forDailyStage = ePassKind.Free;

	public DateTime m_pConnectDateTime = DateTime.Now;

	public int m_nUseBoosterItem = 0;
	public int m_nUseAdBoosterItem = 0;

	public int m_nCurrStageRevExp	= 0;

	public ePageType m_eCurrentPage = ePageType.Story;

	public eOwner m_ePVP_CurrTurn_Owner = eOwner.My;

	public bool m_IsSuddenDeathOn_PvP = false;

	public int m_nDamageTargetRandomSeed = 0;

	public bool m_IsShowAttendance = false;

	public eNetwork_RouterServer m_eNetwork_RouterServer = eNetwork_RouterServer.Dev;
	public bool m_IsOneStore = false;

	public bool m_IsUseIAP = false;

	public bool m_IsGachaResultPage = false;

	public bool m_IsPVE_EnemyTurnNoticePass	= false;

	public bool m_IsBattlePass_SkipLevelAction = false;

	public bool m_IsPurchase_byIAP = false;

	public int m_nEventPVPGameSeq = 0;

	private EspressoInfo()
	{
	}

	public void Reset()
	{
		m_IsSkillAttack_ForMoveAndCreate = false;
		m_IsPlayerCharacterSkillTrigger = false;
		m_IsBoosterItemlTrigger = false;
		m_IsInGame_SkillTriggerDrag = false;
		m_IsResultTextShowDone = false;
		m_IsInGameToLobby = false;
		m_IsChapterUnlock = false;
		m_nUnlockChapterIndex = 0;
		m_eActionTriggerSubject = eActionTriggerSubject.None;
		m_IsActionAndUnitDetailCloseBlock = false;
		m_nUseBoosterItem = 0;
		m_nUseAdBoosterItem = 0;
		m_nCurrStageRevExp = 0;
		m_ePVP_CurrTurn_Owner = eOwner.My;
		m_IsSuddenDeathOn_PvP = false;
		m_nDamageTargetRandomSeed = 0;
		m_IsBattlePass_SkipLevelAction = false;
	}
}