using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainGame : MonoBehaviour
{
    protected abstract void Initialize();
    protected abstract void SlotManagerCreateBefore();
    protected abstract void SlotManagerCreateAfter();
    protected abstract void Destroy();
    protected abstract void Inner_Update();
    protected abstract void Inner_LateUpdate();

    protected SlotManager m_pSlotManager;

    void Awake()
    {
        Initialize();
    }

    void Start()
    {
        SlotManagerCreateBefore();

        GameObject ob;

        ob = new GameObject("SlotManager");
        ob.transform.position = Vector3.zero;
        ob.transform.eulerAngles = Vector3.zero;
        ob.transform.localScale = Vector3.one;
        m_pSlotManager = ob.AddComponent<SlotManager>();
        m_pSlotManager.SetMainGame(this);

        SlotManagerCreateAfter();

        StartCoroutine(InitializeSlotManager());
    }

    private IEnumerator InitializeSlotManager()
    {
        if (m_pSlotManager.m_IsInitialize == false)
        {
            yield return null;
        }

        m_pSlotManager.CreateMap();
        m_pSlotManager.Initialize_PossibleMoveSlotCheck();
    }

    private void OnDestroy()
	{
        Destroy();
    }

	void Update()
    {
        Inner_Update();
    }

	private void LateUpdate()
	{
		Inner_LateUpdate();
	}

	public virtual void OnSlotManagerInitializeDone() { }
    public virtual void OnPossibleBlockSwap(Slot pSlot_01, Slot pSlot_02) { }
    public virtual void OnMatchSlot(Dictionary<int, Slot> slotTable, eBlockType eBlockType = eBlockType.Empty) { }
    public virtual void OnDoneSlotMoveAndCreate() { }
    public virtual void OnDoneCheckRemoveSlot() { }
    public virtual void OnMatchTurnComplete() { }
    public virtual void OnBeforeBlockDying(Slot pSlot) { }
    public virtual void OnAfterBlockDying(Slot pSlot) { }
    public virtual void OnDoneAllBlockDying() { }
    public virtual void OnBeforeChangeBlockShape(Slot pSlot, eSpecialItem specialItem) { }
    public virtual void OnAfterChangeBlockShape(Slot pSlot, eSpecialItem specialItem) { }

    public virtual void OnSlotMvoe_SlotFixObject(Slot pFromSlot, Slot pToSlot, SlotFixObject pSlotFixObject, float fMoveTime)
    {
        if (pSlotFixObject.GetSlotFixObjectType() == eSlotFixObjectType.Espresso)
        {
            SlotFixObject_Espresso pSlotFixObject_Espresso = pSlotFixObject as SlotFixObject_Espresso;

            if (pSlotFixObject_Espresso != null)
            {
                switch (pSlotFixObject_Espresso.GetObjectType())
                {
                    case eObjectType.Minion:
                    case eObjectType.MinionBoss:
                        {
                            SlotFixObject_Minion pSlotFixObject_Minion = pSlotFixObject_Espresso as SlotFixObject_Minion;

                            if (pSlotFixObject_Minion != null && pSlotFixObject_Minion.GetMinionType() == eMinionType.PlayerSummonUnit)
                            {
                                MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

                                pDataStack.m_PlayerSummonUnitTable.Remove(pFromSlot.GetSlotIndex());

                                pFromSlot.PopSlotFixObject(pSlotFixObject_Minion);

                                if (pToSlot != null)
                                {
                                    pToSlot.AddSlotFixObject(pSlotFixObject_Minion);
                                    pSlotFixObject_Minion.SetSlot(pToSlot);
                                    pToSlot.SetPossibleMove(true);
                                }

                                if (pDataStack.m_PlayerSummonUnitTable.ContainsKey(pToSlot.GetSlotIndex()) == true)
                                {
                                    pDataStack.m_PlayerSummonUnitTable.Remove(pToSlot.GetSlotIndex());
                                }
                                pDataStack.m_PlayerSummonUnitTable.Add(pToSlot.GetSlotIndex(), pSlotFixObject_Minion);
                            }
                        }
                        break;
                }
            }
        }
    }

    public virtual void SetColor_RndSlotBlock(Color color, float fStartDelay, float fTransTime)
    {
        MainGame_DataStack pDataStack = DataStackManager.Instance.Find<MainGame_DataStack>();

        Dictionary<int, Slot> slotTable = pDataStack.m_pSlotManager.GetSlotTable();

        foreach (KeyValuePair<int, Slot> item in slotTable)
        {
            item.Value.SetCreateColor_RndSlotBlock(color);

            if (item.Value.GetSlotBlock() != null)
            {
                item.Value.GetSlotBlock().SetColor(color, fStartDelay, fTransTime);
            }
        }
    }
}
