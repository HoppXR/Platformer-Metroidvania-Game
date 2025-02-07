using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : BossAIState
{
    public Idle(BossAIManager boss) : base(boss) { }

    public override void EnterState()
    {
        Debug.Log("Idle");
    }

    public override void StateUpdate()
    {
        
    }

    public override void ExitState() { }
}
