using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_EnemyMinionWarp : GameEvent
{
    private SlotFixObject_Minion    m_pMinion           = null;
    private bool                    m_IsShieldAttack    = false;
    private Slot                    m_pWarpSlot         = null;

    public enum eState
    {
        Hide_Effect,
        Hide_Minion,
        Show_Effect,
        Show_Minion,
    }

    private eState                  m_eState            = eState.Hide_Effect;

    private Transformer_Timer       m_pTimer_State      = new Transformer_Timer();
    //private float []                m_fStateTime        = { 0.3f, 0.2f, 0.3f, 0.12f, };
    private float[]                 m_fStateTime        = { 0.3f * GameDefine.ms_fEnemySlotMoveMultiple,
                                                            0.2f * GameDefine.ms_fEnemySlotMoveMultiple,
                                                            0.3f * GameDefine.ms_fEnemySlotMoveMultiple, 
                                                            0.12f * GameDefine.ms_fEnemySlotMoveMultiple, };

    private Transformer_Vector3     m_pPos              = null;

    public GameEvent_EnemyMinionWarp(SlotFixObject_Minion pMinion, PathFind.Point ptGoal, bool IsShieldAttack)
    {
        m_pMinion = pMinion;
        m_IsShieldAttack = IsShieldAttack;

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        Slot pOriginSlot = m_pMinion.GetSlot();
        pOriginSlot.PopSlotFixObject(m_pMinion);

        switch (pMinion.GetMinionType())
        {
            case eMinionType.EnemyMinion:
                {
                    pDataStack.m_EnemyMinionTable.Remove(m_pMinion.GetSlot().GetSlotIndex());

                    int nSlotIndex = Helper.GetSlotIndex(ptGoal.x, ptGoal.y);
                    m_pWarpSlot = pDataStack.m_pSlotManager.GetSlotTable()[nSlotIndex];

                    if (m_pWarpSlot != null)
                    {
                        m_pWarpSlot.AddSlotFixObject(m_pMinion);
                        m_pMinion.SetSlot(m_pWarpSlot);
                    }

                    if (pDataStack.m_EnemyMinionTable.ContainsKey(m_pWarpSlot.GetSlotIndex()) == true)
                    {
                        pDataStack.m_EnemyMinionTable.Remove(m_pWarpSlot.GetSlotIndex());
                    }
                    pDataStack.m_EnemyMinionTable.Add(m_pWarpSlot.GetSlotIndex(), m_pMinion);
                }
                break;

            case eMinionType.EnemySummonUnit:
                {
                    pDataStack.m_EnemySummonUnitTable.Remove(m_pMinion.GetSlot().GetSlotIndex());

                    int nSlotIndex = Helper.GetSlotIndex(ptGoal.x, ptGoal.y);
                    m_pWarpSlot = pDataStack.m_pSlotManager.GetSlotTable()[nSlotIndex];

                    if (m_pWarpSlot != null)
                    {
                        m_pWarpSlot.AddSlotFixObject(m_pMinion);
                        m_pMinion.SetSlot(m_pWarpSlot);
                    }

                    if (pDataStack.m_EnemySummonUnitTable.ContainsKey(m_pWarpSlot.GetSlotIndex()) == true)
                    {
                        pDataStack.m_EnemySummonUnitTable.Remove(m_pWarpSlot.GetSlotIndex());
                    }
                    pDataStack.m_EnemySummonUnitTable.Add(m_pWarpSlot.GetSlotIndex(), m_pMinion);
                }
                break;
        }

        Helper.OnSoundPlay("INGAME_MINION_MOVE_WARP", false);

        Vector3 vPos = pOriginSlot.GetPosition();
        vPos.z = -(float)ePlaneOrder.Fx_TopLayer;
        ParticleManager.Instance.LoadParticleSystem("FX_UnitWarp", vPos).SetScale(InGameInfo.Instance.m_fInGameScale);

        TransformerEvent eventValue;
        eventValue = new TransformerEvent_Timer(m_fStateTime[(int)m_eState]);
        m_pTimer_State.AddEvent(eventValue);
        m_pTimer_State.SetCallback(null, OnDone_Timer_State);
        m_pTimer_State.OnPlay();

        m_pPos = new Transformer_Vector3(pOriginSlot.GetPosition());
        float fTime = m_fStateTime[(int)eState.Hide_Effect] + m_fStateTime[(int)eState.Hide_Minion] + m_fStateTime[(int)eState.Show_Effect];
        eventValue = new TransformerEvent_Vector3(fTime, pOriginSlot.GetPosition());
        m_pPos.AddEvent(eventValue);
        eventValue = new TransformerEvent_Vector3(fTime, m_pWarpSlot.GetPosition());
        m_pPos.AddEvent(eventValue);
        m_pPos.OnPlay();
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
		m_pPos.Update(Time.deltaTime);
		Vector3 vPos = m_pPos.GetCurVector3();
		m_pMinion.SetPosition(vPos);

		m_pTimer_State.Update(Time.deltaTime);
    }

	private void OnDone_Timer_State(TransformerEvent eventValue)
	{
		switch (m_eState)
		{
			case eState.Hide_Effect:
				{
                    m_eState = eState.Hide_Minion;
                    m_pMinion.GetGameObject().SetActive(false);

                    m_pTimer_State.OnReset();
                    eventValue = new TransformerEvent_Timer(m_fStateTime[(int)m_eState]);
                    m_pTimer_State.AddEvent(eventValue);
                    m_pTimer_State.SetCallback(null, OnDone_Timer_State);
                    m_pTimer_State.OnPlay();
                }
				break;
			case eState.Hide_Minion:
				{
                    Helper.OnSoundPlay("INGAME_MINION_MOVE_WARP", false);

                    m_eState = eState.Show_Effect;

                    Vector3 vPos = m_pWarpSlot.GetPosition();
                    vPos.z = -(float)ePlaneOrder.Fx_TopLayer;
                    ParticleManager.Instance.LoadParticleSystem("FX_UnitWarp", vPos).SetScale(InGameInfo.Instance.m_fInGameScale);

                    m_pTimer_State.OnReset();
                    eventValue = new TransformerEvent_Timer(m_fStateTime[(int)m_eState]);
                    m_pTimer_State.AddEvent(eventValue);
                    m_pTimer_State.SetCallback(null, OnDone_Timer_State);
                    m_pTimer_State.OnPlay();
                }
				break;
			case eState.Show_Effect:
				{
                    m_eState = eState.Show_Minion;
                    m_pMinion.SetPosition(m_pWarpSlot.GetPosition());
                    m_pMinion.GetGameObject().SetActive(true);

                    m_pTimer_State.OnReset();
                    eventValue = new TransformerEvent_Timer(m_fStateTime[(int)m_eState]);
                    m_pTimer_State.AddEvent(eventValue);
                    m_pTimer_State.SetCallback(null, OnDone_Timer_State);
                    m_pTimer_State.OnPlay();
                }
				break;
			case eState.Show_Minion:
				{
                    OnDone_Warp();
                }
				break;
		}
	}

	private void OnDone_Warp()
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        if (m_IsShieldAttack == true)
        {
            pDataStack.m_nCurrShildPoint -= 1;
            if (pDataStack.m_nCurrShildPoint < 0) pDataStack.m_nCurrShildPoint = 0;
            EventDelegateManager.Instance.OnInGame_ChangeShieldPoint(pDataStack.m_nCurrShildPoint);

            switch (m_pMinion.GetMinionType())
            {
                case eMinionType.EnemyMinion:
                    {
                        pDataStack.m_EnemyMinionTable.Remove(m_pMinion.GetSlot().GetSlotIndex());
                    }
                    break;

                case eMinionType.EnemySummonUnit:
                    {
                        pDataStack.m_EnemySummonUnitTable.Remove(m_pMinion.GetSlot().GetSlotIndex());
                    }
                    break;
            }

            m_pMinion.OnDead();

            Helper.OnSoundPlay("INGAME_SHIELD_POINT_HIT", false);
        }

        if (pDataStack.m_nCurrShildPoint > 0 && pDataStack.m_nCurrObjectiveCount <= 0 && pDataStack.m_EnemyMinionTable.Count == 0)
        {
            EventDelegateManager.Instance.OnInGame_GameOver(eGameResult.Success);
        }
        else
        {
            AppInstance.Instance.m_pEventDelegateManager.OnInGame_EnemyMinionMoveAndCreateDone();
        }

        OnDone();
    }
}
