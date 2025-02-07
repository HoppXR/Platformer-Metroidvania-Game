using UnityEngine;

public class Death : BossAIState
{
    public Death(BossAIManager boss) : base(boss) { }

    public override void EnterState()
    {
        Debug.Log("Boss is dead.");
        GameObject.Destroy(boss.gameObject);
    }

    public override void StateUpdate() { }
    public override void ExitState() { }
}