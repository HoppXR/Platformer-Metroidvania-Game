using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fight1 : BaseState
{
    public Fight1(BossStateManager bossManager) : base(bossManager) { }

    public override void EnterState()
    {
        Debug.Log("Entered Fight 1");
        // Enable boss UI
        // Start boss attacks
    }

    public override void StateUpdate()
    {
        // Placeholder: Switch state when boss health is depleted
        if (Input.GetMouseButtonDown(0))
        {
            bossManager.SetState(BossStateManager.BossState.Transition);
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Fight 1");
    }
}
