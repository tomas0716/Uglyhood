using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RndSlotBlock
{
    protected SlotBlock             m_pSlotBlock            = null;
    protected Plane2D               m_pPlane_Block          = null;

    protected Vector2               m_vPosition             = Vector2.zero;
    protected Vector3               m_vRotation             = Vector3.zero;
    protected bool                  m_IsVisible             = true;

    protected bool                  m_IsActiveAlpha         = false;

    protected bool                  m_IsFocus               = false;

    protected Transformer_Scalar    m_pAlpha                = new Transformer_Scalar(0);
    protected Transformer_Scalar    m_pScale                = new Transformer_Scalar(1);
    protected Transformer_Scalar    m_pRotation             = new Transformer_Scalar(0);
    
    private bool                    m_IsHighlight           = false;
    private int                     m_nHighlightDepth       = 0;

    private Transformer_Color       m_pColor                = new Transformer_Color(Color.white);
    private Color                   m_Color                 = Color.white;

    public RndSlotBlock(SlotBlock pSlotBlock, GameObject pGameObject)
    {
        m_pSlotBlock = pSlotBlock;

        TransformerEvent eventValue;

        m_pPlane_Block = pGameObject.GetComponent<Plane2D>();
        m_pPlane_Block.SetLayerName("Unit");
        pGameObject.transform.position = new Vector3(10000, 10000, 0);

        if (pSlotBlock.GetSpecialItem() != eSpecialItem.None)
        {
            m_IsActiveAlpha = true;
            eventValue = new TransformerEvent_Scalar(0.5f, 0);
            m_pAlpha.AddEvent(eventValue);
            eventValue = new TransformerEvent_Scalar(0.6f, 1);
            m_pAlpha.AddEvent(eventValue);
            m_pAlpha.SetCallback(null, OnDone_Alpha);
            m_pAlpha.OnPlay();
        }

        Update(0);
    }

    public virtual void     OnDestroy()
    {
        if (m_pPlane_Block != null)
        {
            GameObject.Destroy(m_pPlane_Block.gameObject);
            m_pPlane_Block = null;
        }
    }

    public virtual void     Update(float deltaTime)
    {
        m_pColor.Update(deltaTime);
        m_Color = m_pColor.GetCurColor();
        m_pPlane_Block.SetColor(m_Color);

        if (m_IsActiveAlpha == true)
        {
            m_pAlpha.Update(deltaTime);
            float fAlpha = m_pAlpha.GetCurScalar();

            if (m_pPlane_Block != null)
            {
                m_Color.a = fAlpha;
                m_pPlane_Block.SetColor(m_Color);
            }
        }

        m_pScale.Update(deltaTime);
        float fScale = m_pScale.GetCurScalar();

        if (m_pPlane_Block != null)
        {
            float localScale = InGameInfo.Instance.m_fInGameScale * fScale;
            m_pPlane_Block.transform.localScale = new Vector3(localScale, localScale, localScale);
        }

        m_pRotation.Update(deltaTime);
        float fRotation = m_pRotation.GetCurScalar();

        if (m_pPlane_Block != null)
        {
            m_pPlane_Block.transform.eulerAngles = new Vector3(0, 0, fRotation);
        }
    }

    private void            OnDone_Alpha(TransformerEvent eventValue)
    {
        m_IsActiveAlpha = false;
    }

    public void             SetChangeShuffle()
    {
        m_pAlpha = new Transformer_Scalar(0);
    }

    public void             OnCheckShffleCreate()
    {
        TransformerEvent_Scalar eventValue;

        m_pScale = new Transformer_Scalar(0);
        eventValue = new TransformerEvent_Scalar(0.5f, 0);
        m_pScale.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(0.8f, 1);
        m_pScale.AddEvent(eventValue);
        m_pScale.OnPlay();

        m_pRotation = new Transformer_Scalar(0);
        eventValue = new TransformerEvent_Scalar(0.5f, 0);
        m_pRotation.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(0.8f, 720);
        m_pRotation.AddEvent(eventValue);
        m_pRotation.OnPlay();
    }

    public void             OnChangeShuffleRemove()
    {
        TransformerEvent_Scalar eventValue;

        m_pScale = new Transformer_Scalar(1);
        eventValue = new TransformerEvent_Scalar(0.3f, 0);
        m_pScale.AddEvent(eventValue);
        m_pScale.OnPlay();

        m_pRotation = new Transformer_Scalar(0);
        eventValue = new TransformerEvent_Scalar(0.3f, -720);
        m_pRotation.AddEvent(eventValue);
        m_pRotation.OnPlay();
    }

    public virtual void     OnLinkMove(float fMoveTime)
    {
        m_IsActiveAlpha = true;

        m_pAlpha = new Transformer_Scalar(1);

        TransformerEvent_Scalar eventValue;
        eventValue = new TransformerEvent_Scalar(fMoveTime * 0.4f, 0);
        m_pAlpha.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(fMoveTime * 0.6f, 0);
        m_pAlpha.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(fMoveTime, 1.0f);
        m_pAlpha.AddEvent(eventValue);
        m_pAlpha.SetCallback(null, OnDone_Alpha);
        m_pAlpha.OnPlay();
    }

    public virtual void     SetPosition(Vector2 vPos)
    {
        m_vPosition = vPos;

        if (m_pPlane_Block != null)
        {
            Vector3 vPlanePos = m_pPlane_Block.transform.position;
            vPlanePos.x = m_vPosition.x;
            vPlanePos.y = m_vPosition.y;
            vPlanePos.z = m_IsHighlight  == false ? 0 : - (float)(ePlaneOrder.Highlight_Content + m_nHighlightDepth);
            m_pPlane_Block.transform.position = vPlanePos;
        }
    }

    public virtual Vector2  GetPosition()
    {
        return m_vPosition;
    }

    public virtual void     SetRotation(Vector3 vRot)
    {
        m_vRotation = vRot;

        if (m_pPlane_Block != null)
        {
            m_pPlane_Block.transform.eulerAngles = vRot;
        }
    }

    public virtual Vector3  GetRotation()
    {
        return m_vRotation;
    }

    public Plane2D GetPlane_Block()
    {
        return m_pPlane_Block;
    }

    public virtual void SetColor(Color color, float fStartDelay, float fTransTime)
    {
        m_pColor.OnReset();

        TransformerEvent_Color eventValue;
        eventValue = new TransformerEvent_Color(0, m_Color);
        m_pColor.AddEvent(eventValue);
        eventValue = new TransformerEvent_Color(fStartDelay, m_Color);
        m_pColor.AddEvent(eventValue);
        eventValue = new TransformerEvent_Color(fStartDelay + fTransTime, color);
        m_pColor.AddEvent(eventValue);
        m_pColor.OnPlay();
    }

    public virtual void     SetVisible(bool IsVisible)
    {
        m_IsVisible = IsVisible;
        m_pPlane_Block.gameObject.SetActive(IsVisible);
    }

    public virtual bool     IsVisible()
    {
        return m_IsVisible; 
    }

    public void OnSwapPossibleDirection()
    {
        m_pScale.OnReset();
        TransformerEvent_Scalar eventValue;
        eventValue = new TransformerEvent_Scalar(0, 1);
        m_pScale.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(0.18f, 1.15f);
        m_pScale.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(0.36f, 1);
        m_pScale.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(0.54f, 1.15f);
        m_pScale.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(0.72f, 1);
        m_pScale.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(2.0f, 1);
        m_pScale.AddEvent(eventValue);
        m_pScale.SetCallback(null, OnDone_Scale_SwapPossibleDirection);
        m_pScale.OnPlay();
    }

    private void OnDone_Scale_SwapPossibleDirection(TransformerEvent eventValue)
    {
        m_pScale = new Transformer_Scalar(1);
    }

    public void OnCancelSwapPossibleDirection()
    {
        m_pScale = new Transformer_Scalar(1);
    }

    public void SetHighlight(int nDepth = 0)
    {
        m_IsHighlight = true;
        m_nHighlightDepth = nDepth;
        Vector3 vPos = m_pPlane_Block.gameObject.transform.position;
        vPos.z = -(float)(ePlaneOrder.Highlight_Content + nDepth);
        m_pPlane_Block.gameObject.transform.position = vPos;
    }

    public void ClearHighlight()
    {
        m_IsHighlight = false;
        m_nHighlightDepth = 0;
        Vector3 vPos = m_pPlane_Block.gameObject.transform.position;
        vPos.z = 0;
        m_pPlane_Block.gameObject.transform.position = vPos;
    }
}
