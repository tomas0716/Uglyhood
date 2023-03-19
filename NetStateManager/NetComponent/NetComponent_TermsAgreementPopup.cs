using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetComponent_TermsAgreementPopup : TNetComponent_Next<EventArg_Null,EventArg_Null>
{
    public NetComponent_TermsAgreementPopup()
    {
    }

    public override void OnDestroy()
    {
        EventDelegateManager.Instance.OnEventNext_TermsAgreementPopup -= OnNext_TermsAgreementPopup;
    }

    public override void Update()
    {

    }

    public override void LateUpdate()
    {
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        EventDelegateManager.Instance.OnEventNext_TermsAgreementPopup += OnNext_TermsAgreementPopup;

#if UNITY_EDITOR
        EventDelegateManager.Instance.OnNext_TermsAgreementPopup();
#else
        if (SaveDataInfo.Instance.m_eNetwork_LoginBind != eNetwork_LoginBind.None)
        {
            EventDelegateManager.Instance.OnNext_TermsAgreementPopup();
        }
        else
        {
            EventDelegateManager.Instance.OnShow_TermsAgreementPopup();
        }
#endif
    }

    private void OnNext_TermsAgreementPopup()
    {
        EventDelegateManager.Instance.OnEventNext_TermsAgreementPopup -= OnNext_TermsAgreementPopup;

        GetNextEvent().OnEvent(EventArg_Null.Object);
    }
}
