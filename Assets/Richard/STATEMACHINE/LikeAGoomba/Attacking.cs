using Enemy;
using UnityEngine;
using UnityEngine.AI;

public class Attacking : StateMachineBehaviour
{
    private Transform player;
    private Vector3 startPosition;
    private Vector3 dashDirection;
    public float dashSpeed = 10f;
    public float dashDistance = 5f;
    private float dashTravelled = 0f;
    private NavMeshAgent AI;
    private EnemyDamage enemyDamage;
    private AttackDelay attackDelay;
    private bool attackStarted = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI = animator.GetComponent<NavMeshAgent>();
        AI.speed = 0f;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPosition = animator.transform.position;
        dashDirection = (player.position - startPosition).normalized;
        dashTravelled = 0f;
        attackStarted = false;

        enemyDamage = animator.GetComponent<EnemyDamage>();
        if (enemyDamage != null)
        {
            enemyDamage.enabled = true;
        }

        attackDelay = animator.GetComponent<AttackDelay>();
        if (attackDelay != null)
        {
            attackDelay.DoAttack(() => attackStarted = true);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!attackStarted) return;

        if (dashTravelled < dashDistance)
        {
            float moveDistance = dashSpeed * Time.deltaTime;
            float remainingDistance = dashDistance - dashTravelled;
            float moveAmount = Mathf.Min(moveDistance, remainingDistance);
            animator.transform.position += dashDirection * moveAmount;
            dashTravelled += moveAmount;
        }
        else
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("hasAttacked", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("hasAttacked", false);
        if (enemyDamage != null)
        {
            enemyDamage.enabled = false;
        }
    }
}
