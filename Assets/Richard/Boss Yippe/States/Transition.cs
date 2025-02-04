using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Transition : BaseState
{
    private float timer = 0f;

    public Transition(BossStateManager bossManager) : base(bossManager) { }

    public override void EnterState()
    {
        Debug.Log("Entered Transition");
        // Move camera to boss
        // Refill boss health
    }

    public override void StateUpdate()
    {
        timer += Time.deltaTime;
        if (timer > 3f) // Wait 3 seconds
        {
            bossManager.SetState(BossStateManager.BossState.Parkour2);
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Transition");
    }
}
