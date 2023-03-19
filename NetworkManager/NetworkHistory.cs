using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHistory
{
	public class NetworkData
	{
		public int _index;
		public string _name;
		public string _reqTime;
		public string _ansTime;

		public NetworkData(int index, string name, string req, string ans)
		{
			_index = index;
			_name = name;
			_reqTime = req;
			_ansTime = ans;
		}
	}

	public static int index = 0;
	public static Dictionary<string, NetworkData> m_dictNetworkHistory = new Dictionary<string, NetworkData>();

	public static void SetRequest(string key, string name, string req)
	{
		m_dictNetworkHistory[key] = new NetworkData(index++, name, req, "");
	}

	public static void SetAnswer(string key, string ans)
	{
		if (m_dictNetworkHistory.ContainsKey(key))
		{
			m_dictNetworkHistory[key]._ansTime = ans;
		}

		if (m_dictNetworkHistory.Count > 15)
		{
			try
			{
				int MaxIndex = -1;
				foreach (var item in m_dictNetworkHistory)
				{

					if (item.Value._index > MaxIndex)
						MaxIndex = item.Value._index;
				}

				int removeIndex = MaxIndex - ((m_dictNetworkHistory.Count - 15) + 8);

				List<string> removals = new List<string>();
				using (var enumerator = m_dictNetworkHistory.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						var element = enumerator.Current;
						if (element.Key != null && element.Value._index < removeIndex)
							removals.Add(element.Key);
					}
				}

				foreach (string address in removals)
				{
					m_dictNetworkHistory.Remove(address);
				}
				removals.Clear();

			}
			catch
			{

			}
		}
	}

	public static string GetNetworkHistory()
	{
		List<NetworkData> list = new List<NetworkData>();
		foreach (var item in m_dictNetworkHistory)
		{
			list.Add(item.Value);
		}

		list.Sort((a, b) => { return a._index.CompareTo(b._index); });

		string result = "";
		foreach (var item in list)
		{
			result += "[" + item._index + "] " + item._name + " [REQ:" + item._reqTime + "] " + "[ANS:" + item._ansTime + "]\n";
		}

		return result;
	}

	public static void PrintNetworkHistory()
	{
		UnityEngine.Debug.Log("------------ PrintNetworkHistory ------------");
		UnityEngine.Debug.Log(GetNetworkHistory());
		UnityEngine.Debug.Log("---------------------------------------------");
		UnityEngine.Debug.Log("---------------------------------------------");
	}
};