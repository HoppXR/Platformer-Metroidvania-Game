using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedAttack : StateMachineBehaviour
{
    private NavMeshAgent AI;
    public Transform player;
    private float StopChasing = 16f;
    private AttackCoolDowns attackCoolDowns;
    private Transform attackTransform;

    public GameObject projectilePrefab;
    public GameObject indicatorPrefab;
    public float baseProjectileSpeed;
    public float heightMultiplier;
    public float distanceMultiplier;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI = animator.GetComponentInParent<NavMeshAgent>();
        AI.speed = 0f;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StopChasing = animator.GetFloat("RangeToStopChasing");
        attackTransform = animator.transform.Find("Attack");

        // Reference AttackCoolDowns script
        attackCoolDowns = animator.GetComponent<AttackCoolDowns>();

        animator.SetBool("isAttacking", true);
        animator.SetBool("hasAttacked", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CheckForAttack(animator);
        
        FacePlayer(animator);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isAttacking", false);
        animator.SetBool("hasAttacked", false);
    }

    void CheckForAttack(Animator animator)
    {
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance >= StopChasing)
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("hasAttacked", true);
        }
        else if (attackCoolDowns != null && attackCoolDowns.CanAttack)
        {
            Attack(animator);
        }
    }

    void Attack(Animator animator)
    {
        Debug.Log("Attacking player!");
        attackCoolDowns.StartCooldown();
        Vector3 targetPosition = ChooseTargetPosition();
        ShootProjectile(animator.transform.position, targetPosition);
    }

    Vector3 ChooseTargetPosition()
    {
        float shootAtPredictionChance = 0.5f;
        Transform predictionTarget = GameObject.Find("EnemyPredictionPoint").transform;

        if (Random.value <= shootAtPredictionChance && predictionTarget != null)
        {
            return predictionTarget.position;
        }
        else
        {
            return player.position;
        }
    }

    void FacePlayer(Animator animator)
    {
        Vector3 direction = (player.position - animator.transform.position).normalized;
        
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        
        animator.transform.rotation = Quaternion.Slerp(
            animator.transform.rotation,
            targetRotation,
            Time.deltaTime * 5f
        );
    }

    void ShootProjectile(Vector3 startPosition, Vector3 targetPosition)
    {
        GameObject projectile = Instantiate(projectilePrefab, attackTransform.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction = targetPosition - startPosition;
            float distance = Mathf.Max(0.1f, Vector3.Distance(new Vector3(startPosition.x, 0, startPosition.z), 
                new Vector3(targetPosition.x, 0, targetPosition.z)));

            float heightDifference = targetPosition.y - startPosition.y;
            float gravity = Physics.gravity.magnitude;

            float angle = 45f * Mathf.Deg2Rad;
            float tanAngle = Mathf.Tan(angle);
            float denominator = 2 * (distance * tanAngle - heightDifference);
            if (Mathf.Abs(denominator) < 0.1f) denominator = 0.1f;

            float initialSpeed = Mathf.Sqrt((gravity * distance * distance) / denominator) * baseProjectileSpeed;
            initialSpeed = Mathf.Clamp(initialSpeed, 5f, 100f);

            float Vy = (initialSpeed * Mathf.Sin(angle)) * heightMultiplier;
            float Vx = initialSpeed * Mathf.Cos(angle);

            Vector3 flatDirection = new Vector3(direction.x, 0, direction.z).normalized;
            Vector3 velocity = (flatDirection * Vx) + (Vector3.up * Vy);

            if (!float.IsNaN(velocity.x) && !float.IsNaN(velocity.y) && !float.IsNaN(velocity.z))
            {
                rb.velocity = velocity;
            }
            SpawnIndicatorPrefab(targetPosition);
        }
    }
    
    void SpawnIndicatorPrefab(Vector3 position)
    {
        if (indicatorPrefab != null)
        {
            Quaternion fixedRotation = Quaternion.Euler(90f, 0f, 0f);
            GameObject indicator = Instantiate(indicatorPrefab, position, fixedRotation);
            Destroy(indicator, 2f);
        }
    }


}
