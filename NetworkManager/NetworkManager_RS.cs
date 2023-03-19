using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using System;
using System.Collections.Generic;
using SimpleJSON;

public class RS_ArgData
{
}

public class NetworkManager_RS : MonoBehaviour
{
    private static NetworkManager_RS ms_pInstance = null;
    public static NetworkManager_RS Instance { get { return ms_pInstance; } }

    public string       m_strWebURL     = "";
    public string       m_strDevWebURL  = "";
    private List<Data>  m_DataList      = new List<Data>();

    private float       m_fTimeOut      = 5;

    public string       _Key;

    class Data
    {
        public string m_strUrl;
        public WWWForm m_Form;
        public Action<Data,JSONNode> m_OnFinished;
        public Action<RS_ArgData> m_ClientRecv;
        public int m_nRetryCount = 3;

        public Data(string strUrl, WWWForm pForm, Action<Data,JSONNode> onFinished, Action<RS_ArgData> clientRecv)
        {
            m_strUrl = strUrl;
            m_Form = pForm;
            m_OnFinished = onFinished;
            m_ClientRecv = clientRecv;
        }
    }

    void Awake()
    {
        ms_pInstance = this;

        StartPost();
    }

    void RemoveDataList(Data _Data)
    {
        m_DataList.Remove(_Data);
    }

    IEnumerator Post()
    {
        if (m_DataList.Count > 0)
        {
            Data pData = m_DataList[0];

            WWW www;

            if (pData.m_Form != null)
            {
                www = new WWW(pData.m_strUrl, pData.m_Form);
            }
            else
            {
                www = new WWW(pData.m_strUrl);
            }

            StartCoroutine("TimeOut", pData);

            do
            {
                yield return null;
            }
            while (!www.isDone);

            StopCoroutine("TimeOut");

            if (www.error == null)
            {
                JSONNode _Json = JSON.Parse(www.text);
                pData.m_OnFinished(pData, _Json);
                RemoveDataList(pData);
            }
            else
            {
                Action<int> retry = (int nRetry) =>
                {
                    --pData.m_nRetryCount;
                    m_DataList.Add(pData);
                    //StartCoroutine("Post");
                };

                NetworkManager.Instance.OnError(sg.protocol.error.errorConstants.kTimeOut, retry, pData.m_nRetryCount);
                RemoveDataList(pData);
            }

            www.Dispose();
        }

        yield return null;

        StartCoroutine("Post");
    }

    IEnumerator TimeOut(Data _Data)
    {
        yield return new WaitForSeconds(m_fTimeOut);
    }

    public void StartPost()
    {
        m_DataList.Clear();

        StopCoroutine("Post");
        StartCoroutine("Post");
    }

    public void StopPost()
    {
        StopCoroutine("Post");
    }

    public void SendGuide(System.Action<RS_ArgData> clientRecv)
    {
        string strURL = "";
        string strProvider = "ios";

        if (Application.platform == RuntimePlatform.Android)
        {
            if (EspressoInfo.Instance.m_IsOneStore == false)
            {
                strProvider = "android";
            }
            else
            {
                strProvider = "onestore";
            }
        }

#if UNITY_EDITOR
        string strWebURL = "http://110.165.16.221:38301/sg_router/guide/?membership_id={0}&client_version={1}&platform_id={2}";
        strURL = string.Format(strWebURL, SaveDataInfo.Instance.m_strNetwork_LoginAccountID, Application.version, "android");
#else
        //switch (EspressoInfo.Instance.m_eNetwork_RouterServer)
        //{
        //    case eNetwork_RouterServer.Dev:
        //        {
        //            string strWebURL = "http://110.165.16.221:38301/sg_router/guide/?membership_id={0}&client_version={1}&platform_id={2}";
        //            strURL = string.Format(strWebURL, SaveDataInfo.Instance.m_strNetwork_LoginAccountID, Application.version, strProvider);
        //        }
        //        break;

        //    case eNetwork_RouterServer.Review:
        //        {
        //            string strWebURL = "http://223.130.160.184:38301/sg_router/guide/?membership_id={0}&client_version={1}&platform_id={2}";
        //            strURL = string.Format(strWebURL, SaveDataInfo.Instance.m_strNetwork_LoginAccountID, Application.version, strProvider);
        //        }
        //        break;

        //    case eNetwork_RouterServer.Live:
        //        {
        //            strURL = string.Format(m_strWebURL, SaveDataInfo.Instance.m_strNetwork_LoginAccountID, Application.version, strProvider);
        //        }
        //        break;
        //}

        // Live
        strURL = string.Format(m_strWebURL, SaveDataInfo.Instance.m_strNetwork_LoginAccountID, Application.version, strProvider);

        if (GameConfig.Instance.m_IsLive == true)
        {
            strURL = string.Format(m_strWebURL, SaveDataInfo.Instance.m_strNetwork_LoginAccountID, Application.version, strProvider);
        }
#endif

        Data pData = new Data(strURL, null, Receive_Guide, clientRecv);
        m_DataList.Add(pData);
    }

    void Receive_Guide(Data pData, JSONNode pJsonNode)
    {
        NetComponent_RouterServer.RS_ArgData_Base pArg = new NetComponent_RouterServer.RS_ArgData_Base();
        pArg.m_IsInspection_Pass_User = false;
        pArg.m_strEnvName = pJsonNode["env_name"];
        pArg.m_strResult = pJsonNode["api_result"];
        pArg.m_strConnectType = pJsonNode["connect_type"];

        if (pArg.m_strConnectType == "inspection")
        {
            pArg.m_IsUpdateNeed = false;
            pArg.m_IsUpdateForce = false;
            pArg.m_strMaketURL = "";
            pArg.m_IsInspection_Now = bool.Parse(pJsonNode["inspection_is_now"]);
            pArg.m_strAPIServer = "";

            pData.m_ClientRecv(pArg);
        }
        else if (pArg.m_strConnectType == "inspection_pass")
        {
            pArg.m_IsUpdateNeed = bool.Parse(pJsonNode["client_update_need"]);
            pArg.m_IsUpdateForce = bool.Parse(pJsonNode["client_update_force"]);
            pArg.m_strMaketURL = pJsonNode["client_update_detail"]["MarketURL"];
            pArg.m_IsInspection_Now = bool.Parse(pJsonNode["inspection_is_now"]);
            pArg.m_IsInspection_Pass_User = bool.Parse(pJsonNode["inspection_is_pass_user"]);
            pArg.m_strAPIServer = pJsonNode["api_server"];

            pData.m_ClientRecv(pArg);
        }
        else if (pArg.m_strConnectType == "other_env")
        {
            string strURL = pJsonNode["new_router_server"];
            m_strWebURL = strURL + m_strDevWebURL;
            SendGuide(pData.m_ClientRecv);
        }
        else
        {
            pArg.m_IsUpdateNeed = bool.Parse(pJsonNode["client_update_need"]);
            pArg.m_IsUpdateForce = bool.Parse(pJsonNode["client_update_force"]);
            pArg.m_strMaketURL = pJsonNode["client_update_detail"]["MarketURL"];
            pArg.m_IsInspection_Now = bool.Parse(pJsonNode["inspection_is_now"]);
            pArg.m_strAPIServer = pJsonNode["api_server"];

            pData.m_ClientRecv(pArg);
        }
    }
}
