using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SlotFixObject_PlayerCharacter : SlotFixObject_Unit
{
    protected CharacterInvenItemInfo m_pOriginalCharacterInvenItemInfo = null;
    protected CharacterInvenItemInfo m_pCharacterInvenItemInfo = null;

    protected GameObject            m_pGameObject           = null;
    protected Plane2D               m_pPlane_Back           = null;
    protected Plane2D               m_pPlane_Icon           = null;

    protected Plane2D               m_pPlane_Gauge_HP       = null;
    protected Plane2D               m_pPlane_Gauge_SP       = null;

    protected TextMeshPro           m_pTextMesh_HP          = null;

    protected Plane2D               m_pPlan_SP_Full_Effect  = null;
    protected GameObject            m_pGameObject_NumberDamageAction   = null;

    protected TextMeshPro           m_pTextMesh_DamageText  = null;
    private Transformer_Scalar      m_pAlpha_DamageText     = new Transformer_Scalar(0);

    protected GameObject            m_pGameObject_Die_Over  = null;

    private bool                    m_IsDie                 = false;
   
    private Transformer_Vector2     m_pPos                  = new Transformer_Vector2(Vector2.zero);

    private Dictionary<NumberDamageAction, Vector3> m_NumberDamageActionPosTable = new Dictionary<NumberDamageAction, Vector3>();

    private bool                    m_IsApplyPassiveAction_ForOnce  = false;

    private ParticleSystem          m_pParticleSystem_MatchDamageAttack = null;

    private int                     m_nOriginal_UnitID          = 0;
    private ParticleInfo            m_pParticleInfo_Transform   = null;
    private GameObject              m_pGameObject_TransformMark = null;

    private bool                    m_IsSkillFirstSelect        = false;

    private GameObject              m_pGameObject_PassiveText   = null;

    public SlotFixObject_PlayerCharacter(Slot pSlot, int nUnitID, int nUniqueID, eObjectType eObjectType, eOwner eOwner, CharacterInvenItemInfo pOtherInvenItemInfo = null) : base(pSlot, nUnitID, eObjectType, eOwner)
    {
        if (eOwner == eOwner.My)
        {
            m_pOriginalCharacterInvenItemInfo = m_pCharacterInvenItemInfo = InventoryInfoManager.Instance.m_pCharacterInvenInfo.GetInvenItem_byUniqueID(nUniqueID);
        }
        else
        {
            m_pOriginalCharacterInvenItemInfo = m_pCharacterInvenItemInfo = pOtherInvenItemInfo;
        }

        m_nOriginal_UnitID = nUnitID;
        GameObject ob;
        Material material;
        Plane2D pPlane;

        ob = Resources.Load<GameObject>("2D/Prefabs/Character/PlayerCharacter");
        m_pGameObject = GameObject.Instantiate(ob);
        m_pGameObject.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
        ob = Helper.FindChildGameObject(m_pGameObject, "Icon");
        m_pPlane_Icon = ob.GetComponent<Plane2D>();

        ob = Helper.FindChildGameObject(m_pGameObject, "BG");
        m_pPlane_Back = ob.GetComponent<Plane2D>();
        AtlasInfo pAtlasInfo = m_pPlane_Back.GetAtlasGroup().FindAtlasInfo("Character_BG_" + m_pUnitInfo.m_eElement);
        m_pPlane_Back.ChangeAtlasInfo(pAtlasInfo);

        material = AppInstance.Instance.m_pMaterialResourceManager.LoadMaterial("Image/PortraitIngame/Materials/", m_pUnitInfo.m_strPortrait_InGame);
        if (material != null)
        {
            m_pPlane_Icon.ChangeMaterial(material);
        }

        ob = Helper.FindChildGameObject(m_pGameObject, "HP");
        m_pPlane_Gauge_HP = ob.GetComponent<Plane2D>();

        ob = Helper.FindChildGameObject(m_pGameObject, "SP");
        m_pPlane_Gauge_SP = ob.GetComponent<Plane2D>();
        m_pPlane_Gauge_SP.SetColor(GameDefine.ms_Color_SP[(int)m_pUnitInfo.m_eElement]);

        ob = Helper.FindChildGameObject(m_pGameObject, "Number_HP");
        m_pTextMesh_HP = ob.GetComponent<TextMeshPro>();

        ob = Helper.FindChildGameObject(m_pGameObject, "SP_Full_Effect");
        m_pPlan_SP_Full_Effect = ob.GetComponent<Plane2D>();

        m_pGameObject_NumberDamageAction = Helper.FindChildGameObject(m_pGameObject, "Text_Damage");
        m_pGameObject_NumberDamageAction.SetActive(false);

        ob = Helper.FindChildGameObject(m_pGameObject, "Text_State");
        m_pTextMesh_DamageText = ob.GetComponent<TextMeshPro>();

        m_pGameObject_Die_Over = Helper.FindChildGameObject(m_pGameObject, "Die_Over");
        m_pGameObject_Die_Over.SetActive(false);

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

        m_nLevel = m_pCharacterInvenItemInfo.m_nLevel;
        m_nHP = m_pCharacterInvenItemInfo.m_nMaxHP;
        m_nMaxHP = m_nHP;
        m_nVirtualHP = m_nHP;

        m_nATK = m_pCharacterInvenItemInfo.m_nATK;
        m_nChargePerBlock_SP = m_pCharacterInvenItemInfo.m_nSp_ChargePerBlock;

        m_nSP = 0;
        m_nMaxSP = m_pCharacterInvenItemInfo.m_nMaxSP;

        m_pPlane_Gauge_HP.m_rcTexRect.xMax = (float)m_nHP / m_nMaxHP;
        m_pPlane_Gauge_HP.UpdateUV();

        m_pPlane_Gauge_SP.m_rcTexRect.yMax = (float)m_nSP / m_nSP;
        m_pPlane_Gauge_SP.UpdateUV();

        m_pPlane_Gauge_SP.gameObject.SetActive(true);
        m_pPlan_SP_Full_Effect.SetColor(new Color(1, 1, 1, 0));

        m_pTextMesh_HP.text = m_nHP.ToString();

        ob = Helper.FindChildGameObject(m_pGameObject, "FX_Character_MatchDamageAttack_" + m_pUnitInfo.m_eElement);
        m_pParticleSystem_MatchDamageAttack = ob.GetComponent<ParticleSystem>();
        m_pParticleSystem_MatchDamageAttack.Stop();

        m_pGameObject_PassiveText = Helper.FindChildGameObject(m_pGameObject, "Text_Passive");
        TextMeshPro pTextMeshPro = m_pGameObject_PassiveText.GetComponent<TextMeshPro>();
        pTextMeshPro.text = ExcelDataHelper.GetString("ACTION_ACTIVATED_TEXT_PASSIVE_EFFECT");
        m_pGameObject_PassiveText.SetActive(false);

        m_pPlane_Icon.AddCallback_LButtonDown(OnCallback_LButtonDown);
        m_pPlane_Icon.AddCallback_LButtonUp(OnCallback_LButtonUp);

        EventDelegateManager.Instance.OnEventInGame_ChangeAutoPlay += OnInGame_ChangeAutoPlay;
        EventDelegateManager.Instance.OnEventInGame_CreateBlockMatchMissile += OnInGame_CreateBlockMatchMissile;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (m_pParticleInfo_Transform != null)
        {
            ParticleManager.Instance.RemoveParticleInfo(m_pParticleInfo_Transform);
            m_pParticleInfo_Transform = null;
        }

        EventDelegateManager.Instance.OnEventInGame_ChangeAutoPlay -= OnInGame_ChangeAutoPlay;
        EventDelegateManager.Instance.OnEventInGame_CreateBlockMatchMissile -= OnInGame_CreateBlockMatchMissile;

        GameObject.Destroy(m_pGameObject);
    }

    public override void Update(float fDeltaTime)
    {
        float fPosZ = m_pGameObject.transform.position.z;
        m_pPos.Update(fDeltaTime);
        Vector3 vPos = m_pPos.GetCurVector2();
        vPos.z = fPosZ;

        if (m_IsSkillFirstSelect == false)
        {
            m_pGameObject.transform.position = vPos;
        }
        else
        {
            vPos.y += InGameInfo.Instance.m_fSlotSize * 0.25f;
            m_pGameObject.transform.position = vPos;
        }

        if (IsFull_SP() == true && m_IsDie == false)
        {
            float fAlpha = SP_Full_Effect_Flesh.GetAlpha();
            m_pPlan_SP_Full_Effect.SetColor(new Color(1, 1, 1, fAlpha));
        }

        m_pAlpha_DamageText.Update(Time.deltaTime);
        float fAlpha_DamageText = m_pAlpha_DamageText.GetCurScalar();
        Color color = m_pTextMesh_DamageText.color;
        color.a = fAlpha_DamageText;
        m_pTextMesh_DamageText.color = color;
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
        OnTransformUnit(nUnitID, nUnitLevel, m_pOriginalCharacterInvenItemInfo.m_nAction_Level);
    }

    public override void OnTransformUnit(int nUnitID, int nUnitLevel, int nActionLevel)
    {
        m_pGameObject_TransformMark.SetActive(true);

        if (nUnitLevel == 0)
        {
            nUnitLevel = GetLevel();
        }

        if (nActionLevel == 0)
        {
            nActionLevel = m_pOriginalCharacterInvenItemInfo.m_nAction_Level;
        }

        float fHPRate = ((float)m_nHP) / m_nMaxHP;
        float fSPRate = ((float)m_nSP) / m_nMaxSP;
        float fPrevMaxHP = m_nMaxHP - m_nAdd_MaxHP;
        float fPrevATK = m_nATK - m_nAdd_ATK;
        float fPrevChargeBlock_SP = m_nChargePerBlock_SP - m_nAdd_ChargePerBlock_SP;

        InnerTransformUnit(nUnitID, nUnitLevel, nActionLevel);

        float fRate_MaxHP = ((float)(m_nMaxHP)) / fPrevMaxHP;
        float fRate_ATK = ((float)(m_nATK)) / fPrevATK;
        float fRate_ChargePer_SP = ((float)(m_nChargePerBlock_SP)) / fPrevChargeBlock_SP;

        m_nAdd_MaxHP = 0;
        m_nAdd_ATK = 0;
        m_nAdd_ChargePerTurn_SP = 0;
        m_nAdd_ChargePerBlock_SP = 0;

        for (int i = 0; i < (int)eElement.Max; ++i)
        {
            MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
            pDataStack.m_nPlayerCharacterAttackValue[i] = 0;
        }

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

    public override void OnTransformUnitRestore()
    {
        if (m_eEffecType_TransformType == eDamageEffectType.UnitTransform || m_eEffecType_TransformType == eDamageEffectType.UnitClone)
        {
            m_pGameObject_TransformMark.SetActive(false);

            m_eEffecType_TransformType = eDamageEffectType.None;

            float fHPRate = ((float)m_nHP) / m_nMaxHP;
            float fSPRate = ((float)m_nSP) / m_nMaxSP;
            float fPrevMaxHP = m_nMaxHP - m_nAdd_MaxHP;
            float fPrevATK = m_nATK - m_nAdd_ATK;
            float fPrevChargeBlock_SP = m_nChargePerBlock_SP - m_nAdd_ChargePerBlock_SP;

            InnerTransformUnitRestore(m_nOriginal_UnitID);

            float fRate_MaxHP = ((float)(m_nMaxHP)) / fPrevMaxHP;
            float fRate_ATK = ((float)(m_nATK)) / fPrevATK;
            float fRate_ChargePer_SP = ((float)(m_nChargePerBlock_SP)) / fPrevChargeBlock_SP;

            m_nAdd_MaxHP = 0;
            m_nAdd_ATK = 0;
            m_nAdd_ChargePerTurn_SP = 0;
            m_nAdd_ChargePerBlock_SP = 0;

            for (int i = 0; i < (int)eElement.Max; ++i)
            {
                MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();
                pDataStack.m_nPlayerCharacterAttackValue[i] = 0;
            }

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
    }

    private void InnerTransformUnit(int nUnitID, int nUnitLevel, int nActionLevel)
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
            m_nChargePerBlock_SP = Mathf.RoundToInt(m_pUnitInfo.m_nSP_ChargePerBlock * pLevelUpInfo.m_nChange_SPCharge_Rate / 100);
            m_nChargePerTurn_SP = Mathf.RoundToInt(m_pUnitInfo.m_nSP_ChargePerTurn * pLevelUpInfo.m_nChange_SPCharge_Rate / 100);
        }
        else if (m_pUnitInfo.m_eUnitType == eUnitType.Character)
        {
            int nCombat;
            ExcelDataHelper.GetUnitStat(nUnitID, nUnitLevel, out m_nMaxHP, out m_nMaxSP, out m_nChargePerBlock_SP, out m_nChargePerTurn_SP, out m_nATK, out nCombat);
        }

        m_pCharacterInvenItemInfo = new CharacterInvenItemInfo();
        m_pCharacterInvenItemInfo.m_nTableID = m_nUnitID;
        m_pCharacterInvenItemInfo.m_nLevel = nUnitLevel;
        m_pCharacterInvenItemInfo.m_nMaxHP = m_nMaxHP;
        m_pCharacterInvenItemInfo.m_nMaxSP = m_nMaxSP;
        m_pCharacterInvenItemInfo.m_nSp_ChargePerBlock = m_nChargePerBlock_SP;
        m_pCharacterInvenItemInfo.m_nATK = m_nATK;
        m_pCharacterInvenItemInfo.m_nAction_Level = nActionLevel;//m_pOriginalCharacterInvenItemInfo.m_nAction_Level;
    }

    private void InnerTransformUnitRestore(int nUnitID)
    {
        m_nUnitID = nUnitID;
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
        m_pCharacterInvenItemInfo = m_pOriginalCharacterInvenItemInfo;
        m_nLevel = m_pCharacterInvenItemInfo.m_nLevel;
        m_nMaxHP = m_pCharacterInvenItemInfo.m_nMaxHP;
        m_nATK = m_pCharacterInvenItemInfo.m_nATK;
        m_nChargePerBlock_SP = m_pCharacterInvenItemInfo.m_nSp_ChargePerBlock;
        m_nMaxSP = m_pCharacterInvenItemInfo.m_nMaxSP;
    }

    public override GameObject GetGameObject()
    {
        return m_pGameObject;
    }

    public CharacterInvenItemInfo GetCharacterInvenItemInfo()
    {
        return m_pCharacterInvenItemInfo;
    }

    public override void SetPosition(Vector2 vPos)
    {
        if (m_eOwner == eOwner.Other)
        {
            vPos.y += 17.0f * InGameInfo.Instance.m_fInGameScale; 
        }

        m_pGameObject.transform.position = vPos;
        m_pPos = new Transformer_Vector2(vPos);
    }
    public override void ChangeHP(int nHP)
    {
        ChangeHP(nHP, Color.red);
    }

    public override void ChangeHP(int nHP, Color color)
    {
        int nDamage = Math.Abs(m_nHP - nHP);

        if (nDamage > 0 && color.a != 0)
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

        m_pTextMesh_HP.text = m_nHP.ToString();
    }

    public override void ChangeSP(int nSP)
    {
        ChangeSP(nSP, new Color(0, 0, 0, 0));
    }

    public override void ChangeSP(int nSP, Color color)
    {
        if (m_nMaxSP == 0 || m_pUnitInfo.m_nActiveSkill_ActionTableID == 0)
        {
            m_pPlane_Gauge_SP.gameObject.SetActive(false);
            m_pPlan_SP_Full_Effect.SetColor(new Color(1, 1, 1, 0));

            return;
        }

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

            NumberDamageAction pNumberDamageAction = ob.GetComponent<NumberDamageAction>();
            pNumberDamageAction.Init(this, nDamage, color);
            m_NumberDamageActionPosTable.Add(pNumberDamageAction, vRandomPos);
        }

        base.ChangeSP(nSP);

        m_pPlane_Gauge_SP.m_rcTexRect.yMax = (float)m_nSP / m_nMaxSP;
        m_pPlane_Gauge_SP.UpdateUV();

        if (IsFull_SP() == false)
        {
            m_pPlane_Gauge_SP.gameObject.SetActive(true);
            m_pPlan_SP_Full_Effect.SetColor(new Color(1, 1, 1, 0));
        }
        else
        {
            m_pPlane_Gauge_SP.gameObject.SetActive(false);
        }
    }

    private Vector3 GetNumberDamageActionPos()
    {
        if (m_NumberDamageActionPosTable.Count > 0)
        {
            float fWidth = 128.0f * InGameInfo.Instance.m_fInGameScale * 0.5f;
            float fHeight = 128.0f * InGameInfo.Instance.m_fInGameScale * 0.25f;

            float x = UnityEngine.Random.Range(-fWidth, fWidth);
            float y = UnityEngine.Random.Range(-fHeight, fHeight);

            return new Vector3(x, y, 0);
        }

        return Vector3.zero;
    }

    public override void OnDamageText(eUnitDamage eString)
    {
        switch (eString)
        {
            case eUnitDamage.Strong:
                {
                    m_pTextMesh_DamageText.text = ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_CORRELATION_STRONG");
                    m_pTextMesh_DamageText.color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Strong];
                }
                break;
            case eUnitDamage.Weak:
                {
                    m_pTextMesh_DamageText.text = ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_CORRELATION_WEAK");
                    m_pTextMesh_DamageText.color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Weak];
                }
                break;
            case eUnitDamage.Hero_Missing:
                {
                    m_pTextMesh_DamageText.text = ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_CORRELATION_MISSING");
                    m_pTextMesh_DamageText.color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.Hero_Missing];
                }
                break;
            case eUnitDamage.HP_Recovery:
                {
                    m_pTextMesh_DamageText.text = ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_HP_RECOVERY");
                    m_pTextMesh_DamageText.color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.HP_Recovery];
                }
                break;
            case eUnitDamage.SP_Recharge:
                {
                    m_pTextMesh_DamageText.text = ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_SP_RECHARGE");
                    m_pTextMesh_DamageText.color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.SP_Recharge];
                }
                break;
            case eUnitDamage.SP_Decrease:
                {
                    m_pTextMesh_DamageText.text = ExcelDataHelper.GetString("ACTION_DAMAGE_TEXT_SP_DECREASE");
                    m_pTextMesh_DamageText.color = GameDefine.ms_UnitDamageColors[(int)eUnitDamage.SP_Decrease];
                }
                break;
        }

        m_pAlpha_DamageText.OnReset();
        TransformerEvent_Scalar eventValue;
        eventValue = new TransformerEvent_Scalar(0, 1);
        m_pAlpha_DamageText.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(1, 1);
        m_pAlpha_DamageText.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(1.2f, 0);
        m_pAlpha_DamageText.AddEvent(eventValue);
        m_pAlpha_DamageText.OnPlay();
    }

    public override void OnDamageText(string strText)
    {
        m_pTextMesh_DamageText.text = strText;

        m_pAlpha_DamageText.OnReset();
        TransformerEvent_Scalar eventValue;
        eventValue = new TransformerEvent_Scalar(0, 1);
        m_pAlpha_DamageText.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(1, 1);
        m_pAlpha_DamageText.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(1.2f, 0);
        m_pAlpha_DamageText.AddEvent(eventValue);
        m_pAlpha_DamageText.OnPlay();
    }

    public override void OnNumberDamageActionDone(NumberDamageAction pNumberDamageAction)
    {
        if (m_NumberDamageActionPosTable.ContainsKey(pNumberDamageAction) == true)
        {
            m_NumberDamageActionPosTable.Remove(pNumberDamageAction);
        }
    }

    public override void OnDead()
    {
        OnTransformUnitRestore();
        ChangeHP(0, new Color(1,1,1,0));
        m_nVirtualHP = 0;

        m_IsDie = true;
        m_pPlan_SP_Full_Effect.SetColor(new Color(1, 1, 1, 0));
        m_pGameObject_Die_Over.SetActive(true);

        if (m_pGameObject_Buff != null)
        {
            m_pGameObject_Buff.SetActive(false);
        }

        if (m_pGameObject_Debuff != null)
        {
            m_pGameObject_Debuff.SetActive(false);
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

    public override void OnCheckBuffDebuffMark()
    {
        if (m_IsDie == false)
        {
            if (m_pGameObject_Buff != null)
            {
                m_pGameObject_Buff.SetActive(m_pUnit_StatusEffect.IsExistBuff());
            }

            if (m_pGameObject_Debuff != null)
            {
                m_pGameObject_Debuff.SetActive(m_pUnit_StatusEffect.IsExistDebuff());
            }
        }
    }

    public void OnInGame_ChangeAutoPlay(bool IsAutoPlay)
    {
        InGameInfo.Instance.m_nClickPlayerCharacterFingerID = -1;
        InGameInfo.Instance.m_pClickPlayerCharacter = null;
    }

    public void OnInGame_CreateBlockMatchMissile(eElement eElem, eOwner eOwn)
    {
        if (m_pUnitInfo.m_eElement == eElem && GetOwner() == eOwn)
        {
            m_pParticleSystem_MatchDamageAttack.Play();
        }
    }

    public void SetApplyPassiveAction_ForOnce(bool IsApply)
    {
        m_IsApplyPassiveAction_ForOnce = IsApply;
    }

    public bool IsApplyPassiveAction_ForOnce()
    {
        return m_IsApplyPassiveAction_ForOnce;
    }

    public void Revival(int nHP)
    {
        m_IsDie = false;
        m_pGameObject_Die_Over.SetActive(false);
        ChangeSP(0);
        ChangeHP(nHP);
        ResetVirtualHP();
    }

    public void SetSkillFirstSelect(bool IsActive)
    {
        if (IsActive == true)
        {
            m_IsSkillFirstSelect = true;
            m_pGameObject_Die_Over.SetActive(true);
        }
        else
        {
            m_IsSkillFirstSelect = false;
            m_pGameObject_Die_Over.SetActive(m_IsDie);
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
            InGameInfo.Instance.m_pClickPlayerCharacter = this;
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
			if (IsDown == true && InGameInfo.Instance.m_pClickPlayerCharacter == this &&
				InGameInfo.Instance.m_nClickPlayerCharacterFingerID == nFingerID && InGameInfo.Instance.m_IsInGameClick == true)
			{
				bool IsValidClick = true;

                if (EspressoInfo.Instance.m_eActionTriggerSubject == eActionTriggerSubject.None)
                {
                    //if (m_IsDie == false && m_nMaxSP != 0 && m_pUnitInfo.m_nActiveSkill_ActionTableID != 0)
                    {
                        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

                        if (pDataStack.m_pPlayerCharacter_SkillTrigger == null)
                        {
                            IsValidClick = false;
                            pDataStack.m_pPlayerCharacter_SkillTrigger = this;
                            EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Open();
                            EventDelegateManager.Instance.OnInGame_Request_ActionTrigger_Open_ToGray();

                            if (m_IsDie == false && (m_nMaxSP == 0 || m_pUnitInfo.m_nActiveSkill_ActionTableID == 0))
                            {
                                GameObject ob_Ms = Resources.Load<GameObject>("GUI/Prefabs/Common/CommonMessageBox");
                                ob_Ms = GameObject.Instantiate(ob_Ms);
                                CommonMessageBox_UI pScript = ob_Ms.GetComponent<CommonMessageBox_UI>();
                                pScript.Init(ExcelDataHelper.GetString("ACTION_MESSAGE_NO_ACTIVE_SKILL"), 0, 0.7f, 0.2f);
                            }
                        }
                    }
                    //else if (m_IsDie == false && (m_nMaxSP == 0 || m_pUnitInfo.m_nActiveSkill_ActionTableID == 0))
                    //{
                    //    GameObject ob_Ms = Resources.Load<GameObject>("GUI/Prefabs/Common/CommonMessageBox");
                    //    ob_Ms = GameObject.Instantiate(ob_Ms);
                    //    CommonMessageBox_UI pScript = ob_Ms.GetComponent<CommonMessageBox_UI>();
                    //    pScript.Init(ExcelDataHelper.GetString("ACTION_MESSAGE_NO_ACTIVE_SKILL"), 0, 0.7f, 0.2f);
                    //}
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
}
