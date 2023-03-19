using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetComponent_LoginTouchToStart : TNetComponent_Next<EventArg_Null, EventArg_Null>
{
    private bool m_IsActive = false;

    public NetComponent_LoginTouchToStart()
    {
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
        if (m_IsActive == true)
        {
            if (Input.GetMouseButtonUp(0) == true)
            {
                m_IsActive = false;
                EventDelegateManager.Instance.OnHide_LoginTouchToStartNotice();
                GetNextEvent().OnEvent(EventArg_Null.Object);
            }
        }
    }

    public override void LateUpdate()
    {
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        m_IsActive = true;
        EventDelegateManager.Instance.OnShow_LoginTouchToStartNotice();
    }
}
