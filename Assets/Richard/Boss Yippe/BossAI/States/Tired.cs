using UnityEngine;

public class Tired : BossAIState
{
    private float timer = 6f;
    private Rigidbody rb;

    public Tired(BossAIManager boss) : base(boss) 
    {
        rb = boss.GetComponent<Rigidbody>();
    }

    public override void EnterState()
    {
        Debug.Log("Boss is tired...");
        
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public override void StateUpdate()
    {
        timer -= Time.unscaledDeltaTime;
        if (timer <= 0)
        {
            boss.SetState(BossAIManager.BossState.Idle);
        }
    }

    public override void ExitState()
    {
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }
}