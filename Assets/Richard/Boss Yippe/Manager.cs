using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    protected BossStateManager bossManager;

    public BaseState(BossStateManager bossManager)
    {
        this.bossManager = bossManager;
    }

    public abstract void EnterState();
    public abstract void StateUpdate();
    public abstract void ExitState();
}
