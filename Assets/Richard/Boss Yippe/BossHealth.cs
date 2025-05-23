using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int health = 100;
    [SerializeField] private Material damageMaterial;
    public BossHealthUI bossHealthUI;
    private Material[] _originalMaterials;
    private Renderer[] _renderers;
    private BossStateManager bossStateManager;

    private void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _originalMaterials = new Material[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalMaterials[i] = _renderers[i].material;
        }
        
        bossStateManager = FindFirstObjectByType<BossStateManager>();
        bossHealthUI?.SetMaxHealth(health);
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
            bossHealthUI.SetHealth(health);
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
        health = Mathf.Min(health + amount, maxHealth);
        bossHealthUI.SetHealth(health);
    }

    private void Die()
    {
        bossStateManager.bossHealthBar.SetActive(false);
        GameManager.Instance.PlayerWin();
        Destroy(gameObject);
    }

    private IEnumerator EDamageFlicker()
    {
        foreach (Renderer r in _renderers)
        {
            r.material = damageMaterial;
        }
            
        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material = _originalMaterials[i];
        }
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
        HealBoss(10);
        bossStateManager.SetState(BossStateManager.BossState.Transition);
        bossStateManager.bossAI.SetState(BossAIManager.BossState.Idle);
    }
}