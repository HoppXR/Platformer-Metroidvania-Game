using UnityEngine;

public class Tired : BossAIState
{
    private float timer = 3f;

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
            if (boss.currentPhase == BossAIManager.BossPhase.Phase1)
                boss.SetState(BossAIManager.BossState.Idle);
            else
                boss.SetState(BossAIManager.BossState.Chase);
        }
    }

    public override void ExitState() { }
}