using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Idle : BossAIState
{
    private NavMeshAgent navAgent;

    public Idle(BossAIManager boss) : base(boss) 
    {
        navAgent = boss.GetComponent<NavMeshAgent>();
    }

    public override void EnterState()
    {
        Debug.Log("Boss is now Idle");
        if (navAgent != null)
        {
            navAgent.speed = 0;
            navAgent.isStopped = true;
        }
        boss.StartCoroutine(TransitionState());
    }

    private IEnumerator TransitionState()
    {
        yield return new WaitForSeconds(2f);
        
        if (boss.currentPhase == BossAIManager.BossPhase.Phase1)
        {
            boss.SetState(BossAIManager.BossState.Attack);
        }
        else
        {
            boss.SetState(BossAIManager.BossState.Chase);
        }
    }

    public override void StateUpdate() { }

    public override void ExitState()
    {
        if (navAgent != null)
        {
            navAgent.isStopped = false;
        }
    }
}