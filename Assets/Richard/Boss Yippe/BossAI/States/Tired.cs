using UnityEngine;

public class Tired : BossAIState
{
    private float timer = 6f;

    public Tired(BossAIManager boss) : base(boss) { }

    public override void EnterState()
    {
        Debug.Log("Boss is tired...");
    }

    public override void StateUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            boss.SetState(BossAIManager.BossState.Idle);
        }
    }

    public override void ExitState() { }
}