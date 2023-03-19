using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotFixObject_EnemyColony : SlotFixObject_Espresso
{
    private GameObject                  m_pGameObject       = null;
    private int                         m_nSlotIndex        = 0;
    protected ExcelData_EnemyColonyInfo m_pEnemyColonyInfo  = null;

    public SlotFixObject_EnemyColony(Slot pSlot, int nSlotIndex, ExcelData_EnemyColonyInfo pEnemyColonyInfo, int nHP) : base(pSlot, eObjectType.EnemyColony, eOwner.Other)
    {
        ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);

        m_pSlot = pSlot;
        m_nSlotIndex = nSlotIndex;
        m_pEnemyColonyInfo = pEnemyColonyInfo;
        m_nMaxHP = nHP;
        m_nHP = nHP;

        GameObject ob;

        ob = Resources.Load<GameObject>("2D/Prefabs/SlotObject/" + pEnemyColonyInfo.m_strResourceFileName);
        m_pGameObject = GameObject.Instantiate(ob);
        m_pGameObject.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
        Plane2D pPlane2D = m_pGameObject.GetComponent<Plane2D>();
        Material pMaterial = AppInstance.Instance.m_pMaterialResourceManager.LoadMaterial("Image/EnemyColony/Materials/",  "Block_EnemyColony_" + pStageInfo.m_strStageTheme);
        if (pMaterial != null)
        {
            pPlane2D.ChangeMaterial(pMaterial);
        }

        EnemyColony_Base pEnemyColoy = m_pGameObject.GetComponent<EnemyColony_Base>();
        pEnemyColoy.Init(m_pSlot, pEnemyColonyInfo);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (m_pGameObject != null)
        {
            // 승리, 패배 텍스트 출력 이후 함수 호출 X

            //if(InGameInfo.Instance.m_eCurrGameResult == eGameResult.None)
            //{
            //    ColonyToGold();
            //}

            if (!EspressoInfo.Instance.m_IsResultTextShowDone)
            {
                ColonyToGold();
            }

            GameObject.Destroy(m_pGameObject);
            m_pGameObject = null;
        }
    }

    public override void Update(float fDeltaTime)
    {
    }

    public override GameObject GetGameObject()
    {
        return m_pGameObject;
    }

    public override void SetPosition(Vector2 vPos) 
    {
        m_pGameObject.transform.position = new Vector3(vPos.x, vPos.y, 0);
    }

    public override void OnDead()
    {
        // 일단 바로 죽임
        m_pSlot.RemoveSlotFixObject(this);
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

    public void ColonyToGold()
    {
        GameObject ob_InGameUI = EventDelegateManager.Instance.OnInGame_GetInGameUIGameObject();
        if (ob_InGameUI != null)
        {
            GameObject ob = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGame_Gold");
            ob = GameObject.Instantiate(ob, Helper.FindChildGameObject(ob_InGameUI, "SafeArea").transform);
            ob.transform.position = m_pGameObject.transform.position;
            ob.transform.position += EventDelegateManager.Instance.OnInGame_GetInGameUIGameObject().transform.position;
        }
    }

    public void ColonyToGold(float nTime)
    {
        if (m_pGameObject != null && m_pGameObject.transform != null)
        {
            GameObject ob = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGame_Gold");
            ob = GameObject.Instantiate(ob, Helper.FindChildGameObject(EventDelegateManager.Instance.OnInGame_GetInGameUIGameObject(), "SafeArea").transform);
            ob.transform.position = m_pGameObject.transform.position;
            ob.transform.position += EventDelegateManager.Instance.OnInGame_GetInGameUIGameObject().transform.position;
            ob.GetComponent<InGame_ColonyToGold>().SetTimeToDestroy(this, nTime);
        }
    }
}
