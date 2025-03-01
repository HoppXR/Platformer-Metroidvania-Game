using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private Material damageMaterial;
    private Material _originalMaterial;
    private Renderer _renderer;
    private BossStateManager bossStateManager;

    private void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _originalMaterial = _renderer.material;
        
        bossStateManager = FindObjectOfType<BossStateManager>();
    }

    public void TakeDamage(int damage)
    {
        if (damage >= health)
        {
            health = 0;
        }
        else
        {
            health -= damage;
        }

        StartCoroutine(EDamageFlicker());

        if (bossStateManager == null) return;

        // If health is 10 or less, switch to Phase 2
        if (health <= 10 && bossStateManager.currentState == BossStateManager.BossState.Fight1)
        {
            bossStateManager.SetState(BossStateManager.BossState.Transition);
        }

        // If health reaches 0 in Fight2, switch to End
        if (health <= 0 && bossStateManager.currentState == BossStateManager.BossState.Fight2)
        {
            bossStateManager.SetState(BossStateManager.BossState.End);
        }
    }

    public void HealBoss(int amount)
    {
        health += amount;
    }

    private void Die()
    {
        // Play death animation (if needed)
        Destroy(gameObject);
    }

    private IEnumerator EDamageFlicker()
    {
        _renderer.material = damageMaterial;
        yield return new WaitForSeconds(0.25f);
        _renderer.material = _originalMaterial;
    }
}