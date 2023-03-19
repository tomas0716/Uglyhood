using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame_Highlight : MonoBehaviour
{
    private ePlayerCharacterHighlight   m_eHighlight                = ePlayerCharacterHighlight.SelectRange;
    private Slot                        m_pSlot                     = null;
    private DamageTargetInfo            m_pDamageTargetInfo         = null;

    private bool                        m_IsSubSkillStatusEffect    = false;

    public void Initialize(ePlayerCharacterHighlight eHighlight, Slot pSlot, DamageTargetInfo pDamageTargetInfo)
    {
        m_eHighlight = eHighlight;
        m_pSlot = pSlot;
        m_pDamageTargetInfo = pDamageTargetInfo;
        //m_pAction_DamageTargetRangeInfo = pDamageTargetRangeInfo; 

        gameObject.transform.position = m_pSlot.GetPosition();

        GameObject ob_SelectRange, ob_SelectTarget, ob_DamageTarget;

        ob_SelectRange = Helper.FindChildGameObject(gameObject, "Highlight_SelectRangeHighlight");
        ob_SelectTarget = Helper.FindChildGameObject(gameObject, "Highlight_SelectTargetMarker");
        ob_DamageTarget = Helper.FindChildGameObject(gameObject, "Highlight_DamageRangeHighlight");

        switch (eHighlight)
        {
            case ePlayerCharacterHighlight.SelectRange:
                {
                    ob_SelectTarget.SetActive(false);
                    ob_DamageTarget.SetActive(false);
                }
                break;

            case ePlayerCharacterHighlight.SelectTarget:
                {
                    ob_SelectRange.SetActive(false);
                    ob_DamageTarget.SetActive(false);
                }
                break;

            case ePlayerCharacterHighlight.DamageTarget:
                {
                    ob_SelectRange.SetActive(false);
                    ob_SelectTarget.SetActive(false);
                }
                break;
        }
    }

    public void SetSubSkillStatusEffect(bool IsSubSkill)
    {
        m_IsSubSkillStatusEffect = IsSubSkill;

        if (m_pDamageTargetInfo != null)
        {
            m_pDamageTargetInfo.m_IsSubSkillStatusEffect = IsSubSkill;
        }
    }

    public bool IsSubSkillStatusEffect()
    {
        return m_IsSubSkillStatusEffect;
    }

	public ePlayerCharacterHighlight GetHighlight()
	{
        return m_eHighlight;
    }

    public Slot GetSlot()
    {
        return m_pSlot;
    }

    public DamageTargetInfo GetAction_DamageTargetInfo()
    {
        return m_pDamageTargetInfo;
    }
}
