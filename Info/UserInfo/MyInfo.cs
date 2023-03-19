using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.user;

public class MyInfo : Singleton<MyInfo>
{
    public UserBaseInfo     m_pUserBaseInfo         = new UserBaseInfo();
    public string           m_strAccessToken        = "";
    public long             m_nUserIndex            = 0;            // m_pUserBaseInfo.UserInfoBasic �� Uid �� ����
    public bool             m_IsConnectReward       = false;        // ���� ����
    public string           m_strUserName           = "";
    public int              m_nProfileAvatar        = 0;
    public int              m_nMaxAP                = 0;
    public int              m_nAPChargeTerm         = 0;
    public bool             m_IsGachaPageRedDotOn   = false;

    public UserProfile      m_pUserProfile          = new UserProfile();
    public bool             m_IsNameChangeFree      = false;
    public int              m_nNameChangePrice      = 0;

    public MyInfo()
    {
        
    }
}
