using System.Collections;
using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent

public class GroundPound : MonoBehaviour
{
    public float jumpHeight = 7f;      // How high the boss jumps
    public float slamSpeed = 40f;       // Speed of the slam down
    public GameObject shockwavePrefab;  // The expanding sphere attack prefab
    public Transform shockwaveSpawnPoint; // Where the sphere appears on landing
    public int attackCount = 4;         // Number of times the attack should repeat
    public float attackDelay = 1f;      // Delay between each attack

    private Rigidbody rb;
    private NavMeshAgent navMeshAgent;
    private bool isJumping = false;
    private bool isSlamming = false;
    private bool hasLanded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>(); // Get NavMeshAgentA
    }

    public IEnumerator GroundSlamSequence()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false; 
        }

        FreezeXZConstraints(true); 

        for (int i = 0; i < attackCount; i++)
        {
            hasLanded = false;
            yield return StartCoroutine(GroundSlam());
            yield return new WaitUntil(() => hasLanded); 
            yield return new WaitForSeconds(attackDelay); 
        }

        FreezeXZConstraints(false);

        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
        }
    }

    public IEnumerator GroundSlam()
    {
        isJumping = true;


        rb.velocity = Vector3.up * jumpHeight;
        yield return new WaitForSeconds(0.6f); 

        isJumping = false;
        isSlamming = true;


        rb.velocity = Vector3.down * slamSpeed;

        yield return new WaitUntil(() => hasLanded);

        isSlamming = false;
        
        if (shockwavePrefab != null && shockwaveSpawnPoint != null)
        {
            GameObject shockwave = Instantiate(shockwavePrefab, shockwaveSpawnPoint.position, Quaternion.identity);
            StartCoroutine(ExpandAndDestroy(shockwave));
        }
    }

    IEnumerator ExpandAndDestroy(GameObject obj)
    {
        float expandTime = 5f;
        float maxSize = 10f;

        float timer = 0f;
        Vector3 startScale = Vector3.one;

        while (timer < expandTime)
        {
            timer += Time.deltaTime;
            obj.transform.localScale = Vector3.Lerp(startScale, Vector3.one * maxSize, timer / expandTime);
            yield return null;
        }

        Destroy(obj); 
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
        }
    }
}