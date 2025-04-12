using System;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public event Action OnDeath;
        
        [Header("UI")]
        [SerializeField] private Sprite[] playerLiveSprites;
        [SerializeField] private Image playerLiveDisplay;

        [Header("Feedback")]
        [SerializeField] private Material damageMaterial;
        [SerializeField] private Material healMaterial;
        private Material[] _originalMaterials;
        private Renderer[] _renderers;
        
        private void Start()
        {
            UpdateSprite();
            
            _renderers = GetComponentsInChildren<Renderer>();
            _originalMaterials = new Material[_renderers.Length];

            for (int i = 0; i < _renderers.Length; i++)
            {
                _originalMaterials[i] = _renderers[i].material;
            }
        }

        public void TakeDamage()
        {
            GameManager.CurrentPlayerHealth--;
        
            if (GameManager.CurrentPlayerHealth <= 0)
            {
                GameManager.CurrentPlayerHealth = 0;
                OnDeath?.Invoke();
            }
        
            UpdateSprite();

            StartCoroutine(EMaterialFlicker(damageMaterial));
        }

        public void MaxHeal()
        {
            GameManager.CurrentPlayerHealth = GameManager.MaxPlayerHealth;
            UpdateSprite();

            StartCoroutine(EMaterialFlicker(healMaterial));
        }

        public void IncreaseMaxHealth()
        {
            if (GameManager.MaxPlayerHealth < 10)
                GameManager.MaxPlayerHealth++;
            
            MaxHeal();
        }

        private void UpdateSprite()
        {
            playerLiveDisplay.sprite = playerLiveSprites[GameManager.CurrentPlayerHealth];
        }

        private IEnumerator EMaterialFlicker(Material material)
        {
            foreach (Renderer r in _renderers)
            {
                r.material = material;
            }
            
            yield return new WaitForSeconds(0.25f);

            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].material = _originalMaterials[i];
            }
        }
    }
}
