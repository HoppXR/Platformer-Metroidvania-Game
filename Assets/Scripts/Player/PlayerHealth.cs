using System;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public event Action OnDeath;
    
        [SerializeField] private int playerLives;
        [SerializeField] private Sprite[] playerLiveSprites;
        [SerializeField] private Image playerLiveDisplay;
        private int _maxPlayerLives;

        private void Start()
        {
            _maxPlayerLives = playerLives;
        
            UpdateSprite();
        }

        public void TakeDamage()
        {
            playerLives--;
        
            if (playerLives <= 0)
            {
                playerLives = 0;
                OnDeath?.Invoke();
            }
        
            UpdateSprite();
        }

        public void Heal()
        {
            playerLives++;
        
            if (playerLives >= _maxPlayerLives)
            {
                playerLives = _maxPlayerLives;
            }
        
            UpdateSprite();
        }

        private void UpdateSprite()
        {
            playerLiveDisplay.sprite = playerLiveSprites[playerLives];
        }
    }
}
