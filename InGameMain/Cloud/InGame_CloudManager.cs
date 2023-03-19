using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame_CloudManager
{
	private List<InGame_Cloud> m_CloudList = new List<InGame_Cloud>();
	private List<InGame_Cloud> m_RemoveCloudList = new List<InGame_Cloud>();

	public InGame_CloudManager()
	{

	}

	private void UpdateRemove()
	{
		foreach (InGame_Cloud Remove in m_RemoveCloudList)
		{
			foreach (InGame_Cloud pCloud in m_CloudList)
			{
				if (Remove == pCloud)
				{
					GameObject.Destroy(pCloud.gameObject);
					m_CloudList.Remove(pCloud);
					break;
				}
			}
		}

		m_RemoveCloudList.Clear();
	}

	public void Update()
	{
		UpdateRemove();
	}

	public void AddCloud(InGame_Cloud pCloud)
	{
		m_CloudList.Add(pCloud);
	}

	public InGame_Cloud GetCloud(Slot pSlot, eOwner eOwn)
	{
		foreach (InGame_Cloud pCloud in m_CloudList)
		{
			if(pCloud.GetSlot() == pSlot && pCloud.GetOwner() == eOwn)
				return pCloud;
		}

		return null;
	}

	public InGame_Cloud GetCloud(Slot pSlot)
	{
		foreach (InGame_Cloud pCloud in m_CloudList)
		{
			if (pCloud.GetSlot() == pSlot)
				return pCloud;
		}

		return null;
	}

	public bool IsExistCloud(Slot pSlot)
	{
		foreach (InGame_Cloud pCloud in m_CloudList)
		{
			if (pCloud.GetSlot() == pSlot)
				return true;
		}

		return false;
	}

	public bool IsExistCloud(eOwner eOwn)
	{
		foreach (InGame_Cloud pCloud in m_CloudList)
		{
			if (pCloud.GetOwner() == eOwn)
				return true;
		}

		return false;
	}

	public void RemoveCloud(InGame_Cloud pCloud)
	{
		m_RemoveCloudList.Add(pCloud);
	}

	public void RemoveCloudAll()
	{
		foreach (InGame_Cloud pCloud in m_CloudList)
		{
			m_RemoveCloudList.Add(pCloud);
		}
	}

	public void DecreaseTurn(eOwner eOwn)
	{
		foreach (InGame_Cloud pCloud in m_CloudList)
		{
			if (pCloud.GetOwner() == eOwn)
			{
				pCloud.DecreaseTurn();
			}
		}
	}
}
