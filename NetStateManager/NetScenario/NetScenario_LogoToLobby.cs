using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;

public class NetScenario_LogoToLobby : NetScenario<EventArg_Null, eNetState>
{
    private NetComponent_LoginTouchToStart      m_pNetComponent_LoginTouchToStart       = new NetComponent_LoginTouchToStart();
    private NetComponent_TermsAgreementPopup    m_pNetComponent_TermAgreementPopup      = new NetComponent_TermsAgreementPopup();
    private NetComponent_SocialLogin            m_pNetComponent_SocialLogin             = new NetComponent_SocialLogin();
    private NetComponent_RouterServer           m_pNetComponent_RouterServer            = new NetComponent_RouterServer();

    private NetComponent_Login                  m_pNetComponent_Login                   = new NetComponent_Login();
    private NetComponent_CreateUser             m_pNetComponent_CreateUser              = new NetComponent_CreateUser();
    private NetComponent_GetUserUnit            m_pNetComponent_GetUserUnit             = new NetComponent_GetUserUnit();
    private NetComponent_GetUserInventory       m_pNetComponent_GetUserInventory        = new NetComponent_GetUserInventory();
    private NetComponent_MainData               m_pNetComponent_MainData                = new NetComponent_MainData();
    private NetComponent_TeamDeck               m_pNetComponent_TeamDeck                = new NetComponent_TeamDeck();
    private List<NetComponent_GetModeTeamDeck>  m_NetComponent_GetModeTeamDeckList      = new List<NetComponent_GetModeTeamDeck>();

    private NetComponent_GetUserEpisodeInfo     m_pNetComponent_GetUserEpisodeInfo      = new NetComponent_GetUserEpisodeInfo();
    private NetComponent_GetSummonShopList      m_pNetComponent_GetSummonShopList       = new NetComponent_GetSummonShopList();
    private NetComponent_GetSpecialStageList    m_pNetComponent_GetSpecialStageList     = new NetComponent_GetSpecialStageList();
    private NetComponent_GetDailyStageList      m_pNetComponent_GetDailyStageList       = new NetComponent_GetDailyStageList();
    private NetComponent_GetEventStageList      m_pNetComponent_GetEventStageList       = new NetComponent_GetEventStageList();
    private NetComponent_GetInfoUserEventPvp    m_pNetComponent_GetInfoUserEventPvp     = new NetComponent_GetInfoUserEventPvp();

    private NetComponent_GetShopBasicProduct    m_pNetComponent_GetShopBasicProduct     = new NetComponent_GetShopBasicProduct();
    private NetComponent_GetShopDailyProduct    m_pNetComponent_GetShopDailyProduct     = new NetComponent_GetShopDailyProduct();

    private NetComponent_GetShopPackageProduct_ForScenario m_pNetComponent_GetShopPackageProduct_ForScenario = new NetComponent_GetShopPackageProduct_ForScenario();
    private NetComponent_GetCyclePackageProduct_ForScenario m_pNetComponent_GetCyclePackageProduct_ForScenario = new NetComponent_GetCyclePackageProduct_ForScenario();
    private NetComponent_GetRankPackageProduct_ForScenario m_pNetComponent_GetRankPackageProduct_ForScenario = new NetComponent_GetRankPackageProduct_ForScenario();
    private NetComponent_GetCharacterPackageProduct_ForScenario m_pNetComponent_GetCharacterPackageProduct_ForScenario = new NetComponent_GetCharacterPackageProduct_ForScenario();
    private NetComponent_GetEventPackageProduct_ForScenario m_pNetComponent_GetEventPackageProduct_ForScenario = new NetComponent_GetEventPackageProduct_ForScenario();

    private NetComponent_GetPostList            m_pNetComponent_GetPostList             = new NetComponent_GetPostList();

    private NetComponent_GetUserInfoPvP         m_pNetComponent_GetUserInfoPvP          = new NetComponent_GetUserInfoPvP();

    private NetComponent_GetAttendance          m_pNetComponent_GetAttendance           = new NetComponent_GetAttendance();
    private NetComponent_UserInfoOfflineReward  m_pNetComponent_UserInfoOfflineReward   = new NetComponent_UserInfoOfflineReward();

