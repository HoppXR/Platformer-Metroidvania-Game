using UnityEngine;
using System.Collections;

public class Attack : BossAIState
{
    private DashAttack dashAttack;
    private ProjectileVolley projectileVolley;
    private GroundPound groundPound;
    private Vector3 resetPosition = new Vector3(-2f, 2f, 2f);

    private static int attackIndex = 0;

    public Attack(BossAIManager boss) : base(boss) 
    {
        dashAttack = boss.GetComponent<DashAttack>();
        projectileVolley = boss.GetComponent<ProjectileVolley>();
        groundPound = boss.GetComponent<GroundPound>();
    }

    public override void EnterState()
    {
        Debug.Log("Boss is attacking!");
        boss.transform.position = resetPosition;
        Rigidbody rb = boss.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        AdjustAttackForPhase();

        if (boss.currentPhase == BossAIManager.BossPhase.Phase2)
        {
            boss.StartCoroutine(DelayedAttack());
        }
        else
        {
            boss.StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator DelayedAttack()
    {
        yield return new WaitForSeconds(2f);
        yield return boss.StartCoroutine(PerformAttack());
    }

    private void AdjustAttackForPhase()
    {
        if (boss.currentPhase == BossAIManager.BossPhase.Phase1)
        {
            boss.projectileVolley.totalSkyProjectiles = 14;
            boss.projectileVolley.totalTargetedProjectiles = 7;
            boss.projectileVolley.skyFireRate = 0.5f;
            boss.projectileVolley.targetedFireRate = 1f;
            boss.projectileVolley.projectileSpeed = 10;
            
            boss.dashAttack.dashSpeed = 15;
            
            boss.groundPound.attackCount = 3;
            boss.groundPound.attackDelay = 1.2f;
            
            
        }
        else
        {
            boss.projectileVolley.totalSkyProjectiles = 18;
            boss.projectileVolley.totalTargetedProjectiles = 10;
            boss.projectileVolley.skyFireRate = 0.3f;
            boss.projectileVolley.targetedFireRate = 0.8f;
            boss.projectileVolley.projectileSpeed = 15;
            boss.projectileVolley.predictionChance = 0.5f;
            
            boss.dashAttack.dashSpeed = 23;
            boss.dashAttack.dashDuration = 16;
            boss.dashAttack.maxBounces = 8;
            
            boss.groundPound.attackCount = 5;
            boss.groundPound.attackDelay = 1f;
            
        }
    }

    private IEnumerator PerformAttack()
    {
        switch (attackIndex)
        {
            case 0:
                if (boss.dashAttack != null) 
                    yield return boss.StartCoroutine(boss.dashAttack.DashRoutine(boss.player.position));
                break;
            case 1:
                if (boss.projectileVolley != null) 
                    yield return boss.StartCoroutine(boss.projectileVolley.FireVolleyRoutine());
                break;
            case 2:
                if (boss.groundPound != null) 
                    yield return boss.StartCoroutine(boss.groundPound.GroundSlamSequence());
                break;
        }
        attackIndex = (attackIndex + 1) % 3;
        boss.SetState(BossAIManager.BossState.Tired);
    }

    public override void StateUpdate() { }

    public override void ExitState() { }
}
