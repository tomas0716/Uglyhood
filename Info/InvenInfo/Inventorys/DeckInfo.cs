using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamDeckInfo
{
    public int m_nTeamDeckID = 0;
    public int m_nRuneDeckID = 0;
    public int m_nBoosterItemDeckID = 0;

    public TeamDeckInfo(int nTeamDeckID)
    {
        m_nTeamDeckID = nTeamDeckID;
    }
}

public class DeckInfo
{
    private Dictionary<int, TeamDeckInfo>   m_TeamDeckInfoTable      = new Dictionary<int, TeamDeckInfo>();
    private Dictionary<eGameMode, int>      m_GameModeDeckIDTable    = new Dictionary<eGameMode, int>();

    public DeckInfo()
    {
        for (int i = 0; i < GameDefine.ms_nMaxTeamCount; ++i)
        {
            m_TeamDeckInfoTable.Add(i, new TeamDeckInfo(i));
        }
    }

    public void SetDeck(int nTeamDeckID, int nRuneDeckID, int nBoosterItemDeckID)
    {
        if (m_TeamDeckInfoTable.ContainsKey(nTeamDeckID) == true)
        {
            m_TeamDeckInfoTable[nTeamDeckID].m_nRuneDeckID = nRuneDeckID;
            m_TeamDeckInfoTable[nTeamDeckID].m_nBoosterItemDeckID = nBoosterItemDeckID;
        }
    }

    public void GetDeck(int nTeamDeckID, out int nRuneDeckID, out int nBoosterItemDeckID)
    {
        if (m_TeamDeckInfoTable.ContainsKey(nTeamDeckID) == true)
        {
            nRuneDeckID = m_TeamDeckInfoTable[nTeamDeckID].m_nRuneDeckID;
            nBoosterItemDeckID = m_TeamDeckInfoTable[nTeamDeckID].m_nBoosterItemDeckID;

            return;
        }

		nRuneDeckID = -1;
		nBoosterItemDeckID = -1;
	}

    public int GetRundDeckID(int nTeamDeckID)
    {
        if (m_TeamDeckInfoTable.ContainsKey(nTeamDeckID) == true)
        {
            return m_TeamDeckInfoTable[nTeamDeckID].m_nRuneDeckID;
        }

        return 0;
    }

    public int GetBoosterItemDeckID(int nTeamDeckID)
    {
        if (m_TeamDeckInfoTable.ContainsKey(nTeamDeckID) == true)
        {
            return m_TeamDeckInfoTable[nTeamDeckID].m_nBoosterItemDeckID;
        }

        return 0;
    }

    public void SetGameModeDeckID(eGameMode eMode, int nDeckID)
    {
        if (m_GameModeDeckIDTable.ContainsKey(eMode) == true)
        {
            m_GameModeDeckIDTable[eMode] = nDeckID;
        }
        else
        {
            m_GameModeDeckIDTable.Add(eMode, nDeckID);
        }
    }

    public int GetGameModeDeckID(eGameMode eMode)
    {
        if (m_GameModeDeckIDTable.ContainsKey(eMode) == true)
        {
            return m_GameModeDeckIDTable[eMode];
        }
        
        return 0;
    }
}
