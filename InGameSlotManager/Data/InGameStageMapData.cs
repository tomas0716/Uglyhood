using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InGameStageMapMetaData
{
    public int              m_nMetaData             = 0;
}

[System.Serializable]
public class InGameStageMapUnitMetaData
{
    public int              m_nUnitLevel            = 0;
    public int              m_nUnitSkillLevel       = 0;
}


[System.Serializable]
public class InGameStageMapSlotData
{
    public int                          m_nX                            = 0;
    public int                          m_nY                            = 0;
    public eMapSlotItem                 m_eMapSlotItem                  = eMapSlotItem.Open;
    public InGameStageMapMetaData       m_pInGameStageMapMetaData       = new InGameStageMapMetaData();
    public InGameStageMapUnitMetaData   m_pInGameStageMapUnitMetaData   = new InGameStageMapUnitMetaData();
}

public class InGameStageMapData : ScriptableObject
{
	public List<InGameStageMapSlotData> m_InGameStageMapSlotDataList = new List<InGameStageMapSlotData>();

    public List<InGameStageMapSlotData> GetInGameStageMapSlotData(int nX, int nY)
    {
        List<InGameStageMapSlotData> dataList = new List<InGameStageMapSlotData>();

        foreach (InGameStageMapSlotData pInGameStageMapSlotData in m_InGameStageMapSlotDataList)
        {
            if (pInGameStageMapSlotData.m_nX == nX && pInGameStageMapSlotData.m_nY == nY)
            {
                dataList.Add(pInGameStageMapSlotData);
            }
        }

        return dataList;
    }

    public List<InGameStageMapSlotData> GetInGameStageMapSlotData(eMapSlotItem eItem)
    {
        List<InGameStageMapSlotData> dataList = new List<InGameStageMapSlotData>();

        foreach (InGameStageMapSlotData pInGameStageMapSlotData in m_InGameStageMapSlotDataList)
        {
            if (pInGameStageMapSlotData.m_eMapSlotItem == eItem)
            {
                dataList.Add(pInGameStageMapSlotData);
            }
        }

        return dataList;
    }

    public InGameStageMapSlotData AddInGameStageMapSlotData(int nX, int nY, eMapSlotItem eItem)
    {
        InGameStageMapSlotData pData = new InGameStageMapSlotData();
        pData.m_nX = nX;
        pData.m_nY = nY;
        pData.m_eMapSlotItem = eItem;
        m_InGameStageMapSlotDataList.Add(pData);

        return pData;
    }

    public InGameStageMapSlotData AddInGameStageMapSlotData(int nX, int nY, eMapSlotItem eItem, int nMetaData)
    {
        InGameStageMapSlotData pData = AddInGameStageMapSlotData(nX, nY, eItem);
        pData.m_pInGameStageMapMetaData.m_nMetaData = nMetaData;

        return pData;
    }

    public InGameStageMapSlotData AddInGameStageMapSlotData(int nX, int nY, eMapSlotItem eItem, int nMetaData, int nUnitLevel)
    {
        InGameStageMapSlotData pData = AddInGameStageMapSlotData(nX, nY, eItem);
        pData.m_pInGameStageMapMetaData.m_nMetaData = nMetaData;
        pData.m_pInGameStageMapUnitMetaData.m_nUnitLevel = nUnitLevel;

        return pData;
    }

    public void RemvoeInGameStageMapSlotData(int nX, int nY, eMapSlotItem eItem)
    {
        for(int i = m_InGameStageMapSlotDataList.Count - 1; i >= 0; --i)
        {
            InGameStageMapSlotData pInGameStageMapSlotData = m_InGameStageMapSlotDataList[i];
            if (pInGameStageMapSlotData.m_nX == nX && pInGameStageMapSlotData.m_nY == nY && pInGameStageMapSlotData.m_eMapSlotItem == eItem)
            {
                m_InGameStageMapSlotDataList.Remove(pInGameStageMapSlotData);
                return;
            }
        }

        return;
    }

    public bool IsInGameStageMapSlotData(int nX, int nY, eMapSlotItem eItem)
    {
        foreach (InGameStageMapSlotData pInGameStageMapSlotData in m_InGameStageMapSlotDataList)
        {
            if (pInGameStageMapSlotData.m_nX == nX && pInGameStageMapSlotData.m_nY == nY && pInGameStageMapSlotData.m_eMapSlotItem == eItem)
            {
                return true;
            }
        }

        return false;
    }

    public void ClearInGameStageMapSlotData(int nX, int nY)
    {
        List<InGameStageMapSlotData> dataList = GetInGameStageMapSlotData(nX, nY);

        foreach (InGameStageMapSlotData pInGameStageMapSlotData in dataList)
        {
            m_InGameStageMapSlotDataList.Remove(pInGameStageMapSlotData);
        }
    }
}
