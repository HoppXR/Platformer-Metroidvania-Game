using UnityEngine;
using UnityEngine.AI;

public class Fight2 : BaseState
{
    private BossStateManager bossManager;
    private BossHealth bossHealth;

    public Fight2(BossStateManager bossManager) : base(bossManager) 
    {
        this.bossManager = bossManager;
    }

    public override void EnterState()
    {
        Debug.Log("Entered Fight 2");

        if (bossManager.bossAI != null)
        {
            bossManager.bossAI.enabled = true;
            bossManager.bossAI.GetComponent<NavMeshAgent>().enabled = true;
            bossManager.bossAI.GetComponent<ProjectileVolley>().enabled = true;
            bossManager.bossAI.GetComponent<DashAttack>().enabled = true;
            bossManager.bossAI.GetComponent<GroundPound>().enabled = true;

            bossHealth = bossManager.bossAI.GetComponent<BossHealth>();

            if (bossHealth != null)
            {
                bossHealth.HealBoss(35); // Heal boss by 35 HP
            }
        }
        
        if (bossManager.arenaColliders != null)
        {
            bossManager.arenaColliders.SetActive(true);
        }
    }

    public override void StateUpdate() { }

    public override void ExitState()
    {
        Debug.Log("Exiting Fight 2");
    }
}