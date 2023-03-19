using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotBlock
{
    private SlotManager             m_pSlotManager                  = null;
    private Slot                    m_pSlot                         = null;

    private GameObject              m_pGameObject_SlotBlock         = null;

    private eBlockType              m_eBlockType                    = eBlockType.Block_Start;
    private eSpecialItem            m_eSpecialItem                  = eSpecialItem.None;
    private SpecialItemCustomize    m_pSpecialItemCustomize         = null;

    private RndSlotBlock            m_pRndSlotBlock                 = null;

    private Vector2                 m_vPos                          = Vector2.zero;
    private Transformer_Vector2     m_pPos                          = new Transformer_Vector2(new Vector2(10000, 10000));
    private bool                    m_IsMoveFlag                    = false;
    private bool                    m_IsMoving                      = false;

    private object                  m_pParameta                     = null;

    //private InGameStageMapSlotData  m_pInGameStageMapSlotData       = null;
    private eMapSlotItem            m_eMapSlotItem                  = eMapSlotItem.None;
    private ParticleInfo            m_pParticleInfo_SpecialBlock    = null;

    public SlotBlock(SlotManager pSlotManager, Slot pSlot, eBlockType eType, eSpecialItem eSpeiclalItem, eMapSlotItem eSlotItem = eMapSlotItem.None)
    {
        m_pSlotManager = pSlotManager;
        m_pSlot = pSlot;
        m_eBlockType = eType;
        m_eSpecialItem = eSpeiclalItem;
        m_eMapSlotItem = eSlotItem;

        m_pGameObject_SlotBlock = new GameObject("SlotBlock");
        m_pGameObject_SlotBlock.transform.SetParent(pSlot.gameObject.transform);
        m_pGameObject_SlotBlock.transform.position = Vector3.zero;

        CreateRndSlotBlock();
    }

    public void OnDestroy()
    {
        if (m_pRndSlotBlock != null)
        {
            m_pRndSlotBlock.OnDestroy();
            m_pRndSlotBlock = null;
        }

        if (m_pGameObject_SlotBlock != null)
        {
            GameObject.Destroy(m_pGameObject_SlotBlock);
            m_pGameObject_SlotBlock = null;
        }

        if (m_pParticleInfo_SpecialBlock != null)
        {
            ParticleManager.Instance.RemoveParticleInfo(m_pParticleInfo_SpecialBlock);
            m_pParticleInfo_SpecialBlock = null;
        }
    }

    public void Update(float deltaTime)
    {
        m_pPos.Update(deltaTime);
        m_vPos = m_pPos.GetCurVector2();

        if (m_pRndSlotBlock != null)
        {
            m_pRndSlotBlock.SetPosition(m_vPos);
            m_pRndSlotBlock.Update(deltaTime);
        }
    }

    public void SetSlot(Slot pSlot)
    {
        if (m_pGameObject_SlotBlock == null)
        {
            OutputLog.Log("SlotBlock SetSlot : m_pGameObject_SlotBlock == null");
        }

        if (pSlot == null)
        {
            OutputLog.Log("SlotBlock SetSlot : pSlot == null");
        }

        if (pSlot.gameObject == null)
        {
            OutputLog.Log("SlotBlock SetSlot : pSlot.gameObject == null");
        }

        if (m_pGameObject_SlotBlock != null && pSlot != null && pSlot.gameObject != null)
        {
            m_pSlot = pSlot;
            m_pGameObject_SlotBlock.transform.SetParent(pSlot.gameObject.transform);
            m_pGameObject_SlotBlock.transform.position = Vector3.zero;
        }
        else
        {
        }
    }

    public void ChangeSlotBlockGameObjectName(string strName)
    {
        if(m_pGameObject_SlotBlock != null)
            m_pGameObject_SlotBlock.name = strName;
    }

    public void SetTempSlot(Slot pSlot)
    {
        m_pSlot = pSlot;
    }

    public Slot GetSlot()
    {
        return m_pSlot;
    }

    public void ChangeOnlySpecialitemAttribute(eSpecialItem eSpecialItem)
    {
        m_eSpecialItem = eSpecialItem;
    }

    public void ChangeBlockSpecialItem(GameObject pGameObject, eSpecialItem eSpecialItem, SpecialItemCustomize pSpecialItemCustomize)
    {
        if (m_pRndSlotBlock != null)
        {
            m_pRndSlotBlock.OnDestroy();
            m_pRndSlotBlock = null;
        }

        m_eSpecialItem = eSpecialItem;
        m_pSpecialItemCustomize = pSpecialItemCustomize;

        GameObject pNewGameObject = GameObject.Instantiate(pGameObject);
        pNewGameObject.transform.SetParent(m_pGameObject_SlotBlock.transform);
        pNewGameObject.transform.localPosition = Vector3.zero;

        m_pRndSlotBlock = new RndSlotBlock(this, pNewGameObject);

		switch (eSpecialItem)
		{
			case eSpecialItem.Plus_B5:
			case eSpecialItem.Square3:
				{
					eElement eElem = (eElement)m_eBlockType;
					m_pParticleInfo_SpecialBlock = ParticleManager.Instance.LoadParticleSystem("FX_ElementalBomb_" + eElem.ToString() + "_Idle", new Vector3(0, 0, 3));
					m_pParticleInfo_SpecialBlock.SetScale(InGameInfo.Instance.m_fInGameScale);
					m_pParticleInfo_SpecialBlock.GetGameObject().transform.SetParent(pNewGameObject.transform);
                    m_pParticleInfo_SpecialBlock.GetGameObject().transform.localPosition = new Vector3(0, 0, -2);

                }
				break;

			case eSpecialItem.Match_Color:
				{
					eElement eElem = (eElement)m_eBlockType;
					m_pParticleInfo_SpecialBlock = ParticleManager.Instance.LoadParticleSystem("FX_ElementalElixir_" + eElem.ToString() + "_Idle", new Vector3(0, 0, 3));
					m_pParticleInfo_SpecialBlock.SetScale(InGameInfo.Instance.m_fInGameScale);
					m_pParticleInfo_SpecialBlock.GetGameObject().transform.SetParent(pNewGameObject.transform);
                    m_pParticleInfo_SpecialBlock.GetGameObject().transform.localPosition = new Vector3(0, 0, -2);
                }
				break;
		}
	}

    private void CreateRndSlotBlock()
    {
        try
        {
            if (m_eMapSlotItem == eMapSlotItem.None)
            {
                ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);

                CombinationShapeData pShapeData;

                if (m_eSpecialItem == eSpecialItem.None)
                    pShapeData = InGameSetting.Instance.m_pInGameSettingData.GetCombinationShapeData_Base();
                else
                    pShapeData = InGameSetting.Instance.m_pInGameSettingData.GetCombinationShapeData(m_eSpecialItem);

                if (pShapeData == null)
                {
                    pShapeData = InGameSetting.Instance.m_pInGameSettingData.GetCombinationShapeData(eSpecialItem.None);

                    if (pShapeData != null)
                    {
                        m_eSpecialItem = eSpecialItem.None;

                        if (pStageInfo == null || pStageInfo.m_strStageTheme == "Basic")
                        {
                            ChangeBlockSpecialItem(pShapeData.m_pGameObject[(int)m_eBlockType], m_eSpecialItem, pShapeData.m_pSpecialItemCustomize);
                        }
                        else
                        {
                            string [] strTheme = { "Water", "Fire", "Wind", "Light", "Dark" };

                            if (m_eBlockType >= eBlockType.Block_Start && m_eBlockType < eBlockType.Block_Start + InGameSetting.Instance.m_pInGameSettingData.m_nBlockCount)
                            {
                                ChangeBlockSpecialItem(pShapeData.m_pGameObject[(int)m_eBlockType], m_eSpecialItem, pShapeData.m_pSpecialItemCustomize);
                                Plane2D pPlane2D = m_pRndSlotBlock.GetPlane_Block();
                                Helper.Change_Plane2D_AtlasInfo(pPlane2D, "Block_" + pStageInfo.m_strStageTheme, "Block_" + strTheme[m_eBlockType - eBlockType.Block_Start]);
                            }
                            else
                            {
                                ChangeBlockSpecialItem(pShapeData.m_pGameObject[(int)m_eBlockType], m_eSpecialItem, pShapeData.m_pSpecialItemCustomize);
                            }
                        }
                    }
                }
                else
                {
                    if (m_eBlockType != eBlockType.Custom)
                    {
                        if (pStageInfo == null || pStageInfo.m_strStageTheme == "Basic")
                        {
                            ChangeBlockSpecialItem(pShapeData.m_pGameObject[(int)m_eBlockType], m_eSpecialItem, pShapeData.m_pSpecialItemCustomize);
                        }
                        else
                        {
                            string[] strTheme = { "Water", "Fire", "Wind", "Light", "Dark" };

                            if (m_eBlockType >= eBlockType.Block_Start && m_eBlockType < eBlockType.Block_Start + InGameSetting.Instance.m_pInGameSettingData.m_nBlockCount && m_eSpecialItem == eSpecialItem.None)
                            {
                                ChangeBlockSpecialItem(pShapeData.m_pGameObject[(int)m_eBlockType], m_eSpecialItem, pShapeData.m_pSpecialItemCustomize);
                                Plane2D pPlane2D = m_pRndSlotBlock.GetPlane_Block();
                                Helper.Change_Plane2D_AtlasInfo(pPlane2D, "Block_" + pStageInfo.m_strStageTheme, "Block_" + strTheme[m_eBlockType - eBlockType.Block_Start]);
                            }
                            else
                            {
                                ChangeBlockSpecialItem(pShapeData.m_pGameObject[(int)m_eBlockType], m_eSpecialItem, pShapeData.m_pSpecialItemCustomize);
                            }
                        }
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                m_eSpecialItem = eSpecialItem.None;
                GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Obstacle_Block/" + m_eMapSlotItem);
                ChangeBlockSpecialItem(ob, m_eSpecialItem, null);
            }
        }
        catch (Exception e)
        {
        }
    }

    public void ChangeBlockType(eBlockType eType)
    {
        m_eBlockType = eType;
    }

    public RndSlotBlock GetRndSlotBlock()
    {
        return m_pRndSlotBlock;
    }

    public GameObject GetGameObject()
    {
        return m_pGameObject_SlotBlock;
    }

    public void SetPosition(Vector2 vPos)
    {
        m_vPos = vPos;

        m_pPos.OnReset();
        TransformerEvent eventValue;
        eventValue = new TransformerEvent_Vector2(0, vPos);
        m_pPos.AddEvent(eventValue);
        m_pPos.OnPlay();
    }

    public void SetColor(Color color, float fStartDelay, float fTransTime)
    {
        if (m_pRndSlotBlock != null)
        {
            m_pRndSlotBlock.SetColor(color, fStartDelay, fTransTime);
        }
    }

    public void SetChangeShuffle()
    {
        m_pRndSlotBlock.SetChangeShuffle();
    }

    public void OnCheckShffleCreate()
    {
        m_pRndSlotBlock.OnCheckShffleCreate();
    }

    public void OnChangeShuffleRemove()
    {
        m_pRndSlotBlock.OnChangeShuffleRemove();
    }

    public void OnCreate(Vector2 vPos, float fMoveTime, int nInterval, bool IsInverse)
    {
        if (fMoveTime != 0)
        {
            m_IsMoveFlag = true;
            m_IsMoving = true;

            float fInterval_X = 0;
            float fInterval_Y = 0;

            if (IsInverse == false)
            {
                switch (InGameSetting.Instance.m_pInGameSettingData.m_eBlockFillDirection)
                {
                    case eBlockFillDirection.UpToBottom: fInterval_Y = InGameInfo.Instance.m_fSlotSize * nInterval; break;
                    case eBlockFillDirection.BottomToUp: fInterval_Y = -InGameInfo.Instance.m_fSlotSize * nInterval; break;
                    case eBlockFillDirection.LeftToRight: fInterval_X = -InGameInfo.Instance.m_fSlotSize * nInterval; break;
                    case eBlockFillDirection.RightToLeft: fInterval_X = InGameInfo.Instance.m_fSlotSize * nInterval; break;
                }
            }
            else
            {
                switch (InGameSetting.Instance.m_pInGameSettingData.m_eBlockFillDirection)
                {
                    case eBlockFillDirection.UpToBottom: fInterval_Y = -InGameInfo.Instance.m_fSlotSize * nInterval; break;
                    case eBlockFillDirection.BottomToUp: fInterval_Y = InGameInfo.Instance.m_fSlotSize * nInterval; break;
                    case eBlockFillDirection.LeftToRight: fInterval_X = InGameInfo.Instance.m_fSlotSize * nInterval; break;
                    case eBlockFillDirection.RightToLeft: fInterval_X = -InGameInfo.Instance.m_fSlotSize * nInterval; break;
                }
            }

            Vector2 vStartPos = new Vector2(vPos.x + fInterval_X, vPos.y + fInterval_Y);
            m_pPos = new Transformer_Vector2(vStartPos);
            TransformerEvent eventValue;
            eventValue = new TransformerEvent_Vector2(fMoveTime, vPos);
            m_pPos.AddEvent(eventValue);
            m_pPos.SetCallback(null, OnDone_Move);
            m_pPos.OnPlay();
        }
        else
        {
            m_pPos = new Transformer_Vector2(vPos);
        }
    }

    public void OnMove(Slot pFromSlot, Slot pToSlot, float fMoveTime)
    {
        try
        {
            m_vPos = m_pPos.GetCurVector2();

            m_IsMoveFlag = true;
            m_IsMoving = true;
            SetSlot(pToSlot);
            m_pSlot = pToSlot;
            m_pGameObject_SlotBlock.transform.SetParent(m_pSlot.gameObject.transform);

            m_pPos = new Transformer_Vector2(m_vPos);
            TransformerEvent eventValue;
            eventValue = new TransformerEvent_Vector2(fMoveTime, pToSlot.GetPosition());
            m_pPos.AddEvent(eventValue);
            m_pPos.SetCallback(null, OnDone_Move);
            m_pPos.OnPlay();
        }
        catch (Exception e)
        {
            Debug.Log("OnMove exception " + e);
        }
    }

    public void OnLinkMove(Slot pFromSlot, Slot pToSlot, float fMoveTime, bool IsReverse)
    {
        try
        {
            float fLinkSlotWarpInterval = GameDefine.ms_fLinkSlotWarpInterval;
            if(IsReverse == true) fLinkSlotWarpInterval = -GameDefine.ms_fLinkSlotWarpInterval;

            m_IsMoveFlag = true;
            m_IsMoving = true;
            SetSlot(pToSlot);
            m_pSlot = pToSlot;
            m_pGameObject_SlotBlock.transform.SetParent(m_pSlot.gameObject.transform);

            m_pPos = new Transformer_Vector2(m_vPos);
            TransformerEvent eventValue;
            eventValue = new TransformerEvent_Vector2(fMoveTime * 0.5f, pFromSlot.GetPosition() + new Vector2(0, fLinkSlotWarpInterval));
            m_pPos.AddEvent(eventValue);
            eventValue = new TransformerEvent_Vector2(fMoveTime * 0.5f, pToSlot.GetPosition() + new Vector2(0, -fLinkSlotWarpInterval));
            m_pPos.AddEvent(eventValue);
            eventValue = new TransformerEvent_Vector2(fMoveTime, pToSlot.GetPosition());
            m_pPos.AddEvent(eventValue);
            m_pPos.SetCallback(null, OnDone_Move);
            m_pPos.OnPlay();

            if (m_pRndSlotBlock != null)
            {
                m_pRndSlotBlock.OnLinkMove(fMoveTime);
            }
        }
        catch (Exception e)
        {
            OutputLog.Log("OnMove exception " + e);
        }
    }

    public void OnWarp(Slot pFromSlot, Slot pToSlot, float fMoveTime)
    {
        if (m_pGameObject_SlotBlock == null || m_pSlot == null || m_pSlot.gameObject == null)
        {
            return;
        }

        m_vPos = m_pPos.GetCurVector2();

        m_IsMoveFlag = true;
        m_IsMoving = true;
        m_pSlot = pToSlot;

        m_pGameObject_SlotBlock.transform.SetParent(m_pSlot.gameObject.transform);

        m_pPos = new Transformer_Vector2(m_vPos);
        TransformerEvent eventValue;
        eventValue = new TransformerEvent_Vector2(fMoveTime * 0.5f, m_vPos);
        m_pPos.AddEvent(eventValue);
        eventValue = new TransformerEvent_Vector2(fMoveTime * 0.5f, pToSlot.GetPosition());
        m_pPos.AddEvent(eventValue);
        eventValue = new TransformerEvent_Vector2(fMoveTime, pToSlot.GetPosition());
        m_pPos.AddEvent(eventValue);
        m_pPos.SetCallback(null, OnDone_Move);
        m_pPos.OnPlay();
    }

    public void OnInPossibleSlotMoveAction(Slot pFromSlot, Slot pToSlot, float fMoveTime)
    {
        float fDistance = Vector3.Distance(m_vPos, pToSlot.GetPosition());
        Vector2 vDirection = pToSlot.GetPosition() - m_vPos;
        vDirection.Normalize();

        m_pPos.OnResetCallback();
        m_vPos = m_pPos.GetCurVector2();
        m_pPos = new Transformer_Vector2(m_vPos);
        TransformerEvent eventValue;
        eventValue = new TransformerEvent_Vector2(fMoveTime * 0.5f, m_vPos + (vDirection * fDistance * 0.08f));
        m_pPos.AddEvent(eventValue);
        eventValue = new TransformerEvent_Vector2(fMoveTime, m_vPos);
        m_pPos.AddEvent(eventValue);
        m_pPos.OnPlay();
    }

    public void OnInPossibleSlotMoveAction_ForLast(float fMoveTime)
    {
    }

    private void OnDone_Move(TransformerEvent eventValue)
    {
        m_IsMoving = false;
        m_pSlot.OnDone_Move();
    }

    public void SetMoveFlag(bool IsMoveFlag)
    {
        m_IsMoveFlag = IsMoveFlag;
    }

    public bool IsMoveFlag()
    {
        return m_IsMoveFlag;
    }

    public void SetMoving(bool IsMoving)
    {
        m_IsMoving = IsMoving;
    }

    public bool IsMoving()
    {
        if (InGameSetting.Instance.m_pInGameSettingData.m_IsAnytimeSwap == true)
            return false;

        return m_IsMoving;
    }

    public void OnMoveComplete()
    {
        m_IsMoving = false;

        float fInterval_X = 0;
        float fInterval_Y = 0;

        switch (InGameSetting.Instance.m_pInGameSettingData.m_eBlockFillDirection)
        {
            case eBlockFillDirection.UpToBottom: fInterval_Y = -InGameInfo.Instance.m_fSlotSize * 0.09f; break;
            case eBlockFillDirection.BottomToUp: fInterval_Y = InGameInfo.Instance.m_fSlotSize * 0.09f; break;
            case eBlockFillDirection.LeftToRight: fInterval_X = -InGameInfo.Instance.m_fSlotSize * 0.09f; break;
            case eBlockFillDirection.RightToLeft: fInterval_X = InGameInfo.Instance.m_fSlotSize * 0.09f; break;
        }

        m_pPos.OnReset();
        TransformerEvent eventValue;
        Vector2 vPos = m_pSlot.GetPosition();
        m_pPos = new Transformer_Vector2(vPos);
        eventValue = new TransformerEvent_Vector2(0.1f, new Vector2(vPos.x + fInterval_X, vPos.y + fInterval_Y));
        m_pPos.AddEvent(eventValue);
        eventValue = new TransformerEvent_Vector2(0.2f, vPos);
        m_pPos.AddEvent(eventValue);
        m_pPos.OnPlay();
    }

    public void PossibleBlockSwapAction(Slot pSlot)
    {
        Vector2 vTargetPos = pSlot.GetPosition();
        m_pPos = new Transformer_Vector2(m_vPos);

        TransformerEvent eventValue;
        eventValue = new TransformerEvent_Vector2(GameDefine.ms_fBlockChnageSlotTime, vTargetPos);
        m_pPos.AddEvent(eventValue);
        m_pPos.SetCallback(null, OnDone_PossibleBlockSwapAction);
        m_pPos.OnPlay();
    }

    private void OnDone_PossibleBlockSwapAction(TransformerEvent eventValue)
    {
        m_pSlot.OnDone_PossibleBlockSwapAction();
    }

    public void InPossibleBlockSwapAction(Slot pSlot)
    {
        Vector2 vTargetPos = pSlot.GetPosition();
        m_pPos = new Transformer_Vector2(m_vPos);

        TransformerEvent eventValue;
        eventValue = new TransformerEvent_Vector2(GameDefine.ms_fBlockChnageSlotTime, vTargetPos);
        m_pPos.AddEvent(eventValue);
        eventValue = new TransformerEvent_Vector2(GameDefine.ms_fBlockChnageSlotTime * 2, m_vPos);
        m_pPos.AddEvent(eventValue);
        m_pPos.OnPlay();
    }

    public void OnSwapPossibleDirection()
    {
        if (m_pRndSlotBlock != null)
            m_pRndSlotBlock.OnSwapPossibleDirection();
    }

    public void OnCancelSwapPossibleDirection()
    {
        if (m_pRndSlotBlock != null)
            m_pRndSlotBlock.OnCancelSwapPossibleDirection();
    }

    public Vector2 GetPosition()
    {
        if (m_pRndSlotBlock != null)
            return m_pRndSlotBlock.GetPosition();

        return new Vector2(-10000, -10000);
    }

    public eBlockType GetBlockType()
    {
        return m_eBlockType;
    }

    public eSpecialItem GetSpecialItem()
    {
        return m_eSpecialItem;
    }

    public SpecialItemCustomize GetSpecialItemCustomize()
    {
        return m_pSpecialItemCustomize;
    }

    //public InGameStageMapSlotData GetInGameStageMapSlotData()
    //{
    //    return m_pInGameStageMapSlotData;
    //}

    public eMapSlotItem GetMapSlotItem()
    {
        return m_eMapSlotItem;
    }

    public void SetParameta(object pParameta)
    {
        m_pParameta = pParameta;
    }

    public object GetParameta()
    {
        return m_pParameta;
    }

    public void SetHighlight(int nDepth = 0)
    {
        if (m_pRndSlotBlock != null)
        {
            m_pRndSlotBlock.SetHighlight(nDepth);
        }
    }

    public void ClearHighlight()
    {
        if (m_pRndSlotBlock != null)
        {
            m_pRndSlotBlock.ClearHighlight();
        }
    }
}