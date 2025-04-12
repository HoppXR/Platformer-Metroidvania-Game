using System.Collections;
using Player;
using Player.Input;
using Player.Movement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
    
        [Header("References")]
        [SerializeField] private InputReader input;
        private PlayerHealth _playerHealth;
        
        [Header("Player Health")]
        public static int CurrentPlayerHealth = 5;
        public static int MaxPlayerHealth = 5;
        [SerializeField] private int coinsToIncrease;
        public static int TotalCoinCount;
        private static int _coinCount;

        [Header("Player Combat")] 
        public static int PlayerDamage = 5;
        
        #region Unity Methods
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    
        private void Start()
        {
            input.PauseEvent += PauseGame;
            input.ResumeEvent += ResumeGame;
        }

        private void OnDestroy()
        {
            input.PauseEvent -= PauseGame;
            input.ResumeEvent -= ResumeGame;
        }
        #endregion

        public void FindPlayerHealth()
        {
            _playerHealth = FindFirstObjectByType<PlayerHealth>();
            _playerHealth.OnDeath += PlayerLose;
        }

        public void CoinCollected()
        {
            _coinCount++;
            TotalCoinCount++;

            if (_coinCount >= coinsToIncrease)
            {
                _playerHealth.IncreaseMaxHealth();
                _coinCount = 0;
            }
            
            FindFirstObjectByType<UIManager>().UpdateCountText();
        }
    
        #region UI Stuff
        public void ResetGame()
        {
            // reset timer
            FindFirstObjectByType<UIManager>().ResetTimer();
            
            // reset the variables
            TotalCoinCount = 0;
            _coinCount = 0;
            MaxPlayerHealth = 5;
            CurrentPlayerHealth = 5;
            PlayerDamage = 5;
            
            // disable all abilities
            AbilityManager.Instance.DisableAllAbilities();
        }
        
        private void PauseGame()
        {
            // play ui sound
        
            input.SetUI();
            
            FindFirstObjectByType<UIManager>()?.PauseGame();
        
            Time.timeScale = 0;
        
            // makes cursor visible and moveable
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void ResumeGame()
        {
            // play ui sound
        
            // locks and hides the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        
            Time.timeScale = 1;
        
            FindFirstObjectByType<UIManager>()?.ResumeGame();
            
            input.SetGameplay();
        }

        public void RestartGame()
        {
            // play ui sound
        
            // locks and hides the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            // resets player animation to prevent bugs
            FindFirstObjectByType<PlayerMovement>()?.ResetAnimation();
        
            // sets back to game controls
            input.SetGameplay();
        
            Time.timeScale = 1;
        
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
            // play ui sound
        
            Application.Quit();
        }

        // for level loads that have animations or need a delay
        public void LoadLevelTimer(int index, float time)
        {
            StartCoroutine(ELoadLevelTimer(index, time));
        }
        private IEnumerator ELoadLevelTimer(int index, float time)
        {
            // play ui sound or animation before scene change
        
            yield return new WaitForSeconds(time);
        
            input.SetGameplay();
            
            SceneManager.LoadScene(index);
            
            Time.timeScale = 1;
        }

        // for instant level loads
        public void LoadLevel(int index)
        {
            input.SetGameplay();
        
            // resets player animation to prevent bugs
            FindFirstObjectByType<PlayerMovement>()?.ResetAnimation();
            
            SceneManager.LoadScene(index);

            Time.timeScale = 1;
        }
        #endregion

        #region Game State
        public void PlayerWin()
        {
            input.SetUI();
            
            FindFirstObjectByType<UIManager>()?.GameWin();

            Time.timeScale = 0;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void PlayerLose()
        {
            input.SetUI();
            
            Time.timeScale = 0;
        
            FindFirstObjectByType<UIManager>()?.GameLose();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        #endregion
    }
}
