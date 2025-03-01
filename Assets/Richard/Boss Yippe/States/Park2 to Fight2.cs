using UnityEngine;

public class Park2toFight2 : MonoBehaviour
{
    private BossStateManager bossStateManager;

    private void Start()
    {
        bossStateManager = FindObjectOfType<BossStateManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && bossStateManager != null)
        {
            bossStateManager.SetState(BossStateManager.BossState.Fight2);
            gameObject.SetActive(false);
        }
    }
}