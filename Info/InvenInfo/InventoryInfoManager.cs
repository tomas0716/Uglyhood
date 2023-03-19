using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryInfoManager : Singleton<InventoryInfoManager>
{
	public CharacterInvenInfo			m_pCharacterInvenInfo				= new CharacterInvenInfo();
	public TeamInvenInfoGroup			m_pTeamInvenInfoGroup				= new TeamInvenInfoGroup();
	public ItemInvenInfo				m_pItemInvenInfo					= new ItemInvenInfo();
	public BoosterItemInvenInfoGroup	m_pBoosterItemInvenInfoGroup		= new BoosterItemInvenInfoGroup();
	public MainStageInvenInfo			m_pMainStageInvenInfo				= new MainStageInvenInfo();
	public DeckInfo						m_pDeckInfo							= new DeckInfo();
	public SummonShopInfo				m_pSummonShopInfo					= new SummonShopInfo();
	public SpecialStageInvenInfo		m_pSpecialStageInvenInfo			= new SpecialStageInvenInfo();
	public StoreInvenInfo				m_pStoreInvenInfo					= new StoreInvenInfo();
	public PostInfo						m_pPostInfo							= new PostInfo();
	public AttendanceInfo				m_pAttendanceInfo					= new AttendanceInfo();
	public OfflineRewardInfo			m_pOfflineRewardInfo				= new OfflineRewardInfo();

	// 편집을 위한 인벤토리 카피 본
	public CharacterInvenInfo			m_pEdit_CharacterInvenInfo			= new CharacterInvenInfo();
	public TeamInvenInfoGroup			m_pEdit_TeamInvenInfoGroup			= new TeamInvenInfoGroup();
	public ItemInvenInfo				m_pEdit_ItemInvenInfo				= new ItemInvenInfo();
	public BoosterItemInvenInfoGroup	m_pEdit_BoosterItemInvenInfoGroup	= new BoosterItemInvenInfoGroup();
	public MainStageInvenInfo			m_pEdit_MainStageInvenInfo			= new MainStageInvenInfo();

	private InventoryInfoManager()
	{
	}

	public void Init()
    {
		m_pItemInvenInfo.Init();
		m_pSummonShopInfo.Init();
		m_pSpecialStageInvenInfo.Init();
		m_pStoreInvenInfo.Init();
		m_pOfflineRewardInfo.Init();
		m_pPostInfo.Init();
    }

	public void OnDestroy()
    {
		m_pItemInvenInfo.OnDestroy();
		m_pSummonShopInfo.OnDestroy();
		m_pSpecialStageInvenInfo.OnDestroy();
		m_pStoreInvenInfo.OnDestroy();
		m_pOfflineRewardInfo.OnDestroy();
		m_pPostInfo.OnDestroy();
	}

	public void ClearAll()
	{
		// 다시 생성한다.
		m_pCharacterInvenInfo = new CharacterInvenInfo();
		m_pTeamInvenInfoGroup = new TeamInvenInfoGroup();
		m_pItemInvenInfo = new ItemInvenInfo();
		m_pBoosterItemInvenInfoGroup = new BoosterItemInvenInfoGroup();
		m_pMainStageInvenInfo = new MainStageInvenInfo();
		m_pDeckInfo = new DeckInfo();
		m_pSummonShopInfo = new SummonShopInfo();
		m_pSpecialStageInvenInfo = new SpecialStageInvenInfo();
		m_pStoreInvenInfo = new StoreInvenInfo();
		m_pPostInfo = new PostInfo();
		m_pAttendanceInfo = new AttendanceInfo();
		m_pOfflineRewardInfo = new OfflineRewardInfo();

		m_pEdit_CharacterInvenInfo = new CharacterInvenInfo();
		m_pEdit_TeamInvenInfoGroup = new TeamInvenInfoGroup();
		m_pEdit_ItemInvenInfo = new ItemInvenInfo();
		m_pEdit_BoosterItemInvenInfoGroup = new BoosterItemInvenInfoGroup();
		m_pEdit_MainStageInvenInfo = new MainStageInvenInfo();
	}

	public void Reset_Editing_CharacterInvenInfo()
	{
		m_pEdit_CharacterInvenInfo.ClearAll();

		int nNumData = m_pCharacterInvenInfo.GetNumInvenItem();
		for (int i = 0; i < nNumData; ++i)
		{
			CharacterInvenItemInfo pInvenItemInfo = m_pCharacterInvenInfo.GetInvenItem_byIndex(i);
			m_pEdit_CharacterInvenInfo.AddInvenItemInfo(pInvenItemInfo.Copy());
		}

		m_pEdit_TeamInvenInfoGroup.ClearAll();

		for (int i = 0; i < GameDefine.ms_nMaxTeamCount; ++i)
		{
			TeamInvenInfo pInvenInfo = m_pTeamInvenInfoGroup.GetTeamInvenInfo(i);

			if(pInvenInfo != null)
            {
				m_pEdit_TeamInvenInfoGroup.AddTeamInvenInfo(pInvenInfo.Copy(m_pEdit_CharacterInvenInfo));
			}
		}
	}

	public bool CompareTeamItemInvenInfo()
    {
		for(int i=0; i<GameDefine.ms_nMaxTeamCount; ++i)
        {
			TeamInvenInfo pInfo = m_pTeamInvenInfoGroup.GetTeamInvenInfo(i);
			TeamInvenInfo pEditInfo = m_pEdit_TeamInvenInfoGroup.GetTeamInvenInfo(i);

			for(int j=0; j<GameDefine.ms_nTeamCharacterCount; ++j)
            {
				if(pInfo != null && pEditInfo != null)
                {
					if(pInfo.GetInvenItemInfo(j) != null && pEditInfo.GetInvenItemInfo(j) != null &&
					   pInfo.GetInvenItemInfo(j).m_nUniqueID != pEditInfo.GetInvenItemInfo(j).m_nUniqueID)
                    {
						return false;
                    }

					if(pInfo.GetInvenItemInfo(j) != null && pEditInfo.GetInvenItemInfo(j) == null)
                    {
						return false;
                    }

					if(pInfo.GetInvenItemInfo(j) == null && pEditInfo.GetInvenItemInfo(j) != null)
                    {
						return false;
                    }
                }
				else if(pInfo != null && pEditInfo == null)
                {
					return false;
                }
                else if(pInfo == null && pEditInfo != null)
                {
					return false;
                }
            }
        }

		return true;
    }

	public void Reset_Editing_BoosterItemInvenInfo()
	{
		m_pEdit_BoosterItemInvenInfoGroup.ClearAll();

		for (int i = 0; i < GameDefine.ms_nMaxBoosterItemSetCount; ++i)
		{
			string strSetName = m_pBoosterItemInvenInfoGroup.GetBoosterItemSetName(i);
			m_pEdit_BoosterItemInvenInfoGroup.SetBoosterItemSetName(i, strSetName);
			for (int j = 0; j < GameDefine.ms_nMaxBoosterItemCount; ++j)
			{
				BoosterItemInvenItemInfo pInvenItemInfo = m_pBoosterItemInvenInfoGroup.GetBoosterItemInfoInfo_bySlot(i, j);

				if (pInvenItemInfo != BoosterItemInvenInfoGroup.ms_EmptyBoosterItemInvenInfo)
				{
					m_pEdit_BoosterItemInvenInfoGroup.AddBoosterItemInvenInfo(i, j, pInvenItemInfo.Copy());
				}
				else
				{
					m_pEdit_BoosterItemInvenInfoGroup.AddBoosterItemInvenInfo(i, j, pInvenItemInfo);
				}
			}
		}
	}

	public bool CompareBoosterItemInvenInfo(BoosterItemInvenInfoGroup A, BoosterItemInvenInfoGroup B)
	{
		for (int i = 0; i < GameDefine.ms_nMaxBoosterItemSetCount; ++i)
		{
			for (int j = 0; j < GameDefine.ms_nMaxBoosterItemCount; ++j)
			{
				BoosterItemInvenItemInfo pInvenItemInfo_A = A.GetBoosterItemInfoInfo_bySlot(i, j);
				BoosterItemInvenItemInfo pInvenItemInfo_B = B.GetBoosterItemInfoInfo_bySlot(i, j);

				if (pInvenItemInfo_A.m_nTableID != pInvenItemInfo_B.m_nTableID ||
					pInvenItemInfo_A.m_nItemCount != pInvenItemInfo_B.m_nItemCount)
				{
					return false;
				}
			}
		}

		for (int i = 0; i < GameDefine.ms_nMaxBoosterItemSetCount; ++i)
		{
			string strSetName_A = A.GetBoosterItemSetName(i);
			string strSetName_B = B.GetBoosterItemSetName(i);

			if (strSetName_A != strSetName_B)
			{
				return false;
			}
		}

		return true;
	}

	public void Reset_Editing_MainStageInvenInfo()
    {

    }
}