    private NetComponent_Team                   m_pNetComponent_Team                    = new NetComponent_Team();
    private NetComponent_GetUserBattlePass      m_pNetComponent_GetUserBattlePass       = new NetComponent_GetUserBattlePass();

    private NetComponent_ChangeScene_Lobby      m_pNetComponent_ChangeScene_Lobby       = new NetComponent_ChangeScene_Lobby();
    private NetComponent_ConnectReward          m_pNetComponent_ConnectReward           = new NetComponent_ConnectReward();

    private NetComponent_Firebase_Register      m_pNetComponent_Firebase_Register       = new NetComponent_Firebase_Register();
    private NetComponent_Firebase_TryLogin      m_pNetComponent_Firebase_TryLogin       = new NetComponent_Firebase_TryLogin();
    private NetComponent_Firebase_SuccessLogin  m_pNetComponent_Firebase_SuccessLogin   = new NetComponent_Firebase_SuccessLogin();
    private NetComponent_Firebase_DailyFirstConnectLogin m_pNetComponent_Firebase_DailyFirstConnectLogin = new NetComponent_Firebase_DailyFirstConnectLogin();

    private TEventDelegate<Error>               m_pFail                                 = new TEventDelegate<Error>();
    private TEventDelegate<EventArg_Null>       m_pDone                                 = new TEventDelegate<EventArg_Null>();

    public NetScenario_LogoToLobby(eNetState eState) : base(eState)
    {
        m_pDone.SetFunc(OnDone);

        int nNumGameMode = ExcelDataManager.Instance.m_pGameMode.GetNumGameMode();

        for (int i = 0; i < nNumGameMode; ++i)
        {
            ExcelData_GameModeInfo pGameModeInfo = ExcelDataManager.Instance.m_pGameMode.GetGameModeInfo_byIndex(i);

            if (pGameModeInfo != null)
            {
                if(pGameModeInfo.m_nGameMode >= (int)eGameMode.EventStage && pGameModeInfo.m_nGameMode <= (int)eGameMode.EventStage_End)
                {
                    NetComponent_GetModeTeamDeck pGetModeTeamDeck = new NetComponent_GetModeTeamDeck(eGameMode.EventStage);
                    m_NetComponent_GetModeTeamDeckList.Add(pGetModeTeamDeck);
                }
                else if (pGameModeInfo.m_nGameMode >= (int)eGameMode.EventPvpStage)
                {
                    NetComponent_GetModeTeamDeck pGetModeTeamDeck = new NetComponent_GetModeTeamDeck(eGameMode.EventPvpStage);
                    m_NetComponent_GetModeTeamDeckList.Add(pGetModeTeamDeck);
                }
                else
                {
                    NetComponent_GetModeTeamDeck pGetModeTeamDeck = new NetComponent_GetModeTeamDeck((eGameMode)pGameModeInfo.m_nGameMode);
                    m_NetComponent_GetModeTeamDeckList.Add(pGetModeTeamDeck);
                }
            }
        }
    }

