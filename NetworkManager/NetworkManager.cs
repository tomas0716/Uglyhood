using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using Thrift.Protocol;
using Thrift.Transport;

using sg.protocol.basic;
using sg.protocol.error;
using sg.protocol.msg_web_api;

using System.Diagnostics;
using ICSharpCode.SharpZipLib.BZip2;

using Firebase.Analytics;

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager m_pInstance = null;
    public static NetworkManager Instance { get { return m_pInstance; } }

    public string       m_strAccessToken    = "";
    public string       m_strURL            = "Empty";//"http://3.34.9.67:38300/thrift";      // 원래는 코드에 박으면 안됨
    public string       m_strEnvName        = "";

    static Dictionary<string, DateTime> m_ReqTime = new Dictionary<string, DateTime>();

	private void Awake()
	{
        m_pInstance = this;
    }

	public bool SendReq<Req, Ans>(Req req, System.Action<Ans> ansHandler) where Req: TBase where Ans: TBase, new()
    { 
        if (typeof(Ans).GetProperty("Error") == null)
        {
            OutputLog.Log("Invalid Ans Type. 'Error' property missing in " + typeof(Ans).Name);
            return false;
        }

        var b64EncodedMsg = Serialize(req);

        SendToApiServer<Req, Ans>(m_strURL, b64EncodedMsg, ansHandler);
        return true;
    }

    private void SendToApiServer<Req, Ans>(string url, string encodedMsg, System.Action<Ans> ansHandler, int nRetryCount = 3) where Req: TBase where Ans: TBase, new()
    {
		string const_retry_url = url;
		string const_encodedMsg = encodedMsg;
		Action<int> retry = (int nRetry) => 
        {
			SendToApiServer<Req,Ans> (const_retry_url, const_encodedMsg, ansHandler, nRetry);
		};

        WWWForm formData = new WWWForm();
        var reqId = typeof(Req).Name;
        formData.AddField("req_id", reqId); 
        formData.AddField("base64msg", encodedMsg);
		formData.AddField("access_token", m_strAccessToken);
		formData.AddField("Cache-Control","no-cache");

        url = url + "/" + reqId;

        if (SetReqTime (reqId) == true) 
		{
			System.Action<Ans> ansHandlerWrap = (ans) =>
			{
				ClearReqTime (reqId);
				ansHandler (ans);
			};

			string current_time = GetNowTimeMillisecond();
			formData.AddField("current_time", current_time);

			NetworkHistory.SetRequest(reqId+GetRequestTime(reqId), reqId, current_time);
			StartCoroutine(WaitForRequest(new WWW(url, formData), ansHandlerWrap, retry, reqId, nRetryCount));
		} 
        else 
        {
			Error error         = new Error();
            error.Is_success    = false;
            error.Err_code      = 1010;
			error.Err_message   = "Error";

			Ans ans = new Ans();
			typeof(Ans).GetProperty("Error").GetSetMethod().Invoke(ans, new object[]{error});
			ClearReqTime (reqId);
			ansHandler(ans);
		}
    }

    private IEnumerator WaitForRequest<Ans>(WWW www, System.Action<Ans> handler, System.Action<int> retry, string reqID = "", int nRetryCount = 3) where Ans : TBase, new()
    {
        float timer = 0;
        bool failed = false;

        while (!www.isDone)
        {
            if ((reqID != "MsgReqMainData") && timer > 7 * Time.timeScale)
            {
                failed = true;
                break;
            }
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if (failed || www.error != null)
        {
            string msg = (failed ? "istimeout" : www.error);

            if (reqID != "MsgReqGetToastMessage")
            {
                if (failed)
                {
                    OutputLog.Log("네트워크 응답 시간 초과");

                    OnError<Ans>(sg.protocol.error.errorConstants.kTimeOut, "", handler, retry, nRetryCount);
                }
                else
                {
                    OutputLog.Log("네트워크 오류 발생");

                    OnError<Ans>(sg.protocol.error.errorConstants.kTimeOut, "", handler, retry, nRetryCount);
                }
            }

            ClearReqTime(reqID);

			// Firebase Error Log
			DateTime currDateTime = System.DateTime.Now;

			string strCurrDate = string.Format("{0}-{1}-{2} {3}:{4}:{5}.{6}",
                                    currDateTime.Year,
                                    currDateTime.Month.ToString("D2"),
                                    currDateTime.Day.ToString("D2"),
                                    currDateTime.Hour.ToString("D2"),
                                    currDateTime.Minute.ToString("D2"),
                                    currDateTime.Second.ToString("D2"),
                                    currDateTime.Millisecond.ToString("D3"));

			Parameter[] parameters = new Parameter[4];
			parameters[0] = new Parameter("api_name", reqID);
            if(failed == true)
                parameters[1] = new Parameter("error_type", "time_out");
            else
                parameters[1] = new Parameter("error_type", "recv_error");
            parameters[2] = new Parameter("error_time", strCurrDate);
			parameters[3] = new Parameter("uin", MyInfo.Instance.m_nUserIndex);

			Helper.FirebaseLogEvent("api_response_error", parameters);
		}
        else
        {
            NetworkHistory.SetAnswer(reqID + GetRequestTime(reqID), GetNowTimeMillisecond());
            Ans ans = Deserialize<Ans>(www.text);
            if (ans == null)
            {
                OnError<Ans>(sg.protocol.error.errorConstants.kInvalidMessageFormat, "", handler, retry, nRetryCount);
            }
            else
            {
                var ansType = typeof(Ans);
                Error error = (Error)ansType.GetProperty("Error").GetGetMethod().Invoke(ans, null);

                if (error.Is_success == true)
                {
                    handler(ans);
                }
                else
                {
                    if (OnError(error.Err_code, retry, nRetryCount) == false)
                    {
                        handler(ans);
                    }
                }
            }
        }

        if (www.isDone)
            www.Dispose();

        www = null;
    }

    private void OnError<Ans>(int errorCode, string errorMsg, System.Action<Ans> handler, Action<int> againSend = null, int nRetryCount = 3)
        where Ans : TBase, new()
    {
        Error error         = new Error();
        error.Err_code      = errorCode;
        error.Err_message   = errorMsg;

		Ans ans = new Ans();

		// ans.Error = error
		typeof(Ans).GetProperty("Error").GetSetMethod().Invoke(ans, new object[] { error });

        if (OnError(errorCode, againSend, nRetryCount) == true)
        {
            return;
        }


        switch (errorCode)
		{
			case 1:     // kTimeOut:
			case 5:     // kHttpError:
				{
                    OutputLog.Log("네트워크 상태 확인");
                }
				break;
			default:
				handler(ans);
				break;
		}

		return;
    }

    public bool OnError(int errorCode, Action<int> againSend = null, int nRetryCount = 3)
    {
        bool IsRetry = false;

        if (ExcelDataManager.Instance != null)
        {
            if (nRetryCount > 0)
            {
                ExcelData_ErrorCodeHandlerInfo pErrorCodeHandlerInfo = ExcelDataManager.Instance.m_pErrorCodeHandler.GetErrorCodeHandlerInfo(errorCode);

                if (pErrorCodeHandlerInfo != null)
                {
                    string strDesc = string.Format("{0}\\n(CODE:{1})", ExcelDataHelper.GetString(pErrorCodeHandlerInfo.m_strPoputDesc_StringTableID), errorCode);
                    switch (pErrorCodeHandlerInfo.m_eNoticeType)
                    {
                        case ExcelData_ErrorCodeHandlerInfo.eNoticeType.Alert:
                            {
                                UIHelper.OnCommonMessagePopupOpen(eCommonMessagePopupType.OneButton,
                                                                  ExcelDataHelper.GetString(pErrorCodeHandlerInfo.m_strPopupTitle_StringTableID),
                                                                  strDesc,
                                                                  ExcelDataHelper.GetString(pErrorCodeHandlerInfo.m_strButtonTextOK_StringTableID),
                                                                  "",
                                                                  () => OnButtonClick_ErrorMessagePopup_OK(pErrorCodeHandlerInfo, againSend, nRetryCount),
                                                                  null);

                                if (pErrorCodeHandlerInfo.m_eCaseOK == ExcelData_ErrorCodeHandlerInfo.eCaseOK.Retry)
                                {
                                    IsRetry = true;
                                }
                            }
                            break;

                        case ExcelData_ErrorCodeHandlerInfo.eNoticeType.Confirm:
                            {
                                UIHelper.OnCommonMessagePopupOpen(eCommonMessagePopupType.TwoButton,
                                                                  ExcelDataHelper.GetString(pErrorCodeHandlerInfo.m_strPopupTitle_StringTableID),
                                                                  strDesc,
                                                                  ExcelDataHelper.GetString(pErrorCodeHandlerInfo.m_strButtonTextOK_StringTableID),
                                                                  ExcelDataHelper.GetString(pErrorCodeHandlerInfo.m_strButtonTextCancel_StringTableID),
                                                                  () => OnButtonClick_ErrorMessagePopup_OK(pErrorCodeHandlerInfo, againSend, nRetryCount),
                                                                  () => OnButtonClick_ErrorMessagePopup_Cancel(pErrorCodeHandlerInfo));

                                if (pErrorCodeHandlerInfo.m_eCaseOK == ExcelData_ErrorCodeHandlerInfo.eCaseOK.Retry)
                                {
                                    IsRetry = true;
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                string strDesc = string.Format("{0}\\n(CODE:{1})", ExcelDataHelper.GetString("ERROR_HANDLER_RETRY_FAIL_POPUP_DESC"), errorCode);

                UIHelper.OnCommonMessagePopupOpen(eCommonMessagePopupType.OneButton,
                                                  ExcelDataHelper.GetString("ERROR_HANDLER_RETRY_FAIL_POPUP_TITLE"),
                                                  strDesc,
                                                  ExcelDataHelper.GetString("ERROR_HANDLER_RETRY_FAIL_POPUP_BUTTON_OK"),
                                                  "",
                                                  () => OnButtonClick_ErrorMessagePopup_AppQuit(),
                                                  null);
            }
        }

        return IsRetry;
    }

    private void OnButtonClick_ErrorMessagePopup_OK(ExcelData_ErrorCodeHandlerInfo pErrorCodeHandlerInfo, Action<int> againSend, int nRetryCount)
    {
        switch (pErrorCodeHandlerInfo.m_eCaseOK)
        {
            case ExcelData_ErrorCodeHandlerInfo.eCaseOK.Terminate:
                {
                    Application.Quit();
                }
                break;
            case ExcelData_ErrorCodeHandlerInfo.eCaseOK.Re_launch:
                {
                    AppInstance.Instance.ReExecute();
                }
                break;
            case ExcelData_ErrorCodeHandlerInfo.eCaseOK.Retry:
                {
                    --nRetryCount;

                    if (nRetryCount >= 0)
                    {
						againSend(nRetryCount);
                    }
                }
                break;
            case ExcelData_ErrorCodeHandlerInfo.eCaseOK.VersionUpLink:
                {
                }
                break;
        }
    }

    private void OnButtonClick_ErrorMessagePopup_Cancel(ExcelData_ErrorCodeHandlerInfo pErrorCodeHandlerInfo)
    {
        switch (pErrorCodeHandlerInfo.m_eCaseCancel)
        {
            case ExcelData_ErrorCodeHandlerInfo.eCaseCancel.Terminate:
                {
                    Application.Quit();
                }
                break;
            case ExcelData_ErrorCodeHandlerInfo.eCaseCancel.Re_launch:
                {
                    AppInstance.Instance.ReExecute();
                }
                break;
        }
    }

    private void OnButtonClick_ErrorMessagePopup_AppQuit()
    {
        Application.Quit();
    }

    private string Serialize(TBase reqMsg)
    {
        // 스트림버퍼, 트렌스포트, 프로토콜 준비.
        var outputStream    = new MemoryStream(1024 * 100);
        var transport       = new TStreamTransport(null, outputStream);
        var binaryProtocol  = new TBinaryProtocol(transport);

        // 시리얼라이즈 
        reqMsg.Write(binaryProtocol);

        // base64 인코딩
        var b64EncodedMsg = System.Convert.ToBase64String(outputStream.ToArray());
        return b64EncodedMsg;
    }

    public static T Deserialize<T>(string base64EncodedMessage) where T : TBase, new()
    {
        var ch = "$compressed$";
        byte[] byteEncodedMessage = null;
		
        try
        {
            if (base64EncodedMessage.Substring(0, ch.Length) == ch)
            {
                // base64 디코딩
                var compressed = System.Convert.FromBase64String(base64EncodedMessage.Substring(ch.Length));

                // 압축 풀기
                var cms = new MemoryStream(compressed);
                BZip2InputStream bz2 = new BZip2InputStream(cms);
                var dcms = new MemoryStream();
                var buffer = new byte[4096];
                while (true)
                {
                    int bytesRead = bz2.Read(buffer, 0, buffer.Length);

                    if (0 < bytesRead)
                    {
                        dcms.Write(buffer, 0, bytesRead);
                    }
                    else
                    {
                        dcms.Flush();
                        break;
                    }
                }

                bz2.Close();
                byteEncodedMessage = dcms.ToArray();
                dcms.Close();
            }
            else // base64 디코딩
            {
                byteEncodedMessage = System.Convert.FromBase64String(base64EncodedMessage);
            }

            // 스트림버퍼, 트랜스포트, 프로토콜 준비.
            var inputStream = new MemoryStream(byteEncodedMessage);
            var transport = new TStreamTransport(inputStream, null);
            var binaryProtocol = new TBinaryProtocol(transport);

            // 디시리얼라이즈
            var ans = new T();
            ans.Read(binaryProtocol);
            return ans;
        }
        catch (TProtocolException e)
        {
            return default(T);
        }
    }

    public DateTime GetRequestTime(string req)
    {
        if (m_ReqTime.ContainsKey(req) == false)
        {
            return DateTime.MinValue;
        }
        return m_ReqTime[req];
    }

    public string GetNowTimeMillisecond()
    {
        return (DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond);
    }

    static bool SetReqTime(string req)
    {
        if (m_ReqTime.ContainsKey(req) == false)
        {
            m_ReqTime[req] = DateTime.Now;
            return true;
        }

        var period = DateTime.Now - m_ReqTime[req];
        var log_msg = "Same " + req + " in " + period.TotalSeconds + " seconds";

        if (5.0f < period.TotalSeconds)
        {
            OutputLog.Log(log_msg);
        }
        else
        {
            OutputLog.Log(log_msg);
        }

        m_ReqTime[req] = DateTime.Now;
        return false;
    }

    static void ClearReqTime(string req)
    {
        if (m_ReqTime.ContainsKey(req))
            m_ReqTime.Remove(req);
    }
}
