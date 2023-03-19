using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameBackground : MonoBehaviour
{
    private Image       m_pImage_Background     = null;

    private Camera      m_pCamera               = null;
    private Vector3     m_vCamera_OriPosition   = Vector3.zero;

    void Start()
    {
        GameObject ob;

        ob = Helper.FindChildGameObject(gameObject, "Canvas");
        RectTransform rt = ob.transform as RectTransform;
        rt.sizeDelta = AppInstance.Instance.m_vCurrResolution;

        ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);

        ob = Helper.FindChildGameObject(gameObject, "BG");
        m_pImage_Background = ob.GetComponent<Image>();
        m_pImage_Background.sprite = Resources.Load<Sprite>("Image/InGameBackground/" + pStageInfo.m_strBackground);

        ob = Helper.FindChildGameObject(gameObject, "Camera");
        m_pCamera = ob.GetComponent<Camera>();
        m_pCamera.orthographicSize = AppInstance.Instance.m_vCurrResolution.y * 0.5f;
        m_pCamera.nearClipPlane = 0.1f;
        m_pCamera.farClipPlane = 1000000;
        m_vCamera_OriPosition = new Vector3(0, 0, -99990);
        m_pCamera.transform.position = m_vCamera_OriPosition;

        EventDelegateManager.Instance.OnEventInGame_Request_ActionTrigger_Open_ToGray += OnInGame_Request_ActionTrigger_Open_ToGray;
        EventDelegateManager.Instance.OnEventInGame_Request_ActionTrigger_Close += OnInGame_Request_ActionTrigger_Close;

        EventDelegateManager.Instance.OnEventInGame_SkillTrigger_Drag += OnInGame_SkillTrigger_Drag;
    }

	private void OnDestroy()
	{
        EventDelegateManager.Instance.OnEventInGame_Request_ActionTrigger_Open_ToGray -= OnInGame_Request_ActionTrigger_Open_ToGray;
        EventDelegateManager.Instance.OnEventInGame_Request_ActionTrigger_Close -= OnInGame_Request_ActionTrigger_Close;

        EventDelegateManager.Instance.OnEventInGame_SkillTrigger_Drag -= OnInGame_SkillTrigger_Drag;
    }

	void Update()
    {
        
    }

    public void OnInGame_Request_ActionTrigger_Open_ToGray()
    {
        m_pImage_Background.color = Color.gray;
    }

    public void OnInGame_Request_ActionTrigger_Close()
    {
        m_pImage_Background.color = Color.white;
    }

    private void OnInGame_SkillTrigger_Drag(float fDragValue)
    {
        m_pCamera.transform.position = m_vCamera_OriPosition + new Vector3(0, fDragValue, 0);
    }
}
