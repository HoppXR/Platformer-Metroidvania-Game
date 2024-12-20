using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private Image healthBarColor;
    private float _lerpSpeed;
    private float _maxHealth;

    private void Start()
    {
        _maxHealth = health;
        healthBarSlider.maxValue = _maxHealth;
        healthBarSlider.value = health;
    }

    private void Update()
    {
        _lerpSpeed = 3f * Time.fixedDeltaTime;
        HealthBar();
        ColorChanger();
    }

    public void TakeDamage(float amount)
    {
        if (amount >= health)
        {
            health = 0;
        }
        else
            health -= amount;
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

    private void HealthBar()
    {
        healthBarSlider.value = Mathf.Lerp(healthBarSlider.value, health, _lerpSpeed);
    }

    private void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, healthBarSlider.value / _maxHealth);

        healthBarColor.color = healthColor;
    }
}
