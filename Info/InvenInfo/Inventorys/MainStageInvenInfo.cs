using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterStarReward
{
    public int m_nChapterID;
    public bool m_IsOpen;
    public bool m_IsClear;
    public int m_nStarReward;
    public int m_nMissionReward;
}

public class MainStageInvenInfo
{
    private Dictionary<int, bool>   m_pChapterUnlockList        = new Dictionary<int, bool>();
    private Dictionary<int, bool>   m_pStageUnlockList          = new Dictionary<int, bool>();
    private Dictionary<int, int>    m_pStageClearRankList       = new Dictionary<int, int>();
    private Dictionary<int, int>    m_pStageMissionClearRankList = new Dictionary<int, int>();
    private Dictionary<int, bool>   m_pStageLimitRewardList     = new Dictionary<int, bool>();
    private Dictionary<int, ChapterStarReward> m_pChapterStarReward = new Dictionary<int, ChapterStarReward>();
    private Dictionary<int, int> m_pStarCount_byChapterID = new Dictionary<int, int>();
    private Dictionary<int, int> m_pChallengeStarCount_byChapterID = new Dictionary<int, int>();

    public MainStageInvenInfo()
    {
    }

    public void ClearAll()
    {
        m_pChapterUnlockList.Clear();
        m_pStageUnlockList.Clear();
        m_pStageClearRankList.Clear();
        m_pStageLimitRewardList.Clear();
        m_pStageMissionClearRankList.Clear();
    }

    //public void Init()
    //{
    //    Dictionary<int, ExcelData_ChapterInfo> pChapterInfo = new Dictionary<int, ExcelData_ChapterInfo>();
    //    pChapterInfo = ExcelDataManager.Instance.m_pChapter.GetAllChapterInfoList();

    //    foreach(KeyValuePair<int, ExcelData_ChapterInfo> pInfo in pChapterInfo)
    //    {
    //        m_pChapterUnlockList.Add(pInfo.Key, false);
    //    }

    //    Dictionary<int, ExcelData_StageInfo> pStageInfo = new Dictionary<int, ExcelData_StageInfo>();
    //    pStageInfo = ExcelDataManager.Instance.m_pStage.GetAllStageInfoList();

    //    foreach(KeyValuePair<int, ExcelData_StageInfo> pInfo in pStageInfo)
    //    {
    //        m_pStageUnlockList.Add(pInfo.Key, false);
    //        m_pStageClearRankList.Add(pInfo.Key, -1);
    //        m_pStageLimitRewardList.Add(pInfo.Key, false);
    //    }

    //    // ?? ????, ?? ???????? ????

    //    m_pChapterUnlockList[101] = true;

    //    m_pStageUnlockList[10101] = true;

    //    SaveDataInfo.Instance.MainStageInfoSave(m_pChapterUnlockList, m_pStageUnlockList, m_pStageClearRankList, m_pStageLimitRewardList);
    //}

    //public void Load(Dictionary<int,bool> pChapterUnlockList, Dictionary<int,bool> pStageUnlockList, Dictionary<int, int> pStageClearRank, Dictionary<int,bool> pStageLimitReward)
    //{
    //    m_pChapterUnlockList = pChapterUnlockList;
    //    m_pStageUnlockList = pStageUnlockList;
    //    m_pStageClearRankList = pStageClearRank;
    //    m_pStageLimitRewardList = pStageLimitReward;
    //}

    public void Save()
    {
        //SaveDataInfo.Instance.MainStageInfoSave(m_pChapterUnlockList, m_pStageUnlockList, m_pStageClearRankList, m_pStageLimitRewardList);
    }

    //public void SetChapterUnlock_byChapter(int nChapter)
    //{
    //    m_pChapterUnlockList[nChapter] = true;

    //    SaveDataInfo.Instance.MainStageInfoSave(m_pChapterUnlockList, m_pStageUnlockList, m_pStageClearRankList, m_pStageLimitRewardList);
    //}

    public void SetChapterUnlock_byChapterID(int nChapterID, bool IsUnlock)
    {
        if (m_pChapterUnlockList.ContainsKey(nChapterID))
        {
            m_pChapterUnlockList[nChapterID] = IsUnlock;
        }
        else
        {
            m_pChapterUnlockList.Add(nChapterID, IsUnlock);
        }
    }

    //public void SetStageUnlock_byStage(int nStage)
    //{
    //    m_pStageUnlockList[nStage] = true;

    //    SaveDataInfo.Instance.MainStageInfoSave(m_pChapterUnlockList, m_pStageUnlockList, m_pStageClearRankList, m_pStageLimitRewardList);
    //}

    public void SetStageUnlock_byStageID(int nStageID, bool IsUnlock)
    {
        if (m_pStageUnlockList.ContainsKey(nStageID))
        {
            m_pStageUnlockList[nStageID] = IsUnlock;
        }
        else
        {
            m_pStageUnlockList.Add(nStageID, IsUnlock);
        }
    }

    public void SetStageClearRank_byStage(int nStage, int nRank)
    {
        if (m_pStageClearRankList.ContainsKey(nStage))
        {
            m_pStageClearRankList[nStage] = nRank;
        }
        else
        {
            m_pStageClearRankList.Add(nStage, nRank);
        }

        //SaveDataInfo.Instance.MainStageInfoSave(m_pChapterUnlockList, m_pStageUnlockList, m_pStageClearRankList, m_pStageLimitRewardList);
    }

