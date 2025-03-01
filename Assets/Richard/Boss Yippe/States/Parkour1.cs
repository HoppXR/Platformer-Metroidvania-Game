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

    }

    public override void ExitState()
    {
        Debug.Log("Exiting Parkour 1");
    }
}//this should be the starting phase where the boss is dormant meaning all script on boss is off, there will be a collider that will trigger part 2 which means switching phase to fight 1