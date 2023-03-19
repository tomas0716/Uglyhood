using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SlotFixObject_Minion : SlotFixObject_Unit
{
    protected eMinionType           m_eMinionType               = eMinionType.EnemyMinion;

    protected GameObject            m_pGameObject               = null;
    protected Plane2D               m_pPlane_Back               = null;
    protected Plane2D               m_pPlane_Icon               = null;

    protected Plane2D               m_pPlane_Gauge_HP           = null;
    protected Plane2D               m_pPlane_Gauge_SP           = null;

    protected TextMeshPro           m_pTextMesh_DelayTurn       = null;
    protected Color                 m_clrDelayTurn              = Color.white;

    protected GameObject            m_pGameObject_Sword         = null;
    protected GameObject            m_pGameObject_SP            = null;

    protected TextMeshPro           m_pTextMesh_DamageText      = null;
    private Transformer_Scalar      m_pAlpha_DamageText         = new Transformer_Scalar(0);

    protected GameObject            m_pGameObject_NumberDamageAction = null;

    private bool                    m_IsSpawn                   = false;
    private Transformer_Vector2     m_vPos                      = new Transformer_Vector2(Vector2.zero);
    private Transformer_Timer       m_pVisible                  = new Transformer_Timer();

    private ParticleInfo            m_pParticleInfo_TurnOne     = null;
    private ParticleInfo            m_pParticleInfo_BossEdge    = null;

    private Dictionary<NumberDamageAction, Vector3> m_NumberDamageActionPosTable = new Dictionary<NumberDamageAction, Vector3>();

    private GameObject              m_pGameObject_BlackLayer    = null;
    private GameObject              m_pGameObject_CancelArea    = null;
    private int                     m_nCancelAreaFingerID       = 0;

    private bool                    m_IsDie                     = false;

    private int                     m_nOriginal_UnitID          = 0;
    private int                     m_nOriginal_UnitLevel       = 0;
    private ParticleInfo            m_pParticleInfo_Transform   = null;
    private GameObject              m_pGameObject_TransformMark = null;

    private GameObject              m_pGameObject_PassiveText   = null;

    public SlotFixObject_Minion(Slot pSlot, int nUnitID, int nUnitLevel, eMinionType eType, eObjectType eObjectType, eOwner eOwner, bool IsSpawn, bool IsMoveAndCreateInclude = false) : base(pSlot, nUnitID, eObjectType, eOwner)
    {
        m_nOriginal_UnitID = nUnitID;
        m_nOriginal_UnitLevel = m_nLevel = nUnitLevel;

        m_eMinionType = eType;
        m_IsSpawn = IsSpawn;
        m_IsMoveAndCreateInclude = IsMoveAndCreateInclude;

        ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
        ExcelData_UnitInfo pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(nUnitID);

        GameObject ob;
        Material material;
        Plane2D pPlane;

        string strPrefabName = "EnemyMinion";

        switch (m_eMinionType)
        {
            case eMinionType.EnemyMinion: strPrefabName = "EnemyMinion"; break;
            case eMinionType.PlayerSummonUnit: strPrefabName = "EnemyMinion"; break;
            case eMinionType.EnemySummonUnit: strPrefabName = "EnemyMinion"; break;
        }

        ob = Resources.Load<GameObject>("2D/Prefabs/Character/" + strPrefabName);
        m_pGameObject = GameObject.Instantiate(ob);
        m_pGameObject.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
        ob = Helper.FindChildGameObject(m_pGameObject, "Icon");
        m_pPlane_Icon = ob.GetComponent<Plane2D>();

        ob = Helper.FindChildGameObject(m_pGameObject, "Back");
        m_pPlane_Back = ob.GetComponent<Plane2D>();
        AtlasInfo pAtlasInfo = m_pPlane_Back.GetAtlasGroup().FindAtlasInfo("Character_BG_" + m_pUnitInfo.m_eElement);
        m_pPlane_Back.ChangeAtlasInfo(pAtlasInfo);

        material = AppInstance.Instance.m_pMaterialResourceManager.LoadMaterial("Image/PortraitIngame/Materials/", m_pUnitInfo.m_strPortrait_InGame);
        if (material != null)
        {
            m_pPlane_Icon.ChangeMaterial(material);
        }

        ob = Helper.FindChildGameObject(m_pGameObject, "Gauge_HP");
        m_pPlane_Gauge_HP = ob.GetComponent<Plane2D>();

        ob = Helper.FindChildGameObject(m_pGameObject, "Gauge_SP");
        m_pPlane_Gauge_SP = ob.GetComponent<Plane2D>();
        m_pPlane_Gauge_SP.SetColor(GameDefine.ms_Color_SP[(int)m_pUnitInfo.m_eElement]);

        ob = Helper.FindChildGameObject(m_pGameObject, "Text_DelayTurn");
        m_pTextMesh_DelayTurn = ob.GetComponent<TextMeshPro>();
        m_clrDelayTurn = m_pTextMesh_DelayTurn.color;

        m_pGameObject_Sword = Helper.FindChildGameObject(m_pGameObject, "Sword");

        m_pGameObject_SP = Helper.FindChildGameObject(m_pGameObject, "SP_Mark");
        m_pGameObject_SP.SetActive(false);

        m_pGameObject_NumberDamageAction = Helper.FindChildGameObject(m_pGameObject, "Text_Damage");
        m_pGameObject_NumberDamageAction.SetActive(false);

        ob = Helper.FindChildGameObject(m_pGameObject, "Text_State");
        m_pTextMesh_DamageText = ob.GetComponent<TextMeshPro>();
        m_pTextMesh_DamageText.gameObject.SetActive(false);

        ob = Helper.FindChildGameObject(m_pGameObject, "StatusEffect");
        m_pGameObject_Buff = Helper.FindChildGameObject(ob, "Buff");
        m_pGameObject_Buff.SetActive(false);
        m_pGameObject_Debuff = Helper.FindChildGameObject(ob, "Debuff");
        m_pGameObject_Debuff.SetActive(false);

        ob = Helper.FindChildGameObject(m_pGameObject_Buff, "Icon_Buff");
        ob.transform.localPosition = Vector3.zero;
        ob = Helper.FindChildGameObject(m_pGameObject_Debuff, "Icon_Debuff");
        ob.transform.localPosition = Vector3.zero;

        m_pGameObject_TransformMark = Helper.FindChildGameObject(m_pGameObject, "Transform");
        m_pGameObject_TransformMark.SetActive(false);

        switch (m_eOwner)
        {
            case eOwner.My:
                {
                    ob = Helper.FindChildGameObject(m_pGameObject_TransformMark, "Icon_Transform");
                    ob.SetActive(true);
                    ob = Helper.FindChildGameObject(m_pGameObject_TransformMark, "Icon_Transform_Enemy");
                    ob.SetActive(false);
                }
                break;

            case eOwner.Other:
                {
                    ob = Helper.FindChildGameObject(m_pGameObject_TransformMark, "Icon_Transform");
                    ob.SetActive(false);
                    ob = Helper.FindChildGameObject(m_pGameObject_TransformMark, "Icon_Transform_Enemy");
                    ob.SetActive(true);
                }
                break;
        }

        ExcelData_Unit_LevelUpInfo pLevelUpInfo = null;
        // 적 미니언 레벨업
        if (pUnitInfo.m_eUnitType == eUnitType.Character)
        {
            pLevelUpInfo = ExcelDataManager.Instance.m_pUnit_LevelUp.GetUnitLevelUpInfo_byKeys(pUnitInfo.m_eUnitType, pUnitInfo.m_nRank, (int)pUnitInfo.m_eElement, (int)pUnitInfo.m_eClass, nUnitLevel);
        }
        else
        {
            pLevelUpInfo = ExcelDataManager.Instance.m_pUnit_LevelUp.GetUnitLevelUpInfo_byKeys(pUnitInfo.m_eUnitType, 0, 0, 0, nUnitLevel);
        }

        m_nMaxHP = Mathf.RoundToInt(pUnitInfo.m_nMaxHP * pLevelUpInfo.m_nChange_MaxHP_Rate / 100);
        m_nVirtualHP = m_nHP = m_nMaxHP;
        m_nMaxSP = Mathf.RoundToInt(pUnitInfo.m_nMaxSP * pLevelUpInfo.m_nChange_MaxSP_Rate / 100);
        m_nATK = Mathf.RoundToInt(pUnitInfo.m_nATK * pLevelUpInfo.m_nChange_ATK_Rate / 100);
        m_nChargePerTurn_SP = Mathf.RoundToInt(pUnitInfo.m_nSP_ChargePerTurn * pLevelUpInfo.m_nChange_SPCharge_Rate / 100);

        m_pPlane_Gauge_HP.m_rcTexRect.xMax = (float)m_nHP / m_nMaxHP;
        m_pPlane_Gauge_HP.UpdateUV();

        m_pPlane_Gauge_SP.m_rcTexRect.yMax = (float)m_nSP / m_nMaxSP;
        m_pPlane_Gauge_SP.UpdateUV();

        m_nDelayTurn = m_pUnitInfo.m_nMaxAttackDelayTurn;
        m_pTextMesh_DelayTurn.text = m_nDelayTurn.ToString();

        if (m_nDelayTurn == 1)
        {
            m_pTextMesh_DelayTurn.color = Color.red;

            if (m_pParticleInfo_TurnOne == null)
            {
                m_pParticleInfo_TurnOne = ParticleManager.Instance.LoadParticleSystem("FX_UnitAttack_Ready", Vector3.zero);
                m_pParticleInfo_TurnOne.SetScale(InGameInfo.Instance.m_fInGameScale);
                m_pParticleInfo_TurnOne.GetGameObject().transform.SetParent(m_pGameObject_Sword.transform);
                m_pParticleInfo_TurnOne.GetGameObject().transform.localPosition = new Vector3(-2, -9, -88 );
            }
        }

        if (m_eMinionType == eMinionType.EnemyMinion && pUnitInfo.m_eUnitType == eUnitType.BossMinion)
        {
            m_pParticleInfo_BossEdge = ParticleManager.Instance.LoadParticleSystem("FX_UnitBoss_Edge", Vector3.zero);
            m_pParticleInfo_BossEdge.SetScale(InGameInfo.Instance.m_fInGameScale);
            m_pParticleInfo_BossEdge.GetGameObject().transform.SetParent(m_pGameObject.transform);
            m_pParticleInfo_BossEdge.GetGameObject().transform.localPosition = new Vector3(0, 0, -(int)ePlaneOrder.MinionCharacter_BossEdge);
        }

        m_pGameObject_PassiveText = Helper.FindChildGameObject(m_pGameObject, "Text_Passive");
        TextMeshPro pTextMeshPro = m_pGameObject_PassiveText.GetComponent<TextMeshPro>();
        pTextMeshPro.text = ExcelDataHelper.GetString("ACTION_ACTIVATED_TEXT_PASSIVE_EFFECT");
        m_pGameObject_PassiveText.SetActive(false);

        m_pPlane_Icon.AddCallback_LButtonDown(OnCallback_LButtonDown);
        m_pPlane_Icon.AddCallback_LButtonUp(OnCallback_LButtonUp);

        ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/BlackLayer");
        m_pGameObject_BlackLayer = GameObject.Instantiate(ob);
        m_pGameObject_BlackLayer.SetActive(false);
        m_pGameObject_BlackLayer.transform.localScale *= AppInstance.Instance.m_fHeightScale;

        if (m_IsSpawn == true)
        {
            m_pGameObject.SetActive(false);

            TransformerEvent_Timer eventValue;
            eventValue = new TransformerEvent_Timer(1.0f * GameDefine.ms_fEnemySlotMoveMultiple);
            m_pVisible.AddEvent(eventValue);
            m_pVisible.SetCallback(null, OnDone_Visible);
            m_pVisible.OnPlay();
        }

        EventDelegateManager.Instance.OnEventInGame_Tooltip_DestroyCancelArea += OnInGame_Tooltip_DestroyCancelArea;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (m_pSlot != null && m_pSlot.GetPlane_Slot() != null && m_pSlot.GetPlane_Slot().GetPickingComponent() != null)
        {
            m_pSlot.GetPlane_Slot().GetPickingComponent().RemoveCallback_LButtonUp(OnCallback_LButtonUp);
        }

        if (m_pParticleInfo_TurnOne != null)
        {
            ParticleManager.Instance.RemoveParticleInfo(m_pParticleInfo_TurnOne);
            m_pParticleInfo_TurnOne = null;
        }

        if (m_pParticleInfo_BossEdge != null)
        {
            ParticleManager.Instance.RemoveParticleInfo(m_pParticleInfo_BossEdge);
            m_pParticleInfo_BossEdge = null;
        }

        if (m_pGameObject_CancelArea != null)
        {
            GameObject.Destroy(m_pGameObject_CancelArea);
            m_pGameObject_CancelArea = null;
        }

        if (m_pGameObject_BlackLayer != null)
        {
            GameObject.Destroy(m_pGameObject_BlackLayer);
            m_pGameObject_BlackLayer = null;
        }

        if (m_pParticleInfo_Transform != null)
        {
            ParticleManager.Instance.RemoveParticleInfo(m_pParticleInfo_Transform);
            m_pParticleInfo_Transform = null;
        }

        GameObject.Destroy(m_pGameObject);
        m_pGameObject = null;

        EventDelegateManager.Instance.OnEventInGame_Tooltip_DestroyCancelArea -= OnInGame_Tooltip_DestroyCancelArea;
    }

    public override void Update(float fDeltaTime)
    {
        float fPosZ = m_pGameObject.transform.position.z;
        m_vPos.Update(fDeltaTime);
        Vector3 vPos = m_vPos.GetCurVector2();
        vPos.z = fPosZ;
        m_pGameObject.transform.position = vPos;

        m_pAlpha_DamageText.Update(Time.deltaTime);
        float fAlpha_DamageText = m_pAlpha_DamageText.GetCurScalar();
        Color color = m_pTextMesh_DamageText.color;
        color.a = fAlpha_DamageText;
        m_pTextMesh_DamageText.color = color;

        m_pVisible.Update(Time.deltaTime);
    }

    public override void SetDamageEffectType_TransformType(eDamageEffectType eType, int nTurnLifeCount, string strTransformRestoreEffect)
    {
        if (eType == eDamageEffectType.UnitSummon || eType == eDamageEffectType.UnitTransform || eType == eDamageEffectType.UnitClone)
        {
            m_pGameObject_TransformMark.SetActive(true);

            m_eEffecType_TransformType = eType;
            m_nTransformTurnLifeCount = nTurnLifeCount;
            if (m_nTransformTurnLifeCount == 0)
            {
                m_nTransformTurnLifeCount = int.MaxValue;
                m_IsTransformUnitLifeTurn_Infinity = true;
            }
            else
            {
                m_IsTransformUnitLifeTurn_Infinity = false;
            }

            m_strTransformRestoreEffect = strTransformRestoreEffect;

            if (m_pParticleInfo_Transform != null)
            {
                ParticleManager.Instance.RemoveParticleInfo(m_pParticleInfo_Transform);
                m_pParticleInfo_Transform = null;
            }

            if (m_eOwner == eOwner.My)
            {
                m_pParticleInfo_Transform = ParticleManager.Instance.LoadParticleSystem("FX_UnitTransformed_Edge", Vector3.zero);
            }
            else
            {
                m_pParticleInfo_Transform = ParticleManager.Instance.LoadParticleSystem("FX_UnitTransformed_Enemy_Edge", Vector3.zero);
            }
            m_pParticleInfo_Transform.SetScale(InGameInfo.Instance.m_fInGameScale);
            m_pParticleInfo_Transform.GetGameObject().transform.SetParent(m_pGameObject.transform);
            m_pParticleInfo_Transform.GetGameObject().transform.localPosition = new Vector3(0, 0, -(int)ePlaneOrder.MinionCharacter_BossEdge);
        }
    }

    public bool VirtualDecreaseTransformTurnLife()
    {
        int TurnLifeCount = m_nTransformTurnLifeCount;
        --TurnLifeCount;

        if (TurnLifeCount == 0)
        {
            return true;
        }

        return false;
    }

    public override bool DecreaseTransformTurnLife()
    {
        --m_nTransformTurnLifeCount;

        if (m_nTransformTurnLifeCount == 0)
        {
            return true;
        }

        return false;
    }

    public override void OnTransformUnit(int nUnitID, int nUnitLevel)
    {
        m_pGameObject_TransformMark.SetActive(true);

        if (nUnitLevel == 0)
        {
            nUnitLevel = GetLevel();
        }

        float fHPRate = ((float)m_nHP) / m_nMaxHP;
        float fSPRate = ((float)m_nSP) / m_nMaxSP;
        float fPrevMaxHP = m_nMaxHP - m_nAdd_MaxHP;
        float fPrevATK = m_nATK - m_nAdd_ATK;
        float fPrevChargePer_SP = m_nChargePerTurn_SP - m_nAdd_ChargePerTurn_SP;

        InnerTransformUnit(nUnitID, nUnitLevel);

        float fRate_MaxHP = ((float)(m_nMaxHP)) / fPrevMaxHP;
        float fRate_ATK = ((float)(m_nATK)) / fPrevATK;
        float fRate_ChargePer_SP = ((float)(m_nChargePerTurn_SP)) / fPrevChargePer_SP;

        m_nAdd_MaxHP = 0;
        m_nAdd_ATK = 0;
        m_nAdd_ChargePerTurn_SP = 0;
        m_nAdd_ChargePerBlock_SP = 0;

        m_pUnit_StatusEffect.OnTransformUnit(fRate_MaxHP, fRate_ATK, fRate_ChargePer_SP);

        int nHP = (int)Math.Ceiling(m_nMaxHP * fHPRate);
        if (nHP > m_nMaxHP)
            nHP = m_nMaxHP;
        ChangeHP(nHP, new Color(1, 1, 1, 0));
        m_nVirtualHP = nHP;

        int nSP = (int)Math.Ceiling(m_nMaxSP * fSPRate);
        if (nSP > m_nMaxSP)
            nSP = m_nMaxSP;
        ChangeSP(nSP, new Color(1, 1, 1, 0));
    }

    public override void OnTransformUnit(int nUnitID, int nUnitLevel, int nActionLevel)
    {
        OnTransformUnit(nUnitID, nUnitLevel);
    }

    public override void OnTransformUnitRestore()
    {
        m_pGameObject_TransformMark.SetActive(false);

        m_eEffecType_TransformType = eDamageEffectType.None;

        float fHPRate = ((float)m_nHP) / m_nMaxHP;
        float fSPRate = ((float)m_nSP) / m_nMaxSP;
        float fPrevMaxHP = m_nMaxHP - m_nAdd_MaxHP;
        float fPrevATK = m_nATK - m_nAdd_ATK;
        float fPrevChargePer_SP = m_nChargePerTurn_SP - m_nAdd_ChargePerTurn_SP;

        InnerTransformUnit(m_nOriginal_UnitID, m_nOriginal_UnitLevel);

        float fRate_MaxHP = ((float)(m_nMaxHP)) / fPrevMaxHP;
        float fRate_ATK = ((float)(m_nATK)) / fPrevATK;
        float fRate_ChargePer_SP = ((float)(m_nChargePerTurn_SP)) / fPrevChargePer_SP;

        m_nAdd_MaxHP = 0;
        m_nAdd_ATK = 0;
        m_nAdd_ChargePerTurn_SP = 0;
        m_nAdd_ChargePerBlock_SP = 0;

        m_pUnit_StatusEffect.OnTransformUnit(fRate_MaxHP, fRate_ATK, fRate_ChargePer_SP);

        int nHP = (int)Math.Ceiling(m_nMaxHP * fHPRate);
        if (nHP > m_nMaxHP)
            nHP = m_nMaxHP;
        ChangeHP(nHP, new Color(1, 1, 1, 0));
        m_nVirtualHP = nHP;

        int nSP = (int)Math.Ceiling(m_nMaxSP * fSPRate);
        if (nSP > m_nMaxSP)
            nSP = m_nMaxSP;
        ChangeSP(nSP, new Color(1, 1, 1, 0));

        if (m_pParticleInfo_Transform != null)
        {
            ParticleManager.Instance.RemoveParticleInfo(m_pParticleInfo_Transform);
            m_pParticleInfo_Transform = null;
        }

        ParticleInfo pParticleInfo = ParticleManager.Instance.LoadParticleSystem(m_strTransformRestoreEffect, Vector3.zero);

        if (pParticleInfo != null && pParticleInfo.GetGameObject() != null)
        {
            pParticleInfo.SetScale(InGameInfo.Instance.m_fInGameScale);
            pParticleInfo.GetGameObject().transform.localPosition = m_pGameObject.transform.position + new Vector3(0, 0, -(int)ePlaneOrder.Fx_TopLayer);
        }
    }

    private void InnerTransformUnit(int nUnitID, int nUnitLevel)
    {
        m_nUnitID = nUnitID;
        m_nLevel = nUnitLevel;
        m_pUnitInfo = ExcelDataManager.Instance.m_pUnit.GetUnitInfo(m_nUnitID);

        // 외형 변화
        AtlasInfo pAtlasInfo = m_pPlane_Back.GetAtlasGroup().FindAtlasInfo("Character_BG_" + m_pUnitInfo.m_eElement);
        m_pPlane_Back.ChangeAtlasInfo(pAtlasInfo);

        Material material = AppInstance.Instance.m_pMaterialResourceManager.LoadMaterial("Image/PortraitIngame/Materials/", m_pUnitInfo.m_strPortrait_InGame);
        if (material != null)
        {
            m_pPlane_Icon.ChangeMaterial(material);
        }

        m_pPlane_Gauge_SP.SetColor(GameDefine.ms_Color_SP[(int)m_pUnitInfo.m_eElement]);

        // 스탯 변화
        if (m_pUnitInfo.m_eUnitType == eUnitType.EnemyMinion || m_pUnitInfo.m_eUnitType == eUnitType.PlayerMinion || m_pUnitInfo.m_eUnitType == eUnitType.RaidBoss)
        {
            ExcelData_Unit_LevelUpInfo pLevelUpInfo = ExcelDataManager.Instance.m_pUnit_LevelUp.GetUnitLevelUpInfo_byKeys(m_pUnitInfo.m_eUnitType, 0, 0, 0, nUnitLevel);
            m_nMaxHP = Mathf.RoundToInt(m_pUnitInfo.m_nMaxHP * pLevelUpInfo.m_nChange_MaxHP_Rate / 100);
            m_nMaxSP = Mathf.RoundToInt(m_pUnitInfo.m_nMaxSP * pLevelUpInfo.m_nChange_MaxSP_Rate / 100);
            m_nATK = Mathf.RoundToInt(m_pUnitInfo.m_nATK * pLevelUpInfo.m_nChange_ATK_Rate / 100);
            m_nChargePerTurn_SP = Mathf.RoundToInt(m_pUnitInfo.m_nSP_ChargePerTurn * pLevelUpInfo.m_nChange_SPCharge_Rate / 100);
        }
        else if (m_pUnitInfo.m_eUnitType == eUnitType.Character)
        {
            int nCombat;
            ExcelDataHelper.GetUnitStat(nUnitID, nUnitLevel, out m_nMaxHP, out m_nMaxSP, out m_nChargePerBlock_SP, out m_nChargePerTurn_SP, out m_nATK, out nCombat);
        }
    }

    public override GameObject GetGameObject()
    {
        return m_pGameObject;
    }

    public void SetVisible(bool IsVisible)
    {
        if(m_pGameObject != null)
            m_pGameObject.SetActive(IsVisible);
    }

    public void OnDone_Visible(TransformerEvent eventValue)
    {
        m_pGameObject.SetActive(true);
    }

    public override void SetPosition(Vector2 vPos)
    {
        if (m_IsSpawn == false)
        {
            if (m_pGameObject != null)
            {
                m_pGameObject.transform.position = vPos;
                m_vPos = new Transformer_Vector2(vPos);
            }
        }
        else
        {
            TransformerEvent_Vector2 eventValue;
            eventValue = new TransformerEvent_Vector2(0, new Vector3(vPos.x, vPos.y + 142 * InGameInfo.Instance.m_fInGameScale));
            m_vPos.AddEvent(eventValue);
            eventValue = new TransformerEvent_Vector2(1.0f, new Vector3(vPos.x, vPos.y + 142 * InGameInfo.Instance.m_fInGameScale));
            m_vPos.AddEvent(eventValue);
            eventValue = new TransformerEvent_Vector2(GameDefine.ms_fEnemyOneBlockMoveTime + 1.0f, vPos);
            m_vPos.AddEvent(eventValue);
            m_vPos.OnPlay();
        }

        m_IsSpawn = false;
    }

    public void SetForcePosition(Vector2 vPos)
    {
        m_vPos = new Transformer_Vector2(vPos);
    }

    public void OnAttackCast()
    {
        Vector2 vPos = m_pGameObject.transform.position;
        Vector2 vTargetPos = new Vector2(vPos.x, vPos.y + (m_eOwner == eOwner.My ? InGameInfo.Instance.m_fSlotSize * 0.5f : -InGameInfo.Instance.m_fSlotSize * 0.5f));

        m_vPos.OnReset();
        TransformerEvent_Vector2 eventValue;
        eventValue = new TransformerEvent_Vector2(0, vPos);
        m_vPos.AddEvent(eventValue);
        eventValue = new TransformerEvent_Vector2(0.05f, vTargetPos);
        m_vPos.AddEvent(eventValue);
        eventValue = new TransformerEvent_Vector2(0.12f, vPos);
        m_vPos.AddEvent(eventValue);
        m_vPos.OnPlay();
    }

    public virtual eMinionType GetMinionType()
    {
        return m_eMinionType;
    }

    public override void ChangeHP(int nHP)
    {
        ChangeHP(nHP, Color.red);
    }

    public override void ChangeHP(int nHP, Color color)
    {
        int nDamage = Math.Abs(m_nHP - nHP);

        if (nDamage != 0 && color.a != 0)
        {
            Vector3 vRandomPos = GetNumberDamageActionPos();

            GameObject ob = GameObject.Instantiate(m_pGameObject_NumberDamageAction);
            ob.SetActive(true);
            Vector3 vPos = m_pGameObject_NumberDamageAction.transform.position;
            vPos.z = -(float)ePlaneOrder.Character_Number_Damage;
            
            MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
            if (pDataStack != null && pDataStack.m_pCloudManager.IsExistCloud(m_pSlot) == true)
            {
                vPos.z = -(float)ePlaneOrder.Unit_CloudAtDamage;
            }

            ob.transform.position = vPos + vRandomPos;

            NumberDamageAction pNumberDamageAction = ob.GetComponent<NumberDamageAction>();
            pNumberDamageAction.Init(this, nDamage, color);
            m_NumberDamageActionPosTable.Add(pNumberDamageAction, vRandomPos);
        }

        base.ChangeHP(nHP);
        m_pPlane_Gauge_HP.m_rcTexRect.xMax = (float)m_nHP / m_nMaxHP;
        m_pPlane_Gauge_HP.UpdateUV();
    }

    public override void ChangeSP(int nSP)
    {
        OutputLog.Log(string.Format("Minion ChangeSP : {0} / {1}", nSP, GetMaxSP()), true);
        ChangeSP(nSP, new Color(0, 1, 0, 0));
    }

    public override void ChangeSP(int nSP, Color color)
    {
        int nDamage = Math.Abs(m_nSP - nSP);

        if (nDamage != 0 && color.a != 0)
        {
            Vector3 vRandomPos = GetNumberDamageActionPos();

            GameObject ob = GameObject.Instantiate(m_pGameObject_NumberDamageAction);
            ob.SetActive(true);
            Vector3 vPos = m_pGameObject_NumberDamageAction.transform.position;
            vPos.z = -(float)ePlaneOrder.Character_Number_Damage;

            MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
            if (pDataStack != null && pDataStack.m_pCloudManager.IsExistCloud(m_pSlot) == true)
            {
                vPos.z = -(float)ePlaneOrder.Unit_CloudAtDamage;
            }
            ob.transform.position = vPos + vRandomPos;
            ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

            NumberDamageAction pNumberDamageAction = ob.GetComponent<NumberDamageAction>();
            pNumberDamageAction.Init(this, nDamage, color);
            m_NumberDamageActionPosTable.Add(pNumberDamageAction, vRandomPos);
        }

        base.ChangeSP(nSP);
        m_pPlane_Gauge_SP.m_rcTexRect.yMax = (float)m_nSP / m_nMaxSP;
        m_pPlane_Gauge_SP.UpdateUV();

        m_pGameObject_SP.SetActive(IsFull_SP());
    }

    private Vector3 GetNumberDamageActionPos()
    {
        if (m_NumberDamageActionPosTable.Count > 0)
        {
            float fWidth = 128.0f * InGameInfo.Instance.m_fInGameScale * 0.5f;
            float fHeight= 128.0f * InGameInfo.Instance.m_fInGameScale * 0.25f;

            float x = UnityEngine.Random.Range(-fWidth, fWidth);
            float y = UnityEngine.Random.Range(-fHeight, fHeight);

            return new Vector3(x, y, 0);
        }

        return Vector3.zero;
    }

    public override void OnNumberDamageActionDone(NumberDamageAction pNumberDamageAction)
    {
        if (m_NumberDamageActionPosTable.ContainsKey(pNumberDamageAction) == true)
        {
            m_NumberDamageActionPosTable.Remove(pNumberDamageAction);
        }
    }

    public override void OnDamageText(eUnitDamage eString)
    {
        string strText = "";
        Color color = Color.white;

        switch (eString)
        {      
            case eUnitDamage.Strong:
                {
                    strText = ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_CORRELATION_STRONG");
                    color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Strong];
                }
                break;
            case eUnitDamage.Weak:
                {
                    strText = ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_CORRELATION_WEAK");
                    color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Weak];
                }
                break;
            case eUnitDamage.Hero_Missing:
                {
                    strText = ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_CORRELATION_MISSING");
                    color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Hero_Missing];
                }
                break;
            case eUnitDamage.HP_Recovery:
                {
                    strText = ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_HP_RECOVERY");
                    color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.HP_Recovery];
                }
                break;
            case eUnitDamage.SP_Recharge:
                {
                    strText = ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_SP_RECHARGE");
                    color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.SP_Recharge];
                }
                break;
            case eUnitDamage.SP_Decrease:
                {
                    strText = ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_SP_DECREASE");
                    color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.SP_Decrease];
                }
                break;
        }

        GameObject ob = GameObject.Instantiate(m_pTextMesh_DamageText.gameObject);
        ob.SetActive(true);
        Vector3 vPos = m_pTextMesh_DamageText.gameObject.transform.position;
        vPos.z = -(float)ePlaneOrder.Character_Number_Damage;
        ob.transform.position = vPos;
        ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

        TextDamageAction pNumberDamageAction = ob.AddComponent<TextDamageAction>();
        pNumberDamageAction.Init(strText, color);
    }

    public override void OnDamageText(string strText)
    {
        GameObject ob = GameObject.Instantiate(m_pTextMesh_DamageText.gameObject);
        ob.SetActive(true);
        Vector3 vPos = m_pTextMesh_DamageText.gameObject.transform.position;
        vPos.z = -(float)ePlaneOrder.Character_Number_Damage;
        ob.transform.position = vPos;
        ob.transform.localScale = new Vector3(InGameInfo.Instance.m_fInGameScale, InGameInfo.Instance.m_fInGameScale, 1);

        TextDamageAction pNumberDamageAction = ob.AddComponent<TextDamageAction>();
        pNumberDamageAction.Init(strText, Color.white);
    }

    public override bool DecreaseTurn()
    {
        OutputLog.Log("Minion DecreaseTurn", true);

        bool IsActiveAttack = base.DecreaseTurn();

        m_pTextMesh_DelayTurn.text = m_nDelayTurn.ToString();

        if (m_nDelayTurn == 1)
        {
            m_pTextMesh_DelayTurn.color = Color.red;

            if (m_pParticleInfo_TurnOne == null)
            {
                m_pParticleInfo_TurnOne = ParticleManager.Instance.LoadParticleSystem("FX_UnitAttack_Ready", Vector3.zero);
                m_pParticleInfo_TurnOne.SetScale(InGameInfo.Instance.m_fInGameScale);
                m_pParticleInfo_TurnOne.GetGameObject().transform.SetParent(m_pGameObject_Sword.transform);
                m_pParticleInfo_TurnOne.GetGameObject().transform.localPosition = new Vector3(-2, -9, -88);

                Helper.OnSoundPlay("INGAME_UNIT_ACTION_START", false);
            }
        }
        else
        {
            m_pTextMesh_DelayTurn.color = m_clrDelayTurn;

            if (m_pParticleInfo_TurnOne != null)
            {
                ParticleManager.Instance.RemoveParticleInfo(m_pParticleInfo_TurnOne);
                m_pParticleInfo_TurnOne = null;
            }
        }

        return IsActiveAttack;
    }

    public override void OnDead()
    {
        if (m_IsDie == false)
        {
            m_IsDie = true;

            if (m_eMinionType == eMinionType.EnemyMinion && m_eEffecType_TransformType != eDamageEffectType.UnitSummon)
            {
                MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
                EventDelegateManager.Instance.OnInGame_EnemyMinionDie(this, --pDataStack.m_nCurrObjectiveCount);
            }

            // 일단 바로 죽임
            m_pSlot.RemoveSlotFixObject(this);

            EventDelegateManager.Instance.OnInGame_TurnComponentDone(eTurnComponentType.EnemyColonyChangeState);
        }
    }

    public bool IsDead()
    {
        return m_IsDie;
    }

    public override void SetHighlight(int nDepth = 0)
    {
        Vector3 vPos = m_pGameObject.transform.position;
        vPos.z = -(float)(ePlaneOrder.Highlight_Content + nDepth);
        m_pGameObject.transform.position = vPos;
    }

    public override void ClearHighlight()
    {
        if (m_pGameObject != null)
        {
            Vector3 vPos = m_pGameObject.transform.position;
            vPos.z = 0;
            m_pGameObject.transform.position = vPos;
        }
    }

    public void ShowPassiveText(bool IsShow)
    {
        m_pGameObject_PassiveText.SetActive(IsShow);
    }

    protected void OnCallback_LButtonDown(GameObject gameObject, Vector3 vPos, object ob, int nFingerID)
    {
        if (InGameInfo.Instance.m_IsAutoPlay == false && EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
        {
            InGameInfo.Instance.m_nClickPlayerCharacterFingerID = nFingerID;

            MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
            pDataStack.m_pMinion_SkillTrigger = this;
        }
    }

    protected void OnCallback_LButtonUp(GameObject gameObject, Vector3 vPos, object ob, int nFingerID, bool IsDown)
    {
        if (InGameInfo.Instance.m_eCurrGameResult != eGameResult.None)
        {
            InGameInfo.Instance.m_nClickPlayerCharacterFingerID = -1;
            InGameInfo.Instance.m_pClickPlayerCharacter = null;
            return;
        }

        if (InGameInfo.Instance.m_IsAutoPlay == false && EspressoInfo.Instance.m_ePVP_CurrTurn_Owner == eOwner.My)
        {
            MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

            if (IsDown == true && pDataStack.m_pMinion_SkillTrigger == this &&
                InGameInfo.Instance.m_nClickPlayerCharacterFingerID == nFingerID && InGameInfo.Instance.m_IsInGameClick == true)
            {
                bool IsValidClick = true;

                if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.None)
                {
                    if (pDataStack.m_pPlayerCharacter_SkillTrigger == null)
                    {
                        if (m_nMaxSP != 0 && m_pUnitInfo.m_nActiveSkill_ActionTableID != 0)
                        {
                            EspressoInfo.Instance.m_eActionTriggerSubject = eActionTriggerSubject.Minion;

                            EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Close();
                            GameObject ob_detail = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGameActionDetail");
                            GameObject.Instantiate(ob_detail);
                        }

                        if (m_pGameObject_CancelArea != null) GameObject.Destroy(m_pGameObject_CancelArea);
                        GameObject ob_cancelArea = Resources.Load<GameObject>("2D/Prefabs/Character/PlayerCharacterAttackCancelArea");
                        m_pGameObject_CancelArea = GameObject.Instantiate(ob_cancelArea);
                        Plane2D pPlane_CancelArea = m_pGameObject_CancelArea.GetComponent<Plane2D>();
                        pPlane_CancelArea.AddCallback_LButtonDown(OnCallback_LButtonDown_CancelArea);
                        pPlane_CancelArea.AddCallback_LButtonUp(OnCallback_LButtonUp_CancelArea);

                        m_pGameObject_BlackLayer.SetActive(true);
                        SetHighlight();

                        EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Open_ToGray();

                        EspressoInfo.Instance.m_IsActionAndUnitDetailCloseBlock = false;
                        EventDelegateManager.Instance.OnInGame_UnitDetail_Close();
                        GameObject ob_unit_detail = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGameUnitDetail");
                        ob_unit_detail = GameObject.Instantiate(ob_unit_detail);
                        InGame_UnitDetail_UI pScript = ob_unit_detail.GetComponent<InGame_UnitDetail_UI>();
                        pScript.Init(this);
                    }
                }

                if (IsValidClick == true)
                {
                    EventDelegateManager.Instance.OnInGame_Slot_Click(m_pSlot);
                }
            }

            InGameInfo.Instance.m_nClickPlayerCharacterFingerID = -1;
            InGameInfo.Instance.m_pClickPlayerCharacter = null;
        }
    }

    protected void OnCallback_LButtonDown_CancelArea(GameObject gameObject, Vector3 vPos, object ob, int nFingerID)
    {
        if (InGameInfo.Instance.m_IsAutoPlay == false)
        {
            m_nCancelAreaFingerID = nFingerID;
        }
    }

    protected void OnCallback_LButtonUp_CancelArea(GameObject gameObject, Vector3 vPos, object ob, int nFingerID, bool IsDown)
    {
        if (IsDown == true && m_nCancelAreaFingerID == nFingerID && EspressoInfo.Instance.m_IsInGame_SkillTriggerDrag == false && 
            InGameInfo.Instance.m_IsAutoPlay == false && EspressoInfo.Instance.m_IsActionAndUnitDetailCloseBlock == false)
        {
            EspressoInfo.Instance.m_eActionTriggerSubject = eActionTriggerSubject.None;

            EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Cancel();
            EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Close();
            EventDelegateManager.Instance.OnInGame_UnitDetail_Close();

            if (m_pGameObject_CancelArea != null)
            {
                GameObject.Destroy(m_pGameObject_CancelArea);
                m_pGameObject_CancelArea = null;
            }

            m_pGameObject_BlackLayer.SetActive(false);

            ClearHighlight();
        }

        EspressoInfo.Instance.m_IsActionAndUnitDetailCloseBlock = false;
    }

    public void OnInGame_Tooltip_DestroyCancelArea()
    {
        m_pGameObject_BlackLayer.SetActive(false);

        if (m_pGameObject_CancelArea != null)
        {
            GameObject.Destroy(m_pGameObject_CancelArea);
            m_pGameObject_CancelArea = null;
        }
    }
}
