using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAIManager : MonoBehaviour
{
    public enum BossPhase { Phase1, Phase2 }
    public enum BossState { Idle, Attack, Tired, Chase, Lunge, Death }

    public BossPhase currentPhase = BossPhase.Phase1;
    public BossState currentState = BossState.Idle;
    
    public Transform player;
    
    public DashAttack dashAttack;
    public ProjectileVolley projectileVolley;
    public GroundPound groundPound;
    private BossStateManager bossStateManager;

    private BossAIState activeState;
    private float attackTimer = 3f;
    
    private void Start()
    {
        bossStateManager = FindObjectOfType<BossStateManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        dashAttack = GetComponent<DashAttack>();
        projectileVolley = GetComponent<ProjectileVolley>();
        groundPound = GetComponent<GroundPound>();

        SetState(BossState.Idle);
    }

    private void Update()
    {
        if (activeState != null)
            activeState.StateUpdate();
    }

    public void SetState(BossState newState)
    {
        if (activeState != null)
            activeState.ExitState();

        currentState = newState;

        switch (newState)
        {
            case BossState.Idle:
                activeState = new Idle(this, bossStateManager);
                break;
            case BossState.Attack:
                activeState = new Attack(this);
                break;
            case BossState.Tired:
                activeState = new Tired(this);
                break;
            case BossState.Chase:
                activeState = new Chase(this);
                break;
            case BossState.Lunge:
                activeState = new Lunge(this);
                break;
            case BossState.Death:
                activeState = new Death(this);
                break;
        }

        if (activeState != null) activeState.EnterState();
    }
}