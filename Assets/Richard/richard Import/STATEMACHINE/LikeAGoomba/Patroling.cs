using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patroling : StateMachineBehaviour
{
    private NavMeshAgent AI;
    private Transform player;

    private float chaseRange = 15f;
    private float patrolRadius = 5f; 

 
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        AI = animator.GetComponent<NavMeshAgent>();
        AI.speed = 5f;


        SetRandomDestination(animator);
        animator.SetBool("isChasing", false);
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (AI.remainingDistance <= AI.stoppingDistance)
        {

            animator.SetBool("isPatroling", false);
        }

        // Check if the player is within chase range
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance < chaseRange)
        {
            animator.SetBool("isChasing", true);
        }
    }
    
    private void SetRandomDestination(Animator animator)
    {

        Vector2 randomPoint = Random.insideUnitCircle * patrolRadius;


        Vector3 randomDestination = new Vector3(randomPoint.x, 0, randomPoint.y) + animator.transform.position;


        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDestination, out hit, patrolRadius, NavMesh.AllAreas))
        {
            AI.SetDestination(hit.position);
        }
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        AI.SetDestination(AI.transform.position);
    }
}
