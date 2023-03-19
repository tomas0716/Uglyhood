using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent_EnemyMinionBossIntro : TComponent<EventArg_Null, EventArg_Null>
{
    private GameObject m_pGameObject_Intro = null;
    private Transformer_Timer m_pTimer_Delay_NextEvent = new Transformer_Timer();

    private GameObject m_pGameObject_BlackLayer = null;

    public TurnComponent_EnemyMinionBossIntro()
    {
        ExcelData_StageInfo pStageInfo = ExcelDataManager.Instance.m_pStage.GetStageInfo_byID(EspressoInfo.Instance.m_nStageID);
        GameObject ob = Resources.Load<GameObject>("2D/Prefabs/Slot_Basic/EnemyMinionBossIntro_BlackLayer");
        m_pGameObject_BlackLayer = GameObject.Instantiate(ob);
        m_pGameObject_BlackLayer.SetActive(false);
        m_pGameObject_BlackLayer.transform.localScale *= AppInstance.Instance.m_fHeightScale;
    }

    public override void OnDestroy()
    {
        if (m_pGameObject_BlackLayer != null)
        {
            GameObject.Destroy(m_pGameObject_BlackLayer);
            m_pGameObject_BlackLayer = null;
        }
    }

    public override void Update()
    {
        m_pTimer_Delay_NextEvent.Update(Time.deltaTime);
    }

    public override void OnEvent(EventArg_Null Arg)
    {
        OutputLog.Log("TurnComponent_EnemyMinionBossIntro : OnEvent");
        InGameTurnLog.Log("TurnComponent_EnemyMinionBossIntro : OnEvent");

        InGameInfo.Instance.m_eTurnComponentType = eTurnComponentType.EnemyBossAppear;
        EventDelegateManager.Instance.OnInGame_ChangeTurnComponent(eTurnComponentType.EnemyBossAppear);

        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        if (pDataStack.m_EnemyMinionBossAppearList.Count != 0)
        {
            GameObject ob = Resources.Load<GameObject>("GUI/Prefabs/InGame/InGameEnemyMinionBossIntro");
            m_pGameObject_Intro = GameObject.Instantiate(ob);
            InGame_EnemyMinionBossIntro_UI pScript = m_pGameObject_Intro.GetComponent<InGame_EnemyMinionBossIntro_UI>();
            pScript.Init(pDataStack.m_EnemyMinionBossAppearList[0].GetUnitInfo());

            ob = Helper.FindChildGameObject(m_pGameObject_Intro, "BossIntro");
            Animator pAnimator = ob.GetComponent<Animator>();

            AnimatorStateInfo animState = pAnimator.GetCurrentAnimatorStateInfo(0);
            float fAnimTime = animState.length;

            m_pTimer_Delay_NextEvent.OnReset();

            TransformerEvent_Timer eventValue;
            eventValue = new TransformerEvent_Timer(fAnimTime - 0.05f);
            m_pTimer_Delay_NextEvent.AddEvent(eventValue);
            m_pTimer_Delay_NextEvent.SetCallback(null, OnDone_Timer_Delay_NextEvent);
            m_pTimer_Delay_NextEvent.OnPlay();

            m_pGameObject_BlackLayer.SetActive(true);

            foreach (SlotFixObject_Minion pMinion in pDataStack.m_EnemyMinionBossAppearList)
            {
                pMinion.SetHighlight();
            }

            Helper.OnSoundPlay("INGAME_BOSS_WARNING", false);
        }
        else
        {
            EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

            OutputLog.Log("TurnComponent_EnemyMinionBossIntro : GetNextEvent().OnEvent(EventArg_Null.Object)");
            GetNextEvent().OnEvent(EventArg_Null.Object);
        }
    }

    private void OnDone_Timer_Delay_NextEvent(TransformerEvent eventValue)
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        m_pGameObject_BlackLayer.SetActive(false);

        foreach (SlotFixObject_Minion pMinion in pDataStack.m_EnemyMinionBossAppearList)
        {
            pMinion.ClearHighlight();
        }

        pDataStack.m_EnemyMinionBossAppearList.Clear();

        if (m_pGameObject_Intro != null)
        {
            GameObject.Destroy(m_pGameObject_Intro);
            m_pGameObject_Intro = null;
        }

        EventDelegateManager.Instance.OnInGame_TurnComponentDone(InGameInfo.Instance.m_eTurnComponentType);

        OutputLog.Log("TurnComponent_EnemyMinionBossIntro : GetNextEvent().OnEvent(EventArg_Null.Object)");
        GetNextEvent().OnEvent(EventArg_Null.Object);
    }

    public int IsProcess()
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        if (pDataStack.m_EnemyMinionBossAppearList.Count != 0)
        {
            pDataStack.m_EnemyMinionBossAppearList.Clear();

            return 1;
        }

        return 0;
    }
}