    public override void OnDestroy()
    {
        m_pNetComponent_LoginTouchToStart.OnDestroy();
        m_pNetComponent_TermAgreementPopup.OnDestroy();
        m_pNetComponent_SocialLogin.OnDestroy();
        m_pNetComponent_RouterServer.OnDestroy();

        m_pNetComponent_Login.OnDestroy();
        m_pNetComponent_CreateUser.OnDestroy();
        m_pNetComponent_GetUserUnit.OnDestroy();
        m_pNetComponent_GetUserInventory.OnDestroy();
        m_pNetComponent_MainData.OnDestroy();
        m_pNetComponent_TeamDeck.OnDestroy();
        foreach (NetComponent_GetModeTeamDeck pNetComponent in m_NetComponent_GetModeTeamDeckList)
        {
            pNetComponent.OnDestroy();
        }

        m_pNetComponent_GetUserEpisodeInfo.OnDestroy();
        m_pNetComponent_GetSummonShopList.OnDestroy();
        m_pNetComponent_GetSpecialStageList.OnDestroy();
        m_pNetComponent_GetDailyStageList.OnDestroy();
        m_pNetComponent_GetEventStageList.OnDestroy();
        m_pNetComponent_GetInfoUserEventPvp.OnDestroy();

        m_pNetComponent_GetShopBasicProduct.OnDestroy();
        m_pNetComponent_GetShopDailyProduct.OnDestroy();
        m_pNetComponent_GetShopPackageProduct_ForScenario.OnDestroy();
        m_pNetComponent_GetCyclePackageProduct_ForScenario.OnDestroy();
        m_pNetComponent_GetRankPackageProduct_ForScenario.OnDestroy();
        m_pNetComponent_GetCharacterPackageProduct_ForScenario.OnDestroy();
        m_pNetComponent_GetEventPackageProduct_ForScenario.OnDestroy();

        m_pNetComponent_GetPostList.OnDestroy();

        m_pNetComponent_GetUserInfoPvP.OnDestroy();

        m_pNetComponent_GetAttendance.OnDestroy();
        m_pNetComponent_GetUserBattlePass.OnDestroy();

        m_pNetComponent_ChangeScene_Lobby.OnDestroy();
        m_pNetComponent_ConnectReward.OnDestroy();

        m_pNetComponent_Firebase_Register.OnDestroy();
        m_pNetComponent_Firebase_TryLogin.OnDestroy();
        m_pNetComponent_Firebase_SuccessLogin.OnDestroy();
        m_pNetComponent_Firebase_DailyFirstConnectLogin.OnDestroy();
    }

    public override void Update()
    {
        m_pNetComponent_LoginTouchToStart.Update();
        m_pNetComponent_TermAgreementPopup.Update();
        m_pNetComponent_SocialLogin.Update();
        m_pNetComponent_RouterServer.Update();

        m_pNetComponent_Login.Update();
        m_pNetComponent_CreateUser.Update();
        m_pNetComponent_GetUserUnit.Update();
        m_pNetComponent_GetUserInventory.Update();
        m_pNetComponent_MainData.Update();
        m_pNetComponent_TeamDeck.Update();
        foreach (NetComponent_GetModeTeamDeck pNetComponent in m_NetComponent_GetModeTeamDeckList)
        {
            pNetComponent.Update();
        }

        m_pNetComponent_GetUserEpisodeInfo.Update();
        m_pNetComponent_GetSummonShopList.Update();
        m_pNetComponent_GetSpecialStageList.Update();
        m_pNetComponent_GetDailyStageList.Update();
        m_pNetComponent_GetEventStageList.Update();
        m_pNetComponent_GetInfoUserEventPvp.Update();

        m_pNetComponent_GetShopBasicProduct.Update();
        m_pNetComponent_GetShopDailyProduct.Update();
        m_pNetComponent_GetShopPackageProduct_ForScenario.Update();
        m_pNetComponent_GetCyclePackageProduct_ForScenario.Update();
        m_pNetComponent_GetRankPackageProduct_ForScenario.Update();
        m_pNetComponent_GetCharacterPackageProduct_ForScenario.Update();
        m_pNetComponent_GetEventPackageProduct_ForScenario.Update();

        m_pNetComponent_GetPostList.Update();

        m_pNetComponent_GetUserInfoPvP.Update();

        m_pNetComponent_GetAttendance.Update();
        m_pNetComponent_GetUserBattlePass.Update();

        m_pNetComponent_ChangeScene_Lobby.Update();
        m_pNetComponent_ConnectReward.Update();

        m_pNetComponent_Firebase_Register.Update();
        m_pNetComponent_Firebase_TryLogin.Update();
        m_pNetComponent_Firebase_SuccessLogin.Update();
        m_pNetComponent_Firebase_DailyFirstConnectLogin.Update();
    }

