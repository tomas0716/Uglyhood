using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eShapeData
{
    None,
    Normal,
    Extension,

    Max,
}

[System.Serializable]
public class SpecialItemCustomize
{
    public int      m_nNumGrid  = 0;
    public bool[]   m_Shapes;

    public SpecialItemCustomize(int nNumGrid)
    {
        m_nNumGrid = nNumGrid;

        if (m_nNumGrid == 0)
        {
            m_Shapes = null;
        }
        else
        {
            m_Shapes = new bool[m_nNumGrid * m_nNumGrid];
        }
    }
}

public class ShapeNode : Node
{
    public eShapeData   m_eShapeData            = eShapeData.Normal;
    public Vector2Int   m_vCheckPoint           = Vector2Int.zero;
    public eNeighbor    m_eNeighbor_ToParent    = eNeighbor.Neighbor_None;

    public ShapeNode(Vector2Int vCheckPoint, eNeighbor eNeighbor, eShapeData eShapeData)
    {
        m_eShapeData = eShapeData;
        m_vCheckPoint = vCheckPoint;
        m_eNeighbor_ToParent = eNeighbor;
    }

    public static ShapeNode FindNodeInChild(ShapeNode pNode, int nX, int nY)
    {
        if (nX == pNode.m_vCheckPoint.x && nY == pNode.m_vCheckPoint.y)
            return pNode;

        return RecursiveFindNodeInChild(pNode, nX, nY);
    }

    private static ShapeNode RecursiveFindNodeInChild(ShapeNode pNode, int nX, int nY)
    {
        int nNum = pNode.GetChildNodeCount();

        for (int i = 0; i < nNum; ++i)
        {
            ShapeNode pFindChild = pNode.GetChildNode_byIndex(i) as ShapeNode;

            if (pFindChild.m_vCheckPoint.x == nX && pFindChild.m_vCheckPoint.y == nY)
            {
                return pFindChild;
            }

            pFindChild = RecursiveFindNodeInChild(pFindChild, nX, nY);

            if (pFindChild != null)
            {
                return pFindChild;
            }
        }

        return null;

    }
}

[System.Serializable]
public class CombinationShapeArray
{
    // Tool Data
    public int[]            m_Shape             = null;
    public Vector2Int       m_vGrid             = new Vector2Int(9, 9);

    // Client Data
    public int[,]           m_ShapeArray        = null;
    public ShapeNode        m_pShapeNode        = null;

    // Etc Data
    public bool             m_IsValid           = false;

    public CombinationShapeArray(Vector2Int vGrid)
    {
        m_vGrid = vGrid;
        m_Shape = new int[vGrid.x * vGrid.y];
    }

    public void ChangeGrid(Vector2Int vPrevGrid, Vector2Int vGrid)
    {
        m_vGrid = vGrid;

        int[] pTempShape = (int[])m_Shape.Clone();
        m_Shape = new int[vGrid.x * vGrid.y];

        for (int y = 0; y < vPrevGrid.y; ++y)
        {
            if (vGrid.y > y)
            {
                for (int x = 0; x < vPrevGrid.x; ++x)
                {
                    if (vGrid.x > x)
                    {
                        int nPrevIndex = y * vPrevGrid.x + x;
                        int nIndex = y * vGrid.x + x;

                        m_Shape[nIndex] = pTempShape[nPrevIndex];
                    }
                }
            }
        }
    }

    public void CalcuBoundary()
    {
        for (int i = 0; i < m_Shape.Length; ++i)
        {
            if (m_Shape[i] == (int)eShapeData.Normal)
            {
                m_IsValid = true;
            }
        }

        if (m_IsValid == true)
        {
            bool IsFindCheckPoint = false;

            m_ShapeArray = new int[m_vGrid.x, m_vGrid.y];

            for (int y = m_vGrid.y - 1; y >= 0; --y)
            {
                for (int x = m_vGrid.x - 1; x >= 0; --x)
                {
                    int nIndex = y * m_vGrid.x + x;

                    m_ShapeArray[x, y] = m_Shape[nIndex];

                    if (m_ShapeArray[x, y] == (int)eShapeData.Normal && IsFindCheckPoint == false)
                    {
                        IsFindCheckPoint = true;
                        m_pShapeNode = new ShapeNode(new Vector2Int(x, y), eNeighbor.Neighbor_None, eShapeData.Normal);
                    }
                }
            }

            RecursiveFindNode(m_pShapeNode);
        }
    }

