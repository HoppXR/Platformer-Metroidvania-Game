using System.Collections;
using UnityEngine;

public class AttackCoolDowns : MonoBehaviour
{
    public bool CanAttack { get; private set; } = true;

    public void StartCooldown()
    {
        if (CanAttack)
        {
            CanAttack = false;
            StartCoroutine(CooldownRoutine());
        }
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(2f); // Cooldown time
        CanAttack = true;
    }
}