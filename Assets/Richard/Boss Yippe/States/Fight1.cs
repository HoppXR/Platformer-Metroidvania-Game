using UnityEngine;
using UnityEngine.AI;

public class Fight1 : BaseState
{
    private BossStateManager bossManager;

    public Fight1(BossStateManager bossManager) : base(bossManager) 
    {
        this.bossManager = bossManager;
    }

    public override void EnterState()
    {
        if (bossManager.bossAI != null)
        {
            bossManager.bossAI.enabled = true;
            
            bossManager.bossAI.GetComponent<NavMeshAgent>().enabled = true;
            bossManager.bossAI.GetComponent<ProjectileVolley>().enabled = true;
            bossManager.bossAI.GetComponent<DashAttack>().enabled = true;
            bossManager.bossAI.GetComponent<GroundPound>().enabled = true;
        }

        bossManager.bossHealthBar.SetActive(true);
        
        if (bossManager.arenaColliders != null)
        {
            bossManager.arenaColliders.SetActive(true);
        }
    }

    public override void StateUpdate()
    {
    }

    public override void ExitState()
    {
    }
}