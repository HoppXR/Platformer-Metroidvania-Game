using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idling : StateMachineBehaviour
{
    private float timer;
    private Transform player;
    private float chaseRange = 15f;
    private float rotationSpeed = 60f;
    private float targetRotationY; 
    private bool hasTurned = false; 


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        float randomAngle = Random.Range(15f, 50f);
        float direction = Random.value > 0.5f ? 1f : -1f;


        targetRotationY = animator.transform.eulerAngles.y + (randomAngle * direction);

        hasTurned = false; 
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;


        if (!hasTurned)
        {

            float currentY = animator.transform.eulerAngles.y;


            float newRotationY = Mathf.MoveTowardsAngle(currentY, targetRotationY, rotationSpeed * Time.deltaTime);
            animator.transform.rotation = Quaternion.Euler(0, newRotationY, 0);


            if (Mathf.Abs(Mathf.DeltaAngle(currentY, targetRotationY)) < 0.1f)
            {
                hasTurned = true; 
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
