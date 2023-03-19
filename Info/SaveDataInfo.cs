using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveDataInfo : Singleton<SaveDataInfo>
{
	public bool								m_IsNewUser						= true;

	public int								m_nRecentSelectChapter			= 0;
	public int								m_nRecentSelectStage			= 0;

	public int								m_nProfileAvatarTableID			= 0;

	public int								m_nYear = 0;
	public int								m_nMonth = 0;
	public int								m_nDay = 0;
	public bool								m_IsGachaPageRedDotOn			= false;

	public DateTime							m_dtDailyStoreRefreshTime		= DateTime.Now;
	public bool								m_IsDailyStoreRefresh			= false;

	public eNetwork_LoginBind				m_eNetwork_LoginBind			= eNetwork_LoginBind.None;
	public string							m_strNetwork_LoginAccountID		= "";

	public bool								m_IsEventPopupShow				= false;

	private SaveDataInfo()
	{
	}

	public void ClearAll()
	{
		m_IsNewUser = true;

		m_nRecentSelectChapter = 0;
		m_nRecentSelectStage = 0;

		m_nProfileAvatarTableID = 0;

		m_nYear = 0;
		m_nMonth = 0;
		m_nDay = 0;
		m_IsGachaPageRedDotOn = false;

		m_dtDailyStoreRefreshTime = DateTime.Now;
		m_IsDailyStoreRefresh = false;

		m_eNetwork_LoginBind = eNetwork_LoginBind.None;
		m_strNetwork_LoginAccountID = "";

		PlayerPrefs.DeleteAll();
	}

	public void Load()
	{
		int nNewUser = PlayerPrefs.GetInt(PlayerPrefsString.ms_NewUser, 0);
		m_IsNewUser = nNewUser == 0 ? true : false;

		if (m_IsNewUser == true)
		{
			//InventoryInfoManager.Instance.m_pMainStageInvenInfo.Init();

			m_nRecentSelectChapter = 101;
			RecentSelectChapterSave();
			m_nRecentSelectStage = 10101;
			RecentSelectStageSave();
			EspressoInfo.Instance.m_nChapterID = m_nRecentSelectChapter;
		}
		else
		{
			// 유저 챕터 해금 목록 불러오기

			Dictionary<int, ExcelData_ChapterInfo> pChapterInfo = new Dictionary<int, ExcelData_ChapterInfo>();
			pChapterInfo = ExcelDataManager.Instance.m_pChapter.GetAllChapterInfoList();
			
			Dictionary<int, bool> pChapterUnlockList = new Dictionary<int, bool>();

			foreach (KeyValuePair<int, ExcelData_ChapterInfo> pChapter in pChapterInfo)
			{
				int nTempNumber = PlayerPrefs.GetInt(PlayerPrefsString.ms_ChapterUnlockList + "-" + pChapter.Key);

				if (nTempNumber == 1)
				{
					pChapterUnlockList.Add(pChapter.Key, true);
				}
				else
				{
					pChapterUnlockList.Add(pChapter.Key, false);
				}
			}

			// 유저 스테이지 해금 목록 불러오기

			Dictionary<int, ExcelData_StageInfo> pStageList = new Dictionary<int, ExcelData_StageInfo>();
			pStageList = ExcelDataManager.Instance.m_pStage.GetAllStageInfoList();
			
			Dictionary<int, bool> pStageUnlockList = new Dictionary<int, bool>();
			Dictionary<int, int> pStageClearRank = new Dictionary<int, int>();
			Dictionary<int, bool> pStageLimitReward = new Dictionary<int, bool>();

			foreach (KeyValuePair<int, ExcelData_StageInfo> pStage in pStageList)
			{
				int nTempNumber = PlayerPrefs.GetInt(PlayerPrefsString.ms_StageUnlockList + "-" + pStage.Key);

				if (nTempNumber == 1)
				{
					pStageUnlockList.Add(pStage.Key, true);
				}
				else
				{
					pStageUnlockList.Add(pStage.Key, false);
				}

				nTempNumber = PlayerPrefs.GetInt(PlayerPrefsString.ms_StageClearRankList + "-" + pStage.Key);

				pStageClearRank.Add(pStage.Key, nTempNumber);

				nTempNumber = PlayerPrefs.GetInt(PlayerPrefsString.ms_StageClearLimitList + "-" + pStage.Key);

				if(nTempNumber == 1)
                {
					pStageLimitReward.Add(pStage.Key, true);
                }
                else
                {
					pStageLimitReward.Add(pStage.Key, false);
                }
			}

			//InventoryInfoManager.Instance.m_pMainStageInvenInfo.Load(pChapterUnlockList, pStageUnlockList, pStageClearRank, pStageLimitReward);
		}

		m_IsNewUser = true;
		PlayerPrefs.SetInt(PlayerPrefsString.ms_NewUser, 1);

		m_nRecentSelectChapter = PlayerPrefs.GetInt(PlayerPrefsString.ms_SelectChapterIndex);
		m_nRecentSelectStage = PlayerPrefs.GetInt(PlayerPrefsString.ms_SelectStageIndex);
		EspressoInfo.Instance.m_nChapterID = m_nRecentSelectChapter;
		EspressoInfo.Instance.m_nStageID = m_nRecentSelectStage;

		//if(PlayerPrefs.GetInt(PlayerPrefsString.ms_ProfileAvaterTableID) == 0)
  //      {
		//	// 엘사로 임시 저장
		//	m_nProfileAvatarTableID = 100001;
		//	ProfileAvatarTableIDSave();
  //      }
  //      else
  //      {
		//	m_nProfileAvatarTableID = PlayerPrefs.GetInt(PlayerPrefsString.ms_ProfileAvaterTableID);
  //      }
		//MyInfo.Instance.m_nProfileAvatar = m_nProfileAvatarTableID;

		if (PlayerPrefs.GetInt(PlayerPrefsString.ms_CurrentDateYear) == 0 && PlayerPrefs.GetInt(PlayerPrefsString.ms_CurrentDateMonth) == 0 && PlayerPrefs.GetInt(PlayerPrefsString.ms_CurrentDateDay) == 0)
		{
			DateTime YesterDay = new DateTime();
			YesterDay = DateTime.Today.AddDays(-1);

			m_nYear = YesterDay.Year;
			m_nMonth = YesterDay.Month;
			m_nDay = YesterDay.Day;

			CurrentDateSave();
		}
        else
        {
			m_nYear = PlayerPrefs.GetInt(PlayerPrefsString.ms_CurrentDateYear);
			m_nMonth = PlayerPrefs.GetInt(PlayerPrefsString.ms_CurrentDateMonth);
			m_nDay = PlayerPrefs.GetInt(PlayerPrefsString.ms_CurrentDateDay);
		}

        if (PlayerPrefs.GetInt(PlayerPrefsString.ms_IsGachaPageRedDot) == 1)
        {
			m_IsGachaPageRedDotOn = true;
        }
        else
        {
			m_IsGachaPageRedDotOn = false;
		}

		GachaPageRedDotSave();

		if(m_nYear != EspressoInfo.Instance.m_pConnectDateTime.Year || m_nMonth != EspressoInfo.Instance.m_pConnectDateTime.Month || m_nDay != EspressoInfo.Instance.m_pConnectDateTime.Day)
        {
			m_IsGachaPageRedDotOn = true;
			PlayerPrefs.SetInt(PlayerPrefsString.ms_IsGachaPageRedDot, 1);
			GachaPageRedDotSave();

			m_nYear = EspressoInfo.Instance.m_pConnectDateTime.Year;
			m_nMonth = EspressoInfo.Instance.m_pConnectDateTime.Month;
			m_nDay = EspressoInfo.Instance.m_pConnectDateTime.Day;

			CurrentDateSave();
		}

		MyInfo.Instance.m_IsGachaPageRedDotOn = m_IsGachaPageRedDotOn;

		CheckStoreRefreshRemainTime();

		m_eNetwork_LoginBind = EnumUtil<eNetwork_LoginBind>.ConvertStringToEnum(PlayerPrefs.GetString(PlayerPrefsString.ms_Login_BindType, "None"));
		m_strNetwork_LoginAccountID = PlayerPrefs.GetString(PlayerPrefsString.ms_Login_AccountID, "");

		int nEventPopupShow = PlayerPrefs.GetInt(PlayerPrefsString.ms_EventPopupShow, 0);

		if(nEventPopupShow == 0)
        {
			m_IsEventPopupShow = false;
        }
        else
        {
			m_IsEventPopupShow = true;
		}
	}

	public void RecentSelectChapterSave()
    {
		PlayerPrefs.SetInt(PlayerPrefsString.ms_SelectChapterIndex, m_nRecentSelectChapter);
    }
	
	public void RecentSelectStageSave()
    {
		PlayerPrefs.SetInt(PlayerPrefsString.ms_SelectStageIndex, m_nRecentSelectStage);
	}

	public void ProfileAvatarTableIDSave()
    {
		PlayerPrefs.SetInt(PlayerPrefsString.ms_ProfileAvaterTableID, m_nProfileAvatarTableID);
    }

	public void CurrentDateSave()
    {
		PlayerPrefs.SetInt(PlayerPrefsString.ms_CurrentDateYear, m_nYear);
		PlayerPrefs.SetInt(PlayerPrefsString.ms_CurrentDateMonth, m_nMonth);
		PlayerPrefs.SetInt(PlayerPrefsString.ms_CurrentDateDay, m_nDay);
    }

	public void GachaPageRedDotSave()
    {
        if (m_IsGachaPageRedDotOn)
        {
			PlayerPrefs.SetInt(PlayerPrefsString.ms_IsGachaPageRedDot, 1);
		}
        else
        {
			PlayerPrefs.SetInt(PlayerPrefsString.ms_IsGachaPageRedDot, 0);
		}
    }

	public void DailyStoreRemainSave(DateTime dtRefreshTime)
    {
		PlayerPrefs.SetInt(PlayerPrefsString.ms_DailyStoreRefreshTime + "Year", dtRefreshTime.Year);
		PlayerPrefs.SetInt(PlayerPrefsString.ms_DailyStoreRefreshTime + "Month", dtRefreshTime.Month);
		PlayerPrefs.SetInt(PlayerPrefsString.ms_DailyStoreRefreshTime + "Day", dtRefreshTime.Day);
		PlayerPrefs.SetInt(PlayerPrefsString.ms_DailyStoreRefreshTime + "Hour", dtRefreshTime.Hour);
		PlayerPrefs.SetInt(PlayerPrefsString.ms_DailyStoreRefreshTime + "Minute", dtRefreshTime.Minute);
		PlayerPrefs.SetInt(PlayerPrefsString.ms_DailyStoreRefreshTime + "Second", dtRefreshTime.Second);
	}

	public void SetDailyStoreRemainTimeZero()
    {
		PlayerPrefs.SetInt(PlayerPrefsString.ms_DailyStoreRefreshTime + "Year", 0);
    }

	private void CheckStoreRefreshRemainTime()
    {
		DateTime dtRemainTime = DateTime.Now;

		if(PlayerPrefs.GetInt(PlayerPrefsString.ms_DailyStoreRefreshTime + "Year") != 0)
        {
			dtRemainTime = new DateTime(PlayerPrefs.GetInt(PlayerPrefsString.ms_DailyStoreRefreshTime + "Year"), PlayerPrefs.GetInt(PlayerPrefsString.ms_DailyStoreRefreshTime + "Month"), PlayerPrefs.GetInt(PlayerPrefsString.ms_DailyStoreRefreshTime + "Day"), PlayerPrefs.GetInt(PlayerPrefsString.ms_DailyStoreRefreshTime + "Hour"), PlayerPrefs.GetInt(PlayerPrefsString.ms_DailyStoreRefreshTime + "Minute"), PlayerPrefs.GetInt(PlayerPrefsString.ms_DailyStoreRefreshTime + "Second"));
        }

		InventoryInfoManager.Instance.m_pStoreInvenInfo.SetDailyStoreRefreshRemainTime_bySaveDataInfo(dtRemainTime);
    }

	public bool GetChapterPackageIsRedDot(int nChapter)
    {
		if (PlayerPrefs.GetInt(PlayerPrefsString.ms_ChapterPackageRedDot + " " + nChapter.ToString()) == 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public void SetChapterPackageIsRedDot(int nChapter)
    {
		PlayerPrefs.SetInt(PlayerPrefsString.ms_ChapterPackageRedDot + " " + nChapter.ToString(), 1);
    }

	public void SetEventPopupShow(bool IsShow)
    {
        if (IsShow)
        {
			PlayerPrefs.SetInt(PlayerPrefsString.ms_EventPopupShow, 1);
			m_IsEventPopupShow = true;
        }
        else
        {
			PlayerPrefs.SetInt(PlayerPrefsString.ms_EventPopupShow, 0);
			m_IsEventPopupShow = false;
		}
    }

	public void Save()
	{
		PlayerPrefs.SetString(PlayerPrefsString.ms_Login_BindType, m_eNetwork_LoginBind.ToString());
		PlayerPrefs.SetString(PlayerPrefsString.ms_Login_AccountID, m_strNetwork_LoginAccountID);
	}
}
