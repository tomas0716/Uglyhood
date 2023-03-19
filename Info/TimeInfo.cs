using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Timers;

using sg.protocol.basic;

public class TimeInfo : SingletonMono<TimeInfo>
{
    private Coroutine m_pRunningCoroutine_Purchase = null;

    private DateTime m_dtPvPStartTime = new DateTime();

    public void SetPvPStartTime()
    {
        m_dtPvPStartTime = DateTime.Now;
    }

    public int GetPvPFinishTime()
    {
        DateTime dtNow = new DateTime();
        dtNow = DateTime.Now;

        TimeSpan tsFinishTime = new TimeSpan();

        tsFinishTime = m_dtPvPStartTime - dtNow;

        return (int)tsFinishTime.TotalSeconds;
    }
}
