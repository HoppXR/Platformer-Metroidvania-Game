using Player;
using UnityEngine;

namespace Enemy
{
    public class EnemyCollisionDamage : MonoBehaviour
    {
        private PlayerHealth _pHealth;
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                _pHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();
                InvokeRepeating(nameof(TickDamage), 0, 1);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                CancelInvoke(nameof(TickDamage));
            }
        }

        private void TickDamage()
        {
            if (_pHealth != null)
                _pHealth.TakeDamage();
        }
    }
}