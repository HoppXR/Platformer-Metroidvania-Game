using UnityEngine;

public class Park2toFight2 : MonoBehaviour
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
        if (other.CompareTag("Player") && bossStateManager != null)
        {
            bossStateManager.SetState(BossStateManager.BossState.Fight2);
            gameObject.SetActive(false);
        }
    }
}