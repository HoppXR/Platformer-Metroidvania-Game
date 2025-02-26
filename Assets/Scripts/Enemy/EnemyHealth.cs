using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health;

    public void TakeDamage(int damage)
    {
        if (damage >= health)
        {
            health = 0;
            Die();
        }
        else
        {
            health -= damage;
        }
    }

    public void Squish()
    {
        StartCoroutine(ESquish());
    }

    private IEnumerator ESquish()
    {
        // play squish animation
        
        yield return null; // wait for animation time
        
        // death logic
    }

    private void Die()
    {
        
    }
}
