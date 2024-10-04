using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeController : MonoBehaviour
{
    private Animator animator;

    public float rangeToStopChasing = 15f;
    public float rangeToAttack = 3.5f;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("RangeToStopChasing", rangeToStopChasing);
        animator.SetFloat("RangeToAttack", rangeToAttack);
    }
}
