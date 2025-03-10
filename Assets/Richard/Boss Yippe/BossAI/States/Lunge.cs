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
    private float preLungeWait = 1f;
    private float postLungeWait = 2f;
    private Vector3 resetPosition = new Vector3(-12.6899996f,52f,-177.639999f);

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
    
        yield return new WaitForSeconds(preLungeWait);
        Vector3 lungeDirection = (player.position - boss.transform.position);
        lungeDirection.y = 0;
        lungeDirection = lungeDirection.normalized;

        float elapsedTime = 0f;
        float groundY = boss.transform.position.y;

        while (elapsedTime < lungeDuration)
        {
            boss.transform.position += lungeDirection * (lungeSpeed * Time.unscaledDeltaTime);
            boss.transform.position = new Vector3(boss.transform.position.x, groundY, boss.transform.position.z);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        hasLunged = true;
        yield return new WaitForSeconds(postLungeWait);

        boss.SetState(BossAIManager.BossState.Attack);
    }
    
    public override void StateUpdate()
    {
    }


    public override void ExitState()
    {
        boss.transform.position = resetPosition;

        if (navAgent != null)
        {
            navAgent.isStopped = false;
        }
    }
}