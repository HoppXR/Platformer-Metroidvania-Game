using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public event Action OnDeath;
    
    [SerializeField] private float playerLives;
    private float _lerpSpeed;
    private float _maxPlayerLives;

    private void Start()
    {
        _maxPlayerLives = playerLives;
    }

    public void TakeDamage()
    {
        if (playerLives <= 0)
        {
            playerLives = 0;
            OnDeath?.Invoke();
        }
        else
            playerLives--;
    }

    public void Heal()
    {
        if (playerLives++ >= _maxPlayerLives)
        {
            playerLives = _maxPlayerLives;
        }

        playerLives++;
    }
}
