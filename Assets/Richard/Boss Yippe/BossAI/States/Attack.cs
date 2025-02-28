using UnityEngine;
using System.Collections;

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
        boss.StartCoroutine(PerformAttack());
    }

    private void AdjustAttackForPhase()
    {
        if (boss.currentPhase == BossAIManager.BossPhase.Phase1)
        {
            //projectileVolley.totalSkyProjectiles = 15;
            //dashAttack.dashSpeed = 15;
            //groundPound.jumpHeight = 10;
            
        }
        else
        {
            //projectileVolley.

        }
    }

    private IEnumerator PerformAttack()
    {
        int attackType = Random.Range(0, 3);

        switch (attackType)
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
        
        boss.SetState(BossAIManager.BossState.Tired);
    }

    public override void StateUpdate()
    {
    }

    public override void ExitState() { }
}
