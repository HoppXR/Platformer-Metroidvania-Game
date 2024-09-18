using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health;
    private float _maxHealth;
    
    public void TakeDamage(float amount)
    {
        if (amount >= health)
        {
            health = 0;
        }
        else
            health -= amount;
        
        Debug.Log(health);
    }

    public void Heal(float amount)
    {
        if (amount + health >= _maxHealth)
        {
            health = _maxHealth;
        }

        health += amount;
    }

    public float GetPlayerHealth()
    {
        return health;
    }
}