    public void SetStageMissionClearRank_byStage(int nStage, int nRank)
    {
        if (m_pStageMissionClearRankList.ContainsKey(nStage))
        {
            m_pStageMissionClearRankList[nStage] = nRank;
        }
        else
        {
            m_pStageMissionClearRankList.Add(nStage, nRank);
        }
    }

    //public void SetStageLimitReward_byStage(int nStage)
    //{
    //    m_pStageLimitRewardList[nStage] = true;

    //    SaveDataInfo.Instance.MainStageInfoSave(m_pChapterUnlockList, m_pStageUnlockList, m_pStageClearRankList, m_pStageLimitRewardList);
    //}

    public void ClearChapterStarReward()
    {
        m_pChapterStarReward.Clear();
    }

    public void ClearChapterStarCount()
    {
        m_pStarCount_byChapterID.Clear();
        m_pChallengeStarCount_byChapterID.Clear();
    }

    public void SetChapterStarReward_byChapter(ChapterStarReward Info)
    {
        m_pChapterStarReward.Add(Info.m_nChapterID, Info);
    }

    public void SetStarCount_byChapter(int nChapterID, int nStarCount, int nChallengeStarCount)
    {
        m_pStarCount_byChapterID.Add(nChapterID, nStarCount);
        m_pChallengeStarCount_byChapterID.Add(nChapterID, nChallengeStarCount);
    }

    public ChapterStarReward GetChapterStarRewardInfo_byChapter(int nChapterID)
    {
        if (m_pChapterStarReward.ContainsKey(nChapterID))
            return m_pChapterStarReward[nChapterID];

        return null;
    }

    public int GetStarCount_byChapterID(int nChapterID)
    {
        if (m_pStarCount_byChapterID.ContainsKey(nChapterID))
            return m_pStarCount_byChapterID[nChapterID];

        return 0;
    }

    public int GetChallengeStarCount_byChapterID(int nChapterID)
    {
        if (m_pChallengeStarCount_byChapterID.ContainsKey(nChapterID))
            return m_pChallengeStarCount_byChapterID[nChapterID];

        return 0;
    }

    public Dictionary<int, bool> GetChapterUnlockList()
    {
        return m_pChapterUnlockList;
    }

    public bool GetChapterUnlock_byChapter(int nChapter)
    {
        if (m_pChapterUnlockList.ContainsKey(nChapter) == true)
        {
            return m_pChapterUnlockList[nChapter];
        }

        return false;
    }

    public bool IsChapterUnlockListContainKey(int nKey)
    {
        if(m_pChapterUnlockList.ContainsKey(nKey) == true)
        {
            return true;
        }

        return false;
    }

    public Dictionary<int, bool> GetStageUnlockList()
    {
        return m_pStageUnlockList;
    }

    public bool GetStageUnlock_byStage(int nStage)
    {
        if(m_pStageUnlockList.ContainsKey(nStage) == true)
        {
            return m_pStageUnlockList[nStage];
        }

        return false;
    }

    public bool IsStageUnlockListContainKey(int nKey)
    {
        if(m_pStageUnlockList.ContainsKey(nKey) == true)
        {
            return true;
        }

        return false;
    }

    public Dictionary<int, int> GetStageClearRankList()
    {
        return m_pStageClearRankList;
    }

    public int GetStageClearRank_byStage(int nStage)
    {
        if (m_pStageClearRankList.ContainsKey(nStage))
        {
            return m_pStageClearRankList[nStage];
        }

        return 0;
    }

    public int GetStageMissionClearRank_byStage(int nStage)
    {
        if (m_pStageMissionClearRankList.ContainsKey(nStage))
        {
            return m_pStageMissionClearRankList[nStage];
        }

        return 0;
    }

    public Dictionary<int, bool> GetStageLimitRewardList()
    {
        return m_pStageLimitRewardList;
    }

    public bool IsStageInfoExist_byChapterID(int nChapterID)
    {
        List<ExcelData_StageInfo> pInfo = ExcelDataManager.Instance.m_pStage.GetStageInfoList_byChapter(nChapterID);

        if (m_pStageUnlockList.ContainsKey(pInfo[0].m_nID))
            return true;

        return false;
    }

    public int GetLatestChapter()
    {
        int nChapter = 0;

        foreach(KeyValuePair<int,bool> pInfo in m_pChapterUnlockList)
        {
            if(pInfo.Value == true)
            {
                nChapter = pInfo.Key;
            }
        }

        return nChapter - 100;
    }

    //public bool GetStageLimitReward_byStage(int nStage)
    //{
    //    return m_pStageLimitRewardList[nStage];
    //}

    //public int GetStarCount_byChapterID(int nChapterID)
    //{
    //    int nStarCount = 0;

    //    //List<ExcelData_StageInfo> pStageInfoList = ExcelDataManager.Instance.m_pStage.GetStageInfoList_byChapter(nChapterID);

    //    //for(int i=0; i<pStageInfoList.Count; i++)
    //    //{
    //    //    if (m_pStageClearRankList.ContainsKey(pStageInfoList[i].m_nID))
    //    //    {
    //    //        nStarCount += m_pStageClearRankList[pStageInfoList[i].m_nID];
    //    //    }
    //    //}

    //    if (m_pChapterStarReward.ContainsKey(nChapterID))
    //        return m_pChapterStarReward[nChapterID].m_nStarCount;

    //    return nStarCount;
    //}
}
