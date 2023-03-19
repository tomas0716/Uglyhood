using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

using Thrift.Protocol;
using Thrift.Transport;

using sg.protocol.msg_web_api;
using sg.protocol.basic;
using sg.protocol.error;

public class TNetComponent<In> : TEvent<In>
{
	//private DateTime	m_SendDateTime		= new DateTime();

	public TNetComponent()
	{
	}

	public virtual void OnDestroy()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void LateUpdate()
	{
	}

	public void SendPacket<Req, Ans>(Req req, System.Action<Req, Ans> ansHandler, System.Action<Req, Error> errHandler)
		where Req : TBase, new()
		where Ans : TBase, new()
	{
		//m_SendDateTime = System.DateTime.Now;

		System.Action<Ans> callback = (Ans ans) =>
					RecvPacket<Req, Ans>(req, ans, ansHandler, errHandler);


		NetworkManager.Instance.SendReq<Req, Ans>(req, callback);
	}

	private void RecvPacket<Req, Ans>(Req req, Ans ans, System.Action<Req, Ans> ansHandler, System.Action<Req, Error> errHandler)
		where Req : TBase
		where Ans : TBase
	{
		var ansType = typeof(Ans);
		Error error = (Error)ansType.GetProperty("Error").GetGetMethod().Invoke(ans, null);

		if (error.Is_success)
		{
			//string strSendDate = string.Format("{0}-{1}-{2} {3}:{4}:{5}.{6}",
			//													m_SendDateTime.Year,
			//													m_SendDateTime.Month.ToString("D2"),
			//													m_SendDateTime.Day.ToString("D2"),
			//													m_SendDateTime.Hour.ToString("D2"),
			//													m_SendDateTime.Minute.ToString("D2"),
			//													m_SendDateTime.Second.ToString("D2"),
			//													m_SendDateTime.Millisecond.ToString("D3"));

			//DateTime recvDateTime = System.DateTime.Now;

			//string strRecvDate = string.Format("{0}-{1}-{2} {3}:{4}:{5}.{6}",
			//						recvDateTime.Year,
			//						recvDateTime.Month.ToString("D2"),
			//						recvDateTime.Day.ToString("D2"),
			//						recvDateTime.Hour.ToString("D2"),
			//						recvDateTime.Minute.ToString("D2"),
			//						recvDateTime.Second.ToString("D2"),
			//						recvDateTime.Millisecond.ToString("D3"));

			//TimeSpan timeSpan = recvDateTime - m_SendDateTime;
			//int LatencyTime = (int)timeSpan.TotalMilliseconds;

			//string[] strAPIName = req.ToString().Split("(");
			//Parameter[] parameters = new Parameter[4];
			//parameters[0] = new Parameter("api_name", strAPIName[0]);
			//parameters[1] = new Parameter("send_time", strSendDate);
			//parameters[2] = new Parameter("response_time", strRecvDate);
			//parameters[3] = new Parameter("latency_time_ms", LatencyTime);

			//Helper.FirebaseLogEvent("api_response_get", parameters);

			//InGameLog.Log(string.Format("{0} : {1},			{2},	{3},	{4}", "api_response_get", strAPIName[0], strSendDate, strRecvDate, LatencyTime.ToString()));

			ansHandler(req, ans);
		}
		else
		{
			errHandler(req, error);
		}
	}
}

public class TNetComponent_Next<In, Out> : TNetComponent<In>
{
	private TEvent<Out> m_pNextEvent = new TEvent<Out>();

	public TNetComponent_Next()
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

	public void SetNextEvent(TEvent<Out> pNextEvent)
	{
		m_pNextEvent = pNextEvent;
	}

	public void ClearNextEvent()
	{
		m_pNextEvent = new TEvent<Out>();
	}

	public TEvent<Out> GetNextEvent()
	{
		return m_pNextEvent;
	}
}


public class TNetComponent_SuccessOrFail<In, Success, Fail> : TNetComponent<In>
{
	private TEvent<Success> m_pSuccessEvent = new TEvent<Success>();
	private TEvent<Fail>	m_pFailureEvent = new TEvent<Fail>();

	public TNetComponent_SuccessOrFail()
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

	public void SetSuccessEvent(TEvent<Success> pSuccessEvent)
	{
		m_pSuccessEvent = pSuccessEvent;
	}

	public void SetFailureEvent(TEvent<Fail> pFailEvent)
	{
		m_pFailureEvent = pFailEvent;
	}

	public void ClearEvent()
	{
		m_pSuccessEvent = new TEvent<Success>();
		m_pFailureEvent = new TEvent<Fail>();
	}

	public TEvent<Success> GetSuccessEvent()
	{
		return m_pSuccessEvent;
	}

	public TEvent<Fail> GetFailureEvent()
	{
		return m_pFailureEvent;
	}
}