    private void RecursiveFindNode(ShapeNode pNode)
    {
        int nX = pNode.m_vCheckPoint.x;
        int nY = pNode.m_vCheckPoint.y;

        // 위
        if (nY - 1 >= 0 && m_ShapeArray[nX, nY - 1] != 0 && ShapeNode.FindNodeInChild(m_pShapeNode, nX, nY - 1) == null)
        {
            ShapeNode pChildNode = new ShapeNode(new Vector2Int(nX, nY - 1), eNeighbor.Neighbor_10, (eShapeData)m_ShapeArray[nX, nY - 1]);
            pNode.AddChild(pChildNode);
            RecursiveFindNode(pChildNode);
        }

        // 왼쪽
        if (nX - 1 >= 0 && m_ShapeArray[nX - 1, nY] != 0 && ShapeNode.FindNodeInChild(m_pShapeNode, nX - 1, nY) == null)
        {
            ShapeNode pChildNode = new ShapeNode(new Vector2Int(nX - 1, nY), eNeighbor.Neighbor_01, (eShapeData)m_ShapeArray[nX - 1, nY]);
            pNode.AddChild(pChildNode);
            RecursiveFindNode(pChildNode);
        }

        // 오른쪽
        if (nX + 1 < m_vGrid.x && m_ShapeArray[nX + 1, nY] != 0 && ShapeNode.FindNodeInChild(m_pShapeNode, nX + 1, nY) == null)
        {
            ShapeNode pChildNode = new ShapeNode(new Vector2Int(nX + 1, nY), eNeighbor.Neighbor_21, (eShapeData)m_ShapeArray[nX + 1, nY]);
            pNode.AddChild(pChildNode);
            RecursiveFindNode(pChildNode);
        }

        // 아래
        if (nY + 1 < m_vGrid.y && m_ShapeArray[nX, nY + 1] != 0 && ShapeNode.FindNodeInChild(m_pShapeNode, nX, nY + 1) == null)
        {
            ShapeNode pChildNode = new ShapeNode(new Vector2Int(nX, nY + 1), eNeighbor.Neighbor_12, (eShapeData)m_ShapeArray[nX, nY + 1]);
            pNode.AddChild(pChildNode);
            RecursiveFindNode(pChildNode);
        }
    }
}

[System.Serializable]
public class CombinationShapeData
{
    public int                          m_nIndex                        = 0;
    public GameObject []                m_pGameObject                   = null;
    public bool                         m_IsIgnoreBlockType             = false;
    public int                          m_nSearchOrder                  = 0;
    public List<CombinationShapeArray>  m_CombinationShapeArrayList     = new List<CombinationShapeArray>();
    public eSpecialItem                 m_eSpcialItem                   = eSpecialItem.None;
    public SpecialItemCustomize         m_pSpecialItemCustomize         = new SpecialItemCustomize(0);

    public string                       m_strTag                        = "Tag";

    public CombinationShapeData(int nBlockCount)
    {
        m_pGameObject = new GameObject[(int)GameDefine.ms_nBlockStart + nBlockCount];
    }

    public void ChangeBlockCount(int nPrevBlockCount, int nBlockCount)
    {
        GameObject [] pTempGameObject = (GameObject[])m_pGameObject.Clone();
        m_pGameObject = new GameObject[(int)eBlockType.Block_Start + nBlockCount];

        int nCopyCount = nPrevBlockCount < nBlockCount ? nPrevBlockCount : nBlockCount;
        for (int i = 0; i < nCopyCount + (int)eBlockType.Block_Start; ++i)
        {
            m_pGameObject[i] = pTempGameObject[i];
        }
    }

    public void ChangeGrid(Vector2Int vPrevGrid, Vector2Int vGrid)
    {
        foreach (CombinationShapeArray pShapeArray in m_CombinationShapeArrayList)
        {
            pShapeArray.ChangeGrid(vPrevGrid, vGrid);
        }
    }

    public void CalcuBoundary()
    {
        foreach (CombinationShapeArray pShapeArray in m_CombinationShapeArrayList)
        {
            pShapeArray.CalcuBoundary();
        }
    }
}

