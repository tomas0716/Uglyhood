using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;

public class NetComponent_Firebase_DailyFirstConnectLogin : TNetComponent_Next<EventArg_Null, EventArg_Null>
{
    public enum eFirebaseLogType
    {
        date,
        user_player_id,
        user_rank, 
        user_gold,
        user_free_gem,
        user_paid_gem,
        user_special_gacha_ticket,
        user_normal_gacha_ticket,
        user_stage_progress,
        user_total_character,
        user_ssr_character,
        user_sr_character,
        user_r_character,
        user_n_character,
        user_water_character,
        user_fire_character,
        user_wind_character,
        user_light_character,
        user_dark_character,
        user_booster_item,
        
        max,
    }

    public NetComponent_Firebase_DailyFirstConnectLogin()
    {
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
    }

    public override void LateUpdate()
    {
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        NetLog.Log("NetComponent_Firebase_DailyFirstConnectLogin : OnEvent");

        if (OptionInfo.Instance.m_nDailyFirstConnectLogin_Year != EspressoInfo.Instance.m_pConnectDateTime.Year ||
            OptionInfo.Instance.m_nDailyFirstConnectLogin_Month != EspressoInfo.Instance.m_pConnectDateTime.Month ||
            OptionInfo.Instance.m_nDailyFirstConnectLogin_Day != EspressoInfo.Instance.m_pConnectDateTime.Day)
        {
            InventoryInfoManager pInvenManager = InventoryInfoManager.Instance;
            ItemInvenItemInfo pItemInvenItemInfo;

            Parameter[] parameters = new Parameter[(int)eFirebaseLogType.max];

            string strDate = string.Format("{0}{1}{2}", EspressoInfo.Instance.m_pConnectDateTime.Year, EspressoInfo.Instance.m_pConnectDateTime.Month.ToString("D2"), EspressoInfo.Instance.m_pConnectDateTime.Day.ToString("D2"));
            parameters[(int)eFirebaseLogType.date] = new Parameter(eFirebaseLogType.date.ToString(), strDate);
            parameters[(int)eFirebaseLogType.user_player_id] = new Parameter(eFirebaseLogType.user_player_id.ToString(), MyInfo.Instance.m_nUserIndex);
            parameters[(int)eFirebaseLogType.user_rank] = new Parameter(eFirebaseLogType.user_rank.ToString(), MyInfo.Instance.m_pUserBaseInfo.m_pUserInfoBasic.I_UserRank);
            parameters[(int)eFirebaseLogType.user_gold] = new Parameter(eFirebaseLogType.user_gold.ToString(), pInvenManager.m_pItemInvenInfo.GetInvenItemInfo_byTableID((int)eItemType.Gold).m_nItemCount);
            parameters[(int)eFirebaseLogType.user_free_gem] = new Parameter(eFirebaseLogType.user_free_gem.ToString(), pInvenManager.m_pItemInvenInfo.GetInvenItemInfo_byTableID((int)eItemType.Gem_Free).m_nItemCount);
            parameters[(int)eFirebaseLogType.user_paid_gem] = new Parameter(eFirebaseLogType.user_paid_gem.ToString(), pInvenManager.m_pItemInvenInfo.GetInvenItemInfo_byTableID((int)eItemType.Gem_Paid).m_nItemCount);
            pItemInvenItemInfo = pInvenManager.m_pItemInvenInfo.GetInvenItemInfo_byTableID(50002);
            parameters[(int)eFirebaseLogType.user_special_gacha_ticket] = new Parameter(eFirebaseLogType.user_special_gacha_ticket.ToString(), pItemInvenItemInfo != null ? pItemInvenItemInfo.m_nItemCount : 0);
            pItemInvenItemInfo = pInvenManager.m_pItemInvenInfo.GetInvenItemInfo_byTableID(50001);
            parameters[(int)eFirebaseLogType.user_normal_gacha_ticket] = new Parameter(eFirebaseLogType.user_normal_gacha_ticket.ToString(), pItemInvenItemInfo != null ? pItemInvenItemInfo.m_nItemCount : 0);
            parameters[(int)eFirebaseLogType.user_stage_progress] = new Parameter(eFirebaseLogType.user_stage_progress.ToString(), Helper.GetLastClearStage());
            parameters[(int)eFirebaseLogType.user_total_character] = new Parameter(eFirebaseLogType.user_total_character.ToString(), pInvenManager.m_pCharacterInvenInfo.GetNumInvenItem());
            parameters[(int)eFirebaseLogType.user_ssr_character] = new Parameter(eFirebaseLogType.user_ssr_character.ToString(), Helper.GetNumCharacter_UnitRank(eUnitRank.SSR));
            parameters[(int)eFirebaseLogType.user_sr_character] = new Parameter(eFirebaseLogType.user_sr_character.ToString(), Helper.GetNumCharacter_UnitRank(eUnitRank.SR));
            parameters[(int)eFirebaseLogType.user_r_character] = new Parameter(eFirebaseLogType.user_r_character.ToString(), Helper.GetNumCharacter_UnitRank(eUnitRank.R));
            parameters[(int)eFirebaseLogType.user_n_character] = new Parameter(eFirebaseLogType.user_n_character.ToString(), Helper.GetNumCharacter_UnitRank(eUnitRank.N));
            parameters[(int)eFirebaseLogType.user_water_character] = new Parameter(eFirebaseLogType.user_water_character.ToString(), Helper.GetNumCharacter_UnitElement(eElement.Water));
            parameters[(int)eFirebaseLogType.user_fire_character] = new Parameter(eFirebaseLogType.user_fire_character.ToString(), Helper.GetNumCharacter_UnitElement(eElement.Fire));
            parameters[(int)eFirebaseLogType.user_wind_character] = new Parameter(eFirebaseLogType.user_wind_character.ToString(), Helper.GetNumCharacter_UnitElement(eElement.Wind));
            parameters[(int)eFirebaseLogType.user_light_character] = new Parameter(eFirebaseLogType.user_light_character.ToString(), Helper.GetNumCharacter_UnitElement(eElement.Light));
            parameters[(int)eFirebaseLogType.user_dark_character] = new Parameter(eFirebaseLogType.user_dark_character.ToString(), Helper.GetNumCharacter_UnitElement(eElement.Dark));
            parameters[(int)eFirebaseLogType.user_booster_item] = new Parameter(eFirebaseLogType.user_booster_item.ToString(), Helper.GetNumBoosterItem());

            Helper.FirebaseLogEvent("daily_user_status", parameters);

            OptionInfo.Instance.m_nDailyFirstConnectLogin_Year = EspressoInfo.Instance.m_pConnectDateTime.Year;
            OptionInfo.Instance.m_nDailyFirstConnectLogin_Month = EspressoInfo.Instance.m_pConnectDateTime.Month;
            OptionInfo.Instance.m_nDailyFirstConnectLogin_Day = EspressoInfo.Instance.m_pConnectDateTime.Day;
            OptionInfo.Instance.Save();
        }

        GetNextEvent().OnEvent(EventArg_Null.Object);
    }
}
