using System.Collections;
using UnityEngine;

public class Parkour2 : BaseState
{

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
        Rigidbody rb = bossManager.player.GetComponent<Rigidbody>();
        rb.position = bossManager.parkour2StartPos.position;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void StateUpdate() { }

    public override void ExitState()
    {
    }
}