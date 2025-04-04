using System;
using General;
using Player.Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        #region Variables
        [Header("References")]
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
    
        [Header("HighScore Board")]
        [SerializeField] private TMP_InputField playerNameInput;
        [SerializeField] private Button saveScoreButton;
        private bool _hasSaved = false;
    
        [Header("Timer")]
        [SerializeField] private TMP_Text timerText;
        static float _timer = 0f;
        private bool _isRunning = true;
    
        [Header("Abilities")]
        [SerializeField] private GameObject dashAbility;
        [SerializeField] private GameObject swingAbility;
        [SerializeField] private GameObject doubleJumpAbility;
        [SerializeField] private GameObject slideAbility;

        [Header ("Tutorial")]
        [SerializeField] private GameObject TextWindow;
        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private int tutorialActiveTime;
        #endregion

        #region Unity Built-in Methods
        private void Start()
        {
            saveScoreButton.onClick.AddListener(OnSaveClicked);
            saveScoreButton.interactable = false;

            playerNameInput.onValueChanged.AddListener(OnNameChanged);
        
            GameManager.Instance.FindPlayerHealth();
            
            DisableUI();
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
        #endregion
    
        #region Public methods
        public void EnableAbilityUI(AbilityManager.Abilities ability)
        {
            switch (ability)
            {
                case AbilityManager.Abilities.Dash:
                    dashAbility.SetActive(true);
                    tutorialText.text = "Press SHIFT or ___ to dash forward with a burst of speed!";
                    TextWindow.SetActive(true);
                    //await Task.Delay(tutorialActiveTime);
                    //TextWindow.SetActive(false);
                    break;
                case AbilityManager.Abilities.Swing:
                    swingAbility.SetActive(true);
                    tutorialText.text = "Press and hold RIGHT CLICK or RIGHT TRIGGER to grapple! Pay attention to overhangs and YELLOW objects for the red indicator to see if a surface is grappleable!";
                    TextWindow.SetActive(true);
                    //await Task.Delay(tutorialActiveTime);
                    //TextWindow.SetActive(false);
                    break;
                case AbilityManager.Abilities.DoubleJump:
                    doubleJumpAbility.SetActive(true);
                    tutorialText.text = "Press SPACE or ___ in the air to preform a double jump!";
                    TextWindow.SetActive(true);
                    //await Task.Delay(tutorialActiveTime);
                    //TextWindow.SetActive(false);
                    break;
                case AbilityManager.Abilities.Slide:
                    slideAbility.SetActive(true);
                    tutorialText.text = "Press and hold CTRL or ____ to roll, allowing you to fit in tight spaces and move faster!";
                    TextWindow.SetActive(true);
                    //await Task.Delay(tutorialActiveTime);
                    //TextWindow.SetActive(false);
                    break;
                case AbilityManager.Abilities.Null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ability), ability, null);
            }
        }
        public void PauseGame()
        {
            pauseMenu?.SetActive(true);
        }

        public void ResumeGame()
        {
            pauseMenu?.SetActive(false);
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

        public void ResetTimer()
        {
            _timer = 0f;
        }
        #endregion
    
        #region Private methods
        private void DisableUI()
        {
            gameUI.SetActive(true);
            pauseMenu.SetActive(false);
            gameLoseUI.SetActive(false);
            gameWinUI.SetActive(false);
            interactionUI.SetActive(false);
        
            dashAbility.SetActive(false);
            swingAbility.SetActive(false);
            doubleJumpAbility.SetActive(false);
            slideAbility.SetActive(false);
        }
    
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
}
