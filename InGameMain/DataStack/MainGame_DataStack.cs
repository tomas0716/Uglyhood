using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame_DataStack
{
    public MainGame                 m_pMainGame                         = null;
    public SlotManager              m_pSlotManager                      = null;
    public ExcelData_StageInfo      m_pStageInfo                        = null;
    public InGameStageMapData       m_pInGameStageMapData               = null;
    public TurnStatePattern         m_pTurnStatePattern                 = null;

    public InGame_CloudManager      m_pCloudManager                     = null;

    public int                      m_nCurrShildPoint                   = 0;        // 실드 포인트
    public int                      m_nCurrObjectiveCount               = 0;        // 적 처치 목표 수
    public int                      m_nPlayerTurnCount                  = 0;
    public int                      m_nEnemyTurnCount                   = 0;

    public int                      m_nPreCheckEnemyColonyEffectCount   = 0;
    public int                      m_nEnemyColonyEffectCount           = 0;

    public int                      m_nCombo                            = 0;
    public int                      m_nMaxCombo                         = 0;
    public int                      m_nBlockProjectileCount             = 0;
    public int                      m_nPlayerCharacterProjectileCount   = 0;
    public int                      m_nPlayerCharacterPassiveProjectileCount = 0;
    public int                      m_nBoosterItemProjectileCount       = 0;

    public int                      m_nReflectDamageProjectileCount     = 0;

    public int                      m_nSPChargeProjectileCount          = 0;
    public int                      m_nEnemyMinionProjectileCount       = 0;

    public int []                   m_nPlayerCharacterAttackValue   = new int[(int)eElement.Max];

    public int                      m_nBreakColonyCount                 = 0;

    public float                    m_fKnockBackDelayTime               = 0;

    public int                      m_nBoosterItemUseCount              = 0;
    public bool                     m_IsUseAutoCombat                   = false;
    public int                      m_nPlayerSkillCount = 0;

    public SlotFixObject_PlayerCharacter    m_pPlayerCharacter_SkillTrigger = null;
    public SlotFixObject_Minion             m_pMinion_SkillTrigger          = null;

    public bool                     m_IsCurrRefillGuideState            = false;

    public Dictionary<int, SlotFixObject_PlayerCharacter>   m_PlayerCharacterTable      = new Dictionary<int, SlotFixObject_PlayerCharacter>();     // Key : SlotIndex
    public Dictionary<int, SlotFixObject_EnemyColony>       m_EnemyColonyTable          = new Dictionary<int, SlotFixObject_EnemyColony>();         // Key : SlotIndex
    public Dictionary<int, SlotFixObject_Minion>            m_EnemyMinionTable          = new Dictionary<int, SlotFixObject_Minion>();              // Key : SlotIndex
    public List<SlotFixObject_Minion>                       m_EnemyMinionBossAppearList = new List<SlotFixObject_Minion>();

    public Dictionary<int, SlotFixObject_Minion>            m_PlayerSummonUnitTable     = new Dictionary<int, SlotFixObject_Minion>();              // Key : SlotIndex
    public Dictionary<int, SlotFixObject_Minion>            m_EnemySummonUnitTable      = new Dictionary<int, SlotFixObject_Minion>();              // Key : SlotIndex

    // EnemyMinion Spawn Info
    public Dictionary<ExcelData_Stage_EnemySpawnInfo, int>  m_EnemySpawnInfoCountTable          = new Dictionary<ExcelData_Stage_EnemySpawnInfo, int>();     // Value : Spawn Count
    public Dictionary<ExcelData_Stage_EnemySpawnInfo, int>  m_EnemySpawnInfoIntervalTurnTable   = new Dictionary<ExcelData_Stage_EnemySpawnInfo, int>();     // Value : Spawn Count

    //public Dictionary<Slot, InGame_Highlight>               m_DamageTarget_HighlightTable       = new Dictionary<Slot, InGame_Highlight>();
    public Dictionary<InGame_Highlight, Slot>               m_DamageTarget_HighlightTable       = new Dictionary<InGame_Highlight, Slot>();

    // EnemyColony
    public Dictionary<Slot, int>                            m_EnemyColonyCreateTable            = new Dictionary<Slot, int>();        // Value : ExcelData_EnemyColonyInfo ID

    public Dictionary<int, SlotFixObject_PlayerCharacter>   m_OtherPlayerCharacterTable = new Dictionary<int, SlotFixObject_PlayerCharacter>();     // Key : SlotIndex

    public MainGame_DataStack()
    {
    }

    public bool IsPlayerCharacterAllDead(eOwner eOwn)
    {
        bool IsAllDead = true;

        if (eOwn == eOwner.My)
        {
            foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_PlayerCharacterTable)
            {
                if (item.Value.IsDead() == false)
                {
                    IsAllDead = false;
                }
            }
        }
        else
        {
            foreach (KeyValuePair<int, SlotFixObject_PlayerCharacter> item in m_OtherPlayerCharacterTable)
            {
                if (item.Value.IsDead() == false)
                {
                    IsAllDead = false;
                }
            }
        }

        return IsAllDead;
    }
}
