using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;          // Normal movement speed
    public float dashSpeed = 20f;        // Speed during a dash
    public float dashDuration = 0.2f;    // Duration of the dash
    public float dashCooldown = 1f;      // Cooldown time between dashes

    private Rigidbody rb;
    private Vector3 moveDirection;

    public bool isDashing = false;
    private float dashEndTime = 0f;
    private float lastDashTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Capture input for WASD or arrow keys
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Calculate the movement direction
        moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        // Handle dashing
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
        {
            StartDash();
        }

        // End dash if the dash duration has elapsed
        if (isDashing && Time.time >= dashEndTime)
        {
            EndDash();
        }
    }

    void FixedUpdate()
    {
        // Apply movement force
        float currentSpeed = isDashing ? dashSpeed : moveSpeed;
        rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);
    }

    private void StartDash()
    {
        isDashing = true;
        dashEndTime = Time.time + dashDuration;
        lastDashTime = Time.time;

        // Optionally, you can add a visual or audio cue for the dash here.
        Debug.Log("Dash started!");
    }

    private void EndDash()
    {
        isDashing = false;

        // Optionally, you can add a visual or audio cue for dash ending here.
        Debug.Log("Dash ended!");
    }
}