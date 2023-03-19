using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointObject : MonoBehaviour
{
    private Slot            m_pSlot             = null;
    private ParticleSystem  m_pParticleSystem   = null;

    public void Init(Slot pSlot)
    {
        m_pSlot = pSlot;

        GameObject ob;
        ob = Helper.FindChildGameObject(gameObject, "FX_Slot_EnemySpawn");
        m_pParticleSystem = ob.GetComponent<ParticleSystem>();

        EventDelegateManager.Instance.OnEventInGame_EnemyMinionSpawn += OnInGame_EnemyMinionSpawn;
    }

	private void OnDestroy()
	{
        EventDelegateManager.Instance.OnEventInGame_EnemyMinionSpawn -= OnInGame_EnemyMinionSpawn;
    }

    public void OnInGame_EnemyMinionSpawn(Slot pSlot, SlotFixObject_Minion pMinion)
    {
        if (m_pSlot == pSlot)
        {
            m_pParticleSystem.Play();
        }
    }
}
