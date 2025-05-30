using UnityEngine;
using UnityEngine.AI;

public class BossStateManager : MonoBehaviour
{
    public enum BossState { Parkour1, Fight1, Transition, Parkour2, Fight2, End }
    public BossState currentState = BossState.Parkour1;
    public GameObject bossHealthBar;
    public BossAIManager bossAI;
    public GameObject player;
    public Cinemachine.CinemachineFreeLook playerCinemachineCamera;
    public Cinemachine.CinemachineFreeLook transitionCinemachineCamera;
    public GameObject arenaColliders;
    public GameObject parkour1;
    public GameObject parkour2;
    public Transform parkour2StartPos;
    public GameObject parkour2Trigger;

    private BaseState activeState;

    private void Start()
    {
        SetState(currentState);
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
            case BossState.Parkour1:
                activeState = new Parkour1(this);
                break;
            case BossState.Fight1:
                activeState = new Fight1(this);
                break;
            case BossState.Transition:
                activeState = new Transition(this);
                break;
            case BossState.Parkour2:
                activeState = new Parkour2(this);
                break;
            case BossState.Fight2:
                activeState = new Fight2(this);
                break;
            case BossState.End:
                activeState = new End(this);
                break;
        }

        activeState.EnterState();
    }
}