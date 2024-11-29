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
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI = animator.GetComponent<NavMeshAgent>();
        AI.speed = 0f;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPosition = animator.transform.position; 
        dashDirection = (player.position - startPosition).normalized; 

        // Reset dashTravelled when entering the state
        dashTravelled = 0f; 

        animator.SetBool("isAttacking", true); 
        animator.SetBool("hasAttacked", false); 
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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
            // Stop moving and end the attack
            animator.SetBool("isAttacking", false);
            animator.SetBool("hasAttacked", true);
        }
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        animator.SetBool("hasAttacked", false); 
    }
}