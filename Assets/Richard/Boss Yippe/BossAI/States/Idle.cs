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
        
        boss.transform.position = new Vector3(-12.6899996f,52f,-177.639999f);

        if (navAgent != null && navAgent.isOnNavMesh)
        {
            if (!navAgent.isStopped)
            {
                navAgent.speed = 0;
                navAgent.isStopped = true;
            }
        }

        boss.StartCoroutine(TransitionState());
    }

    public override void StateUpdate()
    {
        LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        if (boss.player != null)
        {
            Vector3 direction = (boss.player.position - boss.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, lookRotation, Time.unscaledDeltaTime * 3f);
        }
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

    public override void ExitState()
    {
        if (navAgent != null && navAgent.isOnNavMesh)
        {
            if (navAgent.isStopped)
            {
                navAgent.isStopped = false;
            }
        }
    }
}