using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parkour2 : BaseState
{
    public Parkour2(BossStateManager bossManager) : base(bossManager) { }

    public override void EnterState()
    {
        Debug.Log("Entered Parkour 2");
        // Replace old parkour with new
    }

    public override void StateUpdate()
    {
        if (Input.GetMouseButtonDown(0)) // Placeholder for reaching the top
        {
            bossManager.SetState(BossStateManager.BossState.Fight2);
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Parkour 2");
    }
}
