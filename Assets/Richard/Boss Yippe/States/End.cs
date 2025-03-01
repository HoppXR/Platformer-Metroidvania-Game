using System.Collections;
using UnityEngine;

public class End : BaseState
{
    private BossStateManager bossManager;

    public End(BossStateManager bossManager) : base(bossManager)
    {
        this.bossManager = bossManager;
    }

    public override void EnterState()
    {
        bossManager.StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        yield return new WaitForSeconds(2f);
        if (bossManager.bossAI != null)
        {
            Object.Destroy(bossManager.bossAI.gameObject);
        }

        // Placeholder
    }

    public override void StateUpdate() { }

    public override void ExitState() { }
}