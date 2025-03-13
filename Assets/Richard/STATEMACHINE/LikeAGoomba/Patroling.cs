using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patroling : StateMachineBehaviour
{
    private NavMeshAgent AI;
    private Transform player;

    [SerializeField] private float chaseRange = 8f;
    private float patrolRadius = 8f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        AI = animator.GetComponent<NavMeshAgent>();
        AI.speed = 5f;

        // Set a random destination within a 360-degree patrol radius
        SetRandomDestination(animator);
        animator.SetBool("isChasing", false);
        animator.SetBool("canAttack", true);
        
        chaseRange = animator.GetFloat("RangeToChasePlayer");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Check if the enemy reached its patrol destination
        if (AI.remainingDistance <= AI.stoppingDistance)
        {
            // Switch to idle state after reaching the destination
            animator.SetBool("isPatroling", false);
        }

        // Check if the player is within chase range
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance < chaseRange)
        {
            animator.SetBool("isChasing", true);
        }
    }

    // Sets a random destination within a 360-degree radius
    private void SetRandomDestination(Animator animator)
    {
        // Generate a random point within a circle
        Vector2 randomPoint = Random.insideUnitCircle * patrolRadius;

        // Convert to a 3D point and add to the current position
        Vector3 randomDestination = new Vector3(randomPoint.x, 0, randomPoint.y) + animator.transform.position;

        // Check if the point is on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDestination, out hit, patrolRadius, NavMesh.AllAreas))
        {
            // Set destination to the valid point on the NavMesh
            AI.SetDestination(hit.position);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stop movement on exit
        AI.SetDestination(AI.transform.position);
    }
}
