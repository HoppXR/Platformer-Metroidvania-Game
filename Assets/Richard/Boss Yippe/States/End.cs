using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class End : BaseState
{
    private BossStateManager bossManager;

    public End(BossStateManager bossManager) : base(bossManager) 
    {
        this.bossManager = bossManager;
    }

    public override void EnterState()
    {
        //Debug.Log("Game end or something");
        
        if (bossManager.bossAI != null)
        {
            bossManager.bossAI.GetComponent<NavMeshAgent>().enabled = false;
            bossManager.bossAI.GetComponent<ProjectileVolley>().enabled = false;
            bossManager.bossAI.GetComponent<DashAttack>().enabled = false;
            bossManager.bossAI.GetComponent<GroundPound>().enabled = false;
            bossManager.bossAI.enabled = false;
        }
        
        bossManager.bossAI.StartCoroutine(DelayedEnd());
    }

    private IEnumerator DelayedEnd()
    {
        yield return new WaitForSeconds(2f);
        
        // Placeholder for whatever happens after the boss dies
    }

    public override void StateUpdate() { }

    public override void ExitState() { }
}