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
        
        if (bossManager.arenaColliders != null)
        {
            bossManager.arenaColliders.SetActive(true);
        }
    }

    public override void StateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T)) 
        {
            bossManager.SetState(BossStateManager.BossState.Transition);
        }
    }

    public override void ExitState()
    {
    }
}