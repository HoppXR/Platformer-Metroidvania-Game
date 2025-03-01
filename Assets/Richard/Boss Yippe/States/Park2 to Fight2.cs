using UnityEngine;

public class Park2toFight2 : MonoBehaviour
{
    private BossStateManager bossStateManager;
    private BossAIManager bossAI;

    public GameObject arenaBarriers;

    private void Start()
    {
        bossStateManager = FindObjectOfType<BossStateManager>();
        bossAI = FindObjectOfType<BossAIManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (bossAI != null)
            {
                bossAI.enabled = true;
                bossAI.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
                bossAI.GetComponent<ProjectileVolley>().enabled = true;
                bossAI.GetComponent<DashAttack>().enabled = true;
                bossAI.GetComponent<GroundPound>().enabled = true;
            }
            if (arenaBarriers != null)
            {
                arenaBarriers.SetActive(true);
            }
            if (bossStateManager != null)
            {
                bossStateManager.SetState(BossStateManager.BossState.Fight2);
            }
            gameObject.SetActive(false);
        }
    }
}