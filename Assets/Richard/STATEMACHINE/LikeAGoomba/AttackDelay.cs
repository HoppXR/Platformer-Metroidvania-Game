using System;
using System.Collections;
using UnityEngine;

public class AttackDelay : MonoBehaviour
{
    public GameObject squareIndicator;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void DoAttack(Action onAttackReady)
    {
        StartCoroutine(PrepareAttack(onAttackReady));
    }

    private IEnumerator PrepareAttack(Action onAttackReady)
    {
        if (squareIndicator != null)
        {
            squareIndicator.SetActive(true);
        }
        
        yield return new WaitForSeconds(2f);
        
        if (squareIndicator != null)
        {
            squareIndicator.SetActive(false);
        }

        onAttackReady?.Invoke();
    }
}