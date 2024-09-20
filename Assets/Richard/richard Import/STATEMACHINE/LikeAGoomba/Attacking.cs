using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Attacking : StateMachineBehaviour
{
    private Transform player;
    private Vector3 startPosition;
    private Vector3 dashDirection; // Direction of the dash
    public float dashSpeed = 10f;  // Speed of the dash
    public float dashDistance = 5f; // Distance to dash
    private float dashTravelled = 0f; // Distance travelled during dash
    private NavMeshAgent AI;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI = animator.GetComponent<NavMeshAgent>();
        AI.speed = 0f;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPosition = animator.transform.position; // Record the start position
        dashDirection = (player.position - startPosition).normalized; // Initial dash direction

        // Reset dashTravelled when entering the state
        dashTravelled = 0f; 

        animator.SetBool("isAttacking", true); // Ensure attacking state is active
        animator.SetBool("hasAttacked", false); // Reset attack status
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (dashTravelled < dashDistance)
        {
            // Calculate movement for this frame
            float moveDistance = dashSpeed * Time.deltaTime;
            float remainingDistance = dashDistance - dashTravelled;

            // Move the NPC in the dash direction but limit the movement to the remaining distance
            float moveAmount = Mathf.Min(moveDistance, remainingDistance);
            animator.transform.position += dashDirection * moveAmount;
            dashTravelled += moveAmount;
        }
        else
        {
            // Stop moving and end the attack
            animator.SetBool("isAttacking", false);
            animator.SetBool("hasAttacked", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Ensure no movement continues after exiting the state
        animator.SetBool("hasAttacked", false); 
    }
}
