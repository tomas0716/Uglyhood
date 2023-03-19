using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetScenario<In, Out> : TComponent<In, Out>
{
    protected eNetState m_eNetState = eNetState.None;

    public NetScenario(eNetState eState)
    {
        m_eNetState = eState;
    }
}
