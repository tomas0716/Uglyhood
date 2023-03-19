using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameInfo
{
    private static InGameInfo ms_pInstance = null;
    public static InGameInfo Instance { get { return ms_pInstance; } }

    public bool         m_IsCheckSlotMove               = false;

    public bool         m_IsInGameStart                 = false;
    public bool         m_IsInGameClick                 = false;

    public float        m_fSlotSize                     = 128;
    public float        m_fInGameScale                  = 1;
    public Vector2      m_vSlot_LeftTopPos              = Vector2.zero;
    public Vector3      m_vSlotGridCenter               = Vector3.zero;

    public int          m_nSlotStartIndex_X             = 0;
    public int          m_nSlotStartIndex_Y             = 0;
    public int          m_nSlotEndIndex_X               = 0;
    public int          m_nSlotEndIndex_Y               = 0;
    public int          m_nSlotCount_X                  = 0;
    public int          m_nSlotCount_Y                  = 0;

    public float        m_fGameArea_Width               = 0;
    public float        m_fGameArea_Height              = 0;

    public Vector3      m_vGameAreaCenter              = Vector3.zero;

    public bool         m_IsSlotLink                    = false;

    public bool         m_IsAutoPlay                    = false;

    public int          m_nClickSlotFingerID            = -1;
    public Slot         m_pClickSlot                    = null;
    public Slot         m_pSelectModeSlot               = null;

    public int          m_nClickPlayerCharacterFingerID = -1;
    public SlotFixObject_PlayerCharacter    m_pClickPlayerCharacter     = null;

    public eGameResult  m_eCurrGameResult               = eGameResult.None;

    public bool         m_IsPlayerCharacterSkillToSpeicalBlockAttack = false;

    public eTurnComponentType m_eTurnComponentType = eTurnComponentType.None;

    public List<string> m_ActionEffectHit_TargetAndCenterList = new List<string>();

    public bool m_IsChallengeMission1Success = false;
    public bool m_IsChallengeMission2Success = false;
    public bool m_IsChallengeMission3Success = false;

    public InGameInfo()
    {
        ms_pInstance = this;
    }

	public void InGameStart_Reset()
	{
		m_IsCheckSlotMove = false;
        m_IsInGameStart = false;
        m_IsInGameClick = false;

        m_nClickSlotFingerID = -1;
        m_pClickSlot = null;
        m_pSelectModeSlot = null;
        m_pClickPlayerCharacter = null;
        m_eCurrGameResult = eGameResult.None;

        m_IsSlotLink = false;
        m_IsAutoPlay = false;

        m_IsPlayerCharacterSkillToSpeicalBlockAttack = false;
        m_ActionEffectHit_TargetAndCenterList.Clear();

        m_IsChallengeMission1Success = false;
        m_IsChallengeMission2Success = false;
        m_IsChallengeMission3Success = false;
    }
}
