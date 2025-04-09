using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public event Action OnDeath;
        
        [SerializeField] private Sprite[] playerLiveSprites;
        [SerializeField] private Image playerLiveDisplay;

        private void Start()
        {
            UpdateSprite();
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
        }

        public void MaxHeal()
        {
            GameManager.CurrentPlayerHealth = GameManager.MaxPlayerHealth;
            UpdateSprite();
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
    }
}
