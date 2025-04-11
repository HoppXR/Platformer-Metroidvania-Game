using System.Collections;
using System.Collections.Generic;
using Enemy;
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
    private EnemyDamage enemyDamage;
    
    [SerializeField] public CapsuleCollider playerDamageCollider;
    [SerializeField] public CapsuleCollider collisionDamageCollider;

    private BossAIState activeState;
    private float attackTimer = 3f;

    [SerializeField] private ParticleSystem Tired;
    
    private void Start()
    {
        bossStateManager = FindFirstObjectByType<BossStateManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerDamageCollider = GetComponent<CapsuleCollider>();
        dashAttack = GetComponent<DashAttack>();
        projectileVolley = GetComponent<ProjectileVolley>();
        groundPound = GetComponent<GroundPound>();
        enemyDamage = GetComponent<EnemyDamage>();

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
    public void PlayTiredEffect()
    {
        if (Tired != null)
            Tired.Play();
    }
    
    public void StopTiredEffect()
    {
        if (Tired != null && Tired.isPlaying)
            Tired.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}