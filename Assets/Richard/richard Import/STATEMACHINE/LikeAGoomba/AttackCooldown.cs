using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AttackCooldown : StateMachineBehaviour
{
    private float cooldownTimer;
    public float cooldownDuration = 0f;  // Duration of the cooldown in seconds
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cooldownTimer = 0f;  // Reset the cooldown timer
        animator.SetBool("hasAttacked", true); // Ensure enemy is in idle state
        animator.SetBool("isPatroling", false); // Ensure enemy is not patrolling or moving
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cooldownTimer += Time.deltaTime;  // Increment the cooldown timer

        if (cooldownTimer >= cooldownDuration)
        {
            // After the cooldown duration has passed, transition to the appropriate state
            animator.SetBool("hasAttacked", false); // Ensure the enemy is not in idle state anymore
            animator.SetBool("isPatroling", true); // Transition back to patrolling or moving
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Optionally reset parameters or handle state exit logic here
    }
}