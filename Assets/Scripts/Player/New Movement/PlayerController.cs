using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader input;
    [SerializeField] private Transform orientation;
    private Vector3 velocity;
    private Vector2 _inputDir;
    private Vector2 _movementDir;
    
    [Header("Movement")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxAcceleration;

    private void Start()
    {
        input.MoveEvent += GetInputDirection;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 desiredVelocity = new Vector3(_movementDir.x, 0f, _movementDir.y) * maxSpeed;
        
        float maxSpeedChange = maxAcceleration * Time.deltaTime;

        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        Vector3 displacement = velocity * Time.deltaTime;
        transform.localPosition += displacement;
    }

    private void GetInputDirection(Vector2 dir)
    {
        _inputDir = dir;

        // taking camera orientation into account
        _movementDir = orientation.forward * _inputDir.y + orientation.right * _inputDir.x;
        _movementDir = Vector2.ClampMagnitude(_movementDir, 1f); // same as normalize function
    }
}
