using System;
using UnityEngine;

public class Parkour1ToFight1 : MonoBehaviour
{
    private BossStateManager bossStateManager;
    private void Start()
    {
        bossStateManager = FindFirstObjectByType<BossStateManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Phase 1 Triggered");
            bossStateManager.SetState(BossStateManager.BossState.Fight1);
            gameObject.SetActive(false);
        }
    }
}