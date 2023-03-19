using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionInfo : Singleton<OptionInfo>
{
    public string   m_strSystemLanguage     = "None";
    public string   m_strDeviceUID          = "None";

    public bool     m_IsBGM                 = true;
    public bool     m_IsEffectSound         = true;
    public bool     m_IsVibration           = true;

    public bool     m_IsDayTimeAlarm        = true;
    public bool     m_IsNightTimeAlarm      = true;
    public bool     m_IsEnergyAlarm         = true;
    public bool     m_IsSponsorshipAlarm    = true; 
    public bool     m_IsBossRaidAlarm       = true;


    public int      m_nDailyFirstConnectLogin_Year      = 0;
    public int      m_nDailyFirstConnectLogin_Month     = 0;
    public int      m_nDailyFirstConnectLogin_Day       = 0;

    public OptionInfo()
    {
        m_strSystemLanguage = PlayerPrefs.GetString(PlayerPrefsString.ms_SystemLanguage, "None");
        m_strDeviceUID = PlayerPrefs.GetString(PlayerPrefsString.ms_DeviceUID, "None");

        m_IsBGM = PlayerPrefs.GetInt(PlayerPrefsString.ms_BGM, 1) == 1 ? true : false;
        m_IsEffectSound = PlayerPrefs.GetInt(PlayerPrefsString.ms_EffectSound, 1) == 1 ? true : false;
        m_IsVibration = PlayerPrefs.GetInt(PlayerPrefsString.ms_Vibration, 1) == 1 ? true : false;

        m_IsDayTimeAlarm = PlayerPrefs.GetInt(PlayerPrefsString.ms_DayTimeAlarm, 1) == 1 ? true : false;
        m_IsNightTimeAlarm = PlayerPrefs.GetInt(PlayerPrefsString.ms_NightTimeAlarm, 1) == 1 ? true : false;
        m_IsEnergyAlarm = PlayerPrefs.GetInt(PlayerPrefsString.ms_EnergyAlarm, 1) == 1 ? true : false;
        m_IsSponsorshipAlarm = PlayerPrefs.GetInt(PlayerPrefsString.ms_SponsorshipAlarm, 1) == 1 ? true : false;
        m_IsBossRaidAlarm = PlayerPrefs.GetInt(PlayerPrefsString.ms_BossRaidAlarm, 1) == 1 ? true : false;

        m_nDailyFirstConnectLogin_Year = PlayerPrefs.GetInt(PlayerPrefsString.ms_DailyFirstConnectLogin_Year, 0);
        m_nDailyFirstConnectLogin_Month = PlayerPrefs.GetInt(PlayerPrefsString.ms_DailyFirstConnectLogin_Month, 0);
        m_nDailyFirstConnectLogin_Day = PlayerPrefs.GetInt(PlayerPrefsString.ms_DailyFirstConnectLogin_Day, 0);
    }

    public void Save()
    {
        PlayerPrefs.SetString(PlayerPrefsString.ms_SystemLanguage, m_strSystemLanguage);
        PlayerPrefs.SetString(PlayerPrefsString.ms_DeviceUID, m_strDeviceUID);

        PlayerPrefs.SetInt(PlayerPrefsString.ms_BGM, m_IsBGM == true ? 1 : 0);
        PlayerPrefs.SetInt(PlayerPrefsString.ms_EffectSound, m_IsEffectSound == true ? 1 : 0);
        PlayerPrefs.SetInt(PlayerPrefsString.ms_Vibration, m_IsVibration == true ? 1 : 0);

        PlayerPrefs.SetInt(PlayerPrefsString.ms_DayTimeAlarm, m_IsDayTimeAlarm == true ? 1 : 0);
        PlayerPrefs.SetInt(PlayerPrefsString.ms_NightTimeAlarm, m_IsNightTimeAlarm == true ? 1 : 0);
        PlayerPrefs.SetInt(PlayerPrefsString.ms_EnergyAlarm, m_IsEnergyAlarm == true ? 1 : 0);
        PlayerPrefs.SetInt(PlayerPrefsString.ms_SponsorshipAlarm, m_IsSponsorshipAlarm == true ? 1 : 0);
        PlayerPrefs.SetInt(PlayerPrefsString.ms_BossRaidAlarm, m_IsBossRaidAlarm == true ? 1 : 0);

        if (SoundPlayer.Instance != null)
        {
            SoundPlayer.Instance.SetMute_BGM(!m_IsBGM);
            SoundPlayer.Instance.SetMute_Sound(!m_IsEffectSound);
        }

        PlayerPrefs.SetInt(PlayerPrefsString.ms_DailyFirstConnectLogin_Year, m_nDailyFirstConnectLogin_Year);
        PlayerPrefs.SetInt(PlayerPrefsString.ms_DailyFirstConnectLogin_Month, m_nDailyFirstConnectLogin_Month);
        PlayerPrefs.SetInt(PlayerPrefsString.ms_DailyFirstConnectLogin_Day, m_nDailyFirstConnectLogin_Day);
    }

    public void ClearDailyFirstConnectLogin()
    {
        m_nDailyFirstConnectLogin_Year = 0;
        m_nDailyFirstConnectLogin_Month = 0;
        m_nDailyFirstConnectLogin_Day = 0;

        PlayerPrefs.SetInt(PlayerPrefsString.ms_DailyFirstConnectLogin_Year, 0);
        PlayerPrefs.SetInt(PlayerPrefsString.ms_DailyFirstConnectLogin_Month, 0);
        PlayerPrefs.SetInt(PlayerPrefsString.ms_DailyFirstConnectLogin_Day, 0);
    }
}
