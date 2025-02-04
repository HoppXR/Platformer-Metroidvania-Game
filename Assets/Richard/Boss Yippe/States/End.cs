using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : BaseState
{
    public End(BossStateManager bossManager) : base(bossManager) { }

    public override void EnterState()
    {
        Debug.Log("Game end or something");
    }

    public override void StateUpdate() { }

    public override void ExitState() { }
}
