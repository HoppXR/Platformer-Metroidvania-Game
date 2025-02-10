using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeAttackCD : StateMachineBehaviour
{
    private float cooldownTimer;
    public float cooldownDuration = 2f;
    private float StopChasing = 8f;
    private NavMeshAgent AI;
    public Transform player;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI = animator.GetComponentInParent<NavMeshAgent>();
        cooldownTimer = 0f;
        StopChasing = animator.GetFloat("RangeToStopChasing");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator.SetBool("hasAttacked", true);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cooldownTimer += Time.deltaTime;
        float distance = Vector3.Distance(player.position, animator.transform.position);
        FacePlayer(animator);
        if (distance >= StopChasing)
        {
            animator.SetBool("hasAttacked", false);
            animator.SetBool("canAttack", true); // Enable attack again
            animator.SetBool("isPatroling", true);
        }
        else 
        {
            animator.SetBool("canAttack", true);
            animator.SetBool("hasAttacked", false);
        }
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
}
