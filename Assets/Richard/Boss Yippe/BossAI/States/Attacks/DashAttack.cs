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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        boss = GetComponent<BossAIManager>();
        if (boss == null)
        {
            boss = FindObjectOfType<BossAIManager>(); 
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.I) && boss != null && boss.player != null)
        {
            StartDash(boss.player.position); 
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0 || bounceCount >= maxBounces)
            {
                StopDash();
            }
        }
    }

    public void StartDash(Vector3 targetPosition)
    {
        dashDirection = (targetPosition - transform.position).normalized;
        isDashing = true;
        dashTimer = dashDuration;
        bounceCount = 0;
        rb.velocity = dashDirection * dashSpeed;
        
        RotateTowards(dashDirection);
    }

    private void StopDash()
    {
        isDashing = false;
        rb.velocity = Vector3.zero;
        Debug.Log("Dash ended");
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
                rb.velocity = dashDirection * dashSpeed;
                
                RotateTowards(dashDirection);

                bounceCount++;
                Debug.Log("Bounced! Count: " + bounceCount);
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
}
