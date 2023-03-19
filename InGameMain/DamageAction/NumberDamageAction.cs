using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberDamageAction : MonoBehaviour
{
    private bool                    m_IsInitialize      = false;
    private SlotFixObject_Unit      m_pUnit             = null;
    private TextMeshPro             m_pTextMesh_Damage  = null;

    private Transformer_Vector3     m_pPos              = null;
    private Transformer_Scalar      m_pAlpha            = null;

    private Color                   m_pColor            = Color.white;

    void Start()
    {
 
    }

	public void Init(SlotFixObject_Unit pUnit, int nDamage, Color color)
	{
        m_IsInitialize = true;
        m_pUnit = pUnit;
        m_pColor = color;
        m_pTextMesh_Damage = gameObject.GetComponent<TextMeshPro>();
        m_pTextMesh_Damage.color = m_pColor;
        m_pTextMesh_Damage.text = nDamage.ToString();

        gameObject.transform.localScale *= InGameInfo.Instance.m_fInGameScale * 1.2f;
        Vector3 vPos = gameObject.transform.localPosition;

        m_pPos = new Transformer_Vector3(vPos);
        TransformerEvent eventValue;
        eventValue = new TransformerEvent_Vector3(0.4f, vPos + new Vector3(0, 25 * InGameInfo.Instance.m_fInGameScale, 0));
        m_pPos.AddEvent(eventValue);
        eventValue = new TransformerEvent_Vector3(0.6f, vPos + new Vector3(0, 25 * InGameInfo.Instance.m_fInGameScale, 0));
        m_pPos.AddEvent(eventValue);
        m_pPos.OnPlay();

        m_pAlpha = new Transformer_Scalar(1);
        eventValue = new TransformerEvent_Scalar(0.5f, 1);
        m_pAlpha.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(0.6f, 0);
        m_pAlpha.AddEvent(eventValue);
        m_pAlpha.SetCallback(null, OnDone_Alpha);
        m_pAlpha.OnPlay();
    }

	void Update()
    {
        if (m_IsInitialize == true)
        {
            m_pPos.Update(Time.deltaTime);
            Vector3 vPos = m_pPos.GetCurVector3();
            gameObject.transform.localPosition = vPos;

            m_pAlpha.Update(Time.deltaTime);
            float fAlpha = m_pAlpha.GetCurScalar();
            m_pColor.a = fAlpha;
            m_pTextMesh_Damage.color = m_pColor;
        }
    }

    public void OnDone_Alpha(TransformerEvent eventValue)
    {
        m_pUnit.OnNumberDamageActionDone(this);

        GameObject.Destroy(gameObject);
    }
}
