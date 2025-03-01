using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GroundPound : MonoBehaviour
{
    public float jumpHeight = 7f;
    public float slamSpeed = 40f;
    public GameObject shockwavePrefab;
    public Transform shockwaveSpawnPoint;
    public int attackCount = 4;
    public float attackDelay = 1f;

    private Rigidbody rb;
    private NavMeshAgent navMeshAgent;
    private bool isJumping = false;
    private bool isSlamming = false;
    private bool hasLanded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
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

        if (shockwavePrefab != null)
        {
            Vector3 shockwavePosition = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
            GameObject shockwave = Instantiate(shockwavePrefab, shockwavePosition, Quaternion.identity);

            StartCoroutine(ExpandAndDestroy(shockwave));
        }
    }

    IEnumerator ExpandAndDestroy(GameObject obj)
    {
        float expandTime = 6f; 
        float maxSize = 11f;
        float descendSpeed = 0.95f;

        float timer = 0f;
        Vector3 startScale = obj.transform.localScale;
        Vector3 startPosition = obj.transform.position;

        while (timer < expandTime)
        {
            timer += Time.deltaTime;
            float progress = timer / expandTime;
            obj.transform.localScale = Vector3.Lerp(startScale, new Vector3(maxSize, maxSize, maxSize), progress);
            obj.transform.position -= Vector3.up * (descendSpeed * Time.deltaTime);

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
