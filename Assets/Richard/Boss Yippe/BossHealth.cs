using System.Collections;
using Managers;
using UnityEngine;

public class BossHealth : MonoBehaviour
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
        
        if (health <= 0) return;
        
        if (damage >= health)
        {
            health = 0;
            if (bossStateManager.currentState == BossStateManager.BossState.Fight2)
            {
                Die();
            }
        }
        else
        {
            health -= damage;
        }

        StartCoroutine(EDamageFlicker());

        if (bossStateManager == null) return;
        
        if (health <= 10 && bossStateManager.currentState == BossStateManager.BossState.Fight1)
        {
            StartCoroutine(WaitForAttackToEndThenTransition());
        }
        
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
        GameManager.Instance.PlayerWin();
        Destroy(gameObject);
    }

    private IEnumerator EDamageFlicker()
    {
        _renderer.material = damageMaterial;
        yield return new WaitForSeconds(0.25f);
        _renderer.material = _originalMaterial;
    }
    
    private IEnumerator WaitForAttackToEndThenTransition()
    {
        BossAIManager bossAI = GetComponent<BossAIManager>();

        if (bossAI != null)
        {
            while (bossAI.currentState == BossAIManager.BossState.Attack)
            {
                yield return null; 
            }
        }
        
        HealBoss(health);
        bossStateManager.SetState(BossStateManager.BossState.Transition);
        bossStateManager.bossAI.SetState(BossAIManager.BossState.Idle);
    }
}