using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;


public class NetComponent_RouterServer : TNetComponent_Next<EventArg_Null, EventArg_Null>
{
    public class RS_ArgData_Base : RS_ArgData
    {
        public string m_strEnvName;
        public string m_strResult;
        public string m_strConnectType;
        public bool m_IsUpdateNeed;
        public bool m_IsUpdateForce;
        public string m_strMaketURL;
        public bool m_IsInspection_Now;
        public bool m_IsInspection_Pass_User;
        public string m_strAPIServer;
    }

    private GameObject m_pGameObject_Popup = null;

    public NetComponent_RouterServer()
    {
    }

    public override void OnDestroy()
    {
        if (m_pGameObject_Popup != null)
        {
            GameObject.Destroy(m_pGameObject_Popup);
            m_pGameObject_Popup = null;
        }
    }

    public override void Update()
    {
    }

    public override void LateUpdate()
    {
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        NetLog.Log("NetComponent_RouterServer : OnEvent");

        NetworkManager_RS.Instance.SendGuide(RecvPacket_RouterServer);
    }

    public void RecvPacket_RouterServer(RS_ArgData pRS_ArgData)
    {
        NetLog.Log("NetComponent_RouterServer : RecvPacket_RouterServer");

        RS_ArgData_Base pArg = pRS_ArgData as RS_ArgData_Base;
        NetworkManager.Instance.m_strURL = pArg.m_strAPIServer + "/thrift";
        NetworkManager.Instance.m_strEnvName = pArg.m_strEnvName;

        NetLog.Log("NetComponent_RouterServer : RecvPacket_RouterServer URL : " + NetworkManager.Instance.m_strURL);

        if (pArg.m_strConnectType == "inspection" && pArg.m_IsInspection_Now == true)                   // 점검
        {
            UIHelper.OnCommonMessagePopupOpen(eCommonMessagePopupType.TwoButton, 
                                              ExcelDataHelper.GetString("MAINTENANCE_POPUP_TITLE"),
                                              ExcelDataHelper.GetString("MAINTENANCE_POPUP_DESC"),
                                              ExcelDataHelper.GetString("MAINTENANCE_POPUP_BUTTON_QUIT"),
                                              ExcelDataHelper.GetString("MAINTENANCE_POPUP_BUTTON_INFO"),
                                              OnButtonClick_Maintenance_Quit,
                                              OnButtonClick_Maintenance_Info,
                                              false);

            return;
        }
        if (pArg.m_strConnectType == "inspection_pass")                                                 // 점검 중 화이트리스트
        {
            if (pArg.m_IsUpdateNeed == true && pArg.m_IsUpdateForce == true)                            // 강제 업데이트
            {
                UIHelper.OnCommonMessagePopupOpen(eCommonMessagePopupType.OneButton,
                                                  ExcelDataHelper.GetString("FORCE_UPDATE_POPUP_TITLE"),
                                                  ExcelDataHelper.GetString("FORCE_UPDATE_POPUP_DESC"),
                                                  ExcelDataHelper.GetString("FORCE_UPDATE_POPUP_BUTTON_UPDATE"),
                                                  "",
                                                  () => OnButtonClick_ForceUpdate(pArg),
                                                  null, false);
            }
            else if (pArg.m_IsUpdateNeed == true && pArg.m_IsUpdateForce == false)                      // 선택적 업데이트
            {
                UIHelper.OnCommonMessagePopupOpen(eCommonMessagePopupType.TwoButton,
                                                  ExcelDataHelper.GetString("SOFT_UPDATE_POPUP_TITLE"),
                                                  ExcelDataHelper.GetString("SOFT_UPDATE_POPUP_DESC"),
                                                  ExcelDataHelper.GetString("SOFT_UPDATE_POPUP_BUTTON_ SKIP"),
                                                  ExcelDataHelper.GetString("SOFT_UPDATE_POPUP_BUTTON_UPDATE"),
                                                  OnButotnClick_Skip,
                                                  () => OnButtonClick_SoftUpdate(pArg));
            }
            else
            {
                GetNextEvent().OnEvent(EventArg_Null.Object);
            }

        }
        else if (pArg.m_IsUpdateNeed == true && pArg.m_IsUpdateForce == true)                           // 강제 업데이트
        {
            UIHelper.OnCommonMessagePopupOpen(eCommonMessagePopupType.OneButton,
                                              ExcelDataHelper.GetString("FORCE_UPDATE_POPUP_TITLE"),
                                              ExcelDataHelper.GetString("FORCE_UPDATE_POPUP_DESC"),
                                              ExcelDataHelper.GetString("FORCE_UPDATE_POPUP_BUTTON_UPDATE"),
                                              "",
                                              () => OnButtonClick_ForceUpdate(pArg),
                                              null, false);
        }
        else if (pArg.m_IsUpdateNeed == true && pArg.m_IsUpdateForce == false)                          // 선택적 업데이트
        {
            m_pGameObject_Popup = UIHelper.OnCommonMessagePopupOpen(eCommonMessagePopupType.TwoButton,
                                              ExcelDataHelper.GetString("SOFT_UPDATE_POPUP_TITLE"),
                                              ExcelDataHelper.GetString("SOFT_UPDATE_POPUP_DESC"),
                                              ExcelDataHelper.GetString("SOFT_UPDATE_POPUP_BUTTON_ SKIP"),
                                              ExcelDataHelper.GetString("SOFT_UPDATE_POPUP_BUTTON_UPDATE"),
                                              OnButotnClick_Skip,
                                              () => OnButtonClick_SoftUpdate(pArg));
        }
        else
        {
            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
    }

    public void OnButtonClick_Maintenance_Info()
    {
        Application.OpenURL("https://game.naver.com/lounge/Uglyhood_Puzzle_Defense/board/11");
    }

    public void OnButtonClick_Maintenance_Quit()
    {
        Application.Quit();
    }

    public void OnButtonClick_ForceUpdate(RS_ArgData_Base pArg)
    {
        Application.OpenURL(pArg.m_strMaketURL);
    }

    public void OnButtonClick_SoftUpdate(RS_ArgData_Base pArg)
    {
        Application.OpenURL(pArg.m_strMaketURL);
    }

    public void OnButotnClick_Skip()
    {
        if (m_pGameObject_Popup != null)
        {
            GameObject.Destroy(m_pGameObject_Popup);
            m_pGameObject_Popup = null;
        }

        GetNextEvent().OnEvent(EventArg_Null.Object);
    }
}
