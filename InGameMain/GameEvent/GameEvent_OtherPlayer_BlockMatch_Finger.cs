using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_OtherPlayer_BlockMatch_Finger : GameEvent
{
    private bool                m_IsSpecialBlock            = false;
    private Slot                m_pSlot_01                  = null;
    private Slot                m_pSlot_02                  = null;

    private Slot                m_pSlot_SpecialBlock        = null;

    private Transformer_Timer   m_pTimer_Delay              = new Transformer_Timer();
    private Transformer_Vector3 m_pPos_Finger               = new Transformer_Vector3(Vector3.zero);
    private Transformer_Scalar  m_pAlpha_Finger             = new Transformer_Scalar(1);

    private Plane2D             m_pPlane2D_Finger           = null;

    public GameEvent_OtherPlayer_BlockMatch_Finger(Slot pSlot_01, Slot pSlot_02)
    {
        m_IsSpecialBlock = false;
        m_pSlot_01 = pSlot_01;
        m_pSlot_02 = pSlot_02;

        CreateFinger();

        TransformerEvent eventValue;
        eventValue = new TransformerEvent_Timer(0.5f);
        m_pTimer_Delay.AddEvent(eventValue);
        m_pTimer_Delay.SetCallback(null, OnDone_Timer_StartDelay);
        m_pTimer_Delay.OnPlay();

        m_pPlane2D_Finger.SetPosition(pSlot_01.GetPosition());
        m_pPos_Finger = new Transformer_Vector3(pSlot_01.GetPosition());
    }

    public GameEvent_OtherPlayer_BlockMatch_Finger(Slot pSlot_SpecialBlock)
    {
        m_IsSpecialBlock = true;
        m_pSlot_SpecialBlock = pSlot_SpecialBlock;

        CreateFinger();

        TransformerEvent eventValue;
        eventValue = new TransformerEvent_Timer(0.5f);
        m_pTimer_Delay.AddEvent(eventValue);
        m_pTimer_Delay.SetCallback(null, OnDone_Timer_StartDelay);
        m_pTimer_Delay.OnPlay();

        m_pPlane2D_Finger.SetPosition(pSlot_SpecialBlock.GetPosition());
        m_pPos_Finger = new Transformer_Vector3(pSlot_SpecialBlock.GetPosition());
    }

    private void CreateFinger()
    {
        ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
        GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/Hand_Guide");
        ob = GameObject.Instantiate(ob);
        m_pPlane2D_Finger = ob.GetComponent<Plane2D>();
        Helper.Change_Plane2D_AtlasInfo(m_pPlane2D_Finger, "Slot_" + pStageInfo.m_strStageTheme, "Hand_Guide");
    }

    public override void OnDestroy()
    {
        GameObject.Destroy(m_pPlane2D_Finger.gameObject);
    }

    public override void Update()
    {
        m_pPos_Finger.Update(Time.deltaTime);
        Vector3 vPos = m_pPos_Finger.GetCurVector3();
        m_pPlane2D_Finger.SetPosition(vPos);

        m_pAlpha_Finger.Update(Time.deltaTime);
        float fAlpha = m_pAlpha_Finger.GetCurScalar();
        m_pPlane2D_Finger.SetColor(new Color(1,1,1,fAlpha));

        m_pTimer_Delay.Update(Time.deltaTime);
    }

    public void OnDone_Timer_StartDelay(TransformerEvent eventValue)
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        if (m_IsSpecialBlock == false)
        {
            pDataStack.m_pSlotManager.OnBlockSwap(m_pSlot_01, m_pSlot_02);

            if (InGameInfo.Instance.m_pSelectModeSlot != null)
            {
                InGameInfo.Instance.m_pSelectModeSlot.m_pGameObject_SelectMode.SetActive(false);
                InGameInfo.Instance.m_pSelectModeSlot = null;
            }

            eventValue = new TransformerEvent_Vector3(GameDefine.ms_fBlockChnageSlotTime, m_pSlot_02.GetPosition());
            m_pPos_Finger.AddEvent(eventValue);
            m_pPos_Finger.SetCallback(null, OnDone_Pos_Finger);
            m_pPos_Finger.OnPlay();
        }
        else
        {
            pDataStack.m_pSlotManager.RemoveSpecialItemBlock(m_pSlot_SpecialBlock);
            EventDelegateManager.Instance.OnInGame_BeginBlockSwap();

            m_pTimer_Delay.OnReset();

            eventValue = new TransformerEvent_Timer(0.3f);
            m_pTimer_Delay.AddEvent(eventValue);
            m_pTimer_Delay.SetCallback(null, OnDone_Pos_Finger);
            m_pTimer_Delay.OnPlay();
        }
    }

    public void OnDone_Pos_Finger(TransformerEvent eventValue)
    {
        if (m_IsSpecialBlock == false)
        {
            Vector2 vDir = m_pSlot_02.GetPosition() - m_pSlot_01.GetPosition();
            Vector3 vTargetPos = vDir + m_pSlot_02.GetPosition();

            m_pPos_Finger.OnReset();
            eventValue = new TransformerEvent_Vector3(0, m_pSlot_02.GetPosition());
            m_pPos_Finger.AddEvent(eventValue);
            eventValue = new TransformerEvent_Vector3(GameDefine.ms_fBlockChnageSlotTime, vTargetPos);
            m_pPos_Finger.AddEvent(eventValue);
            m_pPos_Finger.SetCallback(null, OnDone_Pos_Finger);
            m_pPos_Finger.OnPlay();

            eventValue = new TransformerEvent_Scalar(GameDefine.ms_fBlockChnageSlotTime, 0);
            m_pAlpha_Finger.AddEvent(eventValue);
            m_pAlpha_Finger.SetCallback(null, OnDone_Alpha_Finger);
            m_pAlpha_Finger.OnPlay();
        }
        else
        {
            eventValue = new TransformerEvent_Scalar(GameDefine.ms_fBlockChnageSlotTime, 0);
            m_pAlpha_Finger.AddEvent(eventValue);
            m_pAlpha_Finger.SetCallback(null, OnDone_Alpha_Finger);
            m_pAlpha_Finger.OnPlay();
        }
    }

    public void OnDone_Alpha_Finger(TransformerEvent eventValue)
    {
        OnDone();
    }
}
