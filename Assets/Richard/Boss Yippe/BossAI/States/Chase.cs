using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : BossAIState
{
    private Transform player;
    private float speed = 2f;

    public Chase(BossAIManager boss) : base(boss) { }

    public override void EnterState()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    public override void StateUpdate()
    {
        if (player != null)
            boss.transform.position = Vector3.MoveTowards(boss.transform.position, player.position, speed * Time.deltaTime);

        float distance = Vector3.Distance(boss.transform.position, player.position);
        if (distance < 3f)
            boss.SetState(BossAIManager.BossState.Lunge);
    }

    public override void ExitState() { }
}
