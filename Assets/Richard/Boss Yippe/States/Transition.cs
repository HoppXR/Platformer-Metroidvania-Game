using System.Collections;
using Enemy;
using UnityEngine;

public class Transition : BaseState
{
    private float timer = 0f;

    public Transition(BossStateManager bossManager) : base(bossManager) { }

    public override void EnterState()
    {
        //Debug.Log("Entered Transition");
        if (bossManager.bossAI != null)
        {
            bossManager.bossAI.SetState(BossAIManager.BossState.Idle);
        }
    
        bossManager.bossAI.transform.position = new Vector3(-12.6899996f, 52f, -177.639999f);
        bossManager.bossAI.transform.rotation = Quaternion.Euler(0, 0, 0);
        
        bossManager.transitionCinemachineCamera.Priority = 20;
        bossManager.playerCinemachineCamera.Priority = 10;
        
        
        
        if (bossManager.bossAI != null)
        {
            var navAgent = bossManager.bossAI.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navAgent != null) navAgent.enabled = false;

            bossManager.bossAI.enabled = false;
            bossManager.bossAI.GetComponent<ProjectileVolley>().enabled = false;
            bossManager.bossAI.GetComponent<DashAttack>().enabled = false;
            bossManager.bossAI.GetComponent<GroundPound>().enabled = false;
            bossManager.bossAI.playerDamageCollider.enabled = false;
            bossManager.bossAI.collisionDamageCollider.enabled = false;
        }


        if (bossManager.arenaColliders != null) bossManager.arenaColliders.SetActive(false);
        if (bossManager.parkour1 != null) bossManager.parkour1.SetActive(false);
        if (bossManager.parkour2 != null) bossManager.parkour2.SetActive(true);
    }

    public override void StateUpdate()
    {
        timer += Time.unscaledDeltaTime; 

        if (timer > 5f) 
        {
            bossManager.playerCinemachineCamera.Priority = 20;
            bossManager.transitionCinemachineCamera.Priority = 10;

            bossManager.SetState(BossStateManager.BossState.Parkour2);
        }
    }
    
    public override void ExitState()
    {
        //Debug.Log("Exiting Transition");
        bossManager.bossAI.playerDamageCollider.enabled = true;
        bossManager.bossAI.collisionDamageCollider.enabled = true;
    }
}
