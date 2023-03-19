using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_CameraShaking : GameEvent
{
    private float       m_fShakeAmount;
    private float       m_fShakeTime;
    private Vector3     m_vInitPos;

    public GameEvent_CameraShaking(float fTime, float fAmount)
    {
        m_fShakeTime = fTime;
        m_fShakeAmount = fAmount;
        m_vInitPos = Camera.main.transform.position;
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
        if (m_fShakeTime > 0)
        {
            Camera.main.transform.position = Random.insideUnitSphere * m_fShakeAmount + m_vInitPos;
            m_fShakeTime -= Time.deltaTime;
        }
        else
        {
            Camera.main.transform.position = m_vInitPos;
            OnDone();
        }
    }
}
