using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [SerializeField] private InputReader input;
    [SerializeField] private HighScoreBoard highScoreBoard;
    [SerializeField] private PlayerInteract playerInteract;
    
    [Header("UI")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameLoseUI;
    [SerializeField] private GameObject gameWinUI;
    [SerializeField] private GameObject gameUI;
    
    [Header("Interaction")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private TMP_Text interactionText;
    
    [Header("Collectables")]
    [SerializeField] private TMP_Text countText;
    
    [Header("HighScore Board")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button saveScoreButton;
    private bool _hasSaved = false;
    
    [Header("Timer")]
    [SerializeField] private TMP_Text timerText;
    private float _timer = 0f;
    private bool _isRunning = true;
    #endregion
    
    private void Start()
    {
        Time.timeScale = 1;
        
        gameUI.SetActive(true);
        pauseMenu.SetActive(false);
        gameLoseUI.SetActive(false);
        gameWinUI.SetActive(false);
        interactionUI.SetActive(false);
        
        input.SetGameplay();
        
        saveScoreButton.onClick.AddListener(OnSaveClicked);
        saveScoreButton.interactable = false;

        playerNameInput.onValueChanged.AddListener(OnNameChanged);
        
        GameManager.MaxCount = GameObject.FindGameObjectsWithTag("Collectable").Length;
        SetCountText();
        
        GameManager.Instance.FindPlayerHealth();
    }
    
    private void Update()
    {
        HandleTimer();

        if (playerInteract.GetInteractable() != null)
        {
            ShowInteractUI(playerInteract.GetInteractable());
        }
        else
        {
            HideInteractUI();
        }
    }
    
    #region Public methods
    public void PauseGame()
    {
        input?.SetUI();

        pauseMenu?.SetActive(true);
    }

    public void ResumeGame()
    {
        pauseMenu?.SetActive(false);
        
        input?.SetGameplay();
    }

    public void GameLose()
    {
        _isRunning = false;
        
        gameUI?.SetActive(false);
        gameLoseUI?.SetActive(true);
    }

    public void GameWin()
    {
        _isRunning = false;
        
        gameUI?.SetActive(false);
        gameWinUI?.SetActive(true);
    }
    
    public void SetCountText()
    {
        countText.text = GameManager.Count + "/" + GameManager.MaxCount;
    }
    #endregion
    
    #region Private methods
    private void ShowInteractUI(IInteractable interactable)
    {
        interactionUI?.SetActive(true);
        interactionText.text = interactable.GetInteractText();
    }

    private void HideInteractUI()
    {
        interactionUI?.SetActive(false);
    }
    
    private void HandleTimer()
    {
        if (!_isRunning) return;
        _timer += Time.deltaTime;
        timerText.text = _timer.ToString("F2");
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
    #endregion
}
