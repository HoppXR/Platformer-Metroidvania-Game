using Platformer;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Collectables
    private int _count;
    private int _maxCount;

    // Timer
    private Timer _timer;
    private StopwatchTimer _timeSinceStart;
    private float _endTime;
    
    // Player Health
    [SerializeField] private PlayerHealth playerHealth;
    
    void Start()
    {
        _timeSinceStart.Start();

        _count = 0;
        _maxCount = GameObject.FindGameObjectsWithTag("Collectable").Length;
    }

    void Update()
    {
        GameOver();
    }

    private void GameOver()
    {
        if (HasPlayerWon())
        {
            _timeSinceStart.Stop();
            _endTime = _timeSinceStart.GetTime();
            
            // add stats to leaderboard
        }
        else if (HasPlayerLost())
        {
            // game over UI & restart or quit button
        }
    }

    private bool HasPlayerWon()
    {
        return _count >= _maxCount;
    }

    private bool HasPlayerLost()
    {
        return playerHealth.GetPlayerHealth() <= 0;
    }
}
