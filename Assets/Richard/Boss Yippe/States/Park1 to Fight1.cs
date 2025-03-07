using UnityEngine;

public class Parkour1ToFight1 : MonoBehaviour
{
    private BossStateManager bossStateManager;

    private void Start()
    {
        bossStateManager = FindObjectOfType<BossStateManager>();
        if (bossStateManager.bossAI != null)
        {
            bossStateManager.bossAI.StopAllCoroutines();
            bossStateManager.bossAI.dashAttack?.StopDash();
            bossStateManager.bossAI.projectileVolley?.StopVolley();
            bossStateManager.bossAI.groundPound?.StopGroundPound();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Phase 1 Triggered");
            bossStateManager.SetState(BossStateManager.BossState.Fight1);
            gameObject.SetActive(false);
        }
    }
}