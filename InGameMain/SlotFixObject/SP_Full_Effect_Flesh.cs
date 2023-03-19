using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_Full_Effect_Flesh
{
    private static Transformer_Scalar   ms_pAlpha_SP_Full_Effect    = new Transformer_Scalar(0);
    private static float                ms_fAlpha                   = 0;

    public SP_Full_Effect_Flesh()
    {
        TransformerEvent_Scalar eventValue;
        eventValue = new TransformerEvent_Scalar(0, 0);
        ms_pAlpha_SP_Full_Effect.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(0.2f, 0.75f);
        ms_pAlpha_SP_Full_Effect.AddEvent(eventValue);
        eventValue = new TransformerEvent_Scalar(0.4f, 0);
        ms_pAlpha_SP_Full_Effect.AddEvent(eventValue);
        ms_pAlpha_SP_Full_Effect.SetCallback(null, OnDone_Alpha_SP_Full_Effect);
        ms_pAlpha_SP_Full_Effect.OnPlay();
    }

    public static void Update()
    {
        ms_pAlpha_SP_Full_Effect.Update(Time.deltaTime);
        ms_fAlpha = ms_pAlpha_SP_Full_Effect.GetCurScalar();

    }

    private static void OnDone_Alpha_SP_Full_Effect(TransformerEvent eventValue)
    {
        ms_pAlpha_SP_Full_Effect.OnPlay();
    }

    public static float GetAlpha()
    {
        return ms_fAlpha;
    }
}
