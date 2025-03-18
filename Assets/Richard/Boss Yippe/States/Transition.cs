using System.Collections;
using UnityEngine;

public class Transition : BaseState
{
    private float timer = 0f;

    public Transition(BossStateManager bossManager) : base(bossManager) { }

    public override void EnterState()
    {
        Debug.Log("Entered Transition");
        if (bossManager.bossAI != null)
        {
            bossManager.bossAI.SetState(BossAIManager.BossState.Idle);
        }
        
        bossManager.bossAI.transform.position = new Vector3(-12.6899996f, 52f, -177.639999f);
        
        var freeLookCam = bossManager.cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>();
        freeLookCam.m_XAxis.Value = 0f;
        freeLookCam.m_YAxis.Value = 0f;
        if (bossManager.bossAI != null)
        {
            var navAgent = bossManager.bossAI.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navAgent != null) navAgent.enabled = false;

            bossManager.bossAI.enabled = false;
            bossManager.bossAI.GetComponent<ProjectileVolley>().enabled = false;
            bossManager.bossAI.GetComponent<DashAttack>().enabled = false;
            bossManager.bossAI.GetComponent<GroundPound>().enabled = false;
        }
        if (bossManager.transitionCamera != null && bossManager.playercameraParent != null)
        {
            bossManager.playercameraParent.SetActive(false);
            bossManager.transitionCamera.SetActive(true);
        }
        if (bossManager.player != null)
        {
            Rigidbody rb = bossManager.player.GetComponent<Rigidbody>();
            rb.position = bossManager.parkour2StartPos.position;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        if (bossManager.arenaColliders != null) bossManager.arenaColliders.SetActive(false);
        if (bossManager.parkour1 != null) bossManager.parkour1.SetActive(false);
        if (bossManager.parkour2 != null) bossManager.parkour2.SetActive(true);
    }

    public override void StateUpdate()
    {
        timer += Time.unscaledDeltaTime; 

        if (timer > 3f) 
        {
            if (bossManager.transitionCamera != null && bossManager.playercameraParent != null)
            {
                bossManager.transitionCamera.SetActive(false);
                bossManager.playercameraParent.SetActive(true);
            }
            bossManager.SetState(BossStateManager.BossState.Parkour2);
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Transition");
    }
}
