using UnityEngine;

public class Attack : BossAIState
{
    public Attack(BossAIManager boss) : base(boss) { }

    public override void EnterState()
    {
        Debug.Log("Boss is attacking!");
        PerformAttack();
    }

    private void PerformAttack()
    {
        int attackType = Random.Range(0, 3);

        if (boss.currentPhase == BossAIManager.BossPhase.Phase1)
        {
            switch (attackType)
            {
                case 0:
                    SDashAttack();
                    break;
                case 1:
                    SRainProjectiles();
                    break;
                case 2:
                    SGroundPound();
                    break;
            }
        }
        else
        {
            switch (attackType)
            {
                case 0:
                    FDashAttack();
                    break;
                case 1:
                    FRainProjectiles();
                    break;
                case 2:
                    FGroundPound();
                    break;
            }
        }
    }

    private void SDashAttack()
    {
        Vector3 targetPosition = boss.player.transform.position;
        boss.GetComponent<DashAttack>().StartDash(targetPosition);
    }

    private void SRainProjectiles()
    {

    }

    private void SGroundPound()
    {

    }
    
    private void FDashAttack()
    {

    }

    private void FRainProjectiles()
    {

    }

    private void FGroundPound()
    {

    }

    public override void StateUpdate()
    {
        boss.SetState(BossAIManager.BossState.Tired);
    }

    public override void ExitState() { }
}