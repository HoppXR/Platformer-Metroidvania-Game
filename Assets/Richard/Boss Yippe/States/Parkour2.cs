using System.Collections;
using UnityEngine;

public class Parkour2 : BaseState
{
    private BossStateManager bossManager;

    public Parkour2(BossStateManager bossManager) : base(bossManager)
    {
        this.bossManager = bossManager;
    }

    public override void EnterState()
    {
        if (bossManager.parkour2Trigger != null)
        {
            bossManager.parkour2Trigger.SetActive(true);
        }
    }

    public override void StateUpdate() { }

    public override void ExitState()
    {
    }
}