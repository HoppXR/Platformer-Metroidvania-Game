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
    
    // UI
    [SerializeField] private GameObject gameOverUI;
    
    void Start()
    {
        Time.timeScale = 1;
        
        _count = 0;
        _maxCount = GameObject.FindGameObjectsWithTag("Collectable").Length;

        _timeSinceStart = new StopwatchTimer();
        _timeSinceStart.Start();
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
            Time.timeScale = 0;
            gameOverUI.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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

    public void IncreaseCount()
    {
        _count++;
    }
}
