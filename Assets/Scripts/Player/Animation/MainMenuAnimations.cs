using Managers;
using UnityEngine;

public class MainMenuAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void BlastOff()
    {
        animator.SetTrigger("BlastOff");
        
        GameManager.Instance.LoadLevelTimer(1, 2f);
    }
}
