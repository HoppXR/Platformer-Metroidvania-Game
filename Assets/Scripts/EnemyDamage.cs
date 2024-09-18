using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private float damage;
    private PlayerHealth _pHealth;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _pHealth = FindFirstObjectByType<PlayerHealth>();
            InvokeRepeating(nameof(TickDamage), 0,1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CancelInvoke(nameof(TickDamage));
        }
    }

    private void TickDamage()
    {
        _pHealth.TakeDamage(damage);
    }
}
