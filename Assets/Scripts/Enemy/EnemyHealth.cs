using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private Material damageMaterial;
    private Material _originalMaterial;
    private Renderer _renderer;
    
    private void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _originalMaterial = _renderer.material;
    }

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

        StartCoroutine(EDamageFlicker());
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
        // play death animation
        
        // TEMP logic
        Destroy(gameObject);
    }

    private IEnumerator EDamageFlicker()
    {
        _renderer.material = damageMaterial;
        
        yield return new WaitForSeconds(0.25f);
        
        _renderer.material = _originalMaterial;
    }
}
