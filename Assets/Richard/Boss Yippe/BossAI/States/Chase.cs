using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Chase : BossAIState
{
    private Transform player;
    private NavMeshAgent navAgent;
    private float chaseDuration = 6f;
    private float chaseTimer = 0f;
    private float lungeDistance = 7f;

    public Chase(BossAIManager boss) : base(boss) 
    {
        navAgent = boss.GetComponent<NavMeshAgent>();
    }

    public override void EnterState()
    {
        player = GameObject.FindWithTag("Player").transform;
        chaseTimer = 0f;

        if (navAgent != null)
        {
            navAgent.isStopped = false;
            navAgent.speed = 5f;
        }
    }

    public override void StateUpdate()
    {
        if (player == null) return;

        chaseTimer += Time.unscaledDeltaTime;
        
        if (navAgent != null)
        {
            navAgent.SetDestination(player.position);
        }

        float distance = Vector3.Distance(boss.transform.position, player.position);
        
        if (distance < lungeDistance)
        {
            boss.SetState(BossAIManager.BossState.Lunge);
            return;
        }
        
        if (chaseTimer >= chaseDuration)
        {
            boss.SetState(BossAIManager.BossState.Attack);
        }
    }

    public override void ExitState()
    {
        if (navAgent != null)
        {
            navAgent.ResetPath();
        }
    }
}