[System.Serializable]
public class SpecialItemCombinationData
{
    public string                   m_strTag_01                 = "";
    public eSpecialItem             m_eSpecialItem_01           = eSpecialItem.None;

    public string                   m_strTag_02                 = "";
    public eSpecialItem             m_eSpecialItem_02           = eSpecialItem.None;

    public eSpecialItem             m_eSpecialItem_Value        = eSpecialItem.None;

    public SpecialItemCustomize     m_pSpecialItemCustomize     = new SpecialItemCustomize(0);

    public List<eSpecialItem>       m_MatchColorTransList       = new List<eSpecialItem>();
    public List<SpecialItemCustomize> m_MatchColorTransSpecialItemCustomizeList = new List<SpecialItemCustomize>();
}

public class InGameSettingData : ScriptableObject
{
    public eBlockFillDirection              m_eBlockFillDirection               = eBlockFillDirection.UpToBottom;
    public bool                             m_IsAnytimeSwap                     = false;
    public eSpecialItemUseWay               m_eSpecialItemUseWay                = eSpecialItemUseWay.Match;
    public bool                             m_IsPossibleEmptySlotSwap           = true;
    public int                              m_nBlockCount                       = 5;
    public Vector2Int                       m_vGrid                             = new Vector2Int(7,7);
    public bool                             m_IsSlotFrame                       = true;

    public List<CombinationShapeData>       m_CombinationShapeList              = new List<CombinationShapeData>();
    public List<SpecialItemCombinationData> m_SpecialItemCombinationDataList    = new List<SpecialItemCombinationData>();

    public void ChangeBlockCount(int nPrevBlockCount)
    {
        foreach (CombinationShapeData pShapeData in m_CombinationShapeList)
        {
            pShapeData.ChangeBlockCount(nPrevBlockCount, m_nBlockCount);
        }
    }

    public void ChangeGrid(Vector2Int vPrevGrid)
    {
        foreach (CombinationShapeData pShapeData in m_CombinationShapeList)
        {
            pShapeData.ChangeGrid(vPrevGrid, m_vGrid);
        }
    }

    public void CalcuBoundary()
    {
        int nIndex = 0;
        foreach (CombinationShapeData pShapeData in m_CombinationShapeList)
        {
            pShapeData.m_nIndex = nIndex++;
            pShapeData.CalcuBoundary();
        }
    }

    public CombinationShapeData GetCombinationShapeData_Base()
    {
        if(m_CombinationShapeList.Count != 0)
            return m_CombinationShapeList[0];

        return null;
    }

    public CombinationShapeData GetCombinationShapeData(eSpecialItem eSpecialItem)
    {
        for(int i = 1; i < m_CombinationShapeList.Count; ++i)
        {
            CombinationShapeData pShapeData = m_CombinationShapeList[i];

            if (pShapeData.m_eSpcialItem == eSpecialItem)
            {
                return pShapeData;
            }
        }

        return null;
    }

    public SpecialItemCombinationData GetSpecialItemCombinationData(eSpecialItem eSpecialItem_01, eSpecialItem eSpecialItem_02)
    {
        foreach (SpecialItemCombinationData pCombinationData in m_SpecialItemCombinationDataList)
        {
            if ((pCombinationData.m_eSpecialItem_01 == eSpecialItem_01 && pCombinationData.m_eSpecialItem_02 == eSpecialItem_02) ||
                (pCombinationData.m_eSpecialItem_01 == eSpecialItem_02 && pCombinationData.m_eSpecialItem_02 == eSpecialItem_01))
            {
                return pCombinationData;
            }
        }

        return null;
    }

    public SpecialItemCombinationData GetSpecialItemCombinationData(string strTag_01, string strTag_02)
    {
        foreach (SpecialItemCombinationData pCombinationData in m_SpecialItemCombinationDataList)
        {
            if ((pCombinationData.m_strTag_01 == strTag_01 && pCombinationData.m_strTag_02 == strTag_02) ||
                (pCombinationData.m_strTag_01 == strTag_02 && pCombinationData.m_strTag_02 == strTag_01))
            {
                return pCombinationData;
            }
        }

        return null;
    }
}
