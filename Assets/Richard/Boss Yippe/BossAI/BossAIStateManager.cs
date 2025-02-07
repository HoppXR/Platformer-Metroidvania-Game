using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossAIState
{
    protected BossAIManager boss;

    public BossAIState(BossAIManager boss)
    {
        this.boss = boss;
    }

    public abstract void EnterState();
    public abstract void StateUpdate();
    public abstract void ExitState();
}