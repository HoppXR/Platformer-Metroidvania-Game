using UnityEngine;

public class Park2toFight2 : MonoBehaviour
{
    private BossStateManager bossStateManager;
    private BossAIManager boss;

    private void Start()
    {
        bossStateManager = FindFirstObjectByType<BossStateManager>();
        boss = FindFirstObjectByType<BossAIManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boss.SetState(BossAIManager.BossState.Chase);
            bossStateManager.SetState(BossStateManager.BossState.Fight2);
            gameObject.SetActive(false);
        }
    }
}