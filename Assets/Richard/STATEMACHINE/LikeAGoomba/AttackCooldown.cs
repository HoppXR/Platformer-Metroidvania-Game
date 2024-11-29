using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AttackCooldown : StateMachineBehaviour
{
    private float cooldownTimer;
    public float cooldownDuration = 2f; // Example cooldown duration

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cooldownTimer = 0f;
        animator.SetBool("hasAttacked", true);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cooldownTimer += Time.deltaTime;

        if (cooldownTimer >= cooldownDuration)
        {
            animator.SetBool("hasAttacked", false);
            animator.SetBool("canAttack", true); // Enable attack again
            animator.SetBool("isPatroling", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Optionally handle state exit logic
    }
}