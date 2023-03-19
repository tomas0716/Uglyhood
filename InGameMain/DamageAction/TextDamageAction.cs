using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextDamageAction : MonoBehaviour
{
    private bool                m_IsInitialize      = false;
    private TextMeshPro         m_pTextMesh_Damage  = null;

    private Transformer_Scalar  m_pAlpha            = null;

    private Color               m_pColor            = Color.white;

    void Start()
    {

    }

    public void Init(string strText, Color color)
    {
        m_IsInitialize = true;
        m_pColor = color;
        m_pTextMesh_Damage = gameObject.GetComponent<TextMeshPro>();
        m_pTextMesh_Damage.color = m_pColor;
        m_pTextMesh_Damage.text = strText;

        m_pAlpha = new Transformer_Scalar(1);
        TransformerEvent_Scalar eventValue;
        eventValue = new TransformerEvent_Scalar(1.0f, 1);
        m_pAlpha.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(1.2f, 0);
        m_pAlpha.AddEvent(eventValue);
        m_pAlpha.SetCallback(null, OnDone_Alpha);
        m_pAlpha.OnPlay();
    }

    void Update()
    {
        if (m_IsInitialize == true)
        {
            m_pAlpha.Update(Time.deltaTime);
            float fAlpha = m_pAlpha.GetCurScalar();
            m_pColor.a = fAlpha;
            m_pTextMesh_Damage.color = m_pColor;
        }
    }

    public void OnDone_Alpha(TransformerEvent eventValue)
    {
        GameObject.Destroy(gameObject);
    }
}
