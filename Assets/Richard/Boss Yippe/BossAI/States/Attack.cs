using UnityEngine;

public class Attack : BossAIState
{
    private DashAttack dashAttack;
    private ProjectileVolley projectileVolley;
    private GroundPound groundPound;

    public Attack(BossAIManager boss) : base(boss) 
    {
        dashAttack = boss.GetComponent<DashAttack>();
        projectileVolley = boss.GetComponent<ProjectileVolley>();
        groundPound = boss.GetComponent<GroundPound>();
    }

    public override void EnterState()
    {
        Debug.Log("Boss is attacking!");
        AdjustAttackForPhase();
        PerformAttack();
    }

    private void AdjustAttackForPhase()
    {
        if (boss.currentPhase == BossAIManager.BossPhase.Phase1)
        {
            projectileVolley.totalSkyProjectiles = 15;
            dashAttack.dashSpeed = 15;
            groundPound.jumpHeight = 10;
            
        }
        else
        {
            //projectileVolley.

        }
    }

    private void PerformAttack()
    {
        int attackType = Random.Range(0, 3);

        if (boss.currentPhase == BossAIManager.BossPhase.Phase1)
        {
            switch (attackType)
            {
                case 0:
                    dashAttack.StartDash(boss.player.position);
                    break;
                case 1:
                    projectileVolley.StartCoroutine("FireVolley");
                    break;
                case 2:
                    groundPound.StartCoroutine("GroundSlamSequence");
                    break;
            }
        }
        else
        {
            switch (attackType)
            {
                case 0:
                    dashAttack.StartDash(boss.player.position);
                    break;
                case 1:
                    projectileVolley.StartCoroutine("FireVolley");
                    break;
                case 2:
                    groundPound.StartCoroutine("GroundSlamSequence");
                    break;
            }
        }
    }

    public override void StateUpdate()
    {
        boss.SetState(BossAIManager.BossState.Tired);
    }

    public override void ExitState() { }
}
