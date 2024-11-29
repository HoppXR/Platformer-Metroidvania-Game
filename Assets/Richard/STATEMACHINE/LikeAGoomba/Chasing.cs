using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chasing : StateMachineBehaviour
{
    NavMeshAgent AI;
    private Transform player;
    [SerializeField] private float rangeToStopChasingPlayer = 15f;
    [SerializeField] private float rangeToAttackPlayer = 3.5f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI = animator.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        AI.speed = 7f;
        rangeToStopChasingPlayer = animator.GetFloat("RangeToStopChasing");
        rangeToAttackPlayer = animator.GetFloat("RangeToAttack");
        animator.SetBool("isPatroling", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI.SetDestination(player.position);
        float distance = Vector3.Distance(player.position, animator.transform.position);

        if (distance > rangeToStopChasingPlayer)
        {
            animator.SetBool("isChasing", false);
        }

        if (distance < rangeToAttackPlayer && animator.GetBool("canAttack")) // Check if can attack
        {
            animator.SetBool("isAttacking", true);
            animator.SetBool("canAttack", false); // Disable attack until cooldown resets it
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI.ResetPath(); // Stop moving when exiting this state
    }
}