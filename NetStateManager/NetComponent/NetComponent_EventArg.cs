using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;

public struct EventArg_SetTeamDeck
{
	public int			m_nTeamDeckID;
	public int[]		m_nUnitSeq;                    // 0번 ~ GameDefine.ms_ms_nTeamCharacterCount - 1 까지 인덱스
	public int			m_nRuneDeckID;
	public int			m_nBoosterItemDeckID;
}

public struct EventArg_SetRuneDeck
{
	public int			m_nRuneDeckID;
	public int[]		m_nRuneIDs;                   
	public string		m_strName;
}

public struct EventArg_SetBoosterDeck
{
	public int			m_nBoosterDeckID;
	public int[]		m_nBoosterIDs;
	public string		m_strName;
}

public struct EventArg_SetModeTeamDeck
{
	public eGameMode	m_eGameMode;
	public int			m_nDeckID;
}

public struct EventArg_UserUnitLevelUp
{
	public int			m_nUnitID;
}

public struct EventArg_UserUnitActive
{
	public int			m_nUnitID;
}


public struct EventArg_UserUnitActionLevelUp
{
	public int			m_nUnitID;
	public int			m_nActionID;
}

public struct EventArg_UnitSummon
{
	public int			m_nProductID;
	public int			m_nPriceID;
}

public struct EventArg_GetUserChapterInfo
{
	public int			m_nEpisodeID;
	public int			m_nChapterID;
}

public struct EventArg_InGameUseItem
{
	public int			m_nEpisodeID;
	public int			m_nChapterID;
	public int			m_nStageID;
	public int			m_nItemID;
	public int			m_nItemCount;
}

public struct EventArg_InGameStart
{
	public int			m_nEpisodeID;
	public int			m_nChapterID;
	public int			m_nStageID;
	public int			m_nIsSweep;
}

public struct EventArg_InGameFinish
{
	public int			m_nEpisodeID;
	public int			m_nChapterID;
	public int			m_nStageID;
	public int			m_nIsClear;
	public int			m_nStarCount;
	public int			m_nDestroyColonyCount;
	public int			m_nMaxCombo;
}

public struct EventArg_CreateUserName
{
	public string		m_strUserName;
}

public struct EventArg_DailyStageStart
{
	public int			m_nModeID;
	public int			m_nDifficulty;
	public int			m_nStageID;
	public int			m_nPassKind;
	public int			m_nIsSweep;
}

public struct EventArg_DailyStageEnd
{
	public int			m_nModeID;
	public int			m_nDifficulty;
	public int			m_nStageID;
	public int			m_nPassKind;
	public int			m_nIsClear;
	public int			m_nStarCount;
	public int			m_nDestroyColonyCount;
}

public struct EventArg_EventStageStart
{
	public int m_nModeID;
	public int m_nDifficulty;
	public int m_nStageID;
	public int m_nIsSweep;
}

public struct EventArg_EventStageFinish
{
	public int m_nModeID;
	public int m_nDifficulty;
	public int m_nStageID;
	public int m_nIsClear;
	public int m_nStarCount;
	public int m_nDestroyColonyCount;
}

public struct EventArg_BuyShopBasicProduct
{
	public int			m_nID;
}

public struct EventArg_BuyShopDailyProduct
{
	public int			m_nID;
}

public struct EventArg_PostConfirm
{
	public List<int> m_nPostSeqList;
}

public struct EventArg_PostDelete
{
	public List<int> m_nPostSeqList;
}

public struct EventArg_GetPostReward
{
	public List<int> m_nPostSeqList;
}

public struct EventArg_PvPGameStart
{
	public int m_nDeckID;
	public int m_nStageID;
}

public struct EventArg_PvPGameFinish
{
	public int m_nGameSeq;
	public int m_nResult;
}

public struct EventArg_UseEnergyChargeItem
{
	public int m_nItemID;
}

public struct EventArg_SetUserAvatar
{
	public int m_nAvatarUnitID;
}

public struct EventArg_UserUnitPassiveSkillUp
{
	public int m_nUnitID;
}

public struct EventArg_UserGetFastOfflineReward
{
	public int m_nKind;
}

public struct EventArg_Send_UserAccountGet
{
	public string m_strDeviceID;
	public eNetwork_LoginBind m_eLoginBind;
}

public struct EventArg_Recv_UserAccountGet
{
	public EventArg_Send_UserAccountGet m_pUserAccountGet;
	public MsgAnsGetUserConnect m_pProtocol_GetUserConnect;
}

public struct EventArg_UserAccountSet
{
	public string m_strDeviceID;
	public eNetwork_LoginBind m_eLoginBind;
}

public struct EventArg_PurchaseShopProductStart
{
	public eIAPContentType m_eIAPContentType;
	public int m_nID;
	public string m_strProductID;
}

public struct EventArg_GetLastPurchaseBuySeq
{
	public eIAPContentType m_eIAPContentType;
	public string m_strPaymentID;
}

public struct EventArg_PurchaseShopProductFinish
{
	public eIAPContentType m_eIAPContentType;
	public int m_nBuySeq;
}

public struct EventArg_PurchaseShopProductCancel
{
	public eIAPContentType m_eIAPContentType;
	public int m_nBuySeq;
}

public struct EventArg_PurchaseShopProduct
{
	public eIAPContentType m_eIAPContentType;
	public int m_nBuySeq;
	public int m_nStore;
	public string m_strPackageName;
	public string m_strProductID;
	public string m_strToken;
}

public struct EventArg_GetShopPackageProduct
{
	public int m_nPackageKind;
}

public struct EventArg_PurchasePackageShopProductStart
{
	public int m_nID;
}

public struct EventArg_GetLastPackagePurchaseBuySeq
{
	public string m_strPaymentID;
}

public struct EventArg_PurchasePackageShopProductFinish
{
	public int m_nBuySeq;
}

public struct EventArg_PurchasePackageShopProductCancel
{
	public int m_nBuySeq;
}

public struct EventArg_PurchasePackageShopProduct
{
	public int m_nBuySeq;
	public int m_nStore;
	public string m_strPackageName;
	public string m_strProductID;
	public string m_strToken;
}

public struct EventArg_NotPurchasePackageShopProduct
{
	public int m_nBuySeq;
}

public struct EventArg_BattlePassReward
{
	public bool m_IsFree;
	public int m_nLevel;
}

public struct EventArg_BattlePassSkip
{
	public int m_nLevel;
}

public struct EventArg_GameEventPvpStart
{
	public int m_nDeckID;
	public int m_nStageID;
}

public struct EventArg_GameEventPvpFinish
{
	public int m_nGameSeq;
	public int m_nResult;
}

public struct EventArg_GetUserChapterStarReward
{
	public int m_nChapterID;
	public int m_nRewardLevel;
}

public struct EventArg_GameChallengeStart
{
	public int m_nEpisodeID;
	public int m_nChapterID;
	public int m_nStageID;
}

public struct EventArg_GameChallengeFinish
{
	public int m_nEpisodeID;
	public int m_nChapterID;
	public int m_nStageID;
	public int m_nIsClear;
	public int m_nDestroyColonyCount;
	public int m_nMaxComboCount;
	public int m_nIsMissionClear_1;
	public int m_nIsMissionClear_2;
	public int m_nIsMissionClear_3;
}

public struct EventArg_GetUserChapterMissionReward
{
	public int m_nChapterID;
	public int m_nRewardLevel;
}