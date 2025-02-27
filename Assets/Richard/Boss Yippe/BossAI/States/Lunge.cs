using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Lunge : BossAIState
{
    private Transform player;
    private NavMeshAgent navAgent;
    private bool hasLunged = false;
    private float lungeSpeed = 25f;
    private float lungeDuration = 0.3f;
    private float postLungeWait = 1f;

    public Lunge(BossAIManager boss) : base(boss) 
    {
        navAgent = boss.GetComponent<NavMeshAgent>();
    }

    public override void EnterState()
    {
        player = GameObject.FindWithTag("Player").transform;
        hasLunged = false;

        if (navAgent != null)
        {
            navAgent.isStopped = true;
        }

        boss.StartCoroutine(PerformLunge());
    }

    private IEnumerator PerformLunge()
    {
        if (player == null) yield break;

        Vector3 lungeDirection = (player.position - boss.transform.position).normalized;
        float elapsedTime = 0f;

        while (elapsedTime < lungeDuration)
        {
            boss.transform.position += lungeDirection * (lungeSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        hasLunged = true;

        yield return new WaitForSeconds(postLungeWait);

        boss.SetState(BossAIManager.BossState.Attack);

        if (navAgent != null)
        {
            navAgent.isStopped = false;
        }
    }

    public override void StateUpdate() { }

    public override void ExitState() { }
}