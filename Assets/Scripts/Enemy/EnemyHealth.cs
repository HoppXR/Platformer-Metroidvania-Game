using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private int health;
        [SerializeField] private Material damageMaterial;
        private Material[] _originalMaterials;
        private Renderer[] _renderers;
    
        private void Start()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _originalMaterials = new Material[_renderers.Length];

            for (int i = 0; i < _renderers.Length; i++)
            {
                _originalMaterials[i] = _renderers[i].material;
            }
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
    }
}
