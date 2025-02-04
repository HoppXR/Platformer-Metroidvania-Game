using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Fight2 : BaseState
{
    public Fight2(BossStateManager bossManager) : base(bossManager) { }

    public override void EnterState()
    {
        Debug.Log("Entered Fight 2");
        // Enable boss final phase
    }

    public override void StateUpdate()
    {
        if (Input.GetMouseButtonDown(0)) // Placeholder for boss death
        {
            bossManager.SetState(BossStateManager.BossState.End);
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Fight 2");
    }
}
