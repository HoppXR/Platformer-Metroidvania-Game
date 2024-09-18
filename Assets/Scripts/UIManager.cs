using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void Retry()
    {
        SceneManager.LoadScene(0);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
