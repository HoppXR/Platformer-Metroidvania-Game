using UnityEngine;

public class Lunge : BossAIState
{
    private Transform player;
    private bool hasLunged = false;

    public Lunge(BossAIManager boss) : base(boss) { }

    public override void EnterState()
    {
        player = GameObject.FindWithTag("Player").transform;
        hasLunged = false;
    }

    public override void StateUpdate()
    {
        if (!hasLunged && player != null)
        {
            boss.transform.position = Vector3.Lerp(boss.transform.position, player.position, 0.5f);
            hasLunged = true;
        }
        else
        {
            boss.SetState(BossAIManager.BossState.Tired);
        }
    }

    public override void ExitState() { }
}