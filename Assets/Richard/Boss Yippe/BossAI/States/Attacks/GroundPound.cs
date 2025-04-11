using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GroundPound : MonoBehaviour
{
    public float jumpHeight = 7f;
    public float slamSpeed = 40f;
    public GameObject shockwavePrefab;
    public Transform shockwaveSpawnPoint;
    public int attackCount;
    public float attackDelay = 1f;
    public float shockwaveSpeed = 12f;
    public float shockwaveLifetime = 1.4f;

    private Rigidbody rb;
    private NavMeshAgent navMeshAgent;
    private bool isJumping;
    private bool isSlamming = false;
    private bool hasLanded = false;
    private bool attackFinished = false;

    private BossAIManager boss;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        boss = GetComponent<BossAIManager>();
    }

    public IEnumerator GroundSlamSequence()
    {
        
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }
        boss.transform.position = new Vector3(-12.6899996f,52.1f,-177.639999f);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        FreezeXZConstraints(true);
        
        attackFinished = false;

        for (int i = 0; i < attackCount; i++)
        {
            hasLanded = false;
            yield return StartCoroutine(GroundSlam(i));
            yield return new WaitUntil(() => hasLanded);
            yield return new WaitForSeconds(attackDelay);
        }
        FreezeXZConstraints(false);
        attackFinished = true;
    }

    public IEnumerator GroundSlam(int slamIndex)
    {
        isJumping = true;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        yield return new WaitForSeconds(0.1f);
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        rb.velocity = Vector3.up * jumpHeight;
        yield return new WaitForSeconds(0.6f);
        isJumping = false;
        isSlamming = true;
        rb.velocity = Vector3.down * slamSpeed;
        yield return new WaitUntil(() => hasLanded);
        isSlamming = false;
        bool isCardinal = slamIndex % 2 == 0;
        yield return StartCoroutine(SpawnShockwaveProjectiles(isCardinal));
    }

    private IEnumerator SpawnShockwaveProjectiles(bool isCardinal)
    {
        if (shockwavePrefab == null) yield break;

        Vector3[] directions;
        float[] rotations;

        if (isCardinal)
        {
            directions = new Vector3[]
            {
                Vector3.forward,
                Vector3.back,
                Vector3.right,
                Vector3.left
            };
            rotations = new float[] { 0f, 180f, 90f, -90f };
        }
        else
        {
            directions = new Vector3[]
            {
                (Vector3.forward + Vector3.right).normalized,
                (Vector3.forward + Vector3.left).normalized,
                (Vector3.back + Vector3.right).normalized,
                (Vector3.back + Vector3.left).normalized
            };
            rotations = new float[] { 45f, -45f, 135f, -135f };
        }

        int projectilesSpawned = 0;
        foreach (Vector3 dir in directions)
        {
            GameObject shockwave = Instantiate(shockwavePrefab, shockwaveSpawnPoint.position, Quaternion.Euler(0, rotations[projectilesSpawned], 0));
            StartCoroutine(MoveShockwave(shockwave, dir));
            projectilesSpawned++;
        }
        yield return new WaitForSeconds(shockwaveLifetime);
    }

    private IEnumerator MoveShockwave(GameObject shockwave, Vector3 direction)
    {
        float timer = 0f;
        while (timer < shockwaveLifetime)
        {
            shockwave.transform.position += direction * (shockwaveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(shockwave);
    }

    private void FreezeXZConstraints(bool freeze)
    {
        if (freeze)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSlamming && other.gameObject.CompareTag("Ground"))
        {
            hasLanded = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    public bool IsAttackFinished()
    {
        return attackFinished;
    }
    
    
}