    public override void LateUpdate()
    {
        m_pNetComponent_LoginTouchToStart.LateUpdate();
        m_pNetComponent_TermAgreementPopup.LateUpdate();
        m_pNetComponent_SocialLogin.LateUpdate();
        m_pNetComponent_RouterServer.LateUpdate();

        m_pNetComponent_Login.LateUpdate();
        m_pNetComponent_CreateUser.LateUpdate();
        m_pNetComponent_GetUserUnit.LateUpdate();
        m_pNetComponent_GetUserInventory.LateUpdate();
        m_pNetComponent_MainData.LateUpdate();
        m_pNetComponent_TeamDeck.LateUpdate();
        foreach (NetComponent_GetModeTeamDeck pNetComponent in m_NetComponent_GetModeTeamDeckList)
        {
            pNetComponent.LateUpdate();
        }

        m_pNetComponent_GetUserEpisodeInfo.LateUpdate();
        m_pNetComponent_GetSummonShopList.LateUpdate();
        m_pNetComponent_GetSpecialStageList.LateUpdate();
        m_pNetComponent_GetDailyStageList.LateUpdate();
        m_pNetComponent_GetEventStageList.LateUpdate();
        m_pNetComponent_GetInfoUserEventPvp.LateUpdate();

        m_pNetComponent_GetShopBasicProduct.LateUpdate();
        m_pNetComponent_GetShopDailyProduct.LateUpdate();
        m_pNetComponent_GetShopPackageProduct_ForScenario.LateUpdate();
        m_pNetComponent_GetCyclePackageProduct_ForScenario.LateUpdate();
        m_pNetComponent_GetRankPackageProduct_ForScenario.LateUpdate();
        m_pNetComponent_GetCharacterPackageProduct_ForScenario.LateUpdate();
        m_pNetComponent_GetEventPackageProduct_ForScenario.LateUpdate();

        m_pNetComponent_GetPostList.LateUpdate();

        m_pNetComponent_GetUserInfoPvP.LateUpdate();

        m_pNetComponent_GetAttendance.LateUpdate();
        m_pNetComponent_GetUserBattlePass.LateUpdate();

        m_pNetComponent_ChangeScene_Lobby.LateUpdate();
        m_pNetComponent_ConnectReward.LateUpdate();

        m_pNetComponent_Firebase_Register.LateUpdate();
        m_pNetComponent_Firebase_TryLogin.LateUpdate();
        m_pNetComponent_Firebase_SuccessLogin.LateUpdate();
        m_pNetComponent_Firebase_DailyFirstConnectLogin.LateUpdate();
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        NetLog.Log("NetScenario_LogoToLobby OnEvent");

        if (GameDefine.ms_UseNetwork == false)
        {
            AppInstance.Instance.m_pSceneManager.ChangeScene(eSceneType.Scene_Lobby);
            return;
        }

        m_pFail.SetFunc(OnLoginFail);

        m_pNetComponent_LoginTouchToStart.SetNextEvent(m_pNetComponent_TermAgreementPopup);
        m_pNetComponent_TermAgreementPopup.SetNextEvent(m_pNetComponent_SocialLogin);
        m_pNetComponent_SocialLogin.SetSuccessEvent(m_pNetComponent_RouterServer);
        m_pNetComponent_RouterServer.SetNextEvent(m_pNetComponent_Firebase_TryLogin);
        m_pNetComponent_Firebase_TryLogin.SetNextEvent(m_pNetComponent_Login);

        m_pNetComponent_Login.SetSuccessEvent(m_pNetComponent_Firebase_SuccessLogin);
        m_pNetComponent_Login.SetFailureEvent(m_pFail);
        m_pNetComponent_Firebase_SuccessLogin.SetNextEvent(m_pNetComponent_GetUserUnit);
        m_pNetComponent_GetUserUnit.SetSuccessEvent(m_pNetComponent_GetUserInventory);
        m_pNetComponent_GetUserUnit.SetFailureEvent(m_pFail);
        m_pNetComponent_GetUserInventory.SetSuccessEvent(m_pNetComponent_MainData);
        m_pNetComponent_GetUserInventory.SetFailureEvent(m_pFail);
        m_pNetComponent_MainData.SetSuccessEvent(m_pNetComponent_GetUserEpisodeInfo);
        m_pNetComponent_MainData.SetFailureEvent(m_pFail);
        m_pNetComponent_GetUserEpisodeInfo.SetSuccessEvent(m_pNetComponent_GetUserBattlePass);
        m_pNetComponent_GetUserEpisodeInfo.SetFailureEvent(m_pFail);
		m_pNetComponent_GetUserBattlePass.SetSuccessEvent(m_pNetComponent_GetSummonShopList);
		m_pNetComponent_GetUserBattlePass.SetFailureEvent(m_pFail);
		m_pNetComponent_GetSummonShopList.SetSuccessEvent(m_pNetComponent_GetSpecialStageList);
        m_pNetComponent_GetSummonShopList.SetFailureEvent(m_pFail);
        m_pNetComponent_GetSpecialStageList.SetSuccessEvent(m_pNetComponent_GetDailyStageList);
        m_pNetComponent_GetSpecialStageList.SetFailureEvent(m_pFail);
        m_pNetComponent_GetDailyStageList.SetSuccessEvent(m_pNetComponent_GetEventStageList);
        m_pNetComponent_GetDailyStageList.SetFailureEvent(m_pFail);
        m_pNetComponent_GetEventStageList.SetSuccessEvent(m_pNetComponent_GetInfoUserEventPvp);
        m_pNetComponent_GetEventStageList.SetFailureEvent(m_pFail);

        m_pNetComponent_GetInfoUserEventPvp.SetSuccessEvent(m_pNetComponent_GetShopPackageProduct_ForScenario);
        m_pNetComponent_GetInfoUserEventPvp.SetFailureEvent(m_pFail);

        m_pNetComponent_GetShopPackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetCyclePackageProduct_ForScenario);
        m_pNetComponent_GetShopPackageProduct_ForScenario.SetFailureEvent(m_pFail);
        m_pNetComponent_GetCyclePackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetRankPackageProduct_ForScenario);
        m_pNetComponent_GetCyclePackageProduct_ForScenario.SetFailureEvent(m_pFail);
        m_pNetComponent_GetRankPackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetCharacterPackageProduct_ForScenario);
        m_pNetComponent_GetRankPackageProduct_ForScenario.SetFailureEvent(m_pFail);
        m_pNetComponent_GetCharacterPackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetEventPackageProduct_ForScenario);
        m_pNetComponent_GetCharacterPackageProduct_ForScenario.SetFailureEvent(m_pFail);
        m_pNetComponent_GetEventPackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetShopBasicProduct);
        m_pNetComponent_GetEventPackageProduct_ForScenario.SetFailureEvent(m_pFail);

        m_pNetComponent_GetShopBasicProduct.SetSuccessEvent(m_pNetComponent_GetShopDailyProduct);
        m_pNetComponent_GetShopBasicProduct.SetFailureEvent(m_pFail);
        m_pNetComponent_GetShopDailyProduct.SetSuccessEvent(m_pNetComponent_GetPostList);
        m_pNetComponent_GetShopDailyProduct.SetFailureEvent(m_pFail);


        //m_pNetComponent_GetShopPackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetCyclePackageProduct_ForScenario);
        //m_pNetComponent_GetShopPackageProduct_ForScenario.SetFailureEvent(m_pFail);
        //m_pNetComponent_GetCyclePackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetRankPackageProduct_ForScenario);
        //m_pNetComponent_GetCyclePackageProduct_ForScenario.SetFailureEvent(m_pFail);
        //m_pNetComponent_GetRankPackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetCharacterPackageProduct_ForScenario);
        //m_pNetComponent_GetRankPackageProduct_ForScenario.SetFailureEvent(m_pFail);
        //m_pNetComponent_GetCharacterPackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetPostList);
        //m_pNetComponent_GetCharacterPackageProduct_ForScenario.SetFailureEvent(m_pFail);

        m_pNetComponent_GetPostList.SetSuccessEvent(m_pNetComponent_GetUserInfoPvP);
        m_pNetComponent_GetPostList.SetFailureEvent(m_pFail);
        m_pNetComponent_GetUserInfoPvP.SetSuccessEvent(m_pNetComponent_GetAttendance);
        m_pNetComponent_GetUserInfoPvP.SetFailureEvent(m_pFail);
        m_pNetComponent_GetAttendance.SetSuccessEvent(m_pNetComponent_UserInfoOfflineReward);
        m_pNetComponent_GetAttendance.SetFailureEvent(m_pFail);
        m_pNetComponent_UserInfoOfflineReward.SetSuccessEvent(m_pNetComponent_Team);
        m_pNetComponent_UserInfoOfflineReward.SetFailureEvent(m_pFail);
        //m_pNetComponent_GetUserBattlePass.SetSuccessEvent(m_pNetComponent_Team);
        //m_pNetComponent_GetUserBattlePass.SetFailureEvent(m_pFail);
        m_pNetComponent_Team.SetSuccessEvent(m_pNetComponent_TeamDeck);
        m_pNetComponent_Team.SetFailureEvent(m_pFail);

        if (m_NetComponent_GetModeTeamDeckList.Count == 0)
        {
            m_pNetComponent_TeamDeck.SetSuccessEvent(m_pNetComponent_Firebase_DailyFirstConnectLogin);
            m_pNetComponent_TeamDeck.SetFailureEvent(m_pFail);
        }
        else
        {
            NetComponent_GetModeTeamDeck pNetComponent_GetModeTeamDeck = m_NetComponent_GetModeTeamDeckList[0];
            m_pNetComponent_TeamDeck.SetSuccessEvent(pNetComponent_GetModeTeamDeck);
            m_pNetComponent_TeamDeck.SetFailureEvent(m_pFail);

            for (int i = 1; i < m_NetComponent_GetModeTeamDeckList.Count; ++i)
            {
                NetComponent_GetModeTeamDeck pNetComponent_NextGetModeTeamDeck = m_NetComponent_GetModeTeamDeckList[i];
                pNetComponent_GetModeTeamDeck.SetSuccessEvent(pNetComponent_NextGetModeTeamDeck);
                pNetComponent_GetModeTeamDeck.SetFailureEvent(m_pFail);
                pNetComponent_GetModeTeamDeck = pNetComponent_NextGetModeTeamDeck;
            }

            pNetComponent_GetModeTeamDeck.SetSuccessEvent(m_pNetComponent_Firebase_DailyFirstConnectLogin);
            pNetComponent_GetModeTeamDeck.SetFailureEvent(m_pFail);
        }

        m_pNetComponent_Firebase_DailyFirstConnectLogin.SetNextEvent(m_pNetComponent_ChangeScene_Lobby);

        m_pNetComponent_ChangeScene_Lobby.SetNextEvent(m_pNetComponent_ConnectReward);
        m_pNetComponent_ConnectReward.SetNextEvent(m_pDone);

        m_pNetComponent_LoginTouchToStart.OnEvent(EventArg_Null.Object);
    }

    public void OnLoginFail(Error Arg)
    {
        if (Arg.Err_code == errorConstants.kNotExistUser)
        {
            m_pFail.SetFunc(OnCreateUserFail);
            m_pNetComponent_CreateUser.SetSuccessEvent(m_pNetComponent_Login);
            m_pNetComponent_CreateUser.SetFailureEvent(m_pFail);
            m_pNetComponent_Login.SetSuccessEvent(m_pNetComponent_Firebase_SuccessLogin);
            m_pNetComponent_Login.SetFailureEvent(m_pFail);
            m_pNetComponent_Firebase_SuccessLogin.SetNextEvent(m_pNetComponent_Firebase_Register);
            m_pNetComponent_Firebase_Register.SetNextEvent(m_pNetComponent_GetUserUnit);     
            m_pNetComponent_GetUserUnit.SetSuccessEvent(m_pNetComponent_GetUserInventory);
            m_pNetComponent_GetUserUnit.SetFailureEvent(m_pFail);
            m_pNetComponent_GetUserInventory.SetSuccessEvent(m_pNetComponent_MainData);
            m_pNetComponent_GetUserInventory.SetFailureEvent(m_pFail);
            m_pNetComponent_MainData.SetSuccessEvent(m_pNetComponent_GetUserEpisodeInfo);
            m_pNetComponent_MainData.SetFailureEvent(m_pFail);
            m_pNetComponent_GetUserEpisodeInfo.SetSuccessEvent(m_pNetComponent_GetUserBattlePass);
            m_pNetComponent_GetUserEpisodeInfo.SetFailureEvent(m_pFail);
			m_pNetComponent_GetUserBattlePass.SetSuccessEvent(m_pNetComponent_GetSummonShopList);
			m_pNetComponent_GetUserBattlePass.SetFailureEvent(m_pFail);
			m_pNetComponent_GetSummonShopList.SetSuccessEvent(m_pNetComponent_GetSpecialStageList);
            m_pNetComponent_GetSummonShopList.SetFailureEvent(m_pFail);
            m_pNetComponent_GetSpecialStageList.SetSuccessEvent(m_pNetComponent_GetDailyStageList);
            m_pNetComponent_GetSpecialStageList.SetFailureEvent(m_pFail);
            m_pNetComponent_GetDailyStageList.SetSuccessEvent(m_pNetComponent_GetEventStageList);
            m_pNetComponent_GetDailyStageList.SetFailureEvent(m_pFail);
            m_pNetComponent_GetEventStageList.SetSuccessEvent(m_pNetComponent_GetInfoUserEventPvp);
            m_pNetComponent_GetEventStageList.SetFailureEvent(m_pFail);

            m_pNetComponent_GetInfoUserEventPvp.SetSuccessEvent(m_pNetComponent_GetShopPackageProduct_ForScenario);
            m_pNetComponent_GetInfoUserEventPvp.SetFailureEvent(m_pFail);

            m_pNetComponent_GetShopPackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetCyclePackageProduct_ForScenario);
            m_pNetComponent_GetShopPackageProduct_ForScenario.SetFailureEvent(m_pFail);
            m_pNetComponent_GetCyclePackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetRankPackageProduct_ForScenario);
            m_pNetComponent_GetCyclePackageProduct_ForScenario.SetFailureEvent(m_pFail);
            m_pNetComponent_GetRankPackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetCharacterPackageProduct_ForScenario);
            m_pNetComponent_GetRankPackageProduct_ForScenario.SetFailureEvent(m_pFail);
            m_pNetComponent_GetCharacterPackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetEventPackageProduct_ForScenario);
            m_pNetComponent_GetCharacterPackageProduct_ForScenario.SetFailureEvent(m_pFail);
            m_pNetComponent_GetEventPackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetShopBasicProduct);
            m_pNetComponent_GetEventPackageProduct_ForScenario.SetFailureEvent(m_pFail);

            m_pNetComponent_GetShopBasicProduct.SetSuccessEvent(m_pNetComponent_GetShopDailyProduct);
            m_pNetComponent_GetShopBasicProduct.SetFailureEvent(m_pFail);
            m_pNetComponent_GetShopDailyProduct.SetSuccessEvent(m_pNetComponent_GetPostList);
            m_pNetComponent_GetShopDailyProduct.SetFailureEvent(m_pFail);


            //m_pNetComponent_GetDailyStageList.SetSuccessEvent(m_pNetComponent_GetShopBasicProduct);
            //m_pNetComponent_GetDailyStageList.SetFailureEvent(m_pFail);
            //m_pNetComponent_GetShopBasicProduct.SetSuccessEvent(m_pNetComponent_GetShopDailyProduct);
            //m_pNetComponent_GetShopBasicProduct.SetFailureEvent(m_pFail);
            //m_pNetComponent_GetShopDailyProduct.SetSuccessEvent(m_pNetComponent_GetShopPackageProduct_ForScenario);
            //m_pNetComponent_GetShopDailyProduct.SetFailureEvent(m_pFail);
            //m_pNetComponent_GetShopPackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetCyclePackageProduct_ForScenario);
            //m_pNetComponent_GetShopPackageProduct_ForScenario.SetFailureEvent(m_pFail);
            //m_pNetComponent_GetCyclePackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetRankPackageProduct_ForScenario);
            //m_pNetComponent_GetCyclePackageProduct_ForScenario.SetFailureEvent(m_pFail);
            //m_pNetComponent_GetRankPackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetCharacterPackageProduct_ForScenario);
            //m_pNetComponent_GetRankPackageProduct_ForScenario.SetFailureEvent(m_pFail);
            //m_pNetComponent_GetCharacterPackageProduct_ForScenario.SetSuccessEvent(m_pNetComponent_GetPostList);
            //m_pNetComponent_GetCharacterPackageProduct_ForScenario.SetFailureEvent(m_pFail);

            m_pNetComponent_GetPostList.SetSuccessEvent(m_pNetComponent_GetUserInfoPvP);
            m_pNetComponent_GetPostList.SetFailureEvent(m_pFail);
            m_pNetComponent_GetUserInfoPvP.SetSuccessEvent(m_pNetComponent_GetAttendance);
            m_pNetComponent_GetUserInfoPvP.SetFailureEvent(m_pFail);
            m_pNetComponent_GetAttendance.SetSuccessEvent(m_pNetComponent_UserInfoOfflineReward);
            m_pNetComponent_GetAttendance.SetFailureEvent(m_pFail);
            m_pNetComponent_UserInfoOfflineReward.SetSuccessEvent(m_pNetComponent_Team);
            m_pNetComponent_UserInfoOfflineReward.SetFailureEvent(m_pFail);
            //m_pNetComponent_GetUserBattlePass.SetSuccessEvent(m_pNetComponent_Team);
            //m_pNetComponent_GetUserBattlePass.SetFailureEvent(m_pFail);
            m_pNetComponent_Team.SetSuccessEvent(m_pNetComponent_TeamDeck);
            m_pNetComponent_Team.SetFailureEvent(m_pFail);

            if (m_NetComponent_GetModeTeamDeckList.Count == 0)
            {
                m_pNetComponent_TeamDeck.SetSuccessEvent(m_pNetComponent_Firebase_DailyFirstConnectLogin);
                m_pNetComponent_TeamDeck.SetFailureEvent(m_pFail);
            }
            else
            {
                NetComponent_GetModeTeamDeck pNetComponent_GetModeTeamDeck = m_NetComponent_GetModeTeamDeckList[0];
                m_pNetComponent_TeamDeck.SetSuccessEvent(pNetComponent_GetModeTeamDeck);
                m_pNetComponent_TeamDeck.SetFailureEvent(m_pFail);

                for (int i = 1; i < m_NetComponent_GetModeTeamDeckList.Count; ++i)
                {
                    NetComponent_GetModeTeamDeck pNetComponent_NextGetModeTeamDeck = m_NetComponent_GetModeTeamDeckList[i];
                    pNetComponent_GetModeTeamDeck.SetSuccessEvent(pNetComponent_NextGetModeTeamDeck);
                    pNetComponent_GetModeTeamDeck.SetFailureEvent(m_pFail);
                    pNetComponent_GetModeTeamDeck = pNetComponent_NextGetModeTeamDeck;
                }

                pNetComponent_GetModeTeamDeck.SetSuccessEvent(m_pNetComponent_Firebase_DailyFirstConnectLogin);
                pNetComponent_GetModeTeamDeck.SetFailureEvent(m_pFail);
            }

            m_pNetComponent_Firebase_DailyFirstConnectLogin.SetNextEvent(m_pNetComponent_ChangeScene_Lobby);
            m_pNetComponent_ChangeScene_Lobby.SetNextEvent(m_pNetComponent_ConnectReward);
            m_pNetComponent_ConnectReward.SetNextEvent(m_pDone);

            m_pNetComponent_CreateUser.OnEvent(EventArg_Null.Object);
        }
    }

    public void OnCreateUserFail(Error Arg)
    {
    }

    public void OnDone(EventArg_Null Arg)
    {
        GetNextEvent().OnEvent(m_eNetState);
    }
}
