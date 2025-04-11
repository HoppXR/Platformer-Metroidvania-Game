using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Idle : BossAIState
{
    private NavMeshAgent navAgent;
    private BossStateManager bossStateManager;

    public Idle(BossAIManager boss, BossStateManager bossStateManager) : base(boss) 
    {
        this.bossStateManager = bossStateManager;
        navAgent = boss.GetComponent<NavMeshAgent>();
    }

    public override void EnterState()
    {
        //Debug.Log("Boss is now Idle");
        
        boss.transform.position = new Vector3(-12.6899996f,52.1f,-177.639999f);

        if (bossStateManager.currentState != BossStateManager.BossState.Transition)
        {
            boss.StartCoroutine(TransitionState());
        }

        if (navAgent != null)
        {
            navAgent.speed = 0;
        }
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