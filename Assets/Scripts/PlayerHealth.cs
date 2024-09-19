using System;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private TMP_Text healthText;
    private float _maxHealth;

    private void Start()
    {
        _maxHealth = health;
        UpdateHealthText();
    }

    public void TakeDamage(float amount)
    {
        if (amount >= health)
        {
            health = 0;
        }
        else
            health -= amount;
        
        UpdateHealthText();
    }

    public void Heal(float amount)
    {
        if (amount + health >= _maxHealth)
        {
            health = _maxHealth;
        }

        health += amount;
        
        UpdateHealthText();
    }

    public float GetPlayerHealth()
    {
        return health;
    }

    private void UpdateHealthText()
    {
        healthText.text = health + "/" + _maxHealth;
    }
}
