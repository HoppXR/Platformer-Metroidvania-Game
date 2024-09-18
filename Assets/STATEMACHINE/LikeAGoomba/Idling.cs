using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idling : StateMachineBehaviour
{
    private float timer;
    private Transform player;
    private float chaseRange = 8f;
    private float rotationSpeed = 60f; // Speed of rotation in degrees per second
    private float targetRotationY; // The target Y rotation
    private bool hasTurned = false; // To track if the turn has been completed

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Set a random angle for one-time rotation, either left (-) or right (+)
        float randomAngle = Random.Range(15f, 50f);
        // Randomly decide whether to turn left (negative) or right (positive)
        float direction = Random.value > 0.5f ? 1f : -1f;

        // Set the target rotation, either left or right
        targetRotationY = animator.transform.eulerAngles.y + (randomAngle * direction);

        hasTurned = false; // Reset the turning status when entering the state
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        // Smoothly rotate towards the target angle only if it hasn't turned yet
        if (!hasTurned)
        {
            // Get the current Y rotation
            float currentY = animator.transform.eulerAngles.y;

            // Smoothly rotate towards the target angle
            float newRotationY = Mathf.MoveTowardsAngle(currentY, targetRotationY, rotationSpeed * Time.deltaTime);
            animator.transform.rotation = Quaternion.Euler(0, newRotationY, 0);

            // Check if the rotation is close enough to the target
            if (Mathf.Abs(Mathf.DeltaAngle(currentY, targetRotationY)) < 0.1f)
            {
                hasTurned = true; // Mark that the enemy has turned once
            }
        }

        // After 5 seconds, switch back to patrolling
        if (timer > 5f)
        {
            animator.SetBool("isPatroling", true);
        }

        // If the player is within range, switch to chasing
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance < chaseRange)
        {
            animator.SetBool("isChasing", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
