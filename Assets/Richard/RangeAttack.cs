using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedAttack : StateMachineBehaviour
{
    private NavMeshAgent AI;

    public Transform Target;

    [SerializeField]
    private Rigidbody AttackProjectile;
    [Range(0, 1)]
    public float ForceRatio = 0;
    [SerializeField]
    private float MaxThrowForce = 25;

    [SerializeField]
    private LayerMask SightLayers;
    [SerializeField]
    private float AttackDelay = 5f;

    [SerializeField]
    private bool UseMovementPrediction;
    public PredictionMode MovementPredictionMode;
    [Range(0.01f, 5f)]
    public float HistoricalTime = 1f;
    [Range(1, 100)]
    public int HistoricalResolution = 10;
    private Queue<Vector3> HistoricalPositions;

    private float HistoricalPositionInterval;
    private float LastHistoryRecordedTime;

    private CharacterController PlayerCharacterController;
    private float SpherecastRadius = 0.5f;
    private float LastAttackTime;

    // Add a flag to track if the attack has been performed
    private bool hasFiredProjectile;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI = animator.GetComponent<NavMeshAgent>();
        AI.speed = 0f;
        Target = GameObject.FindGameObjectWithTag("Player").transform;

        AttackProjectile.useGravity = false;
        AttackProjectile.isKinematic = true;
        SpherecastRadius = AttackProjectile.GetComponent<SphereCollider>().radius;
        LastAttackTime = Random.Range(0, 5);

        int capacity = Mathf.CeilToInt(HistoricalResolution * HistoricalTime);
        HistoricalPositions = new Queue<Vector3>(capacity);
        for (int i = 0; i < capacity; i++)
        {
            HistoricalPositions.Enqueue(Target.position);
        }
        HistoricalPositionInterval = HistoricalTime / HistoricalResolution;

        // Reset the attack state
        hasFiredProjectile = false;

        animator.SetBool("isAttacking", true);
        animator.SetBool("hasAttacked", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Physics.SphereCast(
                AI.transform.position,
                SpherecastRadius,
                (Target.transform.position + Vector3.up - AI.transform.position).normalized,
                out RaycastHit hit,
                float.MaxValue,
                SightLayers)
            && hit.transform == Target && !hasFiredProjectile)  // Check if attack has already happened
        {
            Debug.Log("Target hit! Initiating attack...");
            LastAttackTime = Time.time;
            AttackProjectile.transform.localPosition = new Vector3(0, 0, 1f);
            AttackProjectile.useGravity = false;
            AttackProjectile.velocity = Vector3.zero;

            Attack();  // Fire the projectile

            // Set the flag to true so the attack won't happen again in this state
            hasFiredProjectile = true;
        }
        else
        {
            Debug.Log("No valid target hit or attack already performed.");
        }

        if (LastHistoryRecordedTime + HistoricalPositionInterval < Time.time)
        {
            LastHistoryRecordedTime = Time.time;
            HistoricalPositions.Dequeue();
            HistoricalPositions.Enqueue(Target.position);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isAttacking", false);
        animator.SetBool("hasAttacked", false);
        hasFiredProjectile = false;  // Reset for the next time the state is entered
    }

    public void Attack()
    {
        if (PlayerCharacterController == null)
        {
            PlayerCharacterController = Target.GetComponent<CharacterController>();
        }

        // Instantiate the projectile at the AI's position
        Rigidbody projectileInstance = Instantiate(
            AttackProjectile, 
            AI.transform.position + new Vector3(0, 0.5f, 0), // Adjusted spawn position
            Quaternion.identity
        );

        // Decide whether to use movement prediction or direct targeting
        float randomValue = Random.value; // Get a random value between 0 and 1
        ThrowData throwData;

        if (randomValue < 0.6f) // 60% chance for direct targeting
        {
            throwData = CalculateThrowData(Target.position, projectileInstance.position);
        }
        else // 40% chance for movement prediction
        {
            throwData = CalculateThrowData(
                Target.position + PlayerCharacterController.center,
                projectileInstance.position
            );
            throwData = GetPredictedPositionThrowData(throwData);
        }

        // Apply the calculated throw force to the instantiated projectile
        DoThrow(projectileInstance, throwData);

        // Exit attack state after throwing
        Animator animator = AI.GetComponent<Animator>();
        animator.SetBool("isAttacking", false);
        animator.SetBool("hasAttacked", true);
    }
    
    private ThrowData GetPredictedPositionThrowData(ThrowData DirectThrowData)
    {
        Vector3 throwVelocity = DirectThrowData.ThrowVelocity;
        throwVelocity.y = 0;
        float time = DirectThrowData.DeltaXZ / throwVelocity.magnitude;
        Vector3 playerMovement;

        if (MovementPredictionMode == PredictionMode.CurrentVelocity)
        {
            playerMovement = PlayerCharacterController.velocity * time;
        }
        else
        {
            Vector3[] positions = HistoricalPositions.ToArray();
            Vector3 averageVelocity = Vector3.zero;
            for (int i = 1; i < positions.Length; i++)
            {
                averageVelocity += (positions[i] - positions[i - 1]) / HistoricalPositionInterval;
            }
            averageVelocity /= HistoricalTime * HistoricalResolution;
            playerMovement = averageVelocity;
        }

        Vector3 newTargetPosition = new Vector3(
            Target.position.x + PlayerCharacterController.center.x + playerMovement.x,
            Target.position.y + PlayerCharacterController.center.y + playerMovement.y,
            Target.position.z + PlayerCharacterController.center.z + playerMovement.z
        );

        // Recalculate the trajectory based on the new target position
        ThrowData predictiveThrowData = CalculateThrowData(
            newTargetPosition, 
            AttackProjectile.position
        );

        predictiveThrowData.ThrowVelocity = Vector3.ClampMagnitude(
            predictiveThrowData.ThrowVelocity, 
            MaxThrowForce
        );

        return predictiveThrowData;
    }

    private void DoThrow(Rigidbody projectile, ThrowData throwData)
    {
        Debug.Log("Throwing the projectile!");
        projectile.useGravity = true;
        projectile.isKinematic = false;
        projectile.velocity = throwData.ThrowVelocity;

        // Check if velocity is correctly set
        Debug.Log("Projectile velocity: " + throwData.ThrowVelocity);
    }

    private ThrowData CalculateThrowData(Vector3 TargetPosition, Vector3 StartPosition)
    {
        Vector3 displacement = new Vector3(
            TargetPosition.x,
            StartPosition.y,
            TargetPosition.z
        ) - StartPosition;
        float deltaY = TargetPosition.y - StartPosition.y;
        float deltaXZ = displacement.magnitude;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float throwStrength = Mathf.Clamp(
            Mathf.Sqrt(
                gravity
                * (deltaY + Mathf.Sqrt(Mathf.Pow(deltaY, 2)
                + Mathf.Pow(deltaXZ, 2)))),
            0.01f,
            MaxThrowForce
        );
        throwStrength = Mathf.Lerp(throwStrength, MaxThrowForce, ForceRatio);

        float angle;
        if (ForceRatio == 0)
        {
            angle = Mathf.PI / 2f - (0.5f * (Mathf.PI / 2 - (deltaY / deltaXZ)));
        }
        else
        {
            angle = Mathf.Atan(
                (Mathf.Pow(throwStrength, 2) - Mathf.Sqrt(
                    Mathf.Pow(throwStrength, 4) - gravity
                    * (gravity * Mathf.Pow(deltaXZ, 2)
                    + 2 * deltaY * Mathf.Pow(throwStrength, 2)))
                ) / (gravity * deltaXZ)
            );
        }

        if (float.IsNaN(angle))
        {
            angle = 45f; // Default to a 45-degree angle if calculations fail
        }

        Vector3 initialVelocity =
            Mathf.Cos(angle) * throwStrength * displacement.normalized
            + Mathf.Sin(angle) * throwStrength * Vector3.up;

        return new ThrowData
        {
            ThrowVelocity = initialVelocity,
            Angle = angle,
            DeltaXZ = deltaXZ,
            DeltaY = deltaY
        };
    }

    private struct ThrowData
    {
        public Vector3 ThrowVelocity;
        public float Angle;
        public float DeltaXZ;
        public float DeltaY;
    }

    public enum PredictionMode
    {
        CurrentVelocity,
        AverageVelocity
    }
}