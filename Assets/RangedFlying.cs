using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedFlying : StateMachineBehaviour
{
    private NavMeshAgent AI;
    public Transform player;
    public Transform predictPlayer;
    private Transform enemyMesh;
    public GameObject projectilePrefab;
    private float StopChasing = 16f;
    private AttackCoolDowns attackCoolDowns;
    
    private float shootAtPlayerChance = 0.5f; 
    private float baseProjectileSpeed = 20f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI = animator.GetComponentInParent<NavMeshAgent>();
        AI.speed = 0f;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        predictPlayer = GameObject.Find("Player Prediction").transform;
        StopChasing = animator.GetFloat("RangeToStopChasing");
        attackCoolDowns = animator.GetComponent<AttackCoolDowns>();
        enemyMesh = animator.transform.Find("mesh");

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
        Vector3 targetPosition = ChooseTargetPosition();
        ShootProjectile(animator.transform.position, targetPosition);
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
    
    Vector3 ChooseTargetPosition()
    {
        float randomChance = Random.value;
        if (randomChance <= shootAtPlayerChance)
        {
            //Debug.Log("no");
            return player.position;
        }
        else
        {
            //Debug.Log("predictplayer");
            return predictPlayer.position;
        }
    }

    void ShootProjectile(Vector3 startPosition, Vector3 targetPosition)
    {
        GameObject projectile = Instantiate(projectilePrefab, enemyMesh.position, Quaternion.identity);
        
        Vector3 direction = targetPosition - enemyMesh.position; 
        direction.y = targetPosition.y - enemyMesh.position.y; 
        
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction.normalized * baseProjectileSpeed;
        }
    }
}
