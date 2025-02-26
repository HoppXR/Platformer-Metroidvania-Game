using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private PlayerHealth _pHealth;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _pHealth = other.GetComponentInParent<PlayerHealth>();
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
        _pHealth.TakeDamage();
    }
}
