using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_InGame_Effect_Stripe : GameEvent
{
    private eSpecialItem            m_eSpecialItem      = eSpecialItem.Hor;
    private Vector2                 m_vPos              = Vector2.zero;

    private Plane2D []              m_pPlane_Stripe_01  = new Plane2D[2];
    private Plane2D                 m_pPlane_Stripe_02  = null;

    private Transformer_Scalar      m_pPos_Stripe_01    = new Transformer_Scalar(0);
    private Transformer_Scalar      m_pScale_Stripe_01  = new Transformer_Scalar(0);
    private Transformer_Scalar      m_pAlpha_Stripe_01  = new Transformer_Scalar(0.75f);

    private Transformer_Scalar      m_pScale_Stripe_02  = new Transformer_Scalar(0);
    private Transformer_Scalar      m_pAlpha_Stripe_02  = new Transformer_Scalar(1);

    public GameEvent_InGame_Effect_Stripe(Slot pSlot, eSpecialItem eSpecialItem)
    {
        Init(pSlot.GetPosition(), eSpecialItem);
    }

    public GameEvent_InGame_Effect_Stripe(Vector2 vPos, eSpecialItem eSpecialItem)
    {
        Init(vPos, eSpecialItem);
    }

    private void Init(Vector2 vPos, eSpecialItem eSpecialItem)
    {
        m_vPos = vPos;
        m_eSpecialItem = eSpecialItem;

        GameObject ob;
        GameObject ob_01;

        ob = Resources.Load<GameObject>("2D/Prefabs/FX/Plane2D_FX_Stripte_01");
        ob_01 = GameObject.Instantiate(ob);
        m_pPlane_Stripe_01[0] = ob_01.GetComponent<Plane2D>();
        ob_01 = GameObject.Instantiate(ob);
        m_pPlane_Stripe_01[1] = ob_01.GetComponent<Plane2D>();

        ob = Resources.Load<GameObject>("2D/Prefabs/FX/Plane2D_FX_Stripte_02");
        ob_01 = GameObject.Instantiate(ob);
        m_pPlane_Stripe_02 = ob_01.GetComponent<Plane2D>();
        m_pPlane_Stripe_02.SetPosition(m_vPos);

        TransformerEvent eventValue;

        eventValue = new TransformerEvent_Scalar(GameDefine.ms_fSpecialSlotStraight * 6, 0);
        m_pPos_Stripe_01.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(GameDefine.ms_fSpecialSlotStraight * 12, InGameInfo.Instance.m_fSlotSize * 6);
        m_pPos_Stripe_01.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(GameDefine.ms_fSpecialSlotStraight * 16, InGameInfo.Instance.m_fSlotSize * 20);
        m_pPos_Stripe_01.AddEvent(eventValue);
        m_pPos_Stripe_01.OnPlay();

        eventValue = new TransformerEvent_Scalar(0, 0);
        m_pScale_Stripe_01.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(GameDefine.ms_fSpecialSlotStraight * 8, 6);
        m_pScale_Stripe_01.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(GameDefine.ms_fSpecialSlotStraight * 16, 20);
        m_pScale_Stripe_01.AddEvent(eventValue);
        m_pScale_Stripe_01.OnPlay();

        eventValue = new TransformerEvent_Scalar(GameDefine.ms_fSpecialSlotStraight * 14, 0.75f);
        m_pAlpha_Stripe_01.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(GameDefine.ms_fSpecialSlotStraight * 16, 0);
        m_pAlpha_Stripe_01.AddEvent(eventValue);
        m_pAlpha_Stripe_01.SetCallback(null, OnDone_Alpha_Stripe_01);
        m_pAlpha_Stripe_01.OnPlay();

        switch (m_eSpecialItem)
        {
            case eSpecialItem.Ver:
                {
                    m_pPlane_Stripe_01[0].SetRotation(new Vector3(0, 0, 90));
                    m_pPlane_Stripe_01[1].SetRotation(new Vector3(0, 0, 90));
                    m_pPlane_Stripe_02.SetRotation(new Vector3(0, 0, 90));
                }
                break;
        }

        eventValue = new TransformerEvent_Scalar(0, 0);
        m_pScale_Stripe_02.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(GameDefine.ms_fSpecialSlotStraight * 16, 6);
        m_pScale_Stripe_02.AddEvent(eventValue);
        m_pScale_Stripe_02.OnPlay();

        eventValue = new TransformerEvent_Scalar(GameDefine.ms_fSpecialSlotStraight * 12, 0.55f);
        m_pAlpha_Stripe_02.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(GameDefine.ms_fSpecialSlotStraight * 16, 0);
        m_pAlpha_Stripe_02.AddEvent(eventValue);
        m_pAlpha_Stripe_02.OnPlay();

        Update();
    }

    public override void OnDestroy()
    {
        GameObject.Destroy(m_pPlane_Stripe_01[0].gameObject);
        GameObject.Destroy(m_pPlane_Stripe_01[1].gameObject);
        GameObject.Destroy(m_pPlane_Stripe_02.gameObject);
    }

    public override void Update()
    {
        m_pPos_Stripe_01.Update(Time.deltaTime);
        float fPos_Stripe_01 = m_pPos_Stripe_01.GetCurScalar();

        m_pScale_Stripe_01.Update(Time.deltaTime);
        float fScale_Stripe_01 = m_pScale_Stripe_01.GetCurScalar();

        m_pAlpha_Stripe_01.Update(Time.deltaTime);
        float fAlpha_Stripe_01 = m_pAlpha_Stripe_01.GetCurScalar();

        switch (m_eSpecialItem)
        {
            case eSpecialItem.Hor:
                {
                    m_pPlane_Stripe_01[0].SetPosition(m_vPos + new Vector2(fPos_Stripe_01, 0));
                    m_pPlane_Stripe_01[1].SetPosition(m_vPos + new Vector2(-fPos_Stripe_01, 0));
                }
                break;

            case eSpecialItem.Ver:
                {
                    m_pPlane_Stripe_01[0].SetPosition(m_vPos + new Vector2(0, fPos_Stripe_01));
                    m_pPlane_Stripe_01[1].SetPosition(m_vPos + new Vector2(0, -fPos_Stripe_01));
                }
                break;
        }

        m_pPlane_Stripe_01[0].SetScale(new Vector3(fScale_Stripe_01 * InGameInfo.Instance.m_fInGameScale, 1.2f * InGameInfo.Instance.m_fInGameScale, 1));
        m_pPlane_Stripe_01[1].SetScale(new Vector3(fScale_Stripe_01 * InGameInfo.Instance.m_fInGameScale, 1.2f * InGameInfo.Instance.m_fInGameScale, 1));

        m_pPlane_Stripe_01[0].SetColor(new Color(1, 1, 1, fAlpha_Stripe_01));
        m_pPlane_Stripe_01[1].SetColor(new Color(1, 1, 1, fAlpha_Stripe_01));

        m_pScale_Stripe_02.Update(Time.deltaTime);
        float fScale_Stripe_02 = m_pScale_Stripe_02.GetCurScalar();

        m_pAlpha_Stripe_02.Update(Time.deltaTime);
        float fAlpha_Stripe_02 = m_pAlpha_Stripe_02.GetCurScalar();

        m_pPlane_Stripe_02.SetScale(new Vector3(fScale_Stripe_02 * InGameInfo.Instance.m_fInGameScale, 1.2f * InGameInfo.Instance.m_fInGameScale, 1));
        m_pPlane_Stripe_02.SetColor(new Color(1, 1, 1, fAlpha_Stripe_02));
    }

    public void OnDone_Alpha_Stripe_01(TransformerEvent EventValue)
    {
        OnDone();
    }
}
