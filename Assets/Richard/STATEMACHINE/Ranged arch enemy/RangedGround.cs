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

    public GameObject projectilePrefab; // The projectile prefab to shoot
    public float baseProjectileSpeed; // Base speed of the projectile
    public float heightMultiplier; // Multiplier for height adjustment
    public float distanceMultiplier; // Multiplier for distance calculation

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI = animator.GetComponentInParent<NavMeshAgent>();
        AI.speed = 0f;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StopChasing = animator.GetFloat("RangeToStopChasing");

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
            Debug.Log("Exiting");
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
        
        // Shoot the projectile
        ShootProjectile(animator.transform.position, player.position);
    }

    void FacePlayer(Animator animator)
    {
        Vector3 direction = (player.position - animator.transform.position).normalized;
        
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        
        animator.transform.rotation = Quaternion.Slerp(
            animator.transform.rotation,
            targetRotation,
            Time.deltaTime * 5f // Adjust the speed of rotation
        );
    }

    void ShootProjectile(Vector3 startPosition, Vector3 targetPosition)
    {
        // Instantiate the projectile at the enemy's position
        GameObject projectile = Instantiate(projectilePrefab, startPosition, Quaternion.identity);

        // Calculate the direction and apply the arc using a Rigidbody
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            // Calculate the horizontal distance
            float distance = Vector3.Distance(new Vector3(startPosition.x, 0, startPosition.z), new Vector3(targetPosition.x, 0, targetPosition.z));
            
            // Calculate the height based on vertical difference between enemy and player
            float heightDifference = targetPosition.y - startPosition.y;
            
            // Adjust trajectory based on distance and height difference
            Vector3 direction = (targetPosition - startPosition).normalized;
            float height = heightDifference * heightMultiplier; // Adjust height based on the difference in height
            Vector3 adjustedDirection = new Vector3(direction.x, height, direction.z);

            // Apply the velocity to the Rigidbody
            rb.velocity = adjustedDirection * (baseProjectileSpeed + distance * distanceMultiplier); // Adjust speed based on distance
        }
    }
}
