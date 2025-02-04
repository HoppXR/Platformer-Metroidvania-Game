using UnityEngine;

public class Parkour1 : BaseState
{
    public Parkour1(BossStateManager bossManager) : base(bossManager) { }

    public override void EnterState()
    {
        Debug.Log("Entered Parkour 1");
    }

    public override void StateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bossManager.SetState(BossStateManager.BossState.Fight1);
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Parkour 1");
    }
}