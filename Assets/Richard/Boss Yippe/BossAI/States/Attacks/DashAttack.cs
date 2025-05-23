using System.Collections;
using UnityEngine;

public class DashAttack : MonoBehaviour
{
    public float dashSpeed = 20f;
    public float dashDuration = 11f;
    public int maxBounces = 5;

    private Rigidbody rb;
    private Vector3 dashDirection;
    private int bounceCount = 0;
    private float dashTimer = 0f;
    private bool isDashing = false;

    private BossAIManager boss;

    private BossLineToPlayer bossLineToPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boss = GetComponent<BossAIManager>();
        bossLineToPlayer = GetComponentInChildren<BossLineToPlayer>();
        if (boss == null)
        {
            boss = FindObjectOfType<BossAIManager>(); 
        }
        StartCoroutine(bossLineToPlayer.ShowLineRenderer());
    }

    void Update()
    {
        if (rb.freezeRotation != true)
        {
            rb.freezeRotation = true;
        }
    
        if (isDashing)
        {
            dashTimer -= Time.unscaledDeltaTime;
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

            if (dashTimer <= 0 || bounceCount >= maxBounces)
            {
                StopDash();
            }
        }
    }
    public void StartDash(Vector3 targetPosition)
    {
        
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        
        Vector3 toTarget = new Vector3(targetPosition.x - transform.position.x, 0, targetPosition.z - transform.position.z);
        
        if (toTarget.magnitude < 1f) 
        {
            toTarget = toTarget.normalized * 1f;
        }

        dashDirection = toTarget.normalized;

        isDashing = true;
        dashTimer = dashDuration;
        bounceCount = 0;

        rb.velocity = dashDirection * dashSpeed;

        RotateTowards(dashDirection);
    }

    public void StopDash()
    {
        isDashing = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isDashing)
        {
            if (other.CompareTag("Wall")) 
            {
                Vector3 normal = other.ClosestPoint(transform.position) - transform.position;
                normal = normal.normalized;
                dashDirection = Vector3.Reflect(dashDirection, normal).normalized;
            
                if (boss != null && boss.player != null)
                {
                    Vector3 toPlayer = (boss.player.position - transform.position).normalized;
                    dashDirection = Vector3.Lerp(dashDirection, toPlayer, 0.75f).normalized;
                }

                rb.velocity = dashDirection * dashSpeed;
                RotateTowards(dashDirection);

                bounceCount++;
            }
        }
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
    }

    public IEnumerator DashRoutine(Vector3 targetPosition)
    {
        StartDash(targetPosition);
        yield return new WaitUntil(() => !isDashing); 
    }
}
