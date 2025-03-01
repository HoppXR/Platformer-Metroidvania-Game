using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Fight2 : BaseState
{
    public Fight2(BossStateManager bossManager) : base(bossManager) { }// when the boss reaches 0 health or less it will switch to the lkast phase which is exit

    public override void EnterState()
    {
        Debug.Log("Entered Fight 2");
        // Enable boss final phase
    }

    public override void StateUpdate()
    {

    }

    public override void ExitState()
    {
        Debug.Log("Exiting Fight 2");
    }
}
