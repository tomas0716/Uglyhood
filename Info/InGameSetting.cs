using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSetting
{
    private static InGameSetting ms_pInstance = null;
    public static InGameSetting Instance { get { return ms_pInstance; } }

    public string               m_strSettingData        = "Espresso";

    public InGameSettingData    m_pInGameSettingData    = null;

    public InGameSetting()
    {
        ms_pInstance = this;
    }
}
