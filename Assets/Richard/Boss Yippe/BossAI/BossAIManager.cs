using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAIManager : MonoBehaviour
{
    public enum BossPhase { Phase1, Phase2 }
    public enum BossState { Idle, Attack, Tired, Chase, Lunge, Death }

    public BossPhase currentPhase = BossPhase.Phase1;
    public BossState currentState = BossState.Idle;

    private BossAIState activeState;
    private float attackTimer = 3f;
    
    private void Start()
    {
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
                activeState = new Idle(this);
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

        activeState.EnterState();
    }

    public void TransitionToPhase2()
    {
        currentPhase = BossPhase.Phase2;
        SetState(BossState.Chase);
    }
}