using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader input;
    
    [Header("UI")]
    [SerializeField] private GameObject pauseMenu;
    
    private void Start()
    {
        input.PauseEvent += PauseGame;
        input.ResumeEvent += ResumeGame;
    }

    private void PauseGame()
    {
        input.SetUI();
        
        pauseMenu.SetActive(true);
        
        Time.timeScale = 0;
        
        // makes cursor visible and moveable
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        // locks and hides the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Time.timeScale = 1;
        
        pauseMenu.SetActive(false);
        
        input.SetGameplay();
    }
    
    public void Retry()
    {
        ResumeGame();
        
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
