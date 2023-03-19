using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot_Empty : MonoBehaviour
{
    private Slot[]              m_pNeighborSlots            = new Slot[(int)eNeighbor.eMax];
    private List<Plane2D>       m_PlaneEdgeList             = new List<Plane2D>();

    public void OnDestroy()
    {
        m_PlaneEdgeList.Clear();
    }

    public void Update()
    {
    }

    public void SetNeighbor(eNeighbor eNe, Slot pSlot)
    {
        m_pNeighborSlots[(int)eNe] = pSlot;
    }

    public Slot GetNeighborSlot(eNeighbor eNe)
    {
        return m_pNeighborSlots[(int)eNe];
    }

    public void CreateEdge()
    {
        switch (InGameSetting.Instance.m_pInGameSettingData.m_eBlockFillDirection)
        {
            case eBlockFillDirection.UpToBottom:
                {
                    CreateEdge_Top_Left(eNeighbor.Neighbor_10, eNeighbor.Neighbor_01);
                    CreateEdge_Top_Right(eNeighbor.Neighbor_10, eNeighbor.Neighbor_21);
                    CreateEdge_Bottom_Left(eNeighbor.Neighbor_12, eNeighbor.Neighbor_01);
                    CreateEdge_Bottom_Right(eNeighbor.Neighbor_12, eNeighbor.Neighbor_21);
                    CreateEdge_Left_Top(eNeighbor.Neighbor_01, eNeighbor.Neighbor_10);
                    CreateEdge_Left_Bottom(eNeighbor.Neighbor_01, eNeighbor.Neighbor_12);
                    CreateEdge_Right_Top(eNeighbor.Neighbor_21, eNeighbor.Neighbor_10);
                    CreateEdge_Right_Bottom(eNeighbor.Neighbor_21, eNeighbor.Neighbor_12);

                    CreateEdge_LeftTop_Corner(eNeighbor.Neighbor_00, eNeighbor.Neighbor_10, eNeighbor.Neighbor_01);
                    CreateEdge_RightTop_Corner(eNeighbor.Neighbor_20, eNeighbor.Neighbor_10, eNeighbor.Neighbor_21);
                    CreateEdge_LeftBottom_Corner(eNeighbor.Neighbor_02, eNeighbor.Neighbor_01, eNeighbor.Neighbor_12);
                    CreateEdge_RightBottom_Corner(eNeighbor.Neighbor_22, eNeighbor.Neighbor_21, eNeighbor.Neighbor_12);

                    CreateEdge_LeftTop(eNeighbor.Neighbor_10, eNeighbor.Neighbor_01);
                    CreateEdge_RightTop(eNeighbor.Neighbor_10, eNeighbor.Neighbor_21);
                    CreateEdge_LeftBottom(eNeighbor.Neighbor_01, eNeighbor.Neighbor_12);
                    CreateEdge_RightBottom(eNeighbor.Neighbor_21, eNeighbor.Neighbor_12);
                }
                break;

            case eBlockFillDirection.BottomToUp:
                {
                    CreateEdge_Top_Left(eNeighbor.Neighbor_12, eNeighbor.Neighbor_21);
                    CreateEdge_Top_Right(eNeighbor.Neighbor_12, eNeighbor.Neighbor_01);
                    CreateEdge_Bottom_Left(eNeighbor.Neighbor_10, eNeighbor.Neighbor_21);
                    CreateEdge_Bottom_Right(eNeighbor.Neighbor_10, eNeighbor.Neighbor_01);
                    CreateEdge_Left_Top(eNeighbor.Neighbor_21, eNeighbor.Neighbor_12);
                    CreateEdge_Left_Bottom(eNeighbor.Neighbor_21, eNeighbor.Neighbor_10);
                    CreateEdge_Right_Top(eNeighbor.Neighbor_01, eNeighbor.Neighbor_12);
                    CreateEdge_Right_Bottom(eNeighbor.Neighbor_01, eNeighbor.Neighbor_10);

                    CreateEdge_LeftTop_Corner(eNeighbor.Neighbor_22, eNeighbor.Neighbor_12, eNeighbor.Neighbor_21);
                    CreateEdge_RightTop_Corner(eNeighbor.Neighbor_02, eNeighbor.Neighbor_12, eNeighbor.Neighbor_01);
                    CreateEdge_LeftBottom_Corner(eNeighbor.Neighbor_20, eNeighbor.Neighbor_21, eNeighbor.Neighbor_10);
                    CreateEdge_RightBottom_Corner(eNeighbor.Neighbor_00, eNeighbor.Neighbor_01, eNeighbor.Neighbor_10);

                    CreateEdge_LeftTop(eNeighbor.Neighbor_12, eNeighbor.Neighbor_21);
                    CreateEdge_RightTop(eNeighbor.Neighbor_12, eNeighbor.Neighbor_01);
                    CreateEdge_LeftBottom(eNeighbor.Neighbor_21, eNeighbor.Neighbor_10);
                    CreateEdge_RightBottom(eNeighbor.Neighbor_01, eNeighbor.Neighbor_10);
				}
                break;

            case eBlockFillDirection.LeftToRight:
                {
                    CreateEdge_Top_Left(eNeighbor.Neighbor_21, eNeighbor.Neighbor_10);
                    CreateEdge_Top_Right(eNeighbor.Neighbor_21, eNeighbor.Neighbor_12);
                    CreateEdge_Bottom_Left(eNeighbor.Neighbor_01, eNeighbor.Neighbor_10);
                    CreateEdge_Bottom_Right(eNeighbor.Neighbor_01, eNeighbor.Neighbor_12);
                    CreateEdge_Left_Top(eNeighbor.Neighbor_10, eNeighbor.Neighbor_21);
                    CreateEdge_Left_Bottom(eNeighbor.Neighbor_10, eNeighbor.Neighbor_01);
                    CreateEdge_Right_Top(eNeighbor.Neighbor_12, eNeighbor.Neighbor_21);
                    CreateEdge_Right_Bottom(eNeighbor.Neighbor_12, eNeighbor.Neighbor_01);
                    
                    CreateEdge_LeftTop_Corner(eNeighbor.Neighbor_20, eNeighbor.Neighbor_10, eNeighbor.Neighbor_21);
                    CreateEdge_RightTop_Corner(eNeighbor.Neighbor_22, eNeighbor.Neighbor_21, eNeighbor.Neighbor_12);
                    CreateEdge_LeftBottom_Corner(eNeighbor.Neighbor_00, eNeighbor.Neighbor_10, eNeighbor.Neighbor_01);
                    CreateEdge_RightBottom_Corner(eNeighbor.Neighbor_02, eNeighbor.Neighbor_01, eNeighbor.Neighbor_12);

                    CreateEdge_LeftTop(eNeighbor.Neighbor_10, eNeighbor.Neighbor_21);
                    CreateEdge_RightTop(eNeighbor.Neighbor_21, eNeighbor.Neighbor_12);
                    CreateEdge_LeftBottom(eNeighbor.Neighbor_10, eNeighbor.Neighbor_01);
                    CreateEdge_RightBottom(eNeighbor.Neighbor_01, eNeighbor.Neighbor_12);
                }
                break;

            case eBlockFillDirection.RightToLeft:
                {
                    CreateEdge_Top_Left(eNeighbor.Neighbor_01, eNeighbor.Neighbor_12);
                    CreateEdge_Top_Right(eNeighbor.Neighbor_01, eNeighbor.Neighbor_10);
                    CreateEdge_Bottom_Left(eNeighbor.Neighbor_21, eNeighbor.Neighbor_12);
                    CreateEdge_Bottom_Right(eNeighbor.Neighbor_21, eNeighbor.Neighbor_10);
                    CreateEdge_Left_Top(eNeighbor.Neighbor_12, eNeighbor.Neighbor_01);
                    CreateEdge_Left_Bottom(eNeighbor.Neighbor_12, eNeighbor.Neighbor_21);
                    CreateEdge_Right_Top(eNeighbor.Neighbor_10, eNeighbor.Neighbor_01);
                    CreateEdge_Right_Bottom(eNeighbor.Neighbor_10, eNeighbor.Neighbor_21);

                    CreateEdge_LeftTop_Corner(eNeighbor.Neighbor_02, eNeighbor.Neighbor_01, eNeighbor.Neighbor_12);
                    CreateEdge_RightTop_Corner(eNeighbor.Neighbor_00, eNeighbor.Neighbor_10, eNeighbor.Neighbor_01);
                    CreateEdge_LeftBottom_Corner(eNeighbor.Neighbor_22, eNeighbor.Neighbor_21, eNeighbor.Neighbor_12);
                    CreateEdge_RightBottom_Corner(eNeighbor.Neighbor_20, eNeighbor.Neighbor_10, eNeighbor.Neighbor_21);

                    CreateEdge_LeftTop(eNeighbor.Neighbor_01, eNeighbor.Neighbor_12);
                    CreateEdge_RightTop(eNeighbor.Neighbor_10, eNeighbor.Neighbor_01);
                    CreateEdge_LeftBottom(eNeighbor.Neighbor_21, eNeighbor.Neighbor_12);
                    CreateEdge_RightBottom(eNeighbor.Neighbor_10, eNeighbor.Neighbor_21);
                }
                break;
        }
    }

    private void CreateEdge_Top_Left(eNeighbor neighbor_01, eNeighbor neighbor_02)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] == null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_Top");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_Top");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(-InGameInfo.Instance.m_fSlotSize * 0.25f, InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }

    private void CreateEdge_Top_Right(eNeighbor neighbor_01, eNeighbor neighbor_02)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] == null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_Top");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_Top");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(InGameInfo.Instance.m_fSlotSize * 0.25f, InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }

    private void CreateEdge_Bottom_Left(eNeighbor neighbor_01, eNeighbor neighbor_02)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] == null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_Bottom");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
            
            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_Bottom");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(-InGameInfo.Instance.m_fSlotSize * 0.25f, -InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }

    private void CreateEdge_Bottom_Right(eNeighbor neighbor_01, eNeighbor neighbor_02)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] == null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_Bottom");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;
            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_Bottom");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(InGameInfo.Instance.m_fSlotSize * 0.25f, -InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }

    private void CreateEdge_Left_Top(eNeighbor neighbor_01, eNeighbor neighbor_02)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] == null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_Left");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_Left");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(-InGameInfo.Instance.m_fSlotSize * 0.25f, InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }

    private void CreateEdge_Left_Bottom(eNeighbor neighbor_01, eNeighbor neighbor_02)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] == null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_Left");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_Left");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(-InGameInfo.Instance.m_fSlotSize * 0.25f, -InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }

    private void CreateEdge_Right_Top(eNeighbor neighbor_01, eNeighbor neighbor_02)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] == null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_Right");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_Right");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(InGameInfo.Instance.m_fSlotSize * 0.25f, InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }

    private void CreateEdge_Right_Bottom(eNeighbor neighbor_01, eNeighbor neighbor_02)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] == null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_Right");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_Right");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(InGameInfo.Instance.m_fSlotSize * 0.25f, -InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }

    private void CreateEdge_LeftTop_Corner(eNeighbor neighbor_01, eNeighbor neighbor_02, eNeighbor neighbor_03)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] == null && m_pNeighborSlots[(int)neighbor_03] == null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_LeftTop_Corner");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_LeftTop_Corner");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(-InGameInfo.Instance.m_fSlotSize * 0.25f, InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }

    private void CreateEdge_RightTop_Corner(eNeighbor neighbor_01, eNeighbor neighbor_02, eNeighbor neighbor_03)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] == null && m_pNeighborSlots[(int)neighbor_03] == null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_RightTop_Corner");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_RightTop_Corner");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(InGameInfo.Instance.m_fSlotSize * 0.25f, InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }

    private void CreateEdge_LeftBottom_Corner(eNeighbor neighbor_01, eNeighbor neighbor_02, eNeighbor neighbor_03)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] == null && m_pNeighborSlots[(int)neighbor_03] == null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_LeftBottom_Corner");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_LeftBottom_Corner");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(-InGameInfo.Instance.m_fSlotSize * 0.25f, -InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }

    private void CreateEdge_RightBottom_Corner(eNeighbor neighbor_01, eNeighbor neighbor_02, eNeighbor neighbor_03)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] == null && m_pNeighborSlots[(int)neighbor_03] == null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_RightBottom_Corner");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_RightBottom_Corner");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(InGameInfo.Instance.m_fSlotSize * 0.25f, -InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }

    private void CreateEdge_LeftTop(eNeighbor neighbor_01, eNeighbor neighbor_02)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] != null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_LeftTop");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_LeftTop");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(-InGameInfo.Instance.m_fSlotSize * 0.25f, InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }

    private void CreateEdge_RightTop(eNeighbor neighbor_01, eNeighbor neighbor_02)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] != null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_RightTop");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_RightTop");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(InGameInfo.Instance.m_fSlotSize * 0.25f, InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }

    private void CreateEdge_LeftBottom(eNeighbor neighbor_01, eNeighbor neighbor_02)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] != null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_LeftBottom");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_LeftBottom");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(-InGameInfo.Instance.m_fSlotSize * 0.25f, -InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }

    private void CreateEdge_RightBottom(eNeighbor neighbor_01, eNeighbor neighbor_02)
    {
        if (m_pNeighborSlots[(int)neighbor_01] != null && m_pNeighborSlots[(int)neighbor_02] != null)
        {
            ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
            GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Slot_Edge_RightBottom");
            ob = GameObject.Instantiate(ob);
            ob.transform.SetParent(gameObject.transform);
            ob.transform.localPosition = Vector3.zero;
            ob.transform.localScale *= InGameInfo.Instance.m_fInGameScale;

            Plane2D pPlane = ob.GetComponent<Plane2D>();
            Helper.Change_Plane2D_AtlasInfo(pPlane, "Slot_" + pStageInfo.m_strStageTheme, "Edge_RightBottom");
            pPlane.SetLayerName("Slot");

            pPlane.SetPosition(new Vector2(InGameInfo.Instance.m_fSlotSize * 0.25f, -InGameInfo.Instance.m_fSlotSize * 0.25f));
            m_PlaneEdgeList.Add(pPlane);
        }
    }
}
