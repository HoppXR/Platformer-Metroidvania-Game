using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("References")] 
    [SerializeField] private InputReader input;
    private PlayerHealth _playerHealth;

    public static int Count = 0;
    public static int MaxCount;
    
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

    private void Update()
    {
        //PlayerWin();
    }
    #endregion

    public void FindPlayerHealth()
    {
        _playerHealth = FindFirstObjectByType<PlayerHealth>();
        _playerHealth.OnDeath += PlayerLose;
    }
    
    #region UI Stuff
    private void PauseGame()
    {
        // play ui sound
        
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
    }

    public void RestartGame()
    {
        // play ui sound
        
        // locks and hides the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Time.timeScale = 1;
        
        // Reset Collectable Score
        Count = 0;
        
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
        
        SceneManager.LoadScene(index);
    }

    // for instant level loads
    public void LoadLevel(int index)
    {
        SceneManager.LoadScene(index);
    }
    #endregion

    #region Game State
    private void PlayerWin()
    {
        if (!HasPlayerWon()) return;
        
        FindFirstObjectByType<UIManager>()?.GameWin();

        Time.timeScale = 0;
            
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void PlayerLose()
    {
        Time.timeScale = 0;
        
        FindFirstObjectByType<UIManager>()?.GameLose();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private bool HasPlayerWon()
    {
        return Count >= MaxCount;
    }
    
    public void IncreaseCount()
    {
        Count++;
        FindFirstObjectByType<UIManager>().SetCountText();
    }
    #endregion
}
