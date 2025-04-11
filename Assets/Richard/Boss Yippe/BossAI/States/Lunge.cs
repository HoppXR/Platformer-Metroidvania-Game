using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Lunge : BossAIState
{
    private Transform _player;
    private NavMeshAgent navAgent;
    private bool _hasLunged;
    private float lungeSpeed = 25f;
    private float lungeDuration = 0.3f;
    private float preLungeWait = 1f;
    private float postLungeWait = 2f;
    private Vector3 resetPosition = new Vector3(-12.6899996f, 52f, -177.639999f);
    private GameObject dashIndicator;

    public Lunge(BossAIManager boss) : base(boss)
    {
        navAgent = boss.GetComponent<NavMeshAgent>();
    }

    public override void EnterState()
    {
        _player = GameObject.FindWithTag("Player").transform;
        Transform gameObject = boss.transform.Find("DashIndicator");
        if (gameObject != null)
            dashIndicator = gameObject.gameObject;
        _hasLunged = false;

        if (navAgent != null)
        {
            navAgent.isStopped = true;
        }

        boss.StartCoroutine(PerformLunge());
    }

    private IEnumerator PerformLunge()
    {
        yield return boss.StartCoroutine(ShowIndicator());

        Vector3 lungeDirection = (_player.position - boss.transform.position);
        lungeDirection.y = 0;
        lungeDirection = lungeDirection.normalized;

        yield return new WaitForSeconds(preLungeWait);

        float elapsedTime = 0f;
        float groundY = boss.transform.position.y;

        while (elapsedTime < lungeDuration)
        {
            boss.transform.position += lungeDirection * (lungeSpeed * Time.unscaledDeltaTime);
            boss.transform.position = new Vector3(boss.transform.position.x, groundY, boss.transform.position.z);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        _hasLunged = true;
        yield return new WaitForSeconds(postLungeWait);

        boss.SetState(BossAIManager.BossState.Attack);
    }

    private IEnumerator ShowIndicator()
    {
        if (dashIndicator != null)
            dashIndicator.SetActive(true);

        yield return new WaitForSeconds(1f);

        if (dashIndicator != null)
            dashIndicator.SetActive(false);
    }

    public override void StateUpdate()
    {
        if (_player != null)
        {
            Vector3 lookDirection = _player.position - boss.transform.position;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
                boss.transform.rotation = Quaternion.LookRotation(lookDirection);
        }
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
