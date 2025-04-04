using UnityEngine;
using UnityEngine.AI;

public class Tired : BossAIState
{
    private float timer = 6f;
    private Rigidbody rb;
    private Quaternion originalRotation;

    public Tired(BossAIManager boss) : base(boss) 
    {
        rb = boss.GetComponent<Rigidbody>();
        boss.PlayTiredEffect();
        originalRotation = boss.transform.rotation; 
    }

    public override void EnterState()
    {
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        
        boss.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
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
        boss.transform.rotation = originalRotation;

        boss.StopTiredEffect();
    }
}