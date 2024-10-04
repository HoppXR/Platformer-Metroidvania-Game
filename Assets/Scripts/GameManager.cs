using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Highscore
    [SerializeField] private HighScoreBoard highScoreBoard;
    [SerializeField] private TMP_Text playerTimeText;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button saveScoreButton;
    private bool _hasSaved = false;
    
    // Collectables
    [SerializeField] private TMP_Text countText;
    private int _count = 0;
    private int _maxCount;

    // Timer
    [SerializeField] private TMP_Text timerText;
    private float _timer = 0f;
    private bool _isRunning = true;
    
    // Player Health
    [SerializeField] private PlayerHealth playerHealth;
    
    // UI
    [SerializeField] private GameObject gameLoseUI;
    [SerializeField] private GameObject gameWinUI;
    [SerializeField] private GameObject gameUI;
    
    private void Start()
    {
        Time.timeScale = 1;

        _maxCount = GameObject.FindGameObjectsWithTag("Collectable").Length;
        SetCountText();
        
        saveScoreButton.onClick.AddListener(OnSaveClicked);
        saveScoreButton.interactable = false;

        playerNameInput.onValueChanged.AddListener(OnNameChanged);
    }

    private void Update()
    {
        HandleTimer();
        GameOver();
    }

    private void HandleTimer()
    {
        if (!_isRunning) return;
        _timer += Time.deltaTime;
        timerText.text = _timer.ToString("F3");
    }

    private void GameOver()
    {
        if (HasPlayerWon())
        {
            _isRunning = false;
            Time.timeScale = 0;
            gameWinUI.SetActive(true);
            gameUI.SetActive(false);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (HasPlayerLost())
        {
            Time.timeScale = 0;
            gameLoseUI.SetActive(true);
            gameUI.SetActive(false);

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

    private void SetCountText()
    {
        countText.text = _count + "/" + _maxCount;
    }
    
    public void IncreaseCount()
    {
        _count++;
        SetCountText();
    }

    private void OnSaveClicked()
    {
        _hasSaved = true;
        saveScoreButton.interactable = false;
        highScoreBoard.AddHighScore(playerNameInput.text, _timer);
    }

    private void OnNameChanged(string playerName)
    {
        saveScoreButton.interactable = !_hasSaved && playerName.Length > 0;
    }
